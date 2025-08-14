
namespace Engine3D.OutPut.Uniform.Generic.Float
{
    public abstract class AUniformFloatN : AUniformBase
    {
        public float[] Data;
        protected int Size;
        protected int Count;

        public AUniformFloatN(int size, int count)
        {
            Size = size;
            Count = count;
            Data = new float[Size * Count];
        }
    }
}
