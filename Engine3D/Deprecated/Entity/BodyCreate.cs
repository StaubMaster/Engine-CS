using System;
using System.Collections.Generic;
using System.IO;

using Engine3D.Abstract3D;
using Engine3D.GraphicsOld;

namespace Engine3D.Entity
{
    public partial class BodyStatic
    {
        public static class Create
        {
            public static BodyStatic Cube(double scale)
            {
                ConsoleLog.Log("Cube:");
                List<Point3D> Ecken = new List<Point3D>();
                List<Tri> Seiten = new List<Tri>();

                Ecken.Add(new Point3D(-scale, -scale, -scale));
                Ecken.Add(new Point3D(+scale, -scale, -scale));
                Ecken.Add(new Point3D(-scale, +scale, -scale));
                Ecken.Add(new Point3D(+scale, +scale, -scale));
                Ecken.Add(new Point3D(-scale, -scale, +scale));
                Ecken.Add(new Point3D(+scale, -scale, +scale));
                Ecken.Add(new Point3D(-scale, +scale, +scale));
                Ecken.Add(new Point3D(+scale, +scale, +scale));

                Seiten.Add(new Tri(0, 1, 2, 0x0000FF));
                Seiten.Add(new Tri(2, 1, 3, 0x0000FF));
                Seiten.Add(new Tri(7, 5, 6, 0x0000FF));
                Seiten.Add(new Tri(6, 5, 4, 0x0000FF));

                Seiten.Add(new Tri(0, 2, 4, 0xFF0000));
                Seiten.Add(new Tri(4, 2, 6, 0xFF0000));
                Seiten.Add(new Tri(7, 3, 5, 0xFF0000));
                Seiten.Add(new Tri(5, 3, 1, 0xFF0000));

                Seiten.Add(new Tri(0, 4, 1, 0x00FF00));
                Seiten.Add(new Tri(1, 4, 5, 0x00FF00));
                Seiten.Add(new Tri(7, 6, 3, 0x00FF00));
                Seiten.Add(new Tri(3, 6, 2, 0x00FF00));

                return new BodyStatic(Ecken, Seiten);
            }
            public static BodyStatic SphereQuad(uint ring, uint seg, double scale)
            {
                ConsoleLog.Log("SphereQ: " + ring + " , " + seg);
                List<Point3D> Ecken = new List<Point3D>();
                List<Tri> Seiten = new List<Tri>();
                uint pole, ring1, ring2;

                pole = 0;
                Ecken.Add(new Point3D(0, -scale, 0));
                ring1 = 1;
                for (uint s = 0; s < seg; s++)
                {
                    Seiten.Add(new Tri(
                        pole,
                        (s + 0) % seg + ring1,
                        (s + 1) % seg + ring1,
                        0x00FF00));
                }

                Point3D ecke;
                Angle3D w = Angle3D.Default();

                double vert, hori;
                uint s0, s1;
                for (uint r = 0; r < ring; r++)
                {
                    vert = (1.0 + r) / (1.0 + ring);
                    vert -= 0.5;
                    w.S = vert * Math.PI;

                    ring2 = 1 + r * seg;
                    ring1 = ring2 - seg;
                    for (uint s = 0; s < seg; s++)
                    {
                        hori = (1.0 * s) / seg;
                        w.A = hori * Math.Tau;
                        ecke = new Point3D(0, 0, scale) - w;
                        Ecken.Add(ecke);

                        if (r != 0)
                        {
                            s0 = (s + 0) % seg;
                            s1 = (s + 1) % seg;

                            Seiten.Add(new Tri(
                                ring1 + s0,
                                ring2 + s0,
                                ring1 + s1,
                                0xFF0000));
                            Seiten.Add(new Tri(
                                ring1 + s1,
                                ring2 + s0,
                                ring2 + s1,
                                0x0000FF));
                        }
                    }
                }

                ring2 = 1 + ring * seg;
                Ecken.Add(new Point3D(0, +scale, 0));
                pole = ring2 - seg;
                for (uint s = 0; s < seg; s++)
                {
                    Seiten.Add(new Tri(
                        ring2,
                        (s + 1) % seg + pole,
                        (s + 0) % seg + pole,
                        0x00FF00));
                }

                return new BodyStatic(Ecken, Seiten);
            }
            public static BodyStatic SphereTri(uint ring, uint seg, double scale)
            {
                ConsoleLog.Log("SphereT: " + ring + " , " + seg);
                List<Point3D> Ecken = new List<Point3D>();
                List<Tri> Seiten = new List<Tri>();
                uint pole, ring1, ring2;

                pole = 0;
                Ecken.Add(new Point3D(0, -scale, 0));
                ring1 = 1;
                for (uint s = 0; s < seg; s++)
                {
                    Seiten.Add(new Tri(
                        pole,
                        (s + 0) % seg + ring1,
                        (s + 1) % seg + ring1,
                        0x00FF00));
                }

                Point3D ecke;
                Angle3D w = Angle3D.Default();

                double vert, hori;
                uint s0, s1;
                for (uint r = 0; r < ring; r++)
                {
                    vert = (1.0 + r) / (1.0 + ring);
                    vert -= 0.5;
                    w.S = vert * Math.PI;

                    ring2 = 1 + r * seg;
                    ring1 = ring2 - seg;
                    for (uint s = 0; s < seg; s++)
                    {
                        hori = (1.0 * s + 0.5 * r) / seg;
                        w.A = hori * Math.Tau;
                        ecke = new Point3D(0, 0, scale) - w;
                        Ecken.Add(ecke);

                        if (r != 0)
                        {
                            s0 = (s + 0) % seg;
                            s1 = (s + 1) % seg;

                            Seiten.Add(new Tri(
                                ring1 + s0,
                                ring2 + s0,
                                ring1 + s1,
                                0xFF0000));
                            Seiten.Add(new Tri(
                                ring1 + s1,
                                ring2 + s0,
                                ring2 + s1,
                                0x0000FF));
                        }
                    }
                }

                ring2 = 1 + ring * seg;
                Ecken.Add(new Point3D(0, +scale, 0));
                pole = ring2 - seg;
                for (uint s = 0; s < seg; s++)
                {
                    Seiten.Add(new Tri(
                        ring2,
                        (s + 1) % seg + pole,
                        (s + 0) % seg + pole,
                        0x00FF00));
                }

                return new BodyStatic(Ecken, Seiten);
            }

            public static BodyStatic ErrorBody()
            {
                List<Point3D> Ecken = new List<Point3D>();
                List<Tri> Seiten = new List<Tri>();

                Ecken.Add(new Point3D( 0, +1,  0));
                Ecken.Add(new Point3D(+1,  0,  0));
                Ecken.Add(new Point3D( 0,  0, +1));

                Ecken.Add(new Point3D( 0, -1,  0));
                Ecken.Add(new Point3D(-1,  0,  0));
                Ecken.Add(new Point3D( 0,  0, -1));

                Seiten.Add(new Tri(2, 1, 0, 0x000000));
                Seiten.Add(new Tri(0, 1, 5, 0xFF0000));
                Seiten.Add(new Tri(1, 2, 3, 0x00FF00));
                Seiten.Add(new Tri(2, 0, 4, 0x0000FF));

                Seiten.Add(new Tri(0, 5, 4, 0xFF00FF));
                Seiten.Add(new Tri(1, 3, 5, 0xFFFF00));
                Seiten.Add(new Tri(2, 4, 3, 0x00FFFF));
                Seiten.Add(new Tri(3, 4, 5, 0xFFFFFF));

                return new BodyStatic(Ecken, Seiten);
            }
        }
    }
}
