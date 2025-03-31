using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Filtering_App.Filters.Dithering
{
    public static class YCbCrConverter
    {
        public static (double Y,double Cb, double Cr) ConvertTo(byte r,byte g,byte b)
        {
            return ((0.229 * r + 0.587 * g + 0.114 * b),
                    (128 - 0.168736 * r - 0.331264 * g + 0.5 * b),
                    (128 + 0.5 * r - 0.418688 * g - 0.081312 * b));
        }

        public static (byte r, byte g, byte b) ConvertFrom(double Y, double Cb, double Cr)
        {
            return ((byte)(Y + 1.402 * (Cr - 128)),
                    (byte)(Y - 0.344136 * (Cb - 128) - 0.714136 * (Cr - 128)),
                    (byte)(Y + 1.772 * (Cb - 128)));
        }
    }
}
