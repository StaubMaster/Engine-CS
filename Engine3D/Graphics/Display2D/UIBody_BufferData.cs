using Engine3D.Abstract3D;
using Engine3D.Graphics.Display3D;
using Engine3D.Miscellaneous.EntryContainer;
using Engine3D.Graphics.PolyHedraBase;
using Engine3D.Entity;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.Graphics.Display2D.UserInterface
{
    public struct UIBody_Data
    {
        public UIGridPosition Pos;
        public UIGridSize Size;
        public float Scale;
        public Transformation3D Trans;

        public UIBody_Data(UIGridPosition pos, UIGridSize size, float scale, Angle3D spin)
        {
            Pos = pos;
            Size = size;
            Scale = scale;
            Trans = new Transformation3D(spin);
        }
        public UIBody_Data(UIGridPosition pos, UIGridSize size, float scale, Transformation3D trans)
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

    public class UIBody_Buffer : PolyHedraInstance_Base_Buffer<UIBody_Data>
    {
        public UIBody_Buffer() : base()
        {

        }
        ~UIBody_Buffer()
        {

        }

        public override void Bind_Inst(UIBody_Data[] data, int len)
        {
            Use();

            GL.BindBuffer(BufferTarget.ArrayBuffer, InstBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, len * UIBody_Data.SizeOf, data, BufferUsageHint.StreamDraw);

            System.IntPtr offset = System.IntPtr.Zero;
            UIBody_Data.ToBuffer(UIBody_Data.SizeOf, ref offset, 1, 1, 2, 3, 4, 5, 6, 7, 8, 9);

            InstCount = len;
        }
    }
    public class UIBody_BufferData : PolyHedraInstance_Base_BufferData<UIBody_Buffer, UIBody_Data>
    {
        public UIBody_BufferData(PolyHedra ph) : base(ph)
        {

        }
    }
    public class UIBody_Array : PolyHedraInstance_Base_Array<UIBody_BufferData, UIBody_Buffer, UIBody_Data>
    {
        public UIBody_Array(UIBody_BufferData[] array) : base(array)
        {

        }
        public UIBody_Array(PolyHedra[] bodys) : base()
        {
            Array = new UIBody_BufferData[bodys.Length];
            for (int i = 0; i < bodys.Length; i++)
            {
                Array[i] = new UIBody_BufferData(bodys[i]);
            }
        }
        public UIBody_Array(BodyStatic[] bodys) : base()
        {
            Array = new UIBody_BufferData[bodys.Length];
            for (int i = 0; i < bodys.Length; i++)
            {
                Array[i] = new UIBody_BufferData(bodys[i].ToPolyHedra());
            }
        }
    }
}
