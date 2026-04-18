using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Services;
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
        private readonly ObservableCollection<BuildScheduleRow> _rows = new ObservableCollection<BuildScheduleRow>();

        public Build_Schedule()
        {
            InitializeComponent();

            ScheduleGrid.ItemsSource = _rows;

            SetAvailCol();
        }

        private bool ErrorChecker()
        {
            string scheduleName = ScheduleNameBox.Text;

            if (scheduleName == null || scheduleName == "")
            {
                Messages.Display(new Error(
                    1001,
                    "Schedule must have a name."
                    ));
                return true;
            }

            if (DatabaseRead.ReadAllScheduleNames().Contains(scheduleName))
            {
                Messages.Display(new Error(
                    1002,
                    "Schedule name already exists."
                    ));
                return true;
            }

            return false;
        }


        private void SaveSchedule_Click(object sender, RoutedEventArgs e)
        {
            if (ErrorChecker())
            {
                return;
            }

            string scheduleName = ScheduleNameBox.Text;

            int createdRowsCount = 0;

            foreach (BuildScheduleRow row in _rows)
            {
                List<DayOfWeekStaffPair> selectedStaff = row.getSelectedStaff();

                foreach (DayOfWeekStaffPair dayOfWeekStaffPair in selectedStaff)
                {
                    DatabaseCreate.CreateSchedule(new ScheduleRow(
                        dayOfWeekStaffPair.day,
                        dayOfWeekStaffPair.staff.id,
                        scheduleName
                        ));

                    createdRowsCount++;
                }
            }

            if (createdRowsCount == 0)
            {
                Messages.Display(new Error(
                    1003,
                    "Cannot save an empty schedule."
                    ));

                return;
            }

            Messages.Display(new Message(
                "Schedule Saved Successfully!", 
                "Success"
                ));

        }

        private void SetColVis(List<DayOfWeek> openDays)
        {
            int MON_COL = 0, TUE_COL = 1, WED_COL = 2, THU_COL = 3, FRI_COL = 4, SAT_COL = 5, SUN_COL = 6;

            Dictionary<DayOfWeek, int> settingDict = new Dictionary<DayOfWeek, int>
            {
                {DayOfWeek.Monday, MON_COL },
                {DayOfWeek.Tuesday, TUE_COL },
                {DayOfWeek.Wednesday, WED_COL },
                {DayOfWeek.Thursday, THU_COL },
                {DayOfWeek.Friday, FRI_COL },
                {DayOfWeek.Saturday, SAT_COL },
                {DayOfWeek.Sunday, SUN_COL }
            };

            foreach (DayOfWeek day in openDays)
            {
                foreach (KeyValuePair<DayOfWeek, int> dict in settingDict)
                {
                    if (day == dict.Key)
                    {
                        ScheduleGrid.Columns[dict.Value].Visibility = Visibility.Visible;
                        break;
                    }
                }
            }
        }

        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            BuildScheduleRow newRow = new BuildScheduleRow();

            newRow.DelBTN.Click += (_, _) => DeleteRow(newRow);

            if (_rows.Count < 10)
            {
                _rows.Add(newRow);
            }
            else
            {
                Messages.Display(new Error(0001, "Too many Shifts Scheduled."));
            }
        }

        private void DeleteRow(BuildScheduleRow row)
        {
            if (_rows.Contains(row))
            {
                _rows.Remove(row);
            }
        }

        private void ClearSchedule_Click(object sender, RoutedEventArgs e)
        {
            _rows.Clear();
        }

        private void SetAvailCol()
        {
            List<DayOfWeek> jobSettingDays = DatabaseRead.ReadJobSettingsDays();

            if (jobSettingDays.Count > 0)
            {
                SetColVis(jobSettingDays);
            }
            else
            {
                Messages.Display(new Error(
                    1000,
                    "No job setting days found. Please configure job settings first."
                    ));
            }

        }

        private static ComboBox GetComboBoxForDay(BuildScheduleRow row, DayOfWeek day)
        {
            return day switch
            {
                DayOfWeek.Monday => row.MonBox.Key,
                DayOfWeek.Tuesday => row.TueBox.Key,
                DayOfWeek.Wednesday => row.WedBox.Key,
                DayOfWeek.Thursday => row.ThuBox.Key,
                DayOfWeek.Friday => row.FriBox.Key,
                DayOfWeek.Saturday => row.SatBox.Key,
                DayOfWeek.Sunday => row.SunBox.Key,
                _ => throw new ArgumentOutOfRangeException(nameof(day), day, null)
            };
        }

        private void AutoComplete_BTN_Click(object sender, RoutedEventArgs e)
        {
            AutoScheduleGenerator generator = new AutoScheduleGenerator();
            AutoScheduleGenerator.AutoScheduleResult result = generator.Generate();

            if (!result.Success)
            {
                Messages.Display(new Error(1006, result.Message));
                return;
            }

            _rows.Clear();

            const int requiredRows = 3;
            for (int i = 0; i < requiredRows; i++)
            {
                BuildScheduleRow newRow = new BuildScheduleRow();
                newRow.DelBTN.Click += (_, _) => DeleteRow(newRow);
                _rows.Add(newRow);
            }

            foreach (KeyValuePair<DayOfWeek, List<int>> dayAssignment in result.DayAssignments)
            {
                DayOfWeek day = dayAssignment.Key;
                List<int> assignedIds = dayAssignment.Value;

                for (int rowIndex = 0; rowIndex < _rows.Count && rowIndex < assignedIds.Count; rowIndex++)
                {
                    ComboBox comboBox = GetComboBoxForDay(_rows[rowIndex], day);
                    comboBox.SelectedValue = assignedIds[rowIndex];
                }
            }

            ScheduleNameBox.Text = $"Auto Generated Schedule {DateTime.Now:yyyy-MM-dd HH:mm}";

            if (result.UnassignedStaffIds.Count > 0)
            {
                List<Staff> staff = DatabaseRead.ReadStaff();
                Dictionary<int, string> names = staff.ToDictionary(member => member.id, member => member.displayName);

                string unassignedNames = string.Join(", ", result.UnassignedStaffIds.Where(names.ContainsKey).Select(id => names[id]));

                Messages.Display(new Message(
                    $"{result.Message} Unassigned staff: {unassignedNames}. Half-shift splitting is not available in this version.",
                    "Auto-Generate Complete"
                    ));

                return;
            }

            Messages.Display(new Message(
                "Auto-generated schedule created with 3 staff per shift and one lead per day.",
                "Auto-Generate Complete"
                ));
        }
    }
}
