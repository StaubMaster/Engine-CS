using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine3D.OutPut.Uniform;

namespace Engine3D.OutPut.Shader
{
    public class CShaderUserInterfaceBody : AShader
    {
        public SUniformLocation ScreenRatio;
        public SUniformLocation bodyTrans;
        public SUniformLocation bodyScale;
        public SUniformLocation UIRectangle;

        public CShaderUserInterfaceBody(string name, int id) : base(name, id)
        {
            ScreenRatio = new SUniformLocation("screenRatios", GL_Program);
            bodyTrans = new SUniformLocation("bodyTrans", GL_Program);
            bodyScale = new SUniformLocation("bodyScale", GL_Program);
            UIRectangle = new SUniformLocation("UIRectangle", GL_Program);
        }

        /*public override void UniformDirectUpdate()
        {
            base.UniformDirectUpdate();
            ScreenRatio.Update();
            bodyTrans.Update();
            bodyScale.Update();
            UIRectangle.Update();
        }*/

        /*public override void UniformDirectRef(string name, AUniformData data)
        {
            ScreenRatio.Value(name, data);
            bodyTrans.Value(name, data);
            bodyScale.Value(name, data);
            UIRectangle.Value(name, data);
        }*/
    }
}
