
namespace Engine3D.OutPut.Uniform.Specific
{
    public class CUniformColor : Generic.Float.CUniformFloat3
    {
        public CUniformColor(float r, float g, float b) : base(1)
        {
            Value(r, g, b);
        }
        public CUniformColor((float, float, float) rgb) : base(1)
        {
            Value(rgb);
        }
        public CUniformColor(byte r, byte g, byte b) : base(1)
        {
            Value(r, g, b);
        }
        public CUniformColor(uint rgb) : base(1)
        {
            Value(rgb);
        }

        public void Value(float r, float g, float b)
        {
            Data[0] = r;
            Data[1] = g;
            Data[2] = b;
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
