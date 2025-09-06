
using Engine3D.Abstract3D;

namespace Engine3D.OutPut.Uniform.Specific
{
    public class CUniformPoint : Generic.Float.CUniformFloat3
    {
        public Point3D Pos
        {
            set { Point3D.ShaderFloats(value, Data, 0); }
        }

        public CUniformPoint() : base(1)
        {
            Point3D.ShaderFloats(Point3D.Null(), Data, 0);
        }
        public CUniformPoint(Point3D p) : base(1)
        {
            Point3D.ShaderFloats(p, Data, 0);
        }
    }
}
