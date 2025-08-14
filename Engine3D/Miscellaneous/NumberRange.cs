using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine3D.Abstract3D
{
    public static class NumberRange
    {
        //  Not used : Example of Templates

        public static Number Lowest<Number>(Number[] num) where Number : IComparable<Number>
        {
            Number n = num[0];

            for (int i = 0; i < num.Length; i++)
            {
                if (num[i].CompareTo(n) < 0)
                {
                    n = num[i];
                }
            }

            return n;
        }
        public static Number Highest<Number>(Number[] num) where Number : IComparable<Number>
        {
            Number n = num[0];

            for (int i = 0; i < num.Length; i++)
            {
                if (num[i].CompareTo(n) > 0)
                    n = num[i];
            }

            return n;
        }

        public static Number LowestAbove<Number>(Number[] num, Number min) where Number : IComparable<Number>
        {
            Number n = num[0];

            for (int i = 0; i < num.Length; i++)
            {
                if (num[i].CompareTo(n) < 0 && num[i].CompareTo(min) > 0)
                {
                    n = num[i];
                }
            }

            return n;
        }
    }
}
