using System;

namespace Engine3D.Abstract2D
{
/*  Min Max Overlap
|###|###|###|###|###|
|   0---1           | False
|           2---3   | 0<2 0<3 1<2 1<3
|###|###|###|###|###|
|   2---3           | False
|           0---1   | 0>2 0>3 1>2 1>3
|###|###|###|###|###|
|   0-----------1   | True
|       2---3       | 0<2 0<3 1>2 1>3
|###|###|###|###|###|
|       0---1       | True
|   2-----------3   | 0>2 0<3 1>2 1<3
|###|###|###|###|###|
|   0-------1       | True
|       2-------3   | 0<2 0<3 1>2 1<3
|###|###|###|###|###|
|   2-------3       | True
|       0-------1   | 0>2 0<3 1>2 1>3
|###|###|###|###|###|
|                   |     0<3 1>2
|###|###|###|###|###|
*/

    public struct AxisBox2D : DataStructs.IData
    {
        public Point2D Min;
        public Point2D Max;

        public static AxisBox2D Default()
        {
            AxisBox2D ab = new AxisBox2D();
            ab.Min = new Point2D(float.PositiveInfinity, float.PositiveInfinity);
            ab.Min = new Point2D(float.NegativeInfinity, float.NegativeInfinity);
            return ab;
        }
        private AxisBox2D(Point2D min, Point2D max)
        {
            Min = min;
            Max = max;
        }



        public static AxisBox2D MinMax(Point2D p0, Point2D p1)
        {
            AxisBox2D box = new AxisBox2D();

            box.Min.X = MathF.Min(p0.X, p1.X);
            box.Min.Y = MathF.Min(p0.Y, p1.Y);

            box.Max.X = MathF.Max(p0.X, p1.X);
            box.Max.Y = MathF.Max(p0.Y, p1.Y);

            return box;
        }
        public static AxisBox2D MinMax(Point2D[] arr)
        {
            AxisBox2D box = Default();

            for (int i = 0; i < arr.Length; i++)
            {
                box.Min.X = MathF.Min(box.Min.X, arr[i].X);
                box.Min.Y = MathF.Min(box.Min.Y, arr[i].Y);

                box.Max.X = MathF.Max(box.Max.X, arr[i].X);
                box.Max.Y = MathF.Max(box.Max.Y, arr[i].Y);
            }

            return box;
        }



        private bool IntersektX(float x)
        {
            return (Min.X < x && Max.X > x);
        }
        private bool IntersektY(float y)
        {
            return (Min.Y < y && Max.Y > y);
        }

        public bool Intersekt(Point2D p)
        {
            return (IntersektX(p.X) && IntersektY(p.Y));
        }



        public void ToUniform(params int[] locations)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform2(locations[0], 4, new float[4] { Min.X, Min.Y, Max.X, Max.Y });
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
