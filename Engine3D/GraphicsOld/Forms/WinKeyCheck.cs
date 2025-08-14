using System;

using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine3D.GraphicsOld.Forms
{
    public abstract class KeyCheck
    {
        protected Keys key;
        public abstract void UpdateFrame();
        public abstract void UpdateDown(Keys key);
        public abstract void UpdateUpUp(Keys key);
        public abstract bool Check();
        public override string ToString()
        {
            return '[' + key.ToString() + ']';
        }
    }
    public class KeyPress : KeyCheck
    {
        private bool pressed;
        public KeyPress(Keys key)
        {
            this.key = Keys.PageDown;
            this.key = key;
            pressed = false;
        }

        public override void UpdateFrame()
        {
            pressed = false;
        }
        public override void UpdateDown(Keys key)
        {
            if (this.key == key)
            {
                pressed = true;
            }
        }
        public override void UpdateUpUp(Keys key)
        {

        }
        public override bool Check()
        {
            return pressed;
        }
    }
    public class KeyToggle : KeyCheck
    {
        private bool toggled;
        public KeyToggle(Keys key)
        {
            this.key = key;
            toggled = false;
        }

        public override void UpdateFrame()
        {

        }
        public override void UpdateDown(Keys key)
        {
            if (this.key == key)
            {
                toggled = !toggled;
            }
        }
        public override void UpdateUpUp(Keys key)
        {

        }
        public override bool Check()
        {
            return toggled;
        }
    }
    public class KeyHold : KeyCheck
    {
        private bool held;
        public KeyHold(Keys key)
        {
            this.key = key;
            held = false;
        }

        public override void UpdateFrame()
        {

        }
        public override void UpdateDown(Keys key)
        {
            if (this.key == key)
            {
                held = true;
            }
        }
        public override void UpdateUpUp(Keys key)
        {
            if (this.key == key)
            {
                held = false;
            }
        }
        public override bool Check()
        {
            return held;
        }
    }

    public class KeyList
    {
        private Keys[] keys;
        private bool pressed;
        private int index;

        public KeyList(Keys[] keys)
        {
            this.keys = keys;
            pressed = false;
            index = -1;
        }
        public void UpdateFrame()
        {
            pressed = false;
            index = -1;
        }
        public void UpdateDown(Keys key)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (keys[i] == key)
                {
                    pressed = true;
                    index = i;
                    return;
                }
            }
        }
        public void UpdateUpUp(Keys key)
        {

        }
        public bool Check()
        {
            return pressed;
        }
        public int Index()
        {
            return index;
        }
    }
}
