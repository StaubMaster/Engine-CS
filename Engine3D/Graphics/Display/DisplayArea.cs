using System;

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;

namespace Engine3D.Graphics.Display
{
    public class DisplayArea : GameWindow
    {
        private Vector2 Center;

        private Vector2 MouseLast;
        private Vector2 MouseDelta;
        private bool MouseLockChanging;

        private Color4 DefaultColor;

        private bool IsRunning;
        private bool IsClosing;

        private readonly Action FuncFrame;
        private readonly Action FuncClosing;

        public readonly OutPut.Uniform.Specific.CUniformScreenRatio ScreenRatio;

        private static GameWindowSettings DefaultGameSettings()
        {
            GameWindowSettings gws = GameWindowSettings.Default;
            return gws;
        }
        private static NativeWindowSettings DefaultNativeSettings(int w, int h)
        {
            NativeWindowSettings nws = NativeWindowSettings.Default;
            nws.APIVersion = Version.Parse("4.1");
            nws.ClientSize = new Vector2i(w, h);
            return nws;
        }

        public DisplayArea(int w, int h, Action func_closing, Action func_frame)
            : base(DefaultGameSettings(), DefaultNativeSettings(w, h))
        {
            UpdateFrequency = 64;
            Center = new Vector2(ClientSize.X / 2, ClientSize.Y / 2);

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
                //nws.Location = new Vector2i(2560 - (10 + w), 10);

                //GameWin = new GameWindow(gws, nws);
                //this.RenderFrame += Frame;
                //this.Resize += Resize;

                GL.Enable(EnableCap.DepthTest);
                GL.Enable(EnableCap.CullFace);
                //GL.CullFace(CullFaceMode.Back);
                GL.FrontFace(FrontFaceDirection.Cw);
            }

            ScreenRatio = new OutPut.Uniform.Specific.CUniformScreenRatio(this.ClientSize.X, this.ClientSize.Y);

            MouseDelta = new Vector2();
            MouseLockChanging = false;
        }

        /*private void Closing(System.ComponentModel.CancelEventArgs args)
        {
            FuncClosing();
        }*/
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            Center = new Vector2(ClientSize.X / 2, ClientSize.Y / 2);
            GL.Viewport(0, 0, this.ClientSize.X, this.ClientSize.Y);
            ScreenRatio.Calc(this.ClientSize.X, this.ClientSize.Y);
        }



        protected override void OnRenderFrame(FrameEventArgs args)
        {
            if (!IsRunning || IsClosing) { return; }

            base.OnRenderFrame(args);

            MouseUpdate();

            GL.Clear(ClearBufferMask.DepthBufferBit);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(DefaultColor);

            if (FuncFrame != null) { FuncFrame(); }

            this.SwapBuffers();
        }

        public void ChangeColor(byte r, byte g, byte b)
        {
            DefaultColor = new Color4(r, g, b, 255);
        }
        public void ClearBufferDepth()
        {
            GL.Clear(ClearBufferMask.DepthBufferBit);
        }



        public override void Run()
        {
            if (IsRunning) { return; }
            IsRunning = true;

            base.Run();
        }
        public void Term()
        {
            if (!IsRunning) { return; }

            if (!IsClosing)
            {
                IsClosing = true;
                Close();
            }

            Dispose();

            IsRunning = false;
        }



        public (float, float) Size_Float2()
        {
            return (
                this.ClientSize.X,
                this.ClientSize.Y
            );
        }



        public (float, float) MousePixel()
        {
            return (
                this.MousePosition.X,
                this.ClientSize.Y - this.MousePosition.Y
                );
        }

        public bool MouseLocked { get { return (this.CursorState != CursorState.Normal); } }
        public void MouseLock()
        {
            this.CursorState = CursorState.Grabbed;
            MouseLockChanging = true;
        }
        public void MouseUnlock()
        {
            this.CursorState = CursorState.Normal;
            MouseLockChanging = true;
        }
        private void MouseUpdate()
        {
            if (IsKeyPressed(Keys.Tab))
            {
                if (!MouseLocked)
                {
                    MouseLock();
                }
                else
                {
                    MouseUnlock();
                }
            }

            if (!MouseLockChanging)
            {
                MouseDelta.X = this.MousePosition.X - MouseLast.X;
                MouseDelta.Y = MouseLast.Y - this.MousePosition.Y;
                MouseLast = this.MousePosition;
            }
            MouseLockChanging = false;
        }

        public Abstract3D.Point3D MouseRay()
        {
            if (MouseLocked)
            {
                return new Abstract3D.Point3D(0, 0, 1);
            }

            float mouseCenterX = this.MousePosition.X - Center.X;
            float mouseCenterY = Center.Y - this.MousePosition.Y;

            return new Abstract3D.Point3D(
                mouseCenterX / (ScreenRatio.Data[0] * ScreenRatio.Data[2] * 0.5f),
                mouseCenterY / (ScreenRatio.Data[1] * ScreenRatio.Data[3] * 0.5f),
                1);
        }
        public float MouseScroll()
        {
            return this.MouseState.ScrollDelta.Y;
        }



        public Abstract3D.Point3D MoveByKeys(float speed = 1.0f, float fast = 100.0f)
        {
            Abstract3D.Point3D pos = Abstract3D.Point3D.Default();
            if (this.IsKeyDown(Keys.D)) { pos.C -= (float)speed; }
            if (this.IsKeyDown(Keys.E)) { pos.C += (float)speed; }
            if (this.IsKeyDown(Keys.S)) { pos.Y -= (float)speed; }
            if (this.IsKeyDown(Keys.F)) { pos.Y += (float)speed; }
            if (this.IsKeyDown(Keys.LeftShift)) { pos.X -= (float)speed; }
            if (this.IsKeyDown(Keys.Space))     { pos.X += (float)speed; }
            if (this.IsKeyDown(Keys.LeftControl)) { pos *= fast; }
            return pos;
        }
        public Abstract3D.Angle3D SpinByMouse(float factor = 0.005f)
        {
            Abstract3D.Angle3D rot = Abstract3D.Angle3D.Default();
            if (MouseLocked)
            {
                rot.A = MouseDelta.X * factor;
                rot.S = MouseDelta.Y * factor;
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
            return new KeyStateBoard(this, key);
        }
        public KeyState CheckKey(MouseButton button)
        {
            return new KeyStateMouse(this, button);
        }
    }
}
