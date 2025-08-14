using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine3D.Abstract3D;
using Engine3D.GraphicsOld;
using Engine3D.BitManip;

namespace Engine3D.Entity
{
    public class BodyDynamic
    {
        private struct Ecke
        {
            public Point3D Koord;

            public Ecke(Point3D koord)
            {
                Koord = +koord;
            }
        }
        private List<Ecke> Ecken;
        public int Ecken_Count()
        {
            return Ecken.Count;
        }
        private struct Seit
        {
            public uint A;
            public uint B;
            public uint C;

            public Seit(uint a, uint b, uint c)
            {
                A = a;
                B = b;
                C = c;
            }
            public Seit(BodyStatic.Tri seite)
            {
                A = seite.A;
                B = seite.B;
                C = seite.C;
            }
        }
        private List<Seit> Seitn;
        public int Seitn_Count()
        {
            return Seitn.Count;
        }




        //private const uint Max_Uniform = 256;
        //private const uint Max_Corners = Max_Uniform * 32;
        private struct ColorListArr
        {
            private List<uint> Lst;
            private uint[] Arr;

            public ColorListArr(int len)
            {
                Lst = new List<uint>(len);
                Arr = new uint[len];

                for (int i = 0; i < len; i++)
                {
                    Lst.Add(0);
                    Arr[i] = 0;
                }
            }

            public void Change(int idx, bool hover, bool toggle)
            {
                uint state = Arr[idx];

                if (toggle)
                    state = BitsGeneral.Toggle(state, 1);
                state = BitsGeneral.Set(state, 0, hover);

                Lst[idx] = state;
                Arr[idx] = state;
            }
            public void Change(int idx, uint col)
            {
                Lst[idx] = col;
                Arr[idx] = col;
            }

            public void Reset()
            {
                for (int i = 0; i < Arr.Length; i++)
                {
                    Arr[i] = 0;
                    Lst[i] = 0;
                }
            }

            public void Add()
            {
                Lst.Add(0);
                Arr = Lst.ToArray();
            }
            public void Sub(int idx)
            {
                Lst.RemoveAt(idx);
                Arr = Lst.ToArray();
            }

            public uint[] ToArray()
            {
                return Arr;
            }
        }
        private ColorListArr ColorSeitn;
        private ColorListArr IndicSeitn;
        private ColorListArr IndicEcken;

        private TransUniBuffers Buffer_Color;
        private SelectionBuffers Buffer_Selection;

        public BodyDynamic()
        {
            Ecken = new List<Ecke>();
            IndicEcken = new ColorListArr(0);

            Seitn = new List<Seit>();
            ColorSeitn = new ColorListArr(0);
            IndicSeitn = new ColorListArr(0);
        }
        public BodyDynamic(Point3D[] ecken, BodyStatic.Tri[] seiten)
        {
            Ecken = new List<Ecke>();
            for (int i = 0; i < ecken.Length; i++)
                Ecken.Add(new Ecke(ecken[i]));
            IndicEcken = new ColorListArr(ecken.Length);

            Seitn = new List<Seit>();
            ColorSeitn = new ColorListArr(seiten.Length);
            for (int i = 0; i < seiten.Length; i++)
            {
                Seitn.Add(new Seit(seiten[i]));
                ColorSeitn.Change(i, seiten[i].Color);
            }
            IndicSeitn = new ColorListArr(seiten.Length);
        }


        public double Intersekt(Ray3D ray, out int idx)
        {
            double lowest = double.PositiveInfinity;
            double l;

            idx = -1;
            for (int i = 0; i < Seitn.Count; i++)
            {
                l = ray.Dreieck_Schnitt_Interval(
                    Ecken[(int)Seitn[i].A].Koord,
                    Ecken[(int)Seitn[i].B].Koord,
                    Ecken[(int)Seitn[i].C].Koord);

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
        public double NähsteEcke(Ray3D ray, out int idx)
        {
            double lowest = double.PositiveInfinity;
            double l;
            Point3D p;

            idx = -1;
            for (int i = 0; i < Ecken.Count; i++)
            {
                p = Abstract3D.Intersekt.Ray_Point(ray, (Ecken[i].Koord)).Pos;
                l = (p - Ecken[i].Koord).Len;

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


        public void Select_Seitn_Idx(int idx, bool hover, bool toggle)
        {
            if (idx == -1) { return; }
            IndicSeitn.Change(idx, hover, toggle);
            Buffer_Selection.Select_Seitn(IndicSeitn.ToArray());
        }
        public void Select_Seitn_Non()
        {
            IndicSeitn.Reset();
            Buffer_Selection.Select_Seitn(IndicSeitn.ToArray());
        }

        public void Select_Ecken_Idx(int idx, bool hover, bool toggle)
        {
            if (idx == -1) { return; }
            IndicEcken.Change(idx, hover, toggle);
            Buffer_Selection.Select_Ecken(IndicEcken.ToArray());
        }
        public void Select_Ecken_Non()
        {
            IndicEcken.Reset();
            Buffer_Selection.Select_Ecken(IndicEcken.ToArray());
        }

        public void Set_Ecken_Move(Point3D p)
        {
            uint[] sel = IndicEcken.ToArray();

            Ecke e;
            for (int i = 0; i < Ecken.Count; i++)
            {
                if ((sel[i] & 2) != 0)
                {
                    e = Ecken[i];
                    e.Koord += p;
                    Ecken[i] = e;
                }
            }

            BufferFill_Ecken();
        }
        public Point3D Get_Ecken_Move(int idx)
        {
            if (idx == -1)
                return null;
            return Ecken[idx].Koord;
        }


        public void Ecken_Add(Point3D p)
        {
            Ecken.Add(new Ecke(p));
            IndicEcken.Add();
            BufferFill_Ecken();
        }
        public void Ecken_Sub()
        {
            uint[] sel = IndicEcken.ToArray();

            for (int i = Ecken.Count - 1; i >= 0; i--)
            {
                if ((sel[i] & 2) != 0)
                {
                    Ecken.RemoveAt(i);
                    IndicEcken.Sub(i);
                }
            }
            IndicEcken.Reset();
            BufferFill_Ecken();
        }

        public void Seitn_Add()
        {
            uint[] sel = IndicEcken.ToArray();

            uint a = 0xFFFFFFFF;
            uint b = 0xFFFFFFFF;
            uint c = 0xFFFFFFFF;
            for (uint i = 0; i < sel.Length; i++)
            {
                if (BitsGeneral.Get(sel[i], 1))
                {
                    if (a == 0xFFFFFFFF)
                        a = i;
                    else if (b == 0xFFFFFFFF)
                        b = i;
                    else if (c == 0xFFFFFFFF)
                        c = i;
                }
            }
            if (a != 0xFFFFFFFF && b != 0xFFFFFFFF && c != 0xFFFFFFFF)
                Seitn.Add(new Seit(a, b, c));

            ColorSeitn.Add();
            IndicSeitn.Add();
            IndicSeitn.Reset();
            BufferFill_Seitn();
            IndicEcken.Reset();
            BufferFill_Ecken();
        }
        public void Seitn_Sub()
        {
            uint[] sel = IndicSeitn.ToArray();

            for (int i = Seitn.Count - 1; i >= 0; i--)
            {
                if (BitsGeneral.Get(sel[i], 1))
                {
                    Seitn.RemoveAt(i);
                    IndicSeitn.Sub(i);
                }
            }
            IndicSeitn.Reset();
            BufferFill_Seitn();
        }
        public void Seitn_Flip()
        {
            uint[] sel = IndicSeitn.ToArray();

            Seit s;
            for (int i = 0; i < Seitn.Count; i++)
            {
                if (BitsGeneral.Get(sel[i], 1))
                {
                    s = Seitn[i];
                    Seitn[i] = new Seit(s.C, s.B, s.A);
                }
            }

            IndicSeitn.Reset();
            BufferFill_Seitn();
        }



        public void BufferCreate()
        {
            TransUniBuffers.Create(ref Buffer_Color);

            Buffer_Selection = new SelectionBuffers();
            Buffer_Selection.Create();
            Buffer_Selection.Pallet(new uint[]
            {
                //  Ecken
                0x000000,
                0xFFFF00,
                0x00FFFF,
                0xFF00FF,

                //  Seitn
                0x7F7F7F,
                0x00FF00,
                0x0000FF,
                0xFF0000,
            });
        }
        public void BufferDelete()
        {
            TransUniBuffers.Delete(ref Buffer_Color);

            Buffer_Selection.Delete();
            Buffer_Selection = null;
        }
        private void BufferFill_Ecken()
        {
            float[] koords;

            int i3 = Ecken.Count * 3;
            koords = new float[i3];

            for (int i = 0; i < Ecken.Count; i++)
            {
                i3 = i * 3;
                Ecken[i].Koord.Floats(koords, ref i3);
            }

            Buffer_Color.Koords(koords);

            Buffer_Selection.Koords(koords);
            Buffer_Selection.Select_Ecken(IndicEcken.ToArray());
        }
        private void BufferFill_Seitn()
        {
            uint[] indexe;

            int i3 = Seitn.Count * 3;
            indexe = new uint[i3];

            Seit seite;
            for (int i = 0; i < Seitn.Count; i++)
            {
                i3 = i * 3;
                seite = Seitn[i];

                indexe[i3 + 0] = seite.C;
                indexe[i3 + 1] = seite.B;
                indexe[i3 + 2] = seite.A;
            }

            Buffer_Color.Indexe(indexe);
            Buffer_Color.Colors(ColorSeitn.ToArray());

            Buffer_Selection.Indexe(indexe);
            Buffer_Selection.Select_Seitn(IndicSeitn.ToArray());
        }
        public void BufferFill()
        {
            BufferFill_Ecken();
            BufferFill_Seitn();
        }

        public void BufferDraw_Color()
        {
            Buffer_Color.Draw();
        }
        public void BufferDraw_Seitn_Selection()
        {
            Buffer_Selection.Draw_Seitn();
        }
        public void BufferDraw_Ecken_Selection()
        {
            Buffer_Selection.Draw_Ecken();
        }


        public static class File
        {
            private static string Ecke_ToString(Ecke e)
            {
                string str = "p";
                str += " " + e.Koord.Y.ToString("0.00");
                str += " " + e.Koord.X.ToString("0.00");
                str += " " + e.Koord.C.ToString("0.00");
                return str;
            }
            private static string Seit_ToString(Seit s)
            {
                string str = "d";
                str += " " + s.A;
                str += " " + s.B;
                str += " " + s.C;
                return str;
            }

            public static void Save(string path, BodyDynamic body)
            {
                string str = "\n";

                for (int i = 0; i < body.Ecken.Count; i++)
                    str += Ecke_ToString(body.Ecken[i]) + "\n";
                str += "\n";

                for (int i = 0; i < body.Seitn.Count; i++)
                    str += Seit_ToString(body.Seitn[i]) + "\n";
                str += "\n";

                System.IO.File.WriteAllText(path, str);
            }
        }
    }
}
