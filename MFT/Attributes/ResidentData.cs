using System;
using System.Text;

namespace MFT.Attributes
{
    public class ResidentData
    {
        public ResidentData(byte[] rawBytes)
        {
            var contentLength = BitConverter.ToUInt16(rawBytes, 0x10);
            var contentOffset = BitConverter.ToUInt16(rawBytes, 0x14);

            var nameLen = rawBytes[9];
            var nameOffset = BitConverter.ToUInt16(rawBytes, 0xA);

            Name = string.Empty;


            if (nameLen > 0)
            {
                Name = Encoding.Unicode.GetString(rawBytes, nameOffset, nameLen*2);
            }


            Data = new byte[contentLength];
            Buffer.BlockCopy(rawBytes, contentOffset, Data, 0, contentLength);
        }

        public byte[] Data { get; }
        public string Name { get; }
    }
}