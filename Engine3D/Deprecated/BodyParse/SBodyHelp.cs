
using Engine3D.Abstract3D;
using Engine3D.StringParse;

namespace Engine3D.BodyParse
{
    struct SIndex
    {
        private int Index;
        private bool IsRel;

        public SIndex(string val)
        {
            if (val[0] == '!' || val[0] == '.')
            {
                IsRel = (val[0] == '.');
                Index = TNamedMemory.Context.ParseI(val.Substring(1));
            }
            else
            {
                Index = int.Parse(val);
                IsRel = (val[0] == '+' || val[0] == '-');
            }
        }
        public SIndex(int index, bool isRel)
        {
            Index = index;
            IsRel = isRel;
        }

        public uint Full(uint offset)
        {
            if (IsRel)
            {
                return (uint)(Index + offset);
            }
            else
            {
                return (uint)Index;
            }
        }
    }
    /* Index Variable ?
     * ?   Absolut vs Relaiv
     * ?   when Adding/Subtracting is result Abs/Rel
     * ?   .! arent really relevent for Indexes
     * !   . is Absolut
     * !   ! is Relativ
     * !   Sign at start when Numbers or Math
     * 
     * remember index ? no for now
     * do math with index:
     *     . 231 + 43
     *     ! 231 - 43
     * 
     * Problem: how to return weather the Index is Rel or Abs
     * already extract that part before passing to math ?
     */



    public struct SIndexInfo
    {
        public uint Length_Corner;
        public uint Length_Face;
        public uint Offset_Corner;
        public uint Offset_Face;
        public uint Absolut_Offset;

        public string ToLines()
        {
            string str = "";
            str += Length_Corner.ToString() + " - ";
            str += Offset_Corner.ToString() + " = ";
            str += (Length_Corner - Offset_Corner).ToString() + '\n';
            str += Length_Face.ToString() + " - ";
            str += Offset_Face.ToString() + " = ";
            str += (Length_Face - Offset_Face).ToString() + '\n';
            return str;
        }
    }
    struct SAxis
    {
        public byte IdxY;
        public byte IdxX;
        public byte IdxC;
        public sbyte DirY;
        public sbyte DirX;
        public sbyte DirC;

        public static void Change(string val, out byte idx, out sbyte dir)
        {
            idx = 3;
            dir = 0;
            if (val == "Y") { idx = 0; dir = +1; }
            if (val == "y") { idx = 0; dir = -1; }
            if (val == "X") { idx = 1; dir = +1; }
            if (val == "x") { idx = 1; dir = -1; }
            if (val == "C") { idx = 2; dir = +1; }
            if (val == "c") { idx = 2; dir = -1; }
        }
    }
}
