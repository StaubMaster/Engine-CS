using System;
using System.IO;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.OutPut.Shader
{
    public struct SShaderFileTemplate
    {
        public static string ShaderFileDir = "";

        public string Path;
        public ShaderType Type;

        public string Log;
        public int ID;

        public SShaderFileTemplate(string path, ShaderType type)
        {
            Path = ShaderFileDir + path;
            Type = type;

            Log = "";
            ID = -1;
        }

        public void Create()
        {
            ID = GL.CreateShader(Type);
            GL.ShaderSource(ID, File.ReadAllText(Path));
            GL.CompileShader(ID);

            Log = GL.GetShaderInfoLog(ID);
        }
        public void Delete()
        {
            GL.DeleteShader(ID);
        }

        public bool hasLog()
        {
            return (!string.IsNullOrEmpty(Log));
        }

        public static int Load_Shader_File(string file, ShaderType type)
        {
            file = ShaderFileDir + file;

            int shader = GL.CreateShader(type);

            GL.ShaderSource(shader, File.ReadAllText(file));
            GL.CompileShader(shader);

            return shader;
        }
    }
}
