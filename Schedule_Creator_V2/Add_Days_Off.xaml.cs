using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Services;
using System.Windows;
using System.Windows.Controls;

namespace Schedule_Creator_V2
{
    /// <summary>
    /// Interaction logic for Add_Days_Off.xaml
    /// </summary>
    public partial class Add_Days_Off : Page
    {
        public Add_Days_Off()
        {
            InitializeComponent();
            StaffComboBox.ItemsSource = DatabaseRead.ReadStaff();
        }

        private PageInputsAndLabels GetPageInputsAndLabels()
        {
            return new PageInputsAndLabels(
                new List<PageInput>
                {
                    new PageInput(StaffComboBox, "Staff Selection"),
                    new PageInput(StartDatePicker, "Start Date"),
                    new PageInput(EndDatePicker, "End Date"),
                    new PageInput(ReasonBox, "Reason", true)
                },
                new List<Label>
                {
                    StaffMemberLabel,
                    StartDateLabel,
                    EndDateLabel,
                    ReasonLabel
                }
                );
        }

        private void SaveBtnClick(object sender, RoutedEventArgs e)
        {
            PageInputsAndLabels inputsAndLabels = GetPageInputsAndLabels();

            if (
                inputsAndLabels.CheckIfNulls()
                &&
                UniFunc.StartIsBeforeEnd(
                    DateOnly.FromDateTime(StartDatePicker.SelectedDate.Value),
                    DateOnly.FromDateTime(EndDatePicker.SelectedDate.Value)
               ))
            {
                int id = ((Staff)StaffComboBox.SelectedItem).id;
                DateOnly startDate = DateOnly.FromDateTime(StartDatePicker.SelectedDate.Value);
                DateOnly endtDate = DateOnly.FromDateTime(EndDatePicker.SelectedDate.Value);
                List<DateOnly> DateList = UniFunc.GetRangeOfDates(startDate, endtDate);
                string reason = ReasonBox.Text;


                DatabaseCreate.RemoveDaysOff(id, DateList);
                DatabaseCreate.AddDaysOff(id, DateList, reason);

                Messages.Display(new Message("Requested day(s) off", "Request made!"));

            }
            else
            {
                new ErrorMaker(inputsAndLabels).MakeError();
            }
        }
        private void CancelBtnClick(object sender, RoutedEventArgs e)
        {
            GetPageInputsAndLabels().ResetAll();
        }
    }
}
