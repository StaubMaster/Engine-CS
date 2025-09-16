using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Engine3D
{
    /*  maybe rework this a bit
     *  clean up and such
     */

    public static class ConsoleLog
    {
        public static Action<string> LogFunc = null;
        public static Action ResetFunc = null;

        private static string RepeatStr;
        private static int RepeatNum;
        private static bool ShortenRepeats = false;

        public static void Log(string str)
        {
            str = TabStr + str + '\n';

            if (ShortenRepeats)
            {
                if (RepeatStr == str)
                {
                    RepeatNum++;
                }
                else
                {
                    RepeatStr = str;
                    if (RepeatNum >= 2)
                    {
                        str = "[x" + RepeatNum + "]\n\n" + str;
                        RepeatNum = 1;
                    }
                    if (LogFunc != null)
                        LogFunc(str);
                }
            }
            else
            {
                if (LogFunc != null)
                    LogFunc(str);
            }
        }
        public static void Reset()
        {
            TabNum = 0;
            TabStr = "";

            RepeatStr = null;
            RepeatNum = 1;

            if (ResetFunc != null)
                ResetFunc();
        }

        public static void Direct(string str)
        {
            LogFunc(str);
        }
        public static void NewLine()
        {
            LogFunc("\n");
        }
        public static void NonPrint(string str)
        {
            string text = "";

            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (c >= 0x00 && c <= 0x1F)
                {
                    text += '\\';
                    if (c == '\0') { text += '0'; }
                    else if (c == '\r') { text += 'r'; }
                    else if (c == '\n') { text += 'n'; }
                    else if (c == '\t') { text += 't'; }
                    else { text += 'x'; }
                }
                else if (c >= 0x7F)
                {
                    text += '\\';
                    text += 'x';
                }
                else
                {
                    text += c;
                }
            }

            LogFunc(text);
        }



        private static string TabStr = "";
        private static uint TabNum = 0;
        public static void TabInc()
        {
            TabNum++;
            TabCalc();
        }
        public static void TabDec()
        {
            TabNum--;
            TabCalc();
        }
        private static void TabCalc()
        {
            TabStr = "";
            for (uint i = 0; i < TabNum; i++)
                TabStr += '|';
        }



        /*
                TEST
                TEMP
                TEMPORARY
            Gray

            General Info
            Fade from Red to Blue
                ERROR
                WARNING
                INFO
                DEBUG

            Significant Function (technically part of info)
            Green
                LOAD File
                SAVE File
                COMMAND
         */

        /*
            127 127 127     7F 7F 7F    Gray            TEST
            095 095 159     5F 5F 9F    Blue Light      DEBUG
            063 063 127     3F 3F 7F    Blue Dark       
            031 159 031     1F 9F 1F    Green           INFO
            255 031 031     FF 1F 1F    Red Light       ERROR
            191 031 031     BF 1F 1F    Red Dark        
        */



        private static (string, Color) HeaderTest = (" [ INFO ] ", Color.FromArgb(127, 127, 127));

        private static (string, Color) HeaderError   = (" [ ERROR ] ", Color.FromArgb(255, 31, 31));
        private static (string, Color) HeaderWarning = (" [ WARNING ] ", Color.FromArgb(255, 255, 0));
        private static (string, Color) HeaderInfo    = (" [ INFO ] ", Color.FromArgb(95, 95, 255));
        private static (string, Color) HeaderDebug   = (" [ DEBUG ] ", Color.FromArgb(255, 0, 127));

        private static (string, Color) HeaderLoad = (" [ LOAD ] ", Color.FromArgb(0, 127, 0));
        private static (string, Color) HeaderSave = (" [ SAVE ] ", Color.FromArgb(0, 127, 0));

        private static (string, Color) HeaderSuccess = (" [ SUCCESS ] ", Color.FromArgb(127, 255, 127));
        private static (string, Color) HeaderFailure = (" [ FAILURE ] ", Color.FromArgb(255, 127, 127));
        private static (string, Color) HeaderChecking = (" [ CHECKING ] ", Color.FromArgb(127, 127, 255));
        private static (string, Color) HeaderProgress = (" [ PROGRESS ] ", Color.FromArgb(127, 127, 255));
        private static (string, Color) HeaderDone =     (" [ DONE ] ", Color.FromArgb(127, 127, 255));

        private static void LogHeader((string, Color) header, string str)
        {
            ColorFore(header.Item2);
            LogFunc(header.Item1);
            LogFunc(str);
            LogFunc("\n");
            ColorNone();
        }

        public static void LogTest(string str)
        {
            LogHeader(HeaderTest, str);
        }

        public static void LogError(string str)
        {
            LogHeader(HeaderError, str);
        }
        public static void LogWarning(string str)
        {
            LogHeader(HeaderWarning, str);
        }
        public static void LogInfo(string str)
        {
            LogHeader(HeaderInfo, str);
        }
        public static void LogDebug(string str)
        {
            LogHeader(HeaderDebug, str);
        }

        public static void LogLoad(string str)
        {
            LogHeader(HeaderLoad, str);
        }
        public static void LogSave(string str)
        {
            LogHeader(HeaderSave, str);
        }

        public static void LogSuccess(string str)
        {
            LogHeader(HeaderSuccess, str);
        }
        public static void LogFailure(string str)
        {
            LogHeader(HeaderFailure, str);
        }
        public static void LogChecking(string str)
        {
            LogHeader(HeaderChecking, str);
        }
        public static void LogProgress(string str)
        {
            LogHeader(HeaderProgress, str);
        }
        public static void LogDone(string str)
        {
            LogHeader(HeaderDone, str);
        }


        public static Action ColorNoneFunc;
        public static Action<Color> ColorForeFunc;
        public static Action<Color> ColorBackFunc;
        public static void ColorNone()
        {
            ColorNoneFunc();
        }
        public static void ColorBoth(int fore, int back)
        {
            ColorFore(fore);
            ColorBack(back);
        }

        public static void ColorFore(Color col)
        {
            ColorForeFunc(col);
        }
        public static void ColorFore(int r, int g, int b)
        {
            ColorForeFunc(Color.FromArgb(r, g, b));
        }
        public static void ColorFore(int hex)
        {
            int r = (hex >> 16) & 0xFF;
            int g = (hex >> 8) & 0xFF;
            int b = (hex >> 0) & 0xFF;
            ColorForeFunc(Color.FromArgb(r, g, b));
        }

        public static void ColorBack(Color col)
        {
            ColorBackFunc(col);
        }
        public static void ColorBack(int r, int g, int b)
        {
            ColorBackFunc(Color.FromArgb(r, g, b));
        }
        public static void ColorBack(int hex)
        {
            int r = (hex >> 16) & 0xFF;
            int g = (hex >> 8) & 0xFF;
            int b = (hex >> 0) & 0xFF;
            ColorBackFunc(Color.FromArgb(r, g, b));
        }
    }
}
