
namespace Engine3D.Graphics.Shader.Uniform.Float
{
    public class UniScreenRatio : UniFloat2
    {
        public UniScreenRatio(BaseShader program, string name) : base(program, name, 2) { }

        public void Value(float w, float h)
        {
            float[] data = NewData();

            data[0] = w;
            data[1] = h;

            if (w == h)
            {
                data[2] = 1.0f;
                data[3] = 1.0f;
            }
            else if (w > h)
            {
                data[2] = h / w;
                data[3] = 1.0f;
            }
            else if (w < h)
            {
                data[2] = 1.0f;
                data[3] = w / h;
            }

            Set(data);
        }
        public void Value((float, float) size)
        {
            Value(size.Item1, size.Item2);
        }
    }
}
