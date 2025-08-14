
namespace Engine3D.OutPut.Uniform.Generic.UInt
{
    public abstract class AUniformUIntN : AUniformBase
    {
        public uint[] Data;
        protected int Size;
        protected int Count;

        public AUniformUIntN(int size, int count)
        {
            Size = size;
            Count = count;
            Data = new uint[Size * Count];
        }
    }
}
