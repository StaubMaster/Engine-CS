using System;
using System.Collections.Generic;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.GraphicsOld
{
    public abstract class RenderBuffers
    {
        protected int Buffer_Array;

        protected RenderBuffers()
        {
            Buffer_Array = -1;
        }

        protected void Create(string name)
        {
            ConsoleLog.Log("Create Buffer '" + name + "'");

            Buffer_Array = GL.GenVertexArray();
        }
        public abstract void Create();
        protected void Delete(string name)
        {
            ConsoleLog.Log("Delete Buffer '" + name + "'");

            GL.BindVertexArray(Buffer_Array);
            Buffer_Array = -1;
        }
        public abstract void Delete();

        public abstract void Draw();
    }
    /*public abstract class RenderProgram
    {
        private static uint ID_Number = 0;
        private static uint ID_Current = 0xFFFFFFFF;

        private readonly uint ID;
        public readonly string Name;

        private string Vert_File;
        private string Geom_File;
        private string Frag_File;

        protected int Program;

        protected RenderProgram(string name, string vert_file, string geom_file, string frag_file)
        {
            ConsoleLog.Log("Initialize Program '" + name + "'");
            ConsoleLog.TabInc();
            ConsoleLog.Log("Vert: '" + vert_file + "'");
            ConsoleLog.Log("Geom: '" + geom_file + "'");
            ConsoleLog.Log("Frag: '" + frag_file + "'");
            ConsoleLog.TabDec();
            ConsoleLog.Log("");

            ID = ID_Number++;
            Name = name;

            Vert_File = vert_file;
            Geom_File = geom_file;
            Frag_File = frag_file;

            Program = -1;
        }

        public virtual void Create()
        {
            ConsoleLog.Log("Create Program '" + Name + "'");
            ConsoleLog.TabInc();

            Program = General.Create_Shader_Program(Vert_File, Geom_File, Frag_File);

            ConsoleLog.TabDec();
            ConsoleLog.Log("");
        }
        public virtual void Delete()
        {
            ConsoleLog.Log("Remove Program '" + Name + "'");

            GL.DeleteProgram(Program);
            Program = -1;

            ConsoleLog.Log("");
        }

        public void Use()
        {
            if (ID_Current != ID)
            {
                ID_Current = ID;
                GL.UseProgram(Program);
            }
        }
        public int UniFind(string uni)
        {
            Use();
            return GL.GetUniformLocation(Program, uni);
        }
    }*/
    /*public abstract class RenderCollection<P, B>
    {
        public P Program;
        public List<B> Buffers;

        public abstract void Create();
        public abstract void Delete();

        public int Insert(B buffer)
        {
            int i = Buffers.Count;
            Buffers.Add(buffer);
            return i;
        }
    }*/
}
