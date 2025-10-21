using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Services;
using System.Windows;
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
        public Button DelBTN { get; }

        public BuildScheduleRow()
        {
            MonBox = BuildComboBoxForDay(DayOfWeek.Monday);
            TueBox = BuildComboBoxForDay(DayOfWeek.Tuesday);
            WedBox = BuildComboBoxForDay(DayOfWeek.Wednesday);
            ThuBox = BuildComboBoxForDay(DayOfWeek.Thursday);
            FriBox = BuildComboBoxForDay(DayOfWeek.Friday);
            SatBox = BuildComboBoxForDay(DayOfWeek.Saturday);
            SunBox = BuildComboBoxForDay(DayOfWeek.Sunday);
            DelBTN = new Button { Content = "Delete" };
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
