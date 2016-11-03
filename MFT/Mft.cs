using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NLog;

namespace MFT
{
    public class Mft
    {
        public Mft(byte[] rawbytes)
        {
            FileRecords = new List<FileRecord>(); //new Dictionary<int, FileRecord>();

            var blockSize = 1024;
            
            var index = 0;

            var logger = LogManager.GetCurrentClassLogger();

//            var totalFileRecordChunks =rawbytes.Length/4096;
//
//            var parallelCount = Environment.ProcessorCount/2;
//
//            var chunksToProcessCount = totalFileRecordChunks / parallelCount;
//
//            Debug.WriteLine(chunksToProcessCount);
//
//            //cut up rawBytes into chunks
//
//            var chunks = new List<byte[]>();
//
//            var chunkIndex = 0;
//
//            for (var i = 0; i < parallelCount - 1; i++)
//            {
//                var chunkLen = chunksToProcessCount*4096;
//                var chunk = new byte[chunkLen];
//
//                Buffer.BlockCopy(rawbytes,chunkIndex,chunk,0, chunkLen);
//
//                chunkIndex += chunkLen;
//
//                chunks.Add(chunk);
//            }
//
//            //get everything else
//            var chunkLenF = rawbytes.Length - chunkIndex;
//
//            var chunkF = new byte[chunkLenF];
//
//            Buffer.BlockCopy(rawbytes, chunkIndex, chunkF, 0, chunkLenF);
//
//            chunks.Add(chunkF);

//            double totalSeconds = 0;
//            Parallel.ForEach(chunks, (c) =>
//            {
//                var sw = new Stopwatch();
//                sw.Start();
//                var localIndex = 0;
//
//                while (localIndex<c.Length)
//                {
//                    var fileBytes1 = new byte[1024];
//                    Buffer.BlockCopy(c, localIndex, fileBytes1, 0, blockSize);
//
//                    var f = new FileRecord(fileBytes1, localIndex);
//
//                    localIndex += blockSize;
//                }
//
//               
//
//               sw.Stop();
//
//                totalSeconds += sw.Elapsed.TotalSeconds;
//
//                Debug.WriteLine(sw.Elapsed.TotalSeconds);
//            });
//
//            Debug.WriteLine(totalSeconds);


            var sw1 = new Stopwatch();
            sw1.Start();
            while (index < rawbytes.Length)
            {
                var fileBytes = new byte[1024];

                logger.Trace($"Reading bytes at index 0x{index:x}");

                Buffer.BlockCopy(rawbytes, index, fileBytes, 0, blockSize);

                var f = new FileRecord(fileBytes, index);

                FileRecords.Add(f);

                index += blockSize;
            }
            sw1.Stop();

            logger.Info($"RaW Processing took {sw1.Elapsed.TotalSeconds} seconds");


//            foreach (var fileRecord in FileRecords)
//            {
//               
//            }

        }

        public List<FileRecord> FileRecords { get; }
    }
}