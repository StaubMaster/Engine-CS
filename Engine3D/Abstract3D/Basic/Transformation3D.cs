using System;

namespace Engine3D.Abstract3D
{
    public class Transformation3D
    {
        public Point3D Pos;   //  position
        public Angle3D Rot;   //  rotation

        public Transformation3D() { Pos = new Point3D(); Rot = new Angle3D(); }
        public Transformation3D(Point3D pos) { Pos = pos; Rot = new Angle3D(); }
        public Transformation3D(Angle3D rot) { Pos = new Point3D(); Rot = rot; }
        public Transformation3D(Point3D pos, Angle3D rot) { Pos = pos; Rot = rot; }
        public Transformation3D(Point3D pos, Point3D dir) { Pos = pos; Rot = new Angle3D(dir); }



        public Point3D TFore(Point3D pos)
        {
            return (pos - Rot) + Pos;
        }
        public Point3D TBack(Point3D pos)
        {
            return (pos - Pos) + Rot;
        }
        public Angle3D TFore(Angle3D wnk)
        {
            return wnk + Rot;
        }
        public Angle3D TBack(Angle3D wnk)
        {
            return wnk + Rot;
        }
        public Transformation3D TFore(Transformation3D trans)
        {
            return new Transformation3D(
                TFore(trans.Pos),
                TFore(trans.Rot)
                );
        }
        public Transformation3D TBack(Transformation3D trans)
        {
            return new Transformation3D(
                TBack(trans.Pos),
                TBack(trans.Rot)
                );
        }



        public Point3D ToDir()
        {
            return new Point3D(0, 0, 1) - Rot;
        }
        public Ray3D ToRay()
        {
            return new Ray3D(Pos, ToDir());
        }



        public static void ShaderFloats(Transformation3D trans, float[] flt, int idx)
        {
            if (trans != null)
            {
                Point3D.ShaderFloats(trans.Pos, flt, idx + 0);
                Angle3D.ShaderFloats(trans.Rot, flt, idx + 3);
            }
            else
            {
                Point3D.ShaderFloats(null, flt, idx + 0);
                Angle3D.ShaderFloats(null, flt, idx + 3);
            }
        }
    }
}
