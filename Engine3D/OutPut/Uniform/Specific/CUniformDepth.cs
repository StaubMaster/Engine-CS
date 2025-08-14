
namespace Engine3D.OutPut.Uniform.Specific
{
    public class CUniformDepth : Generic.Float.CUniformFloat1
    {
        public float Near
        {
            get { return Data[0]; }
            set { Calc(value, Data[1]); }
        }
        public float Far
        {
            get { return Data[1]; }
            set { Calc(Data[0], value); }
        }

        public CUniformDepth(float near, float far) : base(7)
        {
            Calc(near, far);
        }
        private void Calc(float near, float far)
        {
            Data[0] = near;
            Data[1] = far;

            Data[2] = Data[1] - Data[0];
            Data[3] = Data[1] + Data[0];
            Data[4] = Data[1] * Data[0] * 2;

            Data[5] = Data[3] / Data[2];
            Data[6] = Data[4] / Data[2];
        }

        public override void Uniform(int uni)
        {
            base.Uniform(uni);
        }

        public override string ToString()
        {
            string str = "";

            for (int i = 0; i < Data.Length; i++)
            {
                str += "[" + i + "]" + Data[i] + "\n";
            }

            return str;
        }
    }
}
