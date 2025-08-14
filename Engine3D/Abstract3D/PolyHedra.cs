using System;
using System.Collections.Generic;

using Engine3D.Graphics;
using Engine3D.Miscellaneous;

namespace Engine3D.Abstract3D
{
    /*
        PolyHedra : only points and triangles
        PolyHedra_Colores : simple color

        PolyHedra with PolyHedra_Colors is a body intedned for drawing
        once color is present it should pretty much allways be drawn
        so basically DrawBody ?
     */

    public class PolyHedraSkin_IndexColors
    {
        public struct IndexColor
        {
            public uint Index;
            public uint Color;
            public IndexColor(uint idx, uint col)
            {
                Index = idx;
                Color = col;
            }
        }

        private uint[] Colors;
        public int ColorCount { get { return Colors.Length; } }

        public PolyHedraSkin_IndexColors(IndexColor[] IdxCol, int len)
        {
            Colors = new uint[len];
            for (int i = 0; i < Colors.Length; i++) { Colors[i] = 0xFFFFFF; }

            for (int i = 0; i < IdxCol.Length; i++)
            {
                IndexColor idx_col = IdxCol[i];
                Colors[idx_col.Index] = idx_col.Color;
            }
        }
    }

    public struct IndexTriangle
    {
        public const int Size = sizeof(uint) * 3;

        public uint A;
        public uint B;
        public uint C;
        public IndexTriangle(uint a, uint b, uint c)
        {
            A = a;
            B = b;
            C = c;
        }
    }

    public class PolyHedra
    {
        private ArrayList<Point3D> Corners;
        private ArrayList<IndexTriangle> Faces;
        private ArrayList<uint> Colors;

        private bool IsEdit;

        public PolyHedra()
        {
            Corners = new ArrayList<Point3D>();
            Faces = new ArrayList<IndexTriangle>();
            Colors = new ArrayList<uint>();

            IsEdit = false;
        }

        public int CornerCount()
        {
            return Corners.Count;
        }
        public int FaceCount()
        {
            return Faces.Count;
        }



        public void Edit_Begin()
        {
            if (IsEdit) { return; }
            Corners.EditBegin();
            Faces.EditBegin();
            Colors.EditBegin();
            IsEdit = true;
        }
        public void Edit_Stop()
        {
            if (!IsEdit) { return; }
            Corners.EditEnd();
            Faces.EditEnd();
            Colors.EditEnd();
            IsEdit = false;
        }

        public uint Edit_Insert_Corner(Point3D corner)
        {
            if (!IsEdit) { throw new ENotEdit(); }
            uint idx = (uint)Corners.Count;
            Corners.Insert(new Point3D(
                corner.Y,
                corner.X,
                corner.C
                ));
            return idx;
        }
        public void Edit_Insert_Face(IndexTriangle face)
        {
            if (!IsEdit) { throw new ENotEdit(); }
            Faces.Insert(face);
            Colors.Insert(0xFFFFFF);
        }
        public void Edit_Change_Color(uint idx, uint color)
        {
            if (!IsEdit) { throw new ENotEdit(); }
            Colors[idx] = color;
        }



        public AxisBox3D CalcBox()
        {
            Point3D Min = new Point3D(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
            Point3D Max = new Point3D(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity);

            for (int i = 0; i < CornerCount(); i++)
            {
                Point3D corn = Corners[i];

                if (corn.Y < Min.Y) { Min.Y = corn.Y; }
                if (corn.X < Min.X) { Min.X = corn.X; }
                if (corn.C < Min.C) { Min.C = corn.C; }

                if (corn.Y > Max.Y) { Max.Y = corn.Y; }
                if (corn.X > Max.X) { Max.X = corn.X; }
                if (corn.C > Max.C) { Max.C = corn.C; }
            }

            return AxisBox3D.MinMax(Min, Max);
        }
        public Point3D CalcAverage()
        {
            Point3D Avg = new Point3D();

            for (int i = 0; i < CornerCount(); i++)
            {
                Point3D corn = Corners[i];

                Avg.Y += corn.Y / i;
                Avg.X += corn.X / i;
                Avg.C += corn.C / i;
            }

            return Avg;
        }
        public void Scale(float scale)
        {
            Point3D p;
            for (int i = 0; i < CornerCount(); i++)
            {
                p = Corners[i];
                p.Y *= scale;
                p.X *= scale;
                p.C *= scale;
                Corners[i] = p;
            }
        }

        public Intersekt.RayInterval Intersekt(Ray3D ray, Transformation3D trans)
        {
            double interval = double.PositiveInfinity;
            int idx = -1;

            for (int i = 0; i < FaceCount(); i++)
            {
                IndexTriangle face = Faces[i];
                Point3D a = Corners[face.A];
                Point3D b = Corners[face.B];
                Point3D c = Corners[face.C];

                Intersekt.RayInterval d;
                if (trans != null)
                {
                    d = Abstract3D.Intersekt.Ray_Triangle(ray,
                        trans.TFore(a),
                        trans.TFore(b),
                        trans.TFore(c)
                        );
                }
                else
                {
                    d = Abstract3D.Intersekt.Ray_Triangle(ray,
                        a,
                        b,
                        c
                        );
                }

                if (d.Interval > 0.0 && d.Interval < interval)
                {
                    interval = d.Interval;
                    idx = i;
                }
            }

            return new Intersekt.RayInterval(ray, interval, idx);
        }

        public void ToBufferElem(BodyElemBuffer buffer)
        {
            buffer.Bind_Corners(Corners.ToArray());
            buffer.Bind_Indexes(Faces.ToArray());
            buffer.Bind_Colors(Colors.ToArray());
        }
        public BodyElemBuffer ToBufferElem()
        {
            BodyElemBuffer buffer = new BodyElemBuffer();
            ToBufferElem(buffer);
            return buffer;
        }

        public static class Generate
        {
            public static PolyHedra Error()
            {
                PolyHedra template = new PolyHedra();
                template.Edit_Begin();

                template.Edit_Insert_Corner(new Point3D(0, +1, 0));
                template.Edit_Insert_Corner(new Point3D(+1, 0, 0));
                template.Edit_Insert_Corner(new Point3D(0, 0, +1));
                template.Edit_Insert_Corner(new Point3D(0, -1, 0));
                template.Edit_Insert_Corner(new Point3D(-1, 0, 0));
                template.Edit_Insert_Corner(new Point3D(0, 0, -1));

                template.Edit_Insert_Face(new IndexTriangle(0, 1, 2));
                template.Edit_Insert_Face(new IndexTriangle(0, 5, 1));
                template.Edit_Insert_Face(new IndexTriangle(0, 2, 4));
                template.Edit_Insert_Face(new IndexTriangle(3, 2, 1));
                template.Edit_Insert_Face(new IndexTriangle(0, 4, 5));
                template.Edit_Insert_Face(new IndexTriangle(3, 1, 5));
                template.Edit_Insert_Face(new IndexTriangle(3, 4, 2));
                template.Edit_Insert_Face(new IndexTriangle(3, 5, 4));

                template.Edit_Change_Color(00, 0x000000);
                template.Edit_Change_Color(01, 0xFF0000);
                template.Edit_Change_Color(02, 0x0000FF);
                template.Edit_Change_Color(03, 0x00FF00);
                template.Edit_Change_Color(04, 0xFF00FF);
                template.Edit_Change_Color(05, 0xFFFF00);
                template.Edit_Change_Color(06, 0x00FFFF);
                template.Edit_Change_Color(07, 0xFFFFFF);

                uint idx = 6;

                template.Edit_Insert_Corner(new Point3D(0, +0.5f, 0));
                template.Edit_Insert_Corner(new Point3D(+0.5f, 0, 0));
                template.Edit_Insert_Corner(new Point3D(0, 0, +0.5f));
                template.Edit_Insert_Corner(new Point3D(0, -0.5f, 0));
                template.Edit_Insert_Corner(new Point3D(-0.5f, 0, 0));
                template.Edit_Insert_Corner(new Point3D(0, 0, -0.5f));

                template.Edit_Insert_Face(new IndexTriangle(idx + 0, idx + 2, idx + 1));
                template.Edit_Insert_Face(new IndexTriangle(idx + 0, idx + 1, idx + 5));
                template.Edit_Insert_Face(new IndexTriangle(idx + 0, idx + 4, idx + 2));
                template.Edit_Insert_Face(new IndexTriangle(idx + 3, idx + 1, idx + 2));
                template.Edit_Insert_Face(new IndexTriangle(idx + 0, idx + 5, idx + 4));
                template.Edit_Insert_Face(new IndexTriangle(idx + 3, idx + 5, idx + 1));
                template.Edit_Insert_Face(new IndexTriangle(idx + 3, idx + 2, idx + 4));
                template.Edit_Insert_Face(new IndexTriangle(idx + 3, idx + 4, idx + 5));

                template.Edit_Change_Color(08, 0x000000);
                template.Edit_Change_Color(09, 0xFF0000);
                template.Edit_Change_Color(10, 0x0000FF);
                template.Edit_Change_Color(11, 0x00FF00);
                template.Edit_Change_Color(12, 0xFF00FF);
                template.Edit_Change_Color(13, 0xFFFF00);
                template.Edit_Change_Color(14, 0x00FFFF);
                template.Edit_Change_Color(15, 0xFFFFFF);

                template.Edit_Stop();
                return template;
            }
            public static PolyHedra Cube(float scale = 1.0f)
            {
                PolyHedra template = new PolyHedra();
                template.Edit_Begin();

                template.Edit_Insert_Corner(new Point3D(-scale, -scale, -scale));
                template.Edit_Insert_Corner(new Point3D(+scale, -scale, -scale));
                template.Edit_Insert_Corner(new Point3D(-scale, +scale, -scale));
                template.Edit_Insert_Corner(new Point3D(+scale, +scale, -scale));
                template.Edit_Insert_Corner(new Point3D(-scale, -scale, +scale));
                template.Edit_Insert_Corner(new Point3D(+scale, -scale, +scale));
                template.Edit_Insert_Corner(new Point3D(-scale, +scale, +scale));
                template.Edit_Insert_Corner(new Point3D(+scale, +scale, +scale));

                template.Edit_Insert_Face(new IndexTriangle(0, 2, 1));
                template.Edit_Insert_Face(new IndexTriangle(2, 3, 1));
                template.Edit_Insert_Face(new IndexTriangle(7, 6, 5));
                template.Edit_Insert_Face(new IndexTriangle(6, 4, 5));

                template.Edit_Insert_Face(new IndexTriangle(0, 4, 2));
                template.Edit_Insert_Face(new IndexTriangle(4, 6, 2));
                template.Edit_Insert_Face(new IndexTriangle(7, 5, 3));
                template.Edit_Insert_Face(new IndexTriangle(5, 1, 3));

                template.Edit_Insert_Face(new IndexTriangle(0, 1, 4));
                template.Edit_Insert_Face(new IndexTriangle(1, 5, 4));
                template.Edit_Insert_Face(new IndexTriangle(7, 3, 6));
                template.Edit_Insert_Face(new IndexTriangle(3, 2, 6));

                template.Edit_Change_Color(00, 0x0000FF);
                template.Edit_Change_Color(01, 0x0000FF);
                template.Edit_Change_Color(02, 0x0000FF);
                template.Edit_Change_Color(03, 0x0000FF);

                template.Edit_Change_Color(04, 0xFF0000);
                template.Edit_Change_Color(05, 0xFF0000);
                template.Edit_Change_Color(06, 0xFF0000);
                template.Edit_Change_Color(07, 0xFF0000);

                template.Edit_Change_Color(08, 0x00FF00);
                template.Edit_Change_Color(09, 0x00FF00);
                template.Edit_Change_Color(10, 0x00FF00);
                template.Edit_Change_Color(11, 0x00FF00);

                template.Edit_Stop();
                return template;
            }

            public static PolyHedra SpherePolar(uint ring, uint seg, float scale)
            {
                PolyHedra template = new PolyHedra();
                template.Edit_Begin();
                uint pole, ring1, ring2;

                uint faceCount = 0;

                pole = 0;
                template.Edit_Insert_Corner(new Point3D(0, -scale, 0));
                ring1 = 1;
                for (uint s = 0; s < seg; s++)
                {
                    template.Edit_Insert_Face(new IndexTriangle(pole, (s + 1) % seg + ring1, (s + 0) % seg + ring1));
                    template.Edit_Change_Color(faceCount, 0x00FF00);
                    faceCount++;
                }

                Point3D ecke;
                Angle3D w = new Angle3D();

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
                        template.Edit_Insert_Corner(new Point3D((float)ecke.Y, (float)ecke.X, (float)ecke.C));

                        if (r != 0)
                        {
                            s0 = (s + 0) % seg;
                            s1 = (s + 1) % seg;

                            template.Edit_Insert_Face(new IndexTriangle(ring1 + s0, ring1 + s1, ring2 + s0));
                            template.Edit_Change_Color(faceCount, 0xFF0000);
                            faceCount++;

                            template.Edit_Insert_Face(new IndexTriangle(ring1 + s1, ring2 + s1, ring2 + s0));
                            template.Edit_Change_Color(faceCount, 0x0000FF);
                            faceCount++;
                        }
                    }
                }

                ring2 = 1 + ring * seg;
                template.Edit_Insert_Corner(new Point3D(0, +scale, 0));
                pole = ring2 - seg;
                for (uint s = 0; s < seg; s++)
                {
                    template.Edit_Insert_Face(new IndexTriangle(ring2, (s + 0) % seg + pole, (s + 1) % seg + pole));
                    template.Edit_Change_Color(faceCount, 0x00FF00);
                    faceCount++;
                }

                template.Edit_Stop();
                return template;
            }

            private static void TemplateSegmentedCube(PolyHedra template, uint segPerEdge, float scale)
            {
                float IndexToScale(float f)
                {
                    //  [     0 ; len   ]
                    //  [     0 ; 1     ] / len
                    //  [  -0.5 ; +0.5  ] - 0.5
                    //  [    -1 ; +1    ] * 2
                    //  [-scale ; +scale]
                    return (((f * 1.0f) / (segPerEdge - 1) - 0.5f) * 2.0f) * scale;
                }

                uint[] corners = new uint[8];
                corners[0] = template.Edit_Insert_Corner(new Point3D(-scale, -scale, -scale)); //  0   --- 0b000
                corners[1] = template.Edit_Insert_Corner(new Point3D(+scale, -scale, -scale)); //  1   +-- 0b001
                corners[2] = template.Edit_Insert_Corner(new Point3D(-scale, +scale, -scale)); //  2   -+- 0b010
                corners[3] = template.Edit_Insert_Corner(new Point3D(+scale, +scale, -scale)); //  3   ++- 0b011
                corners[4] = template.Edit_Insert_Corner(new Point3D(-scale, -scale, +scale)); //  4   --+ 0b100
                corners[5] = template.Edit_Insert_Corner(new Point3D(+scale, -scale, +scale)); //  5   +-+ 0b101
                corners[6] = template.Edit_Insert_Corner(new Point3D(-scale, +scale, +scale)); //  6   -++ 0b110
                corners[7] = template.Edit_Insert_Corner(new Point3D(+scale, +scale, +scale)); //  7   +++ 0b111

                uint[] edgeY00 = new uint[segPerEdge]; edgeY00[0] = corners[0b000]; edgeY00[segPerEdge - 1] = corners[0b001];
                uint[] edgeY01 = new uint[segPerEdge]; edgeY01[0] = corners[0b100]; edgeY01[segPerEdge - 1] = corners[0b101];
                uint[] edgeY10 = new uint[segPerEdge]; edgeY10[0] = corners[0b010]; edgeY10[segPerEdge - 1] = corners[0b011];
                uint[] edgeY11 = new uint[segPerEdge]; edgeY11[0] = corners[0b110]; edgeY11[segPerEdge - 1] = corners[0b111];

                uint[] edgeX00 = new uint[segPerEdge]; edgeX00[0] = corners[0b000]; edgeX00[segPerEdge - 1] = corners[0b010];
                uint[] edgeX01 = new uint[segPerEdge]; edgeX01[0] = corners[0b001]; edgeX01[segPerEdge - 1] = corners[0b011];
                uint[] edgeX10 = new uint[segPerEdge]; edgeX10[0] = corners[0b100]; edgeX10[segPerEdge - 1] = corners[0b110];
                uint[] edgeX11 = new uint[segPerEdge]; edgeX11[0] = corners[0b101]; edgeX11[segPerEdge - 1] = corners[0b111];

                uint[] edgeC00 = new uint[segPerEdge]; edgeC00[0] = corners[0b000]; edgeC00[segPerEdge - 1] = corners[0b100];
                uint[] edgeC01 = new uint[segPerEdge]; edgeC01[0] = corners[0b010]; edgeC01[segPerEdge - 1] = corners[0b110];
                uint[] edgeC10 = new uint[segPerEdge]; edgeC10[0] = corners[0b001]; edgeC10[segPerEdge - 1] = corners[0b101];
                uint[] edgeC11 = new uint[segPerEdge]; edgeC11[0] = corners[0b011]; edgeC11[segPerEdge - 1] = corners[0b111];

                for (uint i = 1; i < segPerEdge - 1; i++)
                {
                    float s = IndexToScale(i);

                    edgeY00[i] = template.Edit_Insert_Corner(new Point3D(s, -scale, -scale));
                    edgeY01[i] = template.Edit_Insert_Corner(new Point3D(s, -scale, +scale));
                    edgeY10[i] = template.Edit_Insert_Corner(new Point3D(s, +scale, -scale));
                    edgeY11[i] = template.Edit_Insert_Corner(new Point3D(s, +scale, +scale));

                    edgeX00[i] = template.Edit_Insert_Corner(new Point3D(-scale, s, -scale));
                    edgeX01[i] = template.Edit_Insert_Corner(new Point3D(+scale, s, -scale));
                    edgeX10[i] = template.Edit_Insert_Corner(new Point3D(-scale, s, +scale));
                    edgeX11[i] = template.Edit_Insert_Corner(new Point3D(+scale, s, +scale));

                    edgeC00[i] = template.Edit_Insert_Corner(new Point3D(-scale, -scale, s));
                    edgeC01[i] = template.Edit_Insert_Corner(new Point3D(-scale, +scale, s));
                    edgeC10[i] = template.Edit_Insert_Corner(new Point3D(+scale, -scale, s));
                    edgeC11[i] = template.Edit_Insert_Corner(new Point3D(+scale, +scale, s));
                }



                uint[,] faceY0 = new uint[segPerEdge, segPerEdge];
                uint[,] faceX0 = new uint[segPerEdge, segPerEdge];
                uint[,] faceC0 = new uint[segPerEdge, segPerEdge];

                uint[,] faceY1 = new uint[segPerEdge, segPerEdge];
                uint[,] faceX1 = new uint[segPerEdge, segPerEdge];
                uint[,] faceC1 = new uint[segPerEdge, segPerEdge];

                void FaceInitFunc(uint[,] face, uint[][] edge, uint i0, uint i1, Point3D tri)
                {
                    uint idx = 0xFFFFFFFF;
                    if (i1 == 0)              { idx = edge[0b00][i0]; }
                    if (i0 == 0)              { idx = edge[0b10][i1]; }
                    if (i1 == segPerEdge - 1) { idx = edge[0b01][i0]; }
                    if (i0 == segPerEdge - 1) { idx = edge[0b11][i1]; }
                    if (idx == 0xFFFFFFFF) { idx = template.Edit_Insert_Corner(tri); }
                    face[i0, i1] = idx;
                }
                for (uint i0 = 0; i0 < segPerEdge; i0++)
                {
                    for (uint i1 = 0; i1 < segPerEdge; i1++)
                    {
                        float s0 = IndexToScale(i0);
                        float s1 = IndexToScale(i1);
                        FaceInitFunc(faceY0, new uint[][] { edgeX00, edgeX10, edgeC00, edgeC01 }, i0, i1, new Point3D(-scale, s0, s1));
                        FaceInitFunc(faceX0, new uint[][] { edgeC00, edgeC10, edgeY00, edgeY01 }, i0, i1, new Point3D(s1, -scale, s0));
                        FaceInitFunc(faceC0, new uint[][] { edgeY00, edgeY10, edgeX00, edgeX01 }, i0, i1, new Point3D(s0, s1, -scale));

                        FaceInitFunc(faceY1, new uint[][] { edgeC10, edgeC11, edgeX01, edgeX11 }, i0, i1, new Point3D(+scale, s1, s0));
                        FaceInitFunc(faceX1, new uint[][] { edgeY10, edgeY11, edgeC01, edgeC11 }, i0, i1, new Point3D(s0, +scale, s1));
                        FaceInitFunc(faceC1, new uint[][] { edgeX10, edgeX11, edgeY01, edgeY11 }, i0, i1, new Point3D(s1, s0, +scale));
                    }
                }



                void FaceFillFunc(uint[,] face, uint i0, uint i1)
                {
                    uint[] idx = new uint[4];
                    idx[0b00] = face[i0 + 0, i1 + 0];
                    idx[0b01] = face[i0 + 0, i1 + 1];
                    idx[0b10] = face[i0 + 1, i1 + 0];
                    idx[0b11] = face[i0 + 1, i1 + 1];

                    uint colorIdx = (uint)template.FaceCount();
                    template.Edit_Insert_Face(new IndexTriangle(idx[0b00], idx[0b01], idx[0b10]));
                    template.Edit_Insert_Face(new IndexTriangle(idx[0b10], idx[0b01], idx[0b11]));
                    template.Edit_Change_Color(colorIdx + 0, 0xFF0000);
                    template.Edit_Change_Color(colorIdx + 1, 0x0000FF);
                }
                for (uint i0 = 0; i0 < segPerEdge - 1; i0++)
                {
                    for (uint i1 = 0; i1 < segPerEdge - 1; i1++)
                    {
                        FaceFillFunc(faceY0, i0, i1);
                        FaceFillFunc(faceX0, i0, i1);
                        FaceFillFunc(faceC0, i0, i1);

                        FaceFillFunc(faceY1, i0, i1);
                        FaceFillFunc(faceX1, i0, i1);
                        FaceFillFunc(faceC1, i0, i1);
                    }
                }
            }
            public static PolyHedra SphereCube(uint segPerEdge, float scale)
            {
                PolyHedra template = new PolyHedra();
                template.Edit_Begin();

                TemplateSegmentedCube(template, segPerEdge, scale);

                Point3D p;
                for (uint i = 0; i < template.CornerCount(); i++)
                {
                    p = template.Corners[i];

                    double f = scale / p.Len;
                    p.Y = p.Y * f;
                    p.X = p.X * f;
                    p.C = p.C * f;

                    template.Corners[i] = p;
                }

                template.Edit_Stop();
                return template;
            }
        }
    }
}
