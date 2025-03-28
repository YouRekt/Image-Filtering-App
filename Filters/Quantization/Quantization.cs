using Image_Filtering_App.Filters.Quantization.OctreeDS;
using System.Windows.Media.Imaging;

namespace Image_Filtering_App.Filters.Quantization
{
    public static class Quantization
    {
        public static BitmapSource OctreeQuantization(BitmapSource source, int maxColors)
        {
            Octree octree = new(maxColors);

            octree.Build(source);

            return octree.ReplaceColors(source);
        }
    }
}
