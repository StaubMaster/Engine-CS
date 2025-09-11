using Engine3D.Abstract3D;
using Engine3D.Graphics.Shader;
using Engine3D.DataStructs;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.Graphics.PolyHedraBase
{
    public abstract class PolyHedraInstanceBaseBuffer<InstanceDataType> : PolyHedraBaseBuffer
    {
        protected readonly int InstBuffer;
        protected int InstCount;

        public PolyHedraInstanceBaseBuffer() : base()
        {
            InstBuffer = GL.GenBuffer();
            InstCount = 0;
        }
        ~PolyHedraInstanceBaseBuffer()
        {
            Use();

            GL.DeleteBuffer(InstBuffer);
        }



        public abstract void Bind_Inst(InstanceDataType[] data, int len);



        public override void Draw_Inst()
        {
            Use();
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, ColorsBuffer);
            GL.DrawElementsInstanced(PrimitiveType.Triangles, ElemCount, DrawElementsType.UnsignedInt, System.IntPtr.Zero, InstCount);
        }
    }
}
