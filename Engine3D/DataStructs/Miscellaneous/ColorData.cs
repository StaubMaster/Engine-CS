using OpenTK.Graphics.OpenGL4;

namespace Engine3D.DataStructs
{
    public struct ColorFData : IData
    {
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

        public const int SizeOf = sizeof(float) * 3;
        public static void ToBuffer(int stride, ref System.IntPtr offset, int divisor, params int[] bindIndex)
        {
            GL.EnableVertexAttribArray(bindIndex[0]);
            GL.VertexAttribPointer(bindIndex[0], 3, VertexAttribPointerType.Float, false, stride, offset);
            GL.VertexAttribDivisor(bindIndex[0], divisor);
            offset += SizeOf;
        }
    }
    public struct ColorUData : IData
    {
        public uint RGB;

        public ColorUData(uint rgb)
        {
            RGB = rgb;
        }



        public void ToUniform(params int[] locations)
        {
            GL.Uniform1(locations[0], RGB);
        }

        public const int SizeOf = sizeof(uint);
        public static void ToBuffer(int stride, ref System.IntPtr offset, int divisor, params int[] bindIndex)
        {
            GL.EnableVertexAttribArray(bindIndex[0]);
            GL.VertexAttribIPointer(bindIndex[0], 1, VertexAttribIntegerType.UnsignedInt, stride, offset);
            GL.VertexAttribDivisor(bindIndex[0], divisor);
            offset += SizeOf;
        }
    }
}
