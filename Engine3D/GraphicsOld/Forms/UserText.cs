using System;

using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine3D.GraphicsOld.Forms
{
    public class UserText
    {
        public UserText(Action<string> commandFunc)
        {
            CommandFunction = commandFunc;
            Stopp();
        }

        private Action<string> CommandFunction;

        private string Text;

        private int TextCursor;
        private string TextCursorText;

        private void CursorInc()
        {
            if (TextCursor < Text.Length)
            {
                TextCursor++;
                TextCursorText = new string(' ', TextCursor) + '#';
            }
        }
        private void CursorDec()
        {
            if (TextCursor > 0)
            {
                TextCursor--;
                TextCursorText = new string(' ', TextCursor) + '#';
            }
        }

        private bool InterDigit(Keys key, bool shift)
        {
            if (!shift && key >= Keys.D0 && key <= Keys.D9)
            {
                string c = new string(((char)key), 1);
                CharInsert(c);
                return true;
            }
            return false;
        }
        private bool InterLetter(Keys key, bool shift)
        {
            if (key >= Keys.A && key <= Keys.Z)
            {
                string c = new string(((char)key), 1);

                if (!shift)
                    CharInsert(c.ToLower());
                else
                    CharInsert(c);
                return true;
            }
            return false;
        }
        private bool InterOther(Keys key, bool shift)
        {
            //ConsoleLog.Log("key: " + key.ToString());

            if (key == Keys.Period)
            {
                if (!shift)
                    CharInsert(".");
                else
                    CharInsert(":");
            }
            else if (key == Keys.Comma)
            {
                if (!shift)
                    CharInsert(",");
                else
                    CharInsert(";");
            }
            else if (shift && key == Keys.D1)
                CharInsert("!");
            else if (shift && key == Keys.Minus)
                CharInsert("?");

            else if (!shift && key == Keys.RightBracket)
                CharInsert("+");
            else if (shift && key == Keys.RightBracket)
                CharInsert("*");
            else if (!shift && key == Keys.Slash)
                CharInsert("-");
            else if (shift && key == Keys.D7)
                CharInsert("/");
            else if (shift && key == Keys.D0)
                CharInsert("=");

            else if (shift && key == Keys.D8)
                CharInsert("(");
            else if (shift && key == Keys.D9)
                CharInsert(")");

            else
                return false;
            return true;
        }

        private void CharInsert(string str)
        {
            Text = Text.Insert(TextCursor, str);
            CursorInc();
        }
        private void CharRemove()
        {
            if (TextCursor > 0)
            {
                Text = Text.Remove(TextCursor - 1, 1);
                CursorDec();
            }
        }

        public bool TextKey(Keys key, bool shift)
        {
            if (Text == null)
            {
                if (key == Keys.Enter)
                {
                    Start();
                    return true;
                }
                return false;
            }
            else
            {
                if (key == Keys.Escape)
                    Stopp();

                else if (InterDigit(key, shift))
                    return true;
                else if (InterLetter(key, shift))
                    return true;
                else if (key == Keys.Space)
                    CharInsert(" ");

                else if (key == Keys.Left)
                    CursorDec();
                else if (key == Keys.Right)
                    CursorInc();

                else if (InterOther(key, shift))
                    return true;

                else if (key == Keys.Backspace)
                    CharRemove();
                else if (key == Keys.Enter)
                {
                    if (CommandFunction != null)
                        CommandFunction(Text);
                    Stopp();
                }
                return true;
            }
        }

        public void Start()
        {
            Text = "";
            TextCursor = 0;
            TextCursorText = "#";
        }
        public bool Check()
        {
            return (Text != null);
        }
        public void Stopp()
        {
            Text = null;
            TextCursor = 0;
            TextCursorText = null;
        }

        public void BufferFill(TextBuffers buffer)
        {
            if (Text == null)
                return;
            buffer.Insert(-0.99f, +0.97f, 0xFFFFFF, false, Text);
            buffer.Insert(-0.99f, +0.97f, 0xFFFFFF, false, TextCursorText);
        }
    }
}
