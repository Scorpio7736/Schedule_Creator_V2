using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Services;
using System.Windows;
using System.Windows.Controls;

namespace Schedule_Creator_V2
{
    /// <summary>
    /// Interaction logic for Remove_Staff.xaml
    /// </summary>
    public partial class Remove_Staff : Page
    {
        public Remove_Staff()
        {
            InitializeComponent();
            StaffSelector.ItemsSource = DatabaseRead.ReadStaff();
        }

        private PageInputsAndLabels GetInputsAndLabels()
        {
            List<PageInput> inputs = new List<PageInput>()
                {
                    new PageInput(StaffSelector, "Selected Staff"),
                    new PageInput(ConfirmDeleteCheckBox, "Confirm Deletion Box")
                };

            List<Label> labels = new List<Label>()
                {
                    StaffSelectorLabel,
                    ConformationLabel
                };

            return new PageInputsAndLabels(inputs, labels);
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            GetInputsAndLabels().ResetAll();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            PageInputsAndLabels inputsAndLabels = GetInputsAndLabels();
            inputsAndLabels.ResetLabels();

            if (inputsAndLabels.CheckIfNulls())
            {
                DatabaseDelete.DeleteAllByID(((Staff)StaffSelector.SelectedItem).id);
                Messages.Display(new Message("We are sad to see you leave :(", "Staff Deleted"));
                inputsAndLabels.ResetAll();
                StaffSelector.ItemsSource = DatabaseRead.ReadStaff();
            }
            else
            {
                new ErrorMaker(inputsAndLabels).MakeError();
            }
        }
    }
}
