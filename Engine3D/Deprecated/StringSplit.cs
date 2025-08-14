using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine3D
{
    [Obsolete("newer Version in StringInter", false)]
    public static class StringSplit
    {
        public interface ISplit
        {
            public void Update(char c);

            public bool isSkip();
            public bool isSplit();

            public bool isActive();

            public readonly static ISplit Comma = new SplitSolo(',');
            public readonly static ISplit Colon = new SplitSolo(':');
            public readonly static ISplit QuoteSingle = new SplitTwin('\'');
            public readonly static ISplit QuoteDouble = new SplitTwin('\"');
            public readonly static ISplit BracketRound = new SplitPair('(', ')');
            public readonly static ISplit BracketSquare = new SplitPair('[', ']');
            public readonly static ISplit BracketSquiggle = new SplitPair('{', '}');
        }
        public struct SplitSolo : ISplit
        {
            private char Character;

            private bool splitHere;

            public SplitSolo(char c)
            {
                Character = c;

                splitHere = false;
            }

            public void Update(char c)
            {
                splitHere = (Character == c);
            }

            public bool isSkip()
            {
                return splitHere;
            }
            public bool isSplit()
            {
                return splitHere;
            }

            public bool isActive()
            {
                return true;
            }
        }
        public struct SplitTwin : ISplit
        {
            private char Character;

            private bool Active;

            private bool skipHere;
            private bool splitHere;

            public SplitTwin(char c)
            {
                Character = c;

                Active = false;

                skipHere = false;
                splitHere = false;
            }

            public void Update(char c)
            {
                skipHere = false;
                splitHere = false;

                if (Character == c)
                {
                    skipHere = true;
                    if (!Active)
                    {
                        Active = true;
                    }
                    else
                    {
                        splitHere = true;
                        Active = false;
                    }
                }
            }

            public bool isSkip()
            {
                return skipHere;
            }
            public bool isSplit()
            {
                return splitHere;
            }

            public bool isActive()
            {
                return Active;
            }
        }
        public struct SplitPair : ISplit
        {
            private char CharS;
            private char CharE;

            private bool Active;

            private bool skipHere;
            private bool splitHere;

            public SplitPair(char s, char e)
            {
                CharS = s;
                CharE = e;

                Active = false;

                skipHere = false;
                splitHere = false;
            }

            public void Update(char c)
            {
                skipHere = false;
                splitHere = false;

                if (CharS == c)
                {
                    Active = true;
                    skipHere = true;
                }

                if (CharE == c)
                {
                    Active = false;
                    skipHere = true;
                    splitHere = true;
                }
            }

            public bool isSkip()
            {
                return skipHere;
            }
            public bool isSplit()
            {
                return splitHere;
            }

            public bool isActive()
            {
                return Active;
            }
        }
        public struct SplitArray : ISplit
        {
            private ISplit[] Array;
            private int ActiveIndex;

            private bool skipHere;
            private bool splitHere;

            public SplitArray(ISplit[] arr)
            {
                Array = arr;
                ActiveIndex = -1;

                skipHere = false;
                splitHere = false;
            }

            public void Update(char c)
            {
                skipHere = false;
                splitHere = false;

                if (ActiveIndex == -1)
                {
                    for (int i = 0; i < Array.Length; i++)
                    {
                        Array[i].Update(c);
                        if (Array[i].isActive())
                        {
                            ActiveIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    Array[ActiveIndex].Update(c);
                }

                if (ActiveIndex != -1)
                {
                    skipHere = Array[ActiveIndex].isSkip();
                    splitHere = Array[ActiveIndex].isSplit();
                    if (!Array[ActiveIndex].isActive())
                        ActiveIndex = -1;
                }
            }

            public bool isSkip()
            {
                return skipHere;
            }
            public bool isSplit()
            {
                return splitHere;
            }

            public bool isActive()
            {
                if (ActiveIndex != -1)
                    return Array[ActiveIndex].isActive();
                return false;
            }
        }

        public static string[] Split(string str, ISplit split)
        {
            int off = 0;
            int idx = 0;
            List<string> segments = new List<string>();

            while (idx < str.Length)
            {
                split.Update(str[idx]);

                if (split.isSplit() && idx > off)
                    segments.Add(str.Substring(off, idx - off));
                if (split.isSkip())
                    off = idx + 1;

                idx++;
            }
            if (split.isActive() && idx > off)
                segments.Add(str.Substring(off, idx - off));

            return segments.ToArray();
        }
        public static string[] Split(string strSplit2, ISplit split, ISplit ignore)
        {
            int off = 0;
            int idx = 0;
            List<string> segments = new List<string>();

            while (idx < strSplit2.Length)
            {
                ignore.Update(strSplit2[idx]);

                if (!ignore.isActive())
                {
                    split.Update(strSplit2[idx]);

                    if (split.isSplit() && idx > off)
                        segments.Add(strSplit2.Substring(off, idx - off));
                    if (split.isSkip())
                        off = idx + 1;
                }

                idx++;
            }
            if (split.isActive() && idx > off)
                segments.Add(strSplit2.Substring(off, idx - off));

            return segments.ToArray();
        }
    }
}
