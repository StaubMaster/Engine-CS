using System;
using System.Collections.Generic;

using Engine3D.StringParse;

namespace Engine3D.BodyParse
{
    public static class TNamedMemory
    {
        private abstract class Variable
        {
            public string Name;

            public static bool IsChange(string[] segs)
            {
                return (segs.Length >= 2 && segs[1] == "=");
            }
            public static string ExtractSign(string str, out char sign)
            {
                if (THelp.CharIsAnyOf(str[0], "+-!."))
                {
                    sign = str[0];
                    return str.Substring(1);
                }
                sign = '.';
                return str;
            }
            public static bool IsNumber(string seg)
            {
                return THelp.StringIsOnly(seg, "0123456789,");
            }
        }
        private class VariableFloat : Variable
        {
            public float Value;
            public VariableFloat(string name, float value)
            {
                Name = name;
                Value = value;
            }
            public override string ToString()
            {
                return Name + " '" + Value + "'";
            }
        }
        public class Context
        {
            private List<VariableFloat> VariableFList;

            public Context()
            {
                VariableFList = new List<VariableFloat>();
            }

            private VariableFloat FindVariableF(string name)
            {
                for (int i = 0; i < VariableFList.Count; i++)
                {
                    if (VariableFList[i].Name == name)
                    {
                        return VariableFList[i];
                    }
                }
                return null;
            }

            private float SetValueF(string name, float value)
            {
                VariableFloat var = FindVariableF(name);
                if (var == null) { VariableFList.Add(new VariableFloat(name, value)); }
                else { var.Value = value; }
                return value;
            }
            private float GetValueF(string name)
            {
                VariableFloat var = FindVariableF(name);
                if (var == null) { throw new EVariableNotFound(name); }
                return var.Value;
            }

            private float ValueF(string str)
            {
                str = Variable.ExtractSign(str, out char sign);

                float val;
                if (Variable.IsNumber(str))
                {
                    val = float.Parse(str);
                }
                else
                {
                    val = GetValueF(str);
                }

                if (sign == '!') { return -val; }
                else if (sign == '+') { return +Math.Abs(val); }
                else if (sign == '-') { return -Math.Abs(val); }
                return val;
            }
            private float ChangeF(string[] segs)
            {
                string variable_name = segs[0];
                string[] variable_math = new string[segs.Length - 2];
                for (int i = 0; i < variable_math.Length; i++)
                {
                    variable_math[i] = segs[i + 2];
                }
                return SetValueF(variable_name, ParseF(variable_math));
            }
            private float CalcF(string[] segs)
            {
                if (segs.Length % 2 != 1)
                {
                    throw new ESegmentCount();
                }

                float temp = ValueF(segs[0]);
                for (int i = 1; i < segs.Length; i += 2)
                {
                    string op = segs[i + 0];
                    string vl = segs[i + 1];
                    if (op == "+") { temp += ValueF(vl); }
                    else if (op == "-") { temp -= ValueF(vl); }
                    else
                    {
                        throw new EOperatorUnknown(op);
                    }
                }
                return temp;
            }
            private float ParseF(string[] segs)
            {
                if (Variable.IsChange(segs))
                {
                    return ChangeF(segs);
                }
                else
                {
                    return CalcF(segs);
                }
            }
            public float ParseF(string str)
            {
                return ParseF(str.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            }

            public override string ToString()
            {
                string str = "";
                for (int i = 0; i < VariableFList.Count; i++)
                    str += VariableFList[i] + "\n";
                return str;
            }



            private static int CalcI(string[] segs)
            {
                if (segs.Length % 2 != 1)
                {
                    throw new ESegmentCount();
                }

                int temp = int.Parse(segs[0]);
                for (int i = 1; i < segs.Length; i += 2)
                {
                    string op = segs[i + 0];
                    string vl = segs[i + 1];
                    if (op == "+") { temp += int.Parse(vl); }
                    else if (op == "-") { temp -= int.Parse(vl); }
                    else
                    {
                        throw new EOperatorUnknown(op);
                    }
                }
                return temp;
            }
            public static int ParseI(string str)
            {
                return CalcI(str.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            }



            private class ESegmentCount : Exception
            {
                public ESegmentCount() : base("Number of Segments must be Odd.") { }
            }
            private class EOperatorUnknown : Exception
            {
                public EOperatorUnknown(string op) : base("Operator '" + op + "' is Unknown.") { }
            }
            private class EVariableNotFound : Exception
            {
                public EVariableNotFound(string name) : base("Variable '" + name + "' not found.") { }
            }
        }
    }
}
