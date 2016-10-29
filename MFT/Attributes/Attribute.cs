using System;

namespace MFT
{
    public enum AttributeType
    {
        Unused = 0x0,
        StandardInformation = 0x10,
        AttributeList = 0x20,
        FileName = 0x30,
        VolumeVersionObjectId = 0x40,
        SecurityDescriptor = 0x50,
        VolumeName = 0x60,
        VolumeInformation = 0x70,
        Data = 0x80,
        IndexRoot = 0x90,
        IndexAllocation = 0xa0,
        Bitmap = 0xb0,
        SymbolicLinkReparsePoint = 0xc0,
        EaInformation = 0xd0,
        Ea = 0xe0,
        PropertySet = 0xf0,
        LoggedUtilityStream = 0x100,
        UserDefinedAttribute = 0x1000
    }

    public enum AttributeDataFlag
    {
        IsCompressed = 0x01,
        IsEncrypted = 0x4000,
        IsSparse = 0x8000
    }

    public abstract class Attribute
    {
        protected Attribute(byte[] rawBytes)
        {
            AttributeNumber = BitConverter.ToInt16(rawBytes, 0xE);

            AttributeType = (AttributeType) BitConverter.ToInt32(rawBytes, 0);
            AttributeSize = BitConverter.ToInt32(rawBytes, 4);

            NonResident = rawBytes[0x8] == 1;

            NameSize = rawBytes[0x09];

            AttributeContentLength = BitConverter.ToInt32(rawBytes, 0x10);
            ContentOffset = BitConverter.ToInt16(rawBytes, 0x14);
        }

        public AttributeType AttributeType { get; }
        public int AttributeSize { get; }
        public int AttributeContentLength { get; }
        public int NameSize { get; }
        public int AttributeNumber { get; }

        public bool NonResident { get; }

        public short ContentOffset { get; }

        public override string ToString()
        {
            return
                $"AttrType: {AttributeType}, Size: {AttributeSize}, Content size: {AttributeContentLength}, name size: {NameSize}, content offset: {ContentOffset}, non-resident: {NonResident}";
        }
    }
}