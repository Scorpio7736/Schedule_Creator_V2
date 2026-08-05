using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Services;
using System.Windows;
using System.Windows.Controls;

namespace Schedule_Creator_V2
{
    /// <summary>
    /// Interaction logic for View_Schedule.xaml
    /// </summary>
    public partial class View_Schedule : Page
    {
        private readonly Dictionary<int, string> _staffNameCache = new();

        public View_Schedule()
        {
            InitializeComponent();

            ScheduleComboBox.ItemsSource =
                DatabaseRead.ReadAllScheduleNames();

            SetVisibility();
        }

        private void SetVisibility()
        {
            /*
             * Reset every column first in case the job settings
             * have changed since the page was last loaded.
             */
            MonCol.Visibility = Visibility.Hidden;
            TueCol.Visibility = Visibility.Hidden;
            WedCol.Visibility = Visibility.Hidden;
            ThuCol.Visibility = Visibility.Hidden;
            FriCol.Visibility = Visibility.Hidden;
            SatCol.Visibility = Visibility.Hidden;
            SunCol.Visibility = Visibility.Hidden;

            List<DayOfWeek> jobDays =
                DatabaseRead.ReadJobSettingsDays();

            foreach (DayOfWeek day in jobDays)
            {
                switch (day)
                {
                    case DayOfWeek.Monday:
                        MonCol.Visibility = Visibility.Visible;
                        break;

                    case DayOfWeek.Tuesday:
                        TueCol.Visibility = Visibility.Visible;
                        break;

                    case DayOfWeek.Wednesday:
                        WedCol.Visibility = Visibility.Visible;
                        break;

                    case DayOfWeek.Thursday:
                        ThuCol.Visibility = Visibility.Visible;
                        break;

                    case DayOfWeek.Friday:
                        FriCol.Visibility = Visibility.Visible;
                        break;

                    case DayOfWeek.Saturday:
                        SatCol.Visibility = Visibility.Visible;
                        break;

                    case DayOfWeek.Sunday:
                        SunCol.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        private void FillBoxes(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (ScheduleComboBox.SelectedItem is not string scheduleName ||
                string.IsNullOrWhiteSpace(scheduleName))
            {
                ScheduleGrid.ItemsSource = null;
                return;
            }

            List<ScheduleRow> savedShifts =
                DatabaseRead.ReadScheduleByScheduleName(scheduleName);

            if (savedShifts.Count == 0)
            {
                ScheduleGrid.ItemsSource =
                    new List<ViewScheduleRow>();

                return;
            }

            /*
             * Group the shifts by day.
             *
             * Each day's shifts are sorted by their start time.
             * The first shift from each day appears in row one,
             * the second shift appears in row two, and so on.
             */
            Dictionary<DayOfWeek, List<ScheduleRow>> shiftsByDay =
                savedShifts
                    .GroupBy(shift => shift.dayOfWeek)
                    .ToDictionary(
                        group => group.Key,
                        group => group
                            .OrderBy(shift => shift.startTime)
                            .ThenBy(shift => shift.endTime)
                            .ThenBy(shift => shift.staffID)
                            .ToList());

            int displayRowCount =
                shiftsByDay.Values.Max(shifts => shifts.Count);

            List<ViewScheduleRow> displayRows = new();

            for (int rowIndex = 0;
                 rowIndex < displayRowCount;
                 rowIndex++)
            {
                ViewScheduleRow displayRow = new();

                ApplyShift(
                    displayRow,
                    DayOfWeek.Monday,
                    GetShiftAtIndex(
                        shiftsByDay,
                        DayOfWeek.Monday,
                        rowIndex));

                ApplyShift(
                    displayRow,
                    DayOfWeek.Tuesday,
                    GetShiftAtIndex(
                        shiftsByDay,
                        DayOfWeek.Tuesday,
                        rowIndex));

                ApplyShift(
                    displayRow,
                    DayOfWeek.Wednesday,
                    GetShiftAtIndex(
                        shiftsByDay,
                        DayOfWeek.Wednesday,
                        rowIndex));

                ApplyShift(
                    displayRow,
                    DayOfWeek.Thursday,
                    GetShiftAtIndex(
                        shiftsByDay,
                        DayOfWeek.Thursday,
                        rowIndex));

                ApplyShift(
                    displayRow,
                    DayOfWeek.Friday,
                    GetShiftAtIndex(
                        shiftsByDay,
                        DayOfWeek.Friday,
                        rowIndex));

                ApplyShift(
                    displayRow,
                    DayOfWeek.Saturday,
                    GetShiftAtIndex(
                        shiftsByDay,
                        DayOfWeek.Saturday,
                        rowIndex));

                ApplyShift(
                    displayRow,
                    DayOfWeek.Sunday,
                    GetShiftAtIndex(
                        shiftsByDay,
                        DayOfWeek.Sunday,
                        rowIndex));

                displayRows.Add(displayRow);
            }

            ScheduleGrid.ItemsSource = displayRows;
        }

        private static ScheduleRow? GetShiftAtIndex(
            Dictionary<DayOfWeek, List<ScheduleRow>> shiftsByDay,
            DayOfWeek day,
            int index)
        {
            if (!shiftsByDay.TryGetValue(
                    day,
                    out List<ScheduleRow>? shifts))
            {
                return null;
            }

            if (index < 0 || index >= shifts.Count)
            {
                return null;
            }

            return shifts[index];
        }

        private void ApplyShift(
    ViewScheduleRow displayRow,
    DayOfWeek day,
    ScheduleRow? shift)
        {
            if (shift == null)
            {
                return;
            }

            string staffName =
                shift.staffID.HasValue
                    ? GetStaffName(shift.staffID.Value)
                    : "Missing";

            switch (day)
            {
                case DayOfWeek.Monday:
                    displayRow.AvailMon = staffName;
                    displayRow.MonStartTime = shift.startTime;
                    displayRow.MonEndTime = shift.endTime;
                    break;

                case DayOfWeek.Tuesday:
                    displayRow.AvailTue = staffName;
                    displayRow.TueStartTime = shift.startTime;
                    displayRow.TueEndTime = shift.endTime;
                    break;

                case DayOfWeek.Wednesday:
                    displayRow.AvailWed = staffName;
                    displayRow.WedStartTime = shift.startTime;
                    displayRow.WedEndTime = shift.endTime;
                    break;

                case DayOfWeek.Thursday:
                    displayRow.AvailThu = staffName;
                    displayRow.ThuStartTime = shift.startTime;
                    displayRow.ThuEndTime = shift.endTime;
                    break;

                case DayOfWeek.Friday:
                    displayRow.AvailFri = staffName;
                    displayRow.FriStartTime = shift.startTime;
                    displayRow.FriEndTime = shift.endTime;
                    break;

                case DayOfWeek.Saturday:
                    displayRow.AvailSat = staffName;
                    displayRow.SatStartTime = shift.startTime;
                    displayRow.SatEndTime = shift.endTime;
                    break;

                case DayOfWeek.Sunday:
                    displayRow.AvailSun = staffName;
                    displayRow.SunStartTime = shift.startTime;
                    displayRow.SunEndTime = shift.endTime;
                    break;
            }
        }

        private string GetStaffName(int staffId)
        {
            if (_staffNameCache.TryGetValue(
                    staffId,
                    out string? cachedName))
            {
                return cachedName;
            }

            Staff staff =
                DatabaseRead.ReadStaffByID(staffId);

            string displayName =
                string.IsNullOrWhiteSpace(staff.displayName)
                    ? $"Staff ID: {staffId}"
                    : staff.displayName;

            _staffNameCache[staffId] = displayName;

            return displayName;
        }
    }
}