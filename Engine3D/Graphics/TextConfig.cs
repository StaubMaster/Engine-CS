using Engine3D.Graphics.Display;

namespace Engine3D.Graphics
{
    public enum TextDirection
    {
        Hori = 0b0011,
        HoriL = 0b0001,
        HoriR = 0b0010,
        HoriC = 0b0000,

        Vert = 0b1100,
        VertD = 0b0100,
        VertU = 0b1000,
        VertC = 0b0000,

        DiagLD = HoriL | VertD,
        DiagRD = HoriR | VertD,
        DiagCD = HoriC | VertD,

        DiagLU = HoriL | VertU,
        DiagRU = HoriR | VertU,
        DiagCU = HoriC | VertU,

        DiagLC = HoriL | VertC,
        DiagRC = HoriR | VertC,
        DiagCC = HoriC | VertC,
    }
    public struct TextOrientation
    {
        public DisplayPoint Pos;
        public TextDirection Dir;
        public (float, float) Offset;

        public DisplayPoint NormalPos;

        public TextOrientation(DisplayPoint pos, TextDirection dir, (float, float) offset)
        {
            Pos = pos;
            Dir = dir;
            Offset = offset;

            NormalPos = null;
        }
        public void Normalize(DisplayPointConverter pointConverter)
        {
            NormalPos = pointConverter.ToNormal1(Pos);
        }
    }
    public struct TextSizeData
    {
        public DisplayPoint Scale;
        public DisplayPoint Thick;
        public DisplayPoint Stride;

        public DisplayPoint NormalScale;
        public DisplayPoint NormalThick;
        public DisplayPoint NormalStride;

        public TextSizeData(float scale, float thick, float dist)
        {
            Scale = new PixelPoint(scale, scale);
            Thick = new PixelPoint(thick, thick);
            Stride = new PixelPoint(
                scale * 2 * TextCharacterPallet.charScaleX + dist,
                scale * 2 * TextCharacterPallet.charScaleY + dist);

            NormalScale = null;
            NormalThick = null;
            NormalStride = null;
        }
        public void Normalize(DisplayPointConverter pointConverter)
        {
            NormalScale = pointConverter.ToNormal0(Scale);
            NormalThick = pointConverter.ToNormal0(Thick);
            NormalStride = pointConverter.ToNormal0(Stride);
        }
    }
}
