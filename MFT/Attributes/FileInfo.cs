using System;
using System.Text;

namespace MFT.Attributes
{
    public class FileInfo : Attribute
    {
        [Flags]
        public enum NameTypes
        {
            Posix = 0x0,
            Windows = 0x1,
            Dos = 0x2,
            Dos_Windows = 0x3
        }

        public FileInfo(byte[] rawBytes) : base(rawBytes)
        {
            var entryBytes = new byte[8];

            Buffer.BlockCopy(rawBytes, 0x18, entryBytes, 0, 8);

            MFTRecordToBaseRecord = new MftEntryInfo(entryBytes);

            CreatedOn = DateTimeOffset.FromFileTime(BitConverter.ToInt64(rawBytes, 0x20));
            ContentModifiedOn = DateTimeOffset.FromFileTime(BitConverter.ToInt64(rawBytes, 0x28));
            RecordModifiedOn = DateTimeOffset.FromFileTime(BitConverter.ToInt64(rawBytes, 0x30));
            LastAccessedOn = DateTimeOffset.FromFileTime(BitConverter.ToInt64(rawBytes, 0x38));

            PhysicalSize = BitConverter.ToUInt64(rawBytes, 0x40);
            LogicalSize = BitConverter.ToUInt64(rawBytes, 0x48);

            Flags = (StandardInfo.Flag) BitConverter.ToInt32(rawBytes, 0x50);

            ReparseValue = BitConverter.ToInt32(rawBytes, 0x54);

            NameLength = rawBytes[0x58];
            NameType = (NameTypes) rawBytes[0x59];

            FileName = Encoding.Unicode.GetString(rawBytes, 0x5A, NameLength*2);
        }

        public int ReparseValue { get; }
        public byte NameLength { get; }
        public NameTypes NameType { get; }
        public string FileName { get; }
        public ulong PhysicalSize { get; }
        public ulong LogicalSize { get; }
        public DateTimeOffset CreatedOn { get; }
        public DateTimeOffset ContentModifiedOn { get; }
        public DateTimeOffset RecordModifiedOn { get; }
        public DateTimeOffset LastAccessedOn { get; }
        public StandardInfo.Flag Flags { get; }

        public MftEntryInfo MFTRecordToBaseRecord { get; }
    }
}