using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine3D.Miscellaneous.StringHelp
{
    public interface IStringCheck
    {
        public bool Check(char c);
    }



    public struct StringCheck_Not : IStringCheck
    {
        private readonly IStringCheck check;
        public StringCheck_Not(IStringCheck check)
        {
            this.check = check;
        }
        public bool Check(char c)
        {
            return !check.Check(c);
        }
    }
    public struct StringCheck_Only : IStringCheck
    {
        private readonly IStringCheck[] checks;
        public StringCheck_Only(IStringCheck[] checks)
        {
            this.checks = checks;
        }
        public bool Check(char c)
        {
            for (int i = 0; i < checks.Length; i++)
            {
                if (!checks[i].Check(c))
                {
                    return false;
                }
            }
            return true;
        }
    }
    public struct StringCheck_None : IStringCheck
    {
        private readonly IStringCheck[] checks;
        public StringCheck_None(IStringCheck[] checks)
        {
            this.checks = checks;
        }
        public bool Check(char c)
        {
            for (int i = 0; i < checks.Length; i++)
            {
                if (checks[i].Check(c))
                {
                    return false;
                }
            }
            return true;
        }
    }



    public struct StringCheck_CharArray : IStringCheck
    {
        private readonly char[] chars;
        public StringCheck_CharArray(char[] chars)
        {
            this.chars = chars;
        }
        public StringCheck_CharArray(string str)
        {
            chars = str.ToCharArray();
        }
        public bool Check(char c)
        {
            for (int i = 0; i < chars.Length; i++)
            {
                if (c == chars[i]) { return true; }
            }
            return false;
        }
    }
    public struct StringCheck_CharRange : IStringCheck
    {
        private readonly char min;
        private readonly char max;
        public StringCheck_CharRange(char min, char max)
        {
            this.min = min;
            this.max = max;
        }
        public bool Check(char c)
        {
            return (c >= min && c <= max);
        }
    }



    public static class StringHelp
    {
        public static readonly StringCheck_CharRange AlphaLo = new StringCheck_CharRange('a', 'z');
        public static readonly StringCheck_CharRange AlphaHi = new StringCheck_CharRange('A', 'Z');
        public static readonly StringCheck_CharRange Digit = new StringCheck_CharRange('0', '9');



        public static bool Check(this string str, IStringCheck check)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (!check.Check(str[i]))
                {
                    return false;
                }
            }
            return true;
        }
        public static int FindNext(this string str, IStringCheck check, int idx)
        {
            for (int i = idx; i < str.Length; i++)
            {
                if (check.Check(str[i]))
                {
                    return i;
                }
            }
            return -1;
        }
        public static int FindPrev(this string str, IStringCheck check, int idx)
        {
            for (int i = idx; i >= 0; i--)
            {
                if (check.Check(str[i]))
                {
                    return i;
                }
            }
            return -1;
        }


        public static int Index0(this string str)
        {
            return 0;
        }
        public static int Index1(this string str)
        {
            return str.Length - 1;
        }



        public static bool InRange(this string str, int idx)
        {
            return (idx >= 0 && idx < str.Length);
        }

        public static bool CutIsValid(this string str, int idx0, int idx1)
        {
            return (idx0 <= idx1 && str.InRange(idx0) && str.InRange(idx1));
        }
        public static string Cut(this string str, int idx0, int idx1)
        {
            char[] c = new char[(idx1 - idx0) + 1];
            for (int i = idx0, o = 0; i <= idx1; i++, o++)
            {
                c[o] = str[i];
            }
            return new string(c);
        }
    }
}
