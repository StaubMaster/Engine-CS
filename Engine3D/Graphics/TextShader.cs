using System;
using System.Collections.Generic;

using OpenTK.Graphics.OpenGL4;

using Engine3D.Graphics.Display;
using Engine3D.Graphics.Shader;

namespace Engine3D.Graphics
{
    public class TextShader : BaseShader
    {
        public TextShader(string shaderDir) : base(new ShaderCode[]
        {
            ShaderCode.FromFile(shaderDir + "Text/TextUni.vert"),
            ShaderCode.FromFile(shaderDir + "Text/TextUni.geom"),
            ShaderCode.FromFile(shaderDir + "Frag/Direct.frag"),
        })
        {

        }

        protected override void UpdateUniforms()
        {

        }
    }
}
