using System;
using System.Collections.Generic;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.GraphicsOld
{
    public class SelectionBuffers : RenderBuffers
    {
        private int Buffer_Koords;
        private int Buffer_Indexe;
        private int Buffer_Pallet;

        private int Buffer_Select_Ecken;
        private int Buffer_Select_Seitn;

        private int Koords_Count;
        private int Indexe_Count;

        public SelectionBuffers() : base()
        {

        }

        public override void Create()
        {
            if (Buffer_Array != -1) { return; }
            Create("Selection");

            Buffer_Koords = GL.GenBuffer();
            Buffer_Indexe = GL.GenBuffer();
            Buffer_Pallet = GL.GenBuffer();

            Buffer_Select_Ecken = GL.GenBuffer();
            Buffer_Select_Seitn = GL.GenBuffer();

            Koords_Count = 0;
            Indexe_Count = 0;
        }
        public override void Delete()
        {
            if (Buffer_Array == -1) { return; }
            Delete("Selection");

            GL.DeleteBuffer(Buffer_Koords);
            GL.DeleteBuffer(Buffer_Indexe);
            GL.DeleteBuffer(Buffer_Pallet);

            GL.DeleteBuffer(Buffer_Select_Ecken);
            GL.DeleteBuffer(Buffer_Select_Seitn);
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

            Indexe_Count = indexe.Length;
        }
        public void Pallet(uint[] pallet)
        {
            GL.BindVertexArray(Buffer_Array);

            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, Buffer_Pallet);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, pallet.Length * sizeof(uint), pallet, BufferUsageHint.StaticDraw);
        }
        public void Select_Ecken(uint[] select)
        {
            GL.BindVertexArray(Buffer_Array);

            GL.BindBuffer(BufferTarget.ArrayBuffer, Buffer_Select_Ecken);
            GL.BufferData(BufferTarget.ArrayBuffer, select.Length * sizeof(uint), select, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribIPointer(1, 1, VertexAttribIntegerType.UnsignedInt, 1 * sizeof(uint), (IntPtr)0);

            Koords_Count = select.Length;
        }
        public void Select_Seitn(uint[] select)
        {
            GL.BindVertexArray(Buffer_Array);

            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, Buffer_Select_Seitn);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, select.Length * sizeof(uint), select, BufferUsageHint.StaticDraw);
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }
        public void Draw_Ecken()
        {
            GL.BindVertexArray(Buffer_Array);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, Buffer_Pallet);

            GL.DrawArrays(PrimitiveType.Points, 0, Koords_Count);
        }
        public void Draw_Seitn()
        {
            GL.BindVertexArray(Buffer_Array);

            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, Buffer_Pallet);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, Buffer_Pallet);
            
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, Buffer_Select_Seitn);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, Buffer_Select_Seitn);

            GL.DrawElements(PrimitiveType.Triangles, Indexe_Count, DrawElementsType.UnsignedInt, 0);
        }
    }
}
