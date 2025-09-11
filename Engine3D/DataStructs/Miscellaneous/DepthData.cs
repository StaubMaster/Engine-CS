using OpenTK.Graphics.OpenGL4;

namespace Engine3D.DataStructs
{
    public struct DepthData : IData
    {
        float Near;
        float Far;

        float Diff;
        float Summ;
        float Mul2;

        float Factor0;
        float Factor1;

        public DepthData(float near, float far)
        {
            Near = near;
            Far = far;

            Diff = float.NaN;
            Summ = float.NaN;
            Mul2 = float.NaN;

            Factor0 = float.NaN;
            Factor1 = float.NaN;

            Calc();
        }
        private void Calc()
        {
            Diff = Far - Near;
            Summ = Far + Near;
            Mul2 = Far * Near * 2;

            Factor0 = Summ / Diff;
            Factor1 = Mul2 / Diff;
        }

        public void ChangeNear(float near)
        {
            Near = near;
            Calc();
        }
        public void ChangeFar(float far)
        {
            Far = far;
            Calc();
        }

        public void ToUniform(params int[] locations)
        {
            GL.Uniform1(locations[0], 7, new float[7] { Near, Far, Diff, Summ, Mul2, Factor0, Factor1 });
        }

        public const int SizeOf = sizeof(float) * 7;
        public static void ToBuffer(int stride, ref System.IntPtr offset, int divisor, params int[] bindIndex)
        {
            GL.EnableVertexAttribArray(bindIndex[0]);
            GL.VertexAttribPointer(bindIndex[0], 7, VertexAttribPointerType.Float, false, stride, offset);
            GL.VertexAttribDivisor(bindIndex[0], divisor);
            offset += SizeOf;
        }
    }
}
