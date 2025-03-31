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

namespace Image_Filtering_App.Filters.Dithering
{
    /// <summary>
    /// Interaction logic for ColorsPerChanelSelector.xaml
    /// </summary>
    public partial class ColorsPerChanelSelector : Window
    {
        public int k { get; set; }
        public ColorsPerChanelSelector(int colors)
        {
            InitializeComponent();
            DataContext = this;
            k = colors;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(ColorsPerChanel.Text, out int colors) && colors >= 2 && colors <= 255)
            {
                k = colors;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Please enter a valid integer value (2 - 255).",
                                "Invalid Input",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
            }
        }
    }
}
