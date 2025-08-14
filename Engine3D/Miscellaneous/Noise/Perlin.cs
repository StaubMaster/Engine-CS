using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine3D.Noise
{
    public class Perlin : NoiseLayer
    {
        private struct Perlin_P
        {
            public double Y;
            public double C;

            public Perlin_P(double y, double c)
            {
                Y = y;
                C = c;
            }
            public static Perlin_P normal(double y, double c)
            {
                double l;
                l = y * y + c * c;
                l = 1.0 / Math.Sqrt(l);

                return new Perlin_P(y * l, c * l);
            }

            public static double operator %(Perlin_P a, Perlin_P b)
            {
                return a.Y * b.Y + a.C * b.C;
            }

            public static double lerp(double w0, double w1, double t)
            {
                return w0 * (1.0 - t) + w1 * t;
            }
        }

        private const int Len = 4;
        private Perlin_P[,] Perlin_Array;

        private double zoom;
        private double scale;
        private int zoom_shift;
        private int scale_shift;
        public void change_shift(int zoom_shift, int scale_shift)
        {
            this.zoom_shift = zoom_shift;
            this.scale_shift = scale_shift;
            zoom = 1.0 / (1 << zoom_shift);
            scale = 1.0 * (1 << scale_shift);
        }
        public void shifts(out int zoom_shift, out int scale_shift)
        {
            zoom_shift = this.zoom_shift;
            scale_shift = this.scale_shift;
        }



        public Perlin(uint seed)
        {
            Perlin_Array = new Perlin_P[Len, Len];

            double sqr2, kY, kC;
            sqr2 = Math.Sqrt(2);
            for (int c = 0; c < Len; c++)
            {
                for (int y = 0; y < Len; y++)
                {
                    kY = (seed & 0b01) != 0 ? +sqr2 : -sqr2;
                    kC = (seed & 0b10) != 0 ? +sqr2 : -sqr2;
                    Perlin_Array[y, c] = new Perlin_P(kY, kC);
                    seed >>= 2;
                }
            }
        }
        public Perlin(uint seed, int zoom_shift, int scale_shift)
        {
            Perlin_Array = new Perlin_P[Len, Len];

            double sqr2, kY, kC;
            sqr2 = Math.Sqrt(2);
            for (int c = 0; c < Len; c++)
            {
                for (int y = 0; y < Len; y++)
                {
                    kY = (seed & 0b01) != 0 ? +sqr2 : -sqr2;
                    kC = (seed & 0b10) != 0 ? +sqr2 : -sqr2;
                    Perlin_Array[y, c] = new Perlin_P(kY, kC);
                    seed >>= 2;
                }
            }

            this.zoom_shift = zoom_shift;
            this.scale_shift = scale_shift;
            zoom = 1.0 / (1 << zoom_shift);
            scale = 1.0 * (1 << scale_shift);
        }
        public Perlin(uint seed, double zoom, double scale)
        {
            Perlin_Array = new Perlin_P[Len, Len];

            double sqr2, kY, kC;
            sqr2 = Math.Sqrt(2);
            for (int c = 0; c < Len; c++)
            {
                for (int y = 0; y < Len; y++)
                {
                    kY = (seed & 0b01) != 0 ? +sqr2 : -sqr2;
                    kC = (seed & 0b10) != 0 ? +sqr2 : -sqr2;
                    Perlin_Array[y, c] = new Perlin_P(kY, kC);
                    seed >>= 2;
                }
            }

            this.zoom = zoom;
            this.scale = scale;
        }



        private static readonly double RangeMultiplyer = 1.0 / Math.Sqrt(625.0 / 512.0);
        public double generate(double y, double c)
        {
            int floorY, floorC;
            floorY = (int)Math.Floor(y);
            floorC = (int)Math.Floor(c);

            int Y_1__, C_1__;
            Y_1__ = ((floorY % Len) + Len) % Len;
            C_1__ = ((floorC % Len) + Len) % Len;

            int Y__2_, C__2_;
            Y__2_ = (Y_1__ + 1) % Len;
            C__2_ = (C_1__ + 1) % Len;

            double relY, relC;
            relY = y - floorY;
            relC = c - floorC;

            double[] skal = new double[4];
            skal[0b00] = Perlin_Array[Y_1__, C_1__] % new Perlin_P(relY - 0, relC - 0);
            skal[0b01] = Perlin_Array[Y_1__, C__2_] % new Perlin_P(relY - 0, relC - 1);
            skal[0b10] = Perlin_Array[Y__2_, C_1__] % new Perlin_P(relY - 1, relC - 0);
            skal[0b11] = Perlin_Array[Y__2_, C__2_] % new Perlin_P(relY - 1, relC - 1);
            //                      2,8284271247461903
            //  sqrt( 8   /   1 ) = 2.82842712475
            //  sqrt( 16  /   2 ) = 2.82842712475
            //  sqrt( 4^2 / 2^1 ) = 2.82842712475

            double lerp1, lerp2;
            lerp1 = Perlin_P.lerp(skal[0b00], skal[0b01], relC);
            lerp2 = Perlin_P.lerp(skal[0b10], skal[0b11], relC);
            //                       2,121320343559643
            //  sqrt(   9  /   2 ) = 2.12132034356
            //  sqrt( 144  /  32 ) = 2.12132034356
            //  sqrt( 12^2 / 2^5 ) = 2.12132034356

            return Perlin_P.lerp(lerp1, lerp2, relY) * RangeMultiplyer;
            //                       1,1048543456039805
            //  sqrt( 625  / 512 ) = 1.1048543456
            //  sqrt( 25^2 / 2^9 ) = 1.1048543456
        }
        public override double Sample(double y, double c)
        {
            return generate(y * zoom - 0.5, c * zoom - 0.5) * scale;
        }

        public static Perlin FromString(string[] str)
        {
            uint seed;
            int zoom;
            int scale;

            seed = uint.Parse(str[0].Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);
            zoom = int.Parse(str[1]);
            scale = int.Parse(str[2]);

            return new Perlin(seed, zoom, scale);
        }

        public class Layers
        {
            private Perlin[] layer;

            public Layers((uint, int, int)[] data)
            {
                layer = new Perlin[data.Length];
                for (int i = 0; i < data.Length; i++)
                    layer[i] = new Perlin(data[i].Item1, data[i].Item2, data[i].Item3);
            }
            public Layers(Perlin[] layer)
            {
               this.layer = layer;
            }

            public double Sum(double y, double c)
            {
                double h = 0.0;
                for (int i = 0; i < layer.Length; i++)
                    h += layer[i].Sample(y, c);
                return h;
            }
        }
    }
}
