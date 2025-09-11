using System.Collections.Generic;

using Engine3D.Graphics.Shader;
using Engine3D.Graphics.Shader.Uniform;
using Engine3D.Graphics.Shader.Uniform.Float;
using Engine3D.DataStructs;

using Engine3D.Abstract3D;
using Engine3D.Miscellaneous;

namespace Engine3D.Graphics.Shader.Uniform
{
    public class GenericUniformProgramLocation
    {
        private readonly GenericUniformBase Data;
        private readonly GenericShader Program;
        private readonly int Location;
        public bool NeedsUpdate;

        public GenericUniformProgramLocation(GenericUniformBase data, GenericShader program, int location)
        {
            Data = data;
            Program = program;
            Location = location;
            NeedsUpdate = false;
            Program.UniformRemember(this);
        }
        public void ChangeData()
        {
            if (Program.Is())
            {
                PutData();
            }
        }
        public void PutData()
        {
            Data.PutData(Location);
        }
    }
    public abstract class GenericUniformBase
    {
        private readonly GenericUniformProgramLocation[] Locations;

        public GenericUniformBase(string name, GenericShader[] programs)
        {
            //ConsoleLog.Log("Uniform: " + name);

            GenericUniformProgramLocation[] locations = new GenericUniformProgramLocation[programs.Length];
            int locCount = 0;

            for (int i = 0; i < programs.Length; i++)
            {
                int loc = programs[i].UniformFind(name);
                if (loc != -1)
                {
                    locations[locCount] = new GenericUniformProgramLocation(this, programs[i], loc);
                    locCount++;
                }
            }

            Locations = new GenericUniformProgramLocation[locCount];
            for (int i = 0; i < locCount; i++)
            {
                Locations[i] = locations[i];
            }
        }

        public void ChangeData()
        {
            for (int i = 0; i < Locations.Length; i++)
            {
                Locations[i].ChangeData();
            }
        }
        public abstract void PutData(int location);
    }
}
