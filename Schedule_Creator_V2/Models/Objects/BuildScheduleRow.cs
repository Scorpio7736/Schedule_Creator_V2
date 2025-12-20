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
                { WedBox.Key, DayOfWeek.Tuesday }, 
                { ThuBox.Key, DayOfWeek.Tuesday }, 
                { FriBox.Key, DayOfWeek.Tuesday }, 
                { SatBox.Key, DayOfWeek.Tuesday },
                { SunBox.Key, DayOfWeek.Tuesday } 
            };
            
            DelBTN = new Button { Content = "Delete" };
        }

        public List<DayOfWeekStaffPair> getSelectedStaff()
        {
            List<DayOfWeekStaffPair> returnList = new List<DayOfWeekStaffPair>();

            foreach (KeyValuePair<ComboBox, DayOfWeek> pair in DayBoxes)
            {
                if (pair.Key.SelectedValue != null)
                {
                    int selectedStaffId = (int)pair.Key.SelectedValue;
                    Staff? selectedStaff = DatabaseRead.GetStaffByID(selectedStaffId);

                    if (selectedStaff != null)
                    {
                        returnList.Add(new DayOfWeekStaffPair(pair.Value, selectedStaff));
                    }
                }
            }

            return returnList;
        }


        private static ComboBox BuildComboBoxForDay(DayOfWeek day)
        {
            ComboBox comboBox = new ComboBox
            {
                ItemsSource = DatabaseRead.GetStaffNameAndAvail(day),
                DisplayMemberPath = nameof(StaffNameAndAvail.displayName),
                SelectedValuePath = nameof(Staff.id),
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(4, 0, 4, 0)
            };

            return comboBox;
        }
    }
}
