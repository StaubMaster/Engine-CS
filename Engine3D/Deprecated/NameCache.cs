using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine3D
{
    [Obsolete("Not used in newer Versions", false)]
    public class NameCache<T>
    {
        private struct Entry
        {
            public string Name;
            public T Item;

            public Entry(string name, T item)
            {
                Name = name;
                Item = item;
            }
        }


        public int Length
        {
            get { return Entry_List.Count; }
        }

        public T this[string name]
        {
            get
            {
                for (int i = 0; i < Entry_List.Count; i++)
                {
                    if (Entry_List[i].Name == name)
                        return Entry_List[i].Item;
                }
                return default(T);
            }
        }


        private List<Entry> Entry_List;
        public NameCache()
        {
            Entry_List = new List<Entry>();
        }

        public void Add(string name, T item)
        {
            Entry_List.Add(new Entry(name, item));
        }
        public void Sub(int idx)
        {
            Entry_List.RemoveAt(idx);
        }
    }
}
