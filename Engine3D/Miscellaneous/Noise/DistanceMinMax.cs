using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine3D.Abstract3D;

namespace Engine3D.Noise
{
    public class DistanceMinMax : NoiseLayer
    {
        private readonly Point3D Point;
        private readonly double Scale;
        private readonly double Shift;
        private readonly double Min;
        private readonly double Max;

        public DistanceMinMax(Point3D point, double scale = 1.0, double shift = 0.0, double min = double.NaN, double max = double.NaN)
        {
            Point = point;
            Scale = scale;
            Shift = shift;
            Min = min;
            Max = max;
        }

        public override double Sample(double y, double c)
        {
            Point3D p = new Point3D((float)y, 0, (float)c);
            double h = (p - Point).Len * Scale + Shift;

            if (h < Min)
                return Min;
            if (h > Max)
                return Max;
            return h;
        }

        public static DistanceMinMax FromString(string[] str)
        {
            Point3D point = new Point3D(
                float.Parse(str[0]),
                float.Parse(str[1]),
                float.Parse(str[2]));
            if (str.Length == 3)
                return new DistanceMinMax(point);

            float scale = float.Parse(str[3]);
            if (str.Length == 4)
                return new DistanceMinMax(point, scale);

            float shift = float.Parse(str[4]);
            if (str.Length == 5)
                return new DistanceMinMax(point, scale, shift);

            float min = float.Parse(str[5]);
            if (str.Length == 6)
                return new DistanceMinMax(point, scale, shift, min);

            float max = float.Parse(str[6]);
            if (str.Length == 7)
                return new DistanceMinMax(point, scale, shift, min, max);

            return null;
        }
    }
}
