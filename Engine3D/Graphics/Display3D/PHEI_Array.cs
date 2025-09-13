
using Engine3D.Entity;
using Engine3D.Abstract3D;
using Engine3D.Graphics.PolyHedraBase;

namespace Engine3D.Graphics.Display3D
{
    public class PHEI_Array : PolyHedraInstance_Base_Array<PolyHedraInstance_3D_BufferData, PolyHedraInstance_3D_Buffer, PolyHedraInstance_3D_Data>
    {
        public PHEI_Array(PolyHedraInstance_3D_BufferData[] array) : base(array)
        {

        }
        public PHEI_Array(PolyHedra[] bodys) : base()
        {
            Array = new PolyHedraInstance_3D_BufferData[bodys.Length];
            for (int i = 0; i < bodys.Length; i++)
            {
                Array[i] = new PolyHedraInstance_3D_BufferData(bodys[i]);
            }
        }
        public PHEI_Array(BodyStatic[] bodys) : base()
        {
            Array = new PolyHedraInstance_3D_BufferData[bodys.Length];
            for (int i = 0; i < bodys.Length; i++)
            {
                Array[i] = new PolyHedraInstance_3D_BufferData(bodys[i].ToPolyHedra());
            }
        }
    }
}
