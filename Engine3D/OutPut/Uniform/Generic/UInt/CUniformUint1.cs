
using OpenTK.Graphics.OpenGL4;

namespace Engine3D.OutPut.Uniform.Generic.UInt
{
    public class CUniformUint1 : AUniformUIntN
    {
        public CUniformUint1(int count) : base(1, count)
        {

        }

        public void Value(uint v, int index = 0)
        {
            index *= Size;
            Data[index + 0] = v;
        }

        public override void Uniform(int uni)
        {
            GL.Uniform1(uni, Count, Data);
        }
    }
}
