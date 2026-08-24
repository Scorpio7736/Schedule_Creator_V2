using Schedule_Creator_V2.ExtensionMethods;
using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Schedule_Creator_V2
{
    /// <summary>
    /// Interaction logic for View_Staff.xaml
    /// </summary>
    public partial class View_Staff : Page
    {
        public View_Staff()
        {
            InitializeComponent();
            StaffSelector.ItemsSource = DatabaseRead.ReadStaff();
        }
       
        private void DisplayData(Staff staff, List<Availability> avaList)
        {
            FirstNameText.Text = staff.fName;
            MiddleNameText.Text = staff.mName;
            LastNameText.Text = staff.lName;
            JobPositionText.Text = staff.position.GetDescription();
            EmailText.Text = staff.email;
            List<ViewStaffRow> dataRows = new List<ViewStaffRow>();

            if (staff.isBelayCertified)
            {
                IsCertText.Text =
                    $"Certified from {staff.certRange}";

                // Checkmark
                CertificationIcon.Text = "\uE73E";

                CertificationBorder.Background =
                    new SolidColorBrush(
                        Color.FromRgb(234, 246, 239));

                CertificationBorder.BorderBrush =
                    new SolidColorBrush(
                        Color.FromRgb(183, 217, 199));

                CertificationIconBorder.Background =
                    new SolidColorBrush(
                        Color.FromRgb(217, 239, 226));

                CertificationIcon.Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(22, 121, 74));

                IsCertText.Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(22, 121, 74));
            }
            else
            {
                IsCertText.Text =
                    "Not Certified";

                // X icon
                CertificationIcon.Text = "\uE711";

                CertificationBorder.Background =
                    new SolidColorBrush(
                        Color.FromRgb(255, 241, 239));

                CertificationBorder.BorderBrush =
                    new SolidColorBrush(
                        Color.FromRgb(237, 192, 186));

                CertificationIconBorder.Background =
                    new SolidColorBrush(
                        Color.FromRgb(253, 224, 220));

                CertificationIcon.Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(180, 35, 24));

                IsCertText.Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(180, 35, 24));
            }


            foreach (Availability avail in avaList)
            {

                dataRows.Add(new ViewStaffRow(
                    avail.dayOfTheWeek,
                    $"Available between: {avail.startTime} - {avail.endTime}"
                    ));
            }

            AvailabilityDataGrid.ItemsSource = dataRows;

        }

        private void StaffSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StaffSelector.HasValue())
            {
                SelectStaffLabel.Reset();

                int id = ((Staff)StaffSelector.SelectedItem).id;
                var availList = DatabaseRead.ReadAvailForStaffByID(id);

                DisplayData((Staff)StaffSelector.SelectedItem, availList);
                
            }
            else
            {
                new ErrorMaker(new PageInput(StaffSelector, "Selected Staff"), SelectStaffLabel).MakeError();
            }
        }
    }
}
