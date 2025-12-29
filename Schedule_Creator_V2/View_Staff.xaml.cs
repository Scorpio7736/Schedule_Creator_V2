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
                IsCertText.Text = $"Certified from {staff.certRange}";
            }
            else
            {
                IsCertText.Text = "Not Certified";
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
