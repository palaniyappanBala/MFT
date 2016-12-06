using System.Text;

namespace MFT.Attributes
{
    public class VolumeName : Attribute
    {
        public string VolName { get; }
        public VolumeName(byte[] rawBytes) : base(rawBytes)
        {
            var residentData = new ResidentData(rawBytes);

            VolName = string.Empty;

            if (residentData.Data.Length > 0)
            {
                VolName = Encoding.Unicode.GetString(residentData.Data);
            }
            
        }
    }
}