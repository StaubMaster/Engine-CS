using System;
using System.Collections.Generic;

using Engine3D.Graphics.Shader;
using Engine3D.Abstract3D;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.Graphics
{
    public class AxisBoxBuffer : BaseBuffer
    {
        private readonly int Buffer_Data;

        private readonly List<RenderAxisBox3D> Box_Data;
        private int Count;

        public AxisBoxBuffer() : base()
        {
            Buffer_Data = GL.GenBuffer();
            Box_Data = new List<RenderAxisBox3D>();
        }
        ~AxisBoxBuffer()
        {
            GL.DeleteBuffer(Buffer_Data);
        }

        public void Insert(AxisBox3D box, uint col)
        {
            Box_Data.Add(new RenderAxisBox3D(box, col));
        }
        public void BindList()
        {
            Bind(Box_Data.ToArray());
            Box_Data.Clear();
        }

        private void Bind(RenderAxisBox3D[] data)
        {
            Use();

            GL.BindBuffer(BufferTarget.ArrayBuffer, Buffer_Data);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * RenderAxisBox3D.Size, data, BufferUsageHint.DynamicDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribIPointer(0, 1, VertexAttribIntegerType.UnsignedInt, RenderAxisBox3D.Size, (IntPtr)RenderAxisBox3D.Size_Color);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, RenderAxisBox3D.Size, (IntPtr)RenderAxisBox3D.Size_Min);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, RenderAxisBox3D.Size, (IntPtr)RenderAxisBox3D.Size_Max);

            Count = data.Length;

            Telematry?.Bind(Count, RenderAxisBox3D.Size);
        }

        public override void Draw()
        {
            Use();

            GL.Disable(EnableCap.DepthTest);
            GL.DrawArrays(PrimitiveType.Points, 0, Count);
            GL.Enable(EnableCap.DepthTest);

            Telematry?.Draw(Count);
        }
    }
}
