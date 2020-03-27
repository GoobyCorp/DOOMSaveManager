using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOOMSaveManager
{
    public class DoomEternalSavePathCollection
    {
        private List<DoomEternalSavePath> Saves = new List<DoomEternalSavePath>();

        public DoomEternalSavePathCollection() {

        }

        public DoomEternalSavePathCollection(IEnumerable<DoomEternalSavePath> saves) {
            Saves.AddRange(saves);
        }

        public void Add(DoomEternalSavePath save) {
            Saves.Add(save);
        }

        public void AddRange(IEnumerable<DoomEternalSavePath> saves) {
            Saves.AddRange(saves);
        }

        public bool SaveExists(string id, out DoomEternalSavePath save) {
            foreach(var single in Saves) {
                if (single.Identifier == id) {
                    save = single;
                    return true;
                }
            }
            save = null;
            return false;
        }

        public DoomEternalSavePath GetSave(string id) {
            DoomEternalSavePath save;
            SaveExists(id, out save);
            return save;
        }

        public string[] GetIdentifiers() {
            return Saves.Select(single => single.Identifier).ToArray();
        }
    }
}
