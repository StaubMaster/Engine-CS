using System;
using System.Collections.Generic;

using Engine3D.Graphics;
using Engine3D.OutPut.Uniform;
using Engine3D.OutPut.Uniform.Specific;
using Engine3D.Graphics.Display;
using Engine3D.Miscellaneous;

using Engine3D.TextParser;
using Engine3D.TextParser.Sectonizer;
using Engine3D.TextParser.Checker;
using Engine3D.Miscellaneous.StringHelp;
using Engine3D.Miscellaneous.EntryContainer;
using Engine3D.Graphics.Display3D;

namespace Engine3D.Abstract3D
{
    public class PolySoma
    {
        private struct BodyStatic
        {
            private PolyHedra Poly;
            private BodyElemBuffer Buffer;
            private Transformation3D Trans;

            public BodyStatic(PolyHedra poly, BodyElemBuffer buffer, Transformation3D trans)
            {
                Poly = poly;
                Buffer = buffer;
                Trans = trans;
            }
            public void Draw(ref SUniformLocation transUni)
            {
                transUni.Value(new CUniformTransformation(Trans));
                Buffer.Draw();
            }
            public void Draw(BodyElemUniShader shader)
            {
                shader.Trans.Value(Trans);
                Buffer.Draw();
            }
        }



        public ArrayList<DisplayPolyHedra> AllPolyHedras;
        public ArrayList<DisplayBody> AllBodysOld;

        private List<PolyHedraInstance_3D_BufferData> AllBodys;
        private List<EntryContainerDynamic<PolyHedraInstance_3D_Data>.Entry> AllTrans;

        private string DirPath;

        private bool IsEdit;

        public PolySoma()
        {
            AllPolyHedras = new ArrayList<DisplayPolyHedra>();
            AllBodysOld = new ArrayList<DisplayBody>();

            AllBodys = new List<PolyHedraInstance_3D_BufferData>();
            AllTrans = new List<EntryContainerBase<PolyHedraInstance_3D_Data>.Entry>();

            DirPath = "";

            IsEdit = false;
        }

        public void Edit_Begin()
        {
            if (IsEdit) { return; }
            AllPolyHedras.EditBegin();
            AllBodysOld.EditBegin();
            IsEdit = true;
        }
        public void Edit_Stop()
        {
            if (!IsEdit) { return; }
            AllPolyHedras.EditEnd();
            AllBodysOld.EditEnd();
            IsEdit = false;
        }

        public void Edit_Insert_Body(Transformation3D trans)
        {
            AllBodysOld.Insert(new DisplayBody(AllPolyHedras[AllPolyHedras.Count - 1], trans));
            AllTrans.Add(AllBodys[AllBodys.Count - 1].Alloc(1));
        }
        public void Edit_Remove_Body(int idx)
        {
            AllBodysOld.Remove(idx);
            AllTrans[idx].Dispose();
            AllTrans.RemoveAt(idx);
        }

        public void Edit_Change_Dir(string dir)
        {
            DirPath = dir;
        }
        public void Edit_Insert_PolyRel(string path)
        {
            AllPolyHedras.Insert(new DisplayPolyHedra(DirPath + path));
            AllBodys.Add(new PolyHedraInstance_3D_BufferData(DirPath + path));
        }
        public void Edit_Insert_PolyAbs(string path)
        {
            AllPolyHedras.Insert(new DisplayPolyHedra(path));
            AllBodys.Add(new PolyHedraInstance_3D_BufferData(path));
        }



        public void Update()
        {
            for (int i = 0; i < AllBodys.Count; i++)
            {
                AllBodys[i].DataUpdate();
            }
        }
        public void DrawInst()
        {
            for (int i = 0; i < AllBodys.Count; i++)
            {
                AllBodys[i].DrawInst();
            }
        }



        public Intersekt.RayInterval Intersekt(Ray3D ray, out int idx)
        {
            Intersekt.RayInterval intersekt = new Intersekt.RayInterval(ray);
            idx = -1;

            for (int i = 0; i < AllBodysOld.Count; i++)
            {
                Intersekt.RayInterval inter = AllBodysOld[i].Intersekt(ray);
                if (inter.Is)
                {
                    if (inter.Interval < intersekt.Interval)
                    {
                        intersekt = inter;
                        idx = i;
                    }
                }
            }

            return intersekt;
        }
        private string AllPlaced(DisplayPolyHedra poly)
        {
            string str = "";
            for (int i = 0; i < AllBodysOld.Count; i++)
            {
                if (AllBodysOld[i].Body == poly)
                {
                    str += AllBodysOld[i].ToYMT();
                }
            }
            return str;
        }
        public string ToYMT()
        {
            string str = "";
            str += "Format PolySoma;\r\n";
            str += "Parse Manual;\r\n";
            /*for (int i = 0; i < AllPolyHedras.Count; i++)
            {
                str += "\r\n";
                str += AllPolyHedras[i].ToYMT();
                str += AllPlaced(AllPolyHedras[i]);
            }*/
            return str;
        }



        public static class Parse
        {
            private static void FuncFormat(Section section)
            {
                //ConsoleLog.Log("##  Format  ##");
                //section.ToConsole(false, false, "  ");
                //ConsoleLog.Log("##  ####  ##");

                if (FormatDynamic != null)
                {
                    FormatDynamic.Structure = new HierarchyMultiChoice(new Hierarchy[]
                    {
                        new HierarchyCommand(new HierarchyFixed(new Hierarchy[]
                        {
                            new HierarchyHeader("dir"),
                            new HierarchyString(),
                            new HierarchyTermination(),
                        }), FuncDir),

                        new HierarchyCommand(new HierarchyFixed(new Hierarchy[]
                        {
                            new HierarchyHeader("body"),
                            new HierarchyString(),
                            new HierarchyTermination(),
                        }), FuncBody),

                        new HierarchyCommand(new HierarchyFixed(new Hierarchy[]
                        {
                            new HierarchyHeader("place"),
                            new HierarchyNewLayer(new HierarchyFixed(new Hierarchy[]
                            {
                                new HierarchyNewLayer(new HierarchyNumber()),
                                new HierarchyNewLayer(new HierarchyNumber()),
                                new HierarchyNewLayer(new HierarchyNumber()),
                            })),
                            new HierarchyNewLayer(new HierarchyFixed(new Hierarchy[]
                            {
                                new HierarchyNewLayer(new HierarchyNumber()),
                                new HierarchyNewLayer(new HierarchyNumber()),
                                new HierarchyNewLayer(new HierarchyNumber()),
                            })),
                            new HierarchyTermination(),
                        }), FuncPlace),
                    });
                }
            }
            private static void FuncParse(Section section)
            {
                //ConsoleLog.Log("##  Parse  ##");
                //section.ToConsole(false, false, "  ");
                //ConsoleLog.Log("##  ####  ##");
            }

            private static void FuncDir(Section section)
            {
                //ConsoleLog.Log("##  Dir  ##");
                //section.ToConsole(false, false, "  ");
                //ConsoleLog.Log("##  ####  ##");
                MainTemplate.Edit_Change_Dir(section.Sections[1].Cut());
            }

            private static void FuncBody(Section section)
            {
                //ConsoleLog.Log("##  Body  ##");
                //section.ToConsole(false, false, "  ");
                //ConsoleLog.Log("##  ####  ##");
                MainTemplate.Edit_Insert_PolyRel(section.Sections[1].Cut());
            }
            private static void FuncPlace(Section section)
            {
                //ConsoleLog.Log("##  Place  ##");
                //section.ToConsole(false, false, "  ");
                //ConsoleLog.Log("##  ####  ##");

                string y = section.Sections[1].Sections[0].Cut();
                string x = section.Sections[1].Sections[1].Cut();
                string c = section.Sections[1].Sections[2].Cut();
                //ConsoleLog.Log(y + " : " + x + " : " + c);
                Point3D pos = new Point3D(
                    float.Parse(y),
                    float.Parse(x),
                    float.Parse(c)
                    );

                string a = section.Sections[2].Sections[0].Cut();
                string s = section.Sections[2].Sections[1].Cut();
                string d = section.Sections[2].Sections[2].Cut();
                //ConsoleLog.Log(a + " : " + s + " : " + d);
                Angle3D rot = Angle3D.FromDegree(
                    float.Parse(a),
                    float.Parse(s),
                    float.Parse(d)
                    );

                MainTemplate.Edit_Insert_Body(new Transformation3D(pos, rot));
            }



            private static PolySoma MainTemplate;
            private static VariableManager VarMan;
            private static HierarchyDynamic FormatDynamic;
            private static PolySoma LayerSplit(string text)
            {
                Layer layer = new Layer(text, Splitter.Default());
                //layer.Show(false, false);

                VarMan = new VariableManager();

                FormatDynamic = new HierarchyDynamic();
                Hierarchy hierarchy = new HierarchyLoop(new HierarchyNewLayer(new HierarchyMultiChoice(new Hierarchy[]
                {
                    /*new HierarchyCommand(new HierarchyFixed(new Hierarchy[]
                    {
                        new HierarchyHeader("var"),
                        new HierarchyVariable(),
                        new HierarchyMultiChoice(new Hierarchy[]
                        {
                            VariableManager.NumberMath,
                            VariableManager.PointMath,
                        }),
                    }), VarMan.Change),*/

                    new HierarchyCommand(new HierarchyFixed(new Hierarchy[]
                    {
                        new HierarchyHeader("Format"),
                        new HierarchyHeader("PolySoma"),
                        new HierarchyTermination(),
                    }), FuncFormat),
                    new HierarchyCommand(new HierarchyFixed(new Hierarchy[]
                    {
                        new HierarchyHeader("Parse"),
                        new HierarchyHeader("Manual"),
                        new HierarchyTermination(),
                    }), FuncParse),

                    FormatDynamic,
                })));



                //Hierarchy.IsLog = true;
                MainTemplate = new PolySoma();
                MainTemplate.Edit_Begin();
                int off = 0;
                if (hierarchy.Check(layer.SectionMain, ref off))
                {
                    ConsoleLog.LogSuccess("Main Check");
                    MainTemplate.Edit_Stop();
                }
                else
                {
                    ConsoleLog.LogFailure("Main Check");
                    layer.SectionMain.ToConsole(false, false, "");
                }
                return MainTemplate;
            }



            public static PolySoma LoadText(string text)
            {
                //text = TextIterator.TrimComments(text);
                //ConsoleLog.Log("################");
                //ConsoleLog.Log(text);
                //ConsoleLog.Log("################");
                //text = TextIterator.TrimWhiteSpace(text);
                //ConsoleLog.Log("################");
                //ConsoleLog.Log(text);
                //ConsoleLog.Log("################");
                //text = TextIterator.TrimControls(text);
                //ConsoleLog.NonPrint(text);
                //ConsoleLog.Log("");

                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

                PolySoma polysoma = LayerSplit(text);

                stopwatch.Stop();
                ConsoleLog.Log("PolySoma Time: " + stopwatch.ElapsedMilliseconds + "ms");

                return polysoma;
            }
            public static PolySoma LoadFile(string path)
            {
                return LoadText(System.IO.File.ReadAllText(path));
            }
        }
    }
}
