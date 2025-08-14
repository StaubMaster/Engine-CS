
using Engine3D.Miscellaneous;

namespace Engine3D.TextParser.Sectonizer
{
    class Section
    {
        //  put Seperator into its own File
        //  put some more stuff into Seperator

        /*
            is including Controls needed ?
            WhiteSpace when splitting:
                WhiteSpace then Control
                split last section at beginning of WhiteSpace
                then Skip next WhiteSpace and split new section
         */

        /*  Remember how this was Split ? */

        private readonly TextIterator Context;
        private readonly Section Meta;

        public readonly ArrayList<Section> Sections;
        public Section CurrentSection;

        private Seperator Seperator0;
        private Seperator Seperator1;

        public int Count()
        {
            return Sections.Count;
        }

        public Section(TextIterator context, Section meta)
        {
            Context = context;
            Meta = meta;

            Seperator0 = null;
            Seperator1 = null;

            Sections = new ArrayList<Section>();
            Sections.EditBegin();

            CurrentSection = null;
        }

        private void Index0(bool wantControl, out int idx, out int level, out bool limit)
        {
            if (Seperator0 != null)
            {
                idx = Seperator0.Index0(wantControl);
                idx = Context.FindWhiteSpaceIndex1(idx);

                level = 0;
                limit = false;
            }
            else if (Meta != null)
            {
                Meta.Index0(wantControl, out idx, out level, out limit);
                level++;
            }
            else
            {
                idx = Context.Limit0();
                level = 0;
                limit = true;
            }
        }
        private void Index1(bool wantControl, out int idx, out int level, out bool limit)
        {
            if (Seperator1 != null)
            {
                idx = Seperator1.Index1(wantControl);
                idx = Context.FindWhiteSpaceIndex0(idx);

                level = 0;
                limit = false;
            }
            else if (Meta != null)
            {
                Meta.Index1(wantControl, out idx, out level, out limit);
                level++;
            }
            else
            {
                idx = Context.Limit1();
                level = 0;
                limit = true;
            }
        }

        public bool isValid(bool wantControl)
        {
            Index0(wantControl, out int idx0, out _, out _);
            Index1(wantControl, out int idx1, out _, out _);
            return Context.CutIsValid(idx0, idx1);
        }
        public string Cut(bool wantControl, out string head)
        {
            head = "";

            Index0(wantControl, out int idx0, out int lvl0, out bool limit0);
            Index1(wantControl, out int idx1, out int lvl1, out bool limit1);

            head += "[";
            if (limit0) { head += "!!!"; } else { head += idx0.ToString("000"); }
            head += ":" + lvl0.ToString("0");
            head += "|";
            if (limit1) { head += "!!!"; } else { head += idx1.ToString("000"); }
            head += ":" + lvl1.ToString("0");
            head += "]";

            return Context.Cut(idx0, idx1);
        }
        public string Cut()
        {
            Index0(false, out int idx0, out _, out _);
            Index1(false, out int idx1, out _, out _);
            return Context.Cut(idx0, idx1);
        }

        public void Insert(Section section, bool wantControl)
        {
            if (section != null && section.isValid(wantControl))
            {
                section.Insert(section.CurrentSection, wantControl);
                section.CurrentSection = null;
                section.Sections.EditEnd();
                Sections.Insert(section);
            }
        }
        public void InsertSub(bool wantControl, InEx seperator1, InEx seperator0)
        {
            if (CurrentSection != null)
            {
                CurrentSection.Cut1(seperator1);
                Insert(CurrentSection, wantControl);
            }

            CurrentSection = null;

            if (seperator0 != InEx.None)
            {
                CurrentSection = new Section(Context, this);
                CurrentSection.Cut0(seperator0);
            }
        }

        public void Cut0(InEx type)
        {
            if (type == InEx.None) { return; }
            Seperator0 = new Seperator(Context.Index1(), type);
        }
        public void Cut1(InEx type)
        {
            if (type == InEx.None) { return; }
            Seperator1 = new Seperator(Context.Index0(), type);
        }

        public void ToConsole(bool showControl, bool showWithSub, string tab)
        {
            if (showWithSub || Sections.Count == 0)
            {
                string str = ">>" + TextIterator.Printify(Cut(showControl, out string head)) + "<<";
                ConsoleLog.Log(head + tab + str);
            }

            for (int i = 0; i < Sections.Count; i++)
            {
                Sections[i].ToConsole(showControl, showWithSub, tab + "  ");
                if (tab == "") { ConsoleLog.Log(""); }
            }
        }
    }
}
