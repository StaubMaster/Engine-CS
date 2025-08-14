using System;
using System.Collections.Generic;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.GraphicsOld
{
    public class IconProgram : RenderProgram
    {
        private int Uni_Pos;
        private int Uni_Rot;
        private int Uni_Scale;

        public IconProgram(string name, string vert_file, string geom_file, string frag_file)
            : base(name, vert_file, geom_file, frag_file)
        {

        }

        public override void Create()
        {
            if (Program != -1) { return; }
            base.Create();

            Uni_Pos = UniFind("pos");
            Uni_Rot = UniFind("rot");
            Uni_Scale = UniFind("scale");
        }
        public override void Delete()
        {
            if (Program == -1) { return; }
            base.Delete();

            Uni_Pos = -1;
            Uni_Rot = -1;
            Uni_Scale = -1;
        }

        public void UniPos(float x, float y)
        {
            Use();
            GL.Uniform2(Uni_Pos, x, y);
        }
        public void UniRot(float[] flt)
        {
            Use();
            GL.Uniform3(Uni_Rot, 2, flt);
        }
        public void UniScale(float s)
        {
            Use();
            GL.Uniform1(Uni_Scale, s);
        }
    }
}
