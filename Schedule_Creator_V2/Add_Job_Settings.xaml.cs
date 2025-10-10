using Microsoft.Graph.Models;
using Schedule_Creator_V2.ExtensionMethods;
using Schedule_Creator_V2.Models;
using System;
using System.Collections.Generic;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using TimePickerControl = Xceed.Wpf.Toolkit.TimePicker;

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
            ComboBox dayOfWeekBox = new ComboBox();
            dayOfWeekBox.Items.Add(Enum.GetValues<DayOfWeek>);



            JobSettingsGrid.Items.Add(new JobSettingsRow(
                dayOfWeekBox, 
                new TimePickerControl(), 
                new TimePickerControl()
                ));
        }

        public void MakeNewRow(object sender, RoutedEventArgs e)
        {
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

            JobSettingsGrid.Items.RemoveAt(index);
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }

}
