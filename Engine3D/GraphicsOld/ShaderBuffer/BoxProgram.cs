using System;
using System.Collections.Generic;

using OpenTK.Graphics.OpenGL4;

using Engine3D.Abstract3D;

namespace Engine3D.GraphicsOld
{
    /*public class BoxProgram : ViewRenderProgram
    {
        public BoxProgram(string name, string vert_file, string geom_file, string frag_file)
            : base(name, vert_file, geom_file, frag_file)
        {

        }

        public override void Create()
        {
            if (Program != -1) { return; }
            base.Create();
        }
        public override void Delete()
        {
            if (Program == -1) { return; }
            base.Delete();
        }
    }*/
    /*public class BoxBuffer : RenderBuffers
    {
        private int Buffer_Data;
        private int Box_Count;

        public override void Create()
        {
            if (Buffer_Array != -1) { return; }
            Create("Surface Chunk");

            Buffer_Data = GL.GenBuffer();
        }
        public override void Delete()
        {
            if (Buffer_Array == -1) { return; }
            Delete("Surface Chunk");

            GL.DeleteBuffer(Buffer_Data);
        }

        public void Data(AxisBox3D.BoxRenderData[] data)
        {
            GL.BindVertexArray(Buffer_Array);

            GL.BindBuffer(BufferTarget.ArrayBuffer, Buffer_Data);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * AxisBox3D.BoxRenderData.Size, data, BufferUsageHint.DynamicDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribIPointer(0, 1, VertexAttribIntegerType.UnsignedInt, AxisBox3D.BoxRenderData.Size, (IntPtr)AxisBox3D.BoxRenderData.Size_Color);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, AxisBox3D.BoxRenderData.Size, (IntPtr)AxisBox3D.BoxRenderData.Size_Min);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, AxisBox3D.BoxRenderData.Size, (IntPtr)AxisBox3D.BoxRenderData.Size_Max);

            Box_Count = data.Length;
        }

        public override void Draw()
        {
            GL.BindVertexArray(Buffer_Array);

            GL.DrawArrays(PrimitiveType.Points, 0, Box_Count);
        }
    }*/
}
