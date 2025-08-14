
namespace Engine3D.Graphics.Shader.Uniform.Float
{
    public class UniDepth : UniFloat1
    {
        public UniDepth(BaseShader program, string name) : base(program, name, 7) { }

        public void Value(float near, float far)
        {
            float[] data = NewData();
            //Get(data);

            data[0] = near;
            data[1] = far;

            data[2] = data[1] - data[0];
            data[3] = data[1] + data[0];
            data[4] = data[1] * data[0] * 2;

            data[5] = data[3] / data[2];
            data[6] = data[4] / data[2];

            Set(data);
        }
    }
}
