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

    public class DoomEternalSave
    {
        public string Identifier;
        public string BasePath;
        public DoomEternalSavePlatform Platform;

        public DoomEternalSave(string id, string path, DoomEternalSavePlatform platform) {
            Identifier = id;
            BasePath = path;
            Platform = platform;
        }
    }
}
