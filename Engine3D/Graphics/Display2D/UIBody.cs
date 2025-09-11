using Engine3D.Abstract3D;
using Engine3D.Graphics.Display3D;
using Engine3D.Miscellaneous.EntryContainer;
using Engine3D.Graphics.PolyHedraBase;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.Graphics.Display2D.UserInterface
{
    public struct UIBodyData
    {
        public UIGridPosition Pos;
        public UIGridSize Size;
        public float Scale;
        public Transformation3D Trans;

        public UIBodyData(UIGridPosition pos, UIGridSize size, float scale, Angle3D spin)
        {
            Pos = pos;
            Size = size;
            Scale = scale;
            Trans = new Transformation3D(spin);
        }
        public UIBodyData(UIGridPosition pos, UIGridSize size, float scale, Transformation3D trans)
        {
            Pos = pos;
            Size = size;
            Scale = scale;
            Trans = trans;
        }



        public const int SizeOf = UIGridPosition.SizeOf + UIGridSize.SizeOf + sizeof(float) + Transformation3D.SizeOf;
        public static void ToBuffer(int stride, ref System.IntPtr offset, int divisor, params int[] bindIndex)
        {
            UIGridPosition.ToBuffer(stride, ref offset, divisor, bindIndex[0], bindIndex[1], bindIndex[2]);
            UIGridSize.ToBuffer(stride, ref offset, divisor, bindIndex[3], bindIndex[4]);

            GL.EnableVertexAttribArray(bindIndex[5]);
            GL.VertexAttribPointer(bindIndex[5], 1, VertexAttribPointerType.Float, false, stride, offset);
            GL.VertexAttribDivisor(bindIndex[5], divisor);
            offset += sizeof(float);

            Transformation3D.ToBuffer(stride, ref offset, divisor, bindIndex[6], bindIndex[7], bindIndex[8]);
        }
    }

    public class UIBody_Buffer : PolyHedraInstanceBaseBuffer<UIBodyData>
    {
        public UIBody_Buffer() : base()
        {

        }
        ~UIBody_Buffer()
        {

        }

        public override void Bind_Inst(UIBodyData[] data, int len)
        {
            Use();

            GL.BindBuffer(BufferTarget.ArrayBuffer, InstBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, len * UIBodyData.SizeOf, data, BufferUsageHint.StreamDraw);

            System.IntPtr offset = System.IntPtr.Zero;
            UIBodyData.ToBuffer(UIBodyData.SizeOf, ref offset, 1, 1, 2, 3, 4, 5, 6, 7, 8, 9);

            InstCount = len;
        }
    }
    public class UIBody : PolyHedraInstance_Base_BufferData<UIBody_Buffer, UIBodyData>
    {
        public UIBody(PolyHedra ph) : base(ph)
        {

        }
    }
}
