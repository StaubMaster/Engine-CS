using Engine3D.Abstract3D;
using Engine3D.Graphics.Shader;
using Engine3D.DataStructs;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.Graphics.PolyHedraBase
{
    public struct PolyHedra_Corner_Data
    {
        public Point3D Position;
        public Point3D Normal;
        public float TexturePos;

        public PolyHedra_Corner_Data(Point3D pos, Point3D norm, float tex)
        {
            Position = pos;
            Normal = norm;
            TexturePos = tex;
        }



        public const int SizeOf = Point3D.SizeOf + Point3D.SizeOf + sizeof(float);
        public static void ToBuffer(int stride, ref System.IntPtr offset, int divisor, params int[] bindIndex)
        {
            Point3D.ToBuffer(stride, ref offset, divisor, bindIndex[0]);
            Point3D.ToBuffer(stride, ref offset, divisor, bindIndex[1]);

            OpenTK.Graphics.OpenGL.GL.EnableVertexAttribArray(bindIndex[2]);
            OpenTK.Graphics.OpenGL.GL.VertexAttribPointer(bindIndex[2], 1, OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float, false, stride, offset);
            OpenTK.Graphics.OpenGL.GL.VertexAttribDivisor(bindIndex[2], divisor);
            offset += sizeof(float);
        }
    }
    public abstract class PolyHedra_Base_Buffer : Base_Buffer
    {
        protected readonly int MainBuffer;
        protected int MainCount;

        protected readonly int TextureArray;

        public PolyHedra_Base_Buffer() : base()
        {
            MainBuffer = GL.GenBuffer();
            MainCount = 0;

            TextureArray = GL.GenTexture();
        }
        ~PolyHedra_Base_Buffer()
        {
            Use();

            GL.DeleteBuffer(MainBuffer);

            GL.DeleteTexture(TextureArray);
        }


        public void BindMain(PolyHedra_Corner_Data[] data)
        {
            Use();

            GL.BindBuffer(BufferTarget.ArrayBuffer, MainBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * PolyHedra_Corner_Data.SizeOf, data, BufferUsageHint.StaticDraw);

            System.IntPtr offset = System.IntPtr.Zero;

            PolyHedra_Corner_Data.ToBuffer(PolyHedra_Corner_Data.SizeOf, ref offset, 0, 0, 1, 2);

            MainCount = data.Length;
        }
        public void BindTex(ColorUData[] data)
        {
            GL.BindTexture(TextureTarget.Texture1DArray, TextureArray);

            int tex_w = data.Length;
            int tex_h = 1;

            GL.TexImage2D(TextureTarget.Texture1DArray, 0, PixelInternalFormat.Rgba8, tex_w, tex_h, 0, PixelFormat.Rgba, PixelType.UnsignedInt8888Rev, System.IntPtr.Zero);

            int i = 0;
            {
                GL.TexSubImage2D(TextureTarget.Texture1DArray, 0, 0, i, tex_w, 1, PixelFormat.Rgba, PixelType.UnsignedInt8888Rev, data);
            }

            GL.TexParameter(TextureTarget.Texture1DArray, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture1DArray, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture1DArray, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture1DArray, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture1DArray);
        }



        public override void Draw_Main()
        {
            Use();
            GL.BindTexture(TextureTarget.Texture1DArray, TextureArray);
            GL.DrawArrays(PrimitiveType.Triangles, 0, MainCount);
        }
    }
}
