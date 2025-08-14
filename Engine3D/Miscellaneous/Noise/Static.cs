using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine3D.Abstract3D;
using Engine3D.BitManip;

namespace Engine3D.Noise
{
    public class Static : NoiseLayer
    {
        private readonly uint Seed;
        private readonly uint Mask;
        private readonly double Scale;

        public Static(uint seed, uint mask, double scale = 1.0)
        {
            Seed = seed;
            Mask = mask;
            Scale = scale;
        }

        public uint generateNum(int y, int c)
        {
            string str = "";
            str += (char)(Seed << 24) & 0xFF;
            str += (char)(Seed << 16) & 0xFF;
            str += (char)(Seed << 8) & 0xFF;
            str += (char)(Seed << 0) & 0xFF;
            str += (char)(y << 24) & 0xFF;
            str += (char)(y << 16) & 0xFF;
            str += (char)(y << 8) & 0xFF;
            str += (char)(y << 0) & 0xFF;
            str += (char)(c << 24) & 0xFF;
            str += (char)(c << 16) & 0xFF;
            str += (char)(c << 8) & 0xFF;
            str += (char)(c << 0) & 0xFF;

            uint[] hash = SHA256.FromText(str);

            uint h = 0;
            for (int i = 0; i < hash.Length; i++) { h ^= hash[i];  }
            return h;
        }
        public bool generateBool(int y, int c)
        {
            return (generateNum(y, c) & Mask) == 0;
        }
        public override double Sample(double y, double c)
        {
            int _y, _c;
            _y = (int)y;
            _c = (int)c;

            if (generateBool(_y, _c))
                return Scale;
            return 0.0;
        }


        public static Static FromString(string[] str)
        {
            uint seed;
            uint mask;

            seed = uint.Parse(str[0].Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);
            mask = uint.Parse(str[1].Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);

            return new Static(seed, mask);
        }
    }
}
