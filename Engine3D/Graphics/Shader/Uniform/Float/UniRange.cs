
namespace Engine3D.Graphics.Shader.Uniform.Float
{
    public class UniRange : UniFloat1
    {
        public UniRange(BaseShader program, string name) : base(program, name, 3) { }

        public void Value(float min, float max)
        {
            float[] data = NewData();

            data[0] = min;
            data[1] = max - min;
            data[2] = max;

            Set(data);
        }
    }
}
