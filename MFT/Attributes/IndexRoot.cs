using System;
using System.Collections.Generic;

namespace MFT.Attributes
{
    public class IndexRoot : Attribute
    {
        public IndexFlag Flags;

        public enum CollationTypes
        {
            Binary = 0x000000,
            Filename = 0x000001,
            Unicode = 0x000002,
            NtOfsUlong = 0x000010,
            NtOfsSid = 0x000011,
            NtOfsSecurityHash = 0x000012,
            NtOfsUlongs = 0x000013
        }

        public enum IndexFlag
        {
            HasSubNode = 0x001,
            IsLast = 0x002
        }

        public IndexRoot(byte[] rawBytes) : base(rawBytes)
        {
            var index = (int) ContentOffset;

            IndexedAttributeType = (AttributeType) BitConverter.ToInt32(rawBytes, index);
            index += 4;

            CollationType = (CollationTypes) BitConverter.ToInt32(rawBytes, index);
            index += 4;

            EntrySize = BitConverter.ToInt32(rawBytes, index);
            index += 4;

            NumberClusterBlocks = BitConverter.ToInt32(rawBytes, index);
            index += 4;

            //index node header starts here

            var rawIndexNodeHeader = new byte[16];

            Buffer.BlockCopy(rawBytes, index, rawIndexNodeHeader, 0, 16);
            index += 16;

            IndexNodeHeader = new IndexNodeHeader(rawIndexNodeHeader);

            FileInfoRecords = new List<FileInfo>();

            //finally, an array of index values
            while (index < rawBytes.Length)
            {
                var fileRefBytes = new byte[8];
                Buffer.BlockCopy(rawBytes, index, fileRefBytes, 0, 8);

                var indexValSize = BitConverter.ToInt16(rawBytes, index + 8);

                var keyDataSize = BitConverter.ToInt16(rawBytes, index + 10);

                Flags = (IndexFlag) BitConverter.ToInt32(rawBytes, index + 12);

                if (keyDataSize > 0)
                {
                    var buff = new byte[keyDataSize];

                    Buffer.BlockCopy(rawBytes, index + 16, buff, 0, keyDataSize);

                    var fi = new FileInfo(buff);
                    FileInfoRecords.Add(fi);
                }

                index += indexValSize;
            }
        }

        public IndexNodeHeader IndexNodeHeader { get; }
        public AttributeType IndexedAttributeType { get; }
        public int EntrySize { get; }
        public int NumberClusterBlocks { get; }

        public List<FileInfo> FileInfoRecords { get; }

        public CollationTypes CollationType { get; }
    }
}