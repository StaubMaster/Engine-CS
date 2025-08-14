using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine3D.Graphics.Display
{
    /*  Scale
     *      Scale have their own conersions and stuff
     *      Points are just 2 Scales
     */



    public abstract class DisplayScale
    {
        public float Value;
        protected DisplayScale(float val)
        {
            Value = val;
        }

        public static implicit operator float(DisplayScale val)
        {
            return val.Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }



    public class PixelScale : DisplayScale
    {
        public PixelScale(float val) : base(val) { }
    }
    public class Normal0Scale : DisplayScale
    {
        public Normal0Scale(float val) : base(val) { }
    }
    public class Normal1Scale : DisplayScale
    {
        public Normal1Scale(float val) : base(val) { }
    }



    public class DisplayScaleConverter
    {
        public float PixelSize;

        public DisplayScaleConverter(float pixelSize)
        {
            PixelSize = pixelSize;
        }

        public float Pixel_To_Normal0(float val)
        {
            return val / PixelSize;
        }
        public float Normal0_To_Pixel(float val)
        {
            return val * PixelSize;
        }

        public float Normal0_To_Normal1(float val)
        {
            return (val * 2) - 1;
        }
        public float Normal1_To_Normal0(float val)
        {
            return (val + 1) / 2;
        }

        public float Normal1_To_Pixel(float val)
        {
            return Normal1_To_Normal0(Normal0_To_Pixel(val));
        }
        public float Pixel_To_Normal1(float val)
        {
            return Pixel_To_Normal0(Normal0_To_Normal1(val));
        }

        public PixelScale ToPixel(DisplayScale scale)
        {
            Type type = scale.GetType();
            if (type == typeof(PixelScale)) { return new PixelScale(scale); }
            if (type == typeof(Normal0Scale)) { return new PixelScale(Normal0_To_Pixel(scale)); }
            if (type == typeof(Normal1Scale)) { return new PixelScale(Normal1_To_Pixel(scale)); }
            throw new EConversionFailed();
        }
        public Normal0Scale ToNormal0(DisplayScale scale)
        {
            Type type = scale.GetType();
            if (type == typeof(PixelScale)) { return new Normal0Scale(Pixel_To_Normal0(scale)); }
            if (type == typeof(Normal0Scale)) { return new Normal0Scale(scale); }
            if (type == typeof(Normal1Scale)) { return new Normal0Scale(Normal1_To_Normal0(scale)); }
            throw new EConversionFailed();
        }
        public Normal1Scale ToNormal1(DisplayScale scale)
        {
            Type type = scale.GetType();
            if (type == typeof(PixelScale)) { return new Normal1Scale(Pixel_To_Normal1(scale)); }
            if (type == typeof(Normal0Scale)) { return new Normal1Scale(Normal0_To_Normal1(scale)); }
            if (type == typeof(Normal1Scale)) { return new Normal1Scale(scale); }
            throw new EConversionFailed();
        }

        private class EConversionFailed : Exception
        {
            public EConversionFailed() : base("DisplayScale Conversion failed.") { }
        }
    }
}
