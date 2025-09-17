
using Engine3D.Entity;
using Engine3D.Abstract3D;
using Engine3D.Graphics.PolyHedraBase;
using Engine3D.Miscellaneous.EntryContainer;

namespace Engine3D.Graphics.Display3D
{
    public class PolyHedraInstance_3D_Array : PolyHedraInstance_Base_Array<PolyHedraInstance_3D_BufferData, PolyHedraInstance_3D_Buffer, PolyHedraInstance_3D_Data>
    {
        public class Entry : EntryContainerBase<PolyHedraInstance_3D_Data>.Entry
        {
            public PolyHedraInstance_3D_BufferData BufferData;

            public Entry(EntryContainerBase<PolyHedraInstance_3D_Data>.Entry entry, PolyHedraInstance_3D_BufferData buffer_data) : base(entry)
            {
                BufferData = buffer_data;
            }

            public Intersekt.RayInterval Intersekt(Ray3D ray, int idx)
            {
                return BufferData.Intersekt(ray, this[idx].Trans);
            }
        }



        public PolyHedraInstance_3D_Array(PolyHedraInstance_3D_BufferData[] array) : base(array)
        {

        }
        public PolyHedraInstance_3D_Array(PolyHedra[] bodys) : base()
        {
            Array = new PolyHedraInstance_3D_BufferData[bodys.Length];
            for (int i = 0; i < bodys.Length; i++)
            {
                Array[i] = new PolyHedraInstance_3D_BufferData(bodys[i]);
            }
        }
        public PolyHedraInstance_3D_Array(BodyStatic[] bodys) : base()
        {
            Array = new PolyHedraInstance_3D_BufferData[bodys.Length];
            for (int i = 0; i < bodys.Length; i++)
            {
                Array[i] = new PolyHedraInstance_3D_BufferData(bodys[i].ToPolyHedra());
            }
        }



        public Entry Alloc(int idx, int size)
        {
            EntryContainerBase<PolyHedraInstance_3D_Data>.Entry entry = Array[idx].Alloc(size);
            if (entry == null) { return null; }
            return new Entry(entry, Array[idx]);
        }
    }
}
