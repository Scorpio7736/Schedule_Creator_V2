using Schedule_Creator_V2.ExtensionMethods;
using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Services;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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

            if (
                StaffComboBox.HasValue()
                &&
                StartDatePicker.HasValue()
                &&
                EndDatePicker.HasValue()
                &&
                UniFunc.StartIsBeforeEnd(
                    DateOnly.FromDateTime(StartDatePicker.SelectedDate.Value),
                    DateOnly.FromDateTime(EndDatePicker.SelectedDate.Value)
                    )
               )
            {

                int id = ((Staff)StaffComboBox.SelectedItem).id;
                DateOnly startDate = DateOnly.FromDateTime(StartDatePicker.SelectedDate.Value);
                DateOnly endtDate = DateOnly.FromDateTime(EndDatePicker.SelectedDate.Value);
                List<DateOnly> DateList = UniFunc.GetRangeOfDates(startDate, endtDate);


                DatabaseCreate.RemoveDaysOff(id, DateList);

                Messages.Display(new Message("Removed day(s) off", "Deletion made!"));

            }
            else
            {
                List<string> errors = new List<string>();
                if (StaffComboBox.HasValue() == false)
                {
                    ErrorLabel(StaffMemberLabel);
                    errors.Add("Staff member");
                }
                if (StartDatePicker.HasValue() == false)
                {
                    ErrorLabel(StartDateLabel);
                    errors.Add("Start date");
                }
                if (EndDatePicker.HasValue() == false)
                {
                    ErrorLabel(EndDateLabel);
                    errors.Add("End date");
                }
                if (StartDatePicker.HasValue()
                    &&
                    EndDatePicker.HasValue()
                    &&
                    UniFunc.StartIsBeforeEnd(
                    DateOnly.FromDateTime(StartDatePicker.SelectedDate.Value),
                    DateOnly.FromDateTime(EndDatePicker.SelectedDate.Value)
                    ) == false)
                {
                    ErrorLabel(StartDateLabel);
                    ErrorLabel(EndDateLabel);
                    errors.Add("Start date is after end date");
                }
                string errorString = $"Missing inputs: {string.Join(", ", errors)}";
                Messages.Display(new Error(1000, errorString));
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
