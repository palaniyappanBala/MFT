using System;

namespace MFT.Attributes
{
    public class IndexAllocation : Attribute
    {
        public IndexAllocation(byte[] rawBytes) : base(rawBytes)
        {
           var NonResidentData = new NonResidentData(rawBytes);
        }
    }
}