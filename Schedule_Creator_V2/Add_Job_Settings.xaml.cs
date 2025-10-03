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
        private readonly IReadOnlyList<DateTime?> _timeOptions;
        private readonly IReadOnlyList<DayTimePickerGroup> _dayTimePickerGroups;

        public Add_Job_Settings()
        {
            InitializeComponent();
            _timeOptions = CreateTimeOptions();
            _dayTimePickerGroups = CreateDayTimePickerGroups();

            foreach (DayTimePickerGroup group in _dayTimePickerGroups)
            {
                group.CheckBox.Click += DayCheckBox_Click;
            }
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

        }


        private void DayCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox checkBox)
            {
                return;
            }

            foreach (DayTimePickerGroup group in _dayTimePickerGroups)
            {
                if (!ReferenceEquals(group.CheckBox, checkBox))
                {
                    continue;
                }

                bool isChecked = checkBox.IsChecked == true;

                UpdateTimePicker(group.OpeningPicker, isChecked);
                UpdateTimePicker(group.ClosingPicker, isChecked);

                break;
            }
        }

        private void UpdateTimePicker(TimePickerControl timePicker, bool isEnabled)
        {
            timePicker.IsEnabled = isEnabled;

            if (isEnabled)
            {
                if (!ReferenceEquals(timePicker.ItemsSource, _timeOptions))
                {
                    timePicker.ItemsSource = _timeOptions;
                }

                if (timePicker.Value == null && _timeOptions.Count > 0)
                {
                    timePicker.Value = _timeOptions[0];
                }
            }
            else
            {
                timePicker.ItemsSource = null;
                timePicker.Value = null;
            }
        }

        private static IReadOnlyList<DateTime?> CreateTimeOptions()
        {
            DateTime baseDate = DateTime.Today;
            List<DateTime?> times = new();

            for (int interval = 0; interval < 24 * 4; interval++)
            {
                times.Add(baseDate.AddMinutes(interval * 15));
            }

            return times;
        }

        private IReadOnlyList<DayTimePickerGroup> CreateDayTimePickerGroups()
        {
            return new List<DayTimePickerGroup>
            {
                new(EveryDayCheckBox, EveryDayOpeningTimePicker, EveryDayClosingTimePicker),
                new(MondayCheckBox, MondayOpeningTimePicker, MondayClosingTimePicker),
                new(TuesdayCheckBox, TuesdayOpeningTimePicker, TuesdayClosingTimePicker),
                new(WednesdayCheckBox, WednesdayOpeningTimePicker, WednesdayClosingTimePicker),
                new(ThursdayCheckBox, ThursdayOpeningTimePicker, ThursdayClosingTimePicker),
                new(FridayCheckBox, FridayOpeningTimePicker, FridayClosingTimePicker),
                new(SaturdayCheckBox, SaturdayOpeningTimePicker, SaturdayClosingTimePicker),
                new(SundayCheckBox, SundayOpeningTimePicker, SundayClosingTimePicker)
            };
        }

        private sealed record DayTimePickerGroup(CheckBox CheckBox, TimePickerControl OpeningPicker, TimePickerControl ClosingPicker);

    }
}
