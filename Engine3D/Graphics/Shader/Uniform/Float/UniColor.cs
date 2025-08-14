
namespace Engine3D.Graphics.Shader.Uniform.Float
{
    public class UniColor : UniFloat3
    {
        public UniColor(BaseShader program, string name) : base(program, name, 1) { }

        public void Value(float r, float g, float b)
        {
            float[] data = NewData();
            data[0] = r;
            data[1] = g;
            data[2] = b;
            Set(data);
        }
        public void Value((float, float, float) rgb)
        {
            Value(rgb.Item1, rgb.Item2, rgb.Item3);
        }
        public void Value(byte r, byte g, byte b)
        {
            Value(
                r / 255.0f,
                g / 255.0f,
                b / 255.0f
                );
        }
        public void Value(uint rgb)
        {
            Value(
                (rgb >> 16) & 0xFF,
                (rgb >> 8) & 0xFF,
                (rgb >> 0) & 0xFF
                );
        }
    }
}
