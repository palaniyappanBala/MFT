using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFT.Attributes
{
    public class AttributeList : Attribute
    {
        public List<AttributeListEntry> AttributeListEntries { get; }

        public AttributeList(byte[] rawBytes) : base(rawBytes)
        {
            var content = new byte[AttributeContentLength];

            Buffer.BlockCopy(rawBytes, ContentOffset, content, 0, AttributeContentLength);

            AttributeListEntries = new List<AttributeListEntry>();

            //loop thru using size

            var index = 0;

            while (index<content.Length)
            {
                var size = BitConverter.ToInt16(content, index + 4);

                var attrBytes = new byte[size];

                Buffer.BlockCopy(content,index,attrBytes,0,size);

                var attrIndex = 0;

                var attrType = (AttributeType)BitConverter.ToInt32(attrBytes, attrIndex);
                attrIndex += 4;

                attrIndex += 2;//skip size

                var nameSize = attrBytes[attrIndex];
                attrIndex += 1;
                var nameOffset = attrBytes[attrIndex];
                attrIndex += 1;

                var name = string.Empty;
                if (nameSize > 0)
                {
                    name = Encoding.Unicode.GetString(attrBytes, nameOffset, nameSize*2);
                }

                var firstVCN = BitConverter.ToUInt64(attrBytes, attrIndex);
                attrIndex += 8;
                
                var buff = new byte[8];
                Buffer.BlockCopy(attrBytes, attrIndex, buff,0,8);
                attrIndex += 8;
                
                var fr = new MftEntryInfo(buff);
                
                var attrId = BitConverter.ToInt16(attrBytes, attrIndex);

                var ale = new AttributeListEntry(size,attrType,nameSize,nameOffset,name,firstVCN,fr,attrId);

                AttributeListEntries.Add(ale);


                index += size;
            }



        }

    }
}
