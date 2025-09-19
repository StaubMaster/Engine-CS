using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine3D.Miscellaneous
{
    public class ArrayHelp
    {
        public static T[] CombineArrays<T>(params T[][] arrs)
        {
            T[] arr;
            {
                int len = 0;
                for (int j = 0; j < arrs.Length; j++)
                {
                    if (arrs[j] == null) { continue; }
                    len += arrs[j].Length;
                }
                arr = new T[len];
            }

            int off = 0;
            for (int j = 0; j < arrs.Length; j++)
            {
                if (arrs[j] == null) { continue; }
                for (int i = 0; i < arrs[j].Length; i++)
                {
                    arr[off + i] = arrs[j][i];
                }
                off += arrs[j].Length;
            }

            return arr;
        }
    }
}
