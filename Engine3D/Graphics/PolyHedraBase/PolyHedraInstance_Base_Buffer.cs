using Engine3D.Abstract3D;
using Engine3D.Graphics.Shader;
using Engine3D.DataStructs;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.Graphics.PolyHedraBase
{
    public abstract class PolyHedraInstance_Base_Buffer<InstanceDataType> : PolyHedra_Base_Buffer
    {
        protected readonly int InstBuffer;
        protected int InstCount;

        public PolyHedraInstance_Base_Buffer() : base()
        {
            InstBuffer = GL.GenBuffer();
            InstCount = 0;
        }
        ~PolyHedraInstance_Base_Buffer()
        {
            Use();

            GL.DeleteBuffer(InstBuffer);
        }



        public abstract void Bind_Inst(InstanceDataType[] data, int len);



        public override void Draw_Inst()
        {
            Use();
            GL.BindTexture(TextureTarget.Texture1DArray, TextureArray);
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, MainCount, InstCount);
        }
    }
}
