﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using MFT.Attributes;
using NLog;

namespace MFT
{
    public class FileRecord
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Flags]
        public enum EntryFlag
        {
            FileRecordSegmentInUse = 0x1,
            FileNameIndexPresent = 0x2,
            Unknown0 = 0x4,
            Unknown1 = 0x8
        }

        private readonly int _baadSig = 0x44414142;
        private readonly int _fileSig = 0x454c4946;

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

            LogSequenceNumber = BitConverter.ToInt32(rawBytes, 0x8);

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

            FirstAvailablAttribueId = BitConverter.ToInt16(rawBytes, 0x28);

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

            if (x1FeValue != expectedFixupVal && ((EntryFlags & EntryFlag.FileRecordSegmentInUse) == EntryFlag.FileRecordSegmentInUse))
            {
                Logger.Warn($"FILE record at offset 0x{offset:X}! Fixup values do not match at 0x1FE. Expected: {expectedFixupVal}, actual: {x1FeValue}, EntryFlags: {EntryFlags}");
            }

            if (x3FeValue != expectedFixupVal && ((EntryFlags & EntryFlag.FileRecordSegmentInUse) == EntryFlag.FileRecordSegmentInUse))
            {
                Logger.Warn($"FILE record at offset 0x{offset:X}! Fixup values do not match at 0x3FE. Expected: {expectedFixupVal}, actual: {x3FeValue}, EntryFlags: {EntryFlags}");
            }

            //header is done, replace fixup bytes with actual bytes
            //0x1fe and 0x3fe should contain fixup bytes 

            Buffer.BlockCopy(fixupActual1, 0, rawBytes, 0x1fe, 2);
            Buffer.BlockCopy(fixupActual2, 0, rawBytes, 0x3fe, 2);

            //start attribute processing at FirstAttributeOffset

            var index = (int) FirstAttributeOffset;

//            if (offset == 0x2DC00)
//                Debug.WriteLine(1);

            while (index < ActualRecordSize)
            {
                var attrType = BitConverter.ToInt32(rawBytes, index);

                var attrSize = BitConverter.ToInt32(rawBytes, index + 4);

//                if (attrSize > 1024 && attrType != -1)
//                {
//                    attrSize = BitConverter.ToInt16(rawBytes, index + 4);
//                }

                Logger.Trace($"ActualRecordSize: {ActualRecordSize} attrType: 0x{attrType:X}, size: {attrSize}, index: {index}, offset: 0x{offset:x}, i+o: 0x{(index + offset):X}");

                if ((attrSize == 0) || (attrType == -1))
                {
                    index += 8; //skip -1 type and 0 size

                    if (EntryFlags == 0) //this is a free record
                    {
                        break;
                    }

                    continue;
                }

//                if (offset + index == 0x7108)
//                {
//                    Debug.WriteLine(1);
//                }

                var rawAttr = new byte[attrSize];
                Buffer.BlockCopy(rawBytes, index, rawAttr, 0, attrSize);

                switch ((AttributeType) attrType)
                {
                    case AttributeType.StandardInformation:
                        var si = new StandardInfo(rawAttr);
                        Attributes.Add(si);
                        break;
                    case AttributeType.FileName:
                        var fi = new FileInfo(rawAttr);
                        Attributes.Add(fi);
                        break;
                    case AttributeType.Data:
                        var data = new Data(rawAttr);
                        Attributes.Add(data);
                        break;
                    case AttributeType.IndexAllocation:

                        break;
                    case AttributeType.IndexRoot:

                        break;

                    case AttributeType.Bitmap:

                        break;

                    case AttributeType.VolumeVersionObjectId:

                        break;

                    case AttributeType.SecurityDescriptor:

                        break;
                    case AttributeType.VolumeName:

                        break;
                    case AttributeType.VolumeInformation:

                        break;

                    case AttributeType.LoggedUtilityStream:

                        break;

                    case AttributeType.SymbolicLinkReparsePoint:

                        break;

                    case AttributeType.AttributeList:

                        break;

                    case AttributeType.Ea:

                        break;

                    case AttributeType.EaInformation:

                        break;

                    default:
                        Logger.Warn($"Unhandled attribute tyoe! Add me: {(AttributeType)attrType}");
                        throw new Exception($"Add me: {(AttributeType) attrType}");
                        break;
                }


                index += attrSize;
            }

            //rest is slack. handle here?
            Logger.Trace($"Slack starts at {index} i+o: 0x{(index + offset):X}");
        }

        public List<Attribute> Attributes { get; }

        public int Offset { get; private set; }
        public int EntryNumber { get; }
        public short FirstAttributeOffset { get; }
        public int ActualRecordSize { get; }
        public int AllocatedRecordSize { get; }
        public MftEntryInfo MFTRecordToBaseRecord { get; }
        public short FirstAvailablAttribueId { get; }
        public short ReferenceCount { get; }
        public short SequenceNumber { get; }
        public EntryFlag EntryFlags { get; }
        public int LogSequenceNumber { get; }
        public short FixupEntryCount { get; }
        public short FixupOffset { get; }
    }
}