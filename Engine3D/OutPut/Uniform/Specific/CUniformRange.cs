
namespace Engine3D.OutPut.Uniform.Specific
{
    public class CUniformRange : Generic.Float.CUniformFloat1
    {
        public float Min
        {
            get { return Data[0]; }
            set
            {
                Data[0] = value;
                Data[1] = Data[2] - Data[0];
            }
        }
        public float Max
        {
            get { return Data[2]; }
            set
            {
                Data[2] = value;
                Data[1] = Data[2] - Data[0];
            }
        }
        public float Range
        {
            get { return Data[1]; }
        }

        public CUniformRange(float min, float max) : base(3)
        {
            Data[0] = min;
            Data[1] = max - min;
            Data[2] = max;
        }
    }
}
