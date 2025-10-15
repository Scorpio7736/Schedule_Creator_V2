using Schedule_Creator_V2.Services;
using System.Windows.Controls;

namespace Schedule_Creator_V2.Models
{
    internal class BuildScheduleRow
    {
        private ComboBox _MonBox { get; set; }
        private ComboBox _TueBox { get; set; }
        private ComboBox _WedBox { get; set; }
        private ComboBox _ThuBox { get; set; }
        private ComboBox _FriBox { get; set; }
        private ComboBox _SatBox { get; set; }
        private ComboBox _SunBox { get; set; }

        public BuildScheduleRow()
        {
            _MonBox = new ComboBox();
            _MonBox.ItemsSource = GetAvailForDay(DayOfWeek.Monday);

            _TueBox = new ComboBox();
            _TueBox.ItemsSource = GetAvailForDay(DayOfWeek.Tuesday);

            _WedBox = new ComboBox();
            _WedBox.ItemsSource = GetAvailForDay(DayOfWeek.Wednesday);

            _ThuBox = new ComboBox();
            _ThuBox.ItemsSource = GetAvailForDay(DayOfWeek.Thursday);

            _FriBox = new ComboBox();
            _FriBox.ItemsSource = GetAvailForDay(DayOfWeek.Friday);

            _SatBox = new ComboBox();
            _SatBox.ItemsSource = GetAvailForDay(DayOfWeek.Saturday);

            _SunBox = new ComboBox();
            _SunBox.ItemsSource = GetAvailForDay(DayOfWeek.Sunday);
        }

        private List<Staff> GetAvailForDay(DayOfWeek day)
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
