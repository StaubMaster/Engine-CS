using System.Collections.Generic;

using Engine3D.Abstract3D;
using Engine3D.TextParser.Checker;
using Engine3D.TextParser.Sectonizer;

namespace Engine3D.TextParser
{
    class VariableManager
    {
        public static Hierarchy NumberOrVar = new HierarchyFixed(new Hierarchy[]
        {
            new HierarchyMultiChoice(new Hierarchy[]
            {
                new HierarchyNumber(),
                new HierarchyVariable(),
            }),
        });
        public static Hierarchy NumberMath = new HierarchyFixed(new Hierarchy[]
        {
            NumberOrVar,
            new HierarchyRepeat(new HierarchyFixed(new Hierarchy[]
            {
                new HierarchyMultiChoice(new Hierarchy[]
                {
                    new HierarchyHeader("+"),
                    new HierarchyHeader("-"),
                }),
                NumberOrVar,
            })),
            new HierarchyTermination(),
        });

        public static Hierarchy Point = new HierarchyFixed(new Hierarchy[]
        {
            new HierarchyHeader("point"),
            new HierarchyNewLayer(new HierarchyFixed(new Hierarchy[]
            {
                new HierarchyNewLayer(NumberMath),
                new HierarchyNewLayer(NumberMath),
                new HierarchyNewLayer(NumberMath),
            })),
        });
        public static Hierarchy PointOrVar = new HierarchyFixed(new Hierarchy[]
        {
            new HierarchyMultiChoice(new Hierarchy[]
            {
                Point,
                new HierarchyVariable(),
            }),
        });
        public static Hierarchy PointMath = new HierarchyFixed(new Hierarchy[]
        {
            PointOrVar,
            new HierarchyRepeat(new HierarchyFixed(new Hierarchy[]
            {
                new HierarchyMultiChoice(new Hierarchy[]
                {
                    new HierarchyHeader("+"),
                    new HierarchyHeader("-"),
                }),
                PointOrVar,
            })),
            new HierarchyTermination(),
        });





        public class Variable
        {
            public readonly string Name;
            protected Variable(string name)
            {
                Name = name;
            }
        }
        public class VarNumber : Variable
        {
            public readonly float Value;
            public VarNumber(string name, float value) : base(name)
            {
                Value = value;
            }
        }
        public class VarPoint : Variable
        {
            public readonly Point3D Value;
            public VarPoint(string name, Point3D value) : base(name)
            {
                Value = value;
            }
        }



        private List<Variable> Variables;
        public VariableManager()
        {
            Variables = new List<Variable>();
        }



        private Variable Find(string name)
        {
            for (int i = Variables.Count - 1; i >= 0; i--)
            {
                if (Variables[i].Name == name)
                {
                    return Variables[i];
                }
            }
            return null;
        }

        private void Remove(string name)
        {
            for (int i = 0; i < Variables.Count; i++)
            {
                if (Variables[i].Name == name)
                {
                    Variables.RemoveAt(i);
                    i--;
                }
            }
        }
        private void Insert(string name, float value)
        {
            Variables.Add(new VarNumber(name, value));
        }



        private float Number_Literal(Section s)
        {
            float val = float.Parse(s.Cut());
            return val;
        }
        private float Number_Variable(Section s)
        {
            float val;
            Variable va = Find(s.Cut());
            if (va.GetType() == typeof(VarNumber))
            {
                val = ((VarNumber)va).Value;
            }
            else
            {
                val = float.NaN;
                ConsoleLog.LogError("Variable Wrong Type");
            }
            return val;
        }
        private float Number_LitOrVar(Section section, ref int offset)
        {
            Hierarchy numberLit = new HierarchyNumber();
            Hierarchy numberVar = new HierarchyVariable();

            Section s = section.Sections[offset];

            float val;
            if (numberLit.Check(section, ref offset))
            {
                val = Number_Literal(s);
                ConsoleLog.Log("Offset " + offset + " Number " + val);
            }
            else if (numberVar.Check(section, ref offset))
            {
                val = Number_Variable(s);
                ConsoleLog.Log("Offset " + offset + " Variable");
            }
            else
            {
                val = float.NaN;
                ConsoleLog.LogError("Value not Extracted");
            }

            return val;
        }
        private float Number_Calculate(Section section, ref int offset)
        {
            Hierarchy opAdd = new HierarchyHeader("+");
            Hierarchy opSub = new HierarchyHeader("-");

            float val = Number_LitOrVar(section, ref offset);
            while (offset < section.Sections.Count)
            {
                byte op = 255;
                if (opAdd.Check(section, ref offset))
                {
                    ConsoleLog.Log("Offset " + offset + " Add");
                    op = 1;
                }
                else if (opSub.Check(section, ref offset))
                {
                    ConsoleLog.Log("Offset " + offset + " Sub");
                    op = 2;
                }
                else
                {
                    ConsoleLog.LogError("Operator not Extracted");
                }

                float v = Number_LitOrVar(section, ref offset);

                if (op == 1) { val = val + v; }
                if (op == 2) { val = val - v; }
            }

            return val;
        }



        public void Change(Section section)
        {
            ConsoleLog.Log("##  VariableManager.Change  ##");
            section.ToConsole(false, false, "  ");
            ConsoleLog.Log("##  ####  ##");

            int offset;
            offset = 2;
            if (NumberMath.Check(section, ref offset))
            {
                ConsoleLog.Log("Offset " + offset + " NumberMath");

                string name = section.Sections[1].Cut();

                offset = 2;
                float val = Number_Calculate(section, ref offset);

                ConsoleLog.LogInfo("Variable " + name + " " + val);
                Remove(name);
                Insert(name, val);
            }
        }
    }
}
