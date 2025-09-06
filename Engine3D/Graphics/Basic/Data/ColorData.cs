using OpenTK.Graphics.OpenGL4;

namespace Engine3D.Graphics.Basic.Data
{
    public struct ColorFData : IData
    {
        public const int Size = sizeof(float) * 3;

        float R;
        float G;
        float B;

        public ColorFData(float r, float g, float b)
        {
            R = r;
            G = g;
            B = b;
        }

        public void ToUniform(params int[] locations)
        {
            GL.Uniform3(locations[0], R, G, B);
        }

        public static void ToBuffer(int stride, ref System.IntPtr offset, int divisor, params int[] bindIndex)
        {
            GL.EnableVertexAttribArray(bindIndex[0]);
            GL.VertexAttribPointer(bindIndex[0], 3, VertexAttribPointerType.Float, false, stride, offset);
            GL.VertexAttribDivisor(bindIndex[0], divisor);
            offset += Size;
        }
    }
    public struct ColorUData : IData
    {
        public const int Size = sizeof(uint);

        uint RGB;

        public ColorUData(uint rgb)
        {
            RGB = rgb;
        }

        public void ToUniform(params int[] locations)
        {
            GL.Uniform1(locations[0], RGB);
        }

        public static void ToBuffer(int stride, ref System.IntPtr offset, int divisor, params int[] bindIndex)
        {
            GL.EnableVertexAttribArray(bindIndex[0]);
            GL.VertexAttribIPointer(bindIndex[0], 1, VertexAttribIntegerType.UnsignedInt, stride, offset);
            GL.VertexAttribDivisor(bindIndex[0], divisor);
            offset += Size;
        }
    }
}
