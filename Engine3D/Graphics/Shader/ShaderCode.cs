using System;
using System.IO;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.Graphics.Shader
{
    public class ShaderCode
    {
        private readonly int ID;
        private readonly string Path;

        private ShaderCode(ShaderType type, string code, string path)
        {
            Path = path;
            ID = GL.CreateShader(type);

            Compile(code);
        }
        ~ShaderCode()
        {
            GL.DeleteShader(ID);
        }

        public void Attach(int ProgramID)
        {
            GL.AttachShader(ProgramID, ID);
        }
        public void Detach(int ProgramID)
        {
            GL.DetachShader(ProgramID, ID);
        }



        private void Compile(string code)
        {
            GL.ShaderSource(ID, code);
            GL.CompileShader(ID);

            string log = GL.GetShaderInfoLog(ID);
            if (!string.IsNullOrEmpty(log))
            {
                if (Path == null)
                {
                    throw new ECompileLog(log);
                }
                else
                {
                    throw new ECompileLog(log, Path);
                }
            }
        }
        class ECompileLog : Exception
        {
            public ECompileLog(string log) : base("Log returned from compiling.\n\n" + log) { }
            public ECompileLog(string log, string path) : base("Log returned from compiling File:" + '"' + path + '"' + ".\n\n" + log) { }
        }



        public static ShaderCode FromFile(string path)
        {
            if (path.Length <= 5) { throw new EInvalidFileExtention(path); }

            /*
             *  .vert - a vertex shader
             *  .tesc - a tessellation control shader
             *  .tese - a tessellation evaluation shader
             *  .geom - a geometry shader
             *  .frag - a fragment shader
             *  .comp - a compute shader
             */

            ShaderType type;
            switch (path.Substring(path.Length - 5))
            {
                case ".vert": type = ShaderType.VertexShader; break;
                case ".geom": type = ShaderType.GeometryShader; break;
                case ".frag": type = ShaderType.FragmentShader; break;
                default: throw new EInvalidFileExtention(path);
            }

            return new ShaderCode(type, File.ReadAllText(path), path);
        }
        class EInvalidFileExtention : Exception
        {
            public EInvalidFileExtention(string path) : base("File:" + '"' + path + '"' + " has an invalid Extention.") { }
        }
    }
}
