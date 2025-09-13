using System;

namespace Engine3D.Abstract2D
{
    public struct Point2D : DataStructs.IData
    {
        public float X;
        public float Y;

        public static Point2D Default()
        {
            return new Point2D(0, 0);
        }
        public static Point2D Null()
        {
            return new Point2D(float.NaN, float.NaN);
        }
        public bool Is()
        {
            return (!float.IsNaN(Y) && !float.IsNaN(X));
        }

        public Point2D(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator Point2D((float, float) values)
        {
            return new Point2D(values.Item1, values.Item2);
        }

        public static Point2D operator +(Point2D p)
        {
            return new Point2D(
                +p.X,
                +p.Y
                );
        }
        public static Point2D operator -(Point2D p)
        {
            return new Point2D(
                -p.X,
                -p.Y
                );
        }

        public static Point2D operator +(Point2D a, Point2D b)
        {
            return new Point2D(
                a.X + b.X,
                a.Y + b.Y
                );
        }
        public static Point2D operator -(Point2D a, Point2D b)
        {
            return new Point2D(
                a.X - b.X,
                a.Y - b.Y
                );
        }
        public static Point2D operator *(Point2D a, Point2D b)
        {
            return new Point2D(
                a.X * b.X,
                a.Y * b.Y
                );
        }
        public static Point2D operator /(Point2D a, Point2D b)
        {
            return new Point2D(
                a.X / b.X,
                a.Y / b.Y
                );
        }

        public static Point2D operator *(Point2D p, float f)
        {
            return new Point2D(
                p.X * f,
                p.Y * f
                );
        }

        public float Len()
        {
            return MathF.Sqrt((X * X) + (Y * Y));
        }
        public Point2D Norm()
        {
            float len = 1 / Len();
            return new Point2D(
                X * len,
                Y * len
                );
        }
        public Point2D PerpX()
        {
            return new Point2D(+Y, -X);
        }
        public Point2D PerpY()
        {
            return new Point2D(-Y, +X);
        }

        public static Point2D IntersektRay(
            Point2D pos0,
            Point2D dir0,
            Point2D pos1,
            Point2D dir1)
        {
            return IntersektRay(pos0, dir0, pos1, dir1, out _, out _, out _);
        }
        public static Point2D IntersektRay(
            Point2D pos0,
            Point2D dir0,
            Point2D pos1,
            Point2D dir1,
            out float f_0,
            out float f_1,
            out float div)
        {
            Point2D dst0 = pos0 + dir0;
            Point2D dst1 = pos1 + dir1;

            f_0 = (pos0.X * dst0.Y) - (pos0.Y * dst0.X);
            f_1 = (pos1.X * dst1.Y) - (pos1.Y * dst1.X);
            div = (dir0.X * dir1.Y) - (dir0.Y * dir1.X);

            return new Point2D(
                ((f_0 * dir1.X) - (dir0.X * f_1)) / div,
                ((f_0 * dir1.Y) - (dir0.Y * f_1)) / div
            );
        }
        public static Point2D IntersektLine(
            Point2D p1,
            Point2D p2,
            Point2D p3,
            Point2D p4)
        {
            Point2D Ldir12 = p1 - p2;
            Point2D Ldir34 = p3 - p4;
            float f12 = (p1.X * p2.Y) - (p1.Y * p2.X);
            float f34 = (p3.X * p4.Y) - (p3.Y * p4.X);
            float div = (Ldir12.X * Ldir34.Y) - (Ldir12.Y * Ldir34.X);

            return new Point2D(
                ((f12 * Ldir34.X) - (Ldir12.X * f34)) / div,
                ((f12 * Ldir34.Y) - (Ldir12.Y * f34)) / div
            );
        }

        public static float Dot(Point2D a, Point2D b)
        {
            return (
                (a.X * b.X) +
                (a.Y * b.Y)
                );
        }





        public void ToUniform(params int[] locations)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform2(locations[0], X, Y);
        }

        public const int SizeOf = sizeof(float) * 2;
        public static void ToBuffer(int stride, ref System.IntPtr offset, int divisor, params int[] bindIndex)
        {
            OpenTK.Graphics.OpenGL4.GL.EnableVertexAttribArray(bindIndex[0]);
            OpenTK.Graphics.OpenGL4.GL.VertexAttribPointer(bindIndex[0], 2, OpenTK.Graphics.OpenGL4.VertexAttribPointerType.Float, false, stride, offset);
            OpenTK.Graphics.OpenGL4.GL.VertexAttribDivisor(bindIndex[0], divisor);
            offset += SizeOf;
        }
    }
}
