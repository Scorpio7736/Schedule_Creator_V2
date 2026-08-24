using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Services;
using System.Windows;
using System.Windows.Controls;

namespace Schedule_Creator_V2
{
    /// <summary>
    /// Interaction logic for View_Days_Off.xaml
    /// </summary>
    public partial class View_Days_Off : Page
    {
        public View_Days_Off()
        {
            InitializeComponent();
            DaysOffComboBox.ItemsSource = DatabaseRead.ReadStaff();
        }

        private void FillDateGrid(object sender, RoutedEventArgs e)
        {

            List<DaysOffLine> daysOffLines = new List<DaysOffLine>();

            foreach(DaysOff daysOff in DatabaseRead.ReadDaysOff())
            {
                Staff staff = DatabaseRead.ReadStaffByID(daysOff.id);

                if (daysOff.date == DateOnly.FromDateTime(DaysOffDatePicker.SelectedDate.Value))
                {
                    daysOffLines.Add(new DaysOffLine(
                    staff.fName,
                    staff.lName,
                    daysOff.date,
                    daysOff.reason
                    ));
                }
            }

            DaysOffDataGrid.ItemsSource = daysOffLines;
        }

        private void FillComboGrid(object sender, RoutedEventArgs e)
        {


            List<DaysOffLine> daysOffLines = new List<DaysOffLine>();

            foreach (DaysOff daysOff in DatabaseRead.ReadDaysOff())
            {
                Staff staff = ((Staff)DaysOffComboBox.SelectedItem);

                if (staff.id == daysOff.id)
                {
                    daysOffLines.Add(new DaysOffLine(
                    staff.fName,
                    staff.lName,
                    daysOff.date,
                    daysOff.reason
                    ));
                }
            }

            DaysOffDataGrid.ItemsSource = daysOffLines;
        }
    }
}
