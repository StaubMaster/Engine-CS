using Engine3D.Abstract3D;
using Engine3D.Graphics.Basic.Data;
using Engine3D.Graphics.Shader;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.Graphics.Display3D
{
    //  PolyHedraElementInstanceBuffer
    public class PHEIBuffer : BaseBuffer
    {
        protected readonly int CornersBuffer;
        protected readonly int IndexesBuffer;
        protected readonly int ColorsBuffer;
        protected int ElemCount;

        protected readonly int InstBuffer;
        protected int InstCount;

        public PHEIBuffer() : base()
        {
            CornersBuffer = GL.GenBuffer();
            IndexesBuffer = GL.GenBuffer();
            ColorsBuffer = GL.GenBuffer();
            ElemCount = 0;

            InstBuffer = GL.GenBuffer();
            InstCount = 0;
        }
        ~PHEIBuffer()
        {
            Use();

            GL.DeleteBuffer(CornersBuffer);
            GL.DeleteBuffer(IndexesBuffer);
            GL.DeleteBuffer(ColorsBuffer);

            GL.DeleteBuffer(InstBuffer);
        }

        public void Bind_Main_Corners(Point3D[] data)
        {
            Use();

            GL.BindBuffer(BufferTarget.ArrayBuffer, CornersBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * Point3D.Size, data, BufferUsageHint.StaticDraw);

            System.IntPtr offset = System.IntPtr.Zero;

            Point3D.ToBuffer(Point3D.Size, ref offset, 0, 0);
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
            GL.BufferData(BufferTarget.ShaderStorageBuffer, colors.Length * ColorUData.Size, colors, BufferUsageHint.StaticDraw);
        }

        public void Bind_Inst_Trans(PHEIData[] data, int len, bool debug = false)
        {
            Use();

            GL.BindBuffer(BufferTarget.ArrayBuffer, InstBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, len * PHEIData.Size, data, BufferUsageHint.StreamDraw);

            System.IntPtr offset = System.IntPtr.Zero;

            PHEIData.ToBuffer(PHEIData.Size, ref offset, 1, 1, 2, 3, 4, 5, 6);

            InstCount = len;
        }

        public void Draw_Main()
        {
            Use();
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, ColorsBuffer);
            GL.DrawElements(BeginMode.Triangles, ElemCount, DrawElementsType.UnsignedInt, 0);
        }
        public void Draw_Inst(bool debug = false)
        {
            Use();

            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, ColorsBuffer);
            GL.DrawElementsInstanced(PrimitiveType.Triangles, ElemCount, DrawElementsType.UnsignedInt, System.IntPtr.Zero, InstCount);
        }

        public override void Draw()
        {
            Use();

            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, ColorsBuffer);
            GL.DrawElementsInstanced(PrimitiveType.Triangles, ElemCount, DrawElementsType.UnsignedInt, System.IntPtr.Zero, InstCount);
        }
    }
}
