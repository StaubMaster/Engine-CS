using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Engine3D.Abstract3D;

namespace Engine3D
{
    [Obsolete("newer Version in StringInter", false)]
    public static class FileInterpret
    {
        /*
            Files are made of Entrys
            Entrys are made of Elements
            Elements are made of Strings
        */

        private static bool IsWhiteSpace(char c)
        {
            return (c == ' ' || c == '\t' || c == '\n' || c == '\r');
        }
        public static string ClearWhiteSpace(string str)
        {
            int idx;
            int len;
            bool QDouble;

            len = 0;
            idx = 0;
            QDouble = false;
            while (idx < str.Length)
            {
                if (QDouble || !IsWhiteSpace(str[idx]))
                    len++;
                if (str[idx] == '"')
                    QDouble = !QDouble;
                idx++;
            }

            char[] clear = new char[len];

            len = 0;
            idx = 0;
            QDouble = false;
            while (idx < str.Length)
            {
                if (QDouble || !IsWhiteSpace(str[idx]))
                {
                    clear[len] = str[idx];
                    len++;
                }
                if (str[idx] == '"')
                    QDouble = !QDouble;
                idx++;
            }

            return new string(clear);
        }

        private static bool IsRemove(char c, string rem)
        {
            for (int i = 0; i < rem.Length; i++)
            {
                if (rem[i] == c)
                    return true;
            }
            return false;
        }
        public static string Remove(string str, string rem, StringSplit.ISplit ignore)
        {
            int r = 0;
            for (int i = 0; i < str.Length; i++)
            {
                ignore.Update(str[i]);
                if (ignore.isActive() || ignore.isSkip() || !IsRemove(str[i], rem))
                {
                    r++;
                }
            }
            char[] chr = new char[r];

            r = 0;
            for (int i = 0; i < str.Length; i++)
            {
                ignore.Update(str[i]);
                if (ignore.isActive() || ignore.isSkip() || !IsRemove(str[i], rem))
                {
                    chr[r] = str[i];
                    r++;
                }
            }
            return new string(chr);
        }


        public struct Query
        {
            public readonly string Search;
            public readonly int E_Min;
            public readonly int E_Max;
            public readonly int I_Min;
            public readonly int I_Max;

            public string[][] Found;
            public int Num;

            public Query(string search, int Emin, int Emax, int Imin, int Imax)
            {
                Search = search;
                E_Min = Emin;
                E_Max = Emax;
                I_Min = Imin;
                I_Max = Imax;

                Found = null;
                Num = 0;
            }

            private bool Run(ElementStruct[] elements)
            {
                ElementStruct elmt;

                Num = 0;
                for (int e = 0; e < elements.Length; e++)
                {
                    elmt = elements[e];
                    if (elmt.Name == Search &&
                        I_Min <= elmt.Item.Length &&
                        elmt.Item.Length <= I_Max)
                    {
                        Num++;
                    }
                }
                Found = new string[Num][];

                Num = 0;
                for (int e = 0; e < elements.Length; e++)
                {
                    elmt = elements[e];
                    if (elmt.Name == Search &&
                        I_Min <= elmt.Item.Length &&
                        elmt.Item.Length <= I_Max)
                    {
                        Found[Num] = elements[e].Item;
                        Num++;
                    }
                }

                return (E_Min <= Num && Num <= E_Max);
            }
            public static bool RunQuerys(EntryStruct entry, Query[] querys)
            {
                int found = 0;
                for (int i = 0; i < querys.Length; i++)
                {
                    if (querys[i].Run(entry.Elements))
                    {
                        found++;
                    }
                }
                return (found == querys.Length);
            }


            public uint ToUInt(int idx1 = 0, int idx2 = 0)
            {
                return uint.Parse(Found[idx1][idx2]);
            }
            public uint ToColor(int idx1 = 0, int idx2 = 0)
            {
                return uint.Parse(Found[idx1][idx2].Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);
            }
            public Point3D ToPunkt(int idx = 0)
            {
                return new Point3D(
                    float.Parse(Found[idx][0]),
                    float.Parse(Found[idx][1]),
                    float.Parse(Found[idx][2])
                    );
            }
            public Point3D[] ToPunktArr()
            {
                Point3D[] arr = new Point3D[Found.Length];
                for (int a = 0; a < arr.Length; a++)
                    arr[a] = ToPunkt(a);
                return arr;
            }
        }


        public struct ElementStruct
        {
            public string Name;
            public string[] Item;

            public ElementStruct(string str)
            {
                string[] seg;
                seg = StringSplit.Split(str, StringSplit.ISplit.Colon, new StringSplit.SplitArray(new StringSplit.ISplit[] { StringSplit.ISplit.QuoteDouble, StringSplit.ISplit.BracketRound }));

                Name = seg[0];
                Item = new string[seg.Length - 1];
                for (int i = 0; i < Item.Length; i++)
                    Item[i] = Remove(seg[i + 1], "\"", StringSplit.ISplit.BracketRound);
            }

            public override string ToString()
            {
                string str = Name;
                for (int i = 0; i < Item.Length; i++)
                    str += " : " + "\"" + Item[i] + "\"";
                return str;
            }
        }
        public struct EntryStruct
        {
            public ElementStruct[] Elements;

            public EntryStruct(string str)
            {
                string[] seg;
                seg = StringSplit.Split(str, StringSplit.ISplit.Comma, new StringSplit.SplitArray(new StringSplit.ISplit[] { StringSplit.ISplit.QuoteDouble, StringSplit.ISplit.BracketRound }));

                Elements = new ElementStruct[seg.Length];
                for (int i = 0; i < seg.Length; i++)
                    Elements[i] = new ElementStruct(seg[i]);
            }

            /*
            public string[] Find(string name)
            {
                for (int i = 0; i < Elements.Length; i++)
                {
                    if (Elements[i].Name == name)
                        return Elements[i].Item;
                }
                return null;
            }
            public bool Find(string[] names, string[][] items)
            {
                int num = names.Length;
                int found = 0;

                for (int i = 0; i < num; i++)
                {
                    items[i] = Find(names[i]);
                    if (items[i] != null)
                        found++;
                }

                return (found == num);
            }

            public string[][] FindAll(string name)
            {
                List<string[]> found = new List<string[]>();

                for (int i = 0; i < Elements.Length; i++)
                {
                    if (Elements[i].Name == name)
                        found.Add(Elements[i].Item);
                }

                return found.ToArray();
            }
            public void FindAll(string[] names, string[][][] items)
            {
                for (int i = 0; i < names.Length; i++)
                {
                    items[i] = FindAll(names[i]);
                }
            }
            */

            public override string ToString()
            {
                string str = "{\n";

                for (int i = 0; i < Elements.Length; i++)
                {
                    str += "\t" + Elements[i].ToString() + ",\n";
                }

                str += "}";
                return str;
            }
        }
        public struct FileStruct
        {
            public EntryStruct[] Entrys;

            public FileStruct(string str)
            {
                string clear;
                clear = ClearWhiteSpace(str);

                string[] seg;
                seg = StringSplit.Split(clear, StringSplit.ISplit.BracketSquiggle);

                Entrys = new EntryStruct[seg.Length];
                for (int i = 0; i < Entrys.Length; i++)
                    Entrys[i] = new EntryStruct(seg[i]);
            }

            public override string ToString()
            {
                string str = "";

                for (int i = 0; i < Entrys.Length; i++)
                {
                    str += "\n" + Entrys[i].ToString() + "\n";
                }

                return str;
            }
        }
    }
}
