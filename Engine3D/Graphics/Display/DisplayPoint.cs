using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine3D.Graphics.Display
{
    /* just start using it and see what kind of problems arise
     */

    /* how it should look in the end
        PixelPoint
        NormalPoint

        should the PixelSize allways be remembered ?
        in DisplayPoint ?
            dont want to pass the Size everytime to convert
            so dont
     */

    /*
        make them seperate
        normal does not know pixel size
        have a converter    DisplaySizeConverter
        have 1D scales
     */

    public abstract class DisplayPoint
    {
        public float X;
        public float Y;

        public DisplayPoint(float x, float y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            string str = "";
            str += "[ ";
            str += X.ToString();
            str += " : ";
            str += Y.ToString();
            str += " ]";
            return str;
        }
    }

    public class PixelPoint : DisplayPoint
    {
        public PixelPoint(float x, float y) : base(x, y) { }
    }
    public class Normal0Point : DisplayPoint
    {
        public Normal0Point(float x, float y) : base(x, y) { }
    }
    public class Normal1Point : DisplayPoint
    {
        public Normal1Point(float x, float y) : base(x, y) { }
    }

    /* why different points if they all have generic data ?
     *  it would also be nice if you could specify a corner
     *  have Direction / Origin ?
     *  how should is work as an "End User":
     *      show_text_at(DisplayPointContext.TopLeft_To_DownRight() + DisplayPoint.Pixel(100, 100))
     */

    /*  
     *  NormalAbsolute  (0)
     *  NormalRelative  (1)
     *  Pixel
     *  
     *  default Origin should be Top Left
     *  default Direction should be Right Down
     *  
     *  show_text_at(DisplayPoint.NormalRel(-1, -1) + DisplayPoint.Pixel(+100, +100));
     *  show_text_at(DisplayPoint.NormalRel(-1, +1) + DisplayPoint.Pixel(+100, -100));
     *  
     *  Problem:
     *      cant calculate now because Pixel Size is not known
     *      but it kind of is ?
     *      Converter.ToPixel(DisplayPoint.NormalRel(-1, +1) + DisplayPoint.Pixel(+100, -100))
     */

    public class DisplayPointConverter
    {
        public DisplayScaleConverter X;
        public DisplayScaleConverter Y;

        public DisplayPointConverter(float x, float y)
        {
            X = new DisplayScaleConverter(x);
            Y = new DisplayScaleConverter(y);
        }

        public PixelPoint ToPixel(DisplayPoint point)
        {
            Type type = point.GetType();
            if (type == typeof(PixelPoint)) { return new PixelPoint(point.X, point.Y); }
            if (type == typeof(Normal0Point)) { return new PixelPoint(X.Normal0_To_Pixel(point.X), Y.Normal0_To_Pixel(point.Y)); }
            if (type == typeof(Normal1Point)) { return new PixelPoint(X.Normal1_To_Pixel(point.X), Y.Normal1_To_Pixel(point.Y)); }
            throw new EConversionFailed();
        }
        public Normal0Point ToNormal0(DisplayPoint point)
        {
            Type type = point.GetType();
            if (type == typeof(PixelPoint)) { return new Normal0Point(X.Pixel_To_Normal0(point.X), Y.Pixel_To_Normal0(point.Y)); }
            if (type == typeof(Normal0Point)) { return new Normal0Point(point.X, point.Y); }
            if (type == typeof(Normal1Point)) { return new Normal0Point(X.Normal1_To_Normal0(point.X), Y.Normal1_To_Normal0(point.Y)); }
            throw new EConversionFailed();
        }
        public Normal1Point ToNormal1(DisplayPoint point)
        {
            Type type = point.GetType();
            if (type == typeof(PixelPoint)) { return new Normal1Point(X.Pixel_To_Normal1(point.X), Y.Pixel_To_Normal1(point.Y)); }
            if (type == typeof(Normal0Point)) { return new Normal1Point(X.Normal0_To_Normal1(point.X), Y.Normal0_To_Normal1(point.Y)); }
            if (type == typeof(Normal1Point)) { return new Normal1Point(point.X, point.Y); }
            throw new EConversionFailed();
        }

        public PixelPoint ToPixel(DisplayPoint point0, DisplayPoint point1)
        {
            point0 = ToPixel(point0);
            point1 = ToPixel(point1);
            return new PixelPoint(point0.X + point1.X, point0.Y + point1.Y);
        }
        public Normal0Point ToNormal0(DisplayPoint point0, DisplayPoint point1)
        {
            point0 = ToNormal0(point0);
            point1 = ToNormal0(point1);
            return new Normal0Point(point0.X + point1.X, point0.Y + point1.Y);
        }
        public Normal1Point ToNormal1(DisplayPoint point0, DisplayPoint point1)
        {
            point0 = ToNormal1(point0);
            point1 = ToNormal1(point1);
            return new Normal1Point(point0.X + point1.X, point0.Y + point1.Y);
        }

        private class EConversionFailed : Exception
        {
            public EConversionFailed() : base("DisplayScale Conversion failed.") { }
        }
    }
}
