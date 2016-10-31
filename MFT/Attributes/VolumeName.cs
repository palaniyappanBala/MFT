using System.Text;

namespace MFT.Attributes
{
    public class VolumeName : Attribute
    {
        public string Name { get; }
        public VolumeName(byte[] rawBytes) : base(rawBytes)
        {
            var residentData = new ResidentData(rawBytes);

            Name = string.Empty;

            if (residentData.Data.Length > 0)
            {
                Name = Encoding.Unicode.GetString(residentData.Data);
            }
            
        }
    }
}