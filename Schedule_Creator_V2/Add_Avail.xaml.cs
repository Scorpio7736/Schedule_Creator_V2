using Schedule_Creator_V2.Services;
using System.Windows;
using System.Windows.Controls;

namespace Schedule_Creator_V2
{
    /// <summary>
    /// Interaction logic for Add_Avail.xaml
    /// </summary>
    public partial class Add_Avail : Page
    {
        public Add_Avail()
        {
            InitializeComponent();
            StaffComboBox.ItemsSource = DatabaseRead.ReadStaff();
        }

        
        private void Reset_Btn_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void MakeNewRow(object sender, RoutedEventArgs e)
        { 
        
        }

        private void RemoveRow(object sender, RoutedEventArgs e)
        {

        }


        private void Save_Btn_Click(object sender, RoutedEventArgs e)
        {
            

        }

        private void Cancel_Btn_Click(object sender, RoutedEventArgs e)
        {
           
        }

    }
}
