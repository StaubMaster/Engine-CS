using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine3D.StringParse
{
    [Obsolete("newer Version in StringInter", false)]
    public abstract class ADataHierarchy
    {
        public abstract string ToStringGraph(string tab, bool last);

        /*public static bool isParent(ADataHierarchy hir)
        {
            return (hir.GetType() == typeof(CDataChildren));
        }*/
        public virtual bool isParent() { return false; }
        /*public static bool isString(ADataHierarchy hir)
        {
            return (hir.GetType() == typeof(CDataString));
        }*/
        public virtual bool isString() { return false; }
    }
    [Obsolete("newer Version in StringInter", false)]
    public class CDataHierarchyChildren : ADataHierarchy
    {
        public CDataHierarchyChildren Parent;
        public List<ADataHierarchy> Child;

        public CDataHierarchyChildren(CDataHierarchyChildren parent)
        {
            Parent = parent;
            Child = new List<ADataHierarchy>();
        }
        public override bool isParent() { return true; }

        public override string ToStringGraph(string tab = "", bool last = true)
        {
            string str = "";

            if (!last)
                str += tab + "┣━" + "┳━[" + Child.Count + "]\n";
            else
                str += tab + "┗━" + "┳━[" + Child.Count + "]\n";

            for (int i = 0; i < Child.Count; i++)
            {
                if (!last)
                    str += Child[i].ToStringGraph(tab + "┃ ", i == Child.Count - 1);
                else
                    str += Child[i].ToStringGraph(tab + "  ", i == Child.Count - 1);
            }

            return str;
        }
    }
    [Obsolete("newer Version in StringInter", false)]
    public class CDataHierarchyString : ADataHierarchy
    {
        public string Str;

        public CDataHierarchyString(string str)
        {
            Str = str;
        }
        public override bool isString() { return true; }

        public override string ToStringGraph(string tab, bool last)
        {
            if (!last)
                return tab + "┣━" + Str + "\n";
            else
                return tab + "┗━" + Str + "\n";
        }
    }



    [Obsolete("newer Version in StringInter", false)]
    public struct SHierarchyTemplate
    {
        public SHierarchyTemplate[] Child;
        public SHierarchyTemplate(SHierarchyTemplate[] child)
        {
            Child = child;
        }

        public bool Compare(ADataHierarchy hir)
        {
            if (Child != null)
            {
                //ConsoleLog.Log("? should be Parent");
                if (!hir.isParent())
                {
                    //ConsoleLog.Log("! is not Parent");
                    return false;
                }

                //ConsoleLog.Log("? Num should be " + Child.Length);
                CDataHierarchyChildren c = (CDataHierarchyChildren)hir;
                if (Child.Length != c.Child.Count)
                {
                    //ConsoleLog.Log("! Num is " + c.Child.Count);
                    return false;
                }

                for (int i = 0; i < Child.Length; i++)
                {
                    if (!Child[i].Compare(c.Child[i]))
                    {
                        return false;
                    }
                }
            }
            else
            {
                //ConsoleLog.Log("? should not be Parent");
                if (hir.isParent())
                {
                    //ConsoleLog.Log("! is Parent");
                    return false;
                }
            }
            return true;
        }
    }
}
