using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.OutPut.Shader
{
    public abstract class ABuffer
    {
        public readonly string Name;

        protected int GL_Array;

        protected ABuffer(string name)
        {
            Name = name;
            GL_Array = GL.GenVertexArray();
        }
        ~ABuffer()
        {
            GL.DeleteVertexArray(GL_Array);
        }

        public virtual void Use()
        {
            GL.BindVertexArray(GL_Array);
        }

        public abstract void Draw();
    }
}
