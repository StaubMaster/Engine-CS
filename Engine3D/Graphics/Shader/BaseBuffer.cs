using Engine3D.Graphics.Telematry;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.Graphics.Shader
{
    public abstract class BaseBuffer
    {
        private readonly int Array;
        public TelematryBuffer Telematry;

        protected BaseBuffer()
        {
            Array = GL.GenVertexArray();
        }
        ~BaseBuffer()
        {
            GL.DeleteVertexArray(Array);
        }

        public void Use()
        {
            GL.BindVertexArray(Array);
        }

        public abstract void Draw();
    }
}
