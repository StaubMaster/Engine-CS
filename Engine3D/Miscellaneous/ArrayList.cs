using System;
using System.Collections.Generic;

namespace Engine3D.Miscellaneous
{
    public class ArrayList<T>
    {
        private T[] Arr;
        private List<T> Lst;

        public int Count
        {
            get
            {
                if (Arr != null) { return Arr.Length; }
                if (Lst != null) { return Lst.Count; }
                return -1;
            }
        }

        public ArrayList()
        {
            Arr = new T[0];
            Lst = null;
        }
        public ArrayList(int len)
        {
            Arr = new T[len];
            Lst = null;
        }

        public T this[int idx]
        {
            get
            {
                if (Arr != null) { return Arr[idx]; }
                if (Lst != null) { return Lst[idx]; }
                return default;
            }
            set
            {
                if (Arr != null) { Arr[idx] = value; }
                if (Lst != null) { Lst[idx] = value; }
            }
        }
        public T this[uint idx]
        {
            get
            {
                if (Arr != null) { return Arr[idx]; }
                if (Lst != null) { return Lst[(int)idx]; }
                return default;
            }
            set
            {
                if (Arr != null) { Arr[idx] = value; }
                if (Lst != null) { Lst[(int)idx] = value; }
            }
        }

        public int Insert(T item)
        {
            int idx = Lst.Count;
            Lst.Add(item);
            return idx;
        }
        public T Remove(int idx)
        {
            T item = Lst[idx];
            Lst.RemoveAt(idx);
            return item;
        }

        public void EditBegin()
        {
            if (Lst == null)
            {
                Lst = new List<T>(Arr);
                Arr = null;
            }
        }
        public void EditEnd()
        {
            if (Arr == null)
            {
                Arr = Lst.ToArray();
                Lst = null;
            }
        }

        public T[] ToArray()
        {
            if (Arr != null) { return Arr; }
            if (Lst != null) { return Lst.ToArray(); }
            return null;
        }
    }
    public class ENotEdit : Exception
    {
        public ENotEdit() : base("Not in Edit Mode.") { }
    }
}
