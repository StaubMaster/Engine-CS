using System;
using System.Collections.Generic;

using OpenTK.Graphics.OpenGL4;

using Engine3D.Abstract2D;
using Engine3D.Graphics.Display;
using Engine3D.Graphics.Shader;

namespace Engine3D.Graphics
{
    public class TextBuffer : BaseBuffer
    {
        public TextSize Default_TextSize;

        private TextCharacterPalletIndex[] CharacterIndexMap;

        private int Buffer_Pallet;
        private int Buffer_String;
        private int Count;

        public TextBuffer() : base()
        {
            Buffer_Pallet = GL.GenBuffer();
            Buffer_String = GL.GenBuffer();
            Count = 0;

            StringDataList = new List<StringData>();

            Default_TextSize = new TextSize(16f, 1.0f, 4f);
        }
        ~TextBuffer()
        {
            Use();
            GL.DeleteBuffer(Buffer_Pallet);
            GL.DeleteBuffer(Buffer_String);
            StringDataList = null;
        }

        private struct StringData
        {
            private readonly UIGridPosition Position;
            private readonly TextSize Size;
            private readonly UIGridDirection Direction;

            private readonly uint Color;
            private readonly string Text;

            public StringData(UIGridPosition pos, TextSize size, UIGridDirection dir, uint color, string text)
            {
                Position = pos;
                Size = size;
                Direction = dir;

                Color = color;
                Text = text;
            }

            private float TextHoriInit(int idx)
            {
                int count = 0;
                for (int i = idx + 1; i < Text.Length; i++)
                {
                    if (Text[i] == '\n') { break; }
                    count++;
                }
                count--;

                float off = 0.0f;
                UIGridDirection hori = Direction & UIGridDirection.Hori;
                if (hori == UIGridDirection.HoriC) { off = count * 0.5f; }
                if (hori == UIGridDirection.HoriL) { off = count; }
                if (hori == UIGridDirection.HoriR) { off = 0.0f; }
                return Position.Offset.X - off;
            }
            private float TextVertInit()
            {
                int count = 0;
                for (int i = 0; i < Text.Length; i++)
                {
                    if (Text[i] == '\n') { count++; }
                }

                float off = 0.0f;
                UIGridDirection vert = Direction & UIGridDirection.Vert;
                if (vert == UIGridDirection.VertC) { off = count * 0.5f; }
                if (vert == UIGridDirection.VertU) { off = count; }
                if (vert == UIGridDirection.VertD) { off = 0.0f; }
                return Position.Offset.Y + off;
            }

            public CharacterBufferData[] Interpret(TextCharacterPalletIndex[] indexMap)
            {
                CharacterBufferData data = new CharacterBufferData();

                data.AnchorX = Position.Normal.X;
                data.AnchorY = Position.Normal.Y;

                data.PositionX = Position.Pixel.X;
                data.PositionY = Position.Pixel.Y;

                data.OffsetX = TextHoriInit(-1);
                data.OffsetY = TextVertInit();

                data.Height = Size.Size.Size.Y;
                data.Padding = Size.Size.Padding;
                data.Thick = Size.Thick;

                data.Color = Color;

                List<CharacterBufferData> list = new List<CharacterBufferData>();

                for (int idx = 0; idx < Text.Length; idx++)
                {
                    TextCharacterPallet.CharControl control = TextCharacterPallet.Interpret_Control(Text[idx]);
                    if (control == TextCharacterPallet.CharControl.None)
                    {
                        data.Character = TextCharacterPalletIndex.Interpret(indexMap, Text[idx]);
                        list.Add(data);
                        data.OffsetX += 1;
                    }
                    else if (control == TextCharacterPallet.CharControl.Space)
                    {
                        data.OffsetX += 1;
                    }
                    else if (control == TextCharacterPallet.CharControl.Tab)
                    {
                        data.OffsetY += 4;
                    }
                    else if (control == TextCharacterPallet.CharControl.NewLine)
                    {
                        data.OffsetX = TextHoriInit(idx);
                        data.OffsetY -= 1;
                    }
                }

                return list.ToArray();
            }
        }
        private List<StringData> StringDataList;



        public void Insert(Point2D anchor, Point2D pixel, UIGridDirection dir, (float, float) charOffset, uint color, TextSize size, string text)
        {
            UIGridPosition orient = new UIGridPosition(anchor, pixel, new Point2D(charOffset.Item1, charOffset.Item2));

            StringDataList.Add(new StringData(
                orient,
                size,
                dir,
                color,
                text));
        }
        public void Insert(UIGridPosition pos, TextSize size, UIGridDirection dir, uint color, string text)
        {
            StringDataList.Add(new StringData(
                pos,
                size,
                dir,
                color,
                text));
        }


        public void InsertTL((float, float) offset, TextSize textSize, uint color, string text)
        {
            Insert(new UIGridPosition(UIAnchor.TL(), new Point2D(0, 0), new Point2D(offset.Item1, offset.Item2) + UICorner.TL()),
                textSize, UIGridDirection.DiagDR, color, text);
        }
        public void InsertTR((float, float) offset, TextSize textSize, uint color, string text)
        {
            Insert(new UIGridPosition(UIAnchor.TR(), new Point2D(0, 0), new Point2D(offset.Item1, offset.Item2) + UICorner.TR()),
                textSize, UIGridDirection.DiagDL, color, text);
        }
        public void InsertBR((float, float) offset, TextSize textSize, uint color, string text)
        {
            Insert(new UIGridPosition(UIAnchor.BR(), new Point2D(0, 0), new Point2D(offset.Item1, offset.Item2) + UICorner.BR()),
                textSize, UIGridDirection.DiagUL, color, text);
        }
        public void InsertBL((float, float) offset, TextSize textSize, uint color, string text)
        {
            Insert(new UIGridPosition(UIAnchor.BL(), new Point2D(0, 0), new Point2D(offset.Item1, offset.Item2) + UICorner.BL()),
                textSize, UIGridDirection.DiagUR, color, text);
        }



        private struct CharacterBufferData
        {
            public const int Offset_Character = 0;
            public uint Character;
            public const int Offset_Anchor = Offset_Character + sizeof(uint);
            public float AnchorX;
            public float AnchorY;
            public const int Offset_Position = Offset_Anchor + sizeof(float) + sizeof(float);
            public float PositionX;
            public float PositionY;
            public const int Offset_Offset = Offset_Position + sizeof(float) + sizeof(float);
            public float OffsetX;
            public float OffsetY;
            public const int Offset_Height = Offset_Offset + sizeof(float) + sizeof(float);
            public float Height;
            public const int Offset_Padding = Offset_Height + sizeof(float);
            public float Padding;
            public const int Offset_Thick = Offset_Padding + sizeof(float);
            public float Thick;
            public const int Offset_Color = Offset_Thick + sizeof(float);
            public uint Color;
            public const int Size = Offset_Color + sizeof(uint);
        }
        private CharacterBufferData[] Interpret_String()
        {
            List<CharacterBufferData> character_list = new List<CharacterBufferData>();
            for (int i = 0; i < StringDataList.Count; i++)
            {
                character_list.AddRange(StringDataList[i].Interpret(CharacterIndexMap));
            }
            return character_list.ToArray();
        }
        public void Bind_Strings()
        {
            CharacterBufferData[] character_arr = Interpret_String();
            StringDataList.Clear();

            Count = character_arr.Length;
            int Size = CharacterBufferData.Size;

            Use();

            GL.BindBuffer(BufferTarget.ArrayBuffer, Buffer_String);
            GL.BufferData(BufferTarget.ArrayBuffer, Count * Size, character_arr, BufferUsageHint.StreamDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribIPointer(0, 1, VertexAttribIntegerType.UnsignedInt, Size, (IntPtr)CharacterBufferData.Offset_Character);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Size, (IntPtr)CharacterBufferData.Offset_Anchor);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Size, (IntPtr)CharacterBufferData.Offset_Position);
            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 2, VertexAttribPointerType.Float, false, Size, (IntPtr)CharacterBufferData.Offset_Offset);

            GL.EnableVertexAttribArray(4);
            GL.VertexAttribPointer(4, 1, VertexAttribPointerType.Float, false, Size, (IntPtr)CharacterBufferData.Offset_Height);
            GL.EnableVertexAttribArray(5);
            GL.VertexAttribPointer(5, 1, VertexAttribPointerType.Float, false, Size, (IntPtr)CharacterBufferData.Offset_Padding);
            GL.EnableVertexAttribArray(6);
            GL.VertexAttribPointer(6, 1, VertexAttribPointerType.Float, false, Size, (IntPtr)CharacterBufferData.Offset_Thick);

            GL.EnableVertexAttribArray(7);
            GL.VertexAttribIPointer(7, 1, VertexAttribIntegerType.UnsignedInt, Size, (IntPtr)CharacterBufferData.Offset_Color);

            Telematry?.Bind(Count, Size);
        }



        public void Bind_Pallets()
        {
            //TextCharacterPallet[] pallets = TextCharacterPallet.Init_Pallets();
            TextCharacterPallet[] pallets = TextCharacterPallet.Init_Pallets(out CharacterIndexMap);
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, Buffer_Pallet);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, pallets.Length * TextCharacterPallet.Size, pallets, BufferUsageHint.StaticDraw);
        }

        public void Test_Char()
        {
            Insert(
                new UIGridPosition(UIAnchor.MM(), new Point2D(0, 0), new Point2D(0, 0)),
                new TextSize(400f, 50f, 50f), UIGridDirection.DiagCC,
                0xFFFFFF, "ZOXIB");
        }
        public void Test_Characters()
        {
            Point2D anch = UIAnchor.MM();
            Point2D pos = new Point2D(0, 0);
            UIGridDirection dir = UIGridDirection.DiagCC;
            TextSize size = new TextSize(80f, 10f, 20f);

            Insert(new UIGridPosition(anch, pos, new Point2D(+0.0f, -0.0f + +4)), size, dir, 0x00FF00, "ABCDEFGHIJKLM");
            Insert(new UIGridPosition(anch, pos, new Point2D(+0.0f, -0.0f + +3)), size, dir, 0x00FF00, "NOPQRSTUVWXYZ");
            Insert(new UIGridPosition(anch, pos, new Point2D(+0.0f, -0.0f + +2)), size, dir, 0x0000FF, "abcdefghijklm");
            Insert(new UIGridPosition(anch, pos, new Point2D(+0.0f, -0.0f + +1)), size, dir, 0x0000FF, "nopqrstuvwxyz");
            Insert(new UIGridPosition(anch, pos, new Point2D(+0.0f, -0.0f +  0)), size, dir, 0xFF0000, "0123456789+-*/");
            Insert(new UIGridPosition(anch, pos, new Point2D(+0.0f, -0.0f + -1)), size, dir, 0x00FF00, ".:,;!?");
            Insert(new UIGridPosition(anch, pos, new Point2D(+0.0f, -0.0f + -2)), size, dir, 0xFF00FF, "<>=\n<>=");
            Insert(new UIGridPosition(anch, pos, new Point2D(+0.0f, -0.0f + -4)), size, dir, 0xFFFF00, "()[]{}");

            //Insert(new TextOrientation(new Normal0Point(0.0f, 0.5f), TextDirection.DiagRD, (+0.5f, -0.5f)), Default_TextSize, 0xFF0000, "0123456789+-*/");
            //Insert(new TextOrientation(new Normal0Point(0.0f, 0.5f), TextDirection.DiagRD, (+0.5f, -1.5f)), Default_TextSize, 0x00FF00, "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            //Insert(new TextOrientation(new Normal0Point(0.0f, 0.5f), TextDirection.DiagRD, (+0.5f, -2.5f)), Default_TextSize, 0x0000FF, "abcdefghijklmnopqrstuvwxyz");
            //Insert(new TextOrientation(new Normal0Point(0.0f, 0.5f), TextDirection.DiagRD, (+0.5f, -3.5f)), Default_TextSize, 0x00FF00, ".:,;!?");
            //Insert(new TextOrientation(new Normal0Point(0.0f, 0.5f), TextDirection.DiagRD, (+0.5f, -4.5f)), Default_TextSize, 0xFF00FF, "<>=\n<>=");
            //Insert(new TextOrientation(new Normal0Point(0.0f, 0.5f), TextDirection.DiagRD, (+0.5f, -5.5f)), Default_TextSize, 0xFFFF00, "()[]{}");
        }
        public void Test_ASCII()
        {
            for (int j = 0; j < 8; j++)
            {
                char[] c = new char[16];
                for (int i = 0; i < 16; i++)
                {
                    c[i] = (char)((j << 4) | (i << 0));
                }
                Insert(
                    new UIGridPosition(UIAnchor.MM(), new Point2D(0, 0), new Point2D(+0.5f, -0.5f - j * 2)),
                    Default_TextSize, UIGridDirection.DiagDR,
                    0xFFFFFF,
                    new string(c));
            }
        }
        public void Test_Direction()
        {
            string str1 = "|";
            string str2 = "0<<1>>2\n>><<\n3>>>4<<<5\n>><<\n6<<7>>8";
            TextSize size = new TextSize(40f, 2f, 10f);

            Insert(new UIGridPosition(UIAnchor.TL(), new Point2D(0, 0), new Point2D(+0.5f, -0.5f)), size, UIGridDirection.DiagDR, 0x000000, str1);
            Insert(new UIGridPosition(UIAnchor.TL(), new Point2D(0, 0), new Point2D(+0.5f, -0.5f)), size, UIGridDirection.DiagDR, 0xFFFFFF, str2);
            Insert(new UIGridPosition(UIAnchor.TM(), new Point2D(0, 0), new Point2D( 0.0f, -0.5f)), size, UIGridDirection.DiagDC, 0x000000, str1);
            Insert(new UIGridPosition(UIAnchor.TM(), new Point2D(0, 0), new Point2D( 0.0f, -0.5f)), size, UIGridDirection.DiagDC, 0xFFFFFF, str2);
            Insert(new UIGridPosition(UIAnchor.TR(), new Point2D(0, 0), new Point2D(-0.5f, -0.5f)), size, UIGridDirection.DiagDL, 0x000000, str1);
            Insert(new UIGridPosition(UIAnchor.TR(), new Point2D(0, 0), new Point2D(-0.5f, -0.5f)), size, UIGridDirection.DiagDL, 0xFFFFFF, str2);

            Insert(new UIGridPosition(UIAnchor.ML(), new Point2D(0, 0), new Point2D(+0.5f,  0.0f)), size, UIGridDirection.DiagCR, 0x000000, str1);
            Insert(new UIGridPosition(UIAnchor.ML(), new Point2D(0, 0), new Point2D(+0.5f,  0.0f)), size, UIGridDirection.DiagCR, 0xFFFFFF, str2);
            Insert(new UIGridPosition(UIAnchor.MM(), new Point2D(0, 0), new Point2D( 0.0f,  0.0f)), size, UIGridDirection.DiagCC, 0x000000, str1);
            Insert(new UIGridPosition(UIAnchor.MM(), new Point2D(0, 0), new Point2D( 0.0f,  0.0f)), size, UIGridDirection.DiagCC, 0xFFFFFF, str2);
            Insert(new UIGridPosition(UIAnchor.MR(), new Point2D(0, 0), new Point2D(-0.5f,  0.0f)), size, UIGridDirection.DiagCL, 0x000000, str1);
            Insert(new UIGridPosition(UIAnchor.MR(), new Point2D(0, 0), new Point2D(-0.5f,  0.0f)), size, UIGridDirection.DiagCL, 0xFFFFFF, str2);

            Insert(new UIGridPosition(UIAnchor.BL(), new Point2D(0, 0), new Point2D(+0.5f, +0.5f)), size, UIGridDirection.DiagUR, 0x000000, str1);
            Insert(new UIGridPosition(UIAnchor.BL(), new Point2D(0, 0), new Point2D(+0.5f, +0.5f)), size, UIGridDirection.DiagUR, 0xFFFFFF, str2);
            Insert(new UIGridPosition(UIAnchor.BM(), new Point2D(0, 0), new Point2D( 0.0f, +0.5f)), size, UIGridDirection.DiagUC, 0x000000, str1);
            Insert(new UIGridPosition(UIAnchor.BM(), new Point2D(0, 0), new Point2D( 0.0f, +0.5f)), size, UIGridDirection.DiagUC, 0xFFFFFF, str2);
            Insert(new UIGridPosition(UIAnchor.BR(), new Point2D(0, 0), new Point2D(-0.5f, +0.5f)), size, UIGridDirection.DiagUL, 0x000000, str1);
            Insert(new UIGridPosition(UIAnchor.BR(), new Point2D(0, 0), new Point2D(-0.5f, +0.5f)), size, UIGridDirection.DiagUL, 0xFFFFFF, str2);
        }

        public override void Draw()
        {
            Use();
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, Buffer_Pallet);

            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace);
            GL.DrawArrays(PrimitiveType.Points, 0, Count);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);

            Telematry?.Draw(Count);
        }
    }
}
