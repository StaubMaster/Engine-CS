using System;

namespace Engine3D.Graphics
{
    struct TextCharacterPalletPoint
    {
        public float X;
        public float Y;
        public TextCharacterPalletPoint(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static TextCharacterPalletPoint operator +(TextCharacterPalletPoint a, TextCharacterPalletPoint b)
        {
            return new TextCharacterPalletPoint(
                a.X + b.X,
                a.Y + b.Y
                );
        }
        public static TextCharacterPalletPoint operator -(TextCharacterPalletPoint a, TextCharacterPalletPoint b)
        {
            return new TextCharacterPalletPoint(
                a.X - b.X,
                a.Y - b.Y
                );
        }
        public static TextCharacterPalletPoint operator *(TextCharacterPalletPoint p, float f)
        {
            return new TextCharacterPalletPoint(
                p.X * f,
                p.Y * f
                );
        }
        public static TextCharacterPalletPoint operator /(TextCharacterPalletPoint p, (float, float) f)
        {
            return new TextCharacterPalletPoint(
                p.X / f.Item1,
                p.Y / f.Item2
                );
        }

        public float Len()
        {
            return MathF.Sqrt((X * X) + (Y * Y));
        }
        public TextCharacterPalletPoint Norm()
        {
            float len = 1 / Len();
            return new TextCharacterPalletPoint(
                X * len,
                Y * len
                );
        }
        public TextCharacterPalletPoint PerpX()
        {
            return new TextCharacterPalletPoint(+Y, -X);
        }
        public TextCharacterPalletPoint PerpY()
        {
            return new TextCharacterPalletPoint(-Y, +X);
        }

        public static TextCharacterPalletPoint LineIntersekt(TextCharacterPalletPoint p1, TextCharacterPalletPoint p2, TextCharacterPalletPoint p3, TextCharacterPalletPoint p4)
        {
            TextCharacterPalletPoint Ldir12 = p1 - p2;
            TextCharacterPalletPoint Ldir34 = p3 - p4;
            float f12 = (p1.X * p2.Y) - (p1.Y * p2.X);
            float f34 = (p3.X * p4.Y) - (p3.Y * p4.X);
            float div = (Ldir12.X * Ldir34.Y) - (Ldir12.Y * Ldir34.X);

            return new TextCharacterPalletPoint(
                ((f12 * Ldir34.X) - (Ldir12.X * f34)) / div,
                ((f12 * Ldir34.Y) - (Ldir12.Y * f34)) / div
            );
        }

        public static readonly TextCharacterPalletPoint NaN = new TextCharacterPalletPoint(255, 255);
        public static readonly TextCharacterPalletPoint Skip = new TextCharacterPalletPoint(255, 0);
        public const int Size = sizeof(float) * 2;
    }
    struct TextCharacterPalletPosDir
    {
        public TextCharacterPalletPoint Pos;
        public TextCharacterPalletPoint Dir;
        public TextCharacterPalletPoint Per;
        public TextCharacterPalletPosDir(float x, float y)
        {
            Pos = new TextCharacterPalletPoint(x, y);
            Dir = new TextCharacterPalletPoint(0, 0);
            Per = new TextCharacterPalletPoint(0, 0);
        }
        public TextCharacterPalletPosDir(TextCharacterPalletPoint p)
        {
            Pos = p;
            Dir = new TextCharacterPalletPoint(0, 0);
            Per = new TextCharacterPalletPoint(0, 0);
        }

        public static readonly TextCharacterPalletPosDir NaN = new TextCharacterPalletPosDir(TextCharacterPalletPoint.NaN);
        public const int Size = TextCharacterPalletPoint.Size * 3;
    }
    struct TextCharacterPallet
    {
        public const float charScaleX = 0.5f;
        public const float charScaleY = 1.0f;

        private TextCharacterPalletPosDir P0, P1, P2, P3, P4, P5, P6, P7;
        private uint skips;
        private uint padding;

        public TextCharacterPalletPosDir this[int idx]
        {
            get
            {
                switch (idx)
                {
                    case 0: return P0;
                    case 1: return P1;
                    case 2: return P2;
                    case 3: return P3;
                    case 4: return P4;
                    case 5: return P5;
                    case 6: return P6;
                    case 7: return P7;
                    default: return TextCharacterPalletPosDir.NaN;
                }
            }
            set
            {
                switch (idx)
                {
                    case 0: P0 = value; break;
                    case 1: P1 = value; break;
                    case 2: P2 = value; break;
                    case 3: P3 = value; break;
                    case 4: P4 = value; break;
                    case 5: P5 = value; break;
                    case 6: P6 = value; break;
                    case 7: P7 = value; break;
                }
            }
        }

        public void Pad(TextCharacterPalletPoint[] arr)
        {
            for (int i = 0; i < 8; i++)
            {
                if (i < arr.Length)
                {
                    if (arr[i].X != 255 && arr[i].Y != 255)
                    {
                        this[i] = new TextCharacterPalletPosDir(arr[i].X * charScaleX, arr[i].Y * charScaleY);
                    }
                    else
                    {
                        this[i] = new TextCharacterPalletPosDir(arr[i]);
                    }
                    //this[i] = new palletPosDir(arr[i]);
                }
                else
                {
                    this[i] = TextCharacterPalletPosDir.NaN;
                    skips |= (uint)(1 << i);
                }
            }
        }
        public void Calc()
        {
            for (int i = 0; i < 8; i++)
            {
                if (this[i].Pos.X == 255)
                {
                    if (this[i].Pos.Y == 255)
                    {
                        break;
                    }
                }
                else
                {
                    TextCharacterPalletPoint p0 = new TextCharacterPalletPoint(0, 0);
                    bool IsPrev = false;
                    if (i > 0 && this[i - 1].Pos.X != 255)
                    {
                        p0 = this[i - 1].Pos;
                        IsPrev = true;
                    }

                    TextCharacterPalletPoint p1 = new TextCharacterPalletPoint(0, 0);
                    p1 = this[i].Pos;

                    TextCharacterPalletPoint p2 = new TextCharacterPalletPoint(0, 0);
                    bool IsNext = false;
                    if (i < 7 && this[i + 1].Pos.X != 255)
                    {
                        p2 = this[i + 1].Pos;
                        IsNext = true;
                    }

                    TextCharacterPalletPosDir p = new TextCharacterPalletPosDir(p1);
                    if (!IsPrev && !IsNext) { }
                    else if (IsPrev && !IsNext)
                    {
                        p.Dir = (p1 - p0).Norm();
                        p.Per = p.Dir.PerpX();
                    }
                    else if (!IsPrev && IsNext)
                    {
                        p.Dir = (p1 - p2).Norm();
                        p.Per = p.Dir.PerpY();
                    }
                    else
                    {
                        TextCharacterPalletPoint per01 = (p1 - p0).Norm().PerpX();
                        TextCharacterPalletPoint per12 = (p1 - p2).Norm().PerpY();

                        TextCharacterPalletPoint inter0 = TextCharacterPalletPoint.LineIntersekt(
                            p0 - per01, p1 - per01,
                            p1 - per12, p2 - per12);

                        TextCharacterPalletPoint inter1 = TextCharacterPalletPoint.LineIntersekt(
                            p0 + per01, p1 + per01,
                            p1 + per12, p2 + per12);

                        p.Per = (inter1 - inter0) * 0.5f;
                    }
                    this[i] = p;
                }
            }
        }

        public TextCharacterPallet(TextCharacterPalletPoint[] arr)
        {
            skips = 0;
            P0 = TextCharacterPalletPosDir.NaN;
            P1 = TextCharacterPalletPosDir.NaN;
            P2 = TextCharacterPalletPosDir.NaN;
            P3 = TextCharacterPalletPosDir.NaN;
            P4 = TextCharacterPalletPosDir.NaN;
            P5 = TextCharacterPalletPosDir.NaN;
            P6 = TextCharacterPalletPosDir.NaN;
            P7 = TextCharacterPalletPosDir.NaN;
            padding = 0;

            Pad(arr);
            Calc();
        }

        public const int Size = (TextCharacterPalletPosDir.Size * 8) + sizeof(uint) * 2;



        private static TextCharacterPalletPoint[][][] Init_Pallat_Points()
        {
            /*
                |--9876543210123456789--|
                |                       |
                9  #        #        #  9
                8                       8
                7         #   #         7
                6  #                 #  6
                5           #           5
                4  #   #    #    #   #  4
                3                       3
                2  #   #         #   #  2
                1                       1
                0  #   #    #    #   #  0
                1                       1
                2  #   #         #   #  2
                3                       3
                4  #   #    #    #   #  4
                5           #           5
                6  #                 #  6
                7         #   #         7
                8                       8
                9  #        #        #  9
                |                       |
                |--9876543210123456789--|
            */

            TextCharacterPalletPoint skip_skip = TextCharacterPalletPoint.Skip;
            TextCharacterPalletPoint middle_00 = new TextCharacterPalletPoint(0.0f, 0.0f);

            TextCharacterPalletPoint side_09_p = new TextCharacterPalletPoint(0.0f, +0.9f);
            TextCharacterPalletPoint side_09_n = new TextCharacterPalletPoint(0.0f, -0.9f);
            TextCharacterPalletPoint side_90_p = new TextCharacterPalletPoint(+0.9f, 0.0f);
            TextCharacterPalletPoint side_90_n = new TextCharacterPalletPoint(-0.9f, 0.0f);

            TextCharacterPalletPoint box_99_nn = new TextCharacterPalletPoint(-0.9f, -0.9f);
            TextCharacterPalletPoint box_99_pn = new TextCharacterPalletPoint(+0.9f, -0.9f);
            TextCharacterPalletPoint box_99_np = new TextCharacterPalletPoint(-0.9f, +0.9f);
            TextCharacterPalletPoint box_99_pp = new TextCharacterPalletPoint(+0.9f, +0.9f);

            TextCharacterPalletPoint box_96_nn = new TextCharacterPalletPoint(-0.9f, -0.6f);
            TextCharacterPalletPoint box_96_np = new TextCharacterPalletPoint(-0.9f, +0.6f);
            TextCharacterPalletPoint box_96_pp = new TextCharacterPalletPoint(+0.9f, +0.6f);
            TextCharacterPalletPoint box_96_pn = new TextCharacterPalletPoint(+0.9f, -0.6f);

            TextCharacterPalletPoint box_94_nn = new TextCharacterPalletPoint(-0.9f, -0.4f);
            TextCharacterPalletPoint box_94_np = new TextCharacterPalletPoint(-0.9f, +0.4f);
            TextCharacterPalletPoint box_94_pp = new TextCharacterPalletPoint(+0.9f, +0.4f);
            TextCharacterPalletPoint box_94_pn = new TextCharacterPalletPoint(+0.9f, -0.4f);

            TextCharacterPalletPoint side_04_p = new TextCharacterPalletPoint(0.0f, +0.4f);
            TextCharacterPalletPoint side_04_n = new TextCharacterPalletPoint(0.0f, -0.4f);

            TextCharacterPalletPoint box_92_nn = new TextCharacterPalletPoint(-0.9f, -0.2f);
            TextCharacterPalletPoint box_92_np = new TextCharacterPalletPoint(-0.9f, +0.2f);
            TextCharacterPalletPoint box_92_pp = new TextCharacterPalletPoint(+0.9f, +0.2f);
            TextCharacterPalletPoint box_92_pn = new TextCharacterPalletPoint(+0.9f, -0.2f);

            TextCharacterPalletPoint box_27_nn = new TextCharacterPalletPoint(-0.2f, -0.7f);
            TextCharacterPalletPoint box_27_np = new TextCharacterPalletPoint(-0.2f, +0.7f);
            TextCharacterPalletPoint box_27_pp = new TextCharacterPalletPoint(+0.2f, +0.7f);
            TextCharacterPalletPoint box_27_pn = new TextCharacterPalletPoint(+0.2f, -0.7f);

            TextCharacterPalletPoint side_05_p = new TextCharacterPalletPoint(0.0f, +0.5f);
            TextCharacterPalletPoint side_05_n = new TextCharacterPalletPoint(0.0f, -0.5f);
            TextCharacterPalletPoint side_50_p = new TextCharacterPalletPoint(+0.5f, 0.0f);
            TextCharacterPalletPoint side_50_n = new TextCharacterPalletPoint(-0.5f, 0.0f);

            TextCharacterPalletPoint box_52_bl = new TextCharacterPalletPoint(-0.5f, -0.2f);
            TextCharacterPalletPoint box_52_tl = new TextCharacterPalletPoint(-0.5f, +0.2f);
            TextCharacterPalletPoint box_52_tr = new TextCharacterPalletPoint(+0.5f, +0.2f);
            TextCharacterPalletPoint box_52_br = new TextCharacterPalletPoint(+0.5f, -0.2f);

            TextCharacterPalletPoint box_54_nn = new TextCharacterPalletPoint(-0.5f, -0.4f);
            TextCharacterPalletPoint box_54_np = new TextCharacterPalletPoint(-0.5f, +0.4f);
            TextCharacterPalletPoint box_54_pp = new TextCharacterPalletPoint(+0.5f, +0.4f);
            TextCharacterPalletPoint box_54_pn = new TextCharacterPalletPoint(+0.5f, -0.4f);

            TextCharacterPalletPoint[][] none = new TextCharacterPalletPoint[1][]
            {
                new TextCharacterPalletPoint[] { box_99_nn, box_99_pn, box_99_pp, box_99_np, box_99_nn, },
            };

            TextCharacterPalletPoint[][] digit = new TextCharacterPalletPoint[10][]
            {
                /*0*/ new TextCharacterPalletPoint[] { box_96_nn, box_96_np, side_09_p, box_96_pp, box_96_pn, side_09_n, box_96_nn, box_96_pp, },
                /*1*/ new TextCharacterPalletPoint[] { box_94_np, side_09_p, side_09_n, skip_skip, box_99_nn, box_99_pn, },
                /*2*/ new TextCharacterPalletPoint[] { box_94_np, side_09_p, box_96_pp, box_92_pp, box_99_nn, box_99_pn, },
                /*3*/ new TextCharacterPalletPoint[] { box_94_np, side_09_p, box_94_pp, middle_00, box_94_pn, side_09_n, box_94_nn, },
                /*4*/ new TextCharacterPalletPoint[] { new TextCharacterPalletPoint(-0.7f, +0.9f), side_90_n, side_90_p, skip_skip, new TextCharacterPalletPoint(+0.2f, +0.9f), new TextCharacterPalletPoint(-0.2f, -0.9f), },
                /*5*/ new TextCharacterPalletPoint[] { box_99_pp, box_99_np, box_92_np, box_92_pp, box_96_pn, side_09_n, box_96_nn, },
                /*6*/ new TextCharacterPalletPoint[] { box_96_pp, side_09_p, box_96_np, box_96_nn, side_09_n, box_96_pn, box_92_pn, box_92_np, },
                /*7*/ new TextCharacterPalletPoint[] { box_99_np, box_99_pp, box_99_nn, skip_skip, side_90_n, side_90_p, },
                /*8*/ new TextCharacterPalletPoint[] { box_94_nn, box_94_pp, side_09_p, box_94_np, box_94_pn, side_09_n, box_94_nn, },
                /*9*/ new TextCharacterPalletPoint[] { box_96_nn, side_09_n, box_96_pn, box_96_pp, side_09_p, box_96_np, box_92_np, box_92_pn, },
            };

            TextCharacterPalletPoint[][] letterHi = new TextCharacterPalletPoint[26][]
            {
                /*A*/ new TextCharacterPalletPoint[] { box_99_nn, side_09_p, box_99_pn, },
                /*B*/ new TextCharacterPalletPoint[] { middle_00, box_94_pp, side_09_p, box_99_np, box_99_nn, side_09_n, box_94_pn, middle_00, },
                /*C*/ new TextCharacterPalletPoint[] { box_94_pp, side_09_p, box_94_np, box_94_nn, side_09_n, box_94_pn, },
                /*D*/ new TextCharacterPalletPoint[] { box_99_np, box_99_nn, side_09_n, box_96_pn, box_96_pp, side_09_p, box_99_np, },
                /*E*/ new TextCharacterPalletPoint[] { box_99_pp, box_99_np, box_99_nn, box_99_pn, skip_skip, side_90_n, middle_00, },
                /*F*/ new TextCharacterPalletPoint[] { box_99_pp, box_99_np, box_99_nn, skip_skip, side_90_n, middle_00, },
                /*G*/ new TextCharacterPalletPoint[] { box_96_pp, side_09_p, box_96_np, box_96_nn, side_09_n, box_96_pn, side_90_p, middle_00, },
                /*H*/ new TextCharacterPalletPoint[] { box_99_np, box_99_nn, skip_skip, box_99_pp, box_99_pn, skip_skip, side_90_n, side_90_p, },
                /*I*/ new TextCharacterPalletPoint[] { box_99_np, box_99_pp, skip_skip, box_99_nn, box_99_pn, skip_skip, side_09_p, side_09_n, },
                /*J*/ new TextCharacterPalletPoint[] { box_99_np, box_99_pp, box_94_pn, side_09_n, box_94_nn, },
                /*K*/ new TextCharacterPalletPoint[] { box_99_np, box_99_nn, skip_skip, box_99_pp, side_90_n, box_99_pn, },
                /*L*/ new TextCharacterPalletPoint[] { box_99_np, box_99_nn, box_99_pn, },
                /*M*/ new TextCharacterPalletPoint[] { box_99_nn, box_99_np, middle_00, box_99_pp, box_99_pn, },
                /*N*/ new TextCharacterPalletPoint[] { box_99_nn, box_99_np, box_99_pn, box_99_pp, },
                /*O*/ new TextCharacterPalletPoint[] { side_09_n, box_96_nn, box_96_np, side_09_p, box_96_pp, box_96_pn, side_09_n, },
                /*P*/ new TextCharacterPalletPoint[] { box_99_nn, box_99_np, side_09_p, box_94_pp, middle_00, side_90_n, },
                /*Q*/ new TextCharacterPalletPoint[] { side_09_n, box_96_nn, box_96_np, side_09_p, box_96_pp, box_96_pn, side_09_n, box_99_pn, },
                /*R*/ new TextCharacterPalletPoint[] { box_99_nn, box_99_np, side_09_p, box_94_pp, middle_00, side_90_n, box_99_pn, },
                /*S*/ new TextCharacterPalletPoint[] { box_94_pp, side_09_p, box_94_np, side_50_n, side_50_p, box_94_pn, side_09_n, box_94_nn, },
                /*T*/ new TextCharacterPalletPoint[] { box_99_np, box_99_pp, skip_skip, side_09_p, side_09_n, },
                /*U*/ new TextCharacterPalletPoint[] { box_99_np, box_96_nn, side_09_n, box_96_pn, box_99_pp, },
                /*V*/ new TextCharacterPalletPoint[] { box_99_np, side_09_n, box_99_pp, },
                /*W*/ new TextCharacterPalletPoint[] { box_99_np, box_99_nn, middle_00, box_99_pn, box_99_pp, },
                /*X*/ new TextCharacterPalletPoint[] { box_99_np, box_99_pn, skip_skip, box_99_pp, box_99_nn, },
                /*Y*/ new TextCharacterPalletPoint[] { box_99_np, middle_00, box_99_pp, skip_skip, middle_00, side_09_n, },
                /*Z*/ new TextCharacterPalletPoint[] { box_99_np, box_99_pp, box_99_nn, box_99_pn, },
            };

            TextCharacterPalletPoint[][] letterLo = new TextCharacterPalletPoint[26][]
            {
                /*a*/ new TextCharacterPalletPoint[] { box_94_pp, box_94_pn, skip_skip, side_90_p, side_04_p, side_90_n, side_04_n, side_90_p, },
                /*b*/ new TextCharacterPalletPoint[] { box_99_np, box_94_nn, skip_skip, side_90_n, side_04_p, side_90_p, side_04_n, side_90_n, },
                /*c*/ new TextCharacterPalletPoint[] { box_52_tr, side_04_p, side_90_n, side_04_n, box_52_br, },
                /*d*/ new TextCharacterPalletPoint[] { box_99_pp, box_94_pn, skip_skip, side_90_p, side_04_p, side_90_n, side_04_n, side_90_p, },
                /*e*/ new TextCharacterPalletPoint[] { side_90_n, side_90_p, side_04_p, side_90_n, side_04_n, box_52_br, },
                /*f*/ new TextCharacterPalletPoint[] { side_04_n, side_09_p, box_94_pp, skip_skip, side_90_n, side_90_p, },
                /*g*/ new TextCharacterPalletPoint[] { side_90_p, side_04_n, side_90_n, side_04_p, box_94_pp, box_94_pn, side_09_n, box_94_nn, },
                /*h*/ new TextCharacterPalletPoint[] { box_99_np, box_94_nn, skip_skip, side_90_n, side_04_p, side_90_p, box_94_pn, },
                /*i*/ new TextCharacterPalletPoint[] { side_04_p, side_04_n, skip_skip, box_27_np, box_27_pp, skip_skip, box_27_nn, box_27_pn, },
                /*j*/ new TextCharacterPalletPoint[] { side_04_p, side_09_n, box_94_nn , skip_skip, box_27_np, box_27_pp },
                /*k*/ new TextCharacterPalletPoint[] { box_99_np, box_94_nn, skip_skip, box_94_pp, side_90_n, box_94_pn, },
                /*l*/ new TextCharacterPalletPoint[] { box_94_np, side_09_p, side_09_n, box_94_pn, },
                /*m*/ new TextCharacterPalletPoint[] { box_94_np, box_94_nn, skip_skip, side_90_n, box_54_np, side_04_n, box_54_pp, box_94_pn, },
                /*n*/ new TextCharacterPalletPoint[] { box_94_np, box_94_nn, skip_skip, side_90_n, side_04_p, side_90_p, box_94_pn, },
                /*o*/ new TextCharacterPalletPoint[] { side_90_n, side_04_p, side_90_p, side_04_n, side_90_n, },
                /*p*/ new TextCharacterPalletPoint[] { box_94_np, box_99_nn, skip_skip, side_90_n, side_04_p, side_90_p, side_04_n, side_90_n, },
                /*q*/ new TextCharacterPalletPoint[] { box_94_pp, box_99_pn, skip_skip, side_90_p, side_04_p, side_90_n, side_04_n, side_90_p, },
                /*r*/ new TextCharacterPalletPoint[] { box_94_np, box_94_nn, skip_skip, side_90_n, side_04_p, side_90_p, },
                /*s*/ new TextCharacterPalletPoint[] { box_92_pp, side_04_p, box_92_np, side_50_n, side_50_p, box_92_pn, side_04_n, box_92_nn, },
                /*t*/ new TextCharacterPalletPoint[] { side_04_p, side_09_n, box_94_pn, skip_skip, side_90_n, side_90_p, },
                /*u*/ new TextCharacterPalletPoint[] { box_94_np, side_90_n, side_04_n, side_90_p, skip_skip, box_94_pp, box_94_pn, },
                /*v*/ new TextCharacterPalletPoint[] { box_94_np, side_04_n, box_94_pp, },
                /*w*/ new TextCharacterPalletPoint[] { box_94_np, box_54_nn, side_04_p, box_54_pn, box_94_pp, },
                /*x*/ new TextCharacterPalletPoint[] { box_94_np, box_94_pn, skip_skip, box_94_pp, box_94_nn, },
                /*y*/ new TextCharacterPalletPoint[] { box_94_np, middle_00, skip_skip, box_94_pp, box_94_nn, },
                /*z*/ new TextCharacterPalletPoint[] { box_94_np, box_94_pp, box_94_nn, box_94_pn, },
            };

            TextCharacterPalletPoint[][] punctuation = new TextCharacterPalletPoint[6][]
            {
                /*.*/ new TextCharacterPalletPoint[] { box_27_nn, box_27_pn, },
                /*:*/ new TextCharacterPalletPoint[] { box_27_np, box_27_pp, skip_skip, box_27_nn, box_27_pn, },
                /*,*/ new TextCharacterPalletPoint[] { side_05_n, side_09_n, },
                /*;*/ new TextCharacterPalletPoint[] { box_27_np, box_27_pp, skip_skip, side_05_n, side_09_n, },
                /*!*/ new TextCharacterPalletPoint[] { side_09_p, side_04_n, skip_skip, box_27_nn, box_27_pn, },
                /*?*/ new TextCharacterPalletPoint[] { box_96_np, side_09_p, box_96_pp, middle_00, side_04_n, skip_skip, box_27_nn, box_27_pn, },
            };

            TextCharacterPalletPoint[][] arithmetic = new TextCharacterPalletPoint[4][]
            {
                /*+*/ new TextCharacterPalletPoint[] { side_09_p, side_09_n, skip_skip, side_90_n, side_90_p, },
                /*-*/ new TextCharacterPalletPoint[] { side_90_n, side_90_p, },
                /***/ new TextCharacterPalletPoint[] { box_94_np, box_94_pn, skip_skip, box_94_pp, box_94_nn, skip_skip, side_90_n, side_90_p, },
                /*/*/ new TextCharacterPalletPoint[] { box_99_nn, box_99_pp, },
            };

            TextCharacterPalletPoint[][] compare = new TextCharacterPalletPoint[3][]
            {
                /*=*/ new TextCharacterPalletPoint[] { box_94_np, box_94_pp, skip_skip, box_94_nn, box_94_pn, },
                /*<*/ new TextCharacterPalletPoint[] { box_94_pp, side_90_n, box_94_pn, },
                /*>*/ new TextCharacterPalletPoint[] { box_94_np, side_90_p, box_94_nn, },
            };

            TextCharacterPalletPoint[][] brackets = new TextCharacterPalletPoint[6][]
            {
                /*(*/ new TextCharacterPalletPoint[] { box_99_pp, side_09_p, box_94_np, box_94_nn, side_09_n, box_99_pn, },
                /*)*/ new TextCharacterPalletPoint[] { box_99_np, side_09_p, box_94_pp, box_94_pn, side_09_n, box_99_nn, },
                /*[*/ new TextCharacterPalletPoint[] { box_99_pp, side_09_p, side_09_n, box_99_pn, },
                /*]*/ new TextCharacterPalletPoint[] { box_99_np, side_09_p, side_09_n, box_99_nn, },
                /*{*/ new TextCharacterPalletPoint[] { box_99_pp, side_09_p, side_04_p, side_90_n, side_04_n, side_09_n, box_99_pn, },
                /*}*/ new TextCharacterPalletPoint[] { box_99_np, side_09_p, side_04_p, side_90_p, side_04_n, side_09_n, box_99_nn, },
            };

            return new TextCharacterPalletPoint[][][]
            {
                none,
                digit,

                letterHi,
                letterLo,
                punctuation,

                arithmetic,
                compare,
                brackets,
            };
        }
        public static TextCharacterPallet[] Init_Pallets()
        {
            TextCharacterPalletPoint[][][] pallet_arr_arr_arr = Init_Pallat_Points();

            int pallet_count = 0;
            for (int j = 0; j < pallet_arr_arr_arr.Length; j++)
            {
                pallet_count += pallet_arr_arr_arr[j].Length;
            }

            TextCharacterPallet[] pallet_arr = new TextCharacterPallet[pallet_count];
            int pallet_idx = 0;
            for (int j = 0; j < pallet_arr_arr_arr.Length; j++)
            {
                for (int i = 0; i < pallet_arr_arr_arr[j].Length; i++)
                {
                    pallet_arr[pallet_idx] = new TextCharacterPallet(pallet_arr_arr_arr[j][i]);
                    pallet_idx++;
                }
            }

            return pallet_arr;
        }



        private const uint off_dig = 0;
        private const uint off_letU = off_dig + 10;
        private const uint off_letL = off_letU + 26;
        private const uint off_punc = off_letL + 26;
        private const uint off_arith = off_punc + 6;
        private const uint off_comp = off_arith + 4;
        private const uint off_brack = off_comp + 3;
        public static byte Interpret_Control(char c)
        {
            if (c == ' ')
                return 1;
            else if (c == '\t')
                return 2;
            else if (c == '\n')
                return 3;
            else if (c == '\r')
                return 4;

            return 0;
        }
        public static byte Interpret_Character(char c)
        {
            if (c >= '0' && c <= '9')
                return (byte)((c - '0') + off_dig);
            else if (c >= 'A' && c <= 'Z')
                return (byte)((c - 'A') + off_letU);
            else if (c >= 'a' && c <= 'z')
                return (byte)((c - 'a') + off_letL);

            else if (c == '.')
                return (byte)((0) + off_punc);
            else if (c == ':')
                return (byte)((1) + off_punc);
            else if (c == ',')
                return (byte)((2) + off_punc);
            else if (c == ';')
                return (byte)((3) + off_punc);
            else if (c == '!')
                return (byte)((4) + off_punc);
            else if (c == '?')
                return (byte)((5) + off_punc);

            else if (c == '+')
                return (byte)((0) + off_arith);
            else if (c == '-')
                return (byte)((1) + off_arith);
            else if (c == '*')
                return (byte)((2) + off_arith);
            else if (c == '/' || c == '\\')
                return (byte)((3) + off_arith);

            else if (c == '=')
                return (byte)((0) + off_comp);
            else if (c == '<')
                return (byte)((1) + off_comp);
            else if (c == '>')
                return (byte)((2) + off_comp);

            else if (c == '(')
                return (byte)((0) + off_brack);
            else if (c == ')')
                return (byte)((1) + off_brack);
            else if (c == '[')
                return (byte)((2) + off_brack);
            else if (c == ']')
                return (byte)((3) + off_brack);
            else if (c == '{')
                return (byte)((4) + off_brack);
            else if (c == '}')
                return (byte)((5) + off_brack);

            return 0xFF;
        }

    }
}

