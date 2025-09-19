using Engine3D.Abstract3D;
using Engine3D.Graphics.Shader;
using Engine3D.Graphics.Shader.Uniform;
using Engine3D.DataStructs;
using Engine3D.Graphics.Basic.Uniforms;

namespace Engine3D.Graphics.Manager
{
    public class PolyHedra_Shader_Manager : GraphicsManager
    {
        /*  Wire
         *      create Individual Shaders for for AxisBox3D and Line3D and such
         *      or create a MainBuffer with the WireFrame of a Box
         *      and then use Instances for Boxes in different places
         *      
         *      how would it work for Lines
         *      Transform and Scale ?
         *      
         *      I kind of want to make a seperate Shader for PolyHedra Wire as well
         *          befor that I want to change Polyhedra Buffers
         *          with Normals and no Element stuff
         *          and maybe Texture-Coords / Color-Indexes
         *          use Element stuff for Wire ?
         *          since the Corners are identical
         */

        //  Global
        public GenericDataUniform<SizeRatio> ViewPortSizeRatio;
        public GenericDataUniform<Transformation3D> View;



        public GenericDataUniform<DepthData> Depth;
        public GenericDataUniform<RangeData> DepthFadeRange;
        public GenericDataUniform<ColorUData> DepthFadeColor;

        public GenericDataUniform<Point3D> LightSolar;
        public GenericDataUniform<RangeData> LightRange;



        //  both Per Body / also Global
        public GenericDataUniform<ColorUData> OtherColor;
        public GenericDataUniform<LInterData> OtherColorInter;

        public GenericDataUniform<LInterData> GrayInter;





        public GenericShader InstShader;
        public GenericShader InstWireShader;

        public GenericShader AxisBoxShader;



        protected override void InitShaders(string shaderDir)
        {
            InstShader = new GenericShader(new ShaderCode[]
            {
                ShaderCode.FromFile(shaderDir + "Inst/Inst.vert"),
                ShaderCode.FromFile(shaderDir + "Inst/Inst.geom"),
                ShaderCode.FromFile(shaderDir + "Inst/Inst.frag"),
            });

            InstWireShader = new GenericShader(new ShaderCode[]
            {
                ShaderCode.FromFile(shaderDir + "Inst/Wire.vert"),
                ShaderCode.FromFile(shaderDir + "Inst/Wire.geom"),
                ShaderCode.FromFile(shaderDir + "Inst/Wire.frag"),
            });

            AxisBoxShader = new GenericShader(new ShaderCode[]
            {
                ShaderCode.FromFile(shaderDir + "Box/Box.vert"),
                ShaderCode.FromFile(shaderDir + "Box/Box.geom"),
                ShaderCode.FromFile(shaderDir + "Box/Box.frag"),
            });

            AppendShaders(
                InstShader,
                InstWireShader,
                AxisBoxShader
                );
        }
        protected override void InitUniforms()
        {
            ViewPortSizeRatio = new GenericDataUniform<SizeRatio>("screenRatios");
            View = new GenericDataUniform<Transformation3D>("view");

            Depth = new GenericDataUniform<DepthData>("depthFactor");
            DepthFadeRange = new GenericDataUniform<RangeData>("depthFadeRange");
            DepthFadeColor = new GenericDataUniform<ColorUData>("depthFadeColor");

            LightSolar = new GenericDataUniform<Point3D>("solar");
            LightRange = new GenericDataUniform<RangeData>("lightRange");

            OtherColor = new GenericDataUniform<ColorUData>("colorOther");
            OtherColorInter = new GenericDataUniform<LInterData>("colorInterPol");

            GrayInter = new GenericDataUniform<LInterData>("GrayInter");

            AppendUniforms(
                ViewPortSizeRatio,
                View,
                Depth, DepthFadeRange,
                DepthFadeColor,
                LightSolar,
                LightRange,
                OtherColor,
                OtherColorInter, GrayInter
                );
        }

        public PolyHedra_Shader_Manager(string shaderDir) : base(shaderDir, null, null) { }
        public PolyHedra_Shader_Manager(
            string shaderDir,
            GenericShader[] shaders,
            GenericUniformBase[] uniforms
            ) : base(shaderDir, shaders, uniforms) { }
    }
}
