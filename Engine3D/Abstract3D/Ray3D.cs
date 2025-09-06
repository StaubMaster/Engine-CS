using System;

namespace Engine3D.Abstract3D
{
    public class Ray3D
    {
        public Point3D Pos;
        public Point3D Dir;

        /*private static Ray3D Default()
        {
            Ray3D ray = new Ray3D();
            ray.Pos = new Point3D();
            ray.Dir = new Point3D();
            return ray;
        }*/
        public Ray3D()
        {
            Pos = Point3D.Default();
            Dir = Point3D.Default();
        }
        public Ray3D(Point3D pos, Point3D dir)
        {
            Pos = pos;
            Dir = dir;
        }

        public Transformation3D ToTrans()
        {
            return new Transformation3D(Pos, Dir);
        }
        public Point3D Scale(double t)
        {
            if (double.IsNaN(t))
                return Point3D.Null();
            return Pos + (Dir * t);
        }

        public void LineFunc2D(Func<int, int, double, bool> func, int limit)
        {
            Dir = Dir * (1.0 / Math.Sqrt((Dir.Y * Dir.Y) + (Dir.C * Dir.C)));

            double unit_y, unit_c;
            unit_y = Dir.C / Dir.Y;
            unit_c = Dir.Y / Dir.C;
            unit_y = Math.Sqrt(1 + unit_y * unit_y);
            unit_c = Math.Sqrt(1 + unit_c * unit_c);

            int idx_y, idx_c;
            idx_y = (int)Math.Floor(Pos.Y);
            idx_c = (int)Math.Floor(Pos.C);

            double sum_y, sum_c, sum;
            int idx_dir_y, idx_dir_c;

            if (Dir.Y < 0)
            {
                idx_dir_y = -1;
                sum_y = (Pos.Y - idx_y) * unit_y;
            }
            else
            {
                idx_dir_y = +1;
                sum_y = ((idx_y + 1) - Pos.Y) * unit_y;
            }

            if (Dir.C < 0)
            {
                idx_dir_c = -1;
                sum_c = (Pos.C - idx_c) * unit_c;
            }
            else
            {
                idx_dir_c = +1;
                sum_c = ((idx_c + 1) - Pos.C) * unit_c;
            }

            sum = 0;

            while (func(idx_y, idx_c, sum) && --limit > 0)
            {
                if (sum_y < sum_c)
                {
                    sum = sum_y;
                    idx_y += idx_dir_y;
                    sum_y += unit_y;
                }
                else
                {
                    sum = sum_c;
                    idx_c += idx_dir_c;
                    sum_c += unit_c;
                }
            }
        }
        public void LineFunc2D<T>(Func<int, int, double, T, bool> func, int limit, T t)
        {
            Dir = Dir * (1.0 / Math.Sqrt((Dir.Y * Dir.Y) + (Dir.C * Dir.C)));

            double unit_y, unit_c;
            unit_y = Dir.C / Dir.Y;
            unit_c = Dir.Y / Dir.C;
            unit_y = Math.Sqrt(1 + unit_y * unit_y);
            unit_c = Math.Sqrt(1 + unit_c * unit_c);

            int idx_y, idx_c;
            idx_y = (int)Math.Floor(Pos.Y);
            idx_c = (int)Math.Floor(Pos.C);

            double sum_y, sum_c, sum;
            int idx_dir_y, idx_dir_c;

            if (Dir.Y < 0)
            {
                idx_dir_y = -1;
                sum_y = (Pos.Y - idx_y) * unit_y;
            }
            else
            {
                idx_dir_y = +1;
                sum_y = ((idx_y + 1) - Pos.Y) * unit_y;
            }

            if (Dir.C < 0)
            {
                idx_dir_c = -1;
                sum_c = (Pos.C - idx_c) * unit_c;
            }
            else
            {
                idx_dir_c = +1;
                sum_c = ((idx_c + 1) - Pos.C) * unit_c;
            }

            sum = 0;

            while (func(idx_y, idx_c, sum, t) && --limit > 0)
            {
                if (sum_y < sum_c)
                {
                    sum = sum_y;
                    idx_y += idx_dir_y;
                    sum_y += unit_y;
                }
                else
                {
                    sum = sum_c;
                    idx_c += idx_dir_c;
                    sum_c += unit_c;
                }
            }
        }

        public static bool IsScalar(double t)
        {
            return (!double.IsNaN(t));
        }
        public static bool IsPositive(double t)
        {
            return (!double.IsNaN(t) && t > 0.0);
        }

        public static double FindMin(double[] t, out int idx)
        {
            double s = double.NaN;

            idx = -1;
            for (int i = 0; i < t.Length; i++)
            {
                if (IsPositive(t[i]) && !(t[i] > s))
                {
                    s = t[i];
                    idx = i;
                }
            }

            return s;
        }





        public double Dreieck_Schnitt_Interval(Point3D a, Point3D b, Point3D c)
        {
            //      8+      15-     24*     3/

            Point3D diff_a_b, diff_a_c, diff_nach_a;
            diff_a_b = b - a;
            diff_a_c = c - a;
            diff_nach_a = Pos - a;

            double p, u, v, t;
            Point3D normale_zu_;

            normale_zu_ = diff_a_c ^ Dir;
            p = normale_zu_ % diff_a_b;
            u = normale_zu_ % diff_nach_a;

            normale_zu_ = diff_a_b ^ diff_nach_a;
            v = normale_zu_ % Dir;
            t = normale_zu_ % diff_a_c;

            u /= p;
            v /= p;
            t /= p;
            if (0.0 <= u && u <= 1.0)
            {
                if (0.0 <= v && (u + v) <= 1.0)
                {
                    return t;
                }
            }
            return double.NaN;
        }
        public double Plane_Schnitt_Interval(Point3D p, Point3D dir1, Point3D dir2)
        {
            Point3D normal = dir1 ^ dir2;
            Point3D rel = p - Pos;

            double d1, d2;
            d2 = Dir % normal;
            if (d2 != 0.0)
            {
                d1 = rel % normal;
                return d1 / d2;
            }
            return double.NaN;
        }
    }
}
