using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine3D.Graphics.Telematry
{
    public class TelematryBuffer
    {
        private TelematryDataBuffer Data;

        public TelematryBuffer()
        {
            Data = new TelematryDataBuffer();
        }
        public TelematryDataBuffer Report()
        {
            TelematryDataBuffer data = Data;
            Data = new TelematryDataBuffer();
            return data;
        }

        public void Bind(int count, int size)
        {
            Data.Bind_Count++;
            Data.Bind_Vertex_Count += count;
            Data.Bind_Memory += count * size;
        }
        public void Draw(int count)
        {
            Data.Draw_Count++;
            Data.Draw_Vertex_Count += count;
        }

        public static void Bind(TelematryBuffer telematry, int count, int size)
        {
            if (telematry == null) { return; }
            telematry.Bind(count, size);
        }
        public static void Draw(TelematryBuffer telematry, int count)
        {
            if (telematry == null) { return; }
            telematry.Draw(count);
        }
    }
    public struct TelematryDataBuffer
    {
        public int Bind_Count;
        public int Bind_Vertex_Count;
        public int Bind_Memory;

        public int Draw_Count;
        public int Draw_Vertex_Count;

        public string ToLine()
        {
            string str = "";
            str += Bind_Count + " ";
            str += Bind_Vertex_Count + " ";
            str += Miscellaneous.UnitToString.Memory1000(Bind_Memory);
            str += " : ";
            str += Draw_Count + " ";
            str += Draw_Vertex_Count;
            return str;
        }
        public string ToMultiLine()
        {
            string str = "";
            str += "Bind Count: " + Bind_Count + "\n";
            str += "Bind Vertex Count: " + Bind_Vertex_Count + "\n";
            str += "Bind Memory: " + Miscellaneous.UnitToString.Memory1000Raw(Bind_Memory) + "\n";
            str += "Draw Count: " + Draw_Count + "\n";
            str += "Draw Vertex Count: " + Draw_Vertex_Count;
            return str;
        }
    }
}
