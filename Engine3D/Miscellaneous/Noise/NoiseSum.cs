using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine3D.Noise
{
    public abstract class NoiseLayer
    {
        public abstract double Sample(double y, double c);
    }
    public class NoiseSum
    {
        private NoiseLayer[] Layers;

        public NoiseSum(NoiseLayer[] layers)
        {
            Layers = layers;
        }

        public double Sum(double y, double c)
        {
            double x = 0.0;
            for (int i = 0; i < Layers.Length; i++)
            {
                x += Layers[i].Sample(y, c);
            }
            return x;
        }
    }
}
