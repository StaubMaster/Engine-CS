using System;

namespace Engine3D.Abstract3D
{
    public struct Point3D : Graphics.Basic.Data.IData
    {
        public const int Size = sizeof(float) * 3;

        public float Y;
        public float X;
        public float C;

        public static Point3D Default()
        {
            return new Point3D(0, 0, 0);
        }
        public static Point3D Null()
        {
            return new Point3D(double.NaN, double.NaN, double.NaN);
        }
        public bool Is()
        {
            return (!double.IsNaN(Y) && !double.IsNaN(X) && !double.IsNaN(C));
        }

        /*public Point3D()
        {
            Y = 0;
            X = 0;
            C = 0;
        }*/
        public Point3D(double y, double x, double c)
        {
            Y = (float)y;
            X = (float)x;
            C = (float)c;
        }



        public static Point3D operator +(Point3D p)
        {
            return new Point3D(
                +p.Y,
                +p.X,
                +p.C);
        }
        public static Point3D operator -(Point3D p)
        {
            return new Point3D(
                -p.Y,
                -p.X,
                -p.C);
        }

        public static Point3D operator +(Point3D a, Point3D b)
        {
            return new Point3D(
                a.Y + b.Y,
                a.X + b.X,
                a.C + b.C);
        }
        public static Point3D operator -(Point3D a, Point3D b)
        {
            return new Point3D(
                a.Y - b.Y,
                a.X - b.X,
                a.C - b.C);
        }
        public static Point3D operator *(Point3D a, Point3D b)
        {
            return new Point3D(
                a.Y * b.Y,
                a.X * b.X,
                a.C * b.C);
        }

        public static Point3D operator *(Point3D p, double d)
        {
            return new Point3D(
                p.Y * d,
                p.X * d,
                p.C * d);
        }


        public static Point3D operator !(Point3D p)
        {
            return p * (1.0 / p.Len);
        }
        public double Len2
        {
            get
            {
                return (Y * Y) + (X * X) + (C * C);
            }
        }
        public double Len
        {
            get
            {
                return Math.Sqrt(Len2);
            }
        }



        //  Dot Product
        public static double operator %(Point3D a, Point3D b)
        {
            return
                a.Y * b.Y +
                a.X * b.X +
                a.C * b.C;
        }
        //  Cross Product
        public static Point3D operator ^(Point3D a, Point3D b)
        {
            return new Point3D(
                a.X * b.C - a.C * b.X,
                a.C * b.Y - a.Y * b.C,
                a.Y * b.X - a.X * b.Y
                );
        }



        public static readonly Point3D Inf_P = new Point3D(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
        public static readonly Point3D Inf_N = new Point3D(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity);
        public static readonly Point3D NaN = new Point3D(double.NaN, double.NaN, double.NaN);



        public void Floats(float[] flt, ref int idx)
        {
            flt[idx] = (float)Y; idx++;
            flt[idx] = (float)X; idx++;
            flt[idx] = (float)C; idx++;
        }
        public void Floats(float[] flt, int idx = 0)
        {
            flt[idx] = (float)Y; idx++;
            flt[idx] = (float)X; idx++;
            flt[idx] = (float)C; idx++;
        }
        public static void ShaderFloats(Point3D pkt, float[] flt, int idx)
        {
            if (pkt.Is())
            {
                flt[idx] = (float)pkt.Y; idx++;
                flt[idx] = (float)pkt.X; idx++;
                flt[idx] = (float)pkt.C; idx++;
            }
            else
            {
                flt[idx] = 0; idx++;
                flt[idx] = 0; idx++;
                flt[idx] = 0; idx++;
            }
        }

        public string ToString_Line(string Format = "+0.00;-0.00; 0.00")
        {
            string str = "";
            str += Y.ToString(Format) + " , ";
            str += X.ToString(Format) + " , ";
            str += C.ToString(Format);
            return str;
        }





        public void ToUniform(params int[] locations)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform3(locations[0], Y, X, C);
        }

        public static void ToBuffer(int stride, ref System.IntPtr offset, int divisor, params int[] bindIndex)
        {
            OpenTK.Graphics.OpenGL.GL.EnableVertexAttribArray(bindIndex[0]);
            OpenTK.Graphics.OpenGL.GL.VertexAttribPointer(bindIndex[0], 3, OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float, false, stride, offset);
            OpenTK.Graphics.OpenGL.GL.VertexAttribDivisor(bindIndex[0], divisor);
            offset += Size;
        }
    }
}
