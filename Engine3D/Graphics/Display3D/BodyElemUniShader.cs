using Engine3D.Graphics.Shader;
using Engine3D.Graphics.Shader.Uniform.Float;

namespace Engine3D.Graphics
{
    public class BodyElemUniShader : BaseShader
    {
        public readonly UniScreenRatio ScreenRatio;

        public readonly UniTransformation View;


        public readonly UniDepth Depth;
        public readonly UniRange DepthFadeRange;
        public readonly UniColor DepthFadeColor;

        public readonly UniPoint LightSolar;
        public readonly UniRange LightRange;

        public readonly UniColor OtherColor;
        public readonly UniInter OtherColorInter;

        public readonly UniInter GrayInter;


        public readonly UniTransformation Trans;



        public BodyElemUniShader(string shaderDir) : base(new ShaderCode[]
        {
            ShaderCode.FromFile(shaderDir + "OAR/OAR.vert"),
            ShaderCode.FromFile(shaderDir + "OAR/OAR.geom"),
            ShaderCode.FromFile(shaderDir + "OAR/OAR.frag"),
        })
        {
            ScreenRatio = new UniScreenRatio(this, "screenRatios");

            View = new UniTransformation(this, "view");
            Trans = new UniTransformation(this, "trans");

            Depth = new UniDepth(this, "depthFactor");
            DepthFadeRange = new UniRange(this, "depthFadeRange");
            DepthFadeColor = new UniColor(this, "depthFadeColor");

            LightSolar = new UniPoint(this, "solar");
            LightRange = new UniRange(this, "lightRange");

            OtherColor = new UniColor(this, "colorOther");
            OtherColorInter = new UniInter(this, "colorInterPol");

            GrayInter = new UniInter(this, "GrayInter");
        }

        protected override void UpdateUniforms()
        {
            ScreenRatio.Update();

            View.Update();
            Trans.Update();

            Depth.Update();
            DepthFadeRange.Update();
            DepthFadeColor.Update();

            LightSolar.Update();
            LightRange.Update();

            OtherColor.Update();
            OtherColorInter.Update();

            GrayInter.Update();
        }
    }
}
