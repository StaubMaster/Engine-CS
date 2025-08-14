using Engine3D.Abstract3D;

namespace Engine3D.Graphics.Shader.Uniform.Float
{
    public class UniPoint : UniFloat3
    {
        public UniPoint(BaseShader program, string name) : base(program, name, 1) { }

        public void Value(Point3D p)
        {
            float[] data = NewData();
            Point3D.ShaderFloats(p, data, 0);
            Set(data);
        }
    }
}
