using Engine3D.Graphics.Shader;
using Engine3D.Graphics.Shader.Uniform.Float;

namespace Engine3D.Graphics
{
    public class BodyElemUniWireShader : BaseShader
    {
        public readonly UniScreenRatio ScreenRatio;

        public readonly UniTransformation View;


        public readonly UniDepth Depth;
        public readonly UniRange DepthFadeRange;
        public readonly UniColor DepthFadeColor;

        public readonly UniColor OtherColor;
        public readonly UniInter OtherColorInter;


        public readonly UniTransformation Trans;



        public BodyElemUniWireShader(string shaderDir) : base(new ShaderCode[]
        {
            ShaderCode.FromFile(shaderDir + "OAR/OAR.vert"),
            ShaderCode.FromFile(shaderDir + "OAR/OAR_Wire.geom"),
            ShaderCode.FromFile(shaderDir + "OAR/OAR_Line.frag"),
        })
        {
            ScreenRatio = new UniScreenRatio(this, "screenRatios");

            View = new UniTransformation(this, "view");
            Trans = new UniTransformation(this, "trans");

            Depth = new UniDepth(this, "depthFactor");
            DepthFadeRange = new UniRange(this, "depthFadeRange");
            DepthFadeColor = new UniColor(this, "depthFadeColor");

            OtherColor = new UniColor(this, "colorOther");
            OtherColorInter = new UniInter(this, "colorInterPol");
        }

        protected override void UpdateUniforms()
        {
            ScreenRatio.Update();

            View.Update();
            Trans.Update();

            Depth.Update();
            DepthFadeRange.Update();
            DepthFadeColor.Update();

            OtherColor.Update();
            OtherColorInter.Update();
        }
    }
}
