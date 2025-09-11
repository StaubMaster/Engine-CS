using Engine3D.Abstract3D;
using Engine3D.DataStructs;
using Engine3D.Graphics.Shader;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.Graphics.Display3D
{
    //  PolyHedraElementInstanceBuffer
    public class PHEI_Buffer : PHE_Buffer
    {
        protected readonly int InstBuffer;
        protected int InstCount;

        public PHEI_Buffer() : base()
        {
            InstBuffer = GL.GenBuffer();
            InstCount = 0;
        }
        ~PHEI_Buffer()
        {
            Use();

            GL.DeleteBuffer(InstBuffer);
        }

        public void Bind_Inst_Trans(PolyHedraInstance_3D_Data[] data, int len, bool debug = false)
        {
            Use();

            GL.BindBuffer(BufferTarget.ArrayBuffer, InstBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, len * PolyHedraInstance_3D_Data.Size, data, BufferUsageHint.StreamDraw);

            System.IntPtr offset = System.IntPtr.Zero;
            PolyHedraInstance_3D_Data.ToBuffer(PolyHedraInstance_3D_Data.Size, ref offset, 1, 1, 2, 3, 4, 5, 6);

            InstCount = len;
        }

        public void Draw_Inst(bool debug = false)
        {
            Use();
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, ColorsBuffer);
            GL.DrawElementsInstanced(PrimitiveType.Triangles, ElemCount, DrawElementsType.UnsignedInt, System.IntPtr.Zero, InstCount);
        }

        public override void Draw()
        {
            throw new System.NotImplementedException();
        }
    }
}
