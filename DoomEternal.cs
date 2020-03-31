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
            if (Directory.Exists(SteamSavePath)) {
                foreach (var steamId3 in Directory.GetDirectories(SteamSavePath, "*.*", SearchOption.TopDirectoryOnly)) {
                    foreach (var single in Directory.GetDirectories(steamId3, "*.*", SearchOption.TopDirectoryOnly)) {
                        if (Path.GetFileNameWithoutExtension(single) == SteamGameID.ToString())
                            Saves.Add(new DoomEternalSavePath(Utilities.Id3ToId64(Path.GetFileNameWithoutExtension(steamId3)), DoomEternalSavePlatform.Steam));
                    }
                }
            }
        }

		public static byte[] FarmHash_Hash32(byte[] data) {
			uint size = (uint)data.Length;
			if (size > 0x18) {
				uint v3 = 5 * (Utilities.rotl(0x1B873593 * Utilities.rotl(0xCC9E2D51 * BitConverter.ToUInt32(data, (int)(size - 0x10)), 0xF) ^ 5 * (Utilities.rotl(size ^ 0x1B873593 * Utilities.rotl(0xCC9E2D51 * BitConverter.ToUInt32(data, (int)(size - 4)), 0xF), 0xD) - 0x52250EC), 0xD) - 0x52250EC);
				uint v4 = 5 * (Utilities.rotl(0x1B873593 * Utilities.rotl(0xCC9E2D51 * BitConverter.ToUInt32(data, (int)(size - 0xC)), 0xF) ^ 5 * (Utilities.rotl(0xCC9E2D51 * size ^ 0x1B873593 * Utilities.rotl(0xCC9E2D51 * BitConverter.ToUInt32(data, (int)(size - 8)), 0xF), 0xD) - 0x52250EC), 0xD) - 0x52250EC);

				uint v5 = (size - 1) / 20;

				uint v6 = Utilities.rotl(0xCC9E2D51 * size + 0x1B873593 * Utilities.rotl(0xCC9E2D51 * BitConverter.ToUInt32(data, (int)(size - 0x14)), 0xF), 0xD) + 0x71;

				int offset = 8;
				for (uint i = v5; i > 0; i--) {
					uint v8 = BitConverter.ToUInt32(data, offset + 4);
					uint v9 = BitConverter.ToUInt32(data, offset - 8);
					uint v10 = BitConverter.ToUInt32(data, offset);
					uint v11 = BitConverter.ToUInt32(data, offset + 8);
					uint v12 = BitConverter.ToUInt32(data, offset - 4);
					offset += 20;
					uint v13 = Utilities.rotl((v9 + v3) ^ 0x1B873593 * Utilities.rotl(0xCC9E2D51 * v8, 0xF), 0xD);
					v3 = v13 - 0x19AB949C + v11 + 4 * v13;
					uint v14 = Utilities.rotl((v12 + v4) ^ 0x1B873593 * Utilities.rotl(0xCC9E2D51 * v10, 0xF), 0xD);
					uint v15 = v14 - 0x19AB949C + v9 + 4 * v14;
					uint v16 = Utilities.rotl((v10 + v6) ^ 0x1B873593 * Utilities.rotl(0x100193A1 * v11 - 0x3361D2AF * v12, 0xF), 0xD);
					v6 = v15 + v16 + v8 + 4 * (v16 - 0x66AE527);
					v4 = v6 + v15;
				}

				uint cksm = 0xCC9E2D51 * Utilities.rotl(5 * Utilities.rotl(0xCC9E2D51 * (Utilities.rotl(0xCC9E2D51 * Utilities.rotr(v6, 0xB), 0xF) + Utilities.rotl(5 * (Utilities.rotl(v3 - 0x3361D2AF * Utilities.rotl(0xCC9E2D51 * Utilities.rotr(v4, 0xB), 0xF), 0xD) - 0x52250EC), 0xF)), 0xD) - 0x19AB949C, 0xF);
				byte[] cksmBytes = BitConverter.GetBytes(cksm);
				Array.Reverse(cksmBytes);
				return cksmBytes;
			} else if (size > 0xC)
				return null;
			else if (size > 4)
				return null;
			else
				return null;
		}
	}
}
