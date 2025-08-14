
namespace Engine3D.OutPut.Uniform.Specific
{
    public class CUniformInterPolate : Generic.Float.CUniformFloat1
    {
        public float T0
        {
            get { return Data[0]; }
            set { Data[0] = value; Data[1] = 1.0f - Data[0]; }
        }
        public float T1
        {
            get { return Data[1]; }
            set { Data[1] = value; Data[0] = 1.0f - Data[1]; }
        }

        public CUniformInterPolate() : base(2)
        {
            Data[0] = 1.0f;
            Data[1] = 0.0f;
        }
    }
}
