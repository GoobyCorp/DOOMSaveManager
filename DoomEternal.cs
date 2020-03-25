using System;
using System.Linq;

using System.IO;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace DOOMSaveManager
{
    public class DoomEternal
    {
        public const string GameName = "Doom Eternal";

        public const int SteamGameID = 782330;
        public static string SteamSavePath = Path.Combine(Utilities.GetSteamPath(), "userdata");
        public static string BnetSavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Saved Games", "id Software", "DOOMEternal", "base", "savegame");

        public const string BackupPassword = "doometernalbackup";

        public static DoomEternalSaveCollection Saves;

        public static void EnumerateSaves() {
            Saves = new DoomEternalSaveCollection();
            if (Directory.Exists(BnetSavePath)) {
                foreach (var single in Directory.GetDirectories(BnetSavePath, "*.*", SearchOption.TopDirectoryOnly)) {
                    if (Utilities.CheckUUID(Path.GetFileNameWithoutExtension(single)))
                        Saves.Add(new DoomEternalSave(Path.GetFileNameWithoutExtension(single), BnetSavePath, DoomEternalSavePlatform.BethesdaNet));
                }
            }
            if (Directory.Exists(SteamSavePath)) {
                foreach (var steamId3 in Directory.GetDirectories(SteamSavePath, "*.*", SearchOption.TopDirectoryOnly)) {
                    Console.WriteLine(Path.GetFileNameWithoutExtension(steamId3));
                    foreach (var single in Directory.GetDirectories(steamId3, "*.*", SearchOption.TopDirectoryOnly)) {
                        if (Path.GetFileNameWithoutExtension(single) == SteamGameID.ToString())
                            Saves.Add(new DoomEternalSave((int.Parse(Path.GetFileNameWithoutExtension(steamId3)) + 76561197960265728).ToString(), SteamSavePath, DoomEternalSavePlatform.Steam));
                    }
                }
            }
        }

        #region Bethesda.net
        public static void BnetFileEncrypt(string fromFile, string toFile, string toUUID) {
            if (fromFile.EndsWith("-BACKUP"))
                return;
            byte[] fromFileData = File.ReadAllBytes(fromFile);
            Directory.CreateDirectory(Path.GetDirectoryName(toFile));
            File.WriteAllBytes(toFile, Crypto.EncryptAndDigest($"{toUUID}PAINELEMENTAL{Path.GetFileName(toFile)}", fromFileData));
        }

        public static void BnetFileDecrypt(string fromFile, string fromUUID, string toFile) {
            if (fromFile.EndsWith("-BACKUP"))
                return;
            byte[] fromFileData = Crypto.DecryptAndVerify($"{fromUUID}PAINELEMENTAL{Path.GetFileName(fromFile)}", File.ReadAllBytes(fromFile));
            Directory.CreateDirectory(Path.GetDirectoryName(toFile));
            File.WriteAllBytes(toFile, fromFileData);
        }

        public static void BnetFileTransfer(string fromFile, string fromUUID, string toFile, string toUUID) {
            if (fromFile.EndsWith("-BACKUP"))
                return;
            byte[] fromFileData = Crypto.DecryptAndVerify($"{fromUUID}PAINELEMENTAL{Path.GetFileName(fromFile)}", File.ReadAllBytes(fromFile));
            Directory.CreateDirectory(Path.GetDirectoryName(toFile));
            File.WriteAllBytes(toFile, Crypto.EncryptAndDigest($"{toUUID}PAINELEMENTAL{Path.GetFileName(toFile)}", fromFileData));
        }

        public static void BnetBulkTransfer(string fromUUID, string toUUID) {
            string fromDir = Path.Combine(BnetSavePath, fromUUID);
            foreach(var single in Directory.GetFiles(fromDir, "*.*", SearchOption.AllDirectories)) {
                BnetFileTransfer(single, fromUUID, single.Replace(fromUUID, toUUID), toUUID);
            }
        }

        public static void BnetBulkEncrypt(string fromDir, string toUUID) {
            string toDir = Path.Combine(BnetSavePath, toUUID);
            foreach (var single in Directory.GetFiles(fromDir, "*.*", SearchOption.AllDirectories)) {
                BnetFileEncrypt(single, Path.Combine(toDir, single.Replace(fromDir, "").Substring(1)), toUUID);
            }
        }

        public static void BnetBulkDecrypt(string fromUUID, string toDir) {
            string fromDir = Path.Combine(BnetSavePath, fromUUID);
            foreach (var single in Directory.GetFiles(fromDir, "*.*", SearchOption.AllDirectories)) {
                BnetFileDecrypt(single, fromUUID, Path.Combine(toDir, single.Replace(Path.Combine(BnetSavePath, fromUUID), "").Substring(1)));
            }
        }
        #endregion

        #region Steam
        public static void SteamFileEncrypt(string fromFile, string toFile, string toId) {
            //toId = Utilities.Id64ToId3(toId);
            if (fromFile.EndsWith("-BACKUP"))
                return;
            byte[] fromFileData = File.ReadAllBytes(fromFile);
            Directory.CreateDirectory(Path.GetDirectoryName(toFile));
            File.WriteAllBytes(toFile, Crypto.EncryptAndDigest($"{toId}MANCUBUS{Path.GetFileName(toFile)}", fromFileData));
        }

        public static void SteamFileDecrypt(string fromFile, string fromId, string toFile) {
            //fromId = Utilities.Id64ToId3(fromId);
            if (fromFile.EndsWith("-BACKUP"))
                return;
            byte[] fromFileData = Crypto.DecryptAndVerify($"{fromId}MANCUBUS{Path.GetFileName(fromFile)}", File.ReadAllBytes(fromFile));
            Directory.CreateDirectory(Path.GetDirectoryName(toFile));
            File.WriteAllBytes(toFile, fromFileData);
        }

        public static void SteamFileTransfer(string fromFile, string fromId, string toFile, string toId) {
            fromId = Utilities.Id64ToId3(fromId);
            toId = Utilities.Id64ToId3(toId);
            if (fromFile.EndsWith("-BACKUP"))
                return;
            byte[] fromFileData = Crypto.DecryptAndVerify($"{Utilities.Id3ToId64(fromId)}MANCUBUS{Path.GetFileName(fromFile)}", File.ReadAllBytes(fromFile));
            Directory.CreateDirectory(Path.GetDirectoryName(toFile));
            File.WriteAllBytes(toFile, Crypto.EncryptAndDigest($"{Utilities.Id3ToId64(toId)}MANCUBUS{Path.GetFileName(toFile)}", fromFileData));
        }

        public static void SteamBulkTransfer(string fromId, string toId) {
            fromId = Utilities.Id64ToId3(fromId);
            toId = Utilities.Id64ToId3(toId);
            string fromDir = Path.Combine(BnetSavePath, fromId);
            foreach (var single in Directory.GetFiles(fromDir, "*.*", SearchOption.AllDirectories)) {
                SteamFileTransfer(single, fromId, single.Replace(fromId, toId), toId);
            }
        }

        public static void SteamBulkEncrypt(string fromDir, string toId) {
            toId = Utilities.Id64ToId3(toId);
            string toDir = Path.Combine(SteamSavePath, toId, SteamGameID.ToString(), "remote");
            foreach (var single in Directory.GetFiles(fromDir, "*.*", SearchOption.AllDirectories)) {
                SteamFileEncrypt(single, Path.Combine(toDir, single.Replace(fromDir, "").Substring(1)), Utilities.Id3ToId64(toId));
            }
        }

        public static void SteamBulkDecrypt(string fromId, string toDir) {
            fromId = Utilities.Id64ToId3(fromId);
            string fromDir = Path.Combine(SteamSavePath, fromId, SteamGameID.ToString(), "remote");
            foreach (var single in Directory.GetFiles(fromDir, "*.*", SearchOption.AllDirectories)) {
                SteamFileDecrypt(single, Utilities.Id3ToId64(fromId), Path.Combine(toDir, single.Replace(Path.Combine(SteamSavePath, fromId, SteamGameID.ToString(), "remote"), "").Substring(1)));
            }
        }
        #endregion

        #region Both
        public static void BnetToSteamTransfer(string fromId, string toId) {
            Directory.CreateDirectory("tmp");
            BnetBulkDecrypt(fromId, "tmp");
            SteamBulkEncrypt("tmp", toId);
            Directory.Delete("tmp", true);
        }

        public static void SteamToBnetTransfer(string fromId, string toId) {
            Directory.CreateDirectory("tmp");
            SteamBulkDecrypt(fromId, "tmp");
            BnetBulkEncrypt("tmp", toId);
            Directory.Delete("tmp", true);
        }
        #endregion
    }
}