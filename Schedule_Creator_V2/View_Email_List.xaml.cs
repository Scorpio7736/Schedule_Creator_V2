using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Services;
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

            try
            {
                DayOfWeek selectedDay = Enum.Parse<DayOfWeek>(AvailabilityComboBox.SelectedItem?.ToString());

                availOnDay = DatabaseRead.ReadStaffAvailOnDay(selectedDay);
            }
            catch
            {
                availOnDay = DatabaseRead.ReadStaffWithNoAvail();
            }

            List<ViewEmailRow> rows = new List<ViewEmailRow>();
            List<string> availEmails = new List<string>();
            StringBuilder stringBuilder = new StringBuilder();

            foreach (Staff staff in availOnDay)
            {
                rows.Add(new ViewEmailRow(
                    staff.fName,
                    staff.lName,
                    staff.email
                ));

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
            Clipboard.SetText(EmailsTextBox.Text);
            MessageBox.Show("Emails copied to clipboard!");
        }
    }
}
