using Engine3D.Graphics;
using Engine3D.Graphics.Shader;
using Engine3D.Graphics.Shader.Uniform;

using Engine3D.Miscellaneous;

namespace Engine3D.Graphics.Manager
{
    //  make abstract
    public abstract class GraphicsManager
    {
        /* generalize
         *  Array of Shaders
         *  Array of Uniforms
         *  
         *  after all Constructing is done
         *  Connect Uniforms and Shaders
         */

        //public readonly ArrayList<GenericShader> ShaderList;

        protected GraphicsManager()
        {
            //ShaderList = new ArrayList<GenericShader>();
        }

        protected abstract GenericShader[] AllShaders();
        protected abstract void InitUniforms(GenericShader[] shaders);
        public void InitUniforms()
        {
            InitUniforms(AllShaders());
        }
    }
}
