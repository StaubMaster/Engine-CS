using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine3D.InnPut.ValueControl
{
    public abstract class ABool
    {
        protected bool Active;

        public bool Check()
        {
            return Active;
        }

        public virtual void Tick()
        {

        }
    }
}
