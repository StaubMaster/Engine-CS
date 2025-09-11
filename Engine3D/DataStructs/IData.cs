
namespace Engine3D.DataStructs
{
    public interface IData
    {
        public const int SizeOf = 0;

        public void ToUniform(params int[] locations);
    }
}
