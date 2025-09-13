using Engine3D.Abstract3D;
using Engine3D.Graphics.PolyHedraBase;

using Engine3D.Miscellaneous.EntryContainer;

namespace Engine3D.Graphics.PolyHedraBase
{
    public abstract class PolyHedraInstance_Base_BufferData<BufferType, DataType>
        where BufferType : PolyHedraInstance_Base_Buffer<DataType>, new()
    {
        protected readonly PolyHedra PH;
        protected readonly BufferType Buffer;
        protected readonly EntryContainerDynamic<DataType> InstanceData;

        public PolyHedraInstance_Base_BufferData(PolyHedra ph)
        {
            PH = ph;

            Buffer = new BufferType();
            PH.ToBuffer(Buffer);

            InstanceData = new EntryContainerDynamic<DataType>();
        }

        public EntryContainerDynamic<DataType>.Entry Alloc(int size)
        {
            return InstanceData.Alloc(size);
        }

        public void DataUpdate()
        {
            if (InstanceData.DataChanged)
            {
                Buffer.Bind_Inst(InstanceData.Data, InstanceData.Length);
                InstanceData.DataChanged = false;
            }
        }

        public void DrawMain()
        {
            Buffer.Draw_Main();
        }
        public void DrawInst()
        {
            Buffer.Draw_Inst();
        }
    }
}
