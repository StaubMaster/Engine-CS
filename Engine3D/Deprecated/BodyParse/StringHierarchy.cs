using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine3D.BodyParse
{
    [Obsolete("newer Version in StringInter", false)]
    class StringHierarchy
    {
        public StringHierarchy[] Child;
        public StringHierarchy Parent;
        public string Text;
        public string Seperator;

        public StringHierarchy()
        {
            Parent = null;
            Child = null;
            Text = "";
            Seperator = "";
        }
        public StringHierarchy(StringHierarchy parent, StringHierarchy[] child, string text, string seperator)
        {
            Parent = parent;
            Child = child;
            Text = text;
            Seperator = seperator;
        }

        public string ToLines(string tab = "")
        {
            string str = Seperator + Text;
            if (Child != null)
            {
                for (int i = 0; i < Child.Length; i++)
                {
                    str += Child[i].ToLines(tab + " ");
                }
            }
            return str;
        }

        public void Parse(int level, Func<string, string[]> split, Func<string, string> change, Func<string, bool> check, string seperator)
        {
            if (level == 0)
            {
                string[] lines = split(Text);
                Parse(level, lines, change, check, seperator);
            }
            else
            {
                if (Child != null)
                {
                    level--;
                    for (int i = 0; i < Child.Length; i++)
                    {
                        Child[i].Parse(level, split, change, check, seperator);
                    }
                }
            }
        }
        public void Parse(int Level, string[] lines, Func<string, string> Change, Func<string, bool> Check, string seperator)
        {
            if (Level == 0)
            {
                if (Child == null)
                {
                    List<StringHierarchy> list = new List<StringHierarchy>();
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string line = lines[i];
                        if (Change != null) { line = Change(line); }
                        if (Check == null || Check(line))
                        {
                            list.Add(new StringHierarchy(this, null, line, seperator));
                        }
                    }
                    Child = list.ToArray();
                }
            }
            else
            {
                if (Child != null)
                {
                    Level--;
                    for (int i = 0; i < Child.Length; i++)
                    {
                        Child[i].Parse(Level, lines, Change, Check, seperator);
                    }
                }
            }
        }
    }
}
