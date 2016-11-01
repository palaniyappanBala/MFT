﻿using System;
using System.Collections.Generic;

namespace MFT.Attributes
{
    public class NonResidentData
    {
        public NonResidentData(byte[] rawBytes)
        {
            //TODO refactor this to only deal with resident data and not attribute header lengths and whatnot

            StartingVirtualClusterNumber = BitConverter.ToUInt64(rawBytes, 0x10);
            EndingVirtualClusterNumber = BitConverter.ToUInt64(rawBytes, 0x18);

            var offsetToDataRuns = BitConverter.ToUInt16(rawBytes, 0x20);

            AllocatedSize = BitConverter.ToUInt64(rawBytes, 0x28);
            ActualSize = BitConverter.ToUInt64(rawBytes, 0x30);
            InitializedSize = BitConverter.ToUInt64(rawBytes, 0x38);

            var index = (int) offsetToDataRuns; //set index into bytes to start reading offsets

            DataRuns = new List<DataRun>();

            var drStart = rawBytes[index];

            while (drStart != 0)
            {
                var offsetLength = (byte) ((drStart & 0xF0) >> 4); //left nibble
                var clusterLenByteCount = (byte) (drStart & 0x0F); //right nibble
                index += 1;

                var runLenBytes = new byte[8]; //length should never exceed 8, so start with 8
                Buffer.BlockCopy(rawBytes, index, runLenBytes, 0, clusterLenByteCount);

                index += clusterLenByteCount;

                var clusterRunLength = BitConverter.ToUInt64(runLenBytes, 0);

                var clusterBytes = new byte[8]; //length should never exceed 8, so start with 8

                //copy in what we have
                Buffer.BlockCopy(rawBytes, index, clusterBytes, 0, offsetLength);

                //we can safely get our cluster #
                var clusterNumber = BitConverter.ToInt64(clusterBytes, 0);


                index += offsetLength;

                var dr = new DataRun(clusterRunLength, clusterNumber);
                DataRuns.Add(dr);

                drStart = rawBytes[index];
            }
        }

        public ulong StartingVirtualClusterNumber { get; }
        public ulong EndingVirtualClusterNumber { get; }
        public ulong AllocatedSize { get; }
        public ulong ActualSize { get; }
        public ulong InitializedSize { get; }
        public List<DataRun> DataRuns { get; }
    }
}