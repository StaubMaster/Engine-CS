using Engine3D.Abstract3D;
using Engine3D.Graphics.Shader;
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
        public DataUniform<SizeRatio> ViewPortSizeRatio;
        public DataUniform<Transformation3D> View;



        public DataUniform<DepthData> Depth;
        public DataUniform<RangeData> DepthFadeRange;
        public DataUniform<ColorUData> DepthFadeColor;

        public DataUniform<Point3D> LightSolar;
        public DataUniform<RangeData> LightRange;



        //  both Per Body / also Global
        public DataUniform<ColorUData> OtherColor;
        public DataUniform<LInterData> OtherColorInter;

        public DataUniform<LInterData> GrayInter;





        public GenericShader InstShader;
        public GenericShader InstWireShader;



        public PolyHedra_Shader_Manager(string shaderDir) : base()
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

            GenericShader[] ShaderList = new GenericShader[]
            {
                InstShader,
                InstWireShader,
            };



            ViewPortSizeRatio = new DataUniform<SizeRatio>("screenRatios", ShaderList);
            View = new DataUniform<Transformation3D>("view", ShaderList);

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
