
using Engine3D.Entity;
using Engine3D.Abstract3D;

namespace Engine3D.Graphics.Display3D
{
    public class PHEI_Array
    {
        private PolyHedraInstance_3D_BufferData[] Bodys;

        public PHEI_Array(PolyHedraInstance_3D_BufferData[] bodys)
        {
            Bodys = bodys;
        }
        public PHEI_Array(PolyHedra[] bodys)
        {
            Bodys = new PolyHedraInstance_3D_BufferData[bodys.Length];
            for (int i = 0; i < bodys.Length; i++)
            {
                Bodys[i] = new PolyHedraInstance_3D_BufferData(bodys[i]);
            }
        }
        public PHEI_Array(BodyStatic[] bodys)
        {
            Bodys = new PolyHedraInstance_3D_BufferData[bodys.Length];
            for (int i = 0; i < bodys.Length; i++)
            {
                Bodys[i] = new PolyHedraInstance_3D_BufferData(bodys[i].ToPolyHedra());
            }
        }

        public int Length { get { return Bodys.Length; } }

        public PolyHedraInstance_3D_BufferData this[uint idx]
        {
            get { return Bodys[idx]; }
        }
        public PolyHedraInstance_3D_BufferData this[int idx]
        {
            get { return Bodys[idx]; }
        }

        public void Update()
        {
            for (int i = 0; i < Bodys.Length; i++)
            {
                Bodys[i].DataUpdate();
            }
        }
        public void Draw()
        {
            for (int i = 0; i < Bodys.Length; i++)
            {
                Bodys[i].DrawInst();
            }
        }
    }
}
