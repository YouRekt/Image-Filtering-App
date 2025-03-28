using System.Windows;
using System.Windows.Media.Imaging;

namespace Image_Filtering_App.Filters
{
    public static class Grayscale
    {
        public static BitmapSource ToGrayscale(BitmapSource source)
        {
            WriteableBitmap bitmap = new WriteableBitmap(source);

            try
            {
                bitmap.Lock();
                unsafe
                {
                    byte* ptr = (byte*)bitmap.BackBuffer;
                    int height = bitmap.PixelHeight;
                    int width = bitmap.PixelWidth;
                    int stride = bitmap.BackBufferStride;

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int index = y * stride + x * 4;

                            byte b = ptr[index];
                            byte g = ptr[index + 1];
                            byte r = ptr[index + 2];

                            byte gray = (byte)(0.299 * r + 0.587 * g + 0.114 * b);

                            ptr[index] = gray;
                            ptr[index + 1] = gray;
                            ptr[index + 2] = gray;
                        }
                    }
                }
                bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
            }
            finally
            {
                bitmap.Unlock();
            }

            return bitmap;
        }
    }
}
