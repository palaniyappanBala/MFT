﻿using System.IO;

namespace MFT
{
    public static class MftFile
    {
        public static Mft Load(string mftPath)
        {
            if (File.Exists(mftPath) == false)
            {
                throw new FileNotFoundException($"'{mftPath}' not found");
            }
            var bytes = File.ReadAllBytes(mftPath);

            return new Mft(bytes);
        }
    }
}