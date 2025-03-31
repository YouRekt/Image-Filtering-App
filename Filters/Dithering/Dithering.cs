using System.Windows;
using System.Windows.Media.Imaging;

namespace Image_Filtering_App.Filters.Dithering
{
    public static class Dithering
    {
        public static BitmapSource AverageDithering(BitmapSource source, int k)
        {
            if (k < 2)
                throw new ArgumentException("K must be at least 2", nameof(k));

            WriteableBitmap bitmap = new WriteableBitmap(source);
            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            int stride = bitmap.BackBufferStride;
            int totalPixels = width * height;

            int binSize = 255 / (k - 1);
            byte[] colors = Enumerable.Range(0, k).Select(i => (byte)(i * binSize)).ToArray();

            int[] sumR = new int[k - 1];
            int[] sumG = new int[k - 1];
            int[] sumB = new int[k - 1];
            int[] countR = new int[k - 1];
            int[] countG = new int[k - 1];
            int[] countB = new int[k - 1];

            bitmap.Lock();
            try
            {
                unsafe
                {
                    byte* ptr = (byte*)bitmap.BackBuffer;

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int index = y * stride + x * 4;

                            byte r = ptr[index + 2];
                            byte g = ptr[index + 1];
                            byte b = ptr[index];

                            // Determine the bin for each channel.
                            int binB = Math.Min(b / binSize, k - 2);
                            int binG = Math.Min(g / binSize, k - 2);
                            int binR = Math.Min(r / binSize, k - 2);

                            sumR[binR] += r;
                            sumG[binG] += g;
                            sumB[binB] += b;
                            countR[binR]++;
                            countG[binG]++;
                            countB[binB]++;
                        }
                    }

                    byte[] avgR = new byte[k - 1];
                    byte[] avgG = new byte[k - 1];
                    byte[] avgB = new byte[k - 1];

                    for (int i = 0; i < k - 1; i++)
                    {
                        // Clamp the values to the beginning of the bin if empty
                        avgB[i] = (byte)(countB[i] > 0 ? sumB[i] / countB[i] : i * binSize);
                        avgG[i] = (byte)(countG[i] > 0 ? sumG[i] / countG[i] : i * binSize);
                        avgR[i] = (byte)(countR[i] > 0 ? sumR[i] / countR[i] : i * binSize);
                    }

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int index = y * stride + x * 4;

                            //int binIndexB = 0;
                            //int binIndexG = 0;
                            //int binIndexR = 0;

                            //while (ptr[index] > avgB[binIndexB])
                            //    if (++binIndexB > k - 2) break;
                            //while (ptr[index + 1] > avgG[binIndexG])
                            //    if (++binIndexG > k - 2) break;
                            //while (ptr[index + 2] > avgR[binIndexR])
                            //    if (++binIndexR > k - 2) break;

                            //ptr[index] = colors[binIndexB];
                            //ptr[index + 1] = colors[binIndexG];
                            //ptr[index + 2] = colors[binIndexR];
                            int binIndexB = Math.Min(ptr[index] / binSize, k - 2);
                            int binIndexG = Math.Min(ptr[index + 1] / binSize, k - 2);
                            int binIndexR = Math.Min(ptr[index + 2] / binSize, k - 2);

                            ptr[index] = (byte)(ptr[index] <= avgB[binIndexB] ? binIndexB * binSize : (binIndexB + 1) * binSize);
                            ptr[index + 1] = (byte)(ptr[index + 1] <= avgG[binIndexG] ? binIndexG * binSize : (binIndexG + 1) * binSize);
                            ptr[index + 2] = (byte)(ptr[index + 2] <= avgR[binIndexR] ? binIndexR * binSize : (binIndexR + 1) * binSize);
                        }
                    }
                }

                bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            }
            finally
            {
                bitmap.Unlock();
            }

            return bitmap;
        }

        public static BitmapSource YCbCrAverageDithering(BitmapSource source, int k)
        {
            if (k < 2)
                throw new ArgumentException("K must be at least 2", nameof(k));

            WriteableBitmap bitmap = new WriteableBitmap(source);
            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            int stride = bitmap.BackBufferStride;
            int totalPixels = width * height;

            int binSize = 255 / (k - 1);

            double[] sumY = new double[k - 1];
            int[] countY = new int[k - 1];
            double[] sumCb = new double[k - 1];
            int[] countCb = new int[k - 1];
            double[] sumCr = new double[k - 1];
            int[] countCr = new int[k - 1];

            bitmap.Lock();
            try
            {
                unsafe
                {
                    byte* ptr = (byte*)bitmap.BackBuffer;

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int index = y * stride + x * 4;

                            byte r = ptr[index + 2];
                            byte g = ptr[index + 1];
                            byte b = ptr[index];

                            (double Y, double Cb, double Cr) = YCbCrConverter.ConvertTo(r, g, b);



                            // Determine the bin for each channel.
                            int binY = Math.Min((int)(Y / binSize), k - 2);
                            int binCb = Math.Min((int)(Cb / binSize), k - 2);
                            int binCr = Math.Min((int)(Cr / binSize), k - 2);

                            sumY[binY] += Y;
                            countY[binY]++;
                            sumCb[binCb] += Cb;
                            countCb[binCb]++;
                            sumCr[binCr] += Cr;
                            countCr[binCr]++;
                        }
                    }

                    double[] avgY = new double[k - 1];
                    double[] avgCb = new double[k - 1];
                    double[] avgCr = new double[k - 1];

                    for (int i = 0; i < k - 1; i++)
                    {
                        // Clamp the values to the beginning of the bin if empty
                        avgY[i] = (countY[i] > 0 ? sumY[i] / countY[i] : i * binSize);
                        avgCb[i] = (countCb[i] > 0 ? sumCb[i] / countCb[i] : i * binSize);
                        avgCr[i] = (countCr[i] > 0 ? sumCr[i] / countCr[i] : i * binSize);
                    }

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int index = y * stride + x * 4;


                            byte r = ptr[index + 2];
                            byte g = ptr[index + 1];
                            byte b = ptr[index];

                            (double Y, double Cb, double Cr) = YCbCrConverter.ConvertTo(r, g, b);

                            int binIndexY = Math.Min((int)(Y / binSize),k-2);
                            int binIndexCb = Math.Min((int)(Cb / binSize),k-2);
                            int binIndexCr = Math.Min((int)(Cr / binSize),k-2);

                            (byte R, byte G, byte B) = YCbCrConverter.ConvertFrom(Y <= avgY[binIndexY] ? binIndexY * binSize : (binIndexY + 1) * binSize, Cb <= avgCb[binIndexCb] ? binIndexCb * binSize : (binIndexCb + 1) * binSize, Cr <= avgCr[binIndexCr] ? binIndexCr * binSize : (binIndexCr + 1) * binSize);
                            //(byte R, byte G, byte B) = YCbCrConverter.ConvertFrom(Y <= avgY[binIndexY] ? binIndexY * binSize : (binIndexY + 1) * binSize, Cb, Cr);

                            ptr[index + 2] = R;
                            ptr[index + 1] = G;
                            ptr[index] = B;
                        }
                    }
                }

                bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            }
            finally
            {
                bitmap.Unlock();
            }

            return bitmap;
        }
    }
}