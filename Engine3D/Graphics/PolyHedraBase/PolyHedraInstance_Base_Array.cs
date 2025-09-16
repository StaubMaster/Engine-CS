using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine3D.Graphics.PolyHedraBase
{
    public abstract class PolyHedraInstance_Base_Array<BufferDataType, BufferType, DataType>
        where BufferDataType : PolyHedraInstance_Base_BufferData<BufferType, DataType>
        where BufferType : PolyHedraInstance_Base_Buffer<DataType>, new()
    {
        protected BufferDataType[] Array;

        protected PolyHedraInstance_Base_Array()
        {
            Array = null;
        }
        protected PolyHedraInstance_Base_Array(BufferDataType[] array)
        {
            Array = array;
        }

        public int Length { get { return Array.Length; } }

        public BufferDataType this[uint idx]
        {
            get
            {
                
                return Array[idx];
            }
        }
        public BufferDataType this[int idx]
        {
            get { return Array[idx]; }
        }

        public void Update()
        {
            for (int i = 0; i < Array.Length; i++)
            {
                Array[i].DataUpdate();
            }
        }
        public void Draw()
        {
            for (int i = 0; i < Array.Length; i++)
            {
                Array[i].DrawInst();
            }
        }
    }
}
