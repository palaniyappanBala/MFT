using System;

namespace MFT.Attributes
{
    public class Data : Attribute
    {
        public Data(byte[] rawBytes) : base(rawBytes)
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

        public ResidentData ResidentData { get; }
        public NonResidentData NonResidentData { get; }
    }
}