using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Enums;
using Schedule_Creator_V2.Services;
using System.Windows;
using System.Windows.Controls;

namespace Schedule_Creator_V2
{
    /// <summary>
    /// Interaction logic for Add_Staff.xaml
    /// </summary>
    public partial class Add_Staff : Page
    {
        
        public Add_Staff()
        {
            InitializeComponent();
            JobPositionComboBox.ItemsSource = Enum.GetValues<Positions>();
        }

        private PageInputsAndLabels GetInputsAndLabels()
        {
            List<PageInput> inputs = new List<PageInput>()
                {
                    new PageInput(FirstNameBox, "First Name"),
                    new PageInput(MiddleNameBox, "Middle Name", true),
                    new PageInput(LastNameBox, "Last Name"),
                    new PageInput(JobPositionComboBox, "Job Position"),
                    new PageInput(StudentEmailBox, "Student Email")
                };   

            List<Label> labels = new List<Label>()
                {
                    FirstNameLabel,
                    MiddleNameLabel,
                    LastNameLabel,
                    PositionLabel,
                    StudentEmailLabel
                };

            return new PageInputsAndLabels(inputs, labels);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ((PageInputsAndLabels)GetInputsAndLabels()).ResetAll();
        }

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            PageInputsAndLabels inputsAndLabels = GetInputsAndLabels();
            inputsAndLabels.ResetLabels();

            if (inputsAndLabels.CheckIfNulls())
            {
                string firstName = FirstNameBox.Text;
                string middleName = MiddleNameBox.Text;
                string lastName = LastNameBox.Text;
                string staffEmail = StudentEmailBox.Text;

                Positions selectedPosition = (Positions)JobPositionComboBox.SelectedItem;

                DatabaseCreate.CreateStaff(firstName, middleName, lastName, selectedPosition, (byte[]?)null, staffEmail, false);

                Messages.Display(new Message($"Staff member {firstName} {lastName} was added to the team!", "Staff Added!"));

                inputsAndLabels.ResetInputs();
            }
            else
            {
                new ErrorMaker(GetInputsAndLabels()).MakeError();
            }
            
        }
    }
}
