using System;

using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine3D.InnPut.KeyBoard
{
    public class CKeyCheck
    {
        private Keys key;

        private bool DownTick;
        private bool UpUpTick;

        private Action DownFunc;
        private Action UpUpFunc;

        public CKeyCheck(Keys key, Action funcD, Action funcU)
        {
            this.key = key;

            DownTick = false;
            UpUpTick = false;

            DownFunc = funcD;
            UpUpFunc = funcU;
        }

        public bool Compare(Keys key)
        {
            return (this.key == key);
        }

        public void Tick()
        {
            if (DownTick)
            {
                if (DownFunc != null)
                    DownFunc();
                DownTick = false;
            }

            if (UpUpTick)
            {
                if (UpUpFunc != null)
                    UpUpFunc();
                UpUpTick = false;
            }
        }
        public void TickDown()
        {
            DownTick = true;
        }
        public void TickUpUp()
        {
            UpUpTick = true;
        }

        public override string ToString()
        {
            return '[' + key.ToString() + ']';
        }
    }
}
