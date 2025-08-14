using System;

using Engine3D.TextParser.Sectonizer;
using Engine3D.Miscellaneous.StringHelp;

/*  TODO
        rename so they dont all start with Hierarchy
        organize into Folders/NameSpaces

        Variable Management
            should Variables immedeatly check the Type ?

        preAssabled Structure
            for Example: point(1:2:3)

        remember previous Hierarchy Index ?
            so it dosent have to be rechecked again ?

        remember Section.Cut() in section
            it might be slow to redo every time ?

        make better string checks
            THelp is already Deprecated
             
 */

namespace Engine3D.TextParser.Checker
{
    abstract class Hierarchy
    {
        public abstract bool Check(Section section, ref int offset);

        public static bool IsLog = false;

        protected static void LogProgress(string name, string extra)
        {
            if (IsLog)
            {
                ConsoleLog.LogProgress(name + " " + extra);
            }
        }
        protected static void LogFailure(string name, string extra)
        {
            if (IsLog)
            {
                ConsoleLog.LogFailure(name + " " + extra);
            }
        }
        protected static void LogSuccess(string name, string extra)
        {
            if (IsLog)
            {
                ConsoleLog.LogSuccess(name + " " + extra);
            }
        }

        protected static void LogProgress(string name, int offset)
        {
            LogProgress(name, "(" + offset + ")");
        }
        protected static void LogFailure(string name, int offset)
        {
            LogFailure(name, "(" + offset + ")");
        }
        protected static void LogSuccess(string name, int offset)
        {
            LogSuccess(name,  "(" + offset + ")");
        }
    }

    class HierarchyFixed : Hierarchy
    {
        private readonly Hierarchy[] Structure;
        public HierarchyFixed(Hierarchy[] structure)
        {
            Structure = structure;
        }
        public override bool Check(Section section, ref int offset)
        {
            LogProgress(nameof(HierarchyFixed), offset);
            int off = offset;
            for (int i = 0; i < Structure.Length; i++)
            {
                if (!Structure[i].Check(section, ref off))
                {
                    LogFailure(nameof(HierarchyFixed), offset);
                    return false;
                }
            }
            offset = off;
            LogSuccess(nameof(HierarchyFixed), offset);
            return true;
        }
    }
    class HierarchyTermination : Hierarchy
    {
        public HierarchyTermination()
        {

        }
        public override bool Check(Section section, ref int offset)
        {
            LogProgress(nameof(HierarchyTermination), offset);
            if (offset != section.Sections.Count)
            {
                LogFailure(nameof(HierarchyTermination), offset);
                return false;
            }
            LogSuccess(nameof(HierarchyTermination), offset);
            return true;
        }
    }

    class HierarchyRepeat : Hierarchy
    {
        private readonly Hierarchy Structure;
        public HierarchyRepeat(Hierarchy structure)
        {
            Structure = structure;
        }
        public override bool Check(Section section, ref int offset)
        {
            LogProgress(nameof(HierarchyRepeat), offset);
            while (Structure.Check(section, ref offset))
            {

            }
            LogSuccess(nameof(HierarchyRepeat), offset);
            return true;
        }
    }

    class HierarchyNewLayer : Hierarchy
    {
        private readonly Hierarchy Structure;
        public HierarchyNewLayer(Hierarchy structure)
        {
            Structure = structure;
        }
        public override bool Check(Section section, ref int offset)
        {
            LogProgress(nameof(HierarchyNewLayer), offset);
            if (offset >= section.Sections.Count)
            {
                LogFailure(nameof(HierarchyNewLayer), "out of Bounds");
                return false;
            }
            section = section.Sections[offset];
            int off = 0;
            if (!Structure.Check(section, ref off))
            {
                LogFailure(nameof(HierarchyNewLayer), offset);
                return false;
            }
            offset++;
            LogSuccess(nameof(HierarchyNewLayer), offset);
            return true;
        }
    }

    class HierarchyMultiChoice : Hierarchy
    {
        private readonly Hierarchy[] Structure;
        public HierarchyMultiChoice(Hierarchy[] structure)
        {
            Structure = structure;
        }
        public override bool Check(Section section, ref int offset)
        {
            LogProgress(nameof(HierarchyMultiChoice), offset);
            for (int i = 0; i < Structure.Length; i++)
            {
                if (Structure[i].Check(section, ref offset))
                {
                    LogSuccess(nameof(HierarchyMultiChoice), offset);
                    return true;
                }
            }
            LogFailure(nameof(HierarchyMultiChoice), offset);
            return false;
        }
    }

    class HierarchyLoop : Hierarchy
    {
        private readonly Hierarchy Structure;
        public HierarchyLoop(Hierarchy structure)
        {
            Structure = structure;
        }
        public override bool Check(Section section, ref int offset)
        {
            LogProgress(nameof(HierarchyLoop), offset);
            for (int i = 0; i < section.Sections.Count; i++)
            {
                if (!Structure.Check(section, ref offset))
                {
                    LogFailure(nameof(HierarchyLoop), offset);
                    return false;
                }
            }
            LogSuccess(nameof(HierarchyLoop), offset);
            return true;
        }
    }

    class HierarchyCommand : Hierarchy
    {
        private readonly Hierarchy Structure;
        private readonly Action<Section> Func;
        public HierarchyCommand(Hierarchy structure, Action<Section> func)
        {
            Structure = structure;
            Func = func;
        }
        public override bool Check(Section section, ref int offset)
        {
            LogProgress(nameof(HierarchyCommand), offset);
            if (!Structure.Check(section, ref offset))
            {
                LogFailure(nameof(HierarchyCommand), offset);
                return false;
            }
            Func(section);
            LogSuccess(nameof(HierarchyCommand), offset);
            return true;
        }
    }

    class HierarchyDynamic : Hierarchy
    {
        public Hierarchy Structure;
        public HierarchyDynamic()
        {
            Structure = null;
        }
        public override bool Check(Section section, ref int offset)
        {
            LogProgress(nameof(HierarchyDynamic), offset);
            if (Structure == null)
            {
                LogFailure(nameof(HierarchyDynamic), "null");
                return false;
            }
            if (!Structure.Check(section, ref offset))
            {
                LogFailure(nameof(HierarchyDynamic), offset);
                return false;
            }
            LogSuccess(nameof(HierarchyDynamic), offset);
            return true;
        }
    }





    abstract class HierarchyElement : Hierarchy
    {
        public override bool Check(Section section, ref int offset)
        {
            LogProgress(nameof(HierarchyElement), offset);
            if (offset >= section.Sections.Count)
            {
                LogFailure(nameof(HierarchyElement), "out of Bounds");
                return false;
            }
            section = section.Sections[offset];
            if (section.Sections.Count != 0)
            {
                LogFailure(nameof(HierarchyElement), "has SubSections");
                return false;
            }
            if (!Check(section))
            {
                LogFailure(nameof(HierarchyElement), offset);
                return false;
            }
            offset++;
            LogSuccess(nameof(HierarchyElement), offset);
            return true;
        }
        public abstract bool Check(Section section);
    }

    class HierarchyHeader : HierarchyElement
    {
        private readonly string Text;
        public HierarchyHeader(string text)
        {
            Text = text;
        }
        public override bool Check(Section section)
        {
            LogProgress(nameof(HierarchyHeader), Text);
            string text = section.Cut();
            if (text != Text)
            {
                LogFailure(nameof(HierarchyHeader), Text + " != " + text);
                return false;
            }
            LogSuccess(nameof(HierarchyHeader), Text + " == " + text);
            return true;
        }
    }

    class HierarchyString : HierarchyElement
    {
        public HierarchyString()
        {

        }
        public override bool Check(Section section)
        {
            LogProgress(nameof(HierarchyString), "#");
            string text = section.Cut();
            LogSuccess(nameof(HierarchyString), text);
            return true;
        }
    }

    class HierarchyVariable : HierarchyElement
    {
        public HierarchyVariable()
        {

        }
        public override bool Check(Section section)
        {
            LogProgress(nameof(HierarchyVariable), "#");
            string text = section.Cut();
            if (text != "name" && text != "var0" && text != "var1")
            {
                LogFailure(nameof(HierarchyVariable), text);
                return false;
            }
            LogSuccess(nameof(HierarchyVariable), text);
            return true;
        }
    }
    class HierarchyNumber : HierarchyElement
    {
        public HierarchyNumber()
        {

        }
        public override bool Check(Section section)
        {
            LogProgress(nameof(HierarchyNumber), "#");
            string text = section.Cut();
            if (!text.Check(StringHelp.Digit))
            {
                LogFailure(nameof(HierarchyNumber), text);
                return false;
            }
            LogSuccess(nameof(HierarchyNumber), text);
            return true;
        }
    }
}

