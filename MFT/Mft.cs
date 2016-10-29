using System;
using System.Collections.Generic;
using NLog;

namespace MFT
{
    public class Mft
    {
        public Mft(byte[] rawbytes)
        {
            FileRecords = new List<FileRecord>();

            var blockSize = 1024;

            var fileBytes = new byte[1024];

            var index = 0;

            var logger = LogManager.GetCurrentClassLogger();

            //TODO add parallelism

            while (index < rawbytes.Length)
            {
                logger.Trace($"Reading bytes at index 0x{index:x}");

                Buffer.BlockCopy(rawbytes, index, fileBytes, 0, blockSize);

                var f = new FileRecord(fileBytes, index);

                FileRecords.Add(f);

                index += blockSize;
            }
        }

        public List<FileRecord> FileRecords { get; }
    }
}