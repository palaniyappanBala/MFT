using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFT.Attributes
{
    public class ResidentData
    {
        public byte[] Data { get; }
        public string Name { get; }

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
            Buffer.BlockCopy(rawBytes,contentOffset,Data,0,contentLength);



        }
    }
}
