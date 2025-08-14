using System;
using System.Collections.Generic;
using System.IO;

namespace Engine3D.StringParse
{
    [Obsolete("newer Version in StringInter", false)]
    public class CFileInterpreter
    {
        /*
            - Split by Quotes
                - go over each char
                - if ' : go foreward until next ', ignore "
                - if " : go foreward until next ", ignore '
            - Split by Brackets into Hierarchy
                - go over each char
                - if char is in quote, skip
                - if {, go foreward, count { and } until both are equal and }
                - if [, go foreward, count [ and ] until both are equal and ]
                - if (, go foreward, count ( and ) until both are equal and )
            BAD
                different brackets mixing
                brackets seperated in different blocks due to quotes
         */

        /*
            do all at the same time
            if "
                skip to next "
                if no next : Error
            if {
                push to Stack
            if }
                if last in Stack is {
                    close { and pop from Stack
                else 
                    last is other bracket : Error
            do same for other brackets
            after loop
                if list not empty : Error
            GOOD
        */

        /*
            - Split File
            - Turn Splits into Hierarchy
            - Turn Hierarchy into Data (custom)
        */

        public static CDataHierarchyChildren FileToHeierarchy(string path)
        {
            string text = File.ReadAllText(path);

            TSplit.SSettings settings = TSplit.SSettings.Default();
            TSplit.SData data = settings.Parse(text);

            return data.ToHierarchy();
        }

        public static void Test(string path)
        {
            ConsoleLog.Log("====    ====    ====    ====");

            string text = File.ReadAllText(path);

            TSplit.SSettings settings = TSplit.SSettings.Default();
            TSplit.SData data = settings.Parse(text);
            data.ToConsoleColor();
            data.ToConsoleText();

            CDataHierarchyChildren Hir = data.ToHierarchy();
            ConsoleLog.Log(Hir.ToStringGraph("#", true));

            ConsoleLog.Log("====    ====    ====    ====");
        }
    }
}
