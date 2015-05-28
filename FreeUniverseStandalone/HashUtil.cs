using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace FreeUniverse
{
    class HashUtils
    {
        private static SHA256 sha256 = System.Security.Cryptography.SHA256.Create();
        private static MD5 md5 = System.Security.Cryptography.MD5.Create();
        private static long md5seed = DateTime.Now.Ticks;

        public static string SHA256(string input)
        {
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] hash = sha256.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("X2"));

            return sb.ToString();
        }

        public static string CalculateMD5Hash(string input)
        {
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("X2"));

            return sb.ToString();
        }

        public static string RandomSHA256()
        {
            long ticks = DateTime.Now.Ticks;
            md5seed <<= 1;
            md5seed += 1;
            md5seed ^= ticks;
            return SHA256(md5seed.ToString());
        }

        public static string RandomMD5()
        {
            long ticks = DateTime.Now.Ticks;
            md5seed <<= 1;
            md5seed += 1;
            md5seed ^= ticks;
            return CalculateMD5Hash(md5seed.ToString());
        }

        public static ulong StringToUINT64(string s)
        {
            Byte[] bytes = System.Text.Encoding.UTF8.GetBytes(s);
            UInt32 h1 = Hash(bytes);
            UInt32 h2 = Hash(bytes, h1);
            return (((ulong)h1) << 32) + (ulong)h2;
        }

        public static UInt32 Hash(Byte[] dataToHash, UInt32 hash)
        {
            Int32 dataLength = dataToHash.Length;

            if (dataLength == 0)
                return 0;

            Int32 remainingBytes = dataLength & 3; // mod 4
            Int32 numberOfLoops = dataLength >> 2; // div 4
            Int32 currentIndex = 0;
            while (numberOfLoops > 0)
            {
                hash += BitConverter.ToUInt16(dataToHash, currentIndex);
                UInt32 tmp = (UInt32)(BitConverter.ToUInt16(dataToHash, currentIndex + 2) << 11) ^ hash;
                hash = (hash << 16) ^ tmp;
                hash += hash >> 11;
                currentIndex += 4;
                numberOfLoops--;
            }

            switch (remainingBytes)
            {
                case 3:
                    hash += BitConverter.ToUInt16(dataToHash, currentIndex);
                    hash ^= hash << 16;
                    hash ^= ((UInt32)dataToHash[currentIndex + 2]) << 18;
                    hash += hash >> 11;
                    break;
                case 2:
                    hash += BitConverter.ToUInt16(dataToHash, currentIndex);
                    hash ^= hash << 11;
                    hash += hash >> 17;
                    break;
                case 1:
                    hash += dataToHash[currentIndex];
                    hash ^= hash << 10;
                    hash += hash >> 1;
                    break;
                default:
                    break;
            }

            /* Force "avalanching" of final 127 bits */
            hash ^= hash << 3;
            hash += hash >> 5;
            hash ^= hash << 4;
            hash += hash >> 17;
            hash ^= hash << 25;
            hash += hash >> 6;

            return hash;
        }

        public static uint StringToUINT32(string s)
        {
            return Hash(System.Text.Encoding.UTF8.GetBytes(s));
        }

        public static UInt32 Hash(Byte[] dataToHash)
        {
            Int32 dataLength = dataToHash.Length;
            if (dataLength == 0)
                return 0;
            UInt32 hash = Convert.ToUInt32(dataLength);
            Int32 remainingBytes = dataLength & 3; // mod 4
            Int32 numberOfLoops = dataLength >> 2; // div 4
            Int32 currentIndex = 0;
            while (numberOfLoops > 0)
            {
                hash += BitConverter.ToUInt16(dataToHash, currentIndex);
                UInt32 tmp = (UInt32)(BitConverter.ToUInt16(dataToHash, currentIndex + 2) << 11) ^ hash;
                hash = (hash << 16) ^ tmp;
                hash += hash >> 11;
                currentIndex += 4;
                numberOfLoops--;
            }

            switch (remainingBytes)
            {
                case 3:
                    hash += BitConverter.ToUInt16(dataToHash, currentIndex);
                    hash ^= hash << 16;
                    hash ^= ((UInt32)dataToHash[currentIndex + 2]) << 18;
                    hash += hash >> 11;
                    break;
                case 2:
                    hash += BitConverter.ToUInt16(dataToHash, currentIndex);
                    hash ^= hash << 11;
                    hash += hash >> 17;
                    break;
                case 1:
                    hash += dataToHash[currentIndex];
                    hash ^= hash << 10;
                    hash += hash >> 1;
                    break;
                default:
                    break;
            }

            /* Force "avalanching" of final 127 bits */
            hash ^= hash << 3;
            hash += hash >> 5;
            hash ^= hash << 4;
            hash += hash >> 17;
            hash ^= hash << 25;
            hash += hash >> 6;

            return hash;
        }
    }
}
