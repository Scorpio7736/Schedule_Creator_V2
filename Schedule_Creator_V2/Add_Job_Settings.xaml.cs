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

        /// <summary>
        /// Adds a new row to the JobSettingsGrid for each JobSettings object in the specified list.
        /// </summary>
        /// <remarks>
        /// Each JobSettings object in the list is used to create a new JobSettingsRow, which is
        /// then added to the JobSettingsGrid. The order of the rows in the grid corresponds to the order of the objects
        /// in the provided list.
        /// </remarks>
        /// <param name="settings">A list of JobSettings objects to be added as new rows to the grid. Cannot be null.</param>
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
            DatabaseDelete.DeleteAllJobSettings();
            CancelButton_Click();

            Messages.Display(new Message("All Job Settings have been deleted.", "All Settings Deleted!"));
        }
        /// <summary>
        /// Handles the click event for the Save button by validating and saving job settings to the database.
        /// </summary>
        /// <remarks>This method deletes all existing job settings before saving new ones. If any job
        /// setting contains a null value, the operation is halted and an error message is displayed. Upon successful
        /// completion, a confirmation message is shown to the user.</remarks>
        /// <param name="sender">The source of the event, typically the Save button that was clicked.</param>
        /// <param name="e">The event data associated with the click event.</param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            DatabaseDelete.DeleteAllJobSettings();

            foreach (JobSettingsRow item in JobSettingsGrid.Items)
            {

                if (item.IsThereNull() == false)
                {
                    DatabaseCreate.CreateJobSettings(new JobSettings(
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
                    return;
                }
            }

            Messages.Display(
                        new Message(
                            "Job Settings have been saved successfully.",
                            "Settings Saved!"
                        ));

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
