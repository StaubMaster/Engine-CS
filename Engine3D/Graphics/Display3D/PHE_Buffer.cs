using Engine3D.Abstract3D;
using Engine3D.DataStructs;
using Engine3D.Graphics.Shader;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.Graphics.Display3D
{
    public class PHE_Buffer : BaseBuffer
    {
        protected readonly int CornersBuffer;
        protected readonly int IndexesBuffer;
        protected readonly int ColorsBuffer;
        protected int ElemCount;

        public PHE_Buffer() : base()
        {
            CornersBuffer = GL.GenBuffer();
            IndexesBuffer = GL.GenBuffer();
            ColorsBuffer = GL.GenBuffer();
            ElemCount = 0;
        }
        ~PHE_Buffer()
        {
            Use();

            GL.DeleteBuffer(CornersBuffer);
            GL.DeleteBuffer(IndexesBuffer);
            GL.DeleteBuffer(ColorsBuffer);
        }

        public void Bind_Main_Corners(Point3D[] data)
        {
            Use();

            GL.BindBuffer(BufferTarget.ArrayBuffer, CornersBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * Point3D.SizeOf, data, BufferUsageHint.StaticDraw);

            System.IntPtr offset = System.IntPtr.Zero;

            Point3D.ToBuffer(Point3D.SizeOf, ref offset, 0, 0);
        }
        public void Bind_Main_Indexes(IndexTriangle[] faces)
        {
            Use();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexesBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, faces.Length * IndexTriangle.Size, faces, BufferUsageHint.StaticDraw);

            ElemCount = faces.Length * 3;
        }
        public void Bind_Main_Colors(ColorUData[] colors)
        {
            Use();

            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, ColorsBuffer);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, colors.Length * ColorUData.SizeOf, colors, BufferUsageHint.StaticDraw);
        }

        public void Draw_Main()
        {
            Use();
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, ColorsBuffer);
            GL.DrawElements(BeginMode.Triangles, ElemCount, DrawElementsType.UnsignedInt, 0);
        }

        public override void Draw()
        {
            throw new System.NotImplementedException();
        }
    }
}
