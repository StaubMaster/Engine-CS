
namespace Engine3D.Miscellaneous.EntryContainer
{
    public class EntryContainerFixed<T> : EntryContainerBase<T>
    {
        private readonly int TSize;

        public EntryContainerFixed(int Size, int tSize) : base(Size)
        {
            TSize = tSize;
        }

        private bool IsFree(int off, int len)
        {
            int end = off + len;

            if (end >= Data.Length) { return false; }

            for (int i = 0; i < EntryRefs.Count; i++)
            {
                Entry ent = EntryRefs[i];
                int o = ent.Offset;
                int l = ent.Length;
                int e = o + l;

                if (off < e && end > o)
                {
                    return false;
                }
            }

            return true;
        }

        protected override void RemapEntrys()
        {
            for (int i = 0; i < EntryRefs.Count; i++)
            {
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

            EntryRefs.RemoveAt(idx);

            RemapEntrys();
        }

        public override Entry Alloc(int size)
        {
            if (IsFree(0, size)) { return Alloc(0, size); }
            for (int i = 0; i < EntryRefs.Count; i++)
            {
                Entry entry = EntryRefs[i];
                int end = entry.Offset + entry.Length;

                if (IsFree(end, size))
                {
                    return Alloc(end, size);
                }
            }

            /* at this point: reorder the other things
             * then check if it fits ?
             * sum up all empty space to check if it will even fit
             */

            return null;
        }

        public string ToInfo()
        {
            int used = 0;
            for (int i = 0; i < EntryRefs.Count; i++)
            {
                used += EntryRefs[i].Length;
            }
            int total = Data.Length;
            double perc = (1.0 * used) / total;

            string str = "";
            str += UnitToString.Memory1000(used) + "/" + UnitToString.Memory1000(total) + "(" + perc + ")" + ":Used";
            return str;
        }




        public static void Test()
        {
            EntryContainerFixed<int> entCont = new EntryContainerFixed<int>(16, sizeof(int));

            ConsoleLog.Log("Data: " + entCont.Data.Length);
            for (int i = 0; i < entCont.Data.Length; i++) { ConsoleLog.Log("[" + i.ToString("00") + "] " + entCont.Data[i].ToString("00")); }

            EntryContainerFixed<int>.Entry[] ent = new EntryContainerFixed<int>.Entry[4];
            ent[0] = entCont.Alloc(3);
            ent[1] = entCont.Alloc(3);
            ent[2] = entCont.Alloc(3);
            ent[3] = entCont.Alloc(3);

            for (int i = 0; i < ent.Length; i++)
            {
                if (ent[i] != null)
                {
                    for (int j = 0; j < ent[i].Length; j++)
                    {
                        ent[i][j] = i * 10 + j;
                    }
                }
            }

            ConsoleLog.Log("Data: " + entCont.Data.Length);
            for (int i = 0; i < entCont.Data.Length; i++) { ConsoleLog.Log("[" + i.ToString("00") + "] " + entCont.Data[i].ToString("00")); }


        }
    }
}
