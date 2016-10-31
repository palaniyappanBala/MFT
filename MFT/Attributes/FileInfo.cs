using System;

namespace MFT.Attributes
{
    public class FileInfo : Attribute
    {
        public FileInfo(byte[] rawBytes) : base(rawBytes)
        {
            var content = new byte[rawBytes.Length - ContentOffset];

            Buffer.BlockCopy(rawBytes, ContentOffset, content, 0, rawBytes.Length - ContentOffset);

            FileInfoBase = new FileInfoBase(content);
        }

        public FileInfoBase FileInfoBase { get; }
    }
}