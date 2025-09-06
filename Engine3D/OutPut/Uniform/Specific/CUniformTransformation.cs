
using Engine3D.Abstract3D;

namespace Engine3D.OutPut.Uniform.Specific
{
    public class CUniformTransformation : Generic.Float.CUniformFloat3
    {
        public Point3D Pos
        {
            set { Point3D.ShaderFloats(value, Data, 0 * Size); }
        }
        public Angle3D Rot
        {
            set { Angle3D.ShaderFloats(value, Data, 1 * Size); }
        }

        public CUniformTransformation() : base(3)
        {
            Point3D.ShaderFloats(Point3D.Null(), Data, 0 * Size);
            Angle3D.ShaderFloats(Angle3D.Null(), Data, 1 * Size);
        }
        public CUniformTransformation(Point3D p) : base(3)
        {
            Point3D.ShaderFloats(p, Data, 0 * Size);
            Angle3D.ShaderFloats(Angle3D.Null(), Data, 1 * Size);
        }
        public CUniformTransformation(Angle3D w) : base(3)
        {
            Point3D.ShaderFloats(Point3D.Null(), Data, 0 * Size);
            Angle3D.ShaderFloats(w, Data, 1 * Size);
        }
        public CUniformTransformation(Point3D p, Angle3D w) : base(3)
        {
            Point3D.ShaderFloats(p, Data, 0 * Size);
            Angle3D.ShaderFloats(w, Data, 1 * Size);
        }
        public CUniformTransformation(Transformation3D t) : base(3)
        {
            Transformation3D.ShaderFloats(t, Data, 0);
        }

        private const string ToStringFormat = "+0.00;-0.00; 0.00";
        public override string ToString()
        {
            string str = "";

            str += "Pos.Y:" + Data[0].ToString(ToStringFormat) + "\n";
            str += "Pos.X:" + Data[1].ToString(ToStringFormat) + "\n";
            str += "Pos.C:" + Data[2].ToString(ToStringFormat) + "\n";

            return str;
        }
    }
}
