using System;

namespace Engine3D.TextParser.Sectonizer
{
    class Layer
    {
        private readonly TextIterator Context;
        public Section SectionMain;

        public Layer(string text, Splitter template)
        {
            Context = new TextIterator(text);
            SplitType(template, null);
        }

        //  SectionMain.CurrentSection is only used to split Children and or to create itself
        //      make private ?
        //      put all this Split stuff into Sectopn ?
        private void SplitMain(Splitter.Main splitter, Section section)
        {
            SectionMain = new Section(Context, section);
            SectionMain.CurrentSection = new Section(Context, section);

            for (Context.LoopInit(); Context.LoopIsLimit(); Context.LoopContinue())
            {
                SplitArr(splitter, SectionMain);
            }

            SectionMain.Insert(SectionMain.CurrentSection, false);
        }
        private void SplitSolo(Splitter.Solo splitter, Section section)
        {
            if (section.CurrentSection == null)
            {
                section.CurrentSection = new Section(Context, section);
            }

            if (splitter.Check(Context.CurrentChar))
            {
                //ConsoleLog.Log(Context.CurrentCharToString("Solo"));
                section.InsertSub(false, InEx.InControl, InEx.ExRegular);
            }

            if (section.CurrentSection != null)
            {
                SplitArr(splitter, section.CurrentSection);
            }
        }
        private void SplitTwin(Splitter.Twin splitter, Section section)
        {
            if (splitter.Check(Context.CurrentChar))
            {
                //ConsoleLog.Log(Context.CurrentCharToString("Twin 0"));
                section.InsertSub(false, InEx.ExRegular, InEx.InControl);

                for (Context.LoopContinue(); Context.LoopIsLimit(); Context.LoopContinue())
                {
                    if (splitter.Check(Context.CurrentChar))
                    {
                        //ConsoleLog.Log(Context.CurrentCharToString("Twin 1"));
                        section.InsertSub(false, InEx.InControl, InEx.ExRegular);
                        return;
                    }

                    if (section.CurrentSection != null)
                    {
                        SplitArr(splitter, section.CurrentSection);
                    }
                }
            }
        }
        private void SplitPair(Splitter.Pair splitter, Section section)
        {
            if (splitter.Check0(Context.CurrentChar))
            {
                //ConsoleLog.Log(Context.CurrentCharToString("Pair 0"));
                section.InsertSub(false, InEx.ExRegular, InEx.InControl);

                for (Context.LoopContinue(); Context.LoopIsLimit(); Context.LoopContinue())
                {
                    if (splitter.Check1(Context.CurrentChar))
                    {
                        //ConsoleLog.Log(Context.CurrentCharToString("Pair 1"));
                        section.InsertSub(false, InEx.InControl, InEx.ExRegular);
                        return;
                    }

                    if (section.CurrentSection != null)
                    {
                        SplitArr(splitter, section.CurrentSection);
                    }
                }
            }
        }
        private void SplitType(Splitter splitter, Section section)
        {
            Type type = splitter.GetType();
            if (type == typeof(Splitter.Main)) { SplitMain((Splitter.Main)splitter, section); }
            if (type == typeof(Splitter.Solo)) { SplitSolo((Splitter.Solo)splitter, section); }
            if (type == typeof(Splitter.Twin)) { SplitTwin((Splitter.Twin)splitter, section); }
            if (type == typeof(Splitter.Pair)) { SplitPair((Splitter.Pair)splitter, section); }
        }

        private void SplitArr(Splitter splitter, Section section)
        {
            if (splitter == null) { return; }
            Splitter[] splitters = splitter.Splitters;

            if (splitters == null) { return; }
            for (int i = 0; i < splitters.Length; i++)
            {
                SplitType(splitters[i], section);
            }
        }

        public void Show(bool showControl, bool showWithSub)
        {
            if (SectionMain == null) { return; }
            ConsoleLog.Log("<><><><><><><><><><><><><><><><>");
            SectionMain.ToConsole(showControl, showWithSub, "");
            ConsoleLog.Log("<><><><><><><><><><><><><><><><>");
        }
    }
}
