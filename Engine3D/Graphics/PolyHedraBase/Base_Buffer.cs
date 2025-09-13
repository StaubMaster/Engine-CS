
using OpenTK.Graphics.OpenGL4;

namespace Engine3D.Graphics.PolyHedraBase
{
    public abstract class Base_Buffer
    {
        private readonly int BufferArray;

        protected Base_Buffer()
        {
            BufferArray = GL.GenVertexArray();
        }
        ~Base_Buffer()
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
