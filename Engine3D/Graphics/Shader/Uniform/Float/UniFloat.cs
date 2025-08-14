using OpenTK.Graphics.OpenGL4;

namespace Engine3D.Graphics.Shader.Uniform.Float
{
    public abstract class UniFloatN : UniBase
    {
        protected readonly int Count;
        protected readonly int Size;
        protected float[] UpdateData;

        protected UniFloatN(BaseShader program, string name, int count, int size) : base(program, name)
        {
            Count = count;
            Size = count * size;
            UpdateData = null;
        }

        public float[] NewData() { return new float[Size]; }

        public override void Update()
        {
            if (UpdateData != null)
            {
                float[] data = UpdateData;
                UpdateData = null;
                Set(data);
            }
        }
        public abstract void Set(float[] data);

        public void Get(float[] data)
        {
            Program.UniformGet(Location, data);
        }
        public float[] Get()
        {
            float[] data = NewData();
            Get(data);
            return data;
        }
    }
    public class UniFloat1 : UniFloatN
    {
        public UniFloat1(BaseShader program, string name, int count) : base(program, name, count, 1) { }

        public override void Set(float[] data)
        {
            if (Program.Is())
            {
                GL.Uniform1(Location, Count, data);
            }
            else { UpdateData = data; }
            //GL.ProgramUniform1(Program, Location, Size, data);
        }
    }
    public class UniFloat2 : UniFloatN
    {
        public UniFloat2(BaseShader program, string name, int count) : base(program, name, count, 2) { }

        public override void Set(float[] data)
        {
            if (Program.Is())
            {
                GL.Uniform2(Location, Count, data);
            }
            else { UpdateData = data; }
            //GL.ProgramUniform2(Program, Location, Size, data);
        }
    }
    public class UniFloat3 : UniFloatN
    {
        public UniFloat3(BaseShader program, string name, int count) : base(program, name, count, 3) { }

        public override void Set(float[] data)
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
