using System;

namespace Engine3D.Abstract3D
{
    //  rename to AxisBox
    public class AxisBox3D
    {
        public Point3D Min;
        public Point3D Max;

        private AxisBox3D()
        {
            Min = +Point3D.Inf_P;
            Max = +Point3D.Inf_N;
        }
        private AxisBox3D(Point3D min, Point3D max)
        {
            Min = min;
            Max = max;
        }

        public static AxisBox3D MinMax(Point3D p1, Point3D p2)
        {
            AxisBox3D box = new AxisBox3D();

            box.Min.Y = Math.Min(p1.Y, p2.Y);
            box.Min.X = Math.Min(p1.X, p2.X);
            box.Min.C = Math.Min(p1.C, p2.C);

            box.Max.Y = Math.Max(p1.Y, p2.Y);
            box.Max.X = Math.Max(p1.X, p2.X);
            box.Max.C = Math.Max(p1.C, p2.C);

            return box;
        }
        public static AxisBox3D MinMax(Point3D[] arr)
        {
            AxisBox3D box = new AxisBox3D();

            for (int i = 0; i < arr.Length; i++)
            {
                box.Min.Y = Math.Min(box.Min.Y, arr[i].Y);
                box.Min.X = Math.Min(box.Min.X, arr[i].X);
                box.Min.C = Math.Min(box.Min.C, arr[i].C);

                box.Max.Y = Math.Max(box.Max.Y, arr[i].Y);
                box.Max.X = Math.Max(box.Max.X, arr[i].X);
                box.Max.C = Math.Max(box.Max.C, arr[i].C);
            }

            return box;
        }
        public static AxisBox3D Distance(Point3D[] arr)
        {
            double dist, d;
            dist = 0.0;

            for (int i = 0; i < arr.Length; i++)
            {
                d = arr[i].Len2;
                if (d > dist)
                    dist = d;
            }

            dist = Math.Sqrt(dist);
            return new AxisBox3D(
                new Point3D(-dist, -dist, -dist),
                new Point3D(+dist, +dist, +dist)
                );
        }

        public AxisBox3D Shift(Point3D p)
        {
            return new AxisBox3D(
                Min + p,
                Max + p
                );
        }

        public double MaxSideLen()
        {
            Point3D side = Max - Min;
            return Math.Max(Math.Max(side.Y, side.X), side.C);
        }
        public Point3D Middle()
        {
            return (Min + Max) * 0.5;
        }

        public bool InRangeY(double y)
        {
            return (Min.Y < y && y < Max.Y);
        }
        public bool InRangeX(double x)
        {
            return (Min.X < x && x < Max.X);
        }
        public bool InRangeC(double c)
        {
            return (Min.C < c && c < Max.C);
        }

        public bool InRangeYX(double y, double x)
        {
            return InRangeY(y) && InRangeX(x);
        }
        public bool InRangeXC(double x, double c)
        {
            return InRangeX(x) && InRangeC(c);
        }
        public bool InRangeYC(double y, double c)
        {
            return InRangeY(y) && InRangeC(c);
        }

        public double Intersekt(Ray3D ray, Point3D pos)
        {
            AxisBox3D box0 = new AxisBox3D(Min + pos, Max + pos);

            Point3D inv = new Point3D(
                1.0 / ray.Dir.Y,
                1.0 / ray.Dir.X,
                1.0 / ray.Dir.C);

            AxisBox3D box1 = new AxisBox3D(
                (box0.Min - ray.Pos) * inv,
                (box0.Max - ray.Pos) * inv
                );

            double dist = double.PositiveInfinity;

            if (box1.Min.Y < dist && box0.InRangeXC(box1.Min.Y, box1.Min.Y)) { dist = box1.Min.Y; }
            if (box1.Min.X < dist && box0.InRangeYC(box1.Min.X, box1.Min.X)) { dist = box1.Min.X; }
            if (box1.Min.C < dist && box0.InRangeYX(box1.Min.C, box1.Min.C)) { dist = box1.Min.C; }

            if (box1.Max.Y < dist && box0.InRangeXC(box1.Max.Y, box1.Max.Y)) { dist = box1.Max.Y; }
            if (box1.Max.X < dist && box0.InRangeYC(box1.Max.X, box1.Max.X)) { dist = box1.Max.X; }
            if (box1.Max.C < dist && box0.InRangeYX(box1.Max.C, box1.Max.C)) { dist = box1.Max.C; }

            if (double.IsPositiveInfinity(dist))
                return double.NaN;
            return dist;
        }
        public static bool Intersekt(AxisBox3D a, AxisBox3D b)
        {
            return (a.Min.Y < b.Max.Y) && (a.Max.Y > b.Min.Y) &&
                   (a.Min.X < b.Max.X) && (a.Max.X > b.Min.X) &&
                   (a.Min.C < b.Max.C) && (a.Max.C > b.Min.C);
        }
    }
}
