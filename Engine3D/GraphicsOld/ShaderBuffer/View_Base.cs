using System;
using System.Collections.Generic;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Engine3D.GraphicsOld
{
    public abstract class ViewRenderProgram : RenderProgram
    {
        private int Uni_View;
        private int Uni_Depth;
        private int Uni_Fov;

        public ViewRenderProgram(string name, string vert_file, string geom_file, string frag_file)
            : base(name, vert_file, geom_file, frag_file)
        {

        }

        public override void Create()
        {
            if (Program != -1) { return; }
            base.Create();

            Uni_View = GL.GetUniformLocation(Program, "view");
            Uni_Depth = GL.GetUniformLocation(Program, "depthFactor");
            Uni_Fov = GL.GetUniformLocation(Program, "fov");
        }
        public override void Delete()
        {
            if (Program == -1) { return; }
            base.Delete();

            Uni_View = -1;
            Uni_Depth = -1;
            Uni_Fov = -1;
        }

        public void UniDepthFov(float near, float far, float fov)
        {
            Use();

            float[] depthF = new float[7]
            {
                near,   // 0
                far,    // 1
                far - near,     // 2
                near + far,     // 3
                near * far * 2, // 4
                0,
                0,
            };
            depthF[5] = depthF[3] / depthF[2];
            depthF[6] = depthF[4] / depthF[2];

            GL.Uniform1(Uni_Depth, 7, depthF);

            GL.Uniform1(Uni_Fov, fov);
        }
        public void UniView(float[] view)
        {
            Use();
            GL.Uniform3(Uni_View, 3, view);
        }

        public void UniView(RenderTrans view)
        {
            Use();
            RenderTrans.Uniform(view, Uni_View);
        }
        public void UniDepth(RenderDepthFactors depth)
        {
            Use();
            RenderDepthFactors.Uniform(depth, Uni_Depth);
        }
        public void UniFov(float fov)
        {
            Use();
            GL.Uniform1(Uni_Fov, fov);
        }
    }
}
