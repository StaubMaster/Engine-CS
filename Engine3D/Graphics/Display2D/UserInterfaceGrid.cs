using Engine3D.Abstract2D;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.Graphics
{
    public enum UIGridDirection
    {
        Hori = 0b0011,
        HoriL = 0b0001,
        HoriR = 0b0010,
        HoriC = 0b0000,

        Vert = 0b1100,
        VertD = 0b0100,
        VertU = 0b1000,
        VertC = 0b0000,

        DiagDL = HoriL | VertD,
        DiagDR = HoriR | VertD,
        DiagDC = HoriC | VertD,

        DiagUL = HoriL | VertU,
        DiagUR = HoriR | VertU,
        DiagUC = HoriC | VertU,

        DiagCL = HoriL | VertC,
        DiagCR = HoriR | VertC,
        DiagCC = HoriC | VertC,
    }

    public struct UIGridPosition
    {
                                //  [  0 ; 1  ]     Normal0
        public Point2D Normal;  //  [ -1 ; +1 ]     Normal1
        public Point2D Pixel;   //  [  0 : n  ]     Pixel
        public Point2D Offset;  //  any

        public UIGridPosition(Point2D normal, Point2D pixel, Point2D offset)
        {
            Normal = normal;
            Pixel = pixel;
            Offset = offset;
        }
        public UIGridPosition WithOffset(Point2D offset)
        {
            return new UIGridPosition(Normal, Pixel, Offset + offset);
        }
        public UIGridPosition WithOffset(float offX, float offY)
        {
            return new UIGridPosition(Normal, Pixel, Offset + new Point2D(offX, offY));
        }



        public const int SizeOf = Point2D.SizeOf * 3;
        public static void ToBuffer(int stride, ref System.IntPtr offset, int divisor, params int[] bindIndex)
        {
            Point2D.ToBuffer(stride, ref offset, divisor, bindIndex[0]);
            Point2D.ToBuffer(stride, ref offset, divisor, bindIndex[1]);
            Point2D.ToBuffer(stride, ref offset, divisor, bindIndex[2]);
        }
    }
    public struct UIGridSize
    {
        public Point2D Size;
        public float Padding;

        public UIGridSize(Point2D size, float padding)
        {
            Size = size;
            Padding = padding;
        }
        public Point2D ToStride()
        {
            return new Point2D(
                Size.X + Padding,
                Size.Y + Padding
                );
        }



        public const int SizeOf = Point2D.SizeOf + sizeof(float);
        public static void ToBuffer(int stride, ref System.IntPtr offset, int divisor, params int[] bindIndex)
        {
            Point2D.ToBuffer(stride, ref offset, divisor, bindIndex[0]);

            GL.EnableVertexAttribArray(bindIndex[1]);
            GL.VertexAttribPointer(bindIndex[1], 1, VertexAttribPointerType.Float, false, stride, offset);
            GL.VertexAttribDivisor(bindIndex[1], divisor);
            offset += sizeof(float);
        }
    }
    public struct UIGrid
    {
        public Point2D ScreenSize;  //  Screen Size Object ?
        public UIGridPosition Pos;
        public UIGridSize Size;

        public UIGrid(UIGridPosition pos, UIGridSize size)
        {
            ScreenSize = new Point2D();
            Pos = pos;
            Size = size;
            Size.Size.X *= 0.5f;
        }

        public Point2D ToNormal0(Point2D offset)
        {
            Point2D p = offset * Size.ToStride();

            p.X = p.X / ScreenSize.X;
            p.Y = p.Y / ScreenSize.Y;

            p.X = (p.X * 2) - 1;
            p.Y = (p.Y * 2) - 1;

            return new Point2D();
        }
    }

    public static class UICorner
    {
        public static Point2D BL() { return new Point2D(+0.5f, +0.5f); }
        public static Point2D ML() { return new Point2D(+0.5f,  0.0f); }
        public static Point2D TL() { return new Point2D(+0.5f, -0.5f); }
        public static Point2D BM() { return new Point2D( 0.0f, +0.5f); }
        public static Point2D MM() { return new Point2D( 0.0f,  0.0f); }
        public static Point2D TM() { return new Point2D( 0.0f, -0.5f); }
        public static Point2D BR() { return new Point2D(-0.5f, +0.5f); }
        public static Point2D MR() { return new Point2D(-0.5f,  0.0f); }
        public static Point2D TR() { return new Point2D(-0.5f, -0.5f); }
    }
    public static class UIAnchor
    {
        //public static Point2D BL() { return new Point2D(0.0f, 0.0f); }
        //public static Point2D ML() { return new Point2D(0.0f, 1.0f); }
        //public static Point2D TL() { return new Point2D(0.0f, 2.0f); }
        //public static Point2D BM() { return new Point2D(1.0f, 0.0f); }
        //public static Point2D MM() { return new Point2D(1.0f, 1.0f); }
        //public static Point2D TM() { return new Point2D(1.0f, 2.0f); }
        //public static Point2D BR() { return new Point2D(2.0f, 0.0f); }
        //public static Point2D MR() { return new Point2D(2.0f, 1.0f); }
        //public static Point2D TR() { return new Point2D(2.0f, 2.0f); }

        //public static Point2D BL() { return new Point2D(0.0f, 0.0f); }
        //public static Point2D ML() { return new Point2D(0.0f, 0.5f); }
        //public static Point2D TL() { return new Point2D(0.0f, 1.0f); }
        //public static Point2D BM() { return new Point2D(0.5f, 0.0f); }
        //public static Point2D MM() { return new Point2D(0.5f, 0.5f); }
        //public static Point2D TM() { return new Point2D(0.5f, 1.0f); }
        //public static Point2D BR() { return new Point2D(1.0f, 0.0f); }
        //public static Point2D MR() { return new Point2D(1.0f, 0.5f); }
        //public static Point2D TR() { return new Point2D(1.0f, 1.0f); }

        public static Point2D BL() { return new Point2D(-1.0f, -1.0f); }
        public static Point2D ML() { return new Point2D(-1.0f,  0.0f); }
        public static Point2D TL() { return new Point2D(-1.0f, +1.0f); }
        public static Point2D BM() { return new Point2D( 0.0f, -1.0f); }
        public static Point2D MM() { return new Point2D( 0.0f,  0.0f); }
        public static Point2D TM() { return new Point2D( 0.0f, +1.0f); }
        public static Point2D BR() { return new Point2D(+1.0f, -1.0f); }
        public static Point2D MR() { return new Point2D(+1.0f,  0.0f); }
        public static Point2D TR() { return new Point2D(+1.0f, +1.0f); }
    }
}
