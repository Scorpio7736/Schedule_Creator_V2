using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Services;
using System.Windows;
using System.Windows.Controls;

namespace Schedule_Creator_V2
{
    /// <summary>
    /// Interaction logic for View_Schedule.xaml
    /// </summary>
    public partial class View_Schedule : Page
    {
        public View_Schedule()
        {
            InitializeComponent();
            ScheduleComboBox.ItemsSource = DatabaseRead.GetAllScheduleNames();

        }



        private void FillBoxes(object sender, RoutedEventArgs e)
        {
         
            List<ScheduleRow> rows = new List<ScheduleRow>();



            

        }
    }
}
