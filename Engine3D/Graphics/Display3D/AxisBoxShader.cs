using Engine3D.Graphics.Shader;
using Engine3D.Graphics.Shader.Uniform.Float;

namespace Engine3D.Graphics
{
    public class AxisBoxShader : BaseShader
    {
        public readonly UniScreenRatio ScreenRatio;

        public readonly UniTransformation View;


        public readonly UniDepth Depth;
        public readonly UniRange DepthFadeRange;
        public readonly UniColor DepthFadeColor;

        public AxisBoxShader(string shaderDir) : base(new ShaderCode[]
        {
            ShaderCode.FromFile(shaderDir + "Box/Box_Fixed2.vert"),
            ShaderCode.FromFile(shaderDir + "Box/Box_Fixed2.geom"),
            ShaderCode.FromFile(shaderDir + "Frag/Direct.frag"),
        })
        {
            ScreenRatio = new UniScreenRatio(this, "screenRatios");

            View = new UniTransformation(this, "view");

            Depth = new UniDepth(this, "depthFactor");
            DepthFadeRange = new UniRange(this, "depthFadeRange");
            DepthFadeColor = new UniColor(this, "depthFadeColor");
        }

        protected override void UpdateUniforms()
        {
            ScreenRatio.Update();

            View.Update();

            Depth.Update();
            DepthFadeRange.Update();
            DepthFadeColor.Update();
        }
    }
}
