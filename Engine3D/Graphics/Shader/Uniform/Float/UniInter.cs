
namespace Engine3D.Graphics.Shader.Uniform.Float
{
    public class UniInter : UniFloat1
    {
        public UniInter(BaseShader program, string name) : base(program, name, 2) { }

        public void T0(float val)
        {
            float[] data = NewData();
            data[0] = val;
            data[1] = 1.0f - val;
            Set(data);
        }
        public void T1(float val)
        {
            float[] data = NewData();
            data[0] = 1.0f - val;
            data[1] = val;
            Set(data);
        }
    }
}
