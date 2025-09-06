using System;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.Graphics.Shader
{
    public abstract class BaseShader
    {
        private static int ActiveShaderProgramID = -1;



        private readonly int ID;

        protected BaseShader(ShaderCode[] code)
        {
            ID = GL.CreateProgram();
            Compile(code);
            //ConsoleLog.Log("Shader Program: " + ID);
        }
        ~BaseShader()
        {
            if (ActiveShaderProgramID == ID)
            {
                ActiveShaderProgramID = -1;
            }
            GL.DeleteProgram(ID);
        }



        public void Use()
        {
            GL.UseProgram(ID);
            ActiveShaderProgramID = ID;
            UpdateUniforms();
        }
        public bool Is()
        {
            return (ActiveShaderProgramID == ID);
        }
        protected abstract void UpdateUniforms();


        public int UniformFind(string name)
        {
            int location = GL.GetUniformLocation(ID, name);
            if (location == -1)
            {
                //ConsoleLog.LogError("Uni '" + name + "' not found in Program " + ID + ".");
            }
            else
            {
                //ConsoleLog.LogInfo("Uni '" + name + "' found at " + location + " in Program " + ID + ".");
            }
            return location;
        }

        public void UniformGet(int location, int[] data)
        {
            GL.GetUniform(ID, location, data);
        }
        public void UniformGet(int location, float[] data)
        {
            GL.GetUniform(ID, location, data);
        }
        public void UniformGet(int location, double[] data)
        {
            GL.GetUniform(ID, location, data);
        }

        private void Compile(ShaderCode[] code)
        {
            for (int i = 0; i < code.Length; i++) { code[i].Attach(ID); }
            GL.LinkProgram(ID);
            for (int i = 0; i < code.Length; i++) { code[i].Detach(ID); }

            string log = GL.GetProgramInfoLog(ID);
            if (!string.IsNullOrEmpty(log))
            {
                throw new ECompileLog(log);
            }
        }
        class ECompileLog : Exception
        {
            public ECompileLog(string log) : base("Log returned from compiling.\n\n" + log) { }
        }
    }
}
