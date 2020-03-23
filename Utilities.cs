using System;

using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;

namespace DOOMSaveManager
{
    public static class Utilities
    {
        public static byte[] RandomBytes(int size) {
            byte[] output = new byte[size];
            new Random().NextBytes(output);
            return output;
        }

        public static bool ByteArraysEqual(byte[] b1, byte[] b2) {
            return b1.SequenceEqual(b2);
        }

        public static bool CheckUUID(string s) {
            Regex re = new Regex("^[0-9a-f]{8}-?[0-9a-f]{4}-?[0-9a-f]{4}-?[0-9a-f]{4}-?[0-9a-f]{12}$", RegexOptions.IgnoreCase);
            return re.IsMatch(s);
        }

        public static void Archive(string zipName, string path, string password = "") {
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

            using (var fsOut = File.Create(zipName))
            using (var zs = new ZipOutputStream(fsOut)) {
                zs.SetLevel(3);
                if (!String.IsNullOrEmpty(password))
                    zs.Password = password;
                foreach(var single in files) {
                    if (single.EndsWith("-BACKUP"))
                        continue;

                    var fi = new FileInfo(single);
                    var entryName = ZipEntry.CleanName(single.Substring(path.Length + 1));
                    var ze = new ZipEntry(entryName);
                    ze.Size = fi.Length;
                    ze.DateTime = fi.LastWriteTime;
                    zs.PutNextEntry(ze);
                    var buffer = new byte[4096];
                    using (var fsIn = File.OpenRead(single)) {
                        StreamUtils.Copy(fsIn, zs, buffer);
                    }
                    zs.CloseEntry();
                }
            }
        }

        public static void Unarchive(string zipName, string path, string password = "") {
            using (Stream fsIn = File.OpenRead(zipName))
            using (var zf = new ZipFile(fsIn)) {
                if (!String.IsNullOrEmpty(password))
                    zf.Password = password;

                foreach (ZipEntry zipEntry in zf) {
                    if (!zipEntry.IsFile)
                        continue;

                    String entryFileName = zipEntry.Name;

                    var fullZipToPath = Path.Combine(path, entryFileName);
                    var directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0) {
                        Directory.CreateDirectory(directoryName);
                    }

                    var buffer = new byte[4096];
                    using (var zs = zf.GetInputStream(zipEntry))
                    using (Stream fsOut = File.Create(fullZipToPath)) {
                        StreamUtils.Copy(zs, fsOut, buffer);
                    }
                }
            }
        }

        public static string Hexlify(byte[] data) {
            return BitConverter.ToString(data).Replace("-", "").ToUpper();
        }

        public static void PrintHex(byte[] data) {
            Console.WriteLine(Hexlify(data));
        }
    }
}