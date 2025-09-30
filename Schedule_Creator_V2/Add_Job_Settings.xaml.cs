using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Services;
using System.Windows;
using System.Windows.Controls;

namespace Schedule_Creator_V2
{
    /// <summary>
    /// Interaction logic for Add_Collection.xaml
    /// </summary>
    public partial class Add_Job_Settings : Page
    {
        private PageInputsAndLabels GetInputsAndLabels()
        {
            List<PageInput> inputs = new List<PageInput>()
                {


                };

            List<Label> labels = new List<Label>()
                {
 
                };

            return new PageInputsAndLabels(inputs, labels);
        }
        public Add_Job_Settings()
        {
            InitializeComponent();
            //DayOfWeekComboBox.ItemsSource = Enum.GetValues(typeof(DayOfWeek));
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            PageInputsAndLabels inputsAndLabels = GetInputsAndLabels();
            inputsAndLabels.ResetLabels();

            if (inputsAndLabels.CheckIfNulls())
            {
                
                

            }
            else
            {
                new ErrorMaker(GetInputsAndLabels()).MakeError();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DayOfWeekComboBox.SelectedIndex = -1;
            OpeningTimePicker.Value = null;
            ClosingTimePicker.Value = null;
        }
    }
}
