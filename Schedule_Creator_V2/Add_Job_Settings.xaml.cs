using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Services;
using Xceed.Wpf.Toolkit;

namespace Schedule_Creator_V2
{
    /// <summary>
    /// Interaction logic for Add_Job_Settings.xaml
    /// </summary>
    public partial class Add_Job_Settings : Page
    {
        private PageInputsAndLabels GetInputsAndLabels()
        {
            List<PageInput> inputs = new List<PageInput>();
            List<Label> labels = new List<Label>();

            void AddDayInputs(string dayName, CheckBox checkBox, Label openingLabel, TimePicker openingPicker, Label closingLabel, TimePicker closingPicker)
            {
                PageInput checkBoxInput = new PageInput(checkBox, $"{dayName} Box", true);
                PageInput openingInput = new PageInput(openingPicker, $"{dayName} Opening Time", false, new[] { checkBoxInput });
                PageInput closingInput = new PageInput(closingPicker, $"{dayName} Closing Time", false, new[] { checkBoxInput });

                inputs.Add(openingInput);
                labels.Add(openingLabel);

                inputs.Add(closingInput);
                labels.Add(closingLabel);
            }

            AddDayInputs("Every Day", EveryDayCheckBox, EveryDayOpeningLabel, EveryDayOpeningTimePicker, EveryDayClosingLabel, EveryDayClosingTimePicker);
            AddDayInputs("Monday", MondayCheckBox, MondayOpeningLabel, MondayOpeningTimePicker, MondayClosingLabel, MondayClosingTimePicker);
            AddDayInputs("Tuesday", TuesdayCheckBox, TuesdayOpeningLabel, TuesdayOpeningTimePicker, TuesdayClosingLabel, TuesdayClosingTimePicker);
            AddDayInputs("Wednesday", WednesdayCheckBox, WednesdayOpeningLabel, WednesdayOpeningTimePicker, WednesdayClosingLabel, WednesdayClosingTimePicker);
            AddDayInputs("Thursday", ThursdayCheckBox, ThursdayOpeningLabel, ThursdayOpeningTimePicker, ThursdayClosingLabel, ThursdayClosingTimePicker);
            AddDayInputs("Friday", FridayCheckBox, FridayOpeningLabel, FridayOpeningTimePicker, FridayClosingLabel, FridayClosingTimePicker);
            AddDayInputs("Saturday", SaturdayCheckBox, SaturdayOpeningLabel, SaturdayOpeningTimePicker, SaturdayClosingLabel, SaturdayClosingTimePicker);
            AddDayInputs("Sunday", SundayCheckBox, SundayOpeningLabel, SundayOpeningTimePicker, SundayClosingLabel, SundayClosingTimePicker);

            return new PageInputsAndLabels(inputs, labels);
        }

        public Add_Job_Settings()
        {
            InitializeComponent();

        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}
