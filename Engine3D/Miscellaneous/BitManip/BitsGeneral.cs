using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine3D.BitManip
{
    static class BitsGeneral
    {
        //  could be turned into a class / struct

        public static uint Set(uint bits, uint shift, bool state)
        {
            uint bit = (uint)(1 << (int)shift);

            if (state)
            {
                bits |= bit;
            }
            else
            {
                bit = bit ^ 0xFFFFFFFF;
                bits &= bit;
            }

            return bits;
        }
        public static uint Toggle(uint bits, uint shift)
        {
            uint bit = (uint)(1 << (int)shift);

            bits ^= bit;

            return bits;
        }
        public static bool Get(uint bits, uint shift)
        {
            uint bit = (uint)(1 << (int)shift);

            return ((bits & bit) != 0);
        }

        public static uint RotateR(uint bits, int rot)
        {
            rot = rot & 31;
            return (bits >> rot) | (bits << (31 - rot));
        }
        public static uint RotateL(uint bits, int rot)
        {
            rot = rot & 31;
            return (bits << rot) | (bits >> (31 - rot));
        }

        public static uint Reverse(uint bits)
        {
            uint hi, lo;

            hi = bits & 0xFFFF0000;
            lo = bits & 0x0000FFFF;
            bits = (hi >> 16) | (lo << 16);

            hi = bits & 0xFF00FF00;
            lo = bits & 0x00FF00FF;
            bits = (hi >> 8) | (lo << 8);

            hi = bits & 0xF0F0F0F0;
            lo = bits & 0x0F0F0F0F;
            bits = (hi >> 4) | (lo << 4);

            hi = bits & 0xCCCCCCCC;
            lo = bits & 0x33333333;
            bits = (hi >> 2) | (lo << 2);

            hi = bits & 0xAAAAAAAA;
            lo = bits & 0x55555555;
            bits = (hi >> 1) | (lo << 1);

            return bits;
        }
    }
}
