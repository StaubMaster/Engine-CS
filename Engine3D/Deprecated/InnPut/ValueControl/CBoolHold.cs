using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine3D.InnPut.ValueControl
{
    public class CBoolHold : ABool
    {
        public void Onn()
        {
            Active = true;
        }
        public void Off()
        {
            Active = false;
        }
    }
}
