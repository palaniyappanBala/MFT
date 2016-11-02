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
            if (NonResident)
            {
        
                NonResidentData = new NonResidentData(rawBytes);
            }
            else
            {
                var content = new byte[AttributeContentLength];

                Buffer.BlockCopy(rawBytes, ContentOffset, content, 0, AttributeContentLength);

                ResidentData = new ResidentData(content);
            }


           

            

        }

        public ResidentData ResidentData { get;}

        public NonResidentData NonResidentData { get;  }
    }
}
