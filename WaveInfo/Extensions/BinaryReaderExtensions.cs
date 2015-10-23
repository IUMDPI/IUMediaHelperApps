using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace WaveInfo.Extensions
{
    public static class BinaryReaderExtensions
    {
        private const int MaxReadLength = 1024; 

        public static string Md5Hash(this BinaryReader reader, ulong length)
        {
            ulong totalRead = 0;
            var toRead = GetLengthToProcess(totalRead, length);
            var finalBlock = false;

            using (var md5 = MD5.Create())
            {
                while (toRead > 0)
                {
                    var buffer = reader.ReadBytes(toRead);
                    totalRead += Convert.ToUInt64(toRead);

                    if (finalBlock == false)
                    {
                        md5.TransformBlock(buffer, 0, toRead, buffer, 0);
                    }
                    else
                    {
                        md5.TransformFinalBlock(buffer, 0, toRead);
                    }


                    toRead = GetLengthToProcess(totalRead, length);
                    finalBlock = IsFinalBlock(totalRead, length);
                }

                return md5.Hash.Aggregate(string.Empty, (current, b) => current + $"{b:x2}");
            }
        }

        private static bool IsFinalBlock(ulong read, ulong length)
        {
            return read + MaxReadLength >= length;
        }

        private static int GetLengthToProcess(ulong read, ulong length)
        {
            return (int)(read + MaxReadLength < length ? MaxReadLength : length - read);
        }
    }
}