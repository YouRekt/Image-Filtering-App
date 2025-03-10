using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.Windows;

namespace Image_Filtering_App.Filters.Convolution_Filters
{
    public static class ConvolutionFilters
    {
        public static BitmapSource Convolve(BitmapSource source, ConvolutionFilter filter)
        {
            WriteableBitmap bitmap = new WriteableBitmap(source);
            bitmap.Lock();

            byte[] originalPixels = new byte[bitmap.PixelHeight * bitmap.BackBufferStride];
            Marshal.Copy(bitmap.BackBuffer, originalPixels, 0, originalPixels.Length);

            int kernelWidth = filter.Kernel.GetLength(1);
            int kernelHeight = filter.Kernel.GetLength(0);

            unsafe
            {
                byte* ptr = (byte*)bitmap.BackBuffer;
                int stride = bitmap.BackBufferStride;

                for (int y = 0; y < bitmap.PixelHeight; y++)
                {
                    for (int x = 0; x < bitmap.PixelWidth; x++)
                    {
                        double[] sum = new double[3] { 0, 0, 0 }; // B, G, R

                        for (int ky = 0; ky < kernelHeight; ky++)
                        {
                            for (int kx = 0; kx < kernelWidth; kx++)
                            {
                                int px = x + (kx - (int)filter.Anchor.X);
                                int py = y + (ky - (int)filter.Anchor.Y);

                                // Clamp edges
                                px = Math.Clamp(px, 0, bitmap.PixelWidth - 1);
                                py = Math.Clamp(py, 0, bitmap.PixelHeight - 1);

                                int index = py * stride + px * 4;
                                double weight = filter.Kernel[ky, kx];

                                sum[0] += originalPixels[index] * weight; // B
                                sum[1] += originalPixels[index + 1] * weight; // G
                                sum[2] += originalPixels[index + 2] * weight; // R
                            }
                        }
                        int outputIndex = y * stride + x * 4;
                        for (int c = 0; c < 3; c++)
                        {
                            double value = (sum[c] / filter.Divisor) + filter.Offset;
                            ptr[outputIndex + c] = (byte)Math.Clamp(value, 0, 255);
                        }
                        ptr[outputIndex + 3] = originalPixels[outputIndex + 3]; // Preserve alpha
                    }
                }
            }

            bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
            bitmap.Unlock();

            return bitmap;
        }
        public static BitmapSource Blur(BitmapSource source)
        {
            ConvolutionFilter filter = new ConvolutionFilter
            {
                Kernel = new double[,]
                {
                    {1, 1, 1},
                    {1, 1, 1},
                    {1, 1, 1}
                },
                Divisor = 9
            };
            return Convolve(source, filter);
        }

        public static BitmapSource GaussianBlur(BitmapSource source)
        {
            ConvolutionFilter filter = new ConvolutionFilter
            {
                Kernel = new double[,]
                {
                    {0, 1, 0},
                    {1, 4, 1},
                    {0, 1, 0}
                },
                Divisor = 8
            };

            return Convolve(source, filter);
        }
        public static BitmapSource Sharpen(BitmapSource source)
        {
            ConvolutionFilter filter = new ConvolutionFilter
            {
                Kernel = new double[,]
                {
                    {0, -1, 0},
                    {-1, 5, -1},
                    {0, -1, 0}
                },
            };

            return Convolve(source, filter);
        }

        public static BitmapSource EdgeDetection(BitmapSource source)
        {
            ConvolutionFilter filter = new ConvolutionFilter
            {
                Kernel = new double[,]
                {
                    {-1, -1, -1},
                    {-1, 8, -1},
                    {-1, -1, -1}
                },
            };

            return Convolve(source, filter);
        }
        public static BitmapSource Emboss(BitmapSource source)
        {
            ConvolutionFilter filter = new ConvolutionFilter
            {
                Kernel = new double[,]
                {
                    {-1, -1, 0},
                    {-1, 1, 1},
                    {0, 1, 1}
                },
            };

            return Convolve(source, filter);
        }
    }
}
