using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

using Engine3D.Abstract3D;

namespace Engine3D
{
    public static class TMovement
    {
        public static void Unrestricted(ref Transformation3D trans, Point3D move, Angle3D spin)
        {
            if (!move.IsNaN())
            {
                trans.Pos += move - trans.Rot;
            }

            if (!spin.IsNaN())
            {
                trans.Rot += spin;
            }
        }
        public static void FlatX(ref Transformation3D trans, Point3D move, Angle3D spin)
        {
            if (!move.IsNaN())
            {
                trans.Pos += move - new Angle3D(trans.Rot.A, 0, 0);
            }

            if (!spin.IsNaN())
            {
                trans.Rot.A += spin.A;
                trans.Rot.S += spin.S;
            }
        }
    }
}
