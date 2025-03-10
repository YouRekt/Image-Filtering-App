using System.Text;
using System.Windows;

namespace Image_Filtering_App.Filters.Convolution_Filters
{
    public class ConvolutionFilter
    {
        public double[,] Kernel { get; set; } = new double[,] { { 0, 0, 0 }, { 0, 1, 0 }, { 0, 0, 0 } };
        public int KernelWidth
        {
            get => Kernel.GetLength(1);
            set
            {
                Kernel = new double[KernelHeight, value];
            }
        }
        public int KernelHeight
        {
            get => Kernel.GetLength(0);
            set
            {
                Kernel = new double[value, KernelWidth];
            }
        }
        public double Divisor { get; set; } = 1;
        public double Offset { get; set; } = 0;
        public Point Anchor { get; set; } = new Point(1, 1);
        public double AnchorX
        {
            get => Anchor.X;
            set
            {
                Anchor = new Point(value, Anchor.Y);
            }
        }
        public double AnchorY
        {
            get => Anchor.Y;
            set
            {
                Anchor = new Point(Anchor.X, value);
            }
        }
        public override string ToString()
        {
            return $"Kernel: {KernelWidth}x{KernelHeight}, Divisor: {Divisor}, Offset: {Offset}, Anchor: {Anchor}\n{printKernel()}";
        }
        private string printKernel()
        {
            StringBuilder sb = new();
            for (int i = 0; i < KernelHeight; i++)
            {
                for (int j = 0; j < KernelWidth; j++)
                {
                    sb.Append(Kernel[i, j]);
                    if (j < KernelWidth - 1)
                    {
                        sb.Append(", ");
                    }
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }
        public ConvolutionFilter Clone()
        {
            return new ConvolutionFilter
            {
                Divisor = this.Divisor,
                Offset = this.Offset,
                Anchor = new Point(this.Anchor.X, this.Anchor.Y),
                Kernel = (double[,])this.Kernel.Clone()
            };
        }
    }
}
