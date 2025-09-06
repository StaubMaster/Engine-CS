using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine3D.Abstract3D;
using Engine3D.Graphics.Shader;
using Engine3D.Graphics.Shader.Uniform;
using Engine3D.Graphics.Basic.Data;
using Engine3D.Graphics.Basic.Uniforms;

using Engine3D.Miscellaneous;

namespace Engine3D.Graphics.Manager
{
    public class PolyHedraTestManager : GraphicsManager
    {
        //  Global
        public GUniData<ScreenRatio> ScreenRatio;
        public GUniData<Transformation3D> View;


        //public GUniDepth Depth;
        public GUniData<DepthData> Depth;
        public GUniData<RangeData> DepthFadeRange;
        public GUniData<ColorUData> DepthFadeColor;

        public GUniData<Point3D> LightSolar;
        public GUniData<RangeData> LightRange;



        //  both Per Body / also Global
        public GUniData<ColorUData> OtherColor;
        public GUniData<LInterData> OtherColorInter;

        public GUniData<LInterData> GrayInter;



        //  only Per Body
        public GUniData<Transformation3D> Trans;


        public GenericShader OARShader;
        public GenericShader InstShader;



        public PolyHedraTestManager(string shaderDir) : base()
        {
            //ConsoleLog.Log("Manager");

            OARShader = new GenericShader(new ShaderCode[]
            {
                ShaderCode.FromFile(shaderDir + "OAR/OAR.vert"),
                ShaderCode.FromFile(shaderDir + "OAR/OAR.geom"),
                ShaderCode.FromFile(shaderDir + "OAR/OAR.frag"),
            });
            InstShader = new GenericShader(new ShaderCode[]
            {
                ShaderCode.FromFile(shaderDir + "Inst/Inst.vert"),
                ShaderCode.FromFile(shaderDir + "Inst/Inst.geom"),
                ShaderCode.FromFile(shaderDir + "Inst/Inst.frag"),
            });

            GenericShader[] ShaderList = new GenericShader[]
            {
                OARShader,
                InstShader,
            };

            ScreenRatio = new GUniData<ScreenRatio>("screenRatios", ShaderList);
            View = new GUniData<Transformation3D>("view", ShaderList);

            Depth = new GUniData<DepthData>("depthFactor", ShaderList);
            DepthFadeRange = new GUniData<RangeData>("depthFadeRange", ShaderList);
            DepthFadeColor = new GUniData<ColorUData>("depthFadeColor", ShaderList);

            LightSolar = new GUniData<Point3D>("solar", ShaderList);
            LightRange = new GUniData<RangeData>("lightRange", ShaderList);

            OtherColor = new GUniData<ColorUData>("colorOther", ShaderList);
            OtherColorInter = new GUniData<LInterData>("colorInterPol", ShaderList);

            GrayInter = new GUniData<LInterData>("GrayInter", ShaderList);

            Trans = new GUniData<Transformation3D>("trans", ShaderList);
        }
    }
}
