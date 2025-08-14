using System;
using System.Collections.Generic;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.GraphicsOld
{
    public class SurfChunkProgram : ViewRenderProgram
    {
        private int Uni_Chunk_Pos;
        private int Uni_Tiles_Size;
        private int Uni_Tiles_Per_Side;

        public SurfChunkProgram(string name, string vert_file, string geom_file, string frag_file)
            : base(name, vert_file, geom_file, frag_file)
        {

        }

        public override void Create()
        {
            if (Program != -1) { return; }
            base.Create();

            Uni_Chunk_Pos = GL.GetUniformLocation(Program, "chunk_pos");
            Uni_Tiles_Size = GL.GetUniformLocation(Program, "tiles_size");
            Uni_Tiles_Per_Side = GL.GetUniformLocation(Program, "tiles_per_side");
        }
        public override void Delete()
        {
            if (Program == -1) { return; }
            base.Delete();

            Uni_Chunk_Pos = -1;
            Uni_Tiles_Size = -1;
            Uni_Tiles_Per_Side = -1;
        }

        public void UniPos(int y, int x, int c)
        {
            Use();
            GL.Uniform3(Uni_Chunk_Pos, y, x, c);
        }
        public void UniTiles(int tiles_size, int tiles_per_side)
        {
            Use();
            GL.Uniform1(Uni_Tiles_Size, tiles_size);
            GL.Uniform1(Uni_Tiles_Per_Side, tiles_per_side);
        }
    }
    public class SurfChunkBuffers : RenderBuffers
    {
        private int Buffer_Heights;
        private int Buffer_Indexe;
        private int Buffer_Colors;
        private int Buffer_Sizes;
        private int Heights_Count;
        private int Index_Count;

        public SurfChunkBuffers() : base()
        {

        }

        public override void Create()
        {
            if (Buffer_Array != -1) { return; }
            Create("Surface Chunk");

            Buffer_Heights = GL.GenBuffer();
            Buffer_Indexe = GL.GenBuffer();
            Buffer_Colors = GL.GenBuffer();
            Buffer_Sizes = GL.GenBuffer();
            Heights_Count = 0;
            Index_Count = 0;
        }
        public override void Delete()
        {
            if (Buffer_Array == -1) { return; }
            Delete("Surface Chunk");

            GL.DeleteBuffer(Buffer_Heights);
            GL.DeleteBuffer(Buffer_Indexe);
            GL.DeleteBuffer(Buffer_Colors);
            GL.DeleteBuffer(Buffer_Sizes);
        }

        public void Indexe(uint[] indexe)
        {
            GL.BindVertexArray(Buffer_Array);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Buffer_Indexe);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indexe.Length * sizeof(uint), indexe, BufferUsageHint.StaticDraw);

            Index_Count = indexe.Length;
        }
        public void Heights(int[] heights)
        {
            GL.BindVertexArray(Buffer_Array);

            GL.BindBuffer(BufferTarget.ArrayBuffer, Buffer_Heights);
            GL.BufferData(BufferTarget.ArrayBuffer, heights.Length * sizeof(int), heights, BufferUsageHint.DynamicDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribIPointer(0, 1, VertexAttribIntegerType.Int, 1 * sizeof(int), (IntPtr)0);

            Heights_Count = heights.Length;
        }
        public void Colors(uint[] colors)
        {
            GL.BindVertexArray(Buffer_Array);

            GL.BindBuffer(BufferTarget.ArrayBuffer, Buffer_Colors);
            GL.BufferData(BufferTarget.ArrayBuffer, colors.Length * sizeof(uint), colors, BufferUsageHint.DynamicDraw);

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribIPointer(2, 1, VertexAttribIntegerType.UnsignedInt, 1 * sizeof(uint), (IntPtr)0);
        }
        public void Sizes(int[] sizes)
        {
            GL.BindVertexArray(Buffer_Array);

            GL.BindBuffer(BufferTarget.ArrayBuffer, Buffer_Sizes);
            GL.BufferData(BufferTarget.ArrayBuffer, sizes.Length * sizeof(int), sizes, BufferUsageHint.DynamicDraw);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribIPointer(1, 1, VertexAttribIntegerType.Int, 1 * sizeof(int), (IntPtr)0);
        }

        public override void Draw()
        {
            GL.BindVertexArray(Buffer_Array);

            //GL.DrawElements(PrimitiveType.Points, Index_Count, DrawElementsType.UnsignedInt, 0);
            GL.DrawArrays(PrimitiveType.Points, 0, Heights_Count);
        }
    }
}
