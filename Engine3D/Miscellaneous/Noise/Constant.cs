using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine3D.Noise
{
    public class Constant : NoiseLayer
    {
        private readonly double Val;

        public Constant(double val)
        {
            Val = val;
        }

        public override double Sample(double y, double c)
        {
            return Val;
        }

        public static Constant FromString(string[] str)
        {
            double val = double.Parse(str[0]);

            return new Constant(val);
        }
    }
}
