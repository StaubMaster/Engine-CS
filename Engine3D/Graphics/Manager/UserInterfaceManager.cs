using Engine3D.Abstract3D;
using Engine3D.Graphics.Shader;
using Engine3D.DataStructs;
using Engine3D.Graphics.Basic.Uniforms;


namespace Engine3D.Graphics.Manager
{
    public class UserInterfaceManager : GraphicsManager
    {
        //  Global
        public DataUniform<SizeRatio> ScreenRatio;


        //public GUniDepth Depth;
        public DataUniform<DepthData> Depth;
        public DataUniform<RangeData> DepthFadeRange;
        public DataUniform<ColorUData> DepthFadeColor;

        public DataUniform<Point3D> LightSolar;
        public DataUniform<RangeData> LightRange;



        //  both Per Body / also Global
        public DataUniform<ColorUData> OtherColor;
        public DataUniform<LInterData> OtherColorInter;

        public DataUniform<LInterData> GrayInter;



        //public TextShader Text_Shader;
        //public TextBuffer Text_Buffer;

        public GenericShader UIBodyShader;

        public UserInterfaceManager(string shaderDir) : base()
        {
            UIBodyShader = new GenericShader(new ShaderCode[]
            {
                ShaderCode.FromFile(shaderDir + "Grid/UI_Body.vert"),
                ShaderCode.FromFile(shaderDir + "Grid/UI_Body.geom"),
                ShaderCode.FromFile(shaderDir + "Grid/UI_Body.frag"),
            });

            GenericShader[] ShaderList = new GenericShader[]
            {
                UIBodyShader,
            };

            ScreenRatio = new DataUniform<SizeRatio>("screenRatios", ShaderList);

            Depth = new DataUniform<DepthData>("depthFactor", ShaderList);
            DepthFadeRange = new DataUniform<RangeData>("depthFadeRange", ShaderList);
            DepthFadeColor = new DataUniform<ColorUData>("depthFadeColor", ShaderList);

            LightSolar = new DataUniform<Point3D>("solar", ShaderList);
            LightRange = new DataUniform<RangeData>("lightRange", ShaderList);

            OtherColor = new DataUniform<ColorUData>("colorOther", ShaderList);
            OtherColorInter = new DataUniform<LInterData>("colorInterPol", ShaderList);

            GrayInter = new DataUniform<LInterData>("GrayInter", ShaderList);
        }
    }
}
