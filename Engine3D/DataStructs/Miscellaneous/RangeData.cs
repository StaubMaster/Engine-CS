using OpenTK.Graphics.OpenGL4;

namespace Engine3D.DataStructs
{
    /*
     * some of these arent exclusivly Graphics related
     * and if I want Point3D and such to also be IData
     * then this structure dosent really work
     */
    public struct RangeData : IData
    {
        public float Min;
        public float Len;
        public float Max;

        public RangeData(float min, float max)
        {
            Min = min;
            Len = max - min;
            Max = max;
        }

        public void ChangeMin(float min)
        {
            Min = min;
            Len = Max - min;
        }
        public void ChangeMax(float max)
        {
            Len = max - Min;
            Max = max;
        }

        public void ToUniform(params int[] locations)
        {
            GL.Uniform1(locations[0], 3, new float[3] { Min, Len, Max });
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
}
