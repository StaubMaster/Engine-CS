using System;

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;

namespace Engine3D.Graphics.Display
{
    public class DisplayArea
    {
        private GameWindow GameWin;
        private Color4 DefaultColor;

        private bool IsRunning;
        private bool IsClosing;

        private readonly Action FuncFrame;
        private readonly Action FuncClosing;

        private Vector2 Center;
        public bool MouseLocked;

        public readonly OutPut.Uniform.Specific.CUniformScreenRatio ScreenRatio;

        public DisplayArea(int w, int h, Action func_closing, Action func_frame)
        {
            IsRunning = false;
            IsClosing = false;

            FuncFrame = func_frame;
            FuncClosing = func_closing;

            DefaultColor = new Color4(127, 127, 127, 255);

            {
                GameWindowSettings gws = GameWindowSettings.Default;
                gws.UpdateFrequency = 64;

                NativeWindowSettings nws = NativeWindowSettings.Default;
                nws.APIVersion = Version.Parse("4.1");
                nws.ClientSize = new Vector2i(w, h);
                nws.Location = new Vector2i(2560 - (10 + w), 10);

                GameWin = new GameWindow(gws, nws);
                GameWin.RenderFrame += Frame;
                GameWin.Resize += Resize;

                GL.Enable(EnableCap.DepthTest);
                GL.Enable(EnableCap.CullFace);
                //GL.CullFace(CullFaceMode.Back);
                GL.FrontFace(FrontFaceDirection.Cw);
            }

            ScreenRatio = new OutPut.Uniform.Specific.CUniformScreenRatio(GameWin.ClientSize.X, GameWin.ClientSize.Y);

            Center = GameWin.ClientSize / 2;
            MouseLocked = false;
        }

        private void Closing(System.ComponentModel.CancelEventArgs args)
        {
            FuncClosing();
        }
        private void Resize(ResizeEventArgs args)
        {
            Center = GameWin.ClientSize / 2;
            GL.Viewport(0, 0, GameWin.ClientSize.X, GameWin.ClientSize.Y);
            ScreenRatio.Calc(GameWin.ClientSize.X, GameWin.ClientSize.Y);
        }
        private void Frame(FrameEventArgs args)
        {
            if (!IsRunning || IsClosing) { return; }

            if (GameWin.IsKeyPressed(Keys.Tab)) { MouseLocked = !MouseLocked; }

            GL.Clear(ClearBufferMask.DepthBufferBit);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(DefaultColor);

            if (MouseLocked) { GameWin.MousePosition = Center; }
            if (FuncFrame != null) { FuncFrame(); }

            GameWin.SwapBuffers();
        }

        public void ChangeColor(byte r, byte g, byte b)
        {
            DefaultColor = new Color4(r, g, b, 255);
        }
        public void ClearBufferDepth()
        {
            GL.Clear(ClearBufferMask.DepthBufferBit);
        }



        public void Run()
        {
            if (IsRunning) { return; }
            IsRunning = true;

            GameWin.Run();
        }
        public void Term()
        {
            if (!IsRunning) { return; }

            if (!IsClosing)
            {
                IsClosing = true;
                GameWin.Close();
            }

            GameWin.Dispose();
            GameWin = null;

            IsRunning = false;
        }



        public (float, float) Size_Float2()
        {
            return (
                GameWin.ClientSize.X,
                GameWin.ClientSize.Y
            );
        }

        public (float, float) MousePixel()
        {
            return (
                GameWin.MousePosition.X,
                GameWin.ClientSize.Y - GameWin.MousePosition.Y
                );
        }
        public Vector2 MouseCenterDiff_Vector2()
        {
            Vector2 diff = new Vector2();

            diff.X = GameWin.MousePosition.X - Center.X;
            diff.Y = Center.Y - GameWin.MousePosition.Y;

            return diff;
        }
        public (float, float) MouseCenterDiff_Float2()
        {
            return (
                GameWin.MousePosition.X - Center.X,
                Center.Y - GameWin.MousePosition.Y
            );
        }
        public Abstract3D.Point3D MouseRay()
        {
            (float, float) mouseWin = MouseCenterDiff_Float2();
            return new Abstract3D.Point3D(
                mouseWin.Item1 / (ScreenRatio.Data[0] * ScreenRatio.Data[2] * 0.5f),
                mouseWin.Item2 / (ScreenRatio.Data[1] * ScreenRatio.Data[3] * 0.5f),
                1);
        }
        public float MouseScroll()
        {
            return GameWin.MouseState.ScrollDelta.Y;
        }



        public Abstract3D.Point3D MoveByKeys(double speed = 1.0, double fast = 100.0)
        {
            Abstract3D.Point3D pos = new Abstract3D.Point3D();
            if (GameWin.IsKeyDown(Keys.D)) { pos.C -= speed; }
            if (GameWin.IsKeyDown(Keys.E)) { pos.C += speed; }
            if (GameWin.IsKeyDown(Keys.S)) { pos.Y -= speed; }
            if (GameWin.IsKeyDown(Keys.F)) { pos.Y += speed; }
            if (GameWin.IsKeyDown(Keys.LeftShift)) { pos.X -= speed; }
            if (GameWin.IsKeyDown(Keys.Space))     { pos.X += speed; }
            if (GameWin.IsKeyDown(Keys.LeftControl)) { pos *= fast; }
            return pos;
        }
        public Abstract3D.Angle3D SpinByMouse(double factor = 0.005)
        {
            Abstract3D.Angle3D rot = new Abstract3D.Angle3D();
            if (MouseLocked)
            {
                Vector2 diff = MouseCenterDiff_Vector2();
                rot.A = diff.X * factor;
                rot.S = diff.Y * factor;
            }
            return rot;
        }



        public interface KeyState
        {
            public bool IsDown();
            public bool IsUp();
            public bool IsPressed();
            public bool IsReleased();
        }
        public struct KeyStateBoard : KeyState
        {
            private GameWindow Window;
            private Keys Key;

            public KeyStateBoard(GameWindow window, Keys key)
            {
                Window = window;
                Key = key;
            }

            public bool IsDown() { return Window.IsKeyDown(Key); }
            public bool IsUp() { return !Window.IsKeyDown(Key); }
            public bool IsPressed() { return Window.IsKeyPressed(Key); }
            public bool IsReleased() { return Window.IsKeyReleased(Key); }
        }
        public struct KeyStateMouse : KeyState
        {
            private GameWindow Window;
            private MouseButton Button;

            public KeyStateMouse(GameWindow window, MouseButton button)
            {
                Window = window;
                Button = button;
            }

            public bool IsDown() { return Window.MouseState.IsButtonDown(Button); }
            public bool IsUp() { return !Window.MouseState.IsButtonDown(Button); }
            public bool IsPressed() { return Window.MouseState.IsButtonPressed(Button); }
            public bool IsReleased() { return Window.MouseState.IsButtonReleased(Button); }
        }

        public KeyState CheckKey(Keys key)
        {
            return new KeyStateBoard(GameWin, key);
        }
        public KeyState CheckKey(MouseButton button)
        {
            return new KeyStateMouse(GameWin, button);
        }
    }
}
