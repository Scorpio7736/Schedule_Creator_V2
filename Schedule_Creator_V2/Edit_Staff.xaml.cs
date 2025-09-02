using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Services;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Schedule_Creator_V2
{
    /// <summary>
    /// Interaction logic for Edit_Staff.xaml
    /// </summary>
    public partial class Edit_Staff : Page
    {
        
        public Edit_Staff()
        {
            InitializeComponent();
            FillStaffSelector();
            JobPositionComboBox.ItemsSource = Enum.GetValues<Positions>();
        }
        private PageInputsAndLabels GetInputsAndLabels()
        {
            List<PageInput> inputs = new List<PageInput>()
                {
                    new PageInput(StaffSelector, "Staff Member Selection"),
                    new PageInput(FirstNameBox, "First Name"),
                    new PageInput(MiddleNameBox, "Middle Name", true), // true for exclusion
                    new PageInput(LastNameBox, "Last Name"),
                    new PageInput(JobPositionComboBox, "Job Position"),
                    new PageInput(StudentEmailBox, "Student Email")
                };

            List<Label> labels = new List<Label>()
                {
                    Staff_Member_Label,
                    FirstNameLabel,
                    MiddleNameLabel,
                    LastNameLabel,
                    Job_Position_Label,
                    Staff_Email_Label
                };

            return new PageInputsAndLabels(inputs, labels);
        }

        private void FillStaffSelector()
        {
            StaffSelector.ItemsSource = DatabaseRead.ReadStaff();
        }

        private void StaffSelected(object sender, EventArgs e)
        {
            if (StaffSelector.SelectedItem is Staff selectedStaff)
            {
                FirstNameBox.Text = selectedStaff.fName;
                MiddleNameBox.Text = selectedStaff.mName;
                LastNameBox.Text = selectedStaff.lName;
                JobPositionComboBox.SelectedItem = selectedStaff.position;
                StudentEmailBox.Text = selectedStaff.email;
            }
        }


        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            PageInputsAndLabels pageInputsAndLabels = GetInputsAndLabels();

            if (pageInputsAndLabels.CheckIfNulls())
            {
                Staff oldStaff = (Staff)StaffSelector.SelectedItem;

                DatabaseUpdate.UpdateStaff(
                    oldStaff.id,
                    FirstNameBox.Text,
                    MiddleNameBox.Text,
                    LastNameBox.Text,
                    (Positions)JobPositionComboBox.SelectedItem,
                    StudentEmailBox.Text
                );

                FillStaffSelector();

                foreach (Staff staff in (List<Staff>)StaffSelector.ItemsSource)
                {
                    if (staff.id == oldStaff.id)
                    {
                        StaffSelector.SelectedItem = staff;
                        break;
                    }
                }

                Messages.Display(new Message("Staff edit complete!", "Update complete"));
            }
            else
            {
                new ErrorMaker(pageInputsAndLabels).MakeError();
            }
        }



        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void DeleteImage_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void UploadImage_Click(Object sender, RoutedEventArgs e)
        {

        }


    }
}
