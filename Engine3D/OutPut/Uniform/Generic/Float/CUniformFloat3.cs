using OpenTK.Graphics.OpenGL4;

namespace Engine3D.OutPut.Uniform.Generic.Float
{
    public class CUniformFloat3 : AUniformFloatN
    {
        public CUniformFloat3(int count) : base(3, count)
        {

        }

        public void Value(float v0, float v1, float v2, int index = 0)
        {
            index *= Size;
            Data[index + 0] = v0;
            Data[index + 1] = v1;
            Data[index + 2] = v2;
        }
        public void Value((float, float, float) value, int index = 0)
        {
            index *= Size;
            Data[index + 0] = value.Item1;
            Data[index + 1] = value.Item2;
            Data[index + 2] = value.Item3;
        }

        public override void Uniform(int uni)
        {
            GL.Uniform3(uni, Count, Data);
        }
    }
}
