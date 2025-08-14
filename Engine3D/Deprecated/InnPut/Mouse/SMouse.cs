using System;
using System.Collections.Generic;

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;

namespace Engine3D.InnPut.Mouse
{
    public struct SMouse
    {
        private GameWindow Window;

        public SMouseButton L;
        public SMouseButton R;

        public bool IsLocked;
        public Vector2 Lock;

        public SMouse(GameWindow window)
        {
            Window = window;

            L = new SMouseButton(window, MouseButton.Left);
            R = new SMouseButton(window, MouseButton.Right);

            IsLocked = false;
            Lock = window.ClientSize / 2;
        }

        public int Scroll()
        {
            return (int)Window.MouseState.ScrollDelta.Y;
        }
    }
}
