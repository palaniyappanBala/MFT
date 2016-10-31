using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFT.Attributes
{
    public class ExtendedAttributeInformation : Attribute
    {
        public short EaSize { get; }
        public short NumberOfExtendedAttrWithNeedEaSet { get; }
        public int SizeOfEaData { get; }

        public ExtendedAttributeInformation(byte[] rawBytes) : base(rawBytes)
        {
            var content = new byte[AttributeContentLength];

            Buffer.BlockCopy(rawBytes, ContentOffset, content, 0, AttributeContentLength);

            EaSize = BitConverter.ToInt16(content, 0);
            NumberOfExtendedAttrWithNeedEaSet = BitConverter.ToInt16(content, 2);
            SizeOfEaData = BitConverter.ToInt32(content, 4);

        }

    }

  
}
