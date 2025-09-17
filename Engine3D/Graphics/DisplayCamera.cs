
using Engine3D.Abstract3D;
using Engine3D.DataStructs;

namespace Engine3D.OutPut
{
    public class DisplayCamera
    {
        public Transformation3D Trans;
        public DepthData Depth;
        public Ray3D Ray;

        public DisplayCamera()
        {
            Trans = Transformation3D.Default();
            Depth = new DepthData(0.1f, 100.0f);
            Ray = new Ray3D();
        }

        public void Update(Point3D mouse_ray)
        {
            if (mouse_ray.IsNaN()) { mouse_ray = Point3D.Default(); }
            Ray = new Ray3D(Trans.Pos, mouse_ray - Trans.Rot);
        }
    }
}
