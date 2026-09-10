using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Services;
using Schedule_Creator_V2.Services.Scheduling;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Schedule_Creator_V2
{
    /// <summary>
    /// Interaction logic for Build_Schedule.xaml
    /// </summary>
    public partial class Build_Schedule : Page
    {
        private const int MAX_SCHEDULE_ROWS = 10;

        private readonly ObservableCollection<BuildScheduleRow> _rows =
            new ObservableCollection<BuildScheduleRow>();

        public Build_Schedule()
        {
            InitializeComponent();

            ScheduleGrid.ItemsSource = _rows;

            SetAvailCol();
        }

        // =========================================================
        // VALIDATION
        // =========================================================

        private bool ErrorChecker()
        {
            string scheduleName =
                ScheduleNameBox.Text;

            if (string.IsNullOrWhiteSpace(scheduleName))
            {
                Messages.Display(
                    new Error(
                        1001,
                        "Schedule must have a name."
                    ));

                return true;
            }

            if (ScheduleService.ScheduleNameExists(scheduleName))
            {
                Messages.Display(
                    new Error(
                        1002,
                        "Schedule name already exists."
                    ));

                return true;
            }

            return false;
        }

        // =========================================================
        // SAVE SCHEDULE
        // =========================================================

        private void SaveSchedule_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (ErrorChecker())
            {
                return;
            }

            string scheduleName =
                ScheduleNameBox.Text.Trim();

            List<ScheduleRow> rowsToSave =
                new List<ScheduleRow>();

            try
            {
                /*
                 * Read all UI values and validate every shift
                 * before inserting anything into SQL Server.
                 */
                foreach (BuildScheduleRow buildRow in _rows)
                {
                    List<DayOfWeekStaffPair> selectedShifts =
                        buildRow.getSelectedStaff();

                    foreach (DayOfWeekStaffPair shift in selectedShifts)
                    {
                        rowsToSave.Add(
                            new ScheduleRow(
                                shift.day,
                                shift.staffID,
                                shift.startTime,
                                shift.endTime,
                                scheduleName
                            ));
                    }
                }
            }
            catch (InvalidOperationException exception)
            {
                Messages.Display(
                    new Error(
                        1004,
                        exception.Message));

                return;
            }

            if (rowsToSave.Count == 0)
            {
                Messages.Display(
                    new Error(
                        1003,
                        "Cannot save an empty schedule."));

                return;
            }

            try
            {
                ScheduleService.SaveSchedule(
                    rowsToSave);
            }
            catch (Exception exception)
            {
                Messages.Display(
                    new Error(
                        1005,
                        $"The schedule could not be saved: " +
                        $"{exception.Message}"));

                return;
            }

            Messages.Display(
                new Message(
                    $"Schedule saved successfully with " +
                    $"{rowsToSave.Count} shift(s)!",
                    "Success"));
        }

        // =========================================================
        // COLUMN VISIBILITY
        // =========================================================

        private void SetColVis(
            List<DayOfWeek> openDays)
        {
            const int MON_COL = 0;
            const int TUE_COL = 1;
            const int WED_COL = 2;
            const int THU_COL = 3;
            const int FRI_COL = 4;
            const int SAT_COL = 5;
            const int SUN_COL = 6;

            Dictionary<DayOfWeek, int> settingDict =
                new Dictionary<DayOfWeek, int>
                {
                    { DayOfWeek.Monday, MON_COL },
                    { DayOfWeek.Tuesday, TUE_COL },
                    { DayOfWeek.Wednesday, WED_COL },
                    { DayOfWeek.Thursday, THU_COL },
                    { DayOfWeek.Friday, FRI_COL },
                    { DayOfWeek.Saturday, SAT_COL },
                    { DayOfWeek.Sunday, SUN_COL }
                };

            foreach (DayOfWeek day in openDays)
            {
                if (settingDict.TryGetValue(
                    day,
                    out int columnIndex))
                {
                    ScheduleGrid
                        .Columns[columnIndex]
                        .Visibility =
                        Visibility.Visible;
                }
            }
        }

        private void SetAvailCol()
        {
            List<DayOfWeek> jobSettingDays =
                ScheduleService
                    .GetConfiguredScheduleDays();

            if (jobSettingDays.Count > 0)
            {
                SetColVis(
                    jobSettingDays);
            }
            else
            {
                Messages.Display(
                    new Error(
                        1000,
                        "No job setting days found. " +
                        "Please configure job settings first."
                    ));
            }
        }

        // =========================================================
        // MANUAL ROW CONTROLS
        // =========================================================

        private void AddRow_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (_rows.Count >= MAX_SCHEDULE_ROWS)
            {
                Messages.Display(
                    new Error(
                        0001,
                        "Too many Shifts Scheduled."));

                return;
            }

            BuildScheduleRow newRow =
                CreateBuildScheduleRow();

            _rows.Add(
                newRow);
        }

        private BuildScheduleRow CreateBuildScheduleRow()
        {
            BuildScheduleRow newRow =
                new BuildScheduleRow();

            newRow.DelBTN.Click +=
                (_, _) => DeleteRow(newRow);

            return newRow;
        }

        private void DeleteRow(
            BuildScheduleRow row)
        {
            if (_rows.Contains(row))
            {
                _rows.Remove(row);
            }
        }

        private void ClearSchedule_Click(
            object sender,
            RoutedEventArgs e)
        {
            _rows.Clear();
        }

        // =========================================================
        // AUTO GENERATE
        // =========================================================

        private void AutoComplete_BTN_Click(
    object sender,
    RoutedEventArgs e)
        {
            if (ErrorChecker())
            {
                return;
            }

            Auto_Generate_Options optionsWindow =
                new Auto_Generate_Options();

            Window? parentWindow =
                Window.GetWindow(this);

            if (parentWindow != null)
            {
                optionsWindow.Owner =
                    parentWindow;
            }

            bool? result =
                optionsWindow.ShowDialog();

            if (result != true)
            {
                return;
            }

            string scheduleName =
                ScheduleNameBox.Text.Trim();

            List<ScheduleRow> generatedSchedule;

            try
            {
                generatedSchedule =
                    ScheduleGeneratorService
                        .AutoGenerateSchedule(
                            scheduleName,
                            optionsWindow.ShiftTemplates,
                            optionsWindow.ExcludedStaffIds);
            }
            catch (Exception exception)
            {
                Messages.Display(
                    new Error(
                        1300,
                        $"The schedule could not be generated: " +
                        $"{exception.Message}"));

                return;
            }

            if (generatedSchedule.Count == 0)
            {
                Messages.Display(
                    new Error(
                        1301,
                        "No shifts were generated."));

                return;
            }

            int requiredRows =
                generatedSchedule
                    .GroupBy(x =>
                        x.dayOfWeek)
                    .Select(group =>
                        group.Count())
                    .DefaultIfEmpty(0)
                    .Max();

            if (requiredRows > 10)
            {
                Messages.Display(
                    new Error(
                        1302,
                        $"The generated schedule requires " +
                        $"{requiredRows} rows. The builder supports " +
                        $"a maximum of 10 shifts per day."));

                return;
            }

            try
            {
                LoadGeneratedScheduleIntoGrid(
                    generatedSchedule);
            }
            catch (Exception exception)
            {
                Messages.Display(
                    new Error(
                        1303,
                        $"The schedule was generated but could not " +
                        $"be loaded into the builder: " +
                        $"{exception.Message}"));

                return;
            }

            int missingCount =
                generatedSchedule.Count(x =>
                    !x.staffID.HasValue);

            string message =
                missingCount == 0
                    ? $"Schedule generated successfully with " +
                      $"{generatedSchedule.Count} shifts."
                    : $"Schedule generated with " +
                      $"{generatedSchedule.Count} shifts. " +
                      $"{missingCount} shift(s) still need staff.";

            Messages.Display(
                new Message(
                    message,
                    "Auto Generate Complete"));
        }

        // =========================================================
        // LOAD GENERATED SCHEDULE
        // =========================================================

        private void LoadGeneratedScheduleIntoGrid(
            List<ScheduleRow> generatedSchedule)
        {
            /*
             * Group all generated shifts by day.
             *
             * Each day's shifts are sorted chronologically.
             */
            Dictionary<DayOfWeek, List<ScheduleRow>> shiftsByDay =
                generatedSchedule
                    .GroupBy(x => x.dayOfWeek)
                    .ToDictionary(
                        group => group.Key,
                        group => group
                            .OrderBy(x => x.startTime)
                            .ThenBy(x => x.endTime)
                            .ThenBy(x => x.staffID)
                            .ToList());

            int rowCount =
                shiftsByDay
                    .Values
                    .Select(shifts => shifts.Count)
                    .DefaultIfEmpty(0)
                    .Max();

            /*
             * Only clear the existing draft after the generator
             * successfully produced a valid schedule.
             */
            _rows.Clear();

            for (int rowIndex = 0;
                 rowIndex < rowCount;
                 rowIndex++)
            {
                BuildScheduleRow buildRow =
                    CreateBuildScheduleRow();

                foreach (
                    KeyValuePair<
                        DayOfWeek,
                        List<ScheduleRow>> dayGroup
                    in shiftsByDay)
                {
                    /*
                     * This day may have fewer shifts than another
                     * day, so there may be nothing to place in this
                     * particular visual row.
                     */
                    if (rowIndex >= dayGroup.Value.Count)
                    {
                        continue;
                    }

                    ScheduleRow shift =
                        dayGroup.Value[rowIndex];

                    SetGeneratedShift(
                        buildRow,
                        shift);
                }

                _rows.Add(
                    buildRow);
            }
        }

        // =========================================================
        // POPULATE ONE GENERATED SHIFT
        // =========================================================

        private static void SetGeneratedShift(
    BuildScheduleRow buildRow,
    ScheduleRow shift)
        {
            /*
             * Real employee:
             *
             *     staffID = actual database ID
             *
             * Missing employee:
             *
             *     staffID = null
             *
             * The BuildScheduleRow ComboBoxes use -1 to represent
             * the Missing option.
             */
            int selectedStaffId =
                shift.staffID ??
                StaffNameAndAvail.MissingStaffId;

            DateTime startDateTime =
                DateTime.Today.Add(
                    shift.startTime.ToTimeSpan());

            DateTime endDateTime =
                DateTime.Today.Add(
                    shift.endTime.ToTimeSpan());

            switch (shift.dayOfWeek)
            {
                case DayOfWeek.Monday:

                    SetComboBoxValue(
                        buildRow.MonBox.Key,
                        selectedStaffId,
                        shift);

                    buildRow.MonStartTime =
                        startDateTime;

                    buildRow.MonEndTime =
                        endDateTime;

                    break;

                case DayOfWeek.Tuesday:

                    SetComboBoxValue(
                        buildRow.TueBox.Key,
                        selectedStaffId,
                        shift);

                    buildRow.TueStartTime =
                        startDateTime;

                    buildRow.TueEndTime =
                        endDateTime;

                    break;

                case DayOfWeek.Wednesday:

                    SetComboBoxValue(
                        buildRow.WedBox.Key,
                        selectedStaffId,
                        shift);

                    buildRow.WedStartTime =
                        startDateTime;

                    buildRow.WedEndTime =
                        endDateTime;

                    break;

                case DayOfWeek.Thursday:

                    SetComboBoxValue(
                        buildRow.ThuBox.Key,
                        selectedStaffId,
                        shift);

                    buildRow.ThuStartTime =
                        startDateTime;

                    buildRow.ThuEndTime =
                        endDateTime;

                    break;

                case DayOfWeek.Friday:

                    SetComboBoxValue(
                        buildRow.FriBox.Key,
                        selectedStaffId,
                        shift);

                    buildRow.FriStartTime =
                        startDateTime;

                    buildRow.FriEndTime =
                        endDateTime;

                    break;

                case DayOfWeek.Saturday:

                    SetComboBoxValue(
                        buildRow.SatBox.Key,
                        selectedStaffId,
                        shift);

                    buildRow.SatStartTime =
                        startDateTime;

                    buildRow.SatEndTime =
                        endDateTime;

                    break;

                case DayOfWeek.Sunday:

                    SetComboBoxValue(
                        buildRow.SunBox.Key,
                        selectedStaffId,
                        shift);

                    buildRow.SunStartTime =
                        startDateTime;

                    buildRow.SunEndTime =
                        endDateTime;

                    break;

                default:

                    throw new InvalidOperationException(
                        $"Unsupported day: " +
                        $"{shift.dayOfWeek}.");
            }
        }

        // =========================================================
        // COMBOBOX SELECTION
        // =========================================================

        private static void SetComboBoxValue(
    ComboBox comboBox,
    int staffId,
    ScheduleRow shift)
        {
            comboBox.SelectedValue =
                staffId;

            if (comboBox.SelectedValue != null)
            {
                return;
            }

            if (staffId ==
                StaffNameAndAvail.MissingStaffId)
            {
                throw new InvalidOperationException(
                    $"The Missing option could not be loaded for " +
                    $"{shift.dayOfWeek} from " +
                    $"{shift.startTime:h:mm tt} to " +
                    $"{shift.endTime:h:mm tt}.");
            }

            throw new InvalidOperationException(
                $"Staff ID {staffId} could not be selected for " +
                $"{shift.dayOfWeek} from " +
                $"{shift.startTime:h:mm tt} to " +
                $"{shift.endTime:h:mm tt}.");
        }
    }
}