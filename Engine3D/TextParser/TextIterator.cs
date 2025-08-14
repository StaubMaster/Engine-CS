
using Engine3D.Miscellaneous.StringHelp;

namespace Engine3D.TextParser
{
    class TextIterator
    {
        private static char Comment0 = '#';
        private static char Comment1 = '\n';
        private static char CommentReplace = '\n';

        private static readonly IStringCheck WhiteSpace = new StringCheck_CharArray(" \t\n\r");
        private static char WhiteSpaceReplace = ' ';

        private static string ControlChars = ":,()[]{}<>;=+-!.*/";

        /*  Obsolete
        public static string TrimComments(string str)
        {
            bool isComment;
            int len;

            len = 0;
            isComment = false;
            for (int i = 0; i < str.Length; i++)
            {
                if (!isComment)
                {
                    if (str[i] == Comment0)
                    {
                        isComment = true;
                    }
                    len++;
                }
                else
                {
                    if (str[i] == Comment1)
                    {
                        isComment = false;
                    }
                }
            }

            char[] c = new char[len];

            len = 0;
            isComment = false;
            for (int i = 0; i < str.Length; i++)
            {
                if (!isComment)
                {
                    if (str[i] == Comment0)
                    {
                        isComment = true;
                        c[len] = CommentReplace;
                    }
                    else
                    {
                        c[len] = str[i];
                    }
                    len++;
                }
                else
                {
                    if (str[i] == Comment1)
                    {
                        isComment = false;
                    }
                }
            }

            return new string(c);
        }
        public static string TrimWhiteSpace(string str)
        {
            int white0 = StringParse.THelp.FindNotPallet(str, WhiteSpaceChars);
            int white1 = StringParse.THelp.FindLastNotPallet(str, WhiteSpaceChars);

            int len;
            bool inWhiteSpace;

            len = 0;
            inWhiteSpace = false;

            for (int i = white0; i <= white1; i++)
            {
                if (StringParse.THelp.CharIsAnyOf(str[i], WhiteSpaceChars))
                {
                    if (!inWhiteSpace)
                    {
                        inWhiteSpace = true;
                        len++;
                    }
                }
                else
                {
                    if (inWhiteSpace)
                    {
                        inWhiteSpace = false;
                    }
                    len++;
                }
            }

            char[] c = new char[len];
            len = 0;
            inWhiteSpace = false;

            for (int i = white0; i <= white1; i++)
            {
                if (StringParse.THelp.CharIsAnyOf(str[i], WhiteSpaceChars))
                {
                    if (!inWhiteSpace)
                    {
                        inWhiteSpace = true;
                        c[len] = WhiteSpaceReplace;
                        len++;
                    }
                }
                else
                {
                    if (inWhiteSpace)
                    {
                        inWhiteSpace = false;
                    }
                    c[len] = str[i];
                    len++;
                }
            }

            return new string(c);
        }
        public static string TrimControls(string str)
        {
            byte[] ctrlType = new byte[str.Length];
            byte ctrlTypeNone = 0;
            byte ctrlTypeControl = 1;
            byte ctrlTypeSpace = 2;

            for (int i = 0; i < str.Length; i++)
            {
                if (StringParse.THelp.CharIsAnyOf(str[i], ControlChars))
                {
                    ctrlType[i] = ctrlTypeControl;
                }
                else if (str[i] == WhiteSpaceReplace)
                {
                    ctrlType[i] = ctrlTypeSpace;
                }
                else
                {
                    ctrlType[i] = ctrlTypeNone;
                }
            }

            int len;

            len = 0;
            len++;
            for (int i = 1; i < str.Length - 1; i++)
            {
                if (ctrlType[i] == ctrlTypeSpace)
                {
                    if (ctrlType[i - 1] != ctrlTypeControl && ctrlType[i + 1] != ctrlTypeControl)
                    {
                        len++;
                    }
                }
                else
                {
                    len++;
                }
            }
            len++;

            char[] c = new char[len];

            len = 0;
            c[len] = str[0];
            len++;
            for (int i = 1; i < str.Length - 1; i++)
            {
                if (ctrlType[i] == ctrlTypeSpace)
                {
                    if (ctrlType[i - 1] != ctrlTypeControl && ctrlType[i + 1] != ctrlTypeControl)
                    {
                        c[len] = str[i];
                        len++;
                    }
                }
                else
                {
                    c[len] = str[i];
                    len++;
                }
            }
            c[len] = str[str.Length - 1];

            return new string(c);
        }
        */

        public static string Printify(string str)
        {
            string txt = "";
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (c == '\0') { txt += "\\0"; }
                else if (c == '\t') { txt += "\\t"; }
                else if (c == '\n') { txt += "\\n"; }
                else if (c == '\r') { txt += "\\r"; }
                else { txt += c; }
                //if (c != '\n' && c != '\r') { txt += c; }
            }
            return txt;
        }



        private readonly string Text;
        private int Index;

        private bool WhiteSpaceIs;
        private int WhiteSpaceIndex0;
        private int WhiteSpaceIndex1;

        public char CurrentChar
        {
            get
            {
                if (WhiteSpaceIs) { return WhiteSpaceReplace; }
                return Text[Index];
            }
        }

        public TextIterator(string text)
        {
            Text = text;
        }

        public int Index0()
        {
            if (WhiteSpaceIs) { return WhiteSpaceIndex0; }
            return Index;
        }
        public int Index1()
        {
            if (WhiteSpaceIs) { return WhiteSpaceIndex1; }
            return Index;
        }

        public int FindWhiteSpaceIndex0(int idx)
        {
            return Text.FindPrev(new StringCheck_Not(WhiteSpace), idx);
        }
        public int FindWhiteSpaceIndex1(int idx)
        {
            return Text.FindNext(new StringCheck_Not(WhiteSpace), idx);
        }

        private bool SkipComment()
        {
            if (Text[Index] == Comment0)
            {
                if (!WhiteSpaceIs)
                {
                    WhiteSpaceIs = true;
                    WhiteSpaceIndex0 = Index;
                }
                while (LoopIsLimit() && Text[Index] != Comment1)
                {
                    WhiteSpaceIndex1 = Index;
                    Index++;
                }
                return true;
            }
            return false;
        }
        private bool SkipWhiteSpace()
        {
            if (WhiteSpace.Check(Text[Index]))
            {
                if (!WhiteSpaceIs)
                {
                    WhiteSpaceIs = true;
                    WhiteSpaceIndex0 = Index;
                }
                while (LoopIsLimit() && WhiteSpace.Check(Text[Index]))
                {
                    WhiteSpaceIndex1 = Index;
                    Index++;
                }
                return true;
            }
            return false;
        }

        public void LoopInit()
        {
            Index = 0;
            WhiteSpaceIs = false;
        }
        public void LoopContinue()
        {
            if (WhiteSpaceIs)
            {
                WhiteSpaceIs = false;
            }
            else
            {
                Index++;
                while (LoopIsLimit() && (SkipComment() || SkipWhiteSpace())) ;
            }
        }
        public bool LoopIsLimit()
        {
            return (Index >= 0 && Index < Text.Length);
        }

        public int Limit0()
        {
            return 0;
        }
        public int Limit1()
        {
            return Text.Length - 1;
        }

        public bool CutIsValid(int index0, int index1)
        {
            return Text.CutIsValid(index0, index1);
        }
        public string Cut(int index0, int index1)
        {
            return Text.Cut(index0, index1);
        }

        public string CurrentCharToString(string name, string tab = "")
        {
            string str = "";

            string format = "";
            if (Text.Length < 1) { format = ""; }
            else if (Text.Length < 10) { format = "0"; }
            else if (Text.Length < 100) { format = "00"; }
            else if (Text.Length < 1000) { format = "000"; }
            else if (Text.Length < 10000) { format = "0000"; }

            str += "[" + Index.ToString(format) + "]";

            str += tab + name;

            str += "'" + CurrentChar.ToString() + "'";

            return str;
        }
    }
}
