using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine3D;
using Engine3D.Abstract3D;
using Engine3D.OutPut;
using Engine3D.OutPut.Shader;
using Engine3D.OutPut.Uniform;
using Engine3D.OutPut.Uniform.Specific;
using Engine3D.OutPut.Uniform.Generic.Float;
using Engine3D.BodyParse;
using Engine3D.Graphics.Display3D;

using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine3D.Graphics.Display
{
    public class DisplayPolyHedra
    {
        public readonly PolyHedra PHedra;
        private readonly PHEI_Buffer Buffer;
        private readonly string Path;

        public DisplayPolyHedra(string path)
        {
            Path = path;
            PHedra = TBodyFile.LoadTextFile(path);
            Buffer = new PHEI_Buffer();
            PHedra.ToBuffer(Buffer);
        }

        public Intersekt.RayInterval Intersekt(Ray3D ray, Transformation3D trans)
        {
            return PHedra.Intersekt(ray, trans);
        }
        public AxisBox3D BoxFit()
        {
            return PHedra.CalcBox();
        }

        public void Draw()
        {
            Buffer.Draw_Main();
        }
    }
    public class DisplayBody
    {
        public DisplayPolyHedra Body;
        public Transformation3D Trans;

        public DisplayBody(DisplayPolyHedra body, Transformation3D trans)
        {
            Body = body;
            Trans = trans;
        }
        public DisplayBody(string path)
        {
            Body = new DisplayPolyHedra(path);
            Trans = Transformation3D.Default();
        }

        public Intersekt.RayInterval Intersekt(Ray3D ray)
        {
            return Body.Intersekt(ray, Trans);
        }

        public void Draw(BodyElemUniShader shader)
        {
            shader.Trans.Value(Trans);
            Body.Draw();
        }
        public void Draw(BodyElemUniWireShader shader)
        {
            shader.Trans.Value(Trans);
            Body.Draw();
        }
    }
}
