using Engine3D.Graphics.Shader.Uniform;

using Engine3D.Miscellaneous;

namespace Engine3D.Graphics.Shader
{
    public class GenericShader : BaseShader
    {
        private readonly ArrayList<GenericUniformProgramLocation> UniformList;

        public GenericShader(ShaderCode[] code) : base(code)
        {
            UniformList = new ArrayList<GenericUniformProgramLocation>();
            UniformList.EditBegin();
        }
        public void UniformsDone()
        {
            UniformList.EditEnd();
        }

        public void UniformRemember(GenericUniformProgramLocation uni)
        {
            UniformList.Insert(uni);
        }

        protected override void UpdateUniforms()
        {
            for (int i = 0; i < UniformList.Count; i++)
            {
                UniformList[i].PutData();
            }
        }
    }
}
