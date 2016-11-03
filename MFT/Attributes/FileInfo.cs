using System;
using System.Text;

namespace MFT.Attributes
{
    [Flags]
    public enum NameTypes
    {
        Posix = 0x0,
        Windows = 0x1,
        Dos = 0x2,
        Dos_Windows = 0x3
    }

    public class FileInfo
    {
        public FileInfo(byte[] rawBytes)
        {
            if (rawBytes.Length == 0x04)
            {
                return;
            }

            var entryBytes = new byte[8];

            Buffer.BlockCopy(rawBytes, 0, entryBytes, 0, 8);

            MFTRecordToBaseRecord = new MftEntryInfo(entryBytes);

            if (rawBytes.Length <= 0x10)
            {
                return;
            }

            CreatedOn = DateTimeOffset.FromFileTime(BitConverter.ToInt64(rawBytes, 0x8));
            ContentModifiedOn = DateTimeOffset.FromFileTime(BitConverter.ToInt64(rawBytes, 0x10));


            RecordModifiedOn = DateTimeOffset.FromFileTime(BitConverter.ToInt64(rawBytes, 0x18));
            LastAccessedOn = DateTimeOffset.FromFileTime(BitConverter.ToInt64(rawBytes, 0x20));

            PhysicalSize = BitConverter.ToUInt64(rawBytes, 0x28);
            LogicalSize = BitConverter.ToUInt64(rawBytes, 0x30);

            Flags = (StandardInfo.Flag) BitConverter.ToInt32(rawBytes, 0x38);

            ReparseValue = BitConverter.ToInt32(rawBytes, 0x3c);

            NameLength = rawBytes[0x40];
            NameType = (NameTypes) rawBytes[0x41];

            FileName = Encoding.Unicode.GetString(rawBytes, 0x42, NameLength*2);
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