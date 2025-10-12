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

            List<JobSettings> settings =  DatabaseRead.ReadJobSettings();

            if (settings != null && settings.Count > 0)
            {
                MakeNewRow(settings);
            }
            else
            {
                MakeNewRow();
            }                
        }

        public void MakeNewRow(List<JobSettings> settings)
        {
            foreach (JobSettings setting in settings)
            {
                JobSettingsGrid.Items.Add(new JobSettingsRow(setting));
            }
            
        }

        public void MakeNewRow(object? sender = null, RoutedEventArgs? e = null)
        {
            JobSettingsGrid.Items.Add(new JobSettingsRow());
        }

        private void RemoveAll_Click(object sender, RoutedEventArgs e)
        {
            DatabaseCreate.RemoveJobSettings();
            CancelButton_Click();

            Messages.Display(new Message("All Job Settings have been deleted.", "All Settings Deleted!"));
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            DatabaseCreate.RemoveJobSettings();

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

        private void CancelButton_Click(object? sender = null, RoutedEventArgs? e = null)
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
