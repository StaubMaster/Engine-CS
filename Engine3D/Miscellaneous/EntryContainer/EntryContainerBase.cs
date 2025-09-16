using System;
using System.Collections.Generic;

namespace Engine3D.Miscellaneous.EntryContainer
{
    public abstract class EntryContainerBase<T>
    {
        public class Entry
        {
            private EntryContainerBase<T> Container;
            public int EntryIndex;

            public int Offset;
            public int Length;

            public Entry(EntryContainerBase<T> container, int entry_idx, int off, int len)
            {
                Container = container;
                EntryIndex = entry_idx;
                Offset = off;
                Length = len;
            }
            protected Entry(Entry other)
            {
                Container = other.Container;
                EntryIndex = other.EntryIndex;
                Offset = other.Offset;
                Length = other.Length;
            }

            public T this[int idx]
            {
                get
                {
                    if (idx < 0 || idx > Length) { throw new IndexOutOfRangeException(); }
                    return Container.Data[idx + Offset];
                }
                set
                {
                    if (idx < 0 || idx > Length) { throw new IndexOutOfRangeException(); }
                    Container.Data[idx + Offset] = value;
                    Container.DataChanged = true;
                }
            }

            public bool IsValid()
            {
                return (Container != null);
            }
            public void UnReference()
            {
                Container = null;
                EntryIndex = -1;
            }

            public void Dispose(bool debug = false)
            {
                if (debug) { ConsoleLog.Log("Entry.IsValid(): " + IsValid()); }
                if (IsValid())
                {
                    Container.Free(EntryIndex, debug);
                    UnReference();
                }
            }
        }

        public List<Entry> EntryRefs;
        public T[] Data;

        public bool DataChanged;

        public EntryContainerBase(int size)
        {
            EntryRefs = new List<Entry>();

            Data = new T[size];

            DataChanged = false;
        }
        ~EntryContainerBase()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (EntryRefs != null)
            {
                for (int i = 0; i < EntryRefs.Count; i++)
                {
                    EntryRefs[i].UnReference();
                }
            }
        }

        protected abstract void RemapEntrys();

        protected abstract void Free(int idx, bool debug = false);
        protected Entry Alloc(int off, int len)
        {
            Entry entry = new Entry(this, EntryRefs.Count, off, len);
            EntryRefs.Add(entry);
            DataChanged = true;
            return entry;
        }

        public abstract Entry Alloc(int size);
    }
}
