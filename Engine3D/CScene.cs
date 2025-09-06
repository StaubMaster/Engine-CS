using System;
using System.Collections.Generic;
using System.IO;

using Engine3D.Abstract3D;
using Engine3D.OutPut.Uniform.Specific;
using Engine3D.StringParse;
using Engine3D.BodyParse;
using Engine3D.Graphics;

namespace Engine3D.Abstract3D
{
    /*  Load Multiple Bodys with Orientation as a Scene
        normal simpel inputs
        "file path" / specifys a new Body to load, all Bodys are kept in a List
        [0] +1 +2 +3 -0.3 -0.45 0 / places Body with index in [] at a position specified by the first 3 numbers, with a rotation specified by the next 3 numbers
    */
    /*
        more Complex stuff:

        for points, make them relative to the bounds of other objects
            specify from which object a relative point should be used
            when use <|> for each coordinate to specify
            if the point should be from the objects Min(<) Origin(|) or Max(>)
    */

    /*
        TODO:
            generalize these File Parsers
                Identify Quotes and dont split or "remove Comments" from them
    */

    [Obsolete("Use PolySoma instead", false)]
    public class CScene
    {
        public struct SSceneObject
        {
            public readonly PolyHedra Body;
            public readonly BodyElemBuffer Buffer;
            public Transformation3D Trans;
            public CUniformTransformation TransUni;

            public SSceneObject(PolyHedra body, BodyElemBuffer buffer, Point3D pos, Angle3D rot)
            {
                Body = body;
                Buffer = buffer;
                Trans = new Transformation3D(pos, rot);
                TransUni = new CUniformTransformation(pos, rot);
            }
        }
        public struct SSceneTemplate
        {
            public List<PolyHedra> BodyList;
            public List<BodyElemBuffer> BufferList;
            public List<SSceneObject> ObjectsList;
            public List<Point3D> MemPoint;

            public SSceneTemplate(int _)
            {
                BodyList = new List<PolyHedra>();
                BufferList = new List<BodyElemBuffer>();
                ObjectsList = new List<SSceneObject>();
                MemPoint = new List<Point3D>();
            }

            public void Insert(string path)
            {
                PolyHedra body = TBodyFile.LoadTextFile(path);
                BodyList.Add(body);
                BufferList.Add(body.ToBuffer());
            }
            public void Insert(int idx, Point3D pos, Angle3D rot)
            {
                ObjectsList.Add(new SSceneObject(BodyList[idx], BufferList[idx], pos, rot));
            }
        }

        private static SSceneTemplate template;
        private static string Dir;

        private class EUnknownType : Exception
        {
            public EUnknownType(string type) : base(
                "Unkown Type '" + type + "'."
                )
            { }
        }
        private class EInvalidSegmentCount : Exception
        {
            public EInvalidSegmentCount(int expected, int recieved) : base(
                "Invalid Segment Count, Expected " + expected + ", recieved " + recieved + "."
                )
            { }
        }

        /* create new scene like with body
         * this time dont try to redo the old file parser
         * make a new one
         */

        public static SSceneTemplate LoadTemplate(string path)
        {
            template = new SSceneTemplate(1);
            //ConsoleLog.Log("==== Scene ====");
            ConsoleLog.LogInfo("load CScene " + path);

            int i = -1;

            try
            {
                CDataHierarchyChildren mainDataHier = CFileInterpreter.FileToHeierarchy(path);
                //ConsoleLog.Log(mainDataHier.ToStringGraph());

                SHierarchyTemplate loadTemplate = new SHierarchyTemplate(new SHierarchyTemplate[]
                {
                    new SHierarchyTemplate(),
                    new SHierarchyTemplate(),
                });
                SHierarchyTemplate AddIdxPntWnkTemplate = new SHierarchyTemplate(new SHierarchyTemplate[]
                {
                    new SHierarchyTemplate(),
                    new SHierarchyTemplate(new SHierarchyTemplate[] {
                        new SHierarchyTemplate(),
                    }),
                    new SHierarchyTemplate(new SHierarchyTemplate[] {
                        new SHierarchyTemplate(),
                        new SHierarchyTemplate(),
                        new SHierarchyTemplate(),
                    }),
                    new SHierarchyTemplate(new SHierarchyTemplate[] {
                        new SHierarchyTemplate(),
                        new SHierarchyTemplate(),
                        new SHierarchyTemplate(),
                    }),
                });
                SHierarchyTemplate AddIdxMemPntWnkTemplate = new SHierarchyTemplate(new SHierarchyTemplate[]
                {
                    new SHierarchyTemplate(),
                    new SHierarchyTemplate(new SHierarchyTemplate[] {
                        new SHierarchyTemplate(),
                    }),
                    new SHierarchyTemplate(new SHierarchyTemplate[] {
                        new SHierarchyTemplate(),
                    }),
                    new SHierarchyTemplate(new SHierarchyTemplate[] {
                        new SHierarchyTemplate(),
                        new SHierarchyTemplate(),
                        new SHierarchyTemplate(),
                    }),
                });

                SHierarchyTemplate MemIdxPntTemplate = new SHierarchyTemplate(new SHierarchyTemplate[]
                {
                    new SHierarchyTemplate(),
                    new SHierarchyTemplate(new SHierarchyTemplate[] {
                        new SHierarchyTemplate(),
                    }),
                    new SHierarchyTemplate(new SHierarchyTemplate[] {
                        new SHierarchyTemplate(),
                        new SHierarchyTemplate(),
                        new SHierarchyTemplate(),
                    }),
                });

                for (i = 0; i < mainDataHier.Child.Count; i++)
                {
                    if (loadTemplate.Compare(mainDataHier.Child[i]))
                    {
                        //ConsoleLog.Log("is Dir/Load");

                        CDataHierarchyChildren data = (CDataHierarchyChildren)mainDataHier.Child[i];
                        CDataHierarchyString data_0 = (CDataHierarchyString)data.Child[0];
                        CDataHierarchyString data_1 = (CDataHierarchyString)data.Child[1];

                        //ConsoleLog.Log("Command: " + data_0.Str);
                        //ConsoleLog.Log("String: " + data_1.Str);

                        if (data_0.Str == "dir")
                        {
                            Dir = data_1.Str;
                        }
                        else if (data_0.Str == "load")
                        {
                            template.Insert(Dir + data_1.Str);
                        }
                        else
                        {
                            ConsoleLog.Log("Unknown Command: " + data_0.Str);
                        }
                    }
                    else if (AddIdxPntWnkTemplate.Compare(mainDataHier.Child[i]))
                    {
                        //ConsoleLog.Log("is Add");

                        CDataHierarchyChildren data = (CDataHierarchyChildren)mainDataHier.Child[i];
                        CDataHierarchyString data_0 = (CDataHierarchyString)data.Child[0];
                        CDataHierarchyChildren data_1 = (CDataHierarchyChildren)data.Child[1];
                        CDataHierarchyChildren data_2 = (CDataHierarchyChildren)data.Child[2];
                        CDataHierarchyChildren data_3 = (CDataHierarchyChildren)data.Child[3];
                        CDataHierarchyString data_1_0 = (CDataHierarchyString)data_1.Child[0];
                        CDataHierarchyString data_2_0 = (CDataHierarchyString)data_2.Child[0];
                        CDataHierarchyString data_2_1 = (CDataHierarchyString)data_2.Child[1];
                        CDataHierarchyString data_2_2 = (CDataHierarchyString)data_2.Child[2];
                        CDataHierarchyString data_3_0 = (CDataHierarchyString)data_3.Child[0];
                        CDataHierarchyString data_3_1 = (CDataHierarchyString)data_3.Child[1];
                        CDataHierarchyString data_3_2 = (CDataHierarchyString)data_3.Child[2];

                        //ConsoleLog.Log("Command: " + data_0.Str);
                        //ConsoleLog.Log("Index: " + data_1_0.Str);
                        //ConsoleLog.Log("Punkt: " + data_2_0.Str + " " + data_2_1.Str + " " + data_2_2.Str);
                        //ConsoleLog.Log("Winkl: " + data_3_0.Str + " " + data_3_1.Str + " " + data_3_2.Str);

                        int idx;
                        idx = int.Parse(data_1_0.Str);
                        float y, x, c;
                        y = float.Parse(data_2_0.Str);
                        x = float.Parse(data_2_1.Str);
                        c = float.Parse(data_2_2.Str);
                        float a, s, d;
                        a = float.Parse(data_3_0.Str);
                        s = float.Parse(data_3_1.Str);
                        d = float.Parse(data_3_2.Str);

                        if (data_0.Str == "place")
                        {
                            template.Insert(idx, new Point3D(y, x, c), Angle3D.FromDegree(a, s, d));
                        }
                        else
                        {
                            ConsoleLog.Log("Unknown Command: " + data_0.Str);
                        }
                    }
                    else if (AddIdxMemPntWnkTemplate.Compare(mainDataHier.Child[i]))
                    {
                        //ConsoleLog.Log("is Place");

                        CDataHierarchyChildren data = (CDataHierarchyChildren)mainDataHier.Child[i];
                        CDataHierarchyString data_0 = (CDataHierarchyString)data.Child[0];
                        CDataHierarchyChildren data_1 = (CDataHierarchyChildren)data.Child[1];
                        CDataHierarchyChildren data_2 = (CDataHierarchyChildren)data.Child[2];
                        CDataHierarchyChildren data_3 = (CDataHierarchyChildren)data.Child[3];
                        CDataHierarchyString data_1_0 = (CDataHierarchyString)data_1.Child[0];
                        CDataHierarchyString data_2_0 = (CDataHierarchyString)data_2.Child[0];
                        CDataHierarchyString data_3_0 = (CDataHierarchyString)data_3.Child[0];
                        CDataHierarchyString data_3_1 = (CDataHierarchyString)data_3.Child[1];
                        CDataHierarchyString data_3_2 = (CDataHierarchyString)data_3.Child[2];

                        //ConsoleLog.Log("Command: " + data_0.Str);
                        //ConsoleLog.Log("Index: " + data_1_0.Str);
                        //ConsoleLog.Log("Punkt: " + data_2_0.Str);
                        //ConsoleLog.Log("Winkl: " + data_3_0.Str + " " + data_3_1.Str + " " + data_3_2.Str);

                        int idx;
                        idx = int.Parse(data_1_0.Str);
                        int pnt;
                        pnt = int.Parse(data_2_0.Str);
                        float a, s, d;
                        a = float.Parse(data_3_0.Str);
                        s = float.Parse(data_3_1.Str);
                        d = float.Parse(data_3_2.Str);

                        if (data_0.Str == "place")
                        {
                            template.Insert(idx, template.MemPoint[pnt], Angle3D.FromDegree(a, s, d));
                        }
                        else
                        {
                            ConsoleLog.Log("Unknown Command: " + data_0.Str);
                        }
                    }
                    else if (MemIdxPntTemplate.Compare(mainDataHier.Child[i]))
                    {
                        //ConsoleLog.Log("is Mem");

                        CDataHierarchyChildren data = (CDataHierarchyChildren)mainDataHier.Child[i];
                        CDataHierarchyString data_0 = (CDataHierarchyString)data.Child[0];
                        CDataHierarchyChildren data_1 = (CDataHierarchyChildren)data.Child[1];
                        CDataHierarchyChildren data_2 = (CDataHierarchyChildren)data.Child[2];
                        CDataHierarchyString data_1_0 = (CDataHierarchyString)data_1.Child[0];
                        CDataHierarchyString data_2_0 = (CDataHierarchyString)data_2.Child[0];
                        CDataHierarchyString data_2_1 = (CDataHierarchyString)data_2.Child[1];
                        CDataHierarchyString data_2_2 = (CDataHierarchyString)data_2.Child[2];

                        //ConsoleLog.Log("Command: " + data_0.Str);
                        //ConsoleLog.Log("Index: " + data_1_0.Str);
                        //ConsoleLog.Log("Punkt: " + data_2_0.Str + " " + data_2_1.Str + " " + data_2_2.Str);

                        int idx;
                        idx = int.Parse(data_1_0.Str);
                        float y, x, c;
                        y = float.Parse(data_2_0.Str);
                        x = float.Parse(data_2_1.Str);
                        c = float.Parse(data_2_2.Str);

                        if (data_0.Str == "memSet")
                        {
                            if (idx == -1)
                            {
                                template.MemPoint.Add(new Point3D(y, x, c));
                            }
                            else
                            {
                                template.MemPoint[idx] = new Point3D(y, x, c);
                            }
                        }
                        else if (data_0.Str == "memAdd")
                        {
                            template.MemPoint[idx] += new Point3D(y, x, c);
                        }
                        else
                        {
                            ConsoleLog.Log("Unknown Command: " + data_0.Str);
                        }
                    }
                    else
                    {
                        ConsoleLog.Log("Unknown Template");
                    }
                    //ConsoleLog.Log("");
                }
            }
            catch (Exception ex)
            {
                //ConsoleLog.Log("==== Error ====");
                //ConsoleLog.Log("Index: " + (i));
                //ConsoleLog.Log(ex.Message);
                ConsoleLog.LogError("Index:" + i + ":" + ex.Message);
            }

            return template;
        }
    }
}
