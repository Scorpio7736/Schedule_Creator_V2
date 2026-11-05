using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Services.Database;
using System.Collections;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Schedule_Creator_V2
{
    /// <summary>
    /// Interaction logic for View_Email_List.xaml
    /// </summary>
    public partial class View_Email_List : Page
    {
        private readonly List<int> matchingStaffIds = new();
        private bool hasSelectedAvailabilityDay;

        public View_Email_List()
        {
            InitializeComponent();
            ArrayList AvailSelectorValues = new ArrayList()
            {
                "Monday",
                "Tuesday",
                "Wednesday",
                "Thursday",
                "Friday",
                "Saturday",
                "Sunday",
                "No Availability"
            };

            AvailabilityComboBox.ItemsSource = AvailSelectorValues;
        }


        private void LoadData(object sender, RoutedEventArgs e)
        {
            List<Staff> availOnDay;
            DayOfWeek selectedDay = default;
            hasSelectedAvailabilityDay = Enum.TryParse(
                AvailabilityComboBox.SelectedItem?.ToString(), out selectedDay);

            if (hasSelectedAvailabilityDay)
            {
                availOnDay = DatabaseRead.ReadStaffAvailOnDay(selectedDay);
            }
            else
            {
                availOnDay = DatabaseRead.ReadStaffWithNoAvail();
            }

            List<ViewEmailRow> rows = new List<ViewEmailRow>();
            List<string> availEmails = new List<string>();
            StringBuilder stringBuilder = new StringBuilder();
            matchingStaffIds.Clear();

            foreach (Staff staff in availOnDay)
            {
                string availableTimes = "No saved availability";
                if (hasSelectedAvailabilityDay)
                {
                    var ranges = DatabaseRead.ReadAvailByID(staff.id)
                        .Where(a => a.dayOfTheWeek == selectedDay)
                        .OrderBy(a => a.startTime)
                        .Select(a => $"{a.startTime:h:mm tt}–{a.endTime:h:mm tt}");
                    availableTimes = string.Join(", ", ranges);
                }

                rows.Add(new ViewEmailRow(
                    staff.id,
                    staff.displayName,
                    staff.email,
                    availableTimes
                ));

                matchingStaffIds.Add(staff.id);
                availEmails.Add(staff.email);
            }

            UsersDataGrid.ItemsSource = rows;

            for (int i = 0; i < availEmails.Count; i++)
            {
                stringBuilder.Append(availEmails[i]);
                if (i < availEmails.Count - 1)
                {
                    stringBuilder.Append(",");
                }
            }

            EmailsTextBox.Text = stringBuilder.ToString();
        }


        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!hasSelectedAvailabilityDay || matchingStaffIds.Count == 0)
            {
                MessageBox.Show(
                    "Select a day and load staff with availability before creating an email.",
                    "No Available Recipients", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            NavigationService?.Navigate(new Send_Email(matchingStaffIds));
        }
    }
}
