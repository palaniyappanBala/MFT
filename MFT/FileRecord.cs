using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MFT.Attributes;
using NLog;
using Attribute = MFT.Attributes.Attribute;

namespace MFT
{
    public class FileRecord
    {
        [Flags]
        public enum EntryFlag
        {
            FileRecordSegmentInUse = 0x1,
            FileNameIndexPresent = 0x2,
            Unknown0 = 0x4,
            Unknown1 = 0x8
        }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly int _baadSig = 0x44414142;
        private static readonly int _fileSig = 0x454c4946;

        public FileRecord(byte[] rawBytes, int offset)
        {
            Offset = offset;
            var sig = BitConverter.ToInt32(rawBytes, 0);

            if ((sig != _fileSig) && (sig != _baadSig) && (sig != 0x0))
            {
                Logger.Fatal($"Invalid signature! 0x{sig:X}");
                return;
                //throw new Exception("Invalid signature!");
            }

            if (sig == _baadSig)
            {
                Logger.Warn($"Bad signature at offset 0x{offset:X}");
                return;
            }

            Attributes = new List<Attribute>();

            FixupOffset = BitConverter.ToInt16(rawBytes, 2);
            FixupEntryCount = BitConverter.ToInt16(rawBytes, 4);

            LogSequenceNumber = BitConverter.ToInt64(rawBytes, 0x8);

            SequenceNumber = BitConverter.ToInt16(rawBytes, 0x10);

            ReferenceCount = BitConverter.ToInt16(rawBytes, 0x12);

            FirstAttributeOffset = BitConverter.ToInt16(rawBytes, 0x14);

            EntryFlags = (EntryFlag) BitConverter.ToInt16(rawBytes, 0x16);

            Logger.Trace($"Entry flags: {EntryFlags}");

            ActualRecordSize = BitConverter.ToInt32(rawBytes, 0x18);

            AllocatedRecordSize = BitConverter.ToInt32(rawBytes, 0x1c);

            var entryBytes = new byte[8];

            Buffer.BlockCopy(rawBytes, 0x20, entryBytes, 0, 8);

            MFTRecordToBaseRecord = new MftEntryInfo(entryBytes);

            FirstAvailableAttribueId = BitConverter.ToInt16(rawBytes, 0x28);

            EntryNumber = BitConverter.ToInt32(rawBytes, 0x2c);

            var fixupExpectedBytes = new byte[2];
            var fixupActual1 = new byte[2];
            var fixupActual2 = new byte[2];

            Buffer.BlockCopy(rawBytes, 0x30, fixupExpectedBytes, 0, 2);
            Buffer.BlockCopy(rawBytes, 0x32, fixupActual1, 0, 2);
            Buffer.BlockCopy(rawBytes, 0x34, fixupActual2, 0, 2);

            //verify this record looks ok based on fixup bytes
            //0x1FE and 0x3fe

            var expectedFixupVal = BitConverter.ToInt16(fixupExpectedBytes, 0);
            var x1FeValue = BitConverter.ToInt16(rawBytes, 0x1FE);
            var x3FeValue = BitConverter.ToInt16(rawBytes, 0x3FE);

            if ((x1FeValue != expectedFixupVal) &&
                ((EntryFlags & EntryFlag.FileRecordSegmentInUse) == EntryFlag.FileRecordSegmentInUse))
            {
                Logger.Warn(
                    $"FILE record at offset 0x{offset:X}! Fixup values do not match at 0x1FE. Expected: {expectedFixupVal}, actual: {x1FeValue}, EntryFlags: {EntryFlags}");
            }

            if ((x3FeValue != expectedFixupVal) &&
                ((EntryFlags & EntryFlag.FileRecordSegmentInUse) == EntryFlag.FileRecordSegmentInUse))
            {
                Logger.Warn(
                    $"FILE record at offset 0x{offset:X}! Fixup values do not match at 0x3FE. Expected: {expectedFixupVal}, actual: {x3FeValue}, EntryFlags: {EntryFlags}");
            }

            //header is done, replace fixup bytes with actual bytes
            //0x1fe and 0x3fe should contain fixup bytes 

            Buffer.BlockCopy(fixupActual1, 0, rawBytes, 0x1fe, 2);
            Buffer.BlockCopy(fixupActual2, 0, rawBytes, 0x3fe, 2);

            //start attribute processing at FirstAttributeOffset

            var index = (int) FirstAttributeOffset;

            while (index < ActualRecordSize)
            {
                var attrType = BitConverter.ToInt32(rawBytes, index);

                var attrSize = BitConverter.ToInt32(rawBytes, index + 4);

//                Logger.Trace(
//                    $"ActualRecordSize: {ActualRecordSize} attrType: 0x{attrType:X}, size: {attrSize}, index: {index}, offset: 0x{offset:x}, i+o: 0x{index + offset:X}");

                if ((attrSize == 0) || (attrType == -1))
                {
                    index += 8; //skip -1 type and 0 size

                    if (EntryFlags == 0) //this is a free record
                    {
                        break;
                    }

                    continue;
                }

                var rawAttr = new byte[attrSize];
                Buffer.BlockCopy(rawBytes, index, rawAttr, 0, attrSize);

                switch ((AttributeType) attrType)
                {
                    case AttributeType.StandardInformation:
                        var si = new StandardInfo(rawAttr);
                        Attributes.Add(si);

                        SILastAccessedOn = si.LastAccessedOn;
                        SICreatedOn = si.CreatedOn;
                        SIRecordModifiedOn = si.RecordModifiedOn;
                        SIContentModifiedOn = si.ContentModifiedOn;

                        break;
                    case AttributeType.FileName:
                        var fi = new FileName(rawAttr);
                        Attributes.Add(fi);

                        if ((fi.FileInfo.NameType & NameTypes.Windows) == NameTypes.Windows)
                        {
                            FName = fi.FileInfo.FileName;
                        }

                        //if (fi.FileInfo.LastAccessedOn.UtcDateTime != SILastAccessedOn.UtcDateTime)
                        //{
                        FNLastAccessedOn = fi.FileInfo.LastAccessedOn;
                        //}

                        //if (fi.FileInfo.CreatedOn.UtcDateTime != SICreatedOn.UtcDateTime)
                        //{
                        FNCreatedOn = fi.FileInfo.CreatedOn;
                        //}

                        //if (fi.FileInfo.RecordModifiedOn.UtcDateTime != SIRecordModifiedOn.UtcDateTime)
                        //{
                        FNRecordModifiedOn = fi.FileInfo.RecordModifiedOn;
                        //}


                        //if (fi.FileInfo.ContentModifiedOn.UtcDateTime != SIContentModifiedOn.UtcDateTime)
                        //{
                        FNContentModifiedOn = fi.FileInfo.ContentModifiedOn;
                        //}


                        break;
                    case AttributeType.Data:
                        var data = new Data(rawAttr);                       
                        Attributes.Add(data);
                        break;
                    case AttributeType.IndexAllocation:
                        var ia = new IndexAllocation(rawAttr);
                        Attributes.Add(ia);
                        break;
                    case AttributeType.IndexRoot:
                        var ir = new IndexRoot(rawAttr);
                        Attributes.Add(ir);
                        break;

                    case AttributeType.Bitmap:
                       var bm = new Bitmap(rawAttr);
                        Attributes.Add(bm);
                        break;

                    case AttributeType.VolumeVersionObjectId:
                        var oi = new ObjectId(rawAttr);
                        Attributes.Add(oi);
                        break;
                    case AttributeType.SecurityDescriptor:
                        var sd = new SecurityDescriptor(rawAttr);
                        Attributes.Add(sd);

                        break;
                    case AttributeType.VolumeName:
                        var vn = new VolumeName(rawAttr);
                        Attributes.Add(vn);
                        break;
                    case AttributeType.VolumeInformation:
                        var vi = new VolumeInformation(rawAttr);
                        Attributes.Add(vi);
                        break;

                    case AttributeType.LoggedUtilityStream:
                        var lus = new LoggedUtilityStream(rawAttr);
                        Attributes.Add(lus);
                        break;

                    case AttributeType.ReparsePoint:
                        var rp = new ReparsePoint(rawAttr);
                        Attributes.Add(rp);
                        break;

                    case AttributeType.AttributeList:
                       var al = new AttributeList(rawAttr);
                       Attributes.Add(al);
                        break;

                    case AttributeType.Ea:
                        //TODO Finish this
                        var ea = new ExtendedAttribute(rawAttr);
                        Attributes.Add(ea);
                        break;

                    case AttributeType.EaInformation:
                        var eai = new ExtendedAttributeInformation(rawAttr);
                        Attributes.Add(eai);
                        break;

                    default:
                        Logger.Warn($"Unhandled attribute type! Add me: {(AttributeType) attrType}");
                        throw new Exception($"Add me: {(AttributeType) attrType}");
                        break;
                }

                index += attrSize;
            }

            SlackStartOffset = index;

            //rest is slack. handle here?
            Logger.Trace($"Slack starts at {index} i+o: 0x{index + offset:X}");
        }

        public List<Attribute> Attributes { get; }

        public int SlackStartOffset { get; }

        public int Offset { get; private set; }
        public int EntryNumber { get; }
        public short FirstAttributeOffset { get; }
        public int ActualRecordSize { get; }
        public int AllocatedRecordSize { get; }
        public MftEntryInfo MFTRecordToBaseRecord { get; }
        public short FirstAvailableAttribueId { get; }
        public short ReferenceCount { get; }
        public short SequenceNumber { get; }
        public EntryFlag EntryFlags { get; }
        public long LogSequenceNumber { get; }
        public short FixupEntryCount { get; }
        public short FixupOffset { get; }


        //meta stuff

            public string FName { get; }

        public DateTimeOffset SICreatedOn { get; }
        public DateTimeOffset SIContentModifiedOn { get; }
        public DateTimeOffset SILastAccessedOn { get; }
        public DateTimeOffset SIRecordModifiedOn { get; }

        public DateTimeOffset FNCreatedOn { get; }
        public DateTimeOffset FNContentModifiedOn { get; }
        public DateTimeOffset FNLastAccessedOn { get; }
        public DateTimeOffset FNRecordModifiedOn { get; }

        public override string ToString()
        {
//            var fns = Attributes.Where(t => t is FileName).ToList();
//            var name = string.Empty;
//
//            foreach (var attribute in fns)
//            {
//                var fn = attribute as FileName;
//
//                if (fn.FileInfo.NameType == NameTypes.Dos_Windows)
//                {
//                    name = fn.FileInfo.FileName;
//                }
//            }

            var std = (StandardInfo) Attributes.Single(t => t is StandardInfo);
            var fn = (FileName) Attributes.First(t => t is FileName);
            
            return $"fname: {FName} Entry #: {EntryNumber}, std: {std}, fn: {fn}";
        }
    }

    
}