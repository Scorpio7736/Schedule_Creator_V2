using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Schedule_Creator_V2
{
    /// <summary>
    /// Interaction logic for Add_PFP.xaml
    /// </summary>
    public partial class Add_PFP : Page
    {
        public Add_PFP()
        {
            InitializeComponent();
            StaffSelector.ItemsSource = DatabaseRead.ReadStaff();
            MessageBox.Show("This page does not work yet!");
        }
        public void SelectStaff(object sender, RoutedEventArgs e)
        {

        }

        public void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
        public void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
        public void DeleteImage_Click(object sender, RoutedEventArgs e)
        {
            EmployeeImage.Source = null;
        }
        public void UploadImage_Click(object sender, RoutedEventArgs e)
        {
            
        }

    }
}
