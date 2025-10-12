using Schedule_Creator_V2.ExtensionMethods;
using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Services;
using System.Windows;
using System.Windows.Controls;

namespace Schedule_Creator_V2
{
    /// <summary>
    /// Interaction logic for Add_Job_Settings.xaml
    /// </summary>
    public partial class Add_Job_Settings : Page
    {
        public Add_Job_Settings()
        {
            InitializeComponent();
            MakeNewRow();
        }

        public void MakeNewRow()
        {
            JobSettingsGrid.Items.Add(new JobSettingsRow());
        }

        public void MakeNewRow(object sender, RoutedEventArgs e)
        {
            MakeNewRow();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (JobSettingsRow item in JobSettingsGrid.Items)
            {

                if (item.IsThereNull() == false)
                {
                    DatabaseCreate.AddJobSettings(new JobSettings(
                        (DayOfWeek)item.dayOfTheWeek.SelectedItem,
                        TimeOnly.FromDateTime(item.startTimePicker.Value.Value),
                        TimeOnly.FromDateTime(item.endTimePicker.Value.Value)
                    ));
                }
                else
                {
                    Messages.Display(
                        new Error(
                            1000,
                            "Null Value Error"
                        ));
                    break;
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            while (JobSettingsGrid.Items.Count > 0)
            {
                JobSettingsGrid.Items.RemoveAt(0);
            }

            MakeNewRow();

        }

        public void RemoveRow(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn)
            {
                return;
            }

            int index = JobSettingsGrid.GetRowIndexFromButton(btn);

            if (index < 0 || index >= JobSettingsGrid.Items.Count)
            {
                return;
            }

            if (JobSettingsGrid.Items.Count > 1)
            {
                JobSettingsGrid.Items.RemoveAt(index);
            }
        }
    }

}
