using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFT.Attributes
{
    public class Bitmap : Attribute
    {


        public Bitmap(byte[] rawBytes) : base(rawBytes)
        {
            //TODO fix this after refactoring these 2 classes
            if (NonResident)
            {
                NonResidentData = new NonResidentData(rawBytes);
            }
            else
            {
                ResidentData = new ResidentData(rawBytes);
            }


            var content = new byte[AttributeContentLength];

            Buffer.BlockCopy(rawBytes, ContentOffset, content, 0, AttributeContentLength);

            

        }

        public ResidentData ResidentData { get;}

        public NonResidentData NonResidentData { get;  }
    }
}
