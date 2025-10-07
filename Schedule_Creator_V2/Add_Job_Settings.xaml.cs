using Schedule_Creator_V2.Models;
using System;
using System.Collections.Generic;
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

        private void MakeNewRow()
        {
            ComboBox dayOfWeekBox = new ComboBox();
            dayOfWeekBox.Items.Add(Enum.GetValues<DayOfWeek>);



            DaysOffDataGrid.Items.Add(new JobSettingsRow(
                dayOfWeekBox, 
                new TimePickerControl(), 
                new TimePickerControl()));
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }

}
