using System;
using System.Collections.Generic;
using System.IO;

namespace Engine3D.StringParse
{
    [Obsolete("newer Version in StringInter", false)]
    public static class TSplit
    {
        /*
            this class is to split a string into parts
            the interpreter is to specify how to parse the string
        */

        public abstract class ASplitAB
        {
            //  base for splits
            public int A;
            public int B;

            public virtual void ColorBefor(int i)
            {

            }
            public virtual void ColorAfter(int i)
            {

            }

            public override string ToString()
            {
                string str = "";

                str += "(";

                if (A != -1)
                    str += A.ToString("000");
                else
                    str += "---";

                str += "|";

                if (B != -1)
                    str += B.ToString("000");
                else
                    str += "---";

                str += ")";

                return str;
            }
        }
        public class CSplitNone : ASplitAB
        {
            public CSplitNone()
            {
                A = -1;
                B = -1;
            }
        }
        public class CSplitSolo : ASplitAB
        {
            //  split string enery time a certain char is found, also seperated by Twin and Pair, e.g.:
            //  1"23"4:56{78}9 gives 1 4 56 9
            public CSplitSolo()
            {
                A = -1;
                B = -1;
            }

            public override void ColorBefor(int i)
            {
                if (i == A) { ConsoleLog.ColorFore(0xCFCFFF); }
            }
            public override void ColorAfter(int i)
            {
                if (i == B) { ConsoleLog.ColorFore(0x7F7F7F); }
            }

            public override string ToString()
            {
                return "Solo " + base.ToString();
            }
        }
        public class CSplitTwin : ASplitAB
        {
            //  split string between 2 identical chars e.g.:
            //  1"23"4:56{78}9 gives 23
            public CSplitTwin()
            {
                A = -1;
                B = -1;
            }

            public override void ColorBefor(int i)
            {
                if (i == A) { ConsoleLog.ColorFore(0xFF7F00); }
            }
            public override void ColorAfter(int i)
            {
                if (i == B) { ConsoleLog.ColorFore(0xFF7F00); }
            }

            public override string ToString()
            {
                return "Twin " + base.ToString();
            }
        }
        public class CSplitPair : ASplitAB
        {
            //  split string between 2 unique chars e.g.:
            //  1"23"4:56{78}9 gives 78
            public CSplitPair()
            {
                A = -1;
                B = -1;
            }

            public override void ColorBefor(int i)
            {
                if (i == A) { ConsoleLog.ColorFore(0xCFCF00); }
                if (i == B) { ConsoleLog.ColorFore(0xCFCF00); }
            }
            public override void ColorAfter(int i)
            {
                if (i == A) { ConsoleLog.ColorFore(0x7F7F7F); }
                if (i == B) { ConsoleLog.ColorFore(0x7F7F7F); }
            }

            public override string ToString()
            {
                return "Pair " + base.ToString();
            }
        }
        public class CSplitCmnt : ASplitAB
        {
            //  comments, split string from char to end of string
            public CSplitCmnt()
            {
                A = -1;
                B = -1;
            }

            public override void ColorBefor(int i)
            {
                if (i == A) { ConsoleLog.ColorFore(0x3FFF3F); }
            }
            public override void ColorAfter(int i)
            {
                if (i == B) { ConsoleLog.ColorFore(0x7F7F7F); }
            }

            public override string ToString()
            {
                return "Cmnt " + base.ToString();
            }
        }



        public struct SSettings
        {
            public (char, char) CommentChar;
            public char[] WhiteSpaceChar;
            public char[] SoloChar;
            public char[] TwinChar;
            public (char, char)[] PairChar;

            public SSettings((char, char) comment, char[] white_space, char[] solo, char[] twin, (char, char)[] pair)
            {
                CommentChar = comment;
                WhiteSpaceChar = white_space;
                SoloChar = solo;
                TwinChar = twin;
                PairChar = pair;
            }

            public static SSettings Default()
            {
                return new SSettings(
                    ('!', '\n'),
                    new char[] { ' ', '\t', '\r', '\n' },
                    new char[] { ':' },
                    new char[] { '\"' },
                    new (char, char)[] { ('{', '}') }
                    );
            }

            private int isWhiteSpace(char c)
            {
                for (int i = 0; i < WhiteSpaceChar.Length; i++)
                {
                    if (c == WhiteSpaceChar[i])
                    {
                        return i;
                    }
                }
                return -1;
            }
            private int isSolo(char c)
            {
                for (int i = 0; i < SoloChar.Length; i++)
                {
                    if (c == SoloChar[i])
                    {
                        return i;
                    }
                }
                return -1;
            }
            private int isTwin(char c)
            {
                for (int i = 0; i < TwinChar.Length; i++)
                {
                    if (c == TwinChar[i])
                    {
                        return i;
                    }
                }
                return -1;
            }
            private int isPairA(char c)
            {
                for (int i = 0; i < PairChar.Length; i++)
                {
                    if (c == PairChar[i].Item1)
                    {
                        return i;
                    }
                }
                return -1;
            }
            private int isPairB(char c)
            {
                for (int i = 0; i < PairChar.Length; i++)
                {
                    if (c == PairChar[i].Item2)
                    {
                        return i;
                    }
                }
                return -1;
            }

            public SData Parse(string text)
            {
                SAuxiliary aux = new SAuxiliary(text);

                aux.SoloCurrent.A = 0;
                for (aux.Index = 0; aux.Index < aux.Length; aux.Index++)
                {
                    int j;
                    aux.Character = aux.Text[aux.Index];

                    j = isWhiteSpace(aux.Character);
                    if (j != -1)
                    {
                        aux.AddSolo();
                        continue;
                    }

                    if (aux.Character == CommentChar.Item1)
                    {
                        aux.AddSolo();
                        aux.AddComm(CommentChar.Item2);
                        aux.SoloCurrent.A = aux.Index + 1;
                    }

                    j = isTwin(aux.Character);
                    if (j != -1)
                    {
                        aux.AddSolo();
                        aux.AddTwin(TwinChar[j]);
                        aux.SoloCurrent.A = aux.Index + 1;
                    }

                    j = isPairA(aux.Character);
                    if (j != -1)
                    {
                        aux.AddSolo();
                        aux.AddPairA();
                    }

                    j = isPairB(aux.Character);
                    if (j != -1)
                    {
                        aux.AddSolo();
                        aux.AddPairB();
                    }

                    j = isSolo(aux.Character);
                    if (j != -1)
                    {
                        aux.AddSolo();
                    }
                }

                aux.AddSolo();
                while (aux.PairStack.Count != 0)
                {
                    CSplitPair pair;
                    pair = aux.PairStack.Pop();
                    aux.SplitList.Add(pair);
                }

                return new SData(text, aux.SplitList.ToArray());
            }
        }
        public struct SAuxiliary
        {
            public string Text;
            public int Length;
            public int Index;
            public char Character;

            public CSplitSolo SoloCurrent;
            public Stack<CSplitPair> PairStack;

            public List<ASplitAB> SplitList;

            public SAuxiliary(string text)
            {
                Text = text;
                Length = text.Length;
                Index = -1;
                Character = '\0';

                SoloCurrent = new CSplitSolo();
                PairStack = new Stack<CSplitPair>();

                SplitList = new List<ASplitAB>();
            }

            public void Skip(char c)
            {
                while (Index < Length)
                {
                    if (Text[Index] == c)
                        break;
                    Index++;
                }

                if (Index == Length)
                {
                    Index--;
                }
            }

            public void AddComm(char c)
            {
                CSplitCmnt comm = new CSplitCmnt();
                comm.A = Index;
                Skip(c);
                comm.B = Index;
                SplitList.Add(comm);
            }
            public void AddSolo()
            {
                SoloCurrent.B = Index - 1;
                if (SoloCurrent.A <= SoloCurrent.B)
                {
                    SplitList.Add(SoloCurrent);
                }
                SoloCurrent = new CSplitSolo();
                SoloCurrent.A = Index + 1;
            }
            public void AddTwin(char c)
            {
                CSplitTwin twin;
                twin = new CSplitTwin();
                twin.A = Index;
                Index++;
                Skip(c);
                twin.B = Index;
                SplitList.Add(twin);
            }
            public void AddPairA()
            {
                CSplitPair pair;
                pair = new CSplitPair();
                pair.A = Index;
                PairStack.Push(pair);
            }
            public void AddPairB()
            {
                CSplitPair pair;
                if (PairStack.Count != 0)
                {
                    pair = PairStack.Pop();
                }
                else
                {
                    pair = new CSplitPair();
                }
                pair.B = Index;
                SplitList.Add(pair);
            }
        }
        public struct SData
        {
            public string Text;
            public ASplitAB[] Splits;

            public SData(string text, ASplitAB[] splits)
            {
                Text = text;
                Splits = splits;
            }

            public void ToConsoleColor()
            {
                for (int i = 0; i < Text.Length; i++)
                {
                    for (int j = 0; j < Splits.Length; j++)
                    {
                        Splits[j].ColorBefor(i);
                    }

                    ConsoleLog.Direct(new string(Text[i], 1));

                    for (int j = 0; j < Splits.Length; j++)
                    {
                        Splits[j].ColorAfter(i);
                    }
                }
                ConsoleLog.Direct("\n");
                ConsoleLog.ColorNone();
            }
            public void ToConsoleText()
            {
                for (int i = 0; i < Splits.Length; i++)
                {
                    //ConsoleLog.Log("[" + i.ToString("000") + "] " + Splits[i].ToString());
                    ConsoleLog.Direct("[" + i.ToString("000") + "] " + Splits[i].ToString());
                    ConsoleLog.Direct("|");
                    string cut;
                    cut = THelp.Cut(Text, Math.Max(Splits[i].A - 2, 0), Math.Min(Splits[i].A + 2, Text.Length));
                    for (int j = 0; j < cut.Length; j++)
                    {
                        if (cut[j] == '\t') { ConsoleLog.ColorFore(0x00FF00); ConsoleLog.Direct("t"); ConsoleLog.ColorNone(); }
                        else if (cut[j] == '\r') { ConsoleLog.ColorFore(0x00FF00); ConsoleLog.Direct("r"); ConsoleLog.ColorNone(); }
                        else if (cut[j] == '\n') { ConsoleLog.ColorFore(0x00FF00); ConsoleLog.Direct("n"); ConsoleLog.ColorNone(); }
                        else { ConsoleLog.Direct(new string(cut[j], 1)); }
                    }
                    ConsoleLog.Direct("|");
                    cut = THelp.Cut(Text, Math.Max(Splits[i].B - 2, 0), Math.Min(Splits[i].B + 2, Text.Length));
                    for (int j = 0; j < cut.Length; j++)
                    {
                        if (cut[j] == '\t')      { ConsoleLog.ColorFore(0x00FF00); ConsoleLog.Direct("t"); ConsoleLog.ColorNone(); }
                        else if (cut[j] == '\r') { ConsoleLog.ColorFore(0x00FF00); ConsoleLog.Direct("r"); ConsoleLog.ColorNone(); }
                        else if (cut[j] == '\n') { ConsoleLog.ColorFore(0x00FF00); ConsoleLog.Direct("n"); ConsoleLog.ColorNone(); }
                        else { ConsoleLog.Direct(new string(cut[j], 1)); }
                    }
                    ConsoleLog.Direct("|\n");
                }
            }

            public CDataHierarchyChildren ToHierarchy()
            {
                CDataHierarchyChildren Main = new CDataHierarchyChildren(null);
                CDataHierarchyChildren hir = Main;
                for (int i = 0; i < Text.Length; i++)
                {
                    for (int j = 0; j < Splits.Length; j++)
                    {
                        if (Splits[j].A == i)
                        {
                            if (Splits[j].GetType() == typeof(CSplitSolo))
                            {
                                hir.Child.Add(new CDataHierarchyString(THelp.Cut(Text, Splits[j].A, Splits[j].B)));
                                i = Splits[j].B;
                            }
                            if (Splits[j].GetType() == typeof(CSplitTwin))
                            {
                                hir.Child.Add(new CDataHierarchyString(THelp.Cut(Text, Splits[j].A + 1, Splits[j].B - 1)));
                                i = Splits[j].B;
                            }
                            if (Splits[j].GetType() == typeof(CSplitPair))
                            {
                                CDataHierarchyChildren h = new CDataHierarchyChildren(hir);
                                hir.Child.Add(h);
                                hir = h;
                            }
                        }
                        if (Splits[j].B == i)
                        {
                            if (Splits[j].GetType() == typeof(CSplitPair)) { hir = hir.Parent; }
                        }
                    }
                }
                return Main;
            }
        }
    }
}
