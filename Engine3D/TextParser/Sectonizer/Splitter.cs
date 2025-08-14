
namespace Engine3D.TextParser.Sectonizer
{
    public abstract class Splitter
    {
        public Splitter[] Splitters;
        protected Splitter() { Splitters = null; }
        protected Splitter(Splitter[] splitters) { Splitters = splitters; }

        public abstract string ToLines(string Tab = "#");

        public class Main : Splitter
        {
            public Main(Splitter[] splitters) : base(splitters) { }

            public override string ToLines(string Tab = "#")
            {
                string str = "";
                if (Splitters != null)
                {
                    for (int i = 0; i < Splitters.Length; i++)
                    {
                        str += Splitters[i].ToLines(Tab + "#");
                    }
                }
                return str;
            }
        }
        public class Solo : Splitter
        {
            private char[] C;
            public Solo(char[] c) : base() { C = c; }
            public Solo(char[] c, Splitter[] splitters) : base(splitters) { C = c; }
            public bool Check(char c)
            {
                for (int i = 0; i < C.Length; i++)
                {
                    if (C[i] == c) { return true; }
                }
                return false;
            }

            public override string ToLines(string Tab = "#")
            {
                string str = "";
                str += Tab;
                for (int i = 0; i < C.Length; i++) { str += C[i]; }
                str += "\n";
                if (Splitters != null)
                {
                    for (int i = 0; i < Splitters.Length; i++)
                    {
                        str += Splitters[i].ToLines(Tab + "#");
                    }
                }
                return str;
            }
        }
        public class Twin : Splitter
        {
            private char C;
            public Twin(char c) : base() { C = c; }
            public Twin(char c, Splitter[] splitters) : base(splitters) { C = c; }
            public bool Check(char c) { return (C == c); }

            public override string ToLines(string Tab = "#")
            {
                string str = "";
                str += Tab + C + "\n";
                if (Splitters != null)
                {
                    for (int i = 0; i < Splitters.Length; i++)
                    {
                        str += Splitters[i].ToLines(Tab + "#");
                    }
                }
                return str;
            }
        }
        public class Pair : Splitter
        {
            private char C0;
            private char C1;
            public Pair(char c0, char c1) : base() { C0 = c0; C1 = c1; }
            public Pair(char c0, char c1, Splitter[] splitters) : base(splitters) { C0 = c0; C1 = c1; }
            public bool Check0(char c) { return (C0 == c); }
            public bool Check1(char c) { return (C1 == c); }

            public override string ToLines(string Tab = "#")
            {
                string str = "";
                str += Tab + C0 + C1 + "\n";
                if (Splitters != null)
                {
                    for (int i = 0; i < Splitters.Length; i++)
                    {
                        str += Splitters[i].ToLines(Tab + "#");
                    }
                }
                return str;
            }
        }



        public static Splitter Default()
        {
            return new Main(new Splitter[]
            {
                new Solo(new char[] { ';' }, new Splitter[]
                {
                    new Solo(new char[] { ' ' }),
                    new Twin('\"'),
                    new Pair('(', ')', new Splitter[]
                    {
                        new Solo(new char[] { ':' }, new Splitter[]
                        {
                            new Solo(new char[] { ' ' }),
                        }),
                    }),
                    new Pair('[', ']', new Splitter[]
                    {
                        new Solo(new char[] { ',' } ),
                    }),
                }),
            });
        }
    }
}
