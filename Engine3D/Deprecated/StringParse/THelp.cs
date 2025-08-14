using System;
using System.Collections.Generic;
using System.IO;

namespace Engine3D.StringParse
{
    [Obsolete("sometimes used but should be reworked", false)]
    public static class THelp
    {
        public static bool StringIndexInrange(string text, int idx)
        {
            return (idx >= 0 && idx < text.Length);
        }
        public static bool CutIsValid(string text, int i0, int i1)
        {
            return (i0 <= i1 && StringIndexInrange(text, i0) && StringIndexInrange(text, i1));
        }
        private static string CutMy(string text, int i0, int i1)
        {
            char[] c = new char[(i1 - i0) + 1];
            for (int i = i0, o = 0; i <= i1; i++, o++)
            {
                c[o] = text[i];
            }
            return new string(c);
        }
        public static string Cut(string text, int i0, int i1)
        {
            if (!CutIsValid(text, i0, i1)) { return "<!>"; }
            //return text.Substring(i0, i1 + 1 - i0);
            return CutMy(text, i0, i1);
        }

        private static int Find(char c, string text, int idx)
        {
            while (idx < text.Length)
            {
                if (c == text[idx])
                    return idx;
                idx++;
            }
            return text.Length;
        }
        private static int FindNot(char c, string text, int idx)
        {
            while (idx < text.Length)
            {
                if (c != text[idx])
                    return idx;
                idx++;
            }
            return text.Length;
        }

        private static bool Compare(string text, int idx, string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (idx + i >= text.Length)
                    return false;
                if (text[idx + i] != s[i])
                    return false;
            }
            return true;
        }
        private static int Find(string s, string text, int idx)
        {
            while (idx < text.Length)
            {
                if (Compare(text, idx, s))
                {
                    return idx;
                }
                idx++;
            }
            return text.Length;
        }
        private static int FindNot(string s, string text, int idx)
        {
            while (idx < text.Length)
            {
                if (!Compare(text, idx, s))
                {
                    return idx;
                }
                idx += s.Length;
            }
            return text.Length;
        }

        public static int FindNotPallet(string str, string pallet)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (!CharIsAnyOf(str[i], pallet))
                {
                    return i;
                }
            }
            return -1;
        }
        public static int FindLastNotPallet(string str, string pallet)
        {
            for (int i = str.Length - 1; i >= 0; i--)
            {
                if (!CharIsAnyOf(str[i], pallet))
                {
                    return i;
                }
            }
            return -1;
        }

        public static bool CharIsAnyOf(char c, string pallet)
        {
            for (int i = 0; i < pallet.Length; i++)
            {
                if (c == pallet[i])
                {
                    return true;
                }
            }
            return false;
        }
        public static bool StringIsOnly(string str, string pallet)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (!CharIsAnyOf(str[i], pallet))
                {
                    return false;
                }
            }
            return true;
        }

        private static int CharCount(string str, char pallet)
        {
            int n = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == pallet) { n++; }
            }
            return n;
        }
        private static int StringCount(string str, string pallet)
        {
            int n = 0;
            for (int i = 0; i < pallet.Length; i++)
            {
                n += CharCount(str, pallet[i]);
            }
            return n;
        }
        private static int FindPallet(string str, string pallet)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (CharIsAnyOf(str[i], pallet)) { return i; }
            }
            return -1;
        }

        public static bool IsAlphaHi(char c)
        {
            return (c >= 'A' && c <= 'Z');
        }
        public static bool IsAlphaLo(char c)
        {
            return (c >= 'a' && c <= 'z');
        }
        public static bool IsDigit(char c)
        {
            return (c >= '0' && c <= '9');
        }

        /* Check Sign
         * only first char can be sign
         * if no sign: len >= 1
         * if sign: len > 1
         */
        /* only 0 or 1 seperators
         * cannot be first
         * cannot be last
         */
        public static bool StringIsNumber(string str, string signs, string digits)
        {
            if (str.Length == 0) { return false; }

            if (StringCount(str, signs) > 1) { return false; }
            int SignI = FindPallet(str, signs);
            if (SignI > 0) { return false; }

            for (int i = 0; i < str.Length; i++)
            {
                if (i != SignI)
                {
                    if (!CharIsAnyOf(str[i], digits)) { return false; }
                }
            }
            return true;
        }
        public static bool StringIsNumber(string str, string signs, string digits, string seps)
        {
            if (str.Length == 0) { return false; }

            if (StringCount(str, signs) > 1) { return false; }
            int SignI = FindPallet(str, signs);
            if (SignI > 0) { return false; }

            if (StringCount(str, seps) > 1) { return false; }
            int SepI = FindPallet(str, seps);
            if (SepI == str.Length - 1) { return false; }
            if (SignI + 1 == SepI) { return false; }

            if ((SignI == -1 && SepI == -1) && str.Length == 0) { return false; }
            if ((SignI != -1 || SepI != -1) && str.Length == 1) { return false; }
            if ((SignI != -1 && SepI != -1) && str.Length == 2) { return false; }
            for (int i = 0; i < str.Length; i++)
            {
                if (i != SignI && i != SepI)
                {
                    if (!CharIsAnyOf(str[i], digits)) { return false; }
                }
            }
            return true;
        }
    }
}
