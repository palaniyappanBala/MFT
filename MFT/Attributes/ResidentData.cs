using System;

namespace MFT.Attributes
{
    public class ResidentData
    {
        public ResidentData(byte[] rawBytes)
        {
            //TODO refactor this to only deal with resident data and not attribute header lengths and whatnot
            var contentLength = BitConverter.ToUInt16(rawBytes, 0x10);
            var contentOffset = BitConverter.ToUInt16(rawBytes, 0x14);


            Data = new byte[contentLength];
            Buffer.BlockCopy(rawBytes, contentOffset, Data, 0, contentLength);
        }

        public byte[] Data { get; }
    }
}