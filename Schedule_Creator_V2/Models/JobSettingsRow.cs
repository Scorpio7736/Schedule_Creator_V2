using System;
using System.Windows;
using System.Windows.Controls;
using TimePickerControl = Xceed.Wpf.Toolkit.TimePicker;

namespace Schedule_Creator_V2.Models
{
    public class JobSettingsRow
    {
        private ComboBox _dayOfWeekBox = new ComboBox();
        private TimePickerControl _startTimePicker;
        private TimePickerControl _endTimePicker;

        public JobSettingsRow()
        {
            this._dayOfWeekBox.ItemsSource = Enum.GetValues(typeof(DayOfWeek));
            this._dayOfWeekBox.Margin = new Thickness(4, 0, 4, 0);
            this._dayOfWeekBox.HorizontalAlignment = HorizontalAlignment.Stretch;

            this._startTimePicker = new TimePickerControl();
            this._startTimePicker.TimeInterval = new TimeSpan(0, 15, 0);
            this._startTimePicker.Margin = new Thickness(4, 0, 4, 0);

            this._endTimePicker = new TimePickerControl();
            this._endTimePicker.TimeInterval = new TimeSpan(0, 15, 0);
            this._endTimePicker.Margin = new Thickness(4, 0, 4, 0);
        }

        public JobSettingsRow(JobSettings settings)
        {
            this._dayOfWeekBox.ItemsSource = Enum.GetValues(typeof(DayOfWeek));
            this._dayOfWeekBox.Margin = new Thickness(4, 0, 4, 0);
            this._dayOfWeekBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            this._dayOfWeekBox.SelectedItem = settings.dayOfWeek;

            this._startTimePicker = new TimePickerControl();
            this._startTimePicker.TimeInterval = new TimeSpan(0, 15, 0);
            this._startTimePicker.Margin = new Thickness(4, 0, 4, 0);
            this._startTimePicker.Value = DateTime.Today.Add(settings.openingTime.ToTimeSpan());

            this._endTimePicker = new TimePickerControl();
            this._endTimePicker.TimeInterval = new TimeSpan(0, 15, 0);
            this._endTimePicker.Margin = new Thickness(4, 0, 4, 0);
            this._endTimePicker.Value = DateTime.Today.Add(settings.closingTime.ToTimeSpan());
        }

        public ComboBox dayOfTheWeek
        {
            get { return this._dayOfWeekBox; }
            set { this._dayOfWeekBox = value; }
        }

        public TimePickerControl startTimePicker
        {
            get { return this._startTimePicker; }
            set { this._startTimePicker = value; }
        }

        public TimePickerControl endTimePicker
        {
            get { return this._endTimePicker; }
            set { this._endTimePicker = value; }
        }

        public bool IsThereNull()
        {
            if (this._dayOfWeekBox.SelectedItem == null)
            {
                return true;
            }

            DateTime? startValue = this._startTimePicker.Value;
            if (startValue.HasValue == false)
            {
                return true;
            }

            DateTime? endValue = this._endTimePicker.Value;
            if (endValue.HasValue == false)
            {
                return true;
            }

            return false;
        }
    }
}
