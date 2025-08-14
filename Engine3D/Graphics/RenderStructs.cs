using Engine3D.Abstract3D;

namespace Engine3D.Graphics
{
    public struct RenderPoint3D
    {
        public const int Size = sizeof(float) * 3;



        public float Y;
        public float X;
        public float C;

        public RenderPoint3D(Point3D p)
        {
            Y = (float)p.Y;
            X = (float)p.X;
            C = (float)p.C;
        }
        public Point3D ToPoint3D()
        {
            return new Point3D(Y, X, C);
        }

        public static RenderPoint3D[] Convert(Point3D[] CArr)
        {
            RenderPoint3D[] SArr = new RenderPoint3D[CArr.Length];
            for (int i = 0; i < CArr.Length; i++)
            {
                SArr[i] = new RenderPoint3D(CArr[i]);
            }
            return SArr;
        }
    }

    public struct RenderAxisBox3D
    {
        public const int Size_Color = 0;
        public const int Size_Min = Size_Color + sizeof(uint);
        public const int Size_Max = Size_Min + RenderPoint3D.Size;
        public const int Size = Size_Max + RenderPoint3D.Size;



        private uint Color;
        private RenderPoint3D Min;
        private RenderPoint3D Max;

        public RenderAxisBox3D(AxisBox3D box, uint col)
        {
            Color = col;
            Min = new RenderPoint3D(box.Min);
            Max = new RenderPoint3D(box.Max);
        }
    }
}
