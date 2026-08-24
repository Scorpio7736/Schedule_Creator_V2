using Schedule_Creator_V2.ExtensionMethods;
using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Services;
using Schedule_Creator_V2.Services.Database;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Schedule_Creator_V2
{
    /// <summary>
    /// Interaction logic for Remove_Days_Off.xaml
    /// </summary>
    public partial class Remove_Days_Off : Page
    {
        public Remove_Days_Off()
        {
            InitializeComponent();
            StaffComboBox.ItemsSource = DatabaseRead.ReadStaff();
        }

        private void ErrorLabel(Label label)
        {
            label.Foreground = new SolidColorBrush(Colors.Red);
            label.FontWeight = FontWeights.Bold;
        }

        private void ResetLabel(Label label)
        {
            label.Foreground = new SolidColorBrush(Colors.Black);
            label.FontWeight = FontWeights.Normal;
        }

        private void SaveBtnClick(object sender, RoutedEventArgs e)
        {
            ResetLabel(StaffMemberLabel);
            ResetLabel(StartDateLabel);
            ResetLabel(EndDateLabel);

            bool hasStaff = StaffComboBox.HasValue();
            bool hasStartDate = StartDatePicker.HasValue();
            bool hasEndDate = EndDatePicker.HasValue();

            DateOnly? startDate = hasStartDate
                ? DateOnly.FromDateTime(StartDatePicker.SelectedDate.Value)
                : null;

            DateOnly? endDate = hasEndDate
                ? DateOnly.FromDateTime(EndDatePicker.SelectedDate.Value)
                : null;

            bool validDateRange =
                startDate.HasValue
                &&
                endDate.HasValue
                &&
                startDate.Value.StartIsBeforeEnd(endDate.Value);

            if (
                hasStaff
                &&
                hasStartDate
                &&
                hasEndDate
                &&
                validDateRange
               )
            {
                int id = ((Staff)StaffComboBox.SelectedItem).id;

                List<DateOnly> dateList =
                    startDate.Value.GetRangeOfDates(endDate.Value);

                DatabaseDelete.DeleteDaysOff(id, dateList);

                Messages.Display(
                    new Message("Removed day(s) off", "Deletion made!")
                );
            }
            else
            {
                List<string> errors = new List<string>();

                if (!hasStaff)
                {
                    ErrorLabel(StaffMemberLabel);
                    errors.Add("Staff member");
                }

                if (!hasStartDate)
                {
                    ErrorLabel(StartDateLabel);
                    errors.Add("Start date");
                }

                if (!hasEndDate)
                {
                    ErrorLabel(EndDateLabel);
                    errors.Add("End date");
                }

                if (
                    hasStartDate
                    &&
                    hasEndDate
                    &&
                    !validDateRange
                   )
                {
                    ErrorLabel(StartDateLabel);
                    ErrorLabel(EndDateLabel);
                    errors.Add("Start date is after end date");
                }

                string errorString =
                    $"Invalid inputs: {string.Join(", ", errors)}";

                Messages.Display(
                    new Error(1000, errorString)
                );
            }
        }

        private void CancelBtnClick(object sender, RoutedEventArgs e)
        {
            StaffComboBox.SelectedItem = null;
            StartDatePicker.SelectedDate = null;
            EndDatePicker.SelectedDate = null;
        }
    }
}
