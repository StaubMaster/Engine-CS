using System;
using System.Collections.Generic;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.GraphicsOld
{
    public class SurfLayerProgram : ViewRenderProgram
    {
        private int Uni_Corns_Per_Side;
        private int Uni_Tiles_Per_Side;
        private int Uni_Tiles_Width;
        private int Uni_Tiles_Middle_Offset;
        private int Uni_Tiles_Cut;

        public SurfLayerProgram(string name, string vert_file, string geom_file, string frag_file)
            : base(name, vert_file, geom_file, frag_file)
        {

        }

        public override void Create()
        {
            if (Program != -1) { return; }
            base.Create();

            Uni_Corns_Per_Side = GL.GetUniformLocation(Program, "corns_per_side");
            Uni_Tiles_Per_Side = GL.GetUniformLocation(Program, "tiles_per_side");
            Uni_Tiles_Width = GL.GetUniformLocation(Program, "tiles_width");
            Uni_Tiles_Middle_Offset = GL.GetUniformLocation(Program, "tiles_middle_offset");
            Uni_Tiles_Cut = GL.GetUniformLocation(Program, "tiles_cut");
        }
        public override void Delete()
        {
            if (Program == -1) { return; }
            base.Delete();

            Uni_Corns_Per_Side = -1;
            Uni_Tiles_Per_Side = -1;
            Uni_Tiles_Width = -1;
            Uni_Tiles_Middle_Offset = -1;
            Uni_Tiles_Cut = -1;
        }

        public void UniTilesCorners(uint tiles_per_side)
        {
            Use();
            GL.Uniform1(Uni_Tiles_Per_Side, tiles_per_side);
            GL.Uniform1(Uni_Corns_Per_Side, tiles_per_side + 1);
        }
        public void UniWidth(uint tiles_width)
        {
            Use();
            GL.Uniform1(Uni_Tiles_Width, tiles_width);
        }
        public void UniOffset(int y, int x, int c)
        {
            Use();
            GL.Uniform3(Uni_Tiles_Middle_Offset, y, x, c);
        }
        public void UniCut(uint[] cut)
        {
            Use();
            GL.Uniform2(Uni_Tiles_Cut, 2, cut);
        }
    }
    public class SurfLayerBuffers : RenderBuffers
    {
        private int Buffer_Heights;
        private int Buffer_Indexe;
        private int Index_Count;

        public SurfLayerBuffers() : base()
        {

        }

        public override void Create()
        {
            if (Buffer_Array != -1) { return; }
            Create("Surface");

            Buffer_Heights = GL.GenBuffer();
            Buffer_Indexe = GL.GenBuffer();
            Index_Count = 0;
        }
        public override void Delete()
        {
            if (Buffer_Array == -1) { return; }
            Delete("Surface");

            GL.DeleteBuffer(Buffer_Heights);
            GL.DeleteBuffer(Buffer_Indexe);
        }

        public void Indexe(uint tiles_per_side)
        {
            uint corners_per_side = tiles_per_side + 1;

            uint[] indexe = new uint[(tiles_per_side * tiles_per_side) * 5];

            uint index_idx, corn_idx;
            index_idx = 0xFFFFFFFF;
            for (uint y = 0; y < tiles_per_side; y++)
            {
                for (uint x = 0; x < tiles_per_side; x++)
                {
                    corn_idx = x + y * corners_per_side;

                    indexe[++index_idx] = corn_idx + 0;
                    indexe[++index_idx] = corn_idx + 1;
                    indexe[++index_idx] = corn_idx + 0 + corners_per_side;
                    indexe[++index_idx] = corn_idx + 1 + corners_per_side;
                    //indexe[++index_idx] = 0xFFFFFFFF;
                }
            }

            GL.BindVertexArray(Buffer_Array);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Buffer_Indexe);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indexe.Length * sizeof(uint), indexe, BufferUsageHint.StaticDraw);

            Index_Count = indexe.Length;
        }
        public void Heights(uint[] heights)
        {
            GL.BindVertexArray(Buffer_Array);

            GL.BindBuffer(BufferTarget.ArrayBuffer, Buffer_Heights);
            GL.BufferData(BufferTarget.ArrayBuffer, heights.Length * sizeof(uint), heights, BufferUsageHint.DynamicDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribIPointer(0, 1, VertexAttribIntegerType.UnsignedInt, 1 * sizeof(uint), (IntPtr)0);
        }

        public override void Draw()
        {
            GL.BindVertexArray(Buffer_Array);

            GL.DrawElements(PrimitiveType.LinesAdjacency, Index_Count, DrawElementsType.UnsignedInt, 0);
        }
    }
    public class SurfLayerCollection : RenderCollection<SurfLayerProgram, SurfLayerBuffers>
    {
        public SurfLayerCollection(SurfLayerProgram program)
        {
            Program = program;
        }

        public override void Create()
        {
            Program.Create();
            Buffers = new List<SurfLayerBuffers>();
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
