using System;

using Engine3D.Abstract2D;

namespace Engine3D.Graphics
{
    struct TextCharacterPalletPoint
    {
        public Point2D Pos;

        public Point2D Dir0;
        public Point2D Per0;
        public Point2D Off0;

        public Point2D Dir1;
        public Point2D Per1;
        public Point2D Off1;

        public TextCharacterPalletPoint(float x, float y)
        {
            Pos = new Point2D(x, y);
            Dir0 = new Point2D(0, 0);
            Per0 = new Point2D(0, 0);
            Off0 = new Point2D(0, 0);
            Dir1 = new Point2D(0, 0);
            Per1 = new Point2D(0, 0);
            Off1 = new Point2D(0, 0);
        }
        public TextCharacterPalletPoint(Point2D p)
        {
            Pos = p;
            Dir0 = new Point2D(0, 0);
            Per0 = new Point2D(0, 0);
            Off0 = new Point2D(0, 0);
            Dir1 = new Point2D(0, 0);
            Per1 = new Point2D(0, 0);
            Off1 = new Point2D(0, 0);
        }

        public const int Size = Point2D.Size * 7;
    }
    public static class TextPalletTest
    {
        public static void Test(char c)
        {
            /*
            TextCharacterPalletCategory[] cats = TextCharacterPallet.Init_Categorys();

            Point2D[] palletPoints = null;
            for (int i = 0; i < cats.Length; i++)
            {
                if (cats[i].Index.Check(c))
                {
                    palletPoints = cats[i].Pallets[0];
                    break;
                }
            }
            if (palletPoints == null)
            {
                ConsoleLog.LogError("Char " + c + " not found");
                return;
            }
            ConsoleLog.Log("Char " + c + " found");

            //ConsoleLog.Log("");

            TextCharacterPallet testUnscaled = new TextCharacterPallet();
            testUnscaled.PadDebug(palletPoints, TextCharacterPallet.charScaleX, TextCharacterPallet.charScaleY, true);
            testUnscaled.CalcDebug(false);

            //ConsoleLog.Log("");

            TextCharacterPallet testScaled = new TextCharacterPallet();
            testScaled.PadDebug(palletPoints, 1.0f, 1.0f, true);
            testScaled.CalcDebug(false);

            //ConsoleLog.Log("");

            string format = "+0.000000;-0.000000; 0.000000";
            int formatLen = 12;
            string formatString(string str)
            {
                if (str.Length < formatLen) { str = str.PadLeft(formatLen); }
                if (str.Length > formatLen) { str = str.Substring(0, formatLen); }
                return str;
            }

            {
                string[] names =
                {
                    "val 0 X", "val 1 X",
                    "val 0 Y", "val 1 Y",
                    "len 0", "len 1",
                };
                string str = "";
                for (int n = 0; n < names.Length; n++)
                {
                    str += " : " + formatString(names[n]);
                }
                ConsoleLog.Log("[i]" + str);
            }

            for (int i = 0; i < TextCharacterPallet.PalletPointLimit; i++)
            {
                TextCharacterPalletPoint p0 = testUnscaled[i];
                TextCharacterPalletPoint p1 = testScaled[i];

                //float val0X = p0.Per.X;
                //float val1X = p1.Per.X;
                //float val0Y = p0.Per.Y;
                //float val1Y = p1.Per.Y;

                float val0X = 0.0f;
                float val1X = 0.0f;
                float val0Y = 0.0f;
                float val1Y = 0.0f;

                float len20 = (val0X * val0X) + (val0Y * val0Y);
                float len21 = (val1X * val1X) + (val1Y * val1Y);

                float len0 = MathF.Sqrt(len20);
                float len1 = MathF.Sqrt(len21);

                float div01X = val0X / val1X;
                float idkX = val1X * div01X;
                float idkY = val1Y * div01X;

                float[] nums = new float[]
                {
                    val0X, val0Y,
                    val1X, val1Y,
                    len0, len1,
                    div01X,
                    idkX, idkY,
                };

                string str = "";
                for (int n = 0; n < nums.Length; n++)
                {
                    str += " : " + formatString(nums[n].ToString(format));
                }
                ConsoleLog.Log("[" + i + "]" + str);
            }
            */
        }
    }
    struct TextCharacterPallet
    {
        struct Template
        {
            public char CharacterMin;
            public char CharacterMax;
            public Point[] PointArr;

            public Template(char c, params Point[] points)
            {
                CharacterMin = c;
                CharacterMax = c;
                PointArr = points;
                //ConsoleLog.Log("Pallet: " + CharacterMin + " : " + CharacterMax);

                int limitMin = 0;
                int limitMax = PointArr.Length - 1;

                for (int i = 0; i < PointArr.Length; i++)
                {
                    EConnectType prev = PointArr[i].IndexPrev;
                    if (prev == EConnectType.Default)
                    {
                        if (i == limitMin)
                        {
                            prev = EConnectType.None;
                            //ConsoleLog.Log("[" + i + "]" + " Prev Default Limit " + prev);
                        }
                        else
                        {
                            prev = (EConnectType)(i - 1);
                            //ConsoleLog.Log("[" + i + "]" + " Prev Default not Limit " + prev);
                        }
                    }
                    else if (prev == EConnectType.Connect)
                    {
                        if (i == limitMin)
                        {    
                            prev = (EConnectType)(limitMax);
                            //ConsoleLog.Log("[" + i + "]" + " Prev Connect Limit " + prev);
                        }
                        else
                        {
                            prev = (EConnectType)(i - 1);
                            //ConsoleLog.Log("[" + i + "]" + " Prev Connect not Limit " + prev);
                        }
                    }
                    else
                    {
                        //ConsoleLog.Log("[" + i + "]" + " Prev None");
                    }
                    PointArr[i].IndexPrev = prev;

                    EConnectType next = PointArr[i].IndexNext;
                    if (next == EConnectType.Default)
                    {
                        if (i == limitMax)
                        {
                            next = EConnectType.None;
                            //ConsoleLog.Log("[" + i + "]" + " Next Default Limit " + next);
                        }
                        else
                        {
                            next = (EConnectType)(i + 1);
                            //ConsoleLog.Log("[" + i + "]" + " Next Default not Limit " + next);
                        }
                    }
                    else if (next == EConnectType.Connect)
                    {
                        if (i == limitMax)
                        {
                            next = (EConnectType)(limitMin);
                            //ConsoleLog.Log("[" + i + "]" + " Next Connect Limit " + next);
                        }
                        else
                        {
                            next = (EConnectType)(i + 1);
                            //ConsoleLog.Log("[" + i + "]" + " Next Connect not Limit " + next);
                        }
                    }
                    else
                    {
                        //ConsoleLog.Log("[" + i + "]" + " Next None");
                    }
                    PointArr[i].IndexNext = next;
                }
            }

            public enum EConnectType : byte
            {
                None = 255,
                Connect = 254,
                Default = 128,

                Index0 = 0,
                Index1 = 1,
                Index2 = 2,
                Index3 = 3,
                Index4 = 4,
                Index5 = 5,
                Index6 = 6,
                Index7 = 7,
            }
            public struct Point
            {
                public Point2D Pos;
                public EConnectType IndexPrev;
                public EConnectType IndexNext;

                public Point(Point2D pos)
                {
                    Pos = pos;
                    IndexPrev = EConnectType.Default;
                    IndexNext = EConnectType.Default;
                }
                public Point(float x, float y)
                {
                    Pos = new Point2D(x, y);
                    IndexPrev = EConnectType.Default;
                    IndexNext = EConnectType.Default;
                }

                public static implicit operator Point(Point2D p)
                {
                    return new Point(p);
                }

                public Point NextNot()
                {
                    Point p = this;
                    p.IndexNext = EConnectType.None;
                    return p;
                }
                public Point PrevNot()
                {
                    Point p = this;
                    p.IndexPrev = EConnectType.None;
                    return p;
                }

                public Point NextCon()
                {
                    Point p = this;
                    p.IndexNext = EConnectType.Connect;
                    return p;
                }
                public Point PrevCon()
                {
                    Point p = this;
                    p.IndexPrev = EConnectType.Connect;
                    return p;
                }
            }
        }

        public const int PalletPointLimit = 8;
        public const float charScaleX = 0.5f;
        public const float charScaleY = 1.0f;

        private TextCharacterPalletPoint P0, P1, P2, P3, P4, P5, P6, P7;
        private uint skips;
        private uint padding;

        public TextCharacterPalletPoint this[int idx]
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
                    default: return new TextCharacterPalletPoint();
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

        private void PadDebug(Template.Point[] arr, float scaleX, float scaleY, bool silent = true)
        {
            if (!silent) { ConsoleLog.Log("Padding ..."); }
            for (int i = 0; i < PalletPointLimit; i++)
            {
                if (i < arr.Length)
                {
                    TextCharacterPalletPoint posDir;
                    if (arr[i].Pos.X != 255 && arr[i].Pos.Y != 255)
                    {
                        posDir = new TextCharacterPalletPoint(arr[i].Pos.X * scaleX, arr[i].Pos.Y * scaleY);
                    }
                    else
                    {
                        posDir = new TextCharacterPalletPoint(arr[i].Pos);
                    }
                    if (!silent) { ConsoleLog.Log("[" + i + "] " + posDir.Pos.X + ":" + posDir.Pos.Y); }
                    this[i] = posDir;
                }
                else
                {
                    skips |= (uint)(1 << i);
                    if (!silent) { ConsoleLog.Log("[" + i + "] skip"); }
                }
            }
            if (!silent) { ConsoleLog.Log("Padding Done"); }
        }
        private void CalcDebug(bool silent = true)
        {
            //ConsoleLog.Log("Calculating ...");
            for (int i = 0; i < PalletPointLimit; i++)
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
                    Point2D p0 = new Point2D(0, 0);
                    bool IsPrev = false;
                    if (i > 0 && this[i - 1].Pos.X != 255)
                    {
                        p0 = this[i - 1].Pos;
                        IsPrev = true;
                    }

                    Point2D p1 = new Point2D(0, 0);
                    p1 = this[i].Pos;

                    Point2D p2 = new Point2D(0, 0);
                    bool IsNext = false;
                    if (i < 7 && this[i + 1].Pos.X != 255)
                    {
                        p2 = this[i + 1].Pos;
                        IsNext = true;
                    }

                    TextCharacterPalletPoint p = new TextCharacterPalletPoint(p1);

                    Point2D dir01 = (p1 - p0).Norm();
                    Point2D dir12 = (p1 - p2).Norm();

                    Point2D per01 = dir01.PerpX();
                    Point2D per12 = dir12.PerpY();

                    if (!IsPrev && !IsNext) { }
                    else if (IsPrev && !IsNext)
                    {
                        p.Dir0 = per01;
                        p.Per0 = new Point2D();
                        p.Off0 = -dir01;

                        p.Dir1 = dir01;
                        p.Per1 = per01;
                        p.Off1 = new Point2D();
                    }
                    else if (!IsPrev && IsNext)
                    {
                        p.Dir0 = dir12;
                        p.Per0 = per12;
                        p.Off0 = new Point2D();

                        p.Dir1 = per12;
                        p.Per1 = new Point2D();
                        p.Off1 = -dir12;
                    }
                    else
                    {
                        p.Dir0 = dir01;
                        p.Per0 = per01;
                        p.Off0 = new Point2D();

                        p.Dir1 = dir12;
                        p.Per1 = per12;
                        p.Off1 = new Point2D();
                    }
                    this[i] = p;
                }
            }
            //ConsoleLog.Log("Calculating done");
        }
        private void InsertPoint(Point2D pHere, Point2D? pPrev, Point2D? pNext, ref int idx)
        {
            //ConsoleLog.Log("Here Pos " + pHere.X + " : " + pHere.Y);

            Point2D? dirPrev = null;
            Point2D? perPrev = null;
            if (pPrev != null)
            {
                dirPrev = (pHere - pPrev)?.Norm();
                perPrev = dirPrev?.PerpX();
                //ConsoleLog.Log("Prev Pos " + pPrev.Value.X + " : " + pPrev.Value.Y);
                //ConsoleLog.Log("Prev Dir " + dirPrev.Value.X + " : " + dirPrev.Value.Y);
                //ConsoleLog.Log("Prev Per " + perPrev.Value.X + " : " + perPrev.Value.Y);
            }

            Point2D? dirNext = null;
            Point2D? perNext = null;
            if (pNext != null)
            {
                dirNext = (pHere - pNext)?.Norm();
                perNext = dirNext?.PerpY();
                //ConsoleLog.Log("Next Pos " + pNext.Value.X + " : " + pNext.Value.Y);
                //ConsoleLog.Log("Next Dir " + dirNext.Value.X + " : " + dirNext.Value.Y);
                //ConsoleLog.Log("Next Per " + perNext.Value.X + " : " + perNext.Value.Y);
            }



            TextCharacterPalletPoint p = new TextCharacterPalletPoint(pHere);
            p.Pos = pHere;

            if (pPrev != null)
            {
                p.Dir0 = dirPrev.Value;
                p.Per0 = perPrev.Value;
                p.Off0 = new Point2D();
            }
            if (pNext != null)
            {
                p.Dir1 = dirNext.Value;
                p.Per1 = perNext.Value;
                p.Off1 = new Point2D();
            }

            if (pPrev == null && pNext == null) { }
            else if (pPrev != null && pNext == null)
            {
                p.Dir1 = +perPrev.Value;
                p.Per1 = new Point2D();
                p.Off1 = -dirPrev.Value;
            }
            else if (pPrev == null && pNext != null)
            {
                p.Dir0 = +perNext.Value;
                p.Per0 = new Point2D();
                p.Off0 = -dirNext.Value;
            }



            this[idx] = p;
            idx++;
        }
        private void CopyPoint(int copyIdx, ref int idx)
        {
            this[idx] = this[copyIdx];
            idx++;
        }
        private void Calc(Template.Point[] arr, float scaleX, float scaleY)
        {
            int palletIdx = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                Template.Point tp = arr[i];
                if (tp.IndexNext == Template.EConnectType.None)
                {
                    skips |= (uint)(1 << palletIdx);
                }



                Point2D? pPrev = null;
                Point2D pHere = arr[i].Pos;
                Point2D? pNext = null;

                Template.EConnectType idxPrev = tp.IndexPrev;
                if (idxPrev != Template.EConnectType.None)
                {
                    pPrev = arr[(int)idxPrev].Pos;
                }

                Template.EConnectType idxNext = tp.IndexNext;
                if (idxNext != Template.EConnectType.None)
                {
                    pNext = arr[(int)idxNext].Pos;
                }



                if (pPrev != null) { pPrev = new Point2D(pPrev.Value.X * scaleX, pPrev.Value.Y * scaleY); }
                pHere = new Point2D(pHere.X * scaleX, pHere.Y * scaleY);
                if (pNext != null) { pNext = new Point2D(pNext.Value.X * scaleX, pNext.Value.Y * scaleY); }

                InsertPoint(pHere, pPrev, pNext, ref palletIdx);

                if (idxNext != Template.EConnectType.None && (int)idxNext != i + 1)
                {
                    skips |= (uint)(1 << palletIdx);
                    CopyPoint((int)idxNext, ref palletIdx);
                }
            }
            for (; palletIdx < PalletPointLimit; palletIdx++)
            {
                skips |= (uint)(1 << palletIdx);
            }
        }

        private TextCharacterPallet(Template.Point[] arr)
        {
            P0 = new TextCharacterPalletPoint();
            P1 = new TextCharacterPalletPoint();
            P2 = new TextCharacterPalletPoint();
            P3 = new TextCharacterPalletPoint();
            P4 = new TextCharacterPalletPoint();
            P5 = new TextCharacterPalletPoint();
            P6 = new TextCharacterPalletPoint();
            P7 = new TextCharacterPalletPoint();
            skips = 0;
            padding = 0;

            //PadDebug(arr, charScaleX, charScaleY, true);
            //CalcDebug(true);
            Calc(arr, charScaleX, charScaleY);
        }

        public const int Size = (TextCharacterPalletPoint.Size * 8) + sizeof(uint) * 2;

        private static Template[] Init_Categorys()
        {
            Template.Point middle_00 = new Point2D(0.0f, 0.0f);

            Template.Point side_09_p = new Point2D(0.0f, +0.9f);
            Template.Point side_09_n = new Point2D(0.0f, -0.9f);
            Template.Point side_90_p = new Point2D(+0.9f, 0.0f);
            Template.Point side_90_n = new Point2D(-0.9f, 0.0f);

            Template.Point box_99_nn = new Point2D(-0.9f, -0.9f);
            Template.Point box_99_pn = new Point2D(+0.9f, -0.9f);
            Template.Point box_99_np = new Point2D(-0.9f, +0.9f);
            Template.Point box_99_pp = new Point2D(+0.9f, +0.9f);

            Template.Point box_96_nn = new Point2D(-0.9f, -0.6f);
            Template.Point box_96_np = new Point2D(-0.9f, +0.6f);
            Template.Point box_96_pp = new Point2D(+0.9f, +0.6f);
            Template.Point box_96_pn = new Point2D(+0.9f, -0.6f);

            Template.Point box_94_nn = new Point2D(-0.9f, -0.4f);
            Template.Point box_94_np = new Point2D(-0.9f, +0.4f);
            Template.Point box_94_pp = new Point2D(+0.9f, +0.4f);
            Template.Point box_94_pn = new Point2D(+0.9f, -0.4f);

            Template.Point side_04_p = new Point2D(0.0f, +0.4f);
            Template.Point side_04_n = new Point2D(0.0f, -0.4f);

            Template.Point box_92_nn = new Point2D(-0.9f, -0.2f);
            Template.Point box_92_np = new Point2D(-0.9f, +0.2f);
            Template.Point box_92_pp = new Point2D(+0.9f, +0.2f);
            Template.Point box_92_pn = new Point2D(+0.9f, -0.2f);

            Template.Point box_27_nn = new Point2D(-0.2f, -0.7f);
            Template.Point box_27_np = new Point2D(-0.2f, +0.7f);
            Template.Point box_27_pp = new Point2D(+0.2f, +0.7f);
            Template.Point box_27_pn = new Point2D(+0.2f, -0.7f);

            Template.Point side_05_p = new Point2D(0.0f, +0.5f);
            Template.Point side_05_n = new Point2D(0.0f, -0.5f);
            Template.Point side_50_p = new Point2D(+0.5f, 0.0f);
            Template.Point side_50_n = new Point2D(-0.5f, 0.0f);

            Template.Point box_52_bl = new Point2D(-0.5f, -0.2f);
            Template.Point box_52_tl = new Point2D(-0.5f, +0.2f);
            Template.Point box_52_tr = new Point2D(+0.5f, +0.2f);
            Template.Point box_52_br = new Point2D(+0.5f, -0.2f);

            Template.Point box_54_nn = new Point2D(-0.5f, -0.4f);
            Template.Point box_54_np = new Point2D(-0.5f, +0.4f);
            Template.Point box_54_pp = new Point2D(+0.5f, +0.4f);
            Template.Point box_54_pn = new Point2D(+0.5f, -0.4f);

            return new Template[]
            {
                new Template('\0', box_99_nn, box_99_pn, box_99_pp, box_99_np, box_99_nn),

                new Template('0', box_96_nn, box_96_np, side_09_p, box_96_pp, box_96_pn, side_09_n, box_96_nn, box_96_pp),
                new Template('1', box_94_np, side_09_p, side_09_n.NextNot(), box_99_nn.PrevNot(), box_99_pn),
                new Template('2', box_94_np, side_09_p, box_96_pp, box_92_pp, box_99_nn, box_99_pn),
                new Template('3', box_94_np, side_09_p, box_94_pp, middle_00, box_94_pn, side_09_n, box_94_nn),
                new Template('4', new Template.Point(-0.7f, +0.9f), side_90_n, side_90_p.NextNot(), new Template.Point(+0.2f, +0.9f).PrevNot(), new Template.Point(-0.2f, -0.9f)),
                new Template('5', box_99_pp, box_99_np, box_92_np, box_92_pp, box_96_pn, side_09_n, box_96_nn),
                new Template('6', box_96_pp, side_09_p, box_96_np, box_96_nn, side_09_n, box_96_pn, box_92_pn, box_92_np),
                new Template('7', box_99_np, box_99_pp, box_99_nn.NextNot(), side_90_n.PrevNot(), side_90_p),
                new Template('8', box_94_nn, box_94_pp, side_09_p, box_94_np, box_94_pn, side_09_n, box_94_nn),
                new Template('9', box_96_nn, side_09_n, box_96_pn, box_96_pp, side_09_p, box_96_np, box_92_np, box_92_pn),

                new Template('A', box_99_nn, side_09_p, box_99_pn),
                new Template('B', middle_00.PrevCon(), box_94_pp, side_09_p, box_99_np, box_99_nn, side_09_n, box_94_pn.NextCon()),
                new Template('C', box_94_pp, side_09_p, box_94_np, box_94_nn, side_09_n, box_94_pn),
                new Template('D', box_99_np, box_99_nn, side_09_n, box_96_pn, box_96_pp, side_09_p, box_99_np),
                new Template('E', box_99_pp, box_99_np, box_99_nn, box_99_pn.NextNot(), side_90_n.PrevNot(), middle_00),
                new Template('F', box_99_pp, box_99_np, box_99_nn.NextNot(), side_90_n.PrevNot(), middle_00),
                new Template('G', box_96_pp, side_09_p, box_96_np, box_96_nn, side_09_n, box_96_pn, side_90_p, middle_00),
                new Template('H', box_99_np, box_99_nn.NextNot(), box_99_pp.PrevNot(), box_99_pn.NextNot(), side_90_n.PrevNot(), side_90_p),
                new Template('I', box_99_np, box_99_pp.NextNot(), box_99_nn.PrevNot(), box_99_pn.NextNot(), side_09_p.PrevNot(), side_09_n),
                new Template('J', box_99_np, box_99_pp, box_94_pn, side_09_n, box_94_nn),
                new Template('K', box_99_np, box_99_nn.NextNot(), box_99_pp.PrevNot(), side_90_n, box_99_pn),
                new Template('L', box_99_np, box_99_nn, box_99_pn),
                new Template('M', box_99_nn, box_99_np, middle_00, box_99_pp, box_99_pn),
                new Template('N', box_99_nn, box_99_np, box_99_pn, box_99_pp),
                new Template('O', side_09_n.PrevCon(), box_96_nn, box_96_np, side_09_p, box_96_pp, box_96_pn.NextCon()),
                new Template('P', box_99_nn, box_99_np, side_09_p, box_94_pp, middle_00, side_90_n),
                new Template('Q', side_09_n, box_96_nn, box_96_np, side_09_p, box_96_pp, box_96_pn, side_09_n, box_99_pn),
                new Template('R', box_99_nn, box_99_np, side_09_p, box_94_pp, middle_00, side_90_n, box_99_pn),
                new Template('S', box_94_pp, side_09_p, box_94_np, side_50_n, side_50_p, box_94_pn, side_09_n, box_94_nn),
                new Template('T', box_99_np, box_99_pp.NextNot(), side_09_p.PrevNot(), side_09_n),
                new Template('U', box_99_np, box_96_nn, side_09_n, box_96_pn, box_99_pp),
                new Template('V', box_99_np, side_09_n, box_99_pp),
                new Template('W', box_99_np, box_99_nn, middle_00, box_99_pn, box_99_pp),
                new Template('X', box_99_np, box_99_pn.NextNot(), box_99_pp.PrevNot(), box_99_nn),
                new Template('Y', box_99_np, middle_00, box_99_pp.NextNot(), middle_00.PrevNot(), side_09_n),
                new Template('Z', box_99_np, box_99_pp, box_99_nn, box_99_pn),

                new Template('a', box_94_pp, box_94_pn.NextNot(), side_90_p.PrevNot(), side_04_p, side_90_n, side_04_n, side_90_p),
                new Template('b', box_99_np, box_94_nn.NextNot(), side_90_n.PrevNot(), side_04_p, side_90_p, side_04_n, side_90_n),
                new Template('c', box_52_tr, side_04_p, side_90_n, side_04_n, box_52_br),
                new Template('d', box_99_pp, box_94_pn.NextNot(), side_90_p.PrevNot(), side_04_p, side_90_n, side_04_n, side_90_p),
                new Template('e', side_90_n, side_90_p, side_04_p, side_90_n, side_04_n, box_52_br),
                new Template('f', side_04_n, side_09_p, box_94_pp.NextNot(), side_90_n.PrevNot(), side_90_p),
                new Template('g', side_90_p, side_04_n, side_90_n, side_04_p, box_94_pp, box_94_pn, side_09_n, box_94_nn),
                new Template('h', box_99_np, box_94_nn.NextNot(), side_90_n.PrevNot(), side_04_p, side_90_p, box_94_pn),
                new Template('i', side_04_p, side_04_n.NextNot(), box_27_np.PrevNot(), box_27_pp.NextNot(), box_27_nn.PrevNot(), box_27_pn),
                new Template('j', side_04_p, side_09_n, box_94_nn.NextNot(), box_27_np.PrevNot(), box_27_pp),
                new Template('k', box_99_np, box_94_nn.NextNot(), box_94_pp.PrevNot(), side_90_n, box_94_pn),
                new Template('l', box_94_np, side_09_p, side_09_n, box_94_pn),
                new Template('m', box_94_np, box_94_nn.NextNot(), side_90_n.PrevNot(), box_54_np, side_04_n, box_54_pp, box_94_pn),

                new Template('n', box_94_np, box_94_nn.NextNot(), side_90_n.PrevNot(), side_04_p, side_90_p, box_94_pn),
                new Template('o', side_90_n, side_04_p, side_90_p, side_04_n, side_90_n),
                new Template('p', box_94_np, box_99_nn.NextNot(), side_90_n.PrevNot(), side_04_p, side_90_p, side_04_n, side_90_n),
                new Template('q', box_94_pp, box_99_pn.NextNot(), side_90_p.PrevNot(), side_04_p, side_90_n, side_04_n, side_90_p),
                new Template('r', box_94_np, box_94_nn.NextNot(), side_90_n.PrevNot(), side_04_p, side_90_p),
                new Template('s', box_92_pp, side_04_p, box_92_np, side_50_n, side_50_p, box_92_pn, side_04_n, box_92_nn),
                new Template('t', side_04_p, side_09_n, box_94_pn.NextNot(), side_90_n.PrevNot(), side_90_p),
                new Template('u', box_94_np, side_90_n, side_04_n, side_90_p.NextNot(), box_94_pp.PrevNot(), box_94_pn),
                new Template('v', box_94_np, side_04_n, box_94_pp),
                new Template('w', box_94_np, box_54_nn, side_04_p, box_54_pn, box_94_pp),
                new Template('x', box_94_np, box_94_pn.NextNot(), box_94_pp.PrevNot(), box_94_nn),
                new Template('y', box_94_np, middle_00.NextNot(), box_94_pp.PrevNot(), box_94_nn),
                new Template('z', box_94_np, box_94_pp, box_94_nn, box_94_pn),

                new Template('.', box_27_nn, box_27_pn),
                new Template(':', box_27_np, box_27_pp.NextNot(), box_27_nn.PrevNot(), box_27_pn),
                new Template(',', side_05_n, side_09_n),
                new Template(';', box_27_np, box_27_pp.NextNot(), side_05_n.PrevNot(), side_09_n),
                new Template('!', side_09_p, side_04_n.NextNot(), box_27_nn.PrevNot(), box_27_pn),
                new Template('?', box_96_np, side_09_p, box_96_pp, middle_00, side_04_n.NextNot(), box_27_nn.PrevNot(), box_27_pn),

                new Template('+', side_09_p, side_09_n.NextNot(), side_90_n.PrevNot(), side_90_p),
                new Template('-', side_90_n, side_90_p),
                new Template('*', box_94_np, box_94_pn.NextNot(), box_94_pp.PrevNot(), box_94_nn.NextNot(), side_90_n.PrevNot(), side_90_p),
                new Template('/', box_99_nn, box_99_pp),

                new Template('=', box_94_np, box_94_pp.NextNot(), box_94_nn.PrevNot(), box_94_pn),
                new Template('<', box_94_pp, side_90_n, box_94_pn),
                new Template('>', box_94_np, side_90_p, box_94_nn),

                new Template('(', box_99_pp, side_09_p, box_94_np, box_94_nn, side_09_n, box_99_pn),
                new Template(')', box_99_np, side_09_p, box_94_pp, box_94_pn, side_09_n, box_99_nn),
                new Template('[', box_99_pp, side_09_p, side_09_n, box_99_pn),
                new Template(']', box_99_np, side_09_p, side_09_n, box_99_nn),
                new Template('{', box_99_pp, side_09_p, side_04_p, side_90_n, side_04_n, side_09_n, box_99_pn),
                new Template('}', box_99_np, side_09_p, side_04_p, side_90_p, side_04_n, side_09_n, box_99_nn),
            };
        }
        public static TextCharacterPallet[] Init_Pallets(out TextCharacterPalletIndex[] indexMap)
        {
            Template[] templates = Init_Categorys();
            int count = templates.Length;
            indexMap = new TextCharacterPalletIndex[count];

            TextCharacterPallet[] pallets = new TextCharacterPallet[count];
            int pallet_idx = 0;
            for (int j = 0; j < templates.Length; j++)
            {
                //ConsoleLog.Log("Pallet: " + templates[j].CharacterMin + " : " + templates[j].CharacterMax);
                pallets[pallet_idx] = new TextCharacterPallet(templates[j].PointArr);
                //ConsoleLog.Log("");

                TextCharacterPalletIndex charIdx = new TextCharacterPalletIndex(templates[j].CharacterMin, templates[j].CharacterMax);
                charIdx.Index = (uint)pallet_idx;
                indexMap[pallet_idx] = charIdx;
                pallet_idx++;
            }

            return pallets;
        }



        public enum CharControl : byte
        {
            None,
            Space,
            Tab,
            NewLine,
            LineReturn,
        }
        public static CharControl Interpret_Control(char c)
        {
            switch (c)
            {
                case ' ': return CharControl.Space;
                case '\t': return CharControl.Tab;
                case '\n': return CharControl.NewLine;
                case '\r': return CharControl.LineReturn;
                default: return CharControl.None;
            }
        }
    }
    struct TextCharacterPalletIndex
    {
        public char CharacterMin;
        public char CharacterMax;
        public uint Index;

        public TextCharacterPalletIndex(char c)
        {
            CharacterMin = c;
            CharacterMax = c;
            Index = 0xFFFFFFFF;
        }
        public TextCharacterPalletIndex(char cMin, char cMax)
        {
            CharacterMin = cMin;
            CharacterMax = cMax;
            Index = 0xFFFFFFFF;
        }
        public bool Check(char c)
        {
            return (CharacterMin <= c && CharacterMax >= c);
        }

        public static uint Interpret(TextCharacterPalletIndex[] indexMap, char c)
        {
            for (int i = 0; i < indexMap.Length; i++)
            {
                TextCharacterPalletIndex idx = indexMap[i];
                if (idx.Check(c))
                {
                    return idx.Index;
                }
            }
            return indexMap[0].Index;
        }
    }
    struct TextCharacterPalletCategory
    {
        public TextCharacterPalletIndex Index;
        public Point2D[][] Pallets;

        public TextCharacterPalletCategory(char c, Point2D[] pallet)
        {
            Index = new TextCharacterPalletIndex(c);
            Pallets = new Point2D[][] { pallet };
        }
        public TextCharacterPalletCategory(char cMin, char cMax, Point2D[][] pallets)
        {
            Index = new TextCharacterPalletIndex(cMin, cMax);
            Pallets = pallets;
        }

        public static int CountPallets(TextCharacterPalletCategory[] categorys)
        {
            int pallet_count = 0;
            for (int j = 0; j < categorys.Length; j++)
            {
                pallet_count += categorys[j].Pallets.Length;
            }
            return pallet_count;
        }
    }
}

