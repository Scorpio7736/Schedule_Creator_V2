using System.Windows;
using System.Windows.Controls;

namespace Schedule_Creator_V2
{
    /// <summary>
    /// Interaction logic for Add_Collection.xaml
    /// </summary>
    public partial class Add_Collection : Page
    {
        public Add_Collection()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Add persistence logic for collection hours.
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DayOfWeekComboBox.SelectedIndex = -1;
            OpeningTimePicker.Value = null;
            ClosingTimePicker.Value = null;
        }
    }
}
