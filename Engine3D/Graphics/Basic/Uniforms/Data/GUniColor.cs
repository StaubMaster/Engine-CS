using Engine3D.Graphics.Shader.Uniform.Float;

using Engine3D.Miscellaneous;

namespace Engine3D.Graphics.Shader.Uniform
{
    /*public class GUniColor : GUniFloat3
    {
        public GUniColor(string name, GenericShader[] programs) : base(name, programs, 1) { }

        public void ChangeData(float r, float g, float b)
        {
            float[] data = NewData();
            data[0] = r;
            data[1] = g;
            data[2] = b;
            base.ChangeData(data);
        }
        public void ChangeData((float, float, float) rgb)
        {
            ChangeData(rgb.Item1, rgb.Item2, rgb.Item3);
        }
        public void ChangeData(byte r, byte g, byte b)
        {
            ChangeData(
                r / 255.0f,
                g / 255.0f,
                b / 255.0f
                );
        }
        public void ChangeData(uint rgb)
        {
            ChangeData(
                (rgb >> 16) & 0xFF,
                (rgb >> 8) & 0xFF,
                (rgb >> 0) & 0xFF
                );
        }
    }*/
}
