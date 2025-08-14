using System;
using System.IO;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.GraphicsOld
{
    public static class General
    {
        private static string ShaderFolder = "";
        public static void Init_Shader_Folder(string folder)
        {
            folder += "/";

            ConsoleLog.Log("Shader Folder: '" + folder + "'\n");

            ShaderFolder = folder;
        }

        public static int Create_Shader(string file, ShaderType type)
        {
            ConsoleLog.Log("Load Shader " + type.ToString() + " from file '" + file + "'");

            file = ShaderFolder + file;

            if (!File.Exists(file))
            {
                ConsoleLog.Log("Error: File not Found");
                return -1;
            }

            int shader;
            shader = GL.CreateShader(type);
            GL.ShaderSource(shader, File.ReadAllText(file));
            GL.CompileShader(shader);

            string log;
            log = GL.GetShaderInfoLog(shader);
            if (!string.IsNullOrEmpty(log))
                ConsoleLog.Log("Shader Error: \n" + log);

            return shader;
        }
        public static int Create_Shader_Program(string vert_file, string geom_file, string frag_file)
        {
            int program;
            program = GL.CreateProgram();

            int vert, geom, frag;
            vert = Create_Shader(vert_file, ShaderType.VertexShader);
            geom = Create_Shader(geom_file, ShaderType.GeometryShader);
            frag = Create_Shader(frag_file, ShaderType.FragmentShader);

            GL.AttachShader(program, vert);
            GL.AttachShader(program, geom);
            GL.AttachShader(program, frag);
            GL.LinkProgram(program);
            GL.DetachShader(program, vert);
            GL.DetachShader(program, geom);
            GL.DetachShader(program, frag);

            GL.DeleteShader(vert);
            GL.DeleteShader(geom);
            GL.DeleteShader(frag);

            string log;
            log = GL.GetProgramInfoLog(program);
            if (!string.IsNullOrEmpty(log))
                ConsoleLog.Log("Program Error: \n" + log);

            return program;
        }
    }

    public class RenderTrans
    {
        private readonly float[] flt;

        public RenderTrans()
        {
            flt = new float[9]
            {
                0, 0, 0,
                0, 0, 0,
                1, 1, 1,
            };
        }
        public RenderTrans(Abstract3D.Point3D pos)
        {
            flt = new float[9]
            {
                0, 0, 0,
                0, 0, 0,
                1, 1, 1,
            };

            if (pos != null)
                pos.Floats(flt, 0);
        }
        public RenderTrans(Abstract3D.Angle3D wnk)
        {
            flt = new float[9]
            {
                0, 0, 0,
                0, 0, 0,
                1, 1, 1,
            };

            if (wnk != null)
                wnk.FloatsSinCos(flt, 3);
        }
        public RenderTrans(Abstract3D.Point3D pos, Abstract3D.Angle3D wnk)
        {
            flt = new float[9]
            {
                0, 0, 0,
                0, 0, 0,
                1, 1, 1,
            };

            if (pos != null)
                pos.Floats(flt, 0);
            if (wnk != null)
                wnk.FloatsSinCos(flt, 3);
        }
        public RenderTrans(Abstract3D.Transformation3D trans)
        {
            flt = new float[9]
            {
                0, 0, 0,
                0, 0, 0,
                1, 1, 1,
            };

            if (trans != null)
            {
                if (trans.Pos != null)
                    trans.Pos.Floats(flt, 0);
                if (trans.Rot != null)
                    trans.Rot.FloatsSinCos(flt, 3);
            }
        }

        public static void Uniform(RenderTrans data, int uni)
        {
            if (data != null)
                GL.Uniform3(uni, 3, data.flt);
        }
    }
    public class RenderDepthFactors
    {
        private readonly float[] flt;

        public float Near { get { return flt[0]; } set { Calc(value, flt[1]); } }
        public float Far { get { return flt[1]; } set { Calc(flt[0], value); } }

        public RenderDepthFactors(float near, float far)
        {
            flt = new float[7];
            Calc(near, far);
        }
        public void Calc(float near, float far)
        {
            flt[0] = near;
            flt[1] = far;

            flt[2] = flt[1] - flt[0];
            flt[3] = flt[1] + flt[0];
            flt[4] = flt[1] * flt[0] * 2;

            flt[5] = flt[3] / flt[2];
            flt[6] = flt[4] / flt[2];
        }

        public static void Uniform(RenderDepthFactors data, int uni)
        {
            GL.Uniform1(uni, 7, data.flt);
        }
    }
}
