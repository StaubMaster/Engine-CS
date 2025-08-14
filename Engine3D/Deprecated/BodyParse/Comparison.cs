
namespace Engine3D.BodyParse
{
    interface IComparison
    {
        public bool Compare(int num);
        public string ToString(int num);
    }
    struct SComparisonEql : IComparison
    {
        private int Num;
        public SComparisonEql(int num) { Num = num; }
        public bool Compare(int num) { return (num == Num); }
        public string ToString(int num) { return (num + " == " + Num); }
    }
    struct SComparisonMin : IComparison
    {
        private int Num;
        public SComparisonMin(int num) { Num = num; }
        public bool Compare(int num) { return (num >= Num); }
        public string ToString(int num) { return (num + " >= " + Num); }
    }
    struct SComparisonMax : IComparison
    {
        private int Num;
        public SComparisonMax(int num) { Num = num; }
        public bool Compare(int num) { return (num <= Num); }
        public string ToString(int num) { return (num + " <= " + Num); }
    }
    struct SComparisonMod : IComparison
    {
        private int Num;
        public SComparisonMod(int num) { Num = num; }
        public bool Compare(int num) { return ((num % Num) == 0); }
        public string ToString(int num) { return ("(" + num + " % " + Num + ") == 0"); }
    }
}
