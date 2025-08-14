using Engine3D.Abstract3D;
using Engine3D.Graphics.Shader;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.Graphics
{
    public class BodyElemBuffer : BaseBuffer
    {
        private readonly int CornersBuffer;
        private readonly int IndexesBuffer;
        private readonly int ColorsBuffer;
        private int Count;

        public BodyElemBuffer() : base()
        {
            CornersBuffer = GL.GenBuffer();
            IndexesBuffer = GL.GenBuffer();
            ColorsBuffer = GL.GenBuffer();

            Count = 0;
        }
        ~BodyElemBuffer()
        {
            Use();
            GL.DeleteBuffer(CornersBuffer);
            GL.DeleteBuffer(IndexesBuffer);
            GL.DeleteBuffer(ColorsBuffer);
        }

        public void Bind_Corners(Point3D[] corners)
        {
            RenderPoint3D[] data = RenderPoint3D.Convert(corners);

            Use();

            GL.BindBuffer(BufferTarget.ArrayBuffer, CornersBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * RenderPoint3D.Size, data, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, RenderPoint3D.Size, 0);
        }
        public void Bind_Indexes(IndexTriangle[] faces)
        {
            Use();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexesBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, faces.Length * IndexTriangle.Size, faces, BufferUsageHint.StaticDraw);

            Count = faces.Length * 3;
        }
        public void Bind_Colors(uint[] colors)
        {
            Use();

            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, ColorsBuffer);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, colors.Length * sizeof(uint), colors, BufferUsageHint.StaticDraw);
        }

        public override void Draw()
        {
            Use();

            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, ColorsBuffer);
            GL.DrawElements(BeginMode.Triangles, Count, DrawElementsType.UnsignedInt, 0);
        }
    }
}
