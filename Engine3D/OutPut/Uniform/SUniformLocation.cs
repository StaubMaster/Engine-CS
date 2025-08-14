using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.OutPut.Uniform
{
    public struct SUniformLocation
    {
        public readonly string Name;
        public int Location;
        public AUniformBase Data;

        public SUniformLocation(string name, int program)
        {
            Name = name;
            Location = GL.GetUniformLocation(program, Name);
            if (Location == -1)
            {
                ConsoleLog.Log("Uniform not found: " + name);
            }
            else
            {
                ConsoleLog.Log("Uniform found: " + name);
            }
            Data = null;
        }
        public SUniformLocation(string name)
        {
            Name = name;
            Location = -1;
            Data = null;
        }
        public void Find(int program)
        {
            Location = GL.GetUniformLocation(program, Name);
        }

        public void Update()
        {
            if (Data == null) { return; }
            Data.Uniform(Location);
        }

        public void Value(AUniformBase data)
        {
            data.Uniform(Location);
        }
        public void Value(string name, AUniformBase data)
        {
            if (Name == name)
            {
                Data = data;
            }
        }
    }
}
