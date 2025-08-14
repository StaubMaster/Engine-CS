using System;
using System.Collections.Generic;
using System.IO;

using Engine3D.Abstract3D;
using Engine3D.GraphicsOld;

namespace Engine3D.Entity
{
    public partial class BodyStatic
    {
        private Point3D[] Ecken;
        private Tri[] Seiten;
        private TransUniBuffers Buffer;

        public struct Tri
        {
            public uint A;
            public uint B;
            public uint C;

            public uint Color;

            public Tri(uint a, uint b, uint c, uint col)
            {
                A = a;
                B = b;
                C = c;
                Color = col;
            }
        }

        private BodyStatic(List<Point3D> ecken, List<Tri> seiten)
        {
            Ecken = ecken.ToArray();
            Seiten = seiten.ToArray();
            Buffer = null;
        }
        public BodyDynamic ToDynamic()
        {
            return new BodyDynamic(Ecken, Seiten);
        }

        public AxisBox3D BoxFit()
        {
            return AxisBox3D.MinMax(Ecken);
        }
        public AxisBox3D BoxDist()
        {
            return AxisBox3D.Distance(Ecken);
        }

        public double Intersekt(Ray3D ray, Transformation3D trans, out int idx)
        {
            Point3D[] ecken = new Point3D[Ecken.Length];
            for (int i = 0; i < ecken.Length; i++)
                ecken[i] = trans.TFore(Ecken[i]);

            double lowest = double.PositiveInfinity;
            double l;

            idx = -1;
            for (int i = 0; i < Seiten.Length; i++)
            {
                l = ray.Dreieck_Schnitt_Interval(
                    ecken[Seiten[i].A],
                    ecken[Seiten[i].B],
                    ecken[Seiten[i].C]);

                if (Ray3D.IsPositive(l) && l < lowest)
                {
                    lowest = l;
                    idx = i;
                }
            }

            if (double.IsPositiveInfinity(lowest))
                return double.NaN;
            return lowest;
        }
        public double Intersekt(Ray3D ray, out int idx)
        {
            double lowest = double.PositiveInfinity;
            double l;

            idx = -1;
            for (int i = 0; i < Seiten.Length; i++)
            {
                l = ray.Dreieck_Schnitt_Interval(
                    Ecken[Seiten[i].A],
                    Ecken[Seiten[i].B],
                    Ecken[Seiten[i].C]);

                if (Ray3D.IsPositive(l) && l < lowest)
                {
                    lowest = l;
                    idx = i;
                }
            }

            if (idx == -1)
                return double.NaN;
            return lowest;
        }


        public void Scale(double d)
        {
            for (int i = 0; i < Ecken.Length; i++)
            {
                Ecken[i] *= d;
            }
        }


        public void BufferCreate()
        {
            Buffer = new TransUniBuffers();
            Buffer.Create();
        }
        public void BufferDelete()
        {
            Buffer.Delete();
            Buffer = null;
        }
        public void BufferFill()
        {
            float[] koords;
            uint[] indexe;
            uint[] colors;

            {
                int i3;
                koords = new float[Ecken.Length * 3];
                for (int i = 0; i < Ecken.Length; i++)
                {
                    i3 = i * 3;
                    Ecken[i].Floats(koords, ref i3);
                }

                Tri tri;
                indexe = new uint[Seiten.Length * 3];
                colors = new uint[Seiten.Length];
                for (int i = 0; i < Seiten.Length; i++)
                {
                    tri = Seiten[i];

                    i3 = i * 3;
                    indexe[i3 + 0] = tri.C;
                    indexe[i3 + 1] = tri.B;
                    indexe[i3 + 2] = tri.A;

                    colors[i] = tri.Color;
                }
            }

            Buffer.Koords(koords);
            Buffer.Indexe(indexe);
            Buffer.Colors(colors);
        }
        public void BufferDraw()
        {
            Buffer.Draw();
        }


        public static void BufferCreate(BodyStatic body)
        {
            body.BufferCreate();
            body.BufferFill();
        }
        public static void BufferDelete(BodyStatic body)
        {
            body.BufferDelete();
        }
    }
}
