using System;
using System.Collections.Generic;

using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine3D.GraphicsOld.Forms
{
    public struct MouseCheck
    {
        private MouseButton button;
        private bool pressed;
        private bool clickD;
        private bool clickU;
        public MouseCheck(MouseButton button)
        {
            this.button = button;
            pressed = false;
            clickD = false;
            clickU = false;
        }

        public void UpdateFrame()
        {
            clickD = false;
            clickU = false;
        }
        public void UpdateDown(MouseButton button)
        {
            if (this.button == button)
            {
                pressed = true;
                clickD = true;
            }
        }
        public void UpdateUpUp(MouseButton button)
        {
            if (this.button == button)
            {
                pressed = false;
                clickU = true;
            }
        }
        public bool CheckClickD()
        {
            return clickD;
        }
        public bool CheckClickU()
        {
            return clickU;
        }
        public bool CheckHold()
        {
            return pressed;
        }
    }
}
