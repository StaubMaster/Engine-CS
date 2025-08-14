using System;

using Engine3D.Abstract3D;



namespace Engine3D.BodyParse
{
    class MemoryIOValues
    {
        private class ECornerIndex : Exception
        {
            public ECornerIndex(uint idx, uint limit, uint offset) : base(
                "Invalid Corner Index: " + idx +
                "(" + ((int)idx - (int)offset).ToString("+#;-#;0") + ")" +
                ", Limit is " + limit +
                "(" + ((int)limit - (int)offset).ToString("+#;-#;0") + ")" +
                "."
                )
            { }
        }
        private class EFaceIndex : Exception
        {
            public EFaceIndex(uint idx, uint limit, uint offset) : base(
                "Invalid Face Index: " + idx +
                "(" + ((int)idx - (int)offset).ToString("+#;-#;#") + ")" +
                ", Limit is " + limit +
                "(" + ((int)limit - (int)offset).ToString("+#;-#;#") + ")" +
                "."
                )
            { }
        }



        private CBodyParserData BodyParser;
        //private string[] values;
        private StringValue[] values;
        private uint index;
        public int length { get { return values.Length; } }

        //public ParserValues(CBodyParser body_parser, string[] val)
        public MemoryIOValues(CBodyParserData body_parser, StringValue[] val)
        {
            BodyParser = body_parser;
            values = val;
            index = 0;
        }

        public string String()
        {
            //string str = values[index];
            string str;

            StringValue v = values[index];
            if (v.GetType() == typeof(StringValue_String))
            {
                StringValue_String val = (StringValue_String)v;
                str = val.Text;
            }
            else { ConsoleLog.LogError("String() : Type"); str = ""; }
            index++;

            return str;
        }

        public float ToFloat()
        {
            float f = float.NaN;
            StringValue v;

            if (index < 0 || index >= values.Length) { ConsoleLog.LogError("ToFloat() : Limit"); }

            v = values[index];
            if (v.GetType() == typeof(StringValue_String))
            {
                StringValue_String val = (StringValue_String)v;
                f = BodyParser.MemoryContext.ParseF(val.Text);
            }
            else { ConsoleLog.LogError("ToFloat() : Type"); }
            index++;

            return f;
        }
        public Point3D ToPoint3D()
        {
            float[] AxisArr = new float[3];

            AxisArr[0] = ToFloat();
            AxisArr[1] = ToFloat();
            AxisArr[2] = ToFloat();

            if (float.IsNaN(AxisArr[0])) { ConsoleLog.LogError("ToTriFloat() 0 : Type"); }
            if (float.IsNaN(AxisArr[1])) { ConsoleLog.LogError("ToTriFloat() 1 : Type"); }
            if (float.IsNaN(AxisArr[2])) { ConsoleLog.LogError("ToTriFloat() 2 : Type"); }

            return new Point3D(
                AxisArr[BodyParser.Axis.IdxY] * BodyParser.Axis.DirY,
                AxisArr[BodyParser.Axis.IdxX] * BodyParser.Axis.DirX,
                AxisArr[BodyParser.Axis.IdxC] * BodyParser.Axis.DirC
                );
        }

        public uint IndexCornerNoLimit()
        {
            //SIndex idx = new SIndex(values[index]);

            StringValue v = values[index];
            SIndex idx = new SIndex();

            /*if (v.GetType() == typeof(StringValue_Index))
            {
                StringValue_Index val = (StringValue_Index)v;
                idx = new SIndex(val.Index, val.IsRel);
            }*/
            if (v.GetType() == typeof(StringValue_String))
            {
                StringValue_String val = (StringValue_String)v;
                idx = new SIndex(val.Text);
            }
            else { ConsoleLog.LogError("IndexCornerNoLimit() : Type"); }
            index++;

            return idx.Full(BodyParser.IndexInfo.Offset_Corner) - BodyParser.IndexInfo.Absolut_Offset;
        }
        public uint IndexCorner()
        {
            uint idx = IndexCornerNoLimit();
            if (idx >= BodyParser.IndexInfo.Length_Corner)
            {
                throw new ECornerIndex(idx, BodyParser.IndexInfo.Length_Corner, BodyParser.IndexInfo.Offset_Corner);
            }
            return idx;
        }
        public uint[] IndexCorner(int num)
        {
            uint[] indexe = new uint[num];
            for (int i = 0; i < num; i++)
            {
                indexe[i] = IndexCorner();
            }
            return indexe;
        }

        public uint IndexFaceNoLimit()
        {
            //SIndex idx = new SIndex(values[index]);

            StringValue v = values[index];
            SIndex idx = new SIndex();

            if (v.GetType() == typeof(StringValue_String))
            {
                StringValue_String val = (StringValue_String)v;
                idx = new SIndex(val.Text);
            }
            else { ConsoleLog.LogError("IndexFaceNoLimit() : Type"); }
            index++;

            return idx.Full(BodyParser.IndexInfo.Offset_Face);
        }
        public uint IndexFace()
        {
            uint idx = IndexFaceNoLimit();
            if (idx >= BodyParser.IndexInfo.Length_Face)
            {
                throw new EFaceIndex(idx, BodyParser.IndexInfo.Length_Face, BodyParser.IndexInfo.Offset_Face);
            }
            return idx;
        }
        public uint[] IndexFace(int num)
        {
            uint[] indexe = new uint[num];
            for (int i = 0; i < num; i++)
            {
                indexe[i] = IndexFace();
            }
            return indexe;
        }

        public uint Color()
        {
            uint r, g, b;
            //r = uint.Parse(values[index + 0]);
            //g = uint.Parse(values[index + 1]);
            //b = uint.Parse(values[index + 2]);
            //index += 3;

            StringValue v;

            v = values[index];
            if (v.GetType() == typeof(StringValue_String))
            {
                StringValue_String val = (StringValue_String)v;
                r = uint.Parse(val.Text);
            }
            else { ConsoleLog.LogError("Color() r : Type"); r = 0; }
            index++;

            v = values[index];
            if (v.GetType() == typeof(StringValue_String))
            {
                StringValue_String val = (StringValue_String)v;
                g = uint.Parse(val.Text);
            }
            else { ConsoleLog.LogError("Color() b : Type"); g = 0; }
            index++;

            v = values[index];
            if (v.GetType() == typeof(StringValue_String))
            {
                StringValue_String val = (StringValue_String)v;
                b = uint.Parse(val.Text);
            }
            else { ConsoleLog.LogError("Color() b : Type"); b = 0; }
            index++;

            return ((r << 16) | (g << 8) | (b << 0));
        }
    }
}
