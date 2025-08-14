using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine3D
{
    [Obsolete("Not used in newer Versions", false)]
    public class Cache<I, O>
    {
        private struct Entry
        {
            public bool Loaded;
            public string Name;
            public I InnPut;
            public O OutPut;

            public Entry(string name, I innput)
            {
                Loaded = false;
                Name = name;

                InnPut = innput;
                OutPut = default(O);
            }
            public Entry(string name, O output)
            {
                Loaded = true;
                Name = name;

                InnPut = default(I);
                OutPut = output;
            }
        }

        private List<Entry> Entrys;
        public int Length
        {
            get { return Entrys.Count; }
        }

        public Cache()
        {
            Entrys = new List<Entry>();
        }

        public void Insert(string name, I innput)
        {
            Entrys.Add(new Entry(name, innput));
        }
        public void Insert(string name, O output)
        {
            Entrys.Add(new Entry(name, output));
        }

        //  not used
        public void Insert(I innput)
        {
            Entrys.Add(new Entry(null, innput));
        }
        public void Insert(O output)
        {
            Entrys.Add(new Entry(null, output));
        }

        public void FuncAllIO(Func<I, O> func)
        {
            ConsoleLog.Log("Cache Func IO");
            Entry entry;
            for (int i = 0; i < Entrys.Count; i++)
            {
                entry = Entrys[i];
                if (entry.Loaded == false)
                {
                    entry.OutPut = func(entry.InnPut);
                    entry.Loaded = true;
                    Entrys[i] = entry;
                }
            }
            ConsoleLog.Log("");
        }
        public void FuncAllO(Action<O> func)
        {
            ConsoleLog.Log("Cache Func O");
            Entry entry;
            for (int i = 0; i < Entrys.Count; i++)
            {
                entry = Entrys[i];
                if (entry.OutPut != null)
                    func(entry.OutPut);
                Entrys[i] = entry;
            }
            ConsoleLog.Log("");
        }


        public int NameToIdx(string name)
        {
            for (int i = 0; i < Entrys.Count; i++)
            {
                if (Entrys[i].Name == name)
                {
                    return (i);
                }
            }
            return (-1);
        }
        public string IdxToName(int idx)
        {
            if (idx >= 0 && idx < Entrys.Count)
            {
                return Entrys[idx].Name;
            }
            return null;
        }


        public O GetOut(int idx)
        {
            if (idx >= 0 && idx < Entrys.Count)
                return Entrys[idx].OutPut;
            return default(O);
        }
        public O GetOut(string name)
        {
            return GetOut(NameToIdx(name));
        }

        //  not used
        public void SetOut(int idx, O output)
        {
            if (idx >= 0 && idx < Entrys.Count)
            {
                Entry ent = Entrys[idx];
                ent.OutPut = output;
                Entrys[idx] = ent;
            }
        }
        public void SetOut(string name, O output)
        {
            SetOut(NameToIdx(name), output);
        }

        //  not used
        public O[] Out_Arr()
        {
            O[] arr = new O[Entrys.Count];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = Entrys[i].OutPut;
            }
            return arr;
        }
    }
}
