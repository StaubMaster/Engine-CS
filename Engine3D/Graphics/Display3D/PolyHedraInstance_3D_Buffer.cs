using Engine3D.Graphics.PolyHedraBase;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.Graphics.Display3D
{
    public class PolyHedraInstance_3D_Buffer : PolyHedraInstanceBaseBuffer<PolyHedraInstance_3D_Data>
    {
        public override void Bind_Inst(PolyHedraInstance_3D_Data[] data, int len)
        {
            Use();

            GL.BindBuffer(BufferTarget.ArrayBuffer, InstBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, len * PolyHedraInstance_3D_Data.Size, data, BufferUsageHint.StreamDraw);

            System.IntPtr offset = System.IntPtr.Zero;
            PolyHedraInstance_3D_Data.ToBuffer(PolyHedraInstance_3D_Data.Size, ref offset, 1, 1, 2, 3, 4, 5, 6);

            InstCount = len;
        }
    }
}
