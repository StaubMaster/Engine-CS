using System;
using System.Collections.Generic;

namespace Engine3D.Abstract3D
{
    public static class Intersekt
    {
        /* TODO
         *  put RayInterval into Ray as Interval
         *  this is all Ray stuff, put it all in Ray
         *  except maybe stuff like Box vs Box Intersekt stuff, which is in their respective classes anyway
         */
        public struct RayInterval
        {
            private readonly Ray3D Ray;
            public readonly bool Is;
            public readonly double Interval;
            public readonly int Index;
            public RayInterval(Ray3D ray)
            {
                Ray = ray;
                Is = false;
                Interval = double.PositiveInfinity;
                Index = -1;
            }
            public RayInterval(Ray3D ray, double interval)
            {
                Ray = ray;
                Is = true;
                Interval = interval;
                Index = -1;
            }
            public RayInterval(Ray3D ray, double interval, int idx)
            {
                Ray = ray;
                Is = (idx != -1);
                Interval = interval;
                Index = idx;
            }

            public bool IsScalar { get { return Ray3D.IsScalar(Interval); } }
            public bool IsPositive { get { return Ray3D.IsPositive(Interval); } }
            public Point3D Pos { get { return Ray.Scale(Interval); } }
        }



        public static RayInterval Ray_Triangle(Ray3D ray, Point3D a, Point3D b, Point3D c)
        {
            //      8+      15-     24*     3/

            Point3D diff_a_b, diff_a_c, diff_nach_a;
            diff_a_b = b - a;
            diff_a_c = c - a;
            diff_nach_a = ray.Pos - a;

            double p, u, v, t;
            Point3D normale_zu_;

            normale_zu_ = diff_a_c ^ ray.Dir;
            p = normale_zu_ % diff_a_b;
            u = normale_zu_ % diff_nach_a;

            normale_zu_ = diff_a_b ^ diff_nach_a;
            v = normale_zu_ % ray.Dir;
            t = normale_zu_ % diff_a_c;

            u /= p;
            v /= p;
            t /= p;
            if (0.0 <= u && u <= 1.0)
            {
                if (0.0 <= v && (u + v) <= 1.0)
                {
                    return new RayInterval(ray, t);
                }
            }
            return new RayInterval(ray, double.NaN);
        }


        /*
            Plane:
                p + dir1 * t1 + dir2 * t2
            Ray:
                pos + dir * t

            normal = dir1 x dir2
            (p - (pos - dir * t)) * normal = 0
            (p + pos - dir * t) * normal = 0
            (p + pos) * normal - (dir * t) * normal = 0
            (p + pos) * normal = (dir * t) * normal
            (p + pos) * normal = (dir * t) * normal

            (p.y + pos.y) * normal.y +
            (p.x + pos.x) * normal.x +
            (p.c + pos.c) * normal.c
            =
            (dir.y * t) * normal.y +
            (dir.x * t) * normal.x +
            (dir.c * t) * normal.c

            (p + pos) * normal = (dir * normal) * t

            (p.y + pos.y) * normal.y +
            (p.x + pos.x) * normal.x +
            (p.c + pos.c) * normal.c
            =
            ((dir.y * normal.y) +
             (dir.x * normal.x) +
             (dir.c * normal.c)) * t

            ((p.y + pos.y) * normal.y +
             (p.x + pos.x) * normal.x +
             (p.c + pos.c) * normal.c)
            /
            ((dir.y * normal.y) +
             (dir.x * normal.x) +
             (dir.c * normal.c))
            = t

            ((p + pos) * normal) / (dir * normal) = t
        */
        public static RayInterval Ray_Plane(Ray3D ray, Point3D p, Point3D dir1, Point3D dir2)
        {
            Point3D normal = dir1 ^ dir2;
            Point3D rel = p - ray.Pos;

            double d1, d2;
            d2 = ray.Dir % normal;
            if (d2 != 0.0)
            {
                d1 = rel % normal;
                return new RayInterval(ray, d1 / d2);
            }
            return new RayInterval(ray, double.NaN);
        }



        public static RayInterval Ray_Point(Ray3D ray, Point3D p)
        {
            double skal;
            skal = ray.Dir % (ray.Pos - p);

            double sqr;
            sqr = ray.Dir % ray.Dir;

            return new RayInterval(ray, -(skal / sqr));
        }



        public static void Ray_Ray(in Ray3D a, out RayInterval a_t,
                                   in Ray3D b, out RayInterval b_t)
        {
            Point3D norm;
            norm = a.Dir ^ b.Dir;

            double norm_skal_inv;
            norm_skal_inv = 1.0 / (norm % norm);

            Point3D rel;
            rel = b.Pos - a.Pos;

            a_t = new RayInterval(a, ((b.Dir ^ norm) % rel) * norm_skal_inv);
            b_t = new RayInterval(b, ((a.Dir ^ norm) % rel) * norm_skal_inv);
        }



        public static RayInterval Ray_Multiple<T>(Ray3D ray, T[] arr, Func<Ray3D, T, RayInterval> func)
        {
            RayInterval allInter = new RayInterval(ray);

            for (int i = 0; i < arr.Length; i++)
            {
                RayInterval inter = func(ray, arr[i]);
                if (inter.IsPositive && inter.Interval < allInter.Interval)
                {
                    allInter = new RayInterval(ray, inter.Interval, i);
                }
            }

            return allInter;
        }
        public static RayInterval Ray_Multiple<T>(Ray3D ray, List<T> list, Func<Ray3D, T, RayInterval> func)
        {
            RayInterval allInter = new RayInterval(ray);

            for (int i = 0; i < list.Count; i++)
            {
                RayInterval inter = func(ray, list[i]);
                if (inter.IsPositive && inter.Interval < allInter.Interval)
                {
                    allInter = new RayInterval(ray, inter.Interval, i);
                }
            }

            return allInter;
        }
    }
}
