using System;
using System.Collections.Generic;
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

        private Dictionary<ComboBox, DayOfWeek> DayBoxes { get; } 

        public BuildScheduleRow()
        {
            MonBox = new KeyValuePair<ComboBox, DayOfWeek>(BuildComboBoxForDay(DayOfWeek.Monday), DayOfWeek.Monday);
            TueBox = new KeyValuePair<ComboBox, DayOfWeek>(BuildComboBoxForDay(DayOfWeek.Tuesday), DayOfWeek.Tuesday);
            WedBox = new KeyValuePair<ComboBox, DayOfWeek>(BuildComboBoxForDay(DayOfWeek.Wednesday), DayOfWeek.Wednesday);
            ThuBox = new KeyValuePair<ComboBox, DayOfWeek>(BuildComboBoxForDay(DayOfWeek.Thursday), DayOfWeek.Thursday);
            FriBox = new KeyValuePair<ComboBox, DayOfWeek>(BuildComboBoxForDay(DayOfWeek.Friday), DayOfWeek.Friday);
            SatBox = new KeyValuePair<ComboBox, DayOfWeek>(BuildComboBoxForDay(DayOfWeek.Saturday), DayOfWeek.Saturday);
            SunBox = new KeyValuePair<ComboBox, DayOfWeek>(BuildComboBoxForDay(DayOfWeek.Sunday), DayOfWeek.Sunday);
            
            DayBoxes = new Dictionary<ComboBox, DayOfWeek>
            {
                { MonBox.Key, DayOfWeek.Monday }, 
                { TueBox.Key, DayOfWeek.Tuesday }, 
                { WedBox.Key, DayOfWeek.Wednesday }, 
                { ThuBox.Key, DayOfWeek.Thursday }, 
                { FriBox.Key, DayOfWeek.Friday }, 
                { SatBox.Key, DayOfWeek.Saturday },
                { SunBox.Key, DayOfWeek.Sunday } 
            };
            
            DelBTN = new Button { Content = "Delete" };
        }

        public List<DayOfWeekStaffPair> getSelectedStaff(TimeOnly startTime, TimeOnly endTime)
        {
            List<DayOfWeekStaffPair> returnList = new List<DayOfWeekStaffPair>();

            foreach (KeyValuePair<ComboBox, DayOfWeek> pair in DayBoxes)
            {
                if (pair.Key.SelectedValue != null)
                {
                    int selectedStaffId = (int)pair.Key.SelectedValue;
                    Staff? selectedStaff = DatabaseRead.ReadStaffByID(selectedStaffId);

                    if (selectedStaff != null)
                    {
                        returnList.Add(new DayOfWeekStaffPair(pair.Value, selectedStaff, TimeOnly.MinValue, TimeOnly.MinValue));
                    }
                }
            }

            return returnList;
        }


        private static ComboBox BuildComboBoxForDay(DayOfWeek day)
        {
            ComboBox comboBox = new ComboBox
            {
                ItemsSource = DatabaseRead.ReadStaffNamesAndAvailOnDay(day),
                DisplayMemberPath = nameof(StaffNameAndAvail.displayName),
                SelectedValuePath = nameof(Staff.id),
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(4, 0, 4, 0)
            };

            return comboBox;
        }
    }
}
