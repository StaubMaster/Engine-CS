using System;
using System.Collections.Generic;

using OpenTK.Graphics.OpenGL4;

using Engine3D.Graphics.Display;
using Engine3D.Graphics.Shader;

namespace Engine3D.Graphics
{
    public class TextBuffer : BaseBuffer
    {
        public DisplayPointConverter DisplayConverter = new DisplayPointConverter(2000, 1000);
        public TextSizeData Default_TextSize;

        private int Buffer_Pallet;
        private int Buffer_String;
        private int Count;

        public TextBuffer() : base()
        {
            Buffer_Pallet = GL.GenBuffer();
            Buffer_String = GL.GenBuffer();
            Count = 0;

            StringDataList = new List<StringData>();

            DisplayConverter = new DisplayPointConverter(2000, 1000);
            Default_TextSize = new TextSizeData(20f, 1.5f, 10f);
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
            private readonly TextOrientation TextOrient;
            private readonly TextSizeData TextSize;

            private readonly uint Color;
            private readonly string Text;

            public StringData(TextOrientation textOrient, TextSizeData textSize, uint color, string text)
            {
                TextOrient = textOrient;
                TextSize = textSize;

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

                TextDirection hori = TextOrient.Dir & TextDirection.Hori;
                if (hori == TextDirection.HoriC) { return count * TextSize.NormalStride.X * 0.5f; }
                if (hori == TextDirection.HoriL) { return count * TextSize.NormalStride.X; }
                if (hori == TextDirection.HoriR) { return 0.0f; }
                return 0.0f;
            }
            private float TextVertInit()
            {
                int count = 0;
                for (int i = 0; i < Text.Length; i++)
                {
                    if (Text[i] == '\n') { count++; }
                }

                TextDirection vert = TextOrient.Dir & TextDirection.Vert;
                if (vert == TextDirection.VertC) { return count * TextSize.NormalStride.Y * 0.5f; }
                if (vert == TextDirection.VertU) { return count * TextSize.NormalStride.Y; }
                if (vert == TextDirection.VertD) { return 0.0f; }
                return 0.0f;
            }

            public CharacterBufferData[] Interpret()
            {
                CharacterBufferData data = new CharacterBufferData();
                data.PosX = TextOrient.NormalPos.X - TextHoriInit(-1);
                data.PosY = TextOrient.NormalPos.Y + TextVertInit();
                data.ScaleX = TextSize.NormalScale.X;
                data.ScaleY = TextSize.NormalScale.Y;
                data.ThickX = TextSize.NormalThick.X;
                data.ThickY = TextSize.NormalThick.Y;
                data.Color = Color;

                List<CharacterBufferData> list = new List<CharacterBufferData>();

                byte control;

                for (int idx = 0; idx < Text.Length; idx++)
                {
                    control = TextCharacterPallet.Interpret_Control(Text[idx]);
                    if (control == 0)
                    {
                        data.Character = TextCharacterPallet.Interpret_Character(Text[idx]);
                        list.Add(data);
                        data.PosX += TextSize.NormalStride.X;
                    }
                    else if (control == 1)
                    {
                        data.PosX += TextSize.NormalStride.X;
                    }
                    else if (control == 2)
                    {
                        data.PosX += 4 * TextSize.NormalStride.X;
                    }
                    else if (control == 3)
                    {
                        data.PosX = TextOrient.NormalPos.X - TextHoriInit(idx);
                        data.PosY -= TextSize.NormalStride.Y;
                    }
                }

                return list.ToArray();
            }
        }
        private List<StringData> StringDataList;



        public void Insert(DisplayPoint pos, TextDirection dir, (float, float) charOffset, uint color, TextSizeData size, string text)
        {
            TextOrientation orient = new TextOrientation(pos, dir, charOffset);

            size.Normalize(DisplayConverter);
            orient.Normalize(DisplayConverter);

            orient.NormalPos.X += size.NormalStride.X * orient.Offset.Item1;
            orient.NormalPos.Y += size.NormalStride.Y * orient.Offset.Item2;

            StringDataList.Add(new StringData(
                orient,
                size,
                color,
                text));
        }
        public void Insert(TextOrientation orient, TextSizeData size, uint color, string text)
        {
            size.Normalize(DisplayConverter);
            orient.Normalize(DisplayConverter);

            orient.NormalPos.X += size.NormalStride.X * orient.Offset.Item1;
            orient.NormalPos.Y += size.NormalStride.Y * orient.Offset.Item2;

            StringDataList.Add(new StringData(
                orient,
                size,
                color,
                text));
        }

        public void InsertTL((float, float) offset, TextSizeData textSize, uint color, string text)
        {
            Insert(new TextOrientation(new Normal1Point(-1, +1), TextDirection.DiagRD, (offset.Item1 + 0.5f, offset.Item2 - 0.5f)),
                textSize, color, text);
        }
        public void InsertTR((float, float) offset, TextSizeData textSize, uint color, string text)
        {
            Insert(new TextOrientation(new Normal1Point(+1, +1), TextDirection.DiagLD, (offset.Item1 - 0.5f, offset.Item2 - 0.5f)),
                textSize, color, text);
        }
        public void InsertBR((float, float) offset, TextSizeData textSize, uint color, string text)
        {
            Insert(new TextOrientation(new Normal1Point(+1, -1), TextDirection.DiagLU, (offset.Item1 - 0.5f, offset.Item2 + 0.5f)),
                textSize, color, text);
        }
        public void InsertBL((float, float) offset, TextSizeData textSize, uint color, string text)
        {
            Insert(new TextOrientation(new Normal1Point(-1, -1), TextDirection.DiagRU, (offset.Item1 + 0.5f, offset.Item2 + 0.5f)),
                textSize, color, text);
        }



        private struct CharacterBufferData
        {
            public const int Offset_Character = 0;
            public const int Offset_Position = Offset_Character + sizeof(uint);
            public const int Offset_Scale = Offset_Position + sizeof(float) + sizeof(float);
            public const int Offset_Thick = Offset_Scale + sizeof(float) + sizeof(float);
            public const int Offset_Color = Offset_Thick + sizeof(float) + sizeof(float);
            public const int Size = Offset_Color + sizeof(uint);



            public uint Character;

            public float PosX;
            public float PosY;

            public float ScaleX;
            public float ScaleY;

            public float ThickX;
            public float ThickY;

            public uint Color;
        }
        private CharacterBufferData[] Interpret_String()
        {
            List<CharacterBufferData> character_list = new List<CharacterBufferData>();
            for (int i = 0; i < StringDataList.Count; i++)
            {
                character_list.AddRange(StringDataList[i].Interpret());
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
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Size, (IntPtr)CharacterBufferData.Offset_Position);

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Size, (IntPtr)CharacterBufferData.Offset_Scale);

            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 2, VertexAttribPointerType.Float, false, Size, (IntPtr)CharacterBufferData.Offset_Thick);

            GL.EnableVertexAttribArray(4);
            GL.VertexAttribIPointer(4, 1, VertexAttribIntegerType.UnsignedInt, Size, (IntPtr)CharacterBufferData.Offset_Color);

            Telematry?.Bind(Count, Size);
        }



        public void Bind_Pallets()
        {
            TextCharacterPallet[] pallets = TextCharacterPallet.Init_Pallets();
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, Buffer_Pallet);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, pallets.Length * TextCharacterPallet.Size, pallets, BufferUsageHint.StaticDraw);
        }

        public void Test_Characters()
        {
            Insert(new TextOrientation(new Normal0Point(0.0f, 0.5f), TextDirection.DiagRD, (+0.5f, -0.5f)), Default_TextSize, 0xFF0000, "0123456789+-*/");
            Insert(new TextOrientation(new Normal0Point(0.0f, 0.5f), TextDirection.DiagRD, (+0.5f, -1.5f)), Default_TextSize, 0x00FF00, "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            Insert(new TextOrientation(new Normal0Point(0.0f, 0.5f), TextDirection.DiagRD, (+0.5f, -2.5f)), Default_TextSize, 0x0000FF, "abcdefghijklmnopqrstuvwxyz");
            Insert(new TextOrientation(new Normal0Point(0.0f, 0.5f), TextDirection.DiagRD, (+0.5f, -3.5f)), Default_TextSize, 0x00FF00, ".:,;!?");
            Insert(new TextOrientation(new Normal0Point(0.0f, 0.5f), TextDirection.DiagRD, (+0.5f, -4.5f)), Default_TextSize, 0xFF00FF, "<>=\n<>=");
            Insert(new TextOrientation(new Normal0Point(0.0f, 0.5f), TextDirection.DiagRD, (+0.5f, -5.5f)), Default_TextSize, 0xFFFF00, "()[]{}");
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
                    new TextOrientation(new Normal0Point(0.5f, 0.5f), TextDirection.DiagRD, (+0.5f, -0.5f - j * 2)),
                    Default_TextSize,
                    0xFFFFFF,
                    new string(c));
            }
        }
        public void Test_Direction()
        {
            string str1 = "|";
            string str2 = "0<<<1>>2\n3>>>>4<<<<5\n6<<7>>>8";

            Insert(new TextOrientation(new Normal1Point(-1.0f, +1.0f), TextDirection.DiagRD, (+0.5f, -0.5f)), new TextSizeData(50f, 2f, 10f), 0x000000, str1);
            Insert(new TextOrientation(new Normal1Point(-1.0f, +1.0f), TextDirection.DiagRD, (+0.5f, -0.5f)), new TextSizeData(50f, 2f, 10f), 0xFFFFFF, str2);
            Insert(new TextOrientation(new Normal1Point( 0.0f, +1.0f), TextDirection.DiagCD, ( 0.0f, -0.5f)), new TextSizeData(50f, 2f, 10f), 0x000000, str1);
            Insert(new TextOrientation(new Normal1Point( 0.0f, +1.0f), TextDirection.DiagCD, ( 0.0f, -0.5f)), new TextSizeData(50f, 2f, 10f), 0xFFFFFF, str2);
            Insert(new TextOrientation(new Normal1Point(+1.0f, +1.0f), TextDirection.DiagLD, (-0.5f, -0.5f)), new TextSizeData(50f, 2f, 10f), 0x000000, str1);
            Insert(new TextOrientation(new Normal1Point(+1.0f, +1.0f), TextDirection.DiagLD, (-0.5f, -0.5f)), new TextSizeData(50f, 2f, 10f), 0xFFFFFF, str2);

            Insert(new TextOrientation(new Normal1Point(-1.0f,  0.0f), TextDirection.DiagRC, (+0.5f,  0.0f)), new TextSizeData(50f, 2f, 10f), 0x000000, str1);
            Insert(new TextOrientation(new Normal1Point(-1.0f,  0.0f), TextDirection.DiagRC, (+0.5f,  0.0f)), new TextSizeData(50f, 2f, 10f), 0xFFFFFF, str2);
            Insert(new TextOrientation(new Normal1Point( 0.0f,  0.0f), TextDirection.DiagCC, ( 0.0f,  0.0f)), new TextSizeData(50f, 2f, 10f), 0x000000, str1);
            Insert(new TextOrientation(new Normal1Point( 0.0f,  0.0f), TextDirection.DiagCC, ( 0.0f,  0.0f)), new TextSizeData(50f, 2f, 10f), 0xFFFFFF, str2);
            Insert(new TextOrientation(new Normal1Point(+1.0f,  0.0f), TextDirection.DiagLC, (-0.5f,  0.0f)), new TextSizeData(50f, 2f, 10f), 0x000000, str1);
            Insert(new TextOrientation(new Normal1Point(+1.0f,  0.0f), TextDirection.DiagLC, (-0.5f,  0.0f)), new TextSizeData(50f, 2f, 10f), 0xFFFFFF, str2);

            Insert(new TextOrientation(new Normal1Point(-1.0f, -1.0f), TextDirection.DiagRU, (+0.5f, +0.5f)), new TextSizeData(50f, 2f, 10f), 0x000000, str1);
            Insert(new TextOrientation(new Normal1Point(-1.0f, -1.0f), TextDirection.DiagRU, (+0.5f, +0.5f)), new TextSizeData(50f, 2f, 10f), 0xFFFFFF, str2);
            Insert(new TextOrientation(new Normal1Point( 0.0f, -1.0f), TextDirection.DiagCU, ( 0.0f, +0.5f)), new TextSizeData(50f, 2f, 10f), 0x000000, str1);
            Insert(new TextOrientation(new Normal1Point( 0.0f, -1.0f), TextDirection.DiagCU, ( 0.0f, +0.5f)), new TextSizeData(50f, 2f, 10f), 0xFFFFFF, str2);
            Insert(new TextOrientation(new Normal1Point(+1.0f, -1.0f), TextDirection.DiagLU, (-0.5f, +0.5f)), new TextSizeData(50f, 2f, 10f), 0x000000, str1);
            Insert(new TextOrientation(new Normal1Point(+1.0f, -1.0f), TextDirection.DiagLU, (-0.5f, +0.5f)), new TextSizeData(50f, 2f, 10f), 0xFFFFFF, str2);
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
