using Engine3D.Abstract3D;

namespace Engine3D.Graphics
{
    public struct RenderAxisBox3D
    {
        public const int Size_Color = 0;
        public const int Size_Min = Size_Color + sizeof(uint);
        public const int Size_Max = Size_Min + Point3D.SizeOf;
        public const int Size = Size_Max + Point3D.SizeOf;



        private uint Color;
        private Point3D Min;
        private Point3D Max;

        public RenderAxisBox3D(AxisBox3D box, uint col)
        {
            Color = col;
            Min = box.Min;
            Max = box.Max;
        }
    }
}
