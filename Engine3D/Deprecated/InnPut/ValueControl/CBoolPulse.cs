using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine3D.InnPut.ValueControl
{
    public class CBoolPulse : ABool
    {
        public override void Tick()
        {
            Active = false;
        }

        public void Pulse()
        {
            Active = true;
        }
    }
}
