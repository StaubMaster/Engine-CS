using System;

using Engine3D.Abstract3D;
using Engine3D.GraphicsOld;

namespace Engine3D.GraphicsOld.Forms
{
    public class View
    {
        public Transformation3D Trans;
        public RenderTrans renderTrans;

        public float Fov;

        public RenderDepthFactors renderDepth;


        public View()
        {
            Trans = Transformation3D.Default();

            Fov = 0.5f;

            renderTrans = new RenderTrans(Trans);
            renderDepth = new RenderDepthFactors(1, 100);
        }

        public void Update()
        {
            renderTrans = new RenderTrans(Trans);
        }


        private const string Formal = "+0.00;-0.00; 0.00";
        public override string ToString()
        {
            string str = "";

            Point3D dir = Trans.ToDir();
            Point3D pos = Trans.Pos;

            str += "Dir.Y:" + dir.Y.ToString(Formal) + "  ";
            str += "Pos.Y:" + pos.Y.ToString(Formal) + "\n";

            str += "Dir.X:" + dir.X.ToString(Formal) + "  ";
            str += "Pos.X:" + pos.X.ToString(Formal) + "\n";

            str += "Dir.C:" + dir.C.ToString(Formal) + "  ";
            str += "Pos.C:" + pos.C.ToString(Formal);

            return str;
        }

        public void UniTrans(ViewRenderProgram prog)
        {
            prog.Use();
            prog.UniView(renderTrans);
        }
        public void UniDepth(ViewRenderProgram prog)
        {
            prog.Use();
            prog.UniDepth(renderDepth);
            prog.UniFov(Fov);
        }
    }
}
