
namespace Engine3D.TextParser.Sectonizer
{
    enum InEx : byte
    {
        None = 0b1111,

        In = 0b00,
        Ex = 0b01,
        Regular = 0b00,
        Control = 0b10,

        InRegular = In | Regular,
        ExRegular = Ex | Regular,
        InControl = In | Control,
        ExControl = Ex | Control,

        TypeMask = 0b01,
        ControlMask = 0b10,
    }
    class Seperator
    {
        public readonly int Idx;
        public readonly InEx Type;

        public Seperator(int idx, InEx type)
        {
            Idx = idx;
            Type = type;
        }

        public int Index0(bool wantControl)
        {
            InEx type = Type & InEx.TypeMask;
            if (!wantControl && IsControl())
            {
                return Idx + 1;
            }
            else
            {
                if (type == InEx.In) { return Idx; }
                if (type == InEx.Ex) { return Idx + 1; }
            }
            return Idx;
        }
        public int Index1(bool wantControl)
        {
            InEx type = Type & InEx.TypeMask;
            if (!wantControl && IsControl())
            {
                return Idx - 1;
            }
            else
            {
                if (type == InEx.In) { return Idx; }
                if (type == InEx.Ex) { return Idx - 1; }
            }
            return Idx;
        }

        public bool IsControl()
        {
            return ((Type & InEx.ControlMask) == InEx.Control);
        }
    }
}
