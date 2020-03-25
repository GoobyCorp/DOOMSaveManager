using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOOMSaveManager
{
    public class DoomEternalSaveCollection
    {
        private List<DoomEternalSave> Saves = new List<DoomEternalSave>();

        public DoomEternalSaveCollection() {

        }

        public DoomEternalSaveCollection(IEnumerable<DoomEternalSave> saves) {
            Saves.AddRange(saves);
        }

        public void Add(DoomEternalSave save) {
            Saves.Add(save);
        }

        public void AddRange(IEnumerable<DoomEternalSave> saves) {
            Saves.AddRange(saves);
        }

        public bool SaveExists(string id, out DoomEternalSave save) {
            foreach(var single in Saves) {
                if (single.Identifier == id) {
                    save = single;
                    return true;
                }
            }
            save = null;
            return false;
        }

        public DoomEternalSave GetSave(string id) {
            DoomEternalSave save;
            SaveExists(id, out save);
            return save;
        }

        public string[] GetIdentifiers() {
            return Saves.Select(single => single.Identifier).ToArray();
        }
    }
}
