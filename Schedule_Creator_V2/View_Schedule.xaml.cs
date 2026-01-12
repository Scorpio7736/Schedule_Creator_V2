using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Services;
using System.Windows;
using System.Windows.Controls;
using Schedule_Creator_V2.Models;

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
            ScheduleComboBox.ItemsSource = DatabaseRead.ReadAllScheduleNames();
            SetVisibility();
        }

        private void SetVisibility()
        {
            List<DayOfWeek> jobDays = DatabaseRead.ReadJobSettingsDays();
            
            foreach (DayOfWeek day in jobDays)
            {
                switch (day)
                {
                    case DayOfWeek.Monday:
                        MonCol.Visibility = Visibility.Visible;
                        break;
                    case DayOfWeek.Tuesday:
                        TueCol.Visibility = Visibility.Visible;
                        break;
                    case DayOfWeek.Wednesday:
                        WedCol.Visibility = Visibility.Visible;
                        break;
                    case DayOfWeek.Thursday:
                        ThuCol.Visibility = Visibility.Visible;
                        break;
                    case DayOfWeek.Friday:
                        FriCol.Visibility = Visibility.Visible;
                        break;
                    case DayOfWeek.Saturday:
                        SatCol.Visibility = Visibility.Visible;
                        break;
                    case DayOfWeek.Sunday:
                        SunCol.Visibility = Visibility.Visible;
                        break;
                }
            }

        }

        private void FillBoxes(object sender, RoutedEventArgs e)
        {
            // Cannot be null as this function is called when there is an option selected.
            List<ScheduleRow> staffRows = DatabaseRead.ReadScheduleByScheduleName(ScheduleComboBox.SelectedItem.ToString());
            
        }





        /*
         * LOGIC PATH AND IDEA OF HOW THIS PAGE SHOULD WORK:
         * 
         * 1. select * from database where schedule_name = selected_schedule_name on day
         * 2. create lists for each day of the week in order of row
         * 3. for each item in each list for each row, make new record row
         * 4. list of records display in grid 
         */



    }
}
