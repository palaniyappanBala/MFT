using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFT.Attributes
{
    public class SecurityDescriptor : Attribute
    {

        public SecurityDescriptor(byte[] rawBytes) : base(rawBytes)
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

            if (NonResident)
            {
                return;
            }

            SecurityInfo = new SKSecurityDescriptor(ResidentData.Data);
        }

        public SKSecurityDescriptor SecurityInfo { get; }

        public ResidentData ResidentData { get; }

        public NonResidentData NonResidentData { get; }
    }
}
