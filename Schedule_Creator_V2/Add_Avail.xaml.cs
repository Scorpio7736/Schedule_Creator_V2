using Schedule_Creator_V2.ExtensionMethods;
using Schedule_Creator_V2.Models;
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

        private List<AvailBuilder> GetAvailBuilder()
        {
            List<AvailBuilder> returnList = new List<AvailBuilder>
            {
                new AvailBuilder(EveryDayCheckBox, EveryDayLabel, EveryDayTextBox),
                new AvailBuilder(MondayCheckBox, MondayLabel, MondayTextBox),
                new AvailBuilder(TuesdayCheckBox, TuesdayLabel, TuesdayTextBox),
                new AvailBuilder(WednesdayCheckBox, WednesdayLabel, WednesdayTextBox),
                new AvailBuilder(ThursdayCheckBox, ThursdayLabel, ThursdayTextBox),
                new AvailBuilder(FridayCheckBox, FridayLabel, FridayTextBox),
                new AvailBuilder(SaturdayCheckBox, SaturdayLabel, SaturdayTextBox),
                new AvailBuilder(SundayCheckBox, SundayLabel, SundayTextBox)
            };

            return returnList;
        }

        private void ResetInputs()
        {
            foreach (AvailBuilder ab in GetAvailBuilder())
            {
                ab.Reset();
            }
        }

        private void Save_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (StaffComboBox.SelectedItem != null)
            {
                int staffID = ((Staff)StaffComboBox.SelectedItem).id;

                DatabaseCreate.RemoveAllAvailability(staffID);

                foreach (AvailBuilder availBuilder in GetAvailBuilder())
                {
                    if (availBuilder.checkBox.IsChecked == true)
                    {
                        DatabaseCreate.AddAvailability(
                            staffID,
                            Enum.Parse<AvailDays>(availBuilder.checkBox.GetTag()),
                            availBuilder.availTimes.Text.ToString()
                            );
                    }
                }

                Messages.Display(new Message("Staff availability added!", "Availability Added"));

                ResetInputs();

            }
            else
            {
                Messages.Display(new Error(1000, "Please select Staff Member."));
            }

        }

        private void Cancel_Btn_Click(object sender, RoutedEventArgs e)
        {
            ResetInputs();
        }

    }
}
