using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine3D.Abstract3D
{
    public static class Grid2D
    {
        public static void FuncCircle(double y, double c, double rad, Action<int, int, double, double> func)
        {
            int minY, minC;
            minY = (int)Math.Floor(y - rad);
            minC = (int)Math.Floor(c - rad);

            int maxY, maxC;
            maxY = (int)Math.Ceiling(y + rad);
            maxC = (int)Math.Ceiling(c + rad);

            double diffY, diffC, dist, perc;
            for (int _c = minC; _c <= maxC; _c++)
            {
                diffC = c - _c;
                diffC = diffC * diffC;
                for (int _y = minY; _y <= maxY; _y++)
                {
                    diffY = y - _y;
                    diffY = diffY * diffY;

                    dist = Math.Sqrt(diffY + diffC);
                    perc = dist / rad;

                    func(_y, _c, dist, perc);
                }
            }
        }
        public static void FuncCircle<T>(double y, double c, double rad, Action<int, int, double, double, T> func, T t)
        {
            int minY, minC;
            minY = (int)Math.Floor(y - rad);
            minC = (int)Math.Floor(c - rad);

            int maxY, maxC;
            maxY = (int)Math.Ceiling(y + rad);
            maxC = (int)Math.Ceiling(c + rad);

            double diffY, diffC, dist, perc;
            for (int _c = minC; _c <= maxC; _c++)
            {
                diffC = c - _c;
                diffC = diffC * diffC;
                for (int _y = minY; _y <= maxY; _y++)
                {
                    diffY = y - _y;
                    diffY = diffY * diffY;

                    dist = Math.Sqrt(diffY + diffC);
                    perc = dist / rad;

                    func(_y, _c, dist, perc, t);
                }
            }
        }
        public static void FuncCircle<T>(double y, double c, double radMin, double radMax, Action<int, int, double, double, T> func, T t)
        {
            double radDiff = radMax - radMin;

            void distPerc(int _y, int _c, double dist, double perc, T t)
            {
                perc = (radMax - dist) / radDiff;
                func(_y, _c, dist, perc, t);
            }

            FuncCircle(y, c, Math.Max(radMin, radMax), distPerc, t);
        }
    }
}
