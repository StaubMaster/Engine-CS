using OpenTK.Graphics.OpenGL4;

namespace Engine3D.OutPut.Uniform.Generic.Float
{
    public class CUniformFloat1 : AUniformFloatN
    {
        public CUniformFloat1(int count) : base(1, count)
        {

        }

        public void Value(float v, int index = 0)
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
