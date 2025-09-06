using Engine3D.Miscellaneous;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.Graphics.Shader.Uniform.Float
{
    /*public abstract class GUniFloatN : GenericUniformBase
    {
        protected readonly int Count;
        protected readonly int Size;
        protected float[] Data;

        protected GUniFloatN(string name, GenericShader[] programs, int count, int size) : base(name, programs)
        {
            Count = count;
            Size = count * size;
            Data = null;
        }

        public float[] NewData() { return new float[Size]; }

        public virtual void ChangeData(float[] data)
        {
            Data = data;
            ChangeData();
        }
        public override void PutData(int location)
        {
            if (Data != null)
            {
                Put(location, Data);
            }
        }
        public abstract void Put(int location, float[] data);
    }
    public class GUniFloat1 : GUniFloatN
    {
        public GUniFloat1(string name, GenericShader[] programs, int count) : base(name, programs, count, 1) { }

        public override void Put(int location, float[] data)
        {
            GL.Uniform1(location, Count, data);
        }
    }
    public class GUniFloat2 : GUniFloatN
    {
        public GUniFloat2(string name, GenericShader[] programs, int count) : base(name, programs, count, 2) { }

        public override void Put(int location, float[] data)
        {
            GL.Uniform2(location, Count, data);
        }
    }
    public class GUniFloat3 : GUniFloatN
    {
        public GUniFloat3(string name, GenericShader[] programs, int count) : base(name, programs, count, 3) { }

        public override void Put(int location, float[] data)
        {
            GL.Uniform3(location, Count, data);
        }
    }*/
}
