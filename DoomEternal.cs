using System;
using System.Linq;

using System.IO;
using System.Collections.Generic;
using System.Reflection.Emit;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto.Engines;

namespace DOOMSaveManager
{
    public class DoomEternal
    {
        public const string GameName = "Doom Eternal";

        public const int SteamGameID = 782330;
        public static string SteamSavePath = "";
        public static string BnetSavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Saved Games", "id Software", "DOOMEternal", "base", "savegame");

        public static DoomEternalSavePathCollection Saves;

        public static void EnumerateSaves() {
            Saves = new DoomEternalSavePathCollection();

            if (Directory.Exists(BnetSavePath)) {
                Saves.Add(new DoomEternalSavePath("savegame.unencrypted", DoomEternalSavePlatform.BethesdaNet, false));
                foreach (var single in Directory.GetDirectories(BnetSavePath, "*.*", SearchOption.TopDirectoryOnly)) {
                    if (Utilities.CheckUUID(Path.GetFileNameWithoutExtension(single)))
                        Saves.Add(new DoomEternalSavePath(Path.GetFileNameWithoutExtension(single), DoomEternalSavePlatform.BethesdaNet));
                }
            }

            string steamPath = Utilities.GetSteamPath();
            if (!string.IsNullOrEmpty(steamPath)) {
                SteamSavePath = Path.Combine(steamPath, "userdata");
                if (Directory.Exists(SteamSavePath)) {
                    foreach (var steamId3 in Directory.GetDirectories(SteamSavePath, "*.*", SearchOption.TopDirectoryOnly)) {
                        foreach (var single in Directory.GetDirectories(steamId3, "*.*", SearchOption.TopDirectoryOnly)) {
                            if (Path.GetFileNameWithoutExtension(single) == SteamGameID.ToString())
                                Saves.Add(new DoomEternalSavePath(Utilities.Id3ToId64(Path.GetFileNameWithoutExtension(steamId3)), DoomEternalSavePlatform.Steam));
                        }
                    }
                }
            }
        }
	}
}
