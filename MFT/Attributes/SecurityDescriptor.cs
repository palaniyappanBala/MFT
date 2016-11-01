using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFT.Attributes
{
    public class SecurityDescriptor : Attribute
    {
        [Flags]
        public enum ControlFlag
        {
            SeDaclAutoInherited = 0x0400,
            SeDaclAutoInheritReq = 0x0100,
            SeDaclDefaulted = 0x0008,
            SeDaclPresent = 0x0004,
            SeDaclProtected = 0x1000,
            SeGroupDefaulted = 0x0002,
            SeOwnerDefaulted = 0x0001,
            SeRmControlValid = 0x4000,
            SeSaclAutoInherited = 0x0800,
            SeSaclAutoInheritReq = 0x0200,
            SeSaclDefaulted = 0x0020,
            SeSaclPresent = 0x0010,
            SeSaclProtected = 0x2000,
            SeSelfRelative = 0x8000
        }

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
