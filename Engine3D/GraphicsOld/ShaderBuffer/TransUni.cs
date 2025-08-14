using System;
using System.Collections.Generic;

using Engine3D.Abstract3D;
using OpenTK.Graphics.OpenGL4;

namespace Engine3D.GraphicsOld
{
    public class TransUniProgram : ViewRenderProgram
    {
        private int Uni_Trans;

        private int Uni_Solar;

        public TransUniProgram(string name, string vert_file, string geom_file, string frag_file)
            : base(name, vert_file, geom_file, frag_file)
        {

        }

        public override void Create()
        {
            if (Program != -1) { return; }
            base.Create();

            Uni_Trans = UniFind("trans");
            Uni_Solar = UniFind("solar");
        }
        public override void Delete()
        {
            if (Program == -1) { return; }
            base.Delete();

            Uni_Trans = -1;

            Uni_Solar = -1;
        }

        public void UniTrans(float[] trans)
        {
            Use();
            GL.Uniform3(Uni_Trans, 3, trans);
        }
        public void UniTrans(RenderTrans trans)
        {
            Use();
            RenderTrans.Uniform(trans, Uni_Trans);
        }
        public void UniSolar(Point3D p)
        {
            Use();
            GL.Uniform3(Uni_Solar, (float)p.Y, (float)p.X, (float)p.C);
        }
    }
    public class TransUniBuffers : RenderBuffers
    {
        private int Buffer_Koords;
        private int Buffer_Indexe;
        private int Buffer_Colors;
        private int Index_Count;

        public TransUniBuffers() : base()
        {

        }

        public override void Create()
        {
            if (Buffer_Array != -1) { return; }
            Create("TransUni");

            Buffer_Koords = GL.GenBuffer();
            Buffer_Indexe = GL.GenBuffer();
            Buffer_Colors = GL.GenBuffer();
            Index_Count = 0;
        }
        public override void Delete()
        {
            if (Buffer_Array == -1) { return; }
            Delete("TransUni");

            GL.DeleteBuffer(Buffer_Koords);
            GL.DeleteBuffer(Buffer_Indexe);
            GL.DeleteBuffer(Buffer_Colors);
        }
        public static void Create(ref TransUniBuffers buffer)
        {
            buffer = new TransUniBuffers();
            buffer.Create();
        }
        public static void Delete(ref TransUniBuffers buffer)
        {
            buffer.Delete();
            buffer = null;
        }

        public void Koords(float[] koords)
        {
            GL.BindVertexArray(Buffer_Array);

            GL.BindBuffer(BufferTarget.ArrayBuffer, Buffer_Koords);
            GL.BufferData(BufferTarget.ArrayBuffer, koords.Length * sizeof(float), koords, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        }
        public void Indexe(uint[] indexe)
        {
            GL.BindVertexArray(Buffer_Array);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Buffer_Indexe);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indexe.Length * sizeof(uint), indexe, BufferUsageHint.StaticDraw);

            Index_Count = indexe.Length;
        }
        public void Colors(uint[] colors)
        {
            GL.BindVertexArray(Buffer_Array);

            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, Buffer_Colors);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, colors.Length * sizeof(uint), colors, BufferUsageHint.StaticDraw);
        }

        public override void Draw()
        {
            GL.BindVertexArray(Buffer_Array);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, Buffer_Colors);

            GL.DrawElements(PrimitiveType.Triangles, Index_Count, DrawElementsType.UnsignedInt, 0);
        }
    }
    public class TransUniCollection : RenderCollection<TransUniProgram, TransUniBuffers>
    {
        public TransUniCollection(TransUniProgram program)
        {
            Program = program;
        }

        public override void Create()
        {
            Program.Create();
            Buffers = new List<TransUniBuffers>();
        }
        public override void Delete()
        {
            Program.Delete();
            for (int i = 0; i < Buffers.Count; i++)
                Buffers[i].Delete();
            Buffers = null;
        }
    }
}
