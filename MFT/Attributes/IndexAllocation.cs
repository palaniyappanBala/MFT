namespace MFT.Attributes
{
    public class IndexAllocation : Attribute
    {
        public IndexAllocation(byte[] rawBytes) : base(rawBytes)
        {
            var nonResident = new NonResidentData(rawBytes);
        }
    }
}