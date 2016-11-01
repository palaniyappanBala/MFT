﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFT.Attributes
{
 public   class AttributeListEntry
    {
        public int Size { get; }
        public AttributeType AttributeType { get; }
        public int NameSize { get; }
        public int NameOffset { get; }
        public string Name { get; }
        public ulong FirstVirtualClusterNumber { get; }
        public MftEntryInfo EntryInformation { get; }
        public int AttributeID { get; }

        public AttributeListEntry(int size, AttributeType attributeType, int nameSize, int nameOffset, string name,
            ulong firstVCN, MftEntryInfo mftInfo, int attributeId)
        {
            Size = size;
            AttributeType = attributeType;
            NameSize = nameSize;
            NameOffset = nameOffset;
            Name = name;
            FirstVirtualClusterNumber = firstVCN;
            EntryInformation = mftInfo;
            AttributeID = attributeId;
        }


    }
}