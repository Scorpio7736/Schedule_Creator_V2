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
        private readonly List<JobSettingInput> _jobSettingInputs;

        public Add_Job_Settings()
        {
            InitializeComponent();

            _jobSettingInputs = new List<JobSettingInput>
            {
                new JobSettingInput(DayOfWeek.Monday, MondayCheckBox, MondayOpeningTimePicker, MondayClosingTimePicker),
                new JobSettingInput(DayOfWeek.Tuesday, TuesdayCheckBox, TuesdayOpeningTimePicker, TuesdayClosingTimePicker),
                new JobSettingInput(DayOfWeek.Wednesday, WednesdayCheckBox, WednesdayOpeningTimePicker, WednesdayClosingTimePicker),
                new JobSettingInput(DayOfWeek.Thursday, ThursdayCheckBox, ThursdayOpeningTimePicker, ThursdayClosingTimePicker),
                new JobSettingInput(DayOfWeek.Friday, FridayCheckBox, FridayOpeningTimePicker, FridayClosingTimePicker),
                new JobSettingInput(DayOfWeek.Saturday, SaturdayCheckBox, SaturdayOpeningTimePicker, SaturdayClosingTimePicker),
                new JobSettingInput(DayOfWeek.Sunday, SundayCheckBox, SundayOpeningTimePicker, SundayClosingTimePicker)
            };

            foreach (var input in _jobSettingInputs)
            {
                input.CheckBox.Checked += DayCheckBox_StateChanged;
                input.CheckBox.Unchecked += DayCheckBox_StateChanged;
            }

            EveryDayCheckBox.Checked += EveryDayCheckBox_StateChanged;
            EveryDayCheckBox.Unchecked += EveryDayCheckBox_StateChanged;

            ResetInputs();
        }

        private void DayCheckBox_StateChanged(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                var input = _jobSettingInputs.FirstOrDefault(i => i.CheckBox == checkBox);
                if (input is not null)
                {
                    UpdateTimePickerState(input);
                }
            }
        }

        private void EveryDayCheckBox_StateChanged(object sender, RoutedEventArgs e)
        {
            bool isEveryDay = EveryDayCheckBox.IsChecked == true;

            EveryDayOpeningTimePicker.IsEnabled = isEveryDay;
            EveryDayClosingTimePicker.IsEnabled = isEveryDay;

            if (!isEveryDay)
            {
                EveryDayOpeningTimePicker.Value = null;
                EveryDayClosingTimePicker.Value = null;
            }

            foreach (var input in _jobSettingInputs)
            {
                input.CheckBox.IsEnabled = !isEveryDay;

                if (!isEveryDay)
                {
                    UpdateTimePickerState(input);
                }
                else
                {
                    input.CheckBox.IsChecked = false;
                    input.OpeningTimePicker.Value = null;
                    input.ClosingTimePicker.Value = null;
                    input.OpeningTimePicker.IsEnabled = false;
                    input.ClosingTimePicker.IsEnabled = false;
                }
            }
        }

        private void UpdateTimePickerState(JobSettingInput input)
        {
            bool isEnabled = input.CheckBox.IsChecked == true;
            input.OpeningTimePicker.IsEnabled = isEnabled;
            input.ClosingTimePicker.IsEnabled = isEnabled;

            if (!isEnabled)
            {
                input.OpeningTimePicker.Value = null;
                input.ClosingTimePicker.Value = null;
            }
        }

        private void ResetInputs()
        {
            EveryDayCheckBox.IsChecked = false;
            EveryDayOpeningTimePicker.IsEnabled = false;
            EveryDayOpeningTimePicker.Value = null;
            EveryDayClosingTimePicker.IsEnabled = false;
            EveryDayClosingTimePicker.Value = null;

            foreach (var input in _jobSettingInputs)
            {
                input.CheckBox.IsChecked = false;
                input.CheckBox.IsEnabled = true;
                input.OpeningTimePicker.IsEnabled = false;
                input.OpeningTimePicker.Value = null;
                input.ClosingTimePicker.IsEnabled = false;
                input.ClosingTimePicker.Value = null;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsToSave = new List<JobSettings>();

            if (EveryDayCheckBox.IsChecked == true)
            {
                if (EveryDayOpeningTimePicker.Value is not DateTime everyDayOpen || EveryDayClosingTimePicker.Value is not DateTime everyDayClose)
                {
                    Messages.Display(new Error(1000, "Please provide both opening and closing times for every day."));
                    return;
                }

                foreach (var input in _jobSettingInputs)
                {
                    settingsToSave.Add(new JobSettings(
                        input.Day,
                        TimeOnly.FromDateTime(everyDayOpen),
                        TimeOnly.FromDateTime(everyDayClose)));
                }
            }
            else
            {
                foreach (var input in _jobSettingInputs)
                {
                    if (input.CheckBox.IsChecked == true)
                    {
                        if (input.OpeningTimePicker.Value is not DateTime opening || input.ClosingTimePicker.Value is not DateTime closing)
                        {
                            Messages.Display(new Error(1000, $"Please provide both opening and closing times for {input.Day}."));
                            return;
                        }

                        settingsToSave.Add(new JobSettings(
                            input.Day,
                            TimeOnly.FromDateTime(opening),
                            TimeOnly.FromDateTime(closing)));
                    }
                }
            }

            if (!settingsToSave.Any())
            {
                Messages.Display(new Error(1000, "Please select at least one day to configure."));
                return;
            }

            foreach (var jobSetting in settingsToSave)
            {
                DatabaseCreate.RemoveJobSetting(jobSetting.dayOfWeek);
                DatabaseCreate.AddJobSettings(jobSetting.dayOfWeek, jobSetting.openingTime, jobSetting.closingTime);
            }

            Messages.Display(new Message("Job settings saved successfully!", "Job Settings"));
            ResetInputs();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ResetInputs();
        }

        private record JobSettingInput(
            DayOfWeek Day,
            CheckBox CheckBox,
            TimePicker OpeningTimePicker,
            TimePicker ClosingTimePicker);
    }
}
