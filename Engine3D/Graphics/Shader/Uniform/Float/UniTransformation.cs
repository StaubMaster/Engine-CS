using Engine3D.Abstract3D;

namespace Engine3D.Graphics.Shader.Uniform.Float
{
    public class UniTransformation : UniFloat3
    {
        public UniTransformation(BaseShader program, string name) : base(program, name, 3) { }

        public void Value(Transformation3D trans)
        {
            float[] data = NewData();
            //Get(data);
            Transformation3D.ShaderFloats(trans, data, 0);
            Set(data);
        }
        /*public void Value(Punkt pos)
        {
            float[] data = Data();
            Get(data);
            Punkt.ShaderFloats(pos, data, 0);
            Set(data);
        }*/
        /*public void Value(Winkl wnk)
        {
            float[] data = Data();
            Get(data);
            Winkl.ShaderFloats(wnk, data, 3);
            Set(data);
        }*/
    }
}
