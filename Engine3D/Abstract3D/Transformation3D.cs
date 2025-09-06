using System;

namespace Engine3D.Abstract3D
{
    public struct Transformation3D : Graphics.Basic.Data.IData
    {
        public const int Size = Point3D.Size + Angle3D.Size;

        public Point3D Pos;   //  position
        public Angle3D Rot;   //  rotation

        public static Transformation3D Default()
        {
            return new Transformation3D(Point3D.Default(), Angle3D.Default());
        }
        public static Transformation3D Null()
        {
            return new Transformation3D(Point3D.Null(), Angle3D.Null());
        }
        public bool Is()
        {
            return (Pos.Is() || Rot.Is());
        }

        //public Transformation3D() { Pos = Point3D.Default(); Rot = Angle3D.Default(); }
        public Transformation3D(Point3D pos) { Pos = pos; Rot = Angle3D.Default(); }
        public Transformation3D(Angle3D rot) { Pos = Point3D.Default(); Rot = rot; }
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
            if (trans.Is())
            {
                Point3D.ShaderFloats(trans.Pos, flt, idx + 0);
                Angle3D.ShaderFloats(trans.Rot, flt, idx + 3);
            }
            else
            {
                Point3D.ShaderFloats(Point3D.Null(), flt, idx + 0);
                Angle3D.ShaderFloats(Angle3D.Null(), flt, idx + 3);
            }
        }





        public void ToUniform(params int[] locations)
        {
            //Pos.ToUniform(locations[0]);
            //Rot.ToUniform(locations[1]);
            float[] data = new float[9];
            ShaderFloats(this, data, 0);
            OpenTK.Graphics.OpenGL.GL.Uniform3(locations[0], 3, data);
        }

        public static void ToBuffer(int stride, ref System.IntPtr offset, int divisor, params int[] bindIndex)
        {
            Point3D.ToBuffer(stride, ref offset, divisor, bindIndex[0]);
            Angle3D.ToBuffer(stride, ref offset, divisor, bindIndex[1], bindIndex[2]);
        }
    }
}
