namespace MFT.Attributes
{
    public class Data : Attribute
    {
        public ResidentData ResidentData { get; }
        public NonResidentData NonResidentData { get; }

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
    }
}