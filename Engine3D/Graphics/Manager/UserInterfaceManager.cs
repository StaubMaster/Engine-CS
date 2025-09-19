using Engine3D.Abstract3D;
using Engine3D.Graphics.Shader;
using Engine3D.DataStructs;
using Engine3D.Graphics.Basic.Uniforms;


namespace Engine3D.Graphics.Manager
{
    public class UserInterfaceManager : GraphicsManager
    {
        //  Global
        public GenericDataUniform<SizeRatio> ScreenRatio;


        //public GUniDepth Depth;
        public GenericDataUniform<DepthData> Depth;
        public GenericDataUniform<RangeData> DepthFadeRange;
        public GenericDataUniform<ColorUData> DepthFadeColor;

        public GenericDataUniform<Point3D> LightSolar;
        public GenericDataUniform<RangeData> LightRange;



        //  both Per Body / also Global
        public GenericDataUniform<ColorUData> OtherColor;
        public GenericDataUniform<LInterData> OtherColorInter;

        public GenericDataUniform<LInterData> GrayInter;



        public GenericShader TextShader;
        public GenericShader UIBodyShader;



        protected override void InitShaders(string shaderDir)
        {
            UIBodyShader = new GenericShader(new ShaderCode[]
            {
                ShaderCode.FromFile(shaderDir + "Grid/UI_Body.vert"),
                ShaderCode.FromFile(shaderDir + "Grid/UI_Body.geom"),
                ShaderCode.FromFile(shaderDir + "Grid/UI_Body.frag"),
            });

            TextShader = new GenericShader(new ShaderCode[]
            {
                ShaderCode.FromFile(shaderDir + "Text/Text.vert"),
                ShaderCode.FromFile(shaderDir + "Text/Text.geom"),
                ShaderCode.FromFile(shaderDir + "Text/Text.frag"),
            });

            AppendShaders(
                UIBodyShader,
                TextShader
                );
        }
        protected override void InitUniforms()
        {
            ScreenRatio = new GenericDataUniform<SizeRatio>("screenRatios");

            Depth = new GenericDataUniform<DepthData>("depthFactor");
            DepthFadeRange = new GenericDataUniform<RangeData>("depthFadeRange");
            DepthFadeColor = new GenericDataUniform<ColorUData>("depthFadeColor");

            LightSolar = new GenericDataUniform<Point3D>("solar");
            LightRange = new GenericDataUniform<RangeData>("lightRange");

            OtherColor = new GenericDataUniform<ColorUData>("colorOther");
            OtherColorInter = new GenericDataUniform<LInterData>("colorInterPol");

            GrayInter = new GenericDataUniform<LInterData>("GrayInter");

            AppendUniforms(
                ScreenRatio,
                Depth,
                DepthFadeRange,
                DepthFadeColor,
                LightSolar,
                LightRange,
                OtherColor,
                OtherColorInter,
                GrayInter
                );
        }

        public UserInterfaceManager(string shaderDir) : base(shaderDir, null, null) { }
    }
}
