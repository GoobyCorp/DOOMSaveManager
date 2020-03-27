using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOOMSaveManager
{
    public enum DoomEternalSavePlatform
    {
        BethesdaNet,
        Steam,
    }

    public class DoomEternalSavePath
    {
        public string Identifier;
        public string BasePath;
        public DoomEternalSavePlatform Platform;
        public bool Encrypted = true;

        public DoomEternalSavePath(string id, string path, DoomEternalSavePlatform platform) {
            Identifier = id;
            BasePath = path;
            Platform = platform;
            if (id == "savegame.unencrypted")
                Encrypted = false;
        }
    }
}
