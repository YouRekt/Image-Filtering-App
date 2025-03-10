using System.Text.Json.Serialization;
using System.Windows;

namespace Image_Filtering_App.Convolution_Filter_Editor
{
    public class FilterData
    {
        public string Name { get; set; }
        [JsonIgnore]
        public double[,] Kernel { get; set; }
        [JsonPropertyName("Kernel")]
        public double[][] KernelSerializable
        {
            get => ConvertToJagged(Kernel);
            set => Kernel = ConvertTo2D(value);
        }
        public double Divisor { get; set; }
        public double Offset { get; set; }
        public Point Anchor { get; set; }
        private static double[][] ConvertToJagged(double[,] array)
        {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);
            double[][] jaggedArray = new double[rows][];
            for (int i = 0; i < rows; i++)
            {
                jaggedArray[i] = new double[cols];
                for (int j = 0; j < cols; j++)
                {
                    jaggedArray[i][j] = array[i, j];
                }
            }
            return jaggedArray;
        }
        private static double[,] ConvertTo2D(double[][] jaggedArray)
        {
            int rows = jaggedArray.Length;
            int cols = jaggedArray[0].Length;
            double[,] array = new double[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    array[i, j] = jaggedArray[i][j];
                }
            }
            return array;
        }
    }
}
