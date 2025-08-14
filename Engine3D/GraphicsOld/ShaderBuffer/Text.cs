using System;
using System.Collections.Generic;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.GraphicsOld
{
    public class TextProgram : RenderProgram
    {
        public TextProgram(string name, string vert_file, string geom_file, string frag_file)
            : base(name, vert_file, geom_file, frag_file)
        {

        }

        public override void Create()
        {
            if (Program != -1) { return; }
            base.Create();
        }
        public override void Delete()
        {
            if (Program == -1) { return; }
            base.Delete();
        }
    }
    public class TextBuffers : RenderBuffers
    {
        private int Buffer_String;
        private int Buffer_Pallet;
        private int Number;

        public TextBuffers() : base()
        {

        }

        public override void Create()
        {
            if (Buffer_Array != -1) { return; }
            Create("Text");

            Buffer_String = GL.GenBuffer();
            Buffer_Pallet = GL.GenBuffer();
            Number = 0;

            string_list = new List<text_string>();
        }
        public override void Delete()
        {
            if (Buffer_Array == -1) { return; }
            Delete("Text");

            GL.DeleteBuffer(Buffer_String);
            GL.DeleteBuffer(Buffer_Pallet);

            string_list = null;
        }


        private struct character
        {
            float x;
            float y;
            uint off;

            uint col;
            uint chr;

            public character(float x, float y, uint col, byte chr, short off_x, short off_y)
            {
                this.x = x;
                this.y = y;
                this.col = col;

                uint off_x_u = (uint)(off_x);
                uint off_y_u = (uint)(off_y);
                off = (off_x_u << 16) | (off_y_u & 0xFFFF);

                this.chr = chr;
            }


            public const int OffOff = sizeof(float) + sizeof(float);
            public const int OffCol = sizeof(float) + sizeof(float) + sizeof(uint);
            public const int OffChr = sizeof(float) + sizeof(float) + sizeof(uint) + sizeof(uint);
            public const int Size = sizeof(float) + sizeof(float) + sizeof(uint) + sizeof(uint) + sizeof(uint);
        }
        private struct text_string
        {
            readonly float x;
            readonly float y;
            readonly uint col;
            readonly Func<int, string, short> LineStart;

            readonly string str;
            int idx;

            short off_x;
            short off_y;
            List<character> list;

            public text_string(float x, float y, uint col, bool rtl, string str)
            {
                this.x = x;
                this.y = y;
                this.col = col;
                LineStart = null;

                if (string.IsNullOrEmpty(str))
                    str = "";
                this.str = str;

                idx = 0;

                off_x = 0;
                off_y = 0;
                list = null;

                if (!rtl)
                    LineStart = LineStartL;
                else
                    LineStart = LineStartR;
            }
            public character[] Interpret()
            {
                off_x = LineStart(0, str);
                off_y = 0;
                list = new List<character>();

                byte b;
                byte control;

                idx = 0;
                while (idx < str.Length)
                {
                    control = Interpret_char(str[idx], out b);
                    if (control == 0)
                    {
                        list.Add(new character(x, y, col, b, off_x, off_y));
                        off_x++;
                    }
                    else if (control == 1)
                    {
                        off_x++;
                    }
                    else if (control == 2)
                    {
                        off_x += 4;
                    }
                    else if (control == 3)
                    {
                        off_y--;
                        off_x = LineStart(idx + 1, str);
                    }
                    idx++;
                }

                character[] arr = list.ToArray();
                list = null;
                return arr;
            }

            private static short LineStartL(int idx, string str)
            {
                return 0;
            }
            private static short LineStartR(int idx, string str)
            {
                short l = 0;
                while (++idx < str.Length)
                {
                    if (str[idx] == '\n')
                        return l;
                    l--;
                }
                return l;
            }
        }

        private List<text_string> string_list;
        public void Insert(float x, float y, uint col, bool rtl, string str)
        {
            string_list.Add(new text_string(x, y, col, rtl, str));
        }

        public enum Corner
        {
            VertMid = 0b0000,
            VertTop = 0b0001,
            VertBot = 0b0010,
            Vert    = 0b0011,

            HoriMid = 0b0000,
            HoriLef = 0b0100,
            HoriRig = 0b1000,
            Hori    = 0b1100,

            TopLef = VertTop | HoriLef,
            TopRig = VertTop | HoriRig,
            BotLef = VertBot | HoriLef,
            BotRig = VertBot | HoriRig,
        };

        /*
         * put all these as uniforms
         * figure out what they do
         */
        static float charNorm = 0.1f;
        static (float, float) charRatio = (0.5f, 1.0f);
        static (float, float) screenRatio = (0.5f, 1.0f);
        static float charScale = 0.0125f * 2;

        public void Insert(Corner corn, int x, int y, uint col, string str, bool rtl = false)
        {
            float xx, yy;
            xx = x * charNorm * charRatio.Item1 * screenRatio.Item1 * charScale * 24;
            yy = y * charNorm * charRatio.Item2 * screenRatio.Item2 * charScale * 24;
            //xx = x * 0.1f * 0.5f * 0.5f * 0.025f * 24;
            //yy = y * 0.1f * 1.0f * 1.0f * 0.025f * 24;

            if ((corn & Corner.Hori) == Corner.HoriLef)
            {
                //xx = 0.5f * 0.5f * 0.025f + xx;
                xx = charRatio.Item1 * screenRatio.Item1 * charScale + xx;
                xx = xx - 1;
            }
            if ((corn & Corner.Hori) == Corner.HoriRig)
            {
                //xx = 0.5f * 0.5f * 0.025f - xx;
                xx = charRatio.Item1 * screenRatio.Item1 * charScale - xx;
                xx = 1 - xx;
                rtl = !rtl;
            }

            if ((corn & Corner.Vert) == Corner.VertTop)
            {
                //yy = 1.0f * 1.0f * 0.025f + yy;
                yy = charRatio.Item2 * screenRatio.Item2 * charScale + yy;
                yy = 1 - yy;
            }
            if ((corn & Corner.Vert) == Corner.VertBot)
            {
                //yy = 1.0f * 1.0f * 0.025f - yy;
                yy = charRatio.Item2 * screenRatio.Item2 * charScale - yy;
                yy = yy - 1;
            }
            if ((corn & Corner.Vert) == Corner.VertMid)
            {
                yy = -yy;
            }

            string_list.Add(new text_string(xx, yy, col, rtl, str));
        }



        public void Fill_Strings()
        {
            List<character> character_list = new List<character>();

            for (int i = 0; i < string_list.Count; i++)
                character_list.AddRange(string_list[i].Interpret());
            string_list = new List<text_string>();

            character[] character_arr = character_list.ToArray();
            Number = character_arr.Length;


            GL.BindVertexArray(Buffer_Array);
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, Buffer_String);
            GL.BufferData(BufferTarget.ArrayBuffer, character_arr.Length * character.Size, character_arr, BufferUsageHint.StreamDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, character.Size, (IntPtr)0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribIPointer(1, 2, VertexAttribIntegerType.UnsignedInt, character.Size, (IntPtr)character.OffOff);

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribIPointer(2, 1, VertexAttribIntegerType.UnsignedInt, character.Size, (IntPtr)character.OffCol);

            GL.EnableVertexAttribArray(3);
            GL.VertexAttribIPointer(3, 1, VertexAttribIntegerType.UnsignedInt, character.Size, (IntPtr)character.OffChr);


            Fill_Pallets();
        }


        private const uint off_dig = 0;
        private const uint off_letU = off_dig + 10;
        private const uint off_letL = off_letU + 26;
        private const uint off_punc = off_letL + 26;
        private const uint off_arith = off_punc + 6;
        private const uint off_comp = off_arith + 4;
        private const uint off_brack = off_comp + 3;
        private static byte Interpret_char(in char c, out byte b)
        {
            b = 0xFF;

            if (c == ' ')
                return 1;
            else if (c == '\t')
                return 2;
            else if (c == '\n')
                return 3;
            else if (c == '\r')
                return 4;

            if (c >= '0' && c <= '9')
                b = (byte)((c - '0') + off_dig);
            else if (c >= 'A' && c <= 'Z')
                b = (byte)((c - 'A') + off_letU);
            else if (c >= 'a' && c <= 'z')
                b = (byte)((c - 'a') + off_letL);

            else if (c == '.')
                b = (byte)((0) + off_punc);
            else if (c == ':')
                b = (byte)((1) + off_punc);
            else if (c == ',')
                b = (byte)((2) + off_punc);
            else if (c == ';')
                b = (byte)((3) + off_punc);
            else if (c == '!')
                b = (byte)((4) + off_punc);
            else if (c == '?')
                b = (byte)((5) + off_punc);

            else if (c == '+')
                b = (byte)((0) + off_arith);
            else if (c == '-')
                b = (byte)((1) + off_arith);
            else if (c == '*')
                b = (byte)((2) + off_arith);
            else if (c == '/' || c == '\\')
                b = (byte)((3) + off_arith);

            else if (c == '=')
                b = (byte)((0) + off_comp);
            else if (c == '<')
                b = (byte)((1) + off_comp);
            else if (c == '>')
                b = (byte)((2) + off_comp);

            else if (c == '(')
                b = (byte)((0) + off_brack);
            else if (c == ')')
                b = (byte)((1) + off_brack);
            else if (c == '[')
                b = (byte)((2) + off_brack);
            else if (c == ']')
                b = (byte)((3) + off_brack);
            else if (c == '{')
                b = (byte)((4) + off_brack);
            else if (c == '}')
                b = (byte)((5) + off_brack);

            return 0;
        }
        public void Fill_Pallets()
        {
            (int, int) skip = (255, 0);
            (int, int) middle = (0, 0);

            (int, int) side_t = (0, +9);
            (int, int) side_b = (0, -9);
            (int, int) side_r = (+9, 0);
            (int, int) side_l = (-9, 0);

            (int, int) box1_bl = (-9, -9);
            (int, int) box1_br = (+9, -9);
            (int, int) box1_tl = (-9, +9);
            (int, int) box1_tr = (+9, +9);

            (int, int) circle1_bl = (-9, -6);
            (int, int) circle1_tl = (-9, +6);
            (int, int) circle1_tr = (+9, +6);
            (int, int) circle1_br = (+9, -6);

            (int, int) box2_bl = (-9, -4);
            (int, int) box2_tl = (-9, +4);
            (int, int) box2_tr = (+9, +4);
            (int, int) box2_br = (+9, -4);

            (int, int) small_t = (0, +4);
            (int, int) small_b = (0, -4);

            (int, int)[] none = new (int, int)[]
            {
                box1_bl, box1_br, box1_tr, box1_tl, box1_bl,
            };

            (int, int)[][] digit = new (int, int)[10][]
            {
                new (int, int)[]    //  0
                {
                    circle1_bl, circle1_tl, side_t, circle1_tr, circle1_br, side_b, circle1_bl, circle1_tr,
                },
                new (int, int)[]    //  1
                {
                    box2_tl, side_t, side_b, skip, box1_bl, box1_br,
                },
                new (int, int)[]    //  2
                {
                    box2_tl, side_t, circle1_tr, (+9, +2), box1_bl, box1_br,
                },
                new (int, int)[]    //  3
                {
                    box2_tl, side_t, box2_tr, middle, box2_br, side_b, box2_bl,
                },
                new (int, int)[]    //  4
                {
                    (-7, +9), side_l, side_r, skip, (+2, +9), (-2, -9),
                },
                new (int, int)[]    //  5
                {
                    box1_tr, box1_tl, (-9, +2), (+9, +2), circle1_br, side_b, circle1_bl,
                },
                new (int, int)[]    //  6
                {
                    circle1_tr, side_t, circle1_tl, circle1_bl, side_b, circle1_br, (+9, -2), (-9, +2),
                },
                new (int, int)[]    //  7
                {
                    box1_tl, box1_tr, box1_bl, skip, side_l, side_r,
                },
                new (int, int)[]    //  8
                {
                    box2_bl, box2_tr, side_t, box2_tl, box2_br, side_b, box2_bl,
                },
                new (int, int)[]    //  9
                {
                    circle1_bl, side_b, circle1_br, circle1_tr, side_t, circle1_tl, (-9, +2), (+9, -2),
                },
            };

            (int, int)[][] letterU = new (int, int)[26][]
            {
                new (int, int)[]    //  A
                {
                    box1_bl, side_t, box1_br,
                },
                new (int, int)[]    //  B
                {
                    middle, box2_tr, side_t, box1_tl, box1_bl, side_b, box2_br, middle,
                },
                new (int, int)[]    //  C
                {
                    box2_tr, side_t, box2_tl, box2_bl, side_b, box2_br,
                },
                new (int, int)[]    //  D
                {
                    box1_tl, box1_bl, side_b, circle1_br, circle1_tr, side_t, box1_tl,
                },
                new (int, int)[]    //  E
                {
                    box1_tr, box1_tl, box1_bl, box1_br, skip, side_l, middle,
                },
                new (int, int)[]    //  F
                {
                    box1_tr, box1_tl, box1_bl, skip, side_l, middle,
                },
                new (int, int)[]    //  G
                {
                    circle1_tr, side_t, circle1_tl, circle1_bl, side_b, circle1_br, side_r, middle,
                },
                new (int, int)[]    //  H
                {
                    box1_tl, box1_bl, skip, box1_tr, box1_br, skip, side_l, side_r,
                },
                new (int, int)[]    //  I
                {
                    box1_tl, box1_tr, skip, box1_bl, box1_br, skip, side_t, side_b,
                },
                new (int, int)[]    //  J
                {
                    box1_tl, box1_tr, box2_br, side_b, box2_bl,
                },
                new (int, int)[]    //  K
                {
                    box1_tl, box1_bl, skip, box1_tr, side_l, box1_br,
                },
                new (int, int)[]    //  L
                {
                    box1_tl, box1_bl, box1_br,
                },
                new (int, int)[]    //  M
                {
                    box1_bl, box1_tl, middle, box1_tr, box1_br,
                },
                new (int, int)[]    //  N
                {
                    box1_bl, box1_tl, box1_br, box1_tr,
                },
                new (int, int)[]    //  O
                {
                    side_b, circle1_bl, circle1_tl, side_t, circle1_tr, circle1_br, side_b,
                },
                new (int, int)[]    //  P
                {
                    box1_bl, box1_tl, side_t, box2_tr, middle, side_l,
                },
                new (int, int)[]    //  Q
                {
                    side_b, circle1_bl, circle1_tl, side_t, circle1_tr, circle1_br, side_b, box1_br,
                },
                new (int, int)[]    //  R
                {
                    box1_bl, box1_tl, side_t, box2_tr, middle, side_l, box1_br,
                },
                new (int, int)[]    //  S
                {
                    box2_tr, side_t, box2_tl, (-5, 0), (+5, 0), box2_br, side_b, box2_bl,
                },
                new (int, int)[]    //  T
                {
                    box1_tl, box1_tr, skip, side_t, side_b,
                },
                new (int, int)[]    //  U
                {
                    box1_tl, circle1_bl, side_b, circle1_br, box1_tr,
                },
                new (int, int)[]    //  V
                {
                    box1_tl, side_b, box1_tr,
                },
                new (int, int)[]    //  W
                {
                    box1_tl, box1_bl, middle, box1_br, box1_tr,
                },
                new (int, int)[]    //  X
                {
                    box1_tl, box1_br, skip, box1_tr, box1_bl,
                },
                new (int, int)[]    //  Y
                {
                    box1_tl, middle, box1_tr, skip, middle, side_b,
                },
                new (int, int)[]    //  Z
                {
                    box1_tl, box1_tr, box1_bl, box1_br,
                },
            };

            (int, int)[][] letterL = new (int, int)[26][]
            {
                new (int, int)[]    //  a
                {
                    box2_tr, box2_br, skip, side_r, small_t, side_l, small_b, side_r,
                },
                new (int, int)[]    //  b
                {
                    box1_tl, box2_bl, skip, side_l, small_t, side_r, small_b, side_l,
                },
                new (int, int)[]    //  c
                {
                    (+5, +2), small_t, side_l, small_b, (+5, -2),
                },
                new (int, int)[]    //  d
                {
                    box1_tr, box2_br, skip, side_r, small_t, side_l, small_b, side_r,
                },
                new (int, int)[]    //  e
                {
                    side_l, side_r, small_t, side_l, small_b, (+5, -2),
                },
                new (int, int)[]    //  f
                {
                    small_b, side_t, box2_tr, skip, side_l, side_r,
                },
                new (int, int)[]    //  g
                {
                    side_r, small_b, side_l, small_t, box2_tr, box2_br, side_b, box2_bl,
                },
                new (int, int)[]    //  h
                {
                    box1_tl, box2_bl, skip, side_l, small_t, side_r, box2_br,
                },
                new (int, int)[]    //  i
                {
                    small_t, small_b, skip, (-2, +7), (+2, +7), skip, (-2, -7), (+2, -7),
                },
                new (int, int)[]    //  j
                {
                    small_t, side_b, box2_bl , skip, (-2, +7), (+2, +7)
                },
                new (int, int)[]    //  k
                {
                    box1_tl, box2_bl, skip, box2_tr, side_l, box2_br,
                },
                new (int, int)[]    //  l
                {
                    box2_tl, side_t, side_b, box2_br,
                },
                new (int, int)[]    //  m
                {
                    box2_tl, box2_bl, skip, side_l, (-5, +4), small_b, (+5, +4), box2_br,
                },
                new (int, int)[]    //  n
                {
                    box2_tl, box2_bl, skip, side_l, small_t, side_r, box2_br,
                },
                new (int, int)[]    //  o
                {
                    side_l, small_t, side_r, small_b, side_l,
                },
                new (int, int)[]    //  p
                {
                    box2_tl, box1_bl, skip, side_l, small_t, side_r, small_b, side_l,
                },
                new (int, int)[]    //  q
                {
                    box2_tr, box1_br, skip, side_r, small_t, side_l, small_b, side_r,
                },
                new (int, int)[]    //  r
                {
                    box2_tl, box2_bl, skip, side_l, small_t, side_r,
                },
                new (int, int)[]    //  s
                {
                    (+9, +2), small_t, (-9, +2), (-5, 0), (+5, 0), (+9, -2), small_b, (-9, -2),
                },
                new (int, int)[]    //  t
                {
                    small_t, side_b, box2_br, skip, side_l, side_r,
                },
                new (int, int)[]    //  u
                {
                    box2_tl, side_l, small_b, side_r, skip, box2_tr, box2_br,
                },
                new (int, int)[]    //  v
                {
                    box2_tl, small_b, box2_tr,
                },
                new (int, int)[]    //  w
                {
                    box2_tl, (-5, -4), small_t, (+5, -4), box2_tr,
                },
                new (int, int)[]    //  x
                {
                    box2_tl, box2_br, skip, box2_tr, box2_bl,
                },
                new (int, int)[]    //  y
                {
                    box2_tl, middle, skip, box2_tr, middle, box2_bl,
                },
                new (int, int)[]    //  z
                {
                    box2_tl, box2_tr, box2_bl, box2_br,
                },
            };

            (int, int)[][] punctuation = new (int, int)[6][]
            {
                new (int, int)[]    //  .
                {
                    (-2, -7), (+2, -7),
                },
                new (int, int)[]    //  :
                {
                    (-2, +7), (+2, +7), skip, (-2, -7), (+2, -7),
                },
                new (int, int)[]    //  ,
                {
                    (0, -5), (0, -9),
                },
                new (int, int)[]    //  ;
                {
                    (-2, +7), (+2, +7), skip, (0, -5), (0, -9),
                },
                new (int, int)[]    //  !
                {
                    side_t, small_b, skip, (-2, -7), (+2, -7),
                },
                new (int, int)[]    //  ?
                {
                    circle1_tl, side_t, circle1_tr, middle, small_b, skip, (-2, -7), (+2, -7),
                },
            };

            (int, int)[][] arithmetic = new (int, int)[4][]
            {
                new (int, int)[]    //  +
                {
                    side_t, side_b, skip, side_l, side_r,
                },
                new (int, int)[]    //  -
                {
                    side_l, side_r,
                },
                new (int, int)[]    //  *
                {
                    box2_tl, box2_br, skip, box2_tr, box2_bl, skip, side_l, side_r,
                },
                new (int, int)[]    //  /
                {
                    box1_bl, box1_tr,
                },
            };

            (int, int)[][] compare = new (int, int)[3][]
            {
                new (int, int)[]    //  =
                {
                    box2_tl, box2_tr, skip, box2_bl, box2_br,
                },
                new (int, int)[]    //  <
                {
                    box2_tr, side_l, box2_br,
                },
                new (int, int)[]    //  >
                {
                    box2_tl, side_r, box2_bl,
                },
            };

            (int, int)[][] brackets = new (int, int)[6][]
            {
                new (int, int)[]    //  (
                {
                    box1_tr, side_t, box2_tl, box2_bl, side_b, box1_br,
                },
                new (int, int)[]    //  )
                {
                    box1_tl, side_t, box2_tr, box2_br, side_b, box1_bl,
                },
                new (int, int)[]    //  [
                {
                    box1_tr, side_t, side_b, box1_br,
                },
                new (int, int)[]    //  ]
                {
                    box1_tl, side_t, side_b, box1_bl,
                },
                new (int, int)[]    //  {
                {
                    box1_tr, side_t, small_t, side_l, small_b, side_b, box1_br,
                },
                new (int, int)[]    //  }
                {
                    box1_tl, side_t, small_t, side_r, small_b, side_b, box1_bl,
                },
            };

            (int, int)[][][] symbol_arr_arr = new (int, int)[][][]
            {
                digit,
                letterU,
                letterL,
                punctuation,
                arithmetic,
                compare,
                brackets,
            };
            int char_arr_len = 1;
            for (int j = 0; j < symbol_arr_arr.Length; j++)
                char_arr_len += symbol_arr_arr[j].Length;

            (int, int)[][] char_arr = new (int, int)[char_arr_len][];
            uint idx = 0;
            char_arr[0] = none;
            for (int j = 0; j < symbol_arr_arr.Length; j++)
            {
                for (int i = 0; i < symbol_arr_arr[j].Length; i++)
                    char_arr[++idx] = symbol_arr_arr[j][i];
            }

            int[,] data = new int[char_arr.Length, 16];
            for (int i = 0; i < char_arr.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (j < char_arr[i].Length)
                    {
                        data[i, (j * 2) + 0] = char_arr[i][j].Item1;
                        data[i, (j * 2) + 1] = char_arr[i][j].Item2;
                    }
                    else
                    {
                        data[i, (j * 2) + 0] = 255;
                        data[i, (j * 2) + 1] = 255;
                    }
                }
            }

            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, Buffer_Pallet);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, data.Length * sizeof(int), data, BufferUsageHint.StaticDraw);
        }

        public override void Draw()
        {
            GL.BindVertexArray(Buffer_Array);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, Buffer_Pallet);

            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace);
            GL.DrawArrays(PrimitiveType.Points, 0, Number);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
        }
    }
}
