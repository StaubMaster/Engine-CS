using Engine3D.Abstract3D;
using Engine3D.DataStructs;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.Graphics.Display3D
{
    public struct PolyHedraInstance_3D_Data
    {
        public const int Size = Transformation3D.SizeOf + ColorUData.SizeOf + LInterData.SizeOf + LInterData.SizeOf;

        public Transformation3D Trans;
        public ColorUData AltColor;
        public LInterData AltColorLInter;
        public LInterData GrayLInter;

        public PolyHedraInstance_3D_Data(Transformation3D trans)
        {
            Trans = trans;
            AltColor = new ColorUData(0x7F7F7F);
            AltColorLInter = LInterData.LIT0();
            GrayLInter = LInterData.LIT0();
        }
        public PolyHedraInstance_3D_Data(Transformation3D trans, ColorUData altColor, LInterData altColorLInter)
        {
            Trans = trans;
            AltColor = altColor;
            AltColorLInter = altColorLInter;
            GrayLInter = LInterData.LIT0();
        }

        public static void ToBuffer(int stride, ref System.IntPtr offset, int divisor, params int[] bindIndex)
        {
            Transformation3D.ToBuffer(stride, ref offset, divisor, bindIndex[0], bindIndex[1], bindIndex[2]);
            ColorUData.ToBuffer(stride, ref offset, divisor, bindIndex[3]);
            LInterData.ToBuffer(stride, ref offset, divisor, bindIndex[4]);
            LInterData.ToBuffer(stride, ref offset, divisor, bindIndex[5]);
        }
    }
}
