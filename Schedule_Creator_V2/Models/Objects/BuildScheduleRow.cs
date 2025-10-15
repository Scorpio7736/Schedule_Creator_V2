using Schedule_Creator_V2.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Schedule_Creator_V2.Models
{
    internal class BuildScheduleRow
    {
        public ComboBox MonBox { get; }
        public ComboBox TueBox { get; }
        public ComboBox WedBox { get; }
        public ComboBox ThuBox { get; }
        public ComboBox FriBox { get; }
        public ComboBox SatBox { get; }
        public ComboBox SunBox { get; }

        public BuildScheduleRow()
        {
            MonBox = BuildComboBoxForDay(DayOfWeek.Monday);
            TueBox = BuildComboBoxForDay(DayOfWeek.Tuesday);
            WedBox = BuildComboBoxForDay(DayOfWeek.Wednesday);
            ThuBox = BuildComboBoxForDay(DayOfWeek.Thursday);
            FriBox = BuildComboBoxForDay(DayOfWeek.Friday);
            SatBox = BuildComboBoxForDay(DayOfWeek.Saturday);
            SunBox = BuildComboBoxForDay(DayOfWeek.Sunday);
        }

        private static ComboBox BuildComboBoxForDay(DayOfWeek day)
        {
            ComboBox comboBox = new ComboBox
            {
                ItemsSource = GetAvailForDay(day),
                DisplayMemberPath = nameof(Staff.displayName),
                SelectedValuePath = nameof(Staff.id),
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(4, 0, 4, 0)
            };

            return comboBox;
        }

        private static List<Staff> GetAvailForDay(DayOfWeek day)
        {
            List<Staff> returnList = new List<Staff>();
            List<Staff> availOnDay = DatabaseRead.ReadStaffAvailOnDay(day);

            foreach (Staff staff in availOnDay)
            {
                if (!returnList.Contains(staff))
                {
                    returnList.Add(staff);
                }
            }

            return returnList;
        }
    }
}
