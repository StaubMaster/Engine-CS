using Engine3D.Graphics.Basic.Data;
using Engine3D.Graphics.Shader.Uniform.Float;

using Engine3D.Miscellaneous;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.Graphics.Shader.Uniform
{
    /*public class GUniRange : GUniBase
    {
        private RangeData? Data;

        public GUniRange(string name, GenericShader[] programs) : base(name, programs)
        {
            Data = null;
        }

        public void ChangeData(RangeData data)
        {
            Data = data;
            ChangeData();
        }
        public override void PutData(int location)
        {
            if (Data != null)
            {
                Data?.ToUniform(location);
            }
        }
    }*/
}
