using System;

namespace Engine3D.Abstract3D
{
    public struct Point3D : DataStructs.IData
    {
        public float Y;
        public float X;
        public float C;

        public static Point3D Default()
        {
            return new Point3D(0, 0, 0);
        }
        public static Point3D NaN()
        {
            return new Point3D(float.NaN, float.NaN, float.NaN);
        }
        public bool IsNaN()
        {
            return (double.IsNaN(Y) || double.IsNaN(X) || double.IsNaN(C));
        }

        /*public Point3D()
        {
            Y = 0;
            X = 0;
            C = 0;
        }*/
        public Point3D(float y, float x, float c)
        {
            Y = y;
            X = x;
            C = c;
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

        public static Point3D operator *(Point3D p, float d)
        {
            return new Point3D(
                p.Y * d,
                p.X * d,
                p.C * d);
        }


        public static Point3D operator !(Point3D p)
        {
            return p * (1.0f / p.Len);
        }
        public float Len2
        {
            get
            {
                return (Y * Y) + (X * X) + (C * C);
            }
        }
        public float Len
        {
            get
            {
                return MathF.Sqrt(Len2);
            }
        }



        //  Dot Product
        public static float operator %(Point3D a, Point3D b)
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



        public static readonly Point3D Inf_P = new Point3D(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        public static readonly Point3D Inf_N = new Point3D(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);



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
            if (!pkt.IsNaN())
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

        public const int SizeOf = sizeof(float) * 3;
        public static void ToBuffer(int stride, ref System.IntPtr offset, int divisor, params int[] bindIndex)
        {
            OpenTK.Graphics.OpenGL.GL.EnableVertexAttribArray(bindIndex[0]);
            OpenTK.Graphics.OpenGL.GL.VertexAttribPointer(bindIndex[0], 3, OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float, false, stride, offset);
            OpenTK.Graphics.OpenGL.GL.VertexAttribDivisor(bindIndex[0], divisor);
            offset += SizeOf;
        }
    }
}
