using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Services;
using System.Windows;
using System.Windows.Controls;

namespace Schedule_Creator_V2.Models
{
    internal class BuildScheduleRow
    {
        public KeyValuePair<ComboBox, DayOfWeek> MonBox { get; }
        public KeyValuePair<ComboBox, DayOfWeek> TueBox { get; }
        public KeyValuePair<ComboBox, DayOfWeek> WedBox { get; }
        public KeyValuePair<ComboBox, DayOfWeek> ThuBox { get; }
        public KeyValuePair<ComboBox, DayOfWeek> FriBox { get; }
        public KeyValuePair<ComboBox, DayOfWeek> SatBox { get; }
        public KeyValuePair<ComboBox, DayOfWeek> SunBox { get; }

        public DateTime? MonStartTime { get; set; }
        public DateTime? MonEndTime { get; set; }

        public DateTime? TueStartTime { get; set; }
        public DateTime? TueEndTime { get; set; }

        public DateTime? WedStartTime { get; set; }
        public DateTime? WedEndTime { get; set; }

        public DateTime? ThuStartTime { get; set; }
        public DateTime? ThuEndTime { get; set; }

        public DateTime? FriStartTime { get; set; }
        public DateTime? FriEndTime { get; set; }

        public DateTime? SatStartTime { get; set; }
        public DateTime? SatEndTime { get; set; }

        public DateTime? SunStartTime { get; set; }
        public DateTime? SunEndTime { get; set; }

        public Button DelBTN { get; }

        public BuildScheduleRow()
        {
            MonBox = CreateDayBox(DayOfWeek.Monday);
            TueBox = CreateDayBox(DayOfWeek.Tuesday);
            WedBox = CreateDayBox(DayOfWeek.Wednesday);
            ThuBox = CreateDayBox(DayOfWeek.Thursday);
            FriBox = CreateDayBox(DayOfWeek.Friday);
            SatBox = CreateDayBox(DayOfWeek.Saturday);
            SunBox = CreateDayBox(DayOfWeek.Sunday);

            DelBTN = new Button
            {
                Content = "Delete"
            };
        }

        public List<DayOfWeekStaffPair> getSelectedStaff()
        {
            List<DayOfWeekStaffPair> selectedShifts = new();

            AddSelectedShift(
                selectedShifts,
                MonBox,
                MonStartTime,
                MonEndTime);

            AddSelectedShift(
                selectedShifts,
                TueBox,
                TueStartTime,
                TueEndTime);

            AddSelectedShift(
                selectedShifts,
                WedBox,
                WedStartTime,
                WedEndTime);

            AddSelectedShift(
                selectedShifts,
                ThuBox,
                ThuStartTime,
                ThuEndTime);

            AddSelectedShift(
                selectedShifts,
                FriBox,
                FriStartTime,
                FriEndTime);

            AddSelectedShift(
                selectedShifts,
                SatBox,
                SatStartTime,
                SatEndTime);

            AddSelectedShift(
                selectedShifts,
                SunBox,
                SunStartTime,
                SunEndTime);

            return selectedShifts;
        }

        private static void AddSelectedShift(
    List<DayOfWeekStaffPair> selectedShifts,
    KeyValuePair<ComboBox, DayOfWeek> dayBox,
    DateTime? selectedStartTime,
    DateTime? selectedEndTime)
        {
            // Nothing was selected, so no shift should be saved.
            if (dayBox.Key.SelectedValue == null)
            {
                return;
            }

            if (!selectedStartTime.HasValue)
            {
                throw new InvalidOperationException(
                    $"A start time is required for {dayBox.Value}.");
            }

            if (!selectedEndTime.HasValue)
            {
                throw new InvalidOperationException(
                    $"An end time is required for {dayBox.Value}.");
            }

            TimeOnly startTime =
                TimeOnly.FromDateTime(selectedStartTime.Value);

            TimeOnly endTime =
                TimeOnly.FromDateTime(selectedEndTime.Value);

            if (endTime <= startTime)
            {
                throw new InvalidOperationException(
                    $"The end time must be later than the start time for " +
                    $"{dayBox.Value}. Selected time: " +
                    $"{startTime:h:mm tt} - {endTime:h:mm tt}.");
            }

            int selectedValue =
                Convert.ToInt32(dayBox.Key.SelectedValue);

            /*
             * The ComboBox uses -1 for the UI option,
             * but the database uses NULL for a missing employee.
             */
            int? staffID =
                selectedValue == StaffNameAndAvail.MissingStaffId
                    ? null
                    : selectedValue;

            selectedShifts.Add(
                new DayOfWeekStaffPair(
                    dayBox.Value,
                    staffID,
                    startTime,
                    endTime));
        }

        private static KeyValuePair<ComboBox, DayOfWeek> CreateDayBox(
            DayOfWeek day)
        {
            return new KeyValuePair<ComboBox, DayOfWeek>(
                BuildComboBoxForDay(day),
                day);
        }

        private static ComboBox BuildComboBoxForDay(DayOfWeek day)
        {
            List<StaffNameAndAvail> staffOptions =
                DatabaseRead.ReadStaffNamesAndAvailOnDay(day);

            staffOptions.Insert(
                0,
                StaffNameAndAvail.MissingOption);

            return new ComboBox
            {
                ItemsSource = staffOptions,

                DisplayMemberPath =
                    nameof(StaffNameAndAvail.displayName),

                SelectedValuePath =
                    nameof(StaffNameAndAvail.id),

                HorizontalContentAlignment =
                    HorizontalAlignment.Left,

                Margin =
                    new Thickness(4, 0, 4, 0)
            };
        }
    }
}