using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DOOMSaveManager
{
    public class DoomEternalSavePathCollection : IList<DoomEternalSavePath>
    {
        private List<DoomEternalSavePath> Paths = new List<DoomEternalSavePath>();

        public int Count => Paths.Count;

        public bool IsReadOnly => false;

        public DoomEternalSavePathCollection() { }

        public DoomEternalSavePathCollection(IEnumerable<DoomEternalSavePath> paths) => Paths.AddRange(paths);

        public DoomEternalSavePath this[int index] {
            get {
                return Paths[index];
            }
            set {
                Paths[index] = value;
            }
        }

        public int IndexOf(DoomEternalSavePath path) => Paths.IndexOf(path);

        public void Insert(int index, DoomEternalSavePath path) => Paths.Insert(index, path);

        public void RemoveAt(int index) => Paths.RemoveAt(index);

        public void Add(DoomEternalSavePath path) => Paths.Add(path);

        public void Clear() => Paths.Clear();

        public bool Contains(DoomEternalSavePath path) => Paths.Contains(path);

        public void CopyTo(DoomEternalSavePath[] array, int index) => Paths.CopyTo(array, index);

        public bool Remove(DoomEternalSavePath path) => Paths.Remove(path);

        public IEnumerator<DoomEternalSavePath> GetEnumerator() => Paths.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public DoomEternalSavePath Get(int index) => Paths[index];

        public bool SaveExists(string id, out DoomEternalSavePath save) {
            foreach(var single in Paths) {
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

        public string[] GetIdentifiers() => Paths.Select(single => single.Identifier).ToArray();
    }
}
