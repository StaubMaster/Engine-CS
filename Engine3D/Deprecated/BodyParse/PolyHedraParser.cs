using System;
using System.Collections.Generic;
using System.IO;

using Engine3D.Abstract3D;
using Engine3D.OutPut.Shader;

using Engine3D.StringParse;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.BodyParse
{
   /*
        Headers
            all files need a header
            YMT? to indicate a ymt file
            then the Version to use

        Version: 2021-08-06
            first line indicated face index offset
            almost allways 0 but for .obj files there is a 1
            p x y z
                vertex
            d i0 i1 i2
                triangle
            ' ' at the beginning is a comment line
            also anything after the expected values is ignored

        Version: 2023-03-21
            f i0 i1 r g b
            everything after '/' is a comment

        Version: 2023-01-27
            o i0 i1 i2 i3
                pOligon, 2 triangles
            c i r g b
                color single face
            k i0 i1 r0 g0 b0 r1 g1 b1
                kariert
            v iv if
                change index offset of vertex/face

        Version: 2024-04-01
            index can now have a sign
            all are relative to the offset
            no absolut and relative

        Version: 2024-05-24
            qube
                create vertexes and faces of a cuboid
                by giving min and max for each axis

        Version: 2024-06-08
            fan
                connects all indexe to the first index
            ring
                splits indexe into 2 halfs
                connects them

        Version: 2025-04-14
            longer names
            vertex
            faceN
            colorN
            math
                can store/do math with numbers
            everything is now seperated with :
            white space is removed

        Version:
            Multiline
            Face Normals flipped

        Errors:
            stop after first error ?
            stop line parsing, and keep going
            & list all errors

        Line Parser:
            can be multiLine so think of a different name ?
            element parser ?
            output: point(s), face(s), color(s)
    */
   /*   General File Parser
    *   turn lines into segments
    *   check segment types (unsigned int, signed int, float, string, variables)
    *   complex types (point, angle?, color)
    *   multiline
    *   brackets vs no brackets
    *   
    */

    partial class CBodyParserData
    {
        private class EUnknownIdentifyer : Exception
        {
            public EUnknownIdentifyer(string identifyer) : base(
                "Unkown Type '" + identifyer + "'."
                )
            { }
        }
        private class EInvalidSegmentCount : Exception
        {
            public EInvalidSegmentCount(int expected, int recieved) : base(
                "Invalid Segment Count, Expected " + expected + ", recieved " + recieved + "."
                )
            { }
            public EInvalidSegmentCount(string str) : base(
                "Invalid Segment Count, " + str + "."
                )
            { }
        }



        public PolyHedra Template;
        public TNamedMemory.Context MemoryContext;
        public SIndexInfo IndexInfo;
        public SAxis Axis;

        private class MemoryIO
        {
            public CBodyParserData Memory;
            public MemoryIOValues Innput;
            public MemoryIOChanges Output;
            public MemoryIO(CBodyParserData Memory, MemoryIOValues Innput, MemoryIOChanges Output)
            {
                this.Memory = Memory;
                this.Innput = Innput;
                this.Output = Output;
            }
        }



        private enum EParserBits
        {
            OpenBit = 0b00,
            CloseBit = 0b01,
            ForewardBit = 0b00,
            BackwardBit = 0b10,

            ForewardOpen   =  OpenBit | ForewardBit,
            ForewardClosed = CloseBit | ForewardBit,
            BackwardOpen   =  OpenBit | BackwardBit,
            BackwardClosed = CloseBit | BackwardBit,
        }
        private struct SLineParser
        {
            public string Identifyer;
            public Action<MemoryIO> FuncSimple;
            public Action<MemoryIO, EParserBits> FuncComplex;
            public EParserBits FaceComplexBits;
            private bool IsComplex;
            private bool HasComplexBits;

            public IComparison[] Comparisons;

            public SLineParser(string identifyer, IComparison comp, Action<MemoryIO> func)
            {
                Identifyer = identifyer;
                FuncSimple = func;
                FuncComplex = null;
                FaceComplexBits = 0;
                IsComplex = false;
                HasComplexBits = false;

                Comparisons = new IComparison[] { comp };
            }
            public SLineParser(string identifyer, IComparison comp, Action<MemoryIO, EParserBits> func, EParserBits bits)
            {
                Identifyer = identifyer;
                FuncSimple = null;
                FuncComplex = func;
                FaceComplexBits = bits;
                IsComplex = true;
                HasComplexBits = true;

                Comparisons = new IComparison[] { comp };
            }
            public SLineParser(string identifyer, IComparison comp, Action<MemoryIO, EParserBits> func)
            {
                Identifyer = identifyer;
                FuncSimple = null;
                FuncComplex = func;
                FaceComplexBits = 0;
                IsComplex = true;
                HasComplexBits = false;

                Comparisons = new IComparison[] { comp };
            }

            private static EParserBits ParseBitsFromIdentifyer(string identifyer)
            {
                EParserBits bits = 0;
                char close = identifyer[-2];
                char direction = identifyer[-1];
                if (THelp.CharIsAnyOf(close, "OC") && THelp.CharIsAnyOf(direction, "FB"))
                {
                    if (close == 'O' && direction == 'F') { bits = EParserBits.ForewardOpen; }
                    if (close == 'C' && direction == 'F') { bits = EParserBits.ForewardClosed; }
                    if (close == 'O' && direction == 'B') { bits = EParserBits.BackwardOpen; }
                    if (close == 'C' && direction == 'B') { bits = EParserBits.BackwardClosed; }
                }
                return bits;
            }
            public void CallFunc(MemoryIO mio, string identifyer)
            {
                if (!IsComplex)
                {
                    FuncSimple(mio);
                }
                else
                {
                    if (HasComplexBits)
                    {
                        FuncComplex(mio, FaceComplexBits);
                    }
                    else
                    {
                        FuncComplex(mio, ParseBitsFromIdentifyer(identifyer));
                    }
                }
            }

            public bool CheckIdentifyer(string identifyer)
            {
                return Identifyer == identifyer;
            }
            public string CheckNum(int num)
            {
                for (int i = 0; i < Comparisons.Length; i++)
                {
                    if (!Comparisons[i].Compare(num))
                    {
                        return Comparisons[i].ToString(num);
                    }
                }
                return null;
            }
        }
        private SLineParser[] Parsers;
        private SLineParser Parsers_FindIdentifyer(string identifyer)
        {
            for (int i = 0; i < Parsers.Length; i++)
            {
                if (Parsers[i].CheckIdentifyer(identifyer))
                {
                    return Parsers[i];
                }
            }
            throw new EUnknownIdentifyer(identifyer);
        }
        //private void ParseSegments(string[] seg)
        private void ParseSegments(StringValue[] seg)
        {
            string identifyer;
            if (seg[0].GetType() == typeof(StringValue_String))
            {
                StringValue_String val = (StringValue_String)seg[0];
                identifyer = val.Text;
            }
            else { ConsoleLog.LogError("ParseSegments() identifyer : Type"); identifyer = ""; }

            SLineParser parser = Parsers_FindIdentifyer(identifyer);

            StringValue[] values = new StringValue[seg.Length - 1];
            for (int i = 1; i < seg.Length; i++)
            {
                values[i - 1] = seg[i];
            }

            string str = parser.CheckNum(values.Length);
            if (str == null)
            {
                MemoryIOValues val = new MemoryIOValues(this, values);
                MemoryIOChanges bcl = new MemoryIOChanges(this);
                parser.CallFunc(new MemoryIO(this, val, bcl), identifyer);
            }
            else
            {
                throw new EInvalidSegmentCount(str);
            }
        }



        private static void FuncCorner(MemoryIO mio)
        {
            mio.Output.Change(mio.Innput.ToPoint3D());
        }
        private static void FuncArc(MemoryIO mio)
        {
            /*  Arc
                     arcY / arcX / arcC
                middle      y x c
                distance    d
                base angle  ba
                step angle  sa
                step count  sc
            */
        }
        private static void FuncCircle(MemoryIO mio)
        {
            /* Circle
             *  segments          int
             *  segment offset    int
             *  center            x y c
             *  radius            float
             *  nomal axis        x y c
             *  angle offset      float
             */

            int step_num = (int)mio.Innput.ToFloat();
            int step_off = (int)mio.Innput.ToFloat();

            Point3D center = mio.Innput.ToPoint3D();
            float radius = mio.Innput.ToFloat();

            Angle3D angle = new Angle3D(mio.Innput.ToPoint3D());
            double offset = Angle3D.DegreeToRad(mio.Innput.ToFloat());

            int step_abs = 0;
            if (step_num > 0) { step_abs = +step_num; offset += Math.PI * 0; }
            if (step_num < 0) { step_abs = -step_num; offset += Math.PI * 1; }

            for (int i = 0; i < step_abs; i++)
            {
                angle.D = (i + step_off) * (Math.Tau / step_num) + offset;
                Point3D p = new Point3D(radius, 0, 0);
                p = p - angle;
                p = p + center;
                mio.Output.Change(p);
            }
        }

        private static void FuncFace3(MemoryIO mio)
        {
            uint[] idx = mio.Innput.IndexCorner(3);
            mio.Output.Change(new IndexTriangle(idx[0], idx[1], idx[2]));
        }
        private static void FuncFace4(MemoryIO mio)
        {
            uint[] idx = mio.Innput.IndexCorner(4);
            mio.Output.Change(new IndexTriangle(idx[0], idx[1], idx[2]));
            mio.Output.Change(new IndexTriangle(idx[2], idx[1], idx[3]));
        }
        private static void FuncFaceN(MemoryIO mio)
        {
            uint[] idx = mio.Innput.IndexCorner(mio.Innput.length);
            for (int i = 2; i < mio.Innput.length; i++)
            {
                if (i % 2 == 0)
                {
                    mio.Output.Change(new IndexTriangle(idx[i - 2], idx[i - 1], idx[i - 0]));
                }
                else
                {
                    mio.Output.Change(new IndexTriangle(idx[i - 1], idx[i - 2], idx[i - 0]));
                }
            }
        }

        private static void FuncBelt(MemoryIO mio, EParserBits faceBits)
        {
            bool close = (faceBits & EParserBits.CloseBit) != 0;
            bool backward = (faceBits & EParserBits.BackwardBit) != 0;

            int half = mio.Innput.length / 2;

            uint[] idx0 = mio.Innput.IndexCorner(half);
            uint[] idx1 = mio.Innput.IndexCorner(half);

            for (int i = 1; i < half; i++)
            {
                if (!backward)
                {
                    mio.Output.Change(new IndexTriangle(idx0[i - 1], idx0[i - 0], idx1[i - 1]));
                    mio.Output.Change(new IndexTriangle(idx1[i - 1], idx0[i - 0], idx1[i - 0]));
                }
                else
                {
                    mio.Output.Change(new IndexTriangle(idx1[i - 1], idx0[i - 0], idx0[i - 1]));
                    mio.Output.Change(new IndexTriangle(idx1[i - 0], idx0[i - 0], idx1[i - 1]));
                }
            }

            if (close)
            {
                if (!backward)
                {
                    mio.Output.Change(new IndexTriangle(idx0[half - 1], idx0[0], idx1[half - 1]));
                    mio.Output.Change(new IndexTriangle(idx1[half - 1], idx0[0], idx1[0]));
                }
                else
                {
                    mio.Output.Change(new IndexTriangle(idx0[0], idx0[half - 1], idx1[half - 1]));
                    mio.Output.Change(new IndexTriangle(idx0[0], idx1[half - 1], idx1[0]));
                }
            }
        }
        private static void FuncFan(MemoryIO mio, EParserBits faceBits)
        {
            bool close = (faceBits & EParserBits.CloseBit) != 0;
            bool backward = (faceBits & EParserBits.BackwardBit) != 0;
            int len = mio.Innput.length - 1;

            uint middle = mio.Innput.IndexCorner();
            uint[] blade = mio.Innput.IndexCorner(len);

            for (int i = 1; i < len; i++)
            {
                if (!backward)
                {
                    mio.Output.Change(new IndexTriangle(middle, blade[i - 1], blade[i - 0]));
                }
                else
                {
                    mio.Output.Change(new IndexTriangle(middle, blade[i - 0], blade[i - 1]));
                }
            }

            if (close)
            {
                if (!backward)
                {
                    mio.Output.Change(new IndexTriangle(middle, blade[len - 1], blade[0]));
                }
                else
                {
                    mio.Output.Change(new IndexTriangle(middle, blade[0], blade[len - 1]));
                }
            }
        }

        private static void FuncQube(MemoryIO mio)
        {
            Point3D p0 = mio.Innput.ToPoint3D();
            Point3D p1 = mio.Innput.ToPoint3D();

            uint idx = (uint)mio.Memory.Template.CornerCount();

            mio.Output.Change(new Point3D(p0.Y, p0.X, p0.C)); // 0 - - -
            mio.Output.Change(new Point3D(p1.Y, p0.X, p0.C)); // 1 + - -
            mio.Output.Change(new Point3D(p0.Y, p0.X, p1.C)); // 2 - - +
            mio.Output.Change(new Point3D(p1.Y, p0.X, p1.C)); // 3 + - +
            mio.Output.Change(new Point3D(p0.Y, p1.X, p0.C)); // 4 - + -
            mio.Output.Change(new Point3D(p1.Y, p1.X, p0.C)); // 5 + + -
            mio.Output.Change(new Point3D(p0.Y, p1.X, p1.C)); // 6 - + +
            mio.Output.Change(new Point3D(p1.Y, p1.X, p1.C)); // 7 + + +

            mio.Output.Change(new IndexTriangle(idx + 0, idx + 1, idx + 3));
            mio.Output.Change(new IndexTriangle(idx + 0, idx + 3, idx + 2));
            mio.Output.Change(new IndexTriangle(idx + 0, idx + 5, idx + 1));
            mio.Output.Change(new IndexTriangle(idx + 0, idx + 4, idx + 5));
            mio.Output.Change(new IndexTriangle(idx + 0, idx + 2, idx + 6));
            mio.Output.Change(new IndexTriangle(idx + 0, idx + 6, idx + 4));
            mio.Output.Change(new IndexTriangle(idx + 3, idx + 1, idx + 7));
            mio.Output.Change(new IndexTriangle(idx + 1, idx + 5, idx + 7));
            mio.Output.Change(new IndexTriangle(idx + 2, idx + 3, idx + 7));
            mio.Output.Change(new IndexTriangle(idx + 6, idx + 2, idx + 7));
            mio.Output.Change(new IndexTriangle(idx + 5, idx + 4, idx + 7));
            mio.Output.Change(new IndexTriangle(idx + 4, idx + 6, idx + 7));
        }

        private static void FuncColorN(MemoryIO mio)
        {
            uint[] idx = mio.Innput.IndexFace(2);
            uint col = mio.Innput.Color();

            for (uint i = idx[0]; i <= idx[1]; i++)
            {
                mio.Output.Change(i, col);
            }
        }

        private static void FuncIndexOffset(MemoryIO mio)
        {
            mio.Memory.IndexInfo.Offset_Corner = mio.Innput.IndexCornerNoLimit();
            mio.Memory.IndexInfo.Offset_Face = mio.Innput.IndexFaceNoLimit();
        }
        private static void FuncInfo(MemoryIO mio)
        {
            ConsoleLog.Log("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
            ConsoleLog.Log("Corner Count:" + mio.Memory.IndexInfo.Length_Corner);
            ConsoleLog.Log("Face Count:" + mio.Memory.IndexInfo.Length_Face);
            ConsoleLog.Log("Corner Offset:" + mio.Memory.IndexInfo.Offset_Corner);
            ConsoleLog.Log("Face Offset:" + mio.Memory.IndexInfo.Offset_Face);
            ConsoleLog.Log("Axis Y Idx Dir:" + mio.Memory.Axis.IdxY + " " + mio.Memory.Axis.DirY.ToString("+0;-0"));
            ConsoleLog.Log("Axis X Idx Dir:" + mio.Memory.Axis.IdxX + " " + mio.Memory.Axis.DirX.ToString("+0;-0"));
            ConsoleLog.Log("Axis C Idx Dir:" + mio.Memory.Axis.IdxC + " " + mio.Memory.Axis.DirC.ToString("+0;-0"));

            ConsoleLog.Log("Dimensions:");
            float min_y = float.PositiveInfinity, min_x = float.PositiveInfinity, min_c = float.PositiveInfinity;
            float max_y = float.NegativeInfinity, max_x = float.NegativeInfinity, max_c = float.NegativeInfinity;
            float avg_y = 0, avg_x = 0, avg_c = 0;
            /*for (int i = 0; i < mio.Memory.Template.TriFloatList.Count; i++)
            {
                TriFloat corner = mio.Memory.Template.TriFloatList[i];
                if (min_y > corner.Y) { min_y = corner.Y; }
                if (min_x > corner.X) { min_x = corner.X; }
                if (min_c > corner.C) { min_c = corner.C; }
                if (max_y < corner.Y) { max_y = corner.Y; }
                if (max_x < corner.X) { max_x = corner.X; }
                if (max_c < corner.C) { max_c = corner.C; }
                avg_y += corner.Y / mio.Memory.Template.TriFloatList.Count;
                avg_x += corner.X / mio.Memory.Template.TriFloatList.Count;
                avg_c += corner.C / mio.Memory.Template.TriFloatList.Count;
            }*/

            float dif_y = (max_y - min_y);
            float dif_x = (max_x - min_x);
            float dif_c = (max_c - min_c);
            string format = "+0.00; -0.00; 0.00";
            ConsoleLog.Log("  Min: " + min_y.ToString(format) + " " + min_x.ToString(format) + " " + min_c.ToString(format));
            ConsoleLog.Log("  Max: " + max_y.ToString(format) + " " + max_x.ToString(format) + " " + max_c.ToString(format));
            ConsoleLog.Log("Range: " + dif_y.ToString(format) + " " + dif_x.ToString(format) + " " + dif_c.ToString(format));
            ConsoleLog.Log("  Avg: " + avg_y.ToString(format) + " " + avg_x.ToString(format) + " " + avg_c.ToString(format));

            ConsoleLog.Log(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
        }
        private static void FuncMath(MemoryIO mio)
        {
            mio.Memory.MemoryContext.ParseF(mio.Innput.String());
        }
        private static void FuncAxis(MemoryIO mio)
        {
            SAxis.Change(mio.Innput.String(), out mio.Memory.Axis.IdxY, out mio.Memory.Axis.DirY);
            SAxis.Change(mio.Innput.String(), out mio.Memory.Axis.IdxX, out mio.Memory.Axis.DirX);
            SAxis.Change(mio.Innput.String(), out mio.Memory.Axis.IdxC, out mio.Memory.Axis.DirC);
        }



        private SLineParser[] Parsers_Short1()
        {
            return new SLineParser[]
            {
                new SLineParser("p", new SComparisonMin(3), FuncCorner),
                new SLineParser("d", new SComparisonMin(3), FuncFace3),
                new SLineParser("f", new SComparisonMin(5), FuncColorN),

                //new SLineParser("p", new SComparisonEql(3), FuncCorner),
                //new SLineParser("d", new SComparisonEql(3), FuncFace3),
                //new SLineParser("o", new SComparisonEql(4), FuncFace4),
                //new SLineParser("f", new SComparisonEql(5), FuncColorN),
                //new SLineParser("v", new SComparisonEql(2), FuncIndexOffset),
            };
        }
        private SLineParser[] Parsers_Short2()
        {
            return new SLineParser[]
            {
                new SLineParser("o", new SComparisonMin(4), FuncFace4),
                new SLineParser("v", new SComparisonMin(2), FuncIndexOffset),

                //new SLineParser("p", new SComparisonEql(3), FuncCorner),
                //new SLineParser("d", new SComparisonEql(3), FuncFace3),
                //new SLineParser("o", new SComparisonEql(4), FuncFace4),
                //new SLineParser("f", new SComparisonEql(5), FuncColorN),
                //new SLineParser("v", new SComparisonEql(2), FuncIndexOffset),
            };
        }
        private SLineParser[] Parsers_Long1()
        {
            return new SLineParser[]
            {
                new SLineParser("vertex", new SComparisonEql(3), FuncCorner),
                new SLineParser("faceN", new SComparisonMin(3), FuncFaceN),
                new SLineParser("colorN", new SComparisonEql(5), FuncColorN),

                new SLineParser("qube", new SComparisonEql(6), FuncQube),
            };
        }
        private SLineParser[] Parsers_Long2()
        {
            return new SLineParser[]
            {
                new SLineParser("index", new SComparisonEql(2), FuncIndexOffset),
                new SLineParser("info", new SComparisonEql(0), FuncInfo),
                new SLineParser("math", new SComparisonEql(1), FuncMath),
                new SLineParser("axis", new SComparisonEql(3), FuncAxis),
            };
        }
        private SLineParser[] Parsers_ComplexHard()
        {
            return new SLineParser[]
            {
                new SLineParser("belt0", new SComparisonMod(2), FuncBelt, EParserBits.ForewardOpen),
                new SLineParser("belt1", new SComparisonMod(2), FuncBelt, EParserBits.ForewardClosed),
                new SLineParser("fan0", new SComparisonMin(1), FuncFan, EParserBits.ForewardClosed),
                new SLineParser("fan1", new SComparisonMin(1), FuncFan, EParserBits.BackwardClosed),
                new SLineParser("circle", new SComparisonEql(10), FuncCircle),
            };
        }
        private SLineParser[] Parsers_ComplexSoft()
        {
            return new SLineParser[]
            {
                new SLineParser("belt", new SComparisonMod(2), FuncBelt),
                new SLineParser("fan", new SComparisonMin(1), FuncFan),
            };
        }
        private void Parsers_Use(SLineParser[] persers)
        {
            SLineParser[] use = new SLineParser[Parsers.Length + persers.Length];
            for (int i = 0; i < Parsers.Length; i++)
            {
                use[i] = Parsers[i];
            }
            for (int i = 0; i < persers.Length; i++)
            {
                use[i + Parsers.Length] = persers[i];
            }
            Parsers = use;
        }

        public CBodyParserData()
        {
            /*
                split into multiple files ?
                    file to create abstract body
                    then use second file to add color / textures
            */

            Parsers = new SLineParser[0];
            /*Parsers = new SLineParser[]
            {
                //new SLineParser("p", new SComparisonEql(3), FuncCorner),
                //new SLineParser("d", new SComparisonEql(3), FuncFace3),
                //new SLineParser("o", new SComparisonEql(4), FuncFace4),
                //new SLineParser("f", new SComparisonEql(5), FuncColorN),
                //new SLineParser("v", new SComparisonEql(2), FuncIndexOffset),

                new SLineParser("vertex", new SComparisonEql(3), FuncCorner),

                new SLineParser("faceN", new SComparisonMin(3), FuncFaceN),

                //new SLineParser("belt0", new SComparisonMod(2), FuncBelt, EParserBits.ForewardOpen),
                //new SLineParser("belt1", new SComparisonMod(2), FuncBelt, EParserBits.ForewardClosed),
                //new SLineParser("fan0", new SComparisonMin(1), FuncFan, EParserBits.ForewardClosed),
                //new SLineParser("fan1", new SComparisonMin(1), FuncFan, EParserBits.BackwardClosed),

                new SLineParser("qube", new SComparisonEql(6), FuncQube),

                new SLineParser("colorN", new SComparisonEql(5), FuncColorN),

                new SLineParser("index", new SComparisonEql(2), FuncIndexOffset),
                new SLineParser("info", new SComparisonEql(0), FuncInfo),
                new SLineParser("math", new SComparisonEql(1), FuncMath),
                new SLineParser("axis", new SComparisonEql(3), FuncAxis),
            };*/



            MemoryContext = new TNamedMemory.Context();

            IndexInfo.Length_Corner = 0;
            IndexInfo.Length_Face = 0;
            IndexInfo.Offset_Corner = 0;
            IndexInfo.Offset_Face = 0;

            Axis.IdxY = 0;
            Axis.IdxX = 1;
            Axis.IdxC = 2;
            //Axis.DirY = +1;
            //Axis.DirX = +1;
            //Axis.DirC = +1;
            Axis.DirY = -1;
            Axis.DirX = +1;
            Axis.DirC = +1;

            Template = new PolyHedra();
            Template.Edit_Begin();
        }
        ~CBodyParserData()
        {
            Parsers = null;
            MemoryContext = null;
            Template = null;
        }



        private static string LineRemoveComment(string line, char commentChar)
        {
            int commentIdx = line.IndexOf(commentChar);
            if (commentIdx != -1)
            {
                return line.Substring(0, commentIdx);
            }
            return line;
        }
        private static string LineManageMultiline(string line, string[] lines, ref int l)
        {
            if (line.Contains('{'))
            {
                while (!lines[l].Contains('}') && l < lines.Length)
                {
                    l++;
                    line += lines[l];
                }
                line = line.Replace('{', ' ');
                line = line.Replace('}', ' ');
            }
            return line;
        }
        private static bool LineIsIgnore(string line)
        {
            return (line.Length == 0 || line[0] == ' ' || line[0] == '\t');
        }



        /* Parser
         *  1.  Text to Segments
         *  2.  Segments to Values
         *  3.  Values to Changes
         *  4.  Changes to Body
         *  use new one to only make New Bodys
         *  create even newer parser
         *  math a = 2 + 3
         *  vertex { 0 : a : 2 }
         */
        private bool CheckHeader(string[] lines)
        {
            if (lines[0] != "YMT?")
            {
                return false;
            }
            return true;
        }
        public PolyHedra ParseYMT(string[] lines)
        {
            if (!CheckHeader(lines))
            {
                TBodyFile.DebugError = "Header as not \"YMT?\"";
                return PolyHedra.Generate.Error();
            }

            int l = -1;
            try
            {
                Parsers_Use(Parsers_Long1());
                Parsers_Use(Parsers_Long2());
                Parsers_Use(Parsers_ComplexSoft());
                for (l = 1; l < lines.Length; l++)
                {
                    string line = lines[l];

                    line = LineRemoveComment(line, '/');
                    line = LineManageMultiline(line, lines, ref l);
                    if (!LineIsIgnore(line))
                    {
                        string[] seg;
                        seg = line.Split(':', StringSplitOptions.TrimEntries);
                        ConsoleLog.LogError("ParseYMT() UnAvailable");
                        //ParseSegments(seg);
                    }

                    if (TBodyFile.DebugIndexInfo_List != null)
                    {
                        TBodyFile.DebugIndexInfo_List.Add(IndexInfo);
                    }
                }
                Template.Edit_Stop();
                return Template;
            }
            catch (Exception ex)
            {
                TBodyFile.DebugError = "Line:" + (l + 1) + ":" + ex.Message;
                ConsoleLog.LogError(TBodyFile.DebugError);
                return PolyHedra.Generate.Error();
            }
        }
        public PolyHedra GeneralParser(string[] lines, ref int l)
        {
            //ConsoleLog.Log("====####====####====");

            StringHierarchy segMain = new StringHierarchy();

            {
                List<StringHierarchy> segList = new List<StringHierarchy>();
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    line = LineRemoveComment(line, '/');
                    line = LineManageMultiline(line, lines, ref i);
                    if (!LineIsIgnore(line))
                    {
                        segList.Add(new StringHierarchy(segMain, null, line, "\n"));
                    }
                }
                segMain.Child = segList.ToArray();
            }

            //ConsoleLog.LogInfo(segMain.ToLines());

            {
                for (int i = 0; i < segMain.Child.Length; i++)
                {
                    StringHierarchy seg = segMain.Child[i];
                    string[] strs;
                    if (seg.Text.Contains(':'))
                    {
                        strs = seg.Text.Split(":", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    }
                    else
                    {
                        strs = seg.Text.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    }
                    List<StringHierarchy> segList = new List<StringHierarchy>();
                    for (int j = 0; j < strs.Length; j++)
                    {
                        segList.Add(new StringHierarchy(seg, null, strs[j], " : "));
                    }
                    seg.Text = "";
                    seg.Child = segList.ToArray();
                }
            }

            //ConsoleLog.LogInfo(segMain.ToLines());

            /*StringValue valMain = new StringValue_AnyOfArray(new StringValue[] {
                new StringValue_AllOfArray(new StringValue[] {  //  0
                    new StringValue_AnyOfValue(new StringValue[] { new StringValue_Header("vertex"), new StringValue_Header("p") }),
                    new StringValue_AnyOfValue(new StringValue[] { new StringValue_Variable(), new StringValue_Float(), }),
                    new StringValue_AnyOfValue(new StringValue[] { new StringValue_Variable(), new StringValue_Float(), }),
                    new StringValue_AnyOfValue(new StringValue[] { new StringValue_Variable(), new StringValue_Float(), }),
                    //new StringValue_Header("vertex") | new StringValue_Header("p"),
                    //new StringValue_Variable() | new StringValue_Float(),
                    //new StringValue_Variable() | new StringValue_Float(),
                    //new StringValue_Variable() | new StringValue_Float(),
                }),
                new StringValue_AllOfArray(new StringValue[] {  //  1
                    new StringValue_Header("index"),
                    new StringValue_Index(),
                    new StringValue_Index(),
                }),
                new StringValue_AllOfArray(new StringValue[] {  //  2
                    new StringValue_Header("math"),
                    new StringValue_String(),
                }),
                new StringValue_AllOfArray(new StringValue[] {  //  3
                    new StringValue_Header("belt1"),
                }, new StringValue_Index()),
                new StringValue_AllOfArray(new StringValue[] {  //  4
                    new StringValue_Header("colorN"),
                    new StringValue_Index(),
                    new StringValue_Index(),
                    new StringValue_Index(),
                    new StringValue_Index(),
                    new StringValue_Index(),
                }),
                new StringValue_AnyOfArray(new StringValue[] {  //  5
                    new StringValue_String(),
                }),
            });*/

            StringValue valMain = new StringValue_AnyOfArray(new StringValue[] {
                new StringValue_AnyOfArray(new StringValue[] {
                    new StringValue_String(),
                }),
            });

            if (!valMain.Check(segMain)) { return PolyHedra.Generate.Error(); }
            valMain.Parse(segMain);

            //ConsoleLog.Log(valMain.ToLine("\n"));

            if (valMain.GetType() == typeof(StringValue_AnyOfArray))
            {
                StringValue_AnyOfArray val1 = (StringValue_AnyOfArray)valMain;
                for (int i = 0; i < val1.Arbitrary.Count; i++)
                {
                    l = i;
                    if (val1.Arbitrary[i].GetType() == typeof(StringValue_AnyOfArray))
                    {
                        StringValue_AnyOfArray val2 = (StringValue_AnyOfArray)val1.Arbitrary[i];
                        ParseSegments(val2.Arbitrary.ToArray());
                    }
                    else { ConsoleLog.LogError("val[" + i + "] : Type"); }
                }
            }
            else { ConsoleLog.LogError("valMain : Type"); }

            Template.Edit_Stop();
            return Template;
        }
        public PolyHedra ParseMultiVersion(string[] lines)
        {
            int l = -1;
            try
            {
                Parsers_Use(Parsers_Short1());
                Parsers_Use(Parsers_Short2());
                Parsers_Use(Parsers_Long1());
                Parsers_Use(Parsers_Long2());
                Parsers_Use(Parsers_ComplexHard());

                GeneralParser(lines, ref l);

                /*for (l = 0; l < lines.Length; l++)
                {
                    string line = lines[l];

                    if (l == 0 && line.Length != 0 && (line[0] == '0' || line[0] == '1'))
                    {
                        if (line[0] == '1') { IndexInfo.Absolut_Offset = 1; }
                        continue;
                    }

                    line = LineRemoveComment(line, '/');
                    line = LineManageMultiline(line, lines, ref l);
                    if (!LineIsIgnore(line))
                    {
                        string[] seg;
                        if (line.Contains(':')) { seg = line.Split(':', StringSplitOptions.TrimEntries); }
                        else { seg = line.Split(' ', StringSplitOptions.RemoveEmptyEntries); }
                        ConsoleLog.LogError("ParseMultiVersion() UnAvailable");
                        ParseSegments(seg);
                    }

                    TBodyFile.DebugIndexInfo_List.Add(IndexInfo);
                }*/

                Template.Edit_Stop();
                return Template;
            }
            catch (Exception ex)
            {
                TBodyFile.DebugError = "Line:" + (l + 1) + ":" + ex.Message;
                ConsoleLog.LogError(TBodyFile.DebugError);
                //ConsoleLog.LogError("Trace:\n" + ex.StackTrace);
                return null;
            }
        }
    }
    public static class TBodyFile
    {
        public static List<SIndexInfo> DebugIndexInfo_List;

        public static string DebugError;

        public static PolyHedra LoadTextLineInfo(string[] lines, bool indexInfo)
        {
            DebugIndexInfo_List = null;
            if (indexInfo) { DebugIndexInfo_List = new List<SIndexInfo>(); }
            DebugError = "";

            CBodyParserData parser = new CBodyParserData();
            return parser.ParseMultiVersion(lines);
            //return parser.ParseYMT(lines);
        }
        public static PolyHedra LoadTextFile(string path)
        {
            ConsoleLog.LogLoad("PolyHedra: " + path);

            if (!File.Exists(path))
            {
                ConsoleLog.LogError("File '" + path + "' not Found");
                return PolyHedra.Generate.Error();
            }

            string[] lines = File.ReadAllLines(path);
            PolyHedra poly = LoadTextLineInfo(lines, false);
            if (poly == null) { return PolyHedra.Generate.Error(); }

            ConsoleLog.LogDebug(
                "File '" + path + "' loaded with " +
                poly.CornerCount() + " Corners and " +
                poly.FaceCount() + " Faces.");
            return poly;
        }

        public static void ShowInfoAllParsers()
        {
            (string, string)[] Parsers = new (string, string)[]
            {
                ("p", null),
                ("d", null),
                ("o", null),
                ("f", null),
                ("v", null),

                ("vertex", null),

                ("faceN", null),
                ("belt0",
                    "belt0:a_0:a_1:a_2:a_3: ... ::a_n:b_0:b_1:b_2:b_3: ... :b_n\n" +
                    "\n" +
                    "  a_0-----a_1-----a_2-----a_3-----a_4 ... a_n     a_0              0-------1-------2\n" +
                    "   |     / |     / |     / |     / |       |       |                               |\n" +
                    "   |   /   |   /   |   /   |   /   |       |       |                               |\n" +
                    "   | /     | /     | /     | /     |       |       |                               |\n" +
                    "  b_0-----b_1-----b_2-----b_3-----b_4 ... b_n     b_0              n  ...  4-------3\n"
                ),

                ("belt1",
                    "belt1:a_0:a_1:a_2:a_3: ... ::a_n:b_0:b_1:b_2:b_3: ... :b_n\n" +
                    "\n" +
                    "  a_0-----a_1-----a_2-----a_3-----a_4 ... a_n-----a_0              0-------1-------2\n" +
                    "   |     / |     / |     / |     / |       |     / |               |               |\n" +
                    "   |   /   |   /   |   /   |   /   |       |   /   |               |               |\n" +
                    "   | /     | /     | /     | /     |       | /     |               |               |\n" +
                    "  b_0-----b_1-----b_2-----b_3-----b_4 ... b_n-----b_0              n  ...  4-------3\n"
                ),
                ("fan0", null),
                ("fan1", null),

                ("qube", null),

                ("colorN",
                    "colorN: i0 : i1 : R : G : B\n" +
                    "   in0: lower index (inclusive)\n" +
                    "   in1: upper index (inclusive)\n" +
                    "   R: [000;255] Red\n" +
                    "   G: [000;255] Green\n" +
                    "   B: [000;255] Blue\n"
                ),

                ("index", null),
                ("info", null),
                ("math",
                    "Used to change Value of coordinate Variables\n" +
                    "math: test_0 = 123\n" +
                    "   change Variable 'test_0' to value (123)\n" +
                    "math: test_1 = test_0 + 123\n" +
                    "   change Variable 'test_1' to value (test_0 + 123)\n" +
                    "signs:\n" +
                    "   + : takes absolut positive Value of Variable\n" +
                    "   - : takes absolut negative Value of Variable\n" +
                    "   . : takes relative positive Value of Variable\n" +
                    "   ! : takes relative negative Value of Variable\n"
                ),
                ("axis",
                    "changes how Points are interpreted\n" +
                    "    0 : set how point value 0 should be interpreted\n" +
                    "    1 : set how point value 1 should be interpreted\n" +
                    "    2 : set how point value 2 should be interpreted\n" +
                    "valid Values: Y y X x C c\n" +
                    "    upper Case: value will be used in that Coordinate using a Factor of +1\n" +
                    "    lower Case: value will be used in that Coordinate using a Factor of -1\n"
                ),
            };

            for (int i = 0; i < Parsers.Length; i++)
            {
                if (Parsers[i].Item2 != null)
                {
                    ConsoleLog.LogTest(Parsers[i].Item1 + "\n" + Parsers[i].Item2);
                }
                else
                {
                    ConsoleLog.LogTest(Parsers[i].Item1);
                }
            }
        }
    }
}
