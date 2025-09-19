using Engine3D.Graphics.Shader;
using Engine3D.Graphics.Shader.Uniform;
using Engine3D.DataStructs;

using Engine3D.Abstract3D;
using Engine3D.Miscellaneous;

namespace Engine3D.Graphics.Basic.Uniforms
{
    public class GenericDataUniform<T> : GenericUniformBase where T : IData
    {
        private T Data;
        private bool HasValue;

        public GenericDataUniform(string name) : base(name)
        {
            Data = default;
            HasValue = false;
        }
        public GenericDataUniform(string name, GenericShader[] programs) : base(name, programs)
        {
            Data = default;
            HasValue = false;
        }

        public void ChangeData(T data)
        {
            Data = data;
            HasValue = true;
            ChangeData();
        }
        public override void PutData(int location)
        {
            if (HasValue)
            {
                Data.ToUniform(location);
            }
        }
    }
}
