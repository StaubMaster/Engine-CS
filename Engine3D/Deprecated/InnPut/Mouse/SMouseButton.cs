using System;
using System.Collections.Generic;

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;

namespace Engine3D.InnPut.Mouse
{
    public struct SMouseButton
    {
        private readonly GameWindow Window;
        private readonly MouseButton Button;

        public SMouseButton(GameWindow window, MouseButton button)
        {
            Window = window;
            Button = button;
        }

        public bool Down() { return Window.IsMouseButtonDown(Button); }
        public bool Up() { return !Window.IsMouseButtonDown(Button); }
        public bool Press() { return Window.IsMouseButtonPressed(Button); }
        public bool Release() { return Window.IsMouseButtonReleased(Button); }
    }

    /*public class CMouseButton
    {
        private MouseButton button;
        private bool pressed;
        private bool pulseD;
        private bool pulseU;

        public CMouseButton(MouseButton button)
        {
            this.button = button;

            pressed = false;
            pulseD = false;
            pulseU = false;
        }

        public bool Compare(MouseButton button)
        {
            return (this.button == button);
        }

        public void Tick()
        {
            pulseD = false;
            pulseU = false;
        }
        public void Down()
        {
            pressed = true;
            pulseD = true;
        }
        public void UpUp()
        {
            pressed = false;
            pulseU = true;
        }

        public bool CheckPressed()
        {
            return pressed;
        }
        public bool CheckPulseD()
        {
            return pulseD;
        }
        public bool CheckPulseU()
        {
            return pulseU;
        }
    }*/
}
