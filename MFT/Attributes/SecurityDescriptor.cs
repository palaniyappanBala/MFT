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
            //TODO fix this after refactoring these 2 classes
            if (NonResident)
            {
                NonResidentData = new NonResidentData(rawBytes);
            }
            else
            {
                ResidentData = new ResidentData(rawBytes);
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
