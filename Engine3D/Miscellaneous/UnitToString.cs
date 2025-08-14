
namespace Engine3D.Miscellaneous
{
    public static class UnitToString
    {

        private static readonly string[] MetricFix = new string[] { "", "k", "M", "G", "T", "P" };
        private static readonly string[] BinaryFix = new string[] { "", "Ki", "Mi", "Gi", "Ti", "Pi" };

        public static string MemoryRaw(int size)
        {
            return size + " B";
        }
        public static string Memory1000(int size)
        {
            int i;
            for (i = 0; i < MetricFix.Length - 1; i++)
            {
                if (size < 1000) { break; }
                size /= 1000;
            }
            return size + " " + MetricFix[i] + "B";
        }
        public static string Memory1000Raw(int size)
        {
            return Memory1000(size) + " " + "(" + MemoryRaw(size) + ")";
        }



        private struct UnitFloat
        {
            public readonly string Name;
            public readonly string Fix;
            public readonly float Size;
            public UnitFloat(string name, string fix, float size)
            {
                Name = name;
                Fix = fix;
                Size = size;
            }
        }
        private static readonly UnitFloat[] MetricUnits = new UnitFloat[]
        {
            new UnitFloat("nano",  "n", 0.000_000_001f),
            new UnitFloat("micro", "µ", 0.000_001f),
            new UnitFloat("milli", "m", 0.001f),
            new UnitFloat("centi", "c", 0.01f),
            new UnitFloat("deci",  "d", 0.1f),
            new UnitFloat("",      "",  1f),
            new UnitFloat("deca",  "",  1_0f),
            new UnitFloat("hecto", "",  1_00f),
            new UnitFloat("kilo",  "k", 1_000f),
            new UnitFloat("mega",  "M", 1_000_000f),
            new UnitFloat("giga",  "G", 1_000_000_000f),
            new UnitFloat("tera",  "T", 1_000_000_000_000f),
        };
        public static string Metric(double l)
        {
            return (l / 10).ToString("+0.000;-0.000; 0.000") + "m";
        }
        public static string Metric(Abstract3D.Point3D p)
        {
            string str = "";
            str += Metric(p.Y);
            str += " : ";
            str += Metric(p.X);
            str += " : ";
            str += Metric(p.C);
            return str;
        }
        public static string Metric(Abstract3D.AxisBox3D box)
        {
            string str = "";
            str += Metric(box.Max.Y - box.Min.Y);
            str += " : ";
            str += Metric(box.Max.X - box.Min.X);
            str += " : ";
            str += Metric(box.Max.C - box.Min.C);
            return str;
        }
    }
}
