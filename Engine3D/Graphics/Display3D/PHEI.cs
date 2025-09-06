using Engine3D.Abstract3D;

using Engine3D.BodyParse;

using Engine3D.Miscellaneous;

namespace Engine3D.Graphics.Display3D
{
    public class PHEI
    {
        private readonly string Path;
        private readonly PolyHedra Main;
        private readonly PHEIBuffer Buffer;
        private readonly FixedEntryContainer<PHEIData> Trans;
        private bool TransNeedsUpdate;

        public PHEI(string path)
        {
            Path = path;
            Main = TBodyFile.LoadTextFile(path);

            Buffer = new PHEIBuffer();
            Main.ToBuffer(Buffer);

            Trans = new FixedEntryContainer<PHEIData>();

            TransNeedsUpdate = false;
        }
        public PHEI(PolyHedra main)
        {
            Path = null;
            Main = main;

            Buffer = new PHEIBuffer();
            Main.ToBuffer(Buffer);

            Trans = new FixedEntryContainer<PHEIData>();

            TransNeedsUpdate = false;
        }

        public Intersekt.RayInterval Intersekt(Ray3D ray, Transformation3D trans)
        {
            return Main.Intersekt(ray, trans);
        }
        public AxisBox3D BoxFit()
        {
            return Main.CalcBox();
        }

        public void TransChange()
        {
            TransNeedsUpdate = true;
        }
        public void TransUpdate()
        {
            if (TransNeedsUpdate)
            {
                Buffer.Bind_Inst_Trans(Trans.Data, Trans.Length, true);
                TransNeedsUpdate = false;
            }
        }

        public FixedEntryContainer<PHEIData>.Entry Alloc(int size)
        {
            return Trans.Alloc(size);
        }
        public string Info()
        {
            string str = "";
            str += Trans.Length.ToString("000");
            str += " : ";
            str += Trans.EntryRefs.Count.ToString("00");
            return str;
        }

        public void Draw_Main()
        {
            Buffer.Draw_Main();
        }
        public void Draw_Inst(bool debug = false)
        {
            Buffer.Draw_Inst(debug);
        }
    }
}
