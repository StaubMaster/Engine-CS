using System;
using System.IO;
using System.Collections.Generic;

using OpenTK.Graphics.OpenGL4;

using Engine3D.OutPut.Uniform;

namespace Engine3D.OutPut.Shader
{
    public partial class AShader
    {
        public readonly string Name;
        protected readonly int GL_Program;
        protected readonly List<SUniformLocation> UniformList;

        public AShader(string name, int id)
        {
            Name = name;
            GL_Program = id;
            UniformList = new List<SUniformLocation>();
        }
        ~AShader()
        {
            GL.DeleteProgram(GL_Program);
        }

        public void Use()
        {
            GL.UseProgram(GL_Program);
        }
        public SUniformLocation UniformLocFind(string name)
        {
            SUniformLocation uni = new SUniformLocation(name, GL_Program);
            if (uni.Location == -1)
            {
                ConsoleLog.Log("Uniform Not Found: " + name);
                throw new EUniformNotFound(name);
            }
            ConsoleLog.Log("Uniform Found: " + name);
            return uni;
        }

        /*public virtual void UniformDirectUpdate()
        {
            Use();
        }*/
        //public abstract void UniformDirectRef(string name, AUniformData data);
        /*public static void UniformDirectRef(AShader[] shaders, string name, AUniformData data)
        {
            for (int i = 0; i < shaders.Length; i++)
            {
                shaders[i].UniformDirectRef(name, data);
            }
        }*/

        public void UniformListRef(string name, AUniformBase data)
        {
            SUniformLocation uni = new SUniformLocation(name, GL_Program);
            if (uni.Location != -1)
            {
                ConsoleLog.Log("Uniform: " + name);
                uni.Data = data;
                UniformList.Add(uni);
            }
        }
        public static void UniformListRef(AShader[] shaders, string name, AUniformBase data)
        {
            for (int i = 0; i < shaders.Length; i++)
            {
                shaders[i].UniformListRef(name, data);
            }
        }

        public void UniformListUpdate()
        {
            Use();
            for (int i = 0; i < UniformList.Count; i++)
            {
                UniformList[i].Update();
            }
        }
        public static void UniformListUpdate(AShader[] shaders)
        {
            for (int i = 0; i < shaders.Length; i++)
            {
                shaders[i].UniformListUpdate();
            }
        }


        class EShaderLog : Exception
        {
            public EShaderLog(string vert_log, string geom_log, string frag_log) : base("Shader Log:\n"
                + "vert Log:\n" + vert_log + "\n"
                + "geom Log:\n" + geom_log + "\n"
                + "frag Log:\n" + frag_log + "\n"
                ) { }
            public EShaderLog(string log) : base("Program Log:\n" + log) { }
        }
        class EUniformNotFound : Exception
        {
            public EUniformNotFound(string name) : base(
                "Uniform:" + '"' + name + '"' + " not found."
                ) { }
        }
    }
}
