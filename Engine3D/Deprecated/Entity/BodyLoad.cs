using System;
using System.Collections.Generic;
using System.Linq;

using Engine3D.Abstract3D;

namespace Engine3D.Entity
{
    public partial class BodyStatic
    {
        /*[System.Serializable]
        class MyException : Exception
        {
            public MyException() { }
            public MyException(string message) : base(message) { }
            public MyException(string message, Exception inner) : base(message, inner) { }
            protected MyException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }*/

        public static class File
        {
            private struct LoadData
            {
                public int fileIdx;
                public string[] fileTxt;

                public string LineName;
                public string[] LineSegments;
                public int LineType;

                public List<Point3D> Ecken;
                public List<Tri> Seiten;

                public uint E_Offset;
                public uint S_Offset;

                public LoadData(string path)
                {
                    fileIdx = 0;
                    if (!System.IO.File.Exists(path))
                        fileTxt = null;
                    else
                        fileTxt = System.IO.File.ReadAllLines(path);

                    LineType = 0;
                    LineName = null;
                    LineSegments = null;

                    Ecken = new List<Point3D>();
                    Seiten = new List<Tri>();

                    E_Offset = 0;
                    S_Offset = 0;
                }

                private static int checkType(string t)
                {
                    string[] validTypes = new string[]
                    {
                        "p",
                        "d",
                        "o",
                        "c",
                        "f",
                        "k",
                        "v",
                        "qube",
                        "ring",
                        "fan",
                    };

                    for (int i = 0; i < validTypes.Length; i++)
                    {
                        if (t == validTypes[i])
                            return i + 1;
                    }
                    return -1;
                }
                public void SplitCheckLine()
                {
                    string[] noComment = fileTxt[fileIdx].Split("/");

                    if (noComment.Length != 0 &&
                        noComment[0].Length != 0 &&
                        noComment[0][0] != '\t' &&
                        noComment[0][0] != ' ')
                    {
                        int nameSplitIndex = noComment[0].IndexOf(' ');
                        LineName = noComment[0].Substring(0, nameSplitIndex);
                        LineSegments = noComment[0].Substring(nameSplitIndex).Split(" ", StringSplitOptions.RemoveEmptyEntries);
                        LineType = checkType(LineName);
                    }
                    else
                    {
                        LineSegments = null;
                        LineType = 0;
                    }
                }

                private void Error(string name)
                {
                    string err = "Error: ";
                    err += "Line: " + (fileIdx + 1) + ": ";
                    switch (LineType)
                    {
                        case 1: err += "Point"; break;
                        case 2: err += "Dreieck"; break;
                        case 3: err += "Polygon"; break;
                        case 4: err += "Farbe1"; break;
                        case 5: err += "FarbeN"; break;
                        case 6: err += "Kariert"; break;
                        case 7: err += "Offset"; break;

                        case 8: err += "Qube"; break;
                        case 9: err += "Ring"; break;

                        default: err += "None"; break;
                    }
                    err += ": " + name;
                    ConsoleLog.Log(err);
                }

                private bool CheckSegments(uint len)
                {
                    if (LineSegments.Length < len)
                    {
                        Error("Segments " + LineSegments.Length + " < " + len);
                        return false;
                    }
                    return true;
                }
                private uint ParseIndex(string segment, string name, uint offset)
                {
                    if (segment[0] == '+' || segment[0] == '-')
                    {
                        int rel;
                        if (!int.TryParse(segment, out rel)) { Error("off " + name); }
                        return (uint)(offset + rel);
                    }
                    else
                    {
                        uint abs;
                        if (!uint.TryParse(segment, out abs)) { Error("idx " + name); }
                        return abs;
                    }
                }
                private uint ParseChannel(string segment, string name)
                {
                    uint c;
                    if (!uint.TryParse(segment, out c)) { Error(name); }
                    if (c > 0xFF) { Error(name + " > 255"); }
                    return c & 0xFF;
                }
                private uint ParseColor(int segIdx)
                {
                    uint r, g, b;
                    r = ParseChannel(LineSegments[segIdx + 0], "R");
                    g = ParseChannel(LineSegments[segIdx + 1], "G");
                    b = ParseChannel(LineSegments[segIdx + 2], "B");
                    return (r << 16) | (g << 8) | (b << 0);
                }
                private Point3D ParsePunkt(int segIdx)
                {
                    Point3D p = Point3D.Default();
                    if (!float.TryParse(LineSegments[segIdx + 0], out p.Y)) { Error("Y"); }
                    if (!float.TryParse(LineSegments[segIdx + 1], out p.X)) { Error("X"); }
                    if (!float.TryParse(LineSegments[segIdx + 2], out p.C)) { Error("C"); }
                    return p;
                }

                private uint[] ParseIndexe(int segIdx, int count, uint offset)
                {
                    uint[] idx = new uint[count];

                    for (int i = 0; i < count; i++)
                    {
                        idx[i] = ParseIndex(LineSegments[i + segIdx], "idx[" + i + "]", offset);
                    }

                    return idx;
                }

                private void ParsePunkt()
                {
                    if (!CheckSegments(3)) { return; }

                    Point3D p = ParsePunkt(0);
                    Ecken.Add(p);
                }
                private void ParseDreieck()
                {
                    if (!CheckSegments(3)) { return; }

                    uint[] idx = ParseIndexe(0, 3, E_Offset);
                    Seiten.Add(new Tri(idx[0], idx[1], idx[2], 0xFFFFFFFF));
                }
                private void ParsePolygon()
                {
                    if (!CheckSegments(4)) { return; }

                    uint[] idx = ParseIndexe(0, 4, E_Offset);
                    Seiten.Add(new Tri(idx[0], idx[1], idx[2], 0xFFFFFFFF));
                    Seiten.Add(new Tri(idx[2], idx[1], idx[3], 0xFFFFFFFF));
                }
                private void ParseColor1()
                {
                    if (!CheckSegments(4)) { return; }

                    uint idx;
                    idx = ParseIndex(LineSegments[0], "idx", S_Offset);

                    uint col = ParseColor(1);

                    if (idx >= Seiten.Count) { Error("idx out of Range"); return; }

                    Tri tri = Seiten[(int)idx];
                    tri.Color = col;
                    Seiten[(int)idx] = tri;
                }
                private void ParseColorN()
                {
                    if (!CheckSegments(5)) { return; }

                    uint[] idx = ParseIndexe(0, 2, S_Offset);

                    uint col = ParseColor(2);

                    if (idx[0] >= Seiten.Count) { Error("idx1 out of Range"); return; }
                    if (idx[1] >= Seiten.Count) { Error("idx2 out of Range"); return; }

                    for (uint i = idx[0]; i <= idx[1]; i++)
                    {
                        Tri tri = Seiten[(int)i];
                        tri.Color = col;
                        Seiten[(int)i] = tri;
                    }
                }
                private void ParseKariert()
                {
                    if (!CheckSegments(8)) { return; }

                    uint[] idx = ParseIndexe(0, 2, S_Offset);

                    if (idx[0] > Seiten.Count) { Error("idx1 out of Range"); return; }
                    if (idx[1] > Seiten.Count) { Error("idx2 out of Range"); return; }

                    uint col1 = ParseColor(2);
                    uint col2 = ParseColor(5);

                    for (uint i = idx[0]; i <= idx[1]; i++)
                    {
                        Tri tri = Seiten[(int)i];
                        uint mod = i % 4;
                        if (mod == 0 || mod == 3)
                            tri.Color = col1;
                        else
                            tri.Color = col2;
                        Seiten[(int)i] = tri;
                    }
                }
                private void ParseOffset()
                {
                    if (!CheckSegments(2)) { return; }

                    E_Offset = ParseIndex(LineSegments[0], "E", E_Offset);
                    S_Offset = ParseIndex(LineSegments[1], "S", S_Offset);
                }

                private void ParseQube()
                {
                    if (!CheckSegments(6)) { return; }

                    Point3D min = ParsePunkt(0);
                    Point3D max = ParsePunkt(3);

                    uint idx = (uint)Ecken.Count;

                    Ecken.Add(new Point3D(min.Y, min.X, min.C)); // 0 - - -
                    Ecken.Add(new Point3D(max.Y, min.X, min.C)); // 1 + - -
                    Ecken.Add(new Point3D(min.Y, min.X, max.C)); // 2 - - +
                    Ecken.Add(new Point3D(max.Y, min.X, max.C)); // 3 + - +

                    Ecken.Add(new Point3D(min.Y, max.X, min.C)); // 4 - + -
                    Ecken.Add(new Point3D(max.Y, max.X, min.C)); // 5 + + -
                    Ecken.Add(new Point3D(min.Y, max.X, max.C)); // 6 - + +
                    Ecken.Add(new Point3D(max.Y, max.X, max.C)); // 7 + + +

                    Seiten.Add(new Tri(idx + 0, idx + 3, idx + 1, 0xFFFFFFFF));
                    Seiten.Add(new Tri(idx + 0, idx + 2, idx + 3, 0xFFFFFFFF));
                    Seiten.Add(new Tri(idx + 0, idx + 1, idx + 5, 0xFFFFFFFF));
                    Seiten.Add(new Tri(idx + 0, idx + 5, idx + 4, 0xFFFFFFFF));
                    Seiten.Add(new Tri(idx + 0, idx + 6, idx + 2, 0xFFFFFFFF));
                    Seiten.Add(new Tri(idx + 0, idx + 4, idx + 6, 0xFFFFFFFF));

                    Seiten.Add(new Tri(idx + 1, idx + 3, idx + 7, 0xFFFFFFFF));
                    Seiten.Add(new Tri(idx + 5, idx + 1, idx + 7, 0xFFFFFFFF));
                    Seiten.Add(new Tri(idx + 3, idx + 2, idx + 7, 0xFFFFFFFF));
                    Seiten.Add(new Tri(idx + 2, idx + 6, idx + 7, 0xFFFFFFFF));
                    Seiten.Add(new Tri(idx + 4, idx + 5, idx + 7, 0xFFFFFFFF));
                    Seiten.Add(new Tri(idx + 6, idx + 4, idx + 7, 0xFFFFFFFF));
                }
                private void ParseRing()
                {
                    if ((LineSegments.Length % 2) != 0) { Error("Ring % 2 != 0"); return; }

                    int count = (LineSegments.Length) / 2;
                    int rel1, rel2;
                    int rel11, rel12, rel21, rel22;

                    uint[] idx = new uint[4];
                    for (int i = 0; i < count; i++)
                    {
                        rel1 = i;
                        rel2 = (i + 1) % count;

                        rel11 = rel1 + 0;
                        rel12 = rel2 + 0;
                        rel21 = rel1 + count;
                        rel22 = rel2 + count;

                        idx[0] = ParseIndex(LineSegments[rel11], "idx[0]", E_Offset);
                        idx[1] = ParseIndex(LineSegments[rel12], "idx[1]", E_Offset);
                        idx[2] = ParseIndex(LineSegments[rel21], "idx[2]", E_Offset);
                        idx[3] = ParseIndex(LineSegments[rel22], "idx[3]", E_Offset);

                        Seiten.Add(new Tri(idx[0], idx[1], idx[2], 0xFFFFFFFF));
                        Seiten.Add(new Tri(idx[2], idx[1], idx[3], 0xFFFFFFFF));
                    }
                }
                private void ParseFan()
                {
                    int count = LineSegments.Length - 1;
                    int rel1, rel2;

                    uint[] idx = new uint[3];
                    idx[0] = ParseIndex(LineSegments[count],    "idx[0]", E_Offset);
                    for (int i = 0; i < count; i++)
                    {
                        rel1 = i;
                        rel2 = (i + 1) % count;

                        idx[1] = ParseIndex(LineSegments[rel1], "idx[1]", E_Offset);
                        idx[2] = ParseIndex(LineSegments[rel2], "idx[2]", E_Offset);

                        Seiten.Add(new Tri(idx[0], idx[1], idx[2], 0xFFFFFFFF));
                    }
                }

                public void ParseLine()
                {
                    switch (LineType)
                    {
                        case 1: ParsePunkt(); break;
                        case 2: ParseDreieck(); break;
                        case 3: ParsePolygon(); break;
                        case 4: ParseColor1(); break;
                        case 5: ParseColorN(); break;
                        case 6: ParseKariert(); break;
                        case 7: ParseOffset(); break;

                        case 8: ParseQube(); break;
                        case 9: ParseRing(); break;
                        case 10: ParseFan(); break;
                    }
                }
            }
            public static BodyStatic Load(string path)
            {
                ConsoleLog.Log("Load Body: '" + path + "'");

                if (!System.IO.File.Exists(path))
                {
                    ConsoleLog.Log("Error: File '" + path + "' not found.");
                    return Create.ErrorBody();
                }

                try
                {
                    LoadData data = new LoadData(path);

                    while (data.fileIdx < data.fileTxt.Length)
                    {
                        data.SplitCheckLine();
                        data.ParseLine();

                        data.fileIdx++;
                    }

                    ConsoleLog.Log("Body loaded with " + data.Ecken.Count + ": Ecken and " + data.Seiten.Count + ": Seitn");

                    return new BodyStatic(data.Ecken, data.Seiten);
                }
                catch
                {

                }
                return Create.ErrorBody();
            }
        }
    }
}
