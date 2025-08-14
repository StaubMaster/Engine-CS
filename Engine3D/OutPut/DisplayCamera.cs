
using Engine3D.Abstract3D;

using Engine3D.OutPut.Shader;
using Engine3D.OutPut.Uniform.Specific;
using Engine3D.OutPut.Uniform.Generic.Float;

namespace Engine3D.OutPut
{
    public class DisplayCamera
    {
        public readonly CUniformTransformation TransUni;
        public readonly CUniformDepth Depth;
        public Transformation3D Trans;
        public Ray3D Ray;

        public DisplayCamera()
        {
            TransUni = new CUniformTransformation();
            Trans = new Transformation3D();
            Depth = new CUniformDepth(0.1f, 100.0f);
            Ray = new Ray3D();
        }

        public void Update(Point3D mouse_ray)
        {
            TransUni.Pos = Trans.Pos;
            TransUni.Rot = Trans.Rot;

            if (mouse_ray == null) { mouse_ray = new Point3D(); }
            Ray = new Ray3D(Trans.Pos, mouse_ray - Trans.Rot);
        }
    }
}
