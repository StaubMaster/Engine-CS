using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine3D.Graphics.Basic.Data
{
    public interface IData
    {
        public void ToUniform(params int[] locations);
    }
}
