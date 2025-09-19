using Engine3D.Graphics;
using Engine3D.Graphics.Shader;
using Engine3D.Graphics.Shader.Uniform;

using Engine3D.Miscellaneous;

namespace Engine3D.Graphics.Manager
{
    public abstract class GraphicsManager
    {
        /* combine Shaders and Uniforms into a "Packet"
         * when additional stuff is used, they can be put together
         * so for chunk stuff
         *  the Shader and Uniforms can be together
         *  
         * finally do Complex Uniforms
         * 
         */

        protected GenericShader[] Shaders;
        protected GenericUniformBase[] Uniforms;

        protected void AppendShaders(params GenericShader[] shaders)
        {
            Shaders = ArrayHelp.CombineArrays(Shaders, shaders);
        }
        protected void AppendUniforms(params GenericUniformBase[] uniforms)
        {
            Uniforms = ArrayHelp.CombineArrays(Uniforms, uniforms);
        }

        protected abstract void InitShaders(string shaderDir);
        protected abstract void InitUniforms();

        protected GraphicsManager(
            string shaderDir,
            GenericShader[] shaders,
            GenericUniformBase[] uniforms
            )
        {
            Shaders = shaders;
            Uniforms = uniforms;

            InitShaders(shaderDir);
            InitUniforms();

            if (Shaders != null && Uniforms != null)
            {
                for (int i = 0; i < Uniforms.Length; i++)
                {
                    Uniforms[i].FindLocations(Shaders);
                }
            }
        }
    }
}
