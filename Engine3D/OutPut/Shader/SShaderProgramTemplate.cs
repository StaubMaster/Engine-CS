using System;
using System.IO;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.OutPut.Shader
{
    public struct SShaderProgramTemplate
    {
        public string Name;
        public SShaderFileTemplate Vert;
        public SShaderFileTemplate Geom;
        public SShaderFileTemplate Frag;

        public SShaderProgramTemplate(string name, string vert, string geom, string frag)
        {
            Name = name;

            Vert = new SShaderFileTemplate(vert, ShaderType.VertexShader);
            Geom = new SShaderFileTemplate(geom, ShaderType.GeometryShader);
            Frag = new SShaderFileTemplate(frag, ShaderType.FragmentShader);
        }

        public void Create()
        {
            Vert.Create();
            Geom.Create();
            Frag.Create();
        }
        public void Delete()
        {
            Vert.Delete();
            Geom.Delete();
            Frag.Delete();
        }

        public bool hasLog()
        {
            return (Vert.hasLog() || Geom.hasLog() || Frag.hasLog());
        }

        public static int Load_Shader_Prog(SShaderProgramTemplate template)
        {
            //int vert = Load_Shader_File(template.Vert_File, ShaderType.VertexShader);
            //int geom = Load_Shader_File(template.Geom_File, ShaderType.GeometryShader);
            //int frag = Load_Shader_File(template.Frag_File, ShaderType.FragmentShader);

            //string vert_log = GL.GetShaderInfoLog(vert);
            //string geom_log = GL.GetShaderInfoLog(geom);
            //string frag_log = GL.GetShaderInfoLog(frag);
            //if (!string.IsNullOrEmpty(vert_log) || !string.IsNullOrEmpty(geom_log) || !string.IsNullOrEmpty(frag_log))
            //{
            //    GL.DeleteShader(vert);
            //    GL.DeleteShader(geom);
            //    GL.DeleteShader(frag);
            //
            //    throw new EShaderLog(vert_log, geom_log, frag_log);
            //}

            template.Create();
            if (template.hasLog())
            {
                template.Delete();
                throw new AShader.EShaderLog(template.Vert.Log, template.Geom.Log, template.Frag.Log);
            }


            int program = GL.CreateProgram();

            //GL.AttachShader(program, vert);
            //GL.AttachShader(program, geom);
            //GL.AttachShader(program, frag);
            GL.AttachShader(program, template.Vert.ID);
            GL.AttachShader(program, template.Geom.ID);
            GL.AttachShader(program, template.Frag.ID);
            GL.LinkProgram(program);
            GL.DetachShader(program, template.Vert.ID);
            GL.DetachShader(program, template.Geom.ID);
            GL.DetachShader(program, template.Frag.ID);
            //GL.DetachShader(program, vert);
            //GL.DetachShader(program, geom);
            //GL.DetachShader(program, frag);

            //GL.DeleteShader(vert);
            //GL.DeleteShader(geom);
            //GL.DeleteShader(frag);

            template.Delete();

            string log = GL.GetProgramInfoLog(program);
            if (!string.IsNullOrEmpty(log))
            {
                throw new AShader.EShaderLog(log);
            }

            return program;
        }
    }
}
