using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Enums;
using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Services;
using Schedule_Creator_V2.Services.Database;
using System.Collections.ObjectModel;
using System.Windows;

namespace Schedule_Creator_V2
{
    public partial class Auto_Generate_Options : Window
    {
        // =========================================================
        // STAFF EXCLUSION MODEL
        // =========================================================

        private sealed class StaffExclusionOption
        {
            public int StaffId { get; init; }

            public string Name { get; init; } =
                string.Empty;

            public bool IsExcluded { get; set; }
        }

        // =========================================================
        // EDITABLE SHIFT ROW
        // =========================================================

        public sealed class ShiftTemplateRow
        {
            public DateTime? StartTime { get; set; }

            public DateTime? EndTime { get; set; }

            public AutoGenShiftType ShiftType { get; set; } =
                AutoGenShiftType.AnyStaff;
        }

        private readonly ObservableCollection<
            StaffExclusionOption> _staffOptions =
            new();

        private readonly ObservableCollection<
            ShiftTemplateRow> _shiftRows =
            new();

        // =========================================================
        // RESULTS RETURNED TO BUILD SCHEDULE
        // =========================================================

        public List<AutoGenShiftTemplate> ShiftTemplates
        {
            get;
            private set;
        } = new();

        public HashSet<int> ExcludedStaffIds
        {
            get;
        } = new();

        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public Auto_Generate_Options()
        {
            InitializeComponent();

            LoadStaff();

            LoadDefaultShifts();

            StaffCheckboxList.ItemsSource =
                _staffOptions;

            ShiftGrid.ItemsSource =
                _shiftRows;
        }

        // =========================================================
        // LOAD STAFF
        // =========================================================

        private void LoadStaff()
        {
            List<Staff> allStaff =
                DatabaseRead
                    .ReadStaff()
                    .OrderBy(staff =>
                        staff.displayName)
                    .ToList();

            foreach (Staff staff in allStaff)
            {
                _staffOptions.Add(
                    new StaffExclusionOption
                    {
                        StaffId = staff.id,
                        Name = staff.displayName,
                        IsExcluded = false
                    });
            }
        }

        // =========================================================
        // DEFAULT SHIFT TEMPLATE
        // =========================================================

        private void LoadDefaultShifts()
        {
            /*
             * Current tower-style defaults.
             *
             * The user can edit/delete these before generating.
             */

            AddShift(
                new TimeOnly(14, 45),
                new TimeOnly(17, 45),
                AutoGenShiftType.Leadership);

            AddShift(
                new TimeOnly(17, 30),
                new TimeOnly(20, 15),
                AutoGenShiftType.Leadership);

            AddShift(
                new TimeOnly(14, 45),
                new TimeOnly(18, 15),
                AutoGenShiftType.AnyStaff);

            AddShift(
                new TimeOnly(16, 45),
                new TimeOnly(20, 15),
                AutoGenShiftType.AnyStaff);
        }

        // =========================================================
        // ADD SHIFT
        // =========================================================

        private void AddShift_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (_shiftRows.Count >= 10)
            {
                Messages.Display(
                    new Error(
                        1200,
                        "A maximum of 10 shift templates is supported."));

                return;
            }

            /*
             * Default a newly-added shift to the operating window
             * if job settings exist.
             */
            JobSettings? firstSetting =
                DatabaseRead
                    .ReadJobSettings()
                    .OrderBy(x =>
                        GetDaySortOrder(x.dayOfWeek))
                    .FirstOrDefault();

            TimeOnly start =
                firstSetting?.openingTime ??
                new TimeOnly(14, 45);

            TimeOnly end =
                firstSetting?.closingTime ??
                new TimeOnly(20, 15);

            AddShift(
                start,
                end,
                AutoGenShiftType.AnyStaff);
        }

        private void AddShift(
            TimeOnly start,
            TimeOnly end,
            AutoGenShiftType shiftType)
        {
            _shiftRows.Add(
                new ShiftTemplateRow
                {
                    StartTime =
                        DateTime.Today.Add(
                            start.ToTimeSpan()),

                    EndTime =
                        DateTime.Today.Add(
                            end.ToTimeSpan()),

                    ShiftType =
                        shiftType
                });
        }

        // =========================================================
        // DELETE SHIFT
        // =========================================================

        private void DeleteShift_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is not FrameworkElement element)
            {
                return;
            }

            if (element.DataContext is not ShiftTemplateRow row)
            {
                return;
            }

            _shiftRows.Remove(row);
        }

        // =========================================================
        // GENERATE
        // =========================================================

        private void Generate_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (_shiftRows.Count == 0)
            {
                Messages.Display(
                    new Error(
                        1201,
                        "Add at least one shift before generating."));

                return;
            }

            if (_shiftRows.Count > 10)
            {
                Messages.Display(
                    new Error(
                        1202,
                        "A maximum of 10 shift templates is supported."));

                return;
            }

            List<AutoGenShiftTemplate> templates =
                new();

            for (int i = 0;
                 i < _shiftRows.Count;
                 i++)
            {
                ShiftTemplateRow row =
                    _shiftRows[i];

                if (!row.StartTime.HasValue ||
                    !row.EndTime.HasValue)
                {
                    Messages.Display(
                        new Error(
                            1203,
                            $"Shift {i + 1} requires both a start and end time."));

                    return;
                }

                TimeOnly start =
                    NormalizeTime(
                        TimeOnly.FromDateTime(
                            row.StartTime.Value));

                TimeOnly end =
                    NormalizeTime(
                        TimeOnly.FromDateTime(
                            row.EndTime.Value));

                if (end <= start)
                {
                    Messages.Display(
                        new Error(
                            1204,
                            $"Shift {i + 1} must end after it starts."));

                    return;
                }

                templates.Add(
                    new AutoGenShiftTemplate(
                        start,
                        end,
                        row.ShiftType));
            }

            ShiftTemplates =
                templates;

            ExcludedStaffIds.Clear();

            foreach (
                StaffExclusionOption option
                in _staffOptions.Where(
                    option =>
                        option.IsExcluded))
            {
                ExcludedStaffIds.Add(
                    option.StaffId);
            }

            DialogResult = true;

            Close();
        }

        // =========================================================
        // HELPERS
        // =========================================================

        private static TimeOnly NormalizeTime(
            TimeOnly time)
        {
            return new TimeOnly(
                time.Hour,
                time.Minute);
        }

        private static int GetDaySortOrder(
            DayOfWeek day)
        {
            return day switch
            {
                DayOfWeek.Monday => 1,
                DayOfWeek.Tuesday => 2,
                DayOfWeek.Wednesday => 3,
                DayOfWeek.Thursday => 4,
                DayOfWeek.Friday => 5,
                DayOfWeek.Saturday => 6,
                DayOfWeek.Sunday => 7,
                _ => 8
            };
        }
    }
}