using OpenTK.Graphics.OpenGL4;

namespace Engine3D.Graphics.Basic.Data
{
    public struct ScreenRatio : IData
    {
        public const int Size = sizeof(float) * 4;

        private float SizeW;
        private float SizeH;

        private float RatioW;
        private float RatioH;

        public ScreenRatio(float w, float h)
        {
            SizeW = w;
            SizeH = h;

            RatioW = float.NaN;
            RatioH = float.NaN;

            CalcRatio();
        }
        private void CalcRatio()
        {
            if (SizeW == SizeH)
            {
                RatioW = 1.0f;
                RatioH = 1.0f;
            }
            else if (SizeW > SizeH)
            {
                RatioW = SizeH / SizeW;
                RatioH = 1.0f;
            }
            else if (SizeW < SizeH)
            {
                RatioW = 1.0f;
                RatioH = SizeW / SizeH;
            }
        }

        public void ToUniform(params int[] locations)
        {
            GL.Uniform2(locations[0], 2, new float[] { SizeW, SizeH, RatioW, RatioH });
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
