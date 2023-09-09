/**
 * C#版 翻译自网上大神出的Python版 https://github.com/DoubleLabyrinth/MobaXterm-keygen
 * 
 */
using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using static System.Console;
using static System.Text.Encoding; 
namespace mobaxkey
{
    public static class Program
    {
        // base64 cepher string
        private const string cepher = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
        public static byte[] VariantBase64Encode(byte[] bs)
        {
            var result = new List<byte>();
            int blocksCount = bs.Length / 3;
            int leftBytes = bs.Length % 3;

            for (int i = 0; i < blocksCount; i++)
            {
                int codingInt = ReadBytesToInt(bs, 3 * i, 3);
                string block = $"{cepher[codingInt & 0x3f]}{cepher[(codingInt >> 6) & 0x3f]}{cepher[(codingInt >> 12) & 0x3f]}{cepher[(codingInt >> 18) & 0x3f]}";
                result.AddRange(UTF8.GetBytes(block));
            }

            if (leftBytes == 0)
            {
                return result.ToArray();
            }
            else if (leftBytes == 1)
            {
                int codingInt = ReadBytesToInt(bs, 3 * blocksCount, leftBytes);
                string block = $"{cepher[codingInt & 0x3f]}{cepher[(codingInt >> 6) & 0x3f]}";
                result.AddRange(UTF8.GetBytes(block));
                return result.ToArray();
            }
            else
            {
                int codingInt = ReadBytesToInt(bs, 3 * blocksCount, leftBytes);
                string block = $"{cepher[codingInt & 0x3f]}{cepher[(codingInt >> 6) & 0x3f]}{cepher[(codingInt >> 12) & 0x3f]}";
                result.AddRange(UTF8.GetBytes(block));
                return result.ToArray();
            }
        }

        static int ReadBytesToInt(byte[] bs, int start, int length)
        {
            var b4 = new byte[4];
            int end = start + length;
            for (int i = 0; i < 4; i++)
            {
                b4[i] = i < length ? bs[start + i] : byte.MinValue;
            }
            return BitConverter.ToInt32(b4, 0);
        }

        public static byte[] EncryptBytes(int key, byte[] bs)
        {
            var ret = new byte[bs.Length];

            for (int i = 0; i < bs.Length; i++)
            {
                ret[i] = (byte)(bs[i] ^ ((key >> 8) & 0xff));
                key = ret[i] & key | 0x482D;
            }
            return ret;
        }

        public enum LicenseType
        {
            Professional = 1,
            Educational = 3,
            Personal = 4
        }

        public static void GenerateLicense(LicenseType type, int count, string userName, int majorVersion, int minorVersion)
        {
            if (count < 0)
            {
                throw new ArgumentException("Count must be greater than or equal to 0");
            }

            string licenseString = $"{(int)type}#{userName}|{majorVersion}{minorVersion}#{count}#{majorVersion}3{minorVersion}6{minorVersion}#0#0#0#";
            string encodedLicenseString = UTF8.GetString(VariantBase64Encode(EncryptBytes(0x787, UTF8.GetBytes(licenseString))));

            using var zipFile = new ZipArchive(new FileStream("Custom.mxtpro", FileMode.Create), ZipArchiveMode.Create);
            var entry = zipFile.CreateEntry("Pro.key");
            using var writer = new StreamWriter(entry.Open());
            writer.Write(encodedLicenseString);

        }

        public static void Help()
        {
            WriteLine("Usage:");
            WriteLine($"     {Path.GetFileName(Environment.ProcessPath)} <UserName> <Version>");
            WriteLine();
            WriteLine("    <UserName>:      The Name licensed to");
            WriteLine("                     Example:  gwang");
            WriteLine("    <Version>:       The Version of MobaXterm");
            WriteLine("                     Example:    23.2");
            WriteLine("Special: the algorithm was study from: https://github.com/DoubleLabyrinth/MobaXterm-keygen");
            WriteLine();
            WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Help();
                Environment.Exit(0);
            }
            else
            {
                var userName = args[0];
                userName = Environment.UserName;
                string[] versionParts = args[1].Split('.');
                int majorVersion = int.Parse(versionParts[0]);
                int minorVersion = int.Parse(versionParts[1]);
                GenerateLicense(LicenseType.Professional,
                                3,
                                args[0],
                                majorVersion,
                                minorVersion);
                WriteLine("[*] Success!");
                WriteLine($"[*] File generated: {Path.Combine(Directory.GetCurrentDirectory(), "Custom.mxtpro")}");
                WriteLine("[*] Please move or copy the newly-generated file to MobaXterm's installation path.");
                WriteLine();
            }

        }

    }
}
