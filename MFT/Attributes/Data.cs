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
                ResidentData = new ResidentData(rawBytes);
            }
        }

        public ResidentData ResidentData { get; }
        public NonResidentData NonResidentData { get; }
    }
}