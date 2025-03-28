using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Image_Filtering_App.Filters.Quantization
{
    /// <summary>
    /// Interaction logic for MaxColorsSelector.xaml
    /// </summary>
    public partial class MaxColorsSelector : Window
    {
        public int maxColors { get; set; }
        public int uniqueColors { get; set; }
        public MaxColorsSelector(int colors, int imageColors)
        {
            InitializeComponent();
            DataContext = this;
            maxColors = colors;
            uniqueColors = imageColors;
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(ColorsPerChanel.Text, out int colors) && colors > 0)
            {
                maxColors = colors;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Please enter a positive integer value.",
                                "Invalid Input",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
