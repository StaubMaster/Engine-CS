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

        public GenericShader AxisBoxShader;

        public GenericShader[] Auxiliary;

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

            AxisBoxShader = new GenericShader(new ShaderCode[]
            {
                ShaderCode.FromFile(shaderDir + "Box/Box.vert"),
                ShaderCode.FromFile(shaderDir + "Box/Box.geom"),
                ShaderCode.FromFile(shaderDir + "Box/Box.frag"),
            });
        }
        public PolyHedra_Shader_Manager(string shaderDir, params GenericShader[] shaders) : base()
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

            Auxiliary = shaders;
        }

        protected override GenericShader[] AllShaders()
        {
            GenericShader[] main = new GenericShader[]
            {
                InstShader,
                InstWireShader,
                AxisBoxShader,
            };

            GenericShader[] shaders = new GenericShader[main.Length + Auxiliary.Length];
            int i;
            for (i = 0; i < main.Length; i++) { shaders[i] = main[i]; }
            for (int j = 0; j < Auxiliary.Length; j++) { shaders[i] = Auxiliary[j]; i++; }

            return shaders;
        }
        protected override void InitUniforms(GenericShader[] shaders)
        {
            ViewPortSizeRatio = new DataUniform<SizeRatio>("screenRatios", shaders);
            View = new DataUniform<Transformation3D>("view", shaders);

            Depth = new DataUniform<DepthData>("depthFactor", shaders);
            DepthFadeRange = new DataUniform<RangeData>("depthFadeRange", shaders);
            DepthFadeColor = new DataUniform<ColorUData>("depthFadeColor", shaders);

            LightSolar = new DataUniform<Point3D>("solar", shaders);
            LightRange = new DataUniform<RangeData>("lightRange", shaders);

            OtherColor = new DataUniform<ColorUData>("colorOther", shaders);
            OtherColorInter = new DataUniform<LInterData>("colorInterPol", shaders);

            GrayInter = new DataUniform<LInterData>("GrayInter", shaders);
        }
    }
}
