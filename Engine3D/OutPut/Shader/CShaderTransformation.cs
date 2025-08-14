using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine3D.OutPut.Uniform;
using Engine3D.Graphics.Shader.Uniform.Float;

namespace Engine3D.OutPut.Shader
{
    public class CShaderTransformation : AShader
    {
        public UniScreenRatio ScreenRatio;

        public UniTransformation ViewTrans;
        public UniDepth ViewDepth;

        public UniTransformation Trans;

        public UniRange LightRange;

        public UniColor Color;
        public UniInter ColorInter;



        public CShaderTransformation(string name, int id) : base(name, id)
        {
            throw new Exception("Removed");
            //ScreenRatio = new UniScreenRatio(GL_Program, "screenRatios");
            //
            //ViewTrans = new UniTransformation(GL_Program, "view");
            //ViewDepth = new UniDepth(GL_Program, "depthFactor");
            //
            //Trans = new UniTransformation(GL_Program, "trans");
            //
            //LightRange = new UniRange(GL_Program, "lightRange");
            //Color = new UniColor(GL_Program, "colorOther");
            //ColorInter = new UniInter(GL_Program, "colorInterPol");
        }

        public void Update()
        {

        }
    }
}
