using System;
using System.Collections.Generic;

using Engine3D.Abstract3D;
using Engine3D.GraphicsOld;

namespace Engine3D.Entity
{
    public class TransBody
    {
        public Transformation3D Trans;
        private readonly AxisBox3D Box;
        private readonly BodyStatic Body;

        public TransBody(BodyStatic body, bool fitBox)
        {
            Body = body;
            if (fitBox)
                Box = Body.BoxFit();
            else
                Box = Body.BoxDist();
            Trans = Transformation3D.Default();
        }
        public TransBody(BodyStatic body, bool fitBox, Transformation3D trans)
        {
            Body = body;
            if (fitBox)
                Box = Body.BoxFit();
            else
                Box = Body.BoxDist();
            Trans = trans;
        }
        public TransBody(BodyStatic body, bool fitBox, Point3D pos)
        {   
            Body = body;
            if (fitBox)
                Box = Body.BoxFit();
            else
                Box = Body.BoxDist();
            Trans = new Transformation3D(pos, Angle3D.Default());
        }


        public double Intersekt(Ray3D ray)
        {
            double t;

            //t = Box.Intersekt(ray, Trans.Pos);
            //if (!Ray.IsPositive(t))
            //    return double.NaN;

            t = Body.Intersekt(ray, Trans, out _);
            if (!Ray3D.IsPositive(t))
                return double.NaN;

            return t;
        }

        public virtual void Draw(TransUniProgram program)
        {
            program.UniTrans(new RenderTrans(Trans));
            Body.BufferDraw();
        }
    }
}
