using System;
using System.Collections.Generic;

using OpenTK.Graphics.OpenGL4;

using Engine3D.Graphics.Display;
using Engine3D.Graphics.Shader;
using Engine3D.Graphics.Shader.Uniform.Float;

namespace Engine3D.Graphics
{
    public class TextShader : BaseShader
    {
        public readonly UniScreenRatio ScreenRatio;

        public TextShader(string shaderDir) : base(new ShaderCode[]
        {
            ShaderCode.FromFile(shaderDir + "Text/TextUni.vert"),
            ShaderCode.FromFile(shaderDir + "Text/TextUni.geom"),
            ShaderCode.FromFile(shaderDir + "Frag/Direct.frag"),
        })
        {
            ScreenRatio = new UniScreenRatio(this, "screenRatios");
        }

        protected override void UpdateUniforms()
        {
            ScreenRatio.Update();
        }
    }
}
