using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine3D.InnPut.ValueControl
{
    public class CBoolToggleD : ABool
    {
        public void Toggle()
        {
            Active = !Active;
        }
    }
}
