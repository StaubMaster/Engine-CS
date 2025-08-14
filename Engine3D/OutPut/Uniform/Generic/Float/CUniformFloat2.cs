using OpenTK.Graphics.OpenGL4;

namespace Engine3D.OutPut.Uniform.Generic.Float
{
    public class CUniformFloat2 : AUniformFloatN
    {
        public CUniformFloat2(int count) : base(2, count)
        {

        }

        public void Value(float v0, float v1, int index = 0)
        {
            index *= Size;
            Data[index + 0] = v0;
            Data[index + 1] = v1;
        }
        public void Value((float, float) value, int index = 0)
        {
            index *= Size;
            Data[index + 0] = value.Item1;
            Data[index + 1] = value.Item2;
        }

        public override void Uniform(int uni)
        {
            GL.Uniform2(uni, Count, Data);
        }
    }
}
