using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFT.Attributes
{
    public class LoggedUtilityStream : Attribute
    {


        public LoggedUtilityStream(byte[] rawBytes) : base(rawBytes)
        {
            var content = new byte[AttributeContentLength];

            Buffer.BlockCopy(rawBytes, ContentOffset, content, 0, AttributeContentLength);

            //TODO add ResidentData object to hold what is there

        }

    }
}
