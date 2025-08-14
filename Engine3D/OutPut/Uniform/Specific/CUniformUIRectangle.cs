
namespace Engine3D.OutPut.Uniform.Specific
{
    public class CUniformUIRectangle : Generic.Float.CUniformFloat2
    {
        public CUniformUIRectangle(float w, float h, float y, float x, float anchor_y, float anchor_x) : base(3)
        {
            Data[0] = w;
            Data[1] = h;
            Data[2] = y;
            Data[3] = x;
            Data[4] = anchor_y;
            Data[5] = anchor_x;
        }
    }
}
