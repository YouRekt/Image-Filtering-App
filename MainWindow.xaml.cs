using Image_Filtering_App.Convolution_Filter_Editor;
using Image_Filtering_App.Filters;
using Image_Filtering_App.Filters.Convolution_Filters;
using Image_Filtering_App.Filters.Dithering;
using Image_Filtering_App.Filters.Quantization;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Path = System.IO.Path;

namespace Image_Filtering_App;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private BitmapSource? filteredImage;
    private ConvolutionFilter? customConvolutionFilter;
    private int colorsPerChanel { get; set; }
    private int maxColors { get; set; }
    private int imageColors { get; set; }

    private enum FunctionalFilterType
    {
        Invert,
        Brightness_Correction,
        Contrast_Enhancement,
        Gamma_Correction
    };

    private enum ConvolutionFilterType
    {
        Blur,
        Gaussian_Blur,
        Sharpen,
        Edge_Detection,
        Emboss,
        Custom_Filter
    }

    private enum MorphologicalFilterType
    {
        Erosion,
        Dilation
    }
    public MainWindow()
    {
        InitializeComponent();
        customConvolutionFilter = new ConvolutionFilter()
        {
            Kernel = new double[,] { { 0, 0, 0 }, { 0, 1, 0 }, { 0, 0, 0 } }
        };
        PopulateFilterMenu();
        colorsPerChanel = 2;
    }

    private void PopulateFilterMenu()
    {
        foreach (FunctionalFilterType filterType in Enum.GetValues(typeof(FunctionalFilterType)))
        {
            MenuItem filter = new MenuItem
            {
                Header = string.Join(" ", filterType.ToString().Split("_")),
                Tag = filterType,
            };

            filter.Click += FilterMenuItem_Click;
            Functional_Filters.Items.Add(filter);
        }

        foreach (ConvolutionFilterType filterType in Enum.GetValues(typeof(ConvolutionFilterType)))
        {
            MenuItem filter = new MenuItem
            {
                Header = string.Join(" ", filterType.ToString().Split("_")),
                Tag = filterType,
            };

            filter.Click += FilterMenuItem_Click;
            Convolution_Filters.Items.Add(filter);
        }

        foreach (MorphologicalFilterType filterType in Enum.GetValues(typeof(MorphologicalFilterType)))
        {
            MenuItem filter = new MenuItem
            {
                Header = string.Join(" ", filterType.ToString().Split("_")),
                Tag = filterType,
            };

            filter.Click += FilterMenuItem_Click;
            Morphological_Filters.Items.Add(filter);
        }
    }

    private void FilterMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem filter)
        {
            if (filter.Tag is FunctionalFilterType functionalFilterType)
                ApplyFunctionalFilter(functionalFilterType);
            else if (filter.Tag is ConvolutionFilterType convolutionFilterType)
                ApplyConvolutionFilter(convolutionFilterType, customConvolutionFilter);
            else if (filter.Tag is MorphologicalFilterType morphologicalFilterType)
                ApplyMorphologicalFilter(morphologicalFilterType);
        }
    }

    private static int countUniqueColors(BitmapSource source)
    {
        WriteableBitmap bitmap = new WriteableBitmap(source);
        int width = bitmap.PixelWidth;
        int height = bitmap.PixelHeight;
        int stride = bitmap.BackBufferStride;
        HashSet<int> colors = [];

        bitmap.Lock();
        unsafe
        {
            byte* ptr = (byte*)bitmap.BackBuffer;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + x * 4;
                    int color = ptr[index] << 16 | ptr[index + 1] << 8 | ptr[index + 2];

                    colors.Add(color);
                }
            }
        }
        bitmap.Unlock();

        return colors.Count;
    }

    private void LoadImage_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp",
            Title = "Select an Image"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            BitmapImage bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
            imageColors = countUniqueColors(bitmap);
            filteredImage = bitmap;
            // Set the image source
            OriginalImage.Source = bitmap;
            FilteredImage.Source = filteredImage;
        }
    }

    private void ApplyFunctionalFilter(FunctionalFilterType filterType)
    {
        try
        {
            if (filteredImage != null)
            {
                switch (filterType)
                {
                    case FunctionalFilterType.Invert:
                        filteredImage = FunctionalFilters.Invert(filteredImage);
                        break;
                    case FunctionalFilterType.Brightness_Correction:
                        filteredImage = FunctionalFilters.BrightnessCorrection(filteredImage);
                        break;
                    case FunctionalFilterType.Contrast_Enhancement:
                        filteredImage = FunctionalFilters.ContrastEnchancement(filteredImage);
                        break;
                    case FunctionalFilterType.Gamma_Correction:
                        filteredImage = FunctionalFilters.GammaCorrection(filteredImage);
                        break;
                    default:
                        break;
                }
                FilteredImage.Source = filteredImage;
            }
            else
            {
                MessageBox.Show("No image loaded. Please load an image first.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error applying filter: {ex.Message}");
        }
    }
    private void ApplyConvolutionFilter(ConvolutionFilterType filterType, ConvolutionFilter? custom = null)
    {
        try
        {
            if (filteredImage != null)
            {
                switch (filterType)
                {
                    case ConvolutionFilterType.Blur:
                        filteredImage = ConvolutionFilters.Blur(filteredImage);
                        break;
                    case ConvolutionFilterType.Gaussian_Blur:
                        filteredImage = ConvolutionFilters.GaussianBlur(filteredImage);
                        break;
                    case ConvolutionFilterType.Sharpen:
                        filteredImage = ConvolutionFilters.Sharpen(filteredImage);
                        break;
                    case ConvolutionFilterType.Edge_Detection:
                        filteredImage = ConvolutionFilters.EdgeDetection(filteredImage);
                        break;
                    case ConvolutionFilterType.Emboss:
                        filteredImage = ConvolutionFilters.Emboss(filteredImage);
                        break;
                    case ConvolutionFilterType.Custom_Filter:
                        if (custom == null)
                        {
                            throw new ArgumentNullException("Please provide a custom filter");
                        }
                        filteredImage = ConvolutionFilters.Convolve(filteredImage, custom);
                        break;
                    default:
                        break;
                }
                FilteredImage.Source = filteredImage;
            }
            else
            {
                MessageBox.Show("No image loaded. Please load an image first.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error applying filter: {ex.Message}");
        }
    }

    private void ApplyMorphologicalFilter(MorphologicalFilterType filterType)
    {
        try
        {
            if (filteredImage != null)
            {
                switch (filterType)
                {
                    case MorphologicalFilterType.Dilation:
                        filteredImage = MorphologicalFilters.Dilate(filteredImage);
                        break;
                    case MorphologicalFilterType.Erosion:
                        filteredImage = MorphologicalFilters.Erode(filteredImage);
                        break;
                    default:
                        break;
                }
                FilteredImage.Source = filteredImage;
            }
            else
            {
                MessageBox.Show("No image loaded. Please load an image first.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error applying filter: {ex.Message}");
        }
    }
    private void Reset_Click(object sender, RoutedEventArgs e)
    {
        if (OriginalImage.Source is BitmapSource original)
        {
            filteredImage = original;
            FilteredImage.Source = original;
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (filteredImage == null)
        {
            MessageBox.Show("No modified image to save. Please load and apply a filter first.");
            return;
        }

        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Filter = "PNG Image|*.png|JPEG Image|*.jpg|BMP Image|*.bmp",
            Title = "Save modified image"
        };


        if (saveFileDialog.ShowDialog() == true)
        {
            string filePath = saveFileDialog.FileName;
            BitmapEncoder encoder;

            switch (Path.GetExtension(filePath).ToLower())
            {
                case ".png":
                    encoder = new PngBitmapEncoder();
                    break;
                case ".jpg":
                case ".jpeg":
                    encoder = new JpegBitmapEncoder();
                    break;
                case ".bmp":
                    encoder = new BmpBitmapEncoder();
                    break;
                default:
                    MessageBox.Show("Unsupported file format.");
                    return;
            }

            encoder.Frames.Add(BitmapFrame.Create(filteredImage));

            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    encoder.Save(stream);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving image: {ex.Message}");
            }
        }
    }

    private void EditCustomFilter_Click(object sender, RoutedEventArgs e)
    {
        var editorWindow = new ConvolutionFilterEditor(customConvolutionFilter!);
        if (editorWindow.ShowDialog() == true)
        {
            customConvolutionFilter = editorWindow.EditedFilter;
            MessageBox.Show($"Custom filter edited.\n{customConvolutionFilter}");
        }
    }

    private void ToGrayscale_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (filteredImage != null)
            {
                filteredImage = Grayscale.ToGrayscale(filteredImage);

                FilteredImage.Source = filteredImage;
            }
            else
            {
                MessageBox.Show("No image loaded. Please load an image first.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error converting to grayscale: {ex.Message}");
        }
    }

    private void ColorsPerChanel_Click(object sender, RoutedEventArgs e)
    {
        ColorsPerChanelSelector colorsPerChanelSelector = new(colorsPerChanel);
        if (colorsPerChanelSelector.ShowDialog() == true)
        {
            colorsPerChanel = colorsPerChanelSelector.k;
        }
    }

    private void AvgDithering_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (filteredImage != null)
            {
                filteredImage = Dithering.AverageDithering(filteredImage, colorsPerChanel);

                FilteredImage.Source = filteredImage;
            }
            else
            {
                MessageBox.Show("No image loaded. Please load an image first.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error during dithering: {ex.Message}");
        }
    }

    private void MaxColors_Click(object sender, RoutedEventArgs e)
    {
        if (filteredImage != null)
        {
            MaxColorsSelector maxColorsSelector = new(maxColors, imageColors);
            if (maxColorsSelector.ShowDialog() == true)
            {
                maxColors = maxColorsSelector.maxColors;
            }
        }
        else
        {
            MessageBox.Show("No image loaded. Please load an image first.");
        }
    }

    private void OctreeQuantization_Click(object sender, RoutedEventArgs e)
    {
        if (maxColors == 0)
        {
            MessageBox.Show("Please select the maximum number of colors first.");
            return;
        }

        try
        {
            if (filteredImage != null)
            {
                filteredImage = Quantization.OctreeQuantization(filteredImage, maxColors);
                FilteredImage.Source = filteredImage;
            }
            else
            {
                MessageBox.Show("No image loaded. Please load an image first.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error during quantization: {ex.Message}");
        }
    }

    private void CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = true;
    }

    private void Undo_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        Reset_Click(sender, e);
    }

    private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        LoadImage_Click(sender, e);
    }
    private void SaveImage_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        Save_Click(sender, e);
    }

    private void AvgYCbCrDithering_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (filteredImage != null)
            {
                filteredImage = Dithering.YCbCrAverageDithering(filteredImage, colorsPerChanel);

                FilteredImage.Source = filteredImage;
            }
            else
            {
                MessageBox.Show("No image loaded. Please load an image first.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error during dithering: {ex.Message}");
        }
    }
}