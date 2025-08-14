
namespace Engine3D.OutPut.Uniform.Specific
{
    public class CUniformScreenRatio : Generic.Float.CUniformFloat2
    {
        public CUniformScreenRatio(float w, float h) : base(2)
        {
            Calc(w, h);
        }
        public CUniformScreenRatio((float, float) size) : base(2)
        {
            Calc(size.Item1, size.Item2);
        }

        /*
                    2000
                # - - - - - - - #
                |               | 1000
                |               |
                |               |
                |               |
                # - | - - - - - #
                |   | 250 : -0.5

                1000
                # - - - #
                |       | 500
                |       |
                # - | - #
                |   | 250 : 0.0
         */

        /*
            |                                                                               |
            |       |       |       |       |       |       |       |       |       |       |
            Screen:     1000px = [ -1.0 ; +1.0 ] = 2.0
            Size:        100px = [ -1.0 ; -0.8 ] = 0.2
            Dist:         50px = [ -1.0 ; -0.9 ] = 0.1

            |                                                                                               |
            |       |       |       |       |       |       |       |       |       |       |       |       |
            Screen:     1200px = [ -1.0 ; +1.000 ] = 2.0
            Size:        100px = [ -1.0 ; -0.833 ] = 0.167
            Dist:         50px = [ -1.0 ; -0.912 ] = 0.088

            (x / 1200px) * Size
         */

        public void Calc((float, float) size)
        {
            Calc(size.Item1, size.Item2);
        }
        public void Calc(float w, float h)
        {
            Data[0] = w;
            Data[1] = h;

            if (w > h)
            {
                Data[2] = h / w;
                Data[3] = 1.0f;
            }
            else
            {
                Data[2] = 1.0f;
                Data[3] = w / h;
            }
        }
    }
}
