using System;
using System.Collections.Generic;
using NLog;
using NLog.Config;
using NLog.Targets;
using NUnit.Framework;

namespace MFT.Test
{
    [TestFixture]
    public class TestMain
    {
        [SetUp]
        public void Setup()
        {
            var config = new LoggingConfiguration();
            var loglevel = LogLevel.Debug;

            var layout = @"${message}";

            var cc = new ColoredConsoleTarget();

            var tt = new FileTarget();
            tt.FileName = @"C:\temp\mftLog.txt";

            config.AddTarget("cc", cc);
            config.AddTarget("tt", tt);

            cc.Layout = layout;

            var rule1 = new LoggingRule("*", loglevel, cc);
            config.LoggingRules.Add(rule1);
            var rule2 = new LoggingRule("*", loglevel, tt);
            config.LoggingRules.Add(rule2);

            LogManager.Configuration = config;

            Mfts = new Dictionary<string, string>();

         //   Mfts.Add("controller", @"D:\Dropbox\MFTs\controller\$MFT");
         //   Mfts.Add("nfury", @"D:\Dropbox\MFTs\nfury\$MFT");
        //    Mfts.Add("nromanoff", @"D:\Dropbox\MFTs\nromanoff\$MFT");
            Mfts.Add("tdungan", @"..\..\TestFiles\tdungan\$MFT");
         //   Mfts.Add("blake", @"D:\Dropbox\MFTs\blake\$MFT");
       //     Mfts.Add("other", @"..\..\TestFiles\$MFT");
        }


        public Dictionary<string, string> Mfts = new Dictionary<string, string>();

        [Test]
        public void Something()
        {
            var logger = LogManager.GetCurrentClassLogger();

            foreach (var mft in Mfts)
            {
                logger.Info($"Processing {mft.Key}");

                var start = DateTimeOffset.Now;


                var m2 = MftFile.Load(mft.Value);

                var end = DateTimeOffset.Now;

                var dif = end.Subtract(start).TotalSeconds;

                logger.Info($"Took {dif} seconds to process '{mft.Key}'\r\n");
            }
        }
    }
}