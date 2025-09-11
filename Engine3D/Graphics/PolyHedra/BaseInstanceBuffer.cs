
using OpenTK.Graphics.OpenGL4;

namespace Engine3D.Graphics.PolyHedraBase
{
    public abstract class BaseInstanceBuffer
    {
        private readonly int BufferArray;

        protected BaseInstanceBuffer()
        {
            BufferArray = GL.GenVertexArray();
        }
        ~BaseInstanceBuffer()
        {
            GL.DeleteVertexArray(BufferArray);
        }

        public void Use()
        {
            GL.BindVertexArray(BufferArray);
        }

        public abstract void Draw_Main();
        public abstract void Draw_Inst();
    }
}
