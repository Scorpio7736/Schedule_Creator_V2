using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Services;
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
            AvailabilityComboBox.ItemsSource = Enum.GetValues<AvailDays>();
        }

        private List<string> GetEmailList(List<Staff> staffList)
        {
            List<string> returnList = new List<string>();
            foreach (Staff staff in staffList)
            {
                returnList.Add(staff.email);
            }

            return returnList;
        }

        private void LoadData(object sender, RoutedEventArgs e)
        {
            List<Staff> staffList;
            AvailDays boxItem = (AvailDays)AvailabilityComboBox.SelectedItem;

            if (boxItem == AvailDays.No_Availability)
            {
                staffList = DatabaseRead.ReadStaffWithNoAvail();
            }
            else
            {
                staffList = DatabaseRead.ReadStaffAvailOnDay(boxItem);
            }

            UsersDataGrid.ItemsSource = staffList;
            EmailsTextBox.Text = String.Join(", ", GetEmailList(staffList));
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(EmailsTextBox.Text);
            Messages.Display(new Message("Emails Copied", "Copied"));
        }
    }
}
