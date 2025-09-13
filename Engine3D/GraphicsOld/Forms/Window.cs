using System;
using System.Collections.Generic;

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;

using Engine3D;
using Engine3D.Abstract3D;
using Engine3D.GraphicsOld;

namespace Engine3D.GraphicsOld.Forms
{
    public partial class Window
    {
        private Action FuncDelete;
        private GameWindow GameWin;
        private bool isClosing;
        private Vector2 Middle;

        public UserText UText;

        private bool Mouse_tabbed;
        private bool Mouse_fixed;
        public MouseCheck Mouse_L;
        public MouseCheck Mouse_R;

        public List<KeyCheck> KeyChecks;
        public List<KeyList> KeyChecksL;



        public Window()
        {
            KeyChecks = new List<KeyCheck>();
            KeyChecksL = new List<KeyList>();

            Mouse_L = new MouseCheck(MouseButton.Left);
            Mouse_R = new MouseCheck(MouseButton.Right);
        }

        public void Create(int w, int h, Action func_delete, Action<string> commandFunc)
        {
            if (GameWin != null) { return; }
            ConsoleLog.Log("Create Window");
            ConsoleLog.TabInc();

            FuncDelete = func_delete;

            {
                GameWindowSettings gws = GameWindowSettings.Default;
                gws.UpdateFrequency = 64;

                NativeWindowSettings nws = NativeWindowSettings.Default;
                nws.APIVersion = Version.Parse("4.1");
                nws.ClientSize = new Vector2i(w, h);
                nws.Location = new Vector2i(2560 - (10 + w), 10);

                GameWin = new GameWindow(gws, nws);

                GameWin.RenderFrame += Internal_Frame;
                GameWin.Closing += Closing;

                GameWin.Resize += Resize;

                GameWin.KeyDown += KeyDown;
                GameWin.KeyUp += KeyUpUp;

                GameWin.MouseDown += MouseDown;
                GameWin.MouseUp += MouseUpUp;

                GL.Enable(EnableCap.DepthTest);
                GL.Enable(EnableCap.CullFace);
                //GL.CullFace(CullFaceMode.Back);
                GL.FrontFace(FrontFaceDirection.Cw);

                isClosing = false;
            }

            Middle.X = GameWin.ClientSize.X / 2;
            Middle.Y = GameWin.ClientSize.Y / 2;
            Mouse_tabbed = false;
            Mouse_fixed = false;

            UText = new UserText(commandFunc);

            ConsoleLog.TabDec();
            ConsoleLog.Log("");
        }

        private void Resize(ResizeEventArgs obj)
        {
            GL.Viewport(0, 0, GameWin.ClientSize.X, GameWin.ClientSize.Y);
            Middle.X = GameWin.ClientSize.X / 2;
            Middle.Y = GameWin.ClientSize.Y / 2;
        }

        public void Run()
        {
            ConsoleLog.Log("\nRun\n");
            GameWin.Run();
        }
        public void Delete()
        {
            if (!isClosing)
            {
                isClosing = true;
                GameWin.Close();
            }

            if (GameWin == null) { return; }
            ConsoleLog.Log("Remove Window");
            ConsoleLog.TabInc();

            GameWin.Dispose();
            GameWin = null;

            UText = null;

            ConsoleLog.TabDec();
            ConsoleLog.Log("");
        }

        private void Closing(System.ComponentModel.CancelEventArgs obj)
        {
            FuncDelete();
        }



        public Action External_Frame;
        private void Internal_Frame(FrameEventArgs args)
        {
            if (GameWin == null) { return; }

            GL.Clear(ClearBufferMask.DepthBufferBit);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(0.5f, 0.5f, 0.5f, 1);
            //GL.ClearColor(0.0f, 0.0f, 0.0f, 1);

            if (External_Frame != null)
                External_Frame();

            if (KeyChecks != null)
            {
                for (int i = 0; i < KeyChecks.Count; i++)
                {
                    //KeyChecks[i].UpdateFrame();
                }
            }

            if (KeyChecksL != null)
            {
                for (int i = 0; i < KeyChecksL.Count; i++)
                {
                    //KeyChecksL[i].UpdateFrame();
                }
            }

            Mouse_L.UpdateFrame();
            Mouse_R.UpdateFrame();

            GameWin.SwapBuffers();
        }

        public Point3D VelPos_Key(double scale = 1.0, double shift = 100.0)
        {
            Point3D pos = Point3D.Default();

            if (CheckKey(Keys.D)) { pos.C -= (float)scale; }
            if (CheckKey(Keys.E)) { pos.C += (float)scale; }

            if (CheckKey(Keys.S)) { pos.Y -= (float)scale; }
            if (CheckKey(Keys.F)) { pos.Y += (float)scale; }

            if (CheckKey(Keys.LeftShift)) { pos.X -= (float)scale; }
            if (CheckKey(Keys.Space))     { pos.X += (float)scale; }

            if (CheckKey(Keys.LeftControl)) { pos *= (float)shift; }

            return pos;
        }
        public Angle3D? VelRot_Mouse(double scale = 0.005)
        {
            Angle3D? rot = null;

            if (Mouse_fixed)
            {
                //rot = new Angle3D();
                //rot.Value.A = (GameWin.MousePosition.X - Middle.X) * scale;
                //rot.Value.S = (Middle.Y - GameWin.MousePosition.Y) * scale;
                rot = new Angle3D(
                    (GameWin.MousePosition.X - Middle.X) * scale,
                    (Middle.Y - GameWin.MousePosition.Y) * scale,
                    0);
            }
            if (Mouse_tabbed)
            {
                GameWin.MousePosition = Middle;
            }
            Mouse_fixed = Mouse_tabbed;

            return rot;
        }



        private bool CheckKey(Keys key)
        {
            if (UText.Check())
                return false;
            return GameWin.IsKeyDown(key);
        }
        private void KeyDown(KeyboardKeyEventArgs args)
        {
            if (UText.TextKey(args.Key, args.Shift)) { return; }

            if (args.Key == Keys.Escape) { GameWin.Close(); }
            if (args.Key == Keys.Tab) { Mouse_tabbed ^= true; }

            if (KeyChecks != null)
            {
                for (int i = 0; i < KeyChecks.Count; i++)
                {
                    KeyChecks[i].UpdateDown(args.Key);
                }
            }
            if (KeyChecksL != null)
            {
                for (int i = 0; i < KeyChecksL.Count; i++)
                {
                    KeyChecksL[i].UpdateDown(args.Key);
                }
            }
        }
        private void KeyUpUp(KeyboardKeyEventArgs args)
        {
            if (UText.Check()) { return; }

            if (KeyChecks != null)
            {
                for (int i = 0; i < KeyChecks.Count; i++)
                    KeyChecks[i].UpdateUpUp(args.Key);
            }
            if (KeyChecksL != null)
            {
                for (int i = 0; i < KeyChecksL.Count; i++)
                    KeyChecksL[i].UpdateUpUp(args.Key);
            }
        }



        private void MouseDown(MouseButtonEventArgs obj)
        {
            Mouse_L.UpdateDown(obj.Button);
            Mouse_R.UpdateDown(obj.Button);
        }
        private void MouseUpUp(MouseButtonEventArgs obj)
        {
            Mouse_L.UpdateUpUp(obj.Button);
            Mouse_R.UpdateUpUp(obj.Button);
        }

        public bool MouseIsTabbed()
        {
            return Mouse_tabbed;
        }
        public bool MouseL()
        {
            return Mouse_L.CheckHold();
        }
        public bool MouseR()
        {
            return Mouse_R.CheckHold();
        }
        public int MouseScroll()
        {
            return (int)GameWin.MouseState.ScrollDelta.Y;
        }
        public (float, float) MouseFromMiddle()
        {
            return (
                GameWin.MousePosition.X - Middle.X,
                Middle.Y - GameWin.MousePosition.Y
                );
        }

        public (float, float) Size()
        {
            return (GameWin.ClientSize.X, GameWin.ClientSize.Y);
        }
    }
}
