using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using Engine3D.OutPut.Uniform;

namespace Engine3D.OutPut.Shader
{
    public class CShaderView : AShader
    {
        public SUniformLocation ViewTrans;
        public SUniformLocation ViewDepth;
        public SUniformLocation ScreenRatio;

        public CShaderView(string name, int id) : base(name, id)
        {
            ViewTrans = new SUniformLocation("view", GL_Program);
            ViewDepth = new SUniformLocation("depthFactor", GL_Program);
            ScreenRatio = new SUniformLocation("screenRatios", GL_Program);
        }

        /*public override void UniformDirectUpdate()
        {
            base.UniformDirectUpdate();
            ViewTrans.Update();
            ViewDepth.Update();
            ScreenRatio.Update();
        }*/

        /*public override void UniformDirectRef(string name, AUniformData data)
        {
            ViewTrans.Value(name, data);
            ViewDepth.Value(name, data);
            ScreenRatio.Value(name, data);
        }*/
    }
}
