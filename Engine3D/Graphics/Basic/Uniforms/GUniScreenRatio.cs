using Engine3D.Graphics.Shader.Uniform;
using Engine3D.Graphics.Shader.Uniform.Float;

using Engine3D.Abstract3D;
using Engine3D.Miscellaneous;

namespace Engine3D.Graphics.Shader.Uniform
{
    /*public class GUniScreenRatio : GUniFloat2
    {
        public GUniScreenRatio(string name, GenericShader[] programs) : base(name, programs, 2) { }

        public void ChangeData(float w, float h)
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

            base.ChangeData(data);
        }
        public void ChangeData((float, float) size)
        {
            ChangeData(size.Item1, size.Item2);
        }
    }*/
}
