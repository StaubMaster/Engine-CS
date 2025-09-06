using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine3D.Miscellaneous
{
    /*
did a lot.
changed how Text works.
started working on a GraphicsManager. redid Uniform again.
also made an EntryContainer for Instances. but I dont want to copy Trans Data from a Class into a Struct every time. so now I want to change Abstract3D thing to Structs
     */
    public class FixedEntryContainer<T>
    {
        public class Entry
        {
            private FixedEntryContainer<T> Data;
            public int EntryIndex;

            public int Offset;
            public int Length;

            public Entry(FixedEntryContainer<T> data, int idx, int off, int len)
            {
                Data = data;
                EntryIndex = idx;
                Offset = off;
                Length = len;
            }

            public T this[int idx]
            {
                get
                {
                    if (idx < 0 || idx > Length) { throw new IndexOutOfRangeException(); }
                    return Data.Data[idx + Offset];
                }
                set
                {
                    if (idx < 0 || idx > Length) { throw new IndexOutOfRangeException(); }
                    Data.Data[idx + Offset] = value;
                }
            }

            public bool IsValid()
            {
                return (Data != null);
            }

            public void Free(bool debug = false)
            {
                if (debug) { ConsoleLog.Log("Entry.IsValid(): " + IsValid()); }
                if (IsValid())
                {
                    Data.Free(EntryIndex, debug);
                    Data = null;
                    EntryIndex = -1;
                }
            }
        }

        public List<Entry> EntryRefs;

        public T[] Data;
        public int Length;

        public FixedEntryContainer()
        {
            EntryRefs = new List<Entry>();

            Data = new T[0];
            Length = 0;
        }

        private void CopyNewSize(int limit)
        {
            T[] data = new T[limit];

            for (int i = 0; i < Length; i++)
            {
                data[i] = Data[i];
            }

            Data = data;
        }
        private void Grow(int size)
        {
            if (Length + size > Data.Length)
            {
                CopyNewSize(Length + size);
            }
        }
        public void Trim()
        {
            CopyNewSize(Length);
        }

        private void Free(int idx, bool debug = false)
        {
            if (idx < 0 || idx >= EntryRefs.Count)
            {
                ConsoleLog.LogError("EntryCollection: Index " + idx + " out of Range.");
                return;
            }

            if (debug) { ConsoleLog.Log("EntryRefs.Count: " + EntryRefs.Count); }

            Entry entry = EntryRefs[idx];
            EntryRefs.RemoveAt(idx);

            if (debug) { ConsoleLog.Log("EntryRefs.Count: " + EntryRefs.Count); }

            int offset = 0;
            for (int i = 0; i < EntryRefs.Count; i++)
            {
                EntryRefs[i].Offset = offset;
                offset += EntryRefs[i].Length;
                EntryRefs[i].EntryIndex = i;
            }

            //for (int i = entry.Offset + entry.Length; i < Length; i++)
            //{
            //    Data[i - entry.Length] = Data[i];
            //}

            if (debug) { ConsoleLog.Log("Length 0: " + Length); }

            int off0 = entry.Offset;
            int off1 = entry.Length + off0;
            while (off1 < Length)
            {
                Data[off0] = Data[off1];
                off0++;
                off1++;
            }
            Length = off0;

            if (debug) { ConsoleLog.Log("Length 1: " + Length); }
        }
        public Entry Alloc(int size)
        {
            Grow(size);
            Entry entry = new Entry(this, EntryRefs.Count, Length, size);
            Length += size;
            EntryRefs.Add(entry);
            return entry;
        }
    }
}
