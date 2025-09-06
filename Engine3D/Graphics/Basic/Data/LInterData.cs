using OpenTK.Graphics.OpenGL4;

namespace Engine3D.Graphics.Basic.Data
{
    public struct LInterData : IData
    {
        public const int Size = sizeof(float) * 2;

        float T0;
        float T1;

        public static LInterData LIT0()
        {
            LInterData li = new LInterData();
            li.T0 = 1.0f;
            li.T1 = 0.0f;
            return li;
        }
        public static LInterData LIT1()
        {
            LInterData li = new LInterData();
            li.T0 = 0.0f;
            li.T1 = 1.0f;
            return li;
        }

        public void ToUniform(params int[] locations)
        {
            GL.Uniform1(locations[0], 2, new float[] { T0, T1 });
        }

        public static void ToBuffer(int stride, ref System.IntPtr offset, int divisor, params int[] bindIndex)
        {
            GL.EnableVertexAttribArray(bindIndex[0]);
            GL.VertexAttribPointer(bindIndex[0], 2, VertexAttribPointerType.Float, false, stride, offset);
            GL.VertexAttribDivisor(bindIndex[0], divisor);
            offset += Size;
        }
    }
}
