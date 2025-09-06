using Engine3D.Graphics.Shader.Uniform;
using Engine3D.Graphics.Shader.Uniform.Float;
using Engine3D.Graphics.Basic.Data;

using Engine3D.Abstract3D;
using Engine3D.Miscellaneous;

namespace Engine3D.Graphics.Shader.Uniform
{
    /*public class GUniDepth : GUniBase
    {
        private DepthData? Data;

        public GUniDepth(string name, GenericShader[] programs) : base(name, programs)
        {
            Data = null;
        }

        public void ChangeData(float near, float far)
        {
            //float[] data = NewData();
            //
            //data[0] = near;
            //data[1] = far;
            //
            //data[2] = data[1] - data[0];
            //data[3] = data[1] + data[0];
            //data[4] = data[1] * data[0] * 2;
            //
            //data[5] = data[3] / data[2];
            //data[6] = data[4] / data[2];
            //
            //base.ChangeData(data);
            Data = new DepthData(near, far);
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
