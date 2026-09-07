using Microsoft.Win32;
using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Services.Scheduling;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Schedule_Creator_V2
{
    /// <summary>
    /// Interaction logic for View_Schedule.xaml
    /// </summary>
    public partial class View_Schedule : Page
    {
        private readonly Dictionary<int, string>
            _staffNameCache = new();

        public View_Schedule()
        {
            InitializeComponent();

            ScheduleComboBox.ItemsSource = ScheduleService.GetScheduleNames();

            HideAllScheduleColumns();
        }

        /// <summary>
        /// Hides all schedule columns.
        /// Columns will be shown based on the days that actually
        /// exist in the selected saved schedule.
        /// </summary>
        private void HideAllScheduleColumns()
        {
            MonCol.Visibility = Visibility.Hidden;
            TueCol.Visibility = Visibility.Hidden;
            WedCol.Visibility = Visibility.Hidden;
            ThuCol.Visibility = Visibility.Hidden;
            FriCol.Visibility = Visibility.Hidden;
            SatCol.Visibility = Visibility.Hidden;
            SunCol.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Shows only the columns for days contained in the
        /// selected saved schedule.
        /// </summary>
        private void SetVisibility(
            IEnumerable<DayOfWeek> scheduleDays)
        {
            HideAllScheduleColumns();

            foreach (DayOfWeek day
                     in scheduleDays.Distinct())
            {
                switch (day)
                {
                    case DayOfWeek.Monday:
                        MonCol.Visibility =
                            Visibility.Visible;
                        break;

                    case DayOfWeek.Tuesday:
                        TueCol.Visibility =
                            Visibility.Visible;
                        break;

                    case DayOfWeek.Wednesday:
                        WedCol.Visibility =
                            Visibility.Visible;
                        break;

                    case DayOfWeek.Thursday:
                        ThuCol.Visibility =
                            Visibility.Visible;
                        break;

                    case DayOfWeek.Friday:
                        FriCol.Visibility =
                            Visibility.Visible;
                        break;

                    case DayOfWeek.Saturday:
                        SatCol.Visibility =
                            Visibility.Visible;
                        break;

                    case DayOfWeek.Sunday:
                        SunCol.Visibility =
                            Visibility.Visible;
                        break;
                }
            }
        }

        private void FillBoxes(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (ScheduleComboBox.SelectedItem
                    is not string scheduleName ||
                string.IsNullOrWhiteSpace(
                    scheduleName))
            {
                ScheduleGrid.ItemsSource =
                    Array.Empty<ViewScheduleRow>();

                HideAllScheduleColumns();

                return;
            }

            List<ScheduleRow> savedShifts =
    ScheduleService
        .GetSchedule(
            scheduleName);

            if (savedShifts.Count == 0)
            {
                ScheduleGrid.ItemsSource =
                    Array.Empty<ViewScheduleRow>();

                HideAllScheduleColumns();

                return;
            }

            /*
             * Determine which columns should be visible from
             * the selected schedule itself instead of the
             * current JobSettings table.
             */
            SetVisibility(
                savedShifts.Select(
                    shift =>
                        shift.dayOfWeek));

            /*
             * Group the shifts by day.
             *
             * Each day's shifts are sorted by their start time.
             * The first shift from each day appears in row one,
             * the second shift appears in row two, and so on.
             */
            Dictionary<DayOfWeek, List<ScheduleRow>>
                shiftsByDay =
                    savedShifts
                        .GroupBy(
                            shift =>
                                shift.dayOfWeek)
                        .ToDictionary(
                            group =>
                                group.Key,

                            group =>
                                group
                                    .OrderBy(
                                        shift =>
                                            shift.startTime)
                                    .ThenBy(
                                        shift =>
                                            shift.endTime)
                                    .ThenBy(
                                        shift =>
                                            shift.staffID)
                                    .ToList());

            int displayRowCount =
                shiftsByDay.Values
                    .Max(
                        shifts =>
                            shifts.Count);

            List<ViewScheduleRow> displayRows =
                new();

            for (int rowIndex = 0;
                 rowIndex < displayRowCount;
                 rowIndex++)
            {
                ViewScheduleRow displayRow =
                    new();

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

                displayRows.Add(
                    displayRow);
            }

            ScheduleGrid.ItemsSource =
                displayRows;
        }

        private static ScheduleRow?
            GetShiftAtIndex(
                Dictionary<
                    DayOfWeek,
                    List<ScheduleRow>>
                    shiftsByDay,
                DayOfWeek day,
                int index)
        {
            if (!shiftsByDay.TryGetValue(
                    day,
                    out List<ScheduleRow>? shifts))
            {
                return null;
            }

            if (index < 0 ||
                index >= shifts.Count)
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
            if (shift is null)
            {
                return;
            }

            string staffName =
                shift.staffID.HasValue
                    ? GetStaffName(
                        shift.staffID.Value)
                    : "Missing";

            switch (day)
            {
                case DayOfWeek.Monday:
                    displayRow.AvailMon =
                        staffName;

                    displayRow.MonStartTime =
                        shift.startTime;

                    displayRow.MonEndTime =
                        shift.endTime;
                    break;

                case DayOfWeek.Tuesday:
                    displayRow.AvailTue =
                        staffName;

                    displayRow.TueStartTime =
                        shift.startTime;

                    displayRow.TueEndTime =
                        shift.endTime;
                    break;

                case DayOfWeek.Wednesday:
                    displayRow.AvailWed =
                        staffName;

                    displayRow.WedStartTime =
                        shift.startTime;

                    displayRow.WedEndTime =
                        shift.endTime;
                    break;

                case DayOfWeek.Thursday:
                    displayRow.AvailThu =
                        staffName;

                    displayRow.ThuStartTime =
                        shift.startTime;

                    displayRow.ThuEndTime =
                        shift.endTime;
                    break;

                case DayOfWeek.Friday:
                    displayRow.AvailFri =
                        staffName;

                    displayRow.FriStartTime =
                        shift.startTime;

                    displayRow.FriEndTime =
                        shift.endTime;
                    break;

                case DayOfWeek.Saturday:
                    displayRow.AvailSat =
                        staffName;

                    displayRow.SatStartTime =
                        shift.startTime;

                    displayRow.SatEndTime =
                        shift.endTime;
                    break;

                case DayOfWeek.Sunday:
                    displayRow.AvailSun =
                        staffName;

                    displayRow.SunStartTime =
                        shift.startTime;

                    displayRow.SunEndTime =
                        shift.endTime;
                    break;
            }
        }

        private string GetStaffName(
            int staffId)
        {
            if (_staffNameCache.TryGetValue(
                    staffId,
                    out string? cachedName))
            {
                return cachedName;
            }

            string displayName = ScheduleService.GetStaffDisplayName(staffId);

            _staffNameCache[staffId] =
                displayName;

            return displayName;
        }

        // =========================================================
        // DOWNLOAD SCHEDULE IMAGE
        // =========================================================

        private void DownloadScheduleImageButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (ScheduleGrid.Items.Count == 0)
            {
                MessageBox.Show(
                    "Select a schedule before downloading its image.",
                    "No Schedule Selected",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                ScheduleComboBox.Focus();

                ScheduleComboBox.IsDropDownOpen =
                    true;

                return;
            }

            SaveFileDialog saveDialog =
                new SaveFileDialog
                {
                    Title =
                        "Save Schedule Image",

                    Filter =
                        "PNG Image (*.png)|*.png",

                    DefaultExt =
                        ".png",

                    AddExtension =
                        true,

                    FileName =
    ScheduleImageExportService
        .BuildFileName(
            ScheduleComboBox.SelectedItem
                as string)
                };

            if (saveDialog.ShowDialog() !=
                true)
            {
                return;
            }

            Button? downloadButton =
                sender as Button;

            try
            {
                if (downloadButton
                    is not null)
                {
                    downloadButton.IsEnabled =
                        false;
                }

                byte[] imageBytes =
    ScheduleImageExportService
        .CreatePng(
            ScheduleGrid);

                File.WriteAllBytes(
                    saveDialog.FileName,
                    imageBytes);

                MessageBox.Show(
                    "The schedule image was saved successfully.\n\n" +
                    saveDialog.FileName,
                    "Schedule Image Saved",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    "The schedule image could not be saved.\n\n" +
                    exception.Message,
                    "Schedule Image Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                if (downloadButton
                    is not null)
                {
                    downloadButton.IsEnabled =
                        true;
                }
            }
        }
    }
}