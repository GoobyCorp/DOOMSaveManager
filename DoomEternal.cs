using System;
using System.Linq;

using System.IO;

namespace DOOMSaveManager
{
    public class DoomEternal
    {
        public const string GameName = "Doom Eternal";

        public static string SavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Saved Games", "id Software", "DOOMEternal", "base", "savegame");

        public const string BackupPassword = "doometernalbackup";

        public static string[] GetUserIDs() {
            return Directory.GetDirectories(SavePath, "*.*", SearchOption.TopDirectoryOnly).Select(single => Path.GetFileNameWithoutExtension(single)).Where(single => Utilities.CheckUUID(single)).ToArray();
        }

        public static void FileEncrypt(string fromFile, string toFile, string toUUID) {
            if (fromFile.EndsWith("-BACKUP"))
                return;
            byte[] fromFileData = File.ReadAllBytes(fromFile);
            Directory.CreateDirectory(Path.GetDirectoryName(toFile));
            File.WriteAllBytes(toFile, Crypto.EncryptAndDigest($"{toUUID}PAINELEMENTAL{Path.GetFileName(toFile)}", fromFileData));
        }

        public static void FileDecrypt(string fromFile, string fromUUID, string toFile) {
            if (fromFile.EndsWith("-BACKUP"))
                return;
            byte[] fromFileData = Crypto.DecryptAndVerify($"{fromUUID}PAINELEMENTAL{Path.GetFileName(fromFile)}", File.ReadAllBytes(fromFile));
            Directory.CreateDirectory(Path.GetDirectoryName(toFile));
            File.WriteAllBytes(toFile, fromFileData);
        }

        public static void FileTransfer(string fromFile, string fromUUID, string toFile, string toUUID) {
            if (fromFile.EndsWith("-BACKUP"))
                return;
            byte[] fromFileData = Crypto.DecryptAndVerify($"{fromUUID}PAINELEMENTAL{Path.GetFileName(fromFile)}", File.ReadAllBytes(fromFile));
            Directory.CreateDirectory(Path.GetDirectoryName(toFile));
            File.WriteAllBytes(toFile, Crypto.EncryptAndDigest($"{toUUID}PAINELEMENTAL{Path.GetFileName(toFile)}", fromFileData));
        }

        public static void BulkTransfer(string fromUUID, string toUUID) {
            string fromDir = Path.Combine(SavePath, fromUUID);
            foreach(var single in Directory.GetFiles(fromDir, "*.*", SearchOption.AllDirectories)) {
                FileTransfer(single, fromUUID, single.Replace(fromUUID, toUUID), toUUID);
            }
        }

        public static void BulkEncrypt(string fromDir, string toUUID) {
            string toDir = Path.Combine(SavePath, toUUID);
            foreach (var single in Directory.GetFiles(fromDir, "*.*", SearchOption.AllDirectories)) {
                Console.WriteLine(Path.Combine(toDir, single.Replace(fromDir, "").Substring(1)));
                FileEncrypt(single, Path.Combine(toDir, single.Replace(fromDir, "").Substring(1)), toUUID);
            }
        }

        public static void BulkDecrypt(string fromUUID, string toDir) {
            string fromDir = Path.Combine(SavePath, fromUUID);
            foreach (var single in Directory.GetFiles(fromDir, "*.*", SearchOption.AllDirectories)) {
                FileDecrypt(single, fromUUID, Path.Combine(toDir, single.Replace(Path.Combine(SavePath, fromUUID), "").Substring(1)));
            }
        }
    }
}