using OpenTK.Graphics.OpenGL4;

namespace Engine3D.Graphics.Shader.Uniform.Int
{
    public abstract class UniIntN : UniBase
    {
        protected readonly int Count;
        protected readonly int Size;
        protected int[] UpdateData;

        protected UniIntN(BaseShader program, string name, int count, int size) : base(program, name)
        {
            Count = count;
            Size = count * size;
            UpdateData = null;
        }

        public int[] NewData() { return new int[Size]; }

        public override void Update()
        {
            if (UpdateData != null)
            {
                int[] data = UpdateData;
                UpdateData = null;
                Set(data);
            }
        }
        public abstract void Set(int[] data);

        public void Get(int[] data)
        {
            //GL.GetUniform(Program.ID, Location, data);
            Program.UniformGet(Location, data);
        }
        public int[] Get()
        {
            int[] data = NewData();
            Get(data);
            return data;
        }
    }
    public class UniInt1 : UniIntN
    {
        public UniInt1(BaseShader program, string name, int count) : base(program, name, count, 1) { }

        public override void Set(int[] data)
        {
            if (Program.Is())
            {
                GL.Uniform1(Location, Count, data);
            }
            else { UpdateData = data; }
            //GL.ProgramUniform1(Program, Location, Size, data);
        }
    }
    public class UniInt2 : UniIntN
    {
        public UniInt2(BaseShader program, string name, int count) : base(program, name, count, 2) { }

        public override void Set(int[] data)
        {
            if (Program.Is())
            {
                GL.Uniform2(Location, Count, data);
            }
            else { UpdateData = data; }
            //GL.ProgramUniform2(Program, Location, Size, data);
        }
    }
    public class UniInt3 : UniIntN
    {
        public UniInt3(BaseShader program, string name, int count) : base(program, name, count, 3) { }

        public override void Set(int[] data)
        {
            if (Program.Is())
            {
                GL.Uniform3(Location, Count, data);
            }
            else { UpdateData = data; }
            //GL.ProgramUniform3(Program, Location, Size, data);
        }
    }
}
