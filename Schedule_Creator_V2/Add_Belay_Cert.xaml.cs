using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Services;
using System.Windows;
using System.Windows.Controls;

namespace Schedule_Creator_V2
{
    /// <summary>
    /// Interaction logic for Add_Belay_Cert.xaml
    /// </summary>
    public partial class Add_Belay_Cert : Page
    {
        public Add_Belay_Cert()
        {
            InitializeComponent();
            StaffSelector.ItemsSource = DatabaseRead.ReadStaff();
        }

        private PageInputsAndLabels GetPageInputsAndLabels()
        {
            return new PageInputsAndLabels(
                new List<PageInput>
                {
                    new PageInput(StaffSelector, "Staff Selection"),
                    new PageInput(CertDatePicker, "Certification Date", true),
                    new PageInput(ExpireDatePicker, "Experation Date", true)
                },
                new List<Label>
                {
                    StaffSelectorLabel,
                    CertDatePickerLabel,
                    ExpireDatePickerLabel
                }
                );
        }

        public void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            PageInputsAndLabels inputsAndLabels = GetPageInputsAndLabels();

            if (inputsAndLabels.CheckIfNulls())
            {
                DateOnly startDate;
                DateOnly endDate;

                if (CertDatePicker.SelectedDate == null)
                {
                    startDate = new DateOnly(2000, 1, 1);
                }
                else
                {
                    startDate = DateOnly.FromDateTime(CertDatePicker.SelectedDate.Value);
                }

                if (ExpireDatePicker.SelectedDate == null)
                {
                    endDate = new DateOnly(2100, 1, 1);
                }
                else
                {
                    endDate = DateOnly.FromDateTime(ExpireDatePicker.SelectedDate.Value);
                }

                DatabaseUpdate.UpdateBelayCert(
                    ((Staff)StaffSelector.SelectedItem).id,
                    true,
                    startDate,
                    endDate
                    );


                Messages.Display(new Message($"Belay certification added from {startDate} - {endDate}", "Belay Cert Added"));
            }
            else
            {
                new ErrorMaker(inputsAndLabels).MakeError();
            }

            inputsAndLabels.ResetAll();
            
        }

        public void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            GetPageInputsAndLabels().ResetAll();
        }

        public void RemoveCertButton_Click(object sender, RoutedEventArgs e)
        {
            PageInputsAndLabels inputsAndLabels = GetPageInputsAndLabels(); 

            if (inputsAndLabels.CheckIfNulls())
            {
                DatabaseUpdate.UpdateBelayCert(((Staff)StaffSelector.SelectedItem).id, false);

            }
            else
            {
                new ErrorMaker(inputsAndLabels).MakeError();
            }

            Messages.Display(new Message("Belay certification removed", "Belay Cert Removed"));

            inputsAndLabels.ResetAll();
        }
    }
}
