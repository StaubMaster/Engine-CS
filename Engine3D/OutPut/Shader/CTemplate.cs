using System;
using System.IO;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.OutPut.Shader
{
    public partial class AShader
    {
        //  change AShader.CTemplate to CShaderFile
        public class CTemplate
        {
            private int ID;
            //  path

            private CTemplate(ShaderType type, string code, string path = null)
            {
                ID = GL.CreateShader(type);

                GL.ShaderSource(ID, code);
                GL.CompileShader(ID);

                string log = GL.GetShaderInfoLog(ID);
                if (!string.IsNullOrEmpty(log))
                {
                    if (path == null)
                    {
                        throw new ECompileLog(log);
                    }
                    else
                    {
                        throw new ECompileLog(log, path);
                    }
                }
            }
            ~CTemplate()
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

            public static CTemplate FromFile(string path)
            {
                if (path.Length < 5) { throw new EInvalidFileExtention(path); }
                string extention = path.Substring(path.Length - 5);

                /*
                 *  .vert - a vertex shader
                 *  .tesc - a tessellation control shader
                 *  .tese - a tessellation evaluation shader
                 *  .geom - a geometry shader
                 *  .frag - a fragment shader
                 *  .comp - a compute shader
                 */

                ShaderType type;
                switch (extention)
                {
                    case ".vert": type = ShaderType.VertexShader; break;
                    case ".geom": type = ShaderType.GeometryShader; break;
                    case ".frag": type = ShaderType.FragmentShader; break;
                    default: throw new EInvalidFileExtention(path);
                }

                return new CTemplate(type, File.ReadAllText(path), path);
            }

            public static CTemplate[] operator &(CTemplate a, CTemplate b)
            {
                return new CTemplate[] { a, b };
            }
            public static CTemplate[] operator &(CTemplate[] arr, CTemplate sh)
            {
                CTemplate[] new_arr = new CTemplate[arr.Length + 1];
                for (int i = 0; i < arr.Length; i++) { new_arr[i] = arr[i]; }
                new_arr[arr.Length] = sh;
                return new_arr;
            }

            public static int CompileProgram(CTemplate[] shaders)
            {
                int ID = GL.CreateProgram();

                for (int i = 0; i < shaders.Length; i++) { shaders[i].Attach(ID); }
                GL.LinkProgram(ID);
                for (int i = 0; i < shaders.Length; i++) { shaders[i].Detach(ID); }

                string log = GL.GetProgramInfoLog(ID);
                if (!string.IsNullOrEmpty(log))
                {
                    throw new ECompileLog(log);
                }

                return ID;
            }

            class EInvalidFileExtention : Exception
            {
                public EInvalidFileExtention(string path) : base(
                    "File:" + '"' + path + '"' + " has an invalid Extention."
                    ) { }
            }
            public class ECompileLog : Exception
            {
                public ECompileLog(string log) : base(
                    "Log returned from compiling.\n\n" + log
                    ) { }
                public ECompileLog(string log, string path) : base(
                    "Log returned from compiling File:" + '"' + path + '"' + ".\n\n" + log
                    ) { }
            }
        }

        public static int CompileProgram(CTemplate[] shaders)
        {
            int ID = GL.CreateProgram();

            //for (int i = 0; i < shaders.Length; i++) { GL.AttachShader(ID, shaders[i].ID); }
            for (int i = 0; i < shaders.Length; i++) { shaders[i].Attach(ID); }
            GL.LinkProgram(ID);
            for (int i = 0; i < shaders.Length; i++) { shaders[i].Detach(ID); }
            //for (int i = 0; i < shaders.Length; i++) { GL.DetachShader(ID, shaders[i].ID); }

            string log = GL.GetProgramInfoLog(ID);
            if (!string.IsNullOrEmpty(log))
            {
                throw new CTemplate.ECompileLog(log);
            }

            return ID;
        }
    }
}