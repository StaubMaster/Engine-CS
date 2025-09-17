using Engine3D.Abstract3D;

using Engine3D.BodyParse;

using Engine3D.Graphics.PolyHedraBase;

using Engine3D.Miscellaneous.EntryContainer;

namespace Engine3D.Graphics.Display3D
{
    public class PolyHedraInstance_3D_BufferData : PolyHedraInstance_Base_BufferData<PolyHedraInstance_3D_Buffer, PolyHedraInstance_3D_Data>
    {
        private readonly string Path;

        public PolyHedraInstance_3D_BufferData(string path) : base(TBodyFile.LoadTextFile(path))
        {
            Path = path;
        }
        public PolyHedraInstance_3D_BufferData(PolyHedra ph) : base(ph)
        {
            Path = null;
        }

        public AxisBox3D BoxFit()
        {
            return PH.CalcBox();
        }

        public string Info()
        {
            string str = "";
            str += InstanceData.Length.ToString("000");
            str += " : ";
            str += InstanceData.EntryRefs.Count.ToString("00");
            return str;
        }
    }
}
