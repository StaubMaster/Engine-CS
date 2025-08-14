using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine3D.BitManip
{
    public static class SHA256
    {
        private static uint rotR(uint num, int rot)
        {
            rot = rot & 31;
            return (num >> rot) | (num << (32 - rot));
        }
        private static uint rotL(uint num, int rot)
        {
            rot = rot & 31;
            return (num << rot) | (num >> (32 - rot));
        }



        private static readonly uint[] k = new uint[64]
        {
            0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5, 0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
            0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3, 0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
            0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc, 0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
            0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7, 0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
            0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13, 0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
            0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3, 0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
            0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5, 0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
            0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208, 0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2,
        };
        private static readonly uint[] hex = new uint[8]
        {
            0x6a09e667,
            0xbb67ae85,
            0x3c6ef372,
            0xa54ff53a,
            0x510e527f,
            0x9b05688c,
            0x1f83d9ab,
            0x5be0cd19,
        };

        private static void Chunk(uint[] hex, byte[] chunk)
        {
            uint[] wrd = new uint[64];
            for (uint i = 0; i < 16; i++)
            {
                uint j = i * 4;
                wrd[i] |= ((uint)chunk[j + 0]) << 24;
                wrd[i] |= ((uint)chunk[j + 1]) << 16;
                wrd[i] |= ((uint)chunk[j + 2]) << 8;
                wrd[i] |= ((uint)chunk[j + 3]) << 0;
            }

            uint s0, s1;

            for (uint i = 16; i < 64; i++)
            {
                s0 = wrd[i - 15];
                s1 = wrd[i - 2];
                s0 = rotR(s0, 0x7) ^ rotL(s0, 0xE) ^ (s0 >> 0x3);
                s1 = rotL(s1, 0xF) ^ rotL(s1, 0xD) ^ (s1 >> 0xA);
                wrd[i] = wrd[i - 16] + s0 + wrd[i - 7] + s1;
            }

            uint a = hex[0];
            uint b = hex[1];
            uint c = hex[2];
            uint d = hex[3];
            uint e = hex[4];
            uint f = hex[5];
            uint g = hex[6];
            uint h = hex[7];

            uint tmp1, tmp2, maj, ch;
            for (uint i = 0; i < 64; i++)
            {
                s0 = rotR(a, 0x2) ^ rotR(a, 0xD) ^ rotL(a, 0xA);
                s1 = rotR(e, 0x6) ^ rotR(e, 0xB) ^ rotL(e, 0x7);

                maj = (a & b) ^ (a & c) ^ (b & c);
                ch = (e & f) ^ ((~e) & g);

                tmp1 = h + s1 + ch + k[i] + wrd[i];
                tmp2 = s0 + maj;

                h = g;
                g = f;
                f = e;
                e = d + tmp1;
                d = c;
                c = b;
                b = a;
                a = tmp1 + tmp2;
            }

            hex[0] = hex[0] + a;
            hex[1] = hex[1] + b;
            hex[2] = hex[2] + c;
            hex[3] = hex[3] + d;
            hex[4] = hex[4] + e;
            hex[5] = hex[5] + f;
            hex[6] = hex[6] + g;
            hex[7] = hex[7] + h;
        }

        public static uint[] FromText(string text)
        {
            uint[] hex = new uint[8];
            for (int i = 0; i < 8; i++) { hex[i] = SHA256.hex[i]; }
            byte[] chunk = new byte[64];

            int offset = 0;
            int rel_len = text.Length - offset;

            while (rel_len > 64)
            {
                for (int i = 0; i < 64; i++) { chunk[i] = (byte)text[i + offset]; }
                Chunk(hex, chunk);
                offset += 64;
                rel_len = text.Length - offset;
            }

            for (int i = 0; i < rel_len; i++) { chunk[i] = (byte)text[i + offset]; }
            chunk[rel_len] = 0x80;
            for (int i = rel_len + 1; i < 60; i++) { chunk[i] = 0; }

            ulong length = (ulong)(text.Length * 8);
            chunk[60] = (byte)((length >> 24) & 0xFF);
            chunk[61] = (byte)((length >> 16) & 0xFF);
            chunk[62] = (byte)((length >> 8) & 0xFF);
            chunk[63] = (byte)((length >> 0) & 0xFF);

            Chunk(hex, chunk);

            return hex;
        }

        public static void Test()
        {
            //uint[] hash = FromText("A");
            //ConsoleLog.Log("      : 559AEAD0 8264D579 5D390971 8CDD05AB D49572E8 4FE55590 EEF31A88 A08FDFFD");
            uint[] hash = FromText("559AEAD0 8264D579 5D390971 8CDD05AB D49572E8 4FE55590 EEF31A88 A08FDFFD 00755640 C85026DA F228FFF4 B9A7ACDE AF032617 BF2647F0 52301676 41B9E845");
            ConsoleLog.Log("      : D9610886 1205FA7D 7E506F5C F03C9C78 F0D7DF4F 30B93EBA DF29343D 0C45D257");

            string hashStr = "";
            for (int i = 0; i < 8; i++)
            {
                hashStr += hash[i].ToString("X8") + " ";
            }
            ConsoleLog.Log("SHA256: " + hashStr);
        }
    }
}
