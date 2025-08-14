using System;

namespace Engine3D.Abstract3D
{
    public class Angle3D
    {
        private double _A;   // Y _ C
        private double _S;   // _ X C
        private double _D;   // Y X _

        private double sinA;
        private double sinS;
        private double sinD;

        private double cosA;
        private double cosS;
        private double cosD;

        public double A
        {
            get { return _A; }
            set { _A = value; SinCos(_A, ref sinA, ref cosA); }
        }
        public double S
        {
            get { return _S; }
            set { _S = value; SinCos(_S, ref sinS, ref cosS); }
        }
        public double D
        {
            get { return _D; }
            set { _D = value; SinCos(_D, ref sinD, ref cosD); }
        }


        public Angle3D()
        {
            _A = 0;
            _S = 0;
            _D = 0;

            sinA = 0;
            sinS = 0;
            sinD = 0;

            cosA = 1;
            cosS = 1;
            cosD = 1;
        }
        public Angle3D(double a, double s, double d)
        {
            _A = a;
            _S = s;
            _D = d;

            SinCos();
        }
        public Angle3D(Point3D dir)
        {
            double len;
            len = (dir.Y * dir.Y) + (dir.C * dir.C);

            _A = Math.Atan2(dir.Y, dir.C);
            _S = Math.Atan2(dir.X, Math.Sqrt(len));
            _D = 0;

            SinCos();
        }

        public static double DegreeToRad(double degree)
        {
            return ((degree * Math.Tau) / 360);
        }
        public static double RadToDegree(double rad)
        {
            return ((rad * 360) / Math.Tau);
        }
        public static Angle3D FromDegree(double a, double s, double d)
        {
            return new Angle3D(
                DegreeToRad(a),
                DegreeToRad(s),
                DegreeToRad(d)
            );
        }

        private static void SinCos(double w, ref double sin, ref double cos)
        {
            sin = Math.Sin(w);
            cos = Math.Cos(w);
        }
        private void SinCos()
        {
            SinCos(_A, ref sinA, ref cosA);
            SinCos(_S, ref sinS, ref cosS);
            SinCos(_D, ref sinD, ref cosD);
        }



        private static void rotate(ref double pls, ref double mns, double sin, double cos)
        {
            double tmp;
            tmp = cos * pls - sin * mns;
            mns = cos * mns + sin * pls;
            pls = tmp;
        }

        public static Point3D operator +(Point3D p, Angle3D w)
        {
            Point3D n = +p;
            rotate(ref n.Y, ref n.C, w.sinA, w.cosA);
            rotate(ref n.X, ref n.C, w.sinS, w.cosS);
            rotate(ref n.X, ref n.Y, w.sinD, w.cosD);
            return n;
        }
        public static Point3D operator -(Point3D p, Angle3D w)
        {
            Point3D n = +p;
            rotate(ref n.Y, ref n.X, w.sinD, w.cosD);
            rotate(ref n.C, ref n.X, w.sinS, w.cosS);
            rotate(ref n.C, ref n.Y, w.sinA, w.cosA);
            return n;
        }

        public static Angle3D operator +(Angle3D a, Angle3D b)
        {
            Point3D pY, pX, pC;
            pY = (new Point3D(1, 0, 0) + a) + b;
            pX = (new Point3D(0, 1, 0) + a) + b;
            pC = (new Point3D(0, 0, 1) + a) + b;

            return new Angle3D(
                Math.Atan2(pY.C, pC.C),
                Math.Asin(pX.C),
                Math.Atan2(pX.Y, pX.X));
        }
        public static Angle3D operator -(Angle3D a, Angle3D b)
        {
            Point3D pY, pX, pC;
            pY = (new Point3D(1, 0, 0) - a) - b;
            pX = (new Point3D(0, 1, 0) - a) - b;
            pC = (new Point3D(0, 0, 1) - a) - b;

            return new Angle3D(
                Math.Atan2(pC.Y, pC.C),
                Math.Asin(pC.X),
                Math.Atan2(pY.X, pX.X));
        }

        public Angle3D InvertPls()
        {
            Point3D pY, pX, pC;
            pY = (new Point3D(1, 0, 0) + this);
            pX = (new Point3D(0, 1, 0) + this);
            pC = (new Point3D(0, 0, 1) + this);

            return new Angle3D(
                Math.Atan2(pC.Y, pC.C),
                Math.Asin(pC.X),
                Math.Atan2(pY.X, pX.X));
        }
        public Angle3D InvertMns()
        {
            Point3D pY, pX, pC;
            pY = (new Point3D(1, 0, 0) - this);
            pX = (new Point3D(0, 1, 0) - this);
            pC = (new Point3D(0, 0, 1) - this);

            return new Angle3D(
                Math.Atan2(pY.C, pC.C),
                Math.Asin(pX.C),
                Math.Atan2(pX.Y, pX.X));
        }

        public Angle3D Add(Angle3D w)
        {
            return new Angle3D(
                _A + w._A,
                _S + w._S,
                _D + w._D
                );
        }
        public void Inc(Angle3D w)
        {
            _A += w._A;
            _S += w._S;
            _D += w._D;
            SinCos();
        }

        public Angle3D Axis()
        {
            return new Angle3D(
                Math.Round(_A / Quad) * Quad,
                Math.Round(_S / Quad) * Quad,
                Math.Round(_D / Quad) * Quad
                );
        }



        public static Angle3D InterPolate(Angle3D w0, Angle3D w1, double t0)
        {
            double t1 = 1.0 - t0;
            return new Angle3D(
                w0._A * t1 + w1._A * t0,
                w0._S * t1 + w1._S * t0,
                w0._D * t1 + w1._D * t0
                );
        }



        public const double Full = Math.PI * 2.0;
        public const double Half = Math.PI;
        public const double Quad = Math.PI / 2.0;

        public static readonly Angle3D Yp = new Angle3D(Math.PI * 0.0, 0, 0);
        public static readonly Angle3D Yn = new Angle3D(Math.PI * 1.0, 0, 0);

        public static readonly Angle3D Cp = new Angle3D(+Quad, 0, 0);
        public static readonly Angle3D Cn = new Angle3D(-Quad, 0, 0);

        public static readonly Angle3D Xp = new Angle3D(0, +Quad, 0);
        public static readonly Angle3D Xn = new Angle3D(0, -Quad, 0);



        public void FloatsSinCos(float[] flt, ref int idx)
        {
            flt[idx] = (float)sinA; idx++;
            flt[idx] = (float)sinS; idx++;
            flt[idx] = (float)sinD; idx++;
            flt[idx] = (float)cosA; idx++;
            flt[idx] = (float)cosS; idx++;
            flt[idx] = (float)cosD; idx++;
        }
        public void FloatsSinCos(float[] flt, int idx = 0)
        {
            flt[idx] = (float)sinA; idx++;
            flt[idx] = (float)sinS; idx++;
            flt[idx] = (float)sinD; idx++;
            flt[idx] = (float)cosA; idx++;
            flt[idx] = (float)cosS; idx++;
            flt[idx] = (float)cosD; idx++;
        }
        public static void ShaderFloats(Angle3D wnk, float[] flt, int idx)
        {
            if (wnk != null)
            {
                flt[idx] = (float)wnk.sinA; idx++;
                flt[idx] = (float)wnk.sinS; idx++;
                flt[idx] = (float)wnk.sinD; idx++;
                flt[idx] = (float)wnk.cosA; idx++;
                flt[idx] = (float)wnk.cosS; idx++;
                flt[idx] = (float)wnk.cosD; idx++;
            }
            else
            {
                flt[idx] = 0; idx++;
                flt[idx] = 0; idx++;
                flt[idx] = 0; idx++;
                flt[idx] = 1; idx++;
                flt[idx] = 1; idx++;
                flt[idx] = 1; idx++;
            }
        }
    }
}
