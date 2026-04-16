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

        private void AutoComplete_BTN_Click(object sender, RoutedEventArgs e)
        {
            Auto_Generate_Options autoGenerateOptions = new Auto_Generate_Options
            {
                Owner = Window.GetWindow(this)
            };

            bool? shouldGenerate = autoGenerateOptions.ShowDialog();

            if (shouldGenerate != true)
            {
                return;
            }

            List<AutoGeneratedScheduleRow> autoGeneratedRows = AutoBuildScheduleService.BuildAutoScheduleRows(
                autoGenerateOptions.StaffPerDay,
                autoGenerateOptions.ExcludedStaffIds
                );

            if (autoGeneratedRows.Count == 0)
            {
                Messages.Display(new Error(
                    1004,
                    "No available staff found to auto generate a schedule with the selected options."
                    ));
                return;
            }

            _rows.Clear();

            foreach (AutoGeneratedScheduleRow autoGeneratedRow in autoGeneratedRows)
            {
                BuildScheduleRow newRow = new BuildScheduleRow();
                newRow.DelBTN.Click += (_, _) => DeleteRow(newRow);

                SetSelectedStaffForRow(newRow, autoGeneratedRow);

                _rows.Add(newRow);
            }

            ScheduleNameBox.Text = AutoBuildScheduleService.BuildNextAutoGeneratedScheduleName();
        }

        private static void SetSelectedStaffForRow(BuildScheduleRow row, AutoGeneratedScheduleRow autoGeneratedRow)
        {
            row.MonBox.Key.SelectedValue = autoGeneratedRow.mondayStaffId;
            row.TueBox.Key.SelectedValue = autoGeneratedRow.tuesdayStaffId;
            row.WedBox.Key.SelectedValue = autoGeneratedRow.wednesdayStaffId;
            row.ThuBox.Key.SelectedValue = autoGeneratedRow.thursdayStaffId;
            row.FriBox.Key.SelectedValue = autoGeneratedRow.fridayStaffId;
            row.SatBox.Key.SelectedValue = autoGeneratedRow.saturdayStaffId;
            row.SunBox.Key.SelectedValue = autoGeneratedRow.sundayStaffId;
        }
    }
}
