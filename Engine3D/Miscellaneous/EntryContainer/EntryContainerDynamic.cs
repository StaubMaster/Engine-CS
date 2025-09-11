
namespace Engine3D.Miscellaneous.EntryContainer
{
    public class EntryContainerDynamic<T> : EntryContainerBase<T>
    {
        public int Length;

        public EntryContainerDynamic() : base(0)
        {
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

        protected override void RemapEntrys()
        {
            int offset = 0;
            for (int i = 0; i < EntryRefs.Count; i++)
            {
                EntryRefs[i].Offset = offset;
                offset += EntryRefs[i].Length;
                EntryRefs[i].EntryIndex = i;
            }
        }

        protected override void Free(int idx, bool debug = false)
        {
            if (idx < 0 || idx >= EntryRefs.Count)
            {
                ConsoleLog.LogError("EntryCollection: Index " + idx + " out of Range.");
                return;
            }
            DataChanged = true;

            if (debug) { ConsoleLog.Log("EntryRefs.Count: " + EntryRefs.Count); }

            Entry entry = EntryRefs[idx];
            EntryRefs.RemoveAt(idx);

            if (debug) { ConsoleLog.Log("EntryRefs.Count: " + EntryRefs.Count); }

            RemapEntrys();

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
        public override Entry Alloc(int size)
        {
            Grow(size);
            Entry entry = Alloc(Length, size);
            Length += size;
            return entry;
        }





        public static void Test()
        {
            EntryContainerDynamic<int> entCont = new EntryContainerDynamic<int>();

            EntryContainerDynamic<int>.Entry[] ent = new EntryContainerDynamic<int>.Entry[5];
            ent[0] = entCont.Alloc(2);
            ent[1] = entCont.Alloc(1);
            ent[2] = entCont.Alloc(3);
            ent[3] = entCont.Alloc(1);
            ent[4] = entCont.Alloc(3);

            ConsoleLog.Log("Indexe: " + entCont.Length);
            for (int i = 0; i < ent.Length; i++) { ConsoleLog.Log("[" + i.ToString("0") + "] " + ent[i].EntryIndex); }

            ConsoleLog.Log("Data: " + entCont.Length);
            for (int i = 0; i < entCont.Length; i++) { ConsoleLog.Log("[" + i.ToString("00") + "] " + entCont.Data[i]); }

            for (int i = 0; i < ent.Length; i++)
            {
                for (int j = 0; j < ent[i].Length; j++)
                {
                    ent[i][j] = i * 10 + j;
                }
            }

            ConsoleLog.Log("Data: " + entCont.Length);
            for (int i = 0; i < entCont.Length; i++) { ConsoleLog.Log("[" + i.ToString("00") + "] " + entCont.Data[i]); }

            ent[1].Free();

            ConsoleLog.Log("Data: " + entCont.Length);
            for (int i = 0; i < entCont.Length; i++) { ConsoleLog.Log("[" + i.ToString("00") + "] " + entCont.Data[i]); }

            ConsoleLog.Log("Indexe: " + entCont.Length);
            for (int i = 0; i < ent.Length; i++) { ConsoleLog.Log("[" + i.ToString("0") + "] " + ent[i].EntryIndex); }

            ent[2].Free();

            ConsoleLog.Log("Indexe: " + entCont.Length);
            for (int i = 0; i < ent.Length; i++) { ConsoleLog.Log("[" + i.ToString("0") + "] " + ent[i].EntryIndex); }
        }
    }
}
