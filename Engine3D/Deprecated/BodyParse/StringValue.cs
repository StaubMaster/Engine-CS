using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine3D.StringParse;

namespace Engine3D.BodyParse
{
    /* types
     * float
     * index
     * point
     * angle
     * color
     * string
     * variable / operations
     * segment ?
     * 
     * how to implement
     * external
     *  {
     *      index,
     *      {
     *          float,
     *          float,
     *      },
     *      point
     *  }
     * internal
     *  index = { int }
     *  point = { float, float, flaot }
     * check()
     *  fixed   types need to be in the structure specified
     *  array   types can be any of an array
     */

    /* InLine to ExLine
     * singleValues
     * multiValues
     *  exline fixed
     *    check every template[i] with seg.child[i]
     *  exline array
     *    check every template[i] with seg
     *  inline fixed
     *    i = 0; j = 0; k = 0;
     *    {
     *      if (template[i] is singleValue)
     *        check template[i] with seg.child[j]
     *        i++; j++;
     *      else if (template[i] is exline fixed)
     *      
     *    }
     *  arbitrary length needs to be last
     *  put another thing in the constructor
     *  that specifies that everything outside of the array of the last need to be put here
     *  for fixed
     *  make special case so in the constructor it gets reconstructed to be just 1 big template array
     */
    [Obsolete("newer Version in StringInter", false)]
    abstract class StringValue
    {
        public abstract StringValue New();
        public abstract bool Check(StringHierarchy seg);
        public abstract void Parse(StringHierarchy seg);
        public abstract string ToLine(string tab);

        public static StringValue_AnyOfValue operator |(StringValue a, StringValue b)
        {
            return new StringValue_AnyOfValue(new StringValue[] { a, b });
        }
    }
    //  user | operator on things to create an array
    //  user & operator on things to create an fixed
    //  user ^ for Arbitrary
    //
    //  StringValue | StringValue Creats a AnyOfValue
    //  AnyOfValue | StringValue adds it to Template


    [Obsolete("newer Version in StringInter", false)]
    class StringValue_AllOfArray : StringValue
    {
        public StringValue[] Template;
        public StringValue Auxilary;
        public List<StringValue> Arbitrary;
        public StringValue_AllOfArray(StringValue[] template)
        {
            Template = template;
            Auxilary = null;
            Arbitrary = null;
        }
        public StringValue_AllOfArray(StringValue[] template, StringValue auxilary)
        {
            Template = template;
            Auxilary = auxilary;
            Arbitrary = new List<StringValue>();
        }
        public override StringValue New()
        {
            StringValue[] template = new StringValue[Template.Length];
            for (int i = 0; i < Template.Length; i++)
            {
                template[i] = Template[i].New();
            }

            if (Auxilary == null)
            {
                return new StringValue_AllOfArray(template);
            }
            else
            {
                return new StringValue_AllOfArray(template, Auxilary.New());
            }
        }
        public override bool Check(StringHierarchy seg)
        {
            if (seg.Child == null) { return false; }
            if (Auxilary == null)
            {
                if (Template.Length != seg.Child.Length) { return false; }
            }
            else
            {
                if (Template.Length > seg.Child.Length) { return false; }
            }
            for (int i = 0; i < Template.Length; i++)
            {
                if (!Template[i].Check(seg.Child[i])) { return false; }
            }
            if (Auxilary != null)
            {
                for (int i = Template.Length; i < seg.Child.Length; i++)
                {
                    if (!Auxilary.Check(seg.Child[i])) {return false; }
                }
            }
            return true;
        }
        public override void Parse(StringHierarchy seg)
        {
            for (int i = 0; i < Template.Length; i++)
            {
                Template[i].Parse(seg.Child[i]);
            }
            if (Auxilary != null)
            {
                for (int i = Template.Length; i < seg.Child.Length; i++)
                {
                    StringValue strVal = Auxilary.New();
                    strVal.Parse(seg.Child[i]);
                    Arbitrary.Add(strVal);
                }
            }
        }
        public override string ToLine(string tab)
        {
            string str = "";
            for (int i = 0; i < Template.Length; i++)
            {
                str += tab + Template[i].ToLine(" : ");
            }
            if (Arbitrary != null)
            {
                for (int i = 0; i < Arbitrary.Count; i++)
                {
                    str += tab + Arbitrary[i].ToLine(" : ");
                }
            }
            return str;
        }
    }



    /*  AnyOf Value vs Array ?
     */
    [Obsolete("newer Version in StringInter", false)]
    abstract class StringValue_AnyOf : StringValue
    {
        public StringValue[] Template;
        public StringValue_AnyOf(StringValue[] template)
        {
            Template = template;
        }
        protected int Find(StringHierarchy seg)
        {
            for (int i = 0; i < Template.Length; i++)
            {
                if (Template[i].Check(seg))
                {
                    return i;
                }
            }
            return -1;
        }
        protected StringValue[] NewTemplate()
        {
            StringValue[] template = new StringValue[Template.Length];
            for (int i = 0; i < Template.Length; i++)
            {
                template[i] = Template[i].New();
            }
            return template;
        }
    }
    [Obsolete("newer Version in StringInter", false)]
    class StringValue_AnyOfValue : StringValue_AnyOf
    {
        public StringValue Value;
        public StringValue_AnyOfValue(StringValue[] template) : base(template)
        {
            Value = null;
        }
        public override StringValue New()
        {
            return new StringValue_AnyOfValue(NewTemplate());
        }
        public override bool Check(StringHierarchy seg)
        {
            if (seg.Child != null) { return false; }
            if (Find(seg) == -1) { return false; }
            return true;
        }
        public override void Parse(StringHierarchy seg)
        {
            StringValue strVal = Template[Find(seg)].New();
            strVal.Parse(seg);
            Value = strVal;
        }
        public override string ToLine(string tab)
        {
            string str = "";
            if (Value != null)
            {
                str += Value.ToLine(" : ");
            }
            return str;
        }

        public static StringValue_AnyOfValue operator |(StringValue_AnyOfValue a, StringValue b)
        {
            StringValue[] template = new StringValue[a.Template.Length + 1];
            for (int i = 0; i < a.Template.Length; i++)
            {
                template[i] = a.Template[i];
            }
            template[a.Template.Length] = b;
            return new StringValue_AnyOfValue(template);
        }
    }
    [Obsolete("newer Version in StringInter", false)]
    class StringValue_AnyOfArray : StringValue_AnyOf
    {
        public List<StringValue> Arbitrary;
        public StringValue_AnyOfArray(StringValue[] template) : base(template)
        {
            Arbitrary = new List<StringValue>();
        }
        public override StringValue New()
        {
            return new StringValue_AnyOfArray(NewTemplate());
        }
        public override bool Check(StringHierarchy seg)
        {
            if (seg.Child == null) { return false; }
            for (int i = 0; i < seg.Child.Length; i++)
            {
                if (Find(seg.Child[i]) == -1) { return false; }
            }
            return true;
        }
        public override void Parse(StringHierarchy seg)
        {
            for (int i = 0; i < seg.Child.Length; i++)
            {
                StringValue strVal = Template[Find(seg.Child[i])].New();
                strVal.Parse(seg.Child[i]);
                Arbitrary.Add(strVal);
            }
        }
        public override string ToLine(string tab)
        {
            string str = "";
            for (int i = 0; i < Arbitrary.Count; i++)
            {
                //str += "[" + i + "] ";
                str += tab + Arbitrary[i].ToLine(" : ");
            }
            return str;
        }
    }



    [Obsolete("newer Version in StringInter", false)]
    class StringValue_Header : StringValue
    {
        public string Header;
        public StringValue_Header(string header)
        {
            Header = header;
        }
        public override StringValue New()
        {
            return new StringValue_Header(Header);
        }
        public override bool Check(StringHierarchy seg)
        {
            if (seg.Text.Length != Header.Length) { return false; }
            for (int i = 0; i < seg.Text.Length; i++)
            {
                if (seg.Text[i] != Header[i]) { return false; }
            }
            return true;
        }
        public override void Parse(StringHierarchy seg) { }
        public override string ToLine(string tab)
        {
            return "#" + Header + "#";
        }
    }
    [Obsolete("newer Version in StringInter", false)]
    class StringValue_String : StringValue
    {
        public string Text;
        public override StringValue New()
        {
            return new StringValue_String();
        }
        public override bool Check(StringHierarchy seg)
        {
            if (seg.Child != null) { return false; }
            return (seg.Text.Length != 0);
            //if (seg.Text.Length < 3) { return false; }
            //for (int i = 1; i < seg.Text.Length - 1; i++)
            //{
            //    if (seg.Text[i] == '"') { return false; }
            //}
            //return (seg.Text[0] == '"' && seg.Text[seg.Text.Length - 1] == '"');
        }
        public override void Parse(StringHierarchy seg)
        {
            Text = seg.Text;
        }
        public override string ToLine(string tab)
        {
            return '"' + Text + '"';
        }
    }

    [Obsolete("newer Version in StringInter", false)]
    class StringValue_Index : StringValue
    {
        public int Index;
        public bool IsRel;
        public override StringValue New()
        {
            return new StringValue_Index();
        }
        public override bool Check(StringHierarchy seg)
        {
            if (seg.Child != null) { return false; }
            return (THelp.StringIsNumber(seg.Text, "+-.!", "0123456789"));
        }
        public override void Parse(StringHierarchy seg)
        {
            IsRel = (THelp.CharIsAnyOf(seg.Text[0], "+-"));
            Index = int.Parse(seg.Text);
        }
        public override string ToLine(string tab)
        {
            string str = "";
            if (IsRel) { str += "~"; }
            str += "[" + Index.ToString() + "]";
            return str;
        }
    }
    /* Index Array
     *  is only given one child from the parent
     *  needs multiple arrays
     *  
     */

    [Obsolete("newer Version in StringInter", false)]
    class StringValue_Float : StringValue
    {
        public float Float;
        public override StringValue New()
        {
            return new StringValue_Float();
        }
        public override bool Check(StringHierarchy seg)
        {
            if (seg.Child != null) { return false; }
            return (THelp.StringIsNumber(seg.Text, "+-.!", "0123456789", ","));
        }
        public override void Parse(StringHierarchy seg)
        {
            Float = float.Parse(seg.Text);
        }
        public override string ToLine(string tab)
        {
            string str = "";
            str += "(" + Float.ToString() + ")";
            return str;
        }
    }
    [Obsolete("newer Version in StringInter", false)]
    class StringValue_Variable : StringValue
    {
        public string Name;
        public override StringValue New()
        {
            return new StringValue_Variable();
        }
        public override bool Check(StringHierarchy seg)
        {
            if (seg.Child != null) { return false; }
            for (int i = 0; i < seg.Text.Length; i++)
            {
                bool IsName = (
                    THelp.IsDigit(seg.Text[i]) ||
                    THelp.IsAlphaHi(seg.Text[i]) ||
                    THelp.IsAlphaLo(seg.Text[i]) ||
                    (seg.Text[i] == '_'));
                bool IsSign = (i == 0 && THelp.CharIsAnyOf(seg.Text[i], "+-.!"));

                if (!IsName && !IsSign) { return false; }
            }
            return true;
        }
        public override void Parse(StringHierarchy seg)
        {
            Name = seg.Text;
        }
        public override string ToLine(string tab)
        {
            string str = "";
            str += "<" + Name + ">";
            return str;
        }
    }
    /*Color 000 255*/



    /* how to turn strings into values
     * valueTypes can store a value of the type
     * convert string into value
     * Problem:
     *  multiple times: just reset/overwrite
     * Problem:
     *  arbitrary list
     * Solution:
     *  handle somewhere else
     * Fixed allways contains the same stuff
     * the only one that can have different stuff is the Array
     * Problem:
     *  duplicate ?
     *  if an array adds another element, then all data needs to be reset
     *  structs already copy ba value and not referance so this should be fine
     */
}
