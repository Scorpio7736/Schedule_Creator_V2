using Schedule_Creator_V2.Models;
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

            _rows.Add(newRow);
        }

        private void DeleteRow(BuildScheduleRow row)
        {
            if (_rows.Contains(row))
            {
                _rows.Remove(row);
            }
        }

        private void SetAvailCol()
        {
            List<DayOfWeek> openDays = new List<DayOfWeek>();

            DayOfWeek[] allDays =
            {
                DayOfWeek.Monday,
                DayOfWeek.Tuesday,
                DayOfWeek.Wednesday,
                DayOfWeek.Thursday,
                DayOfWeek.Friday,
                DayOfWeek.Saturday,
                DayOfWeek.Sunday
            };

            foreach (DayOfWeek day in allDays)
            {
                if (DatabaseRead.GetStaffNameAndAvail(day).Count > 0)
                {
                    openDays.Add(day);
                }
            }

            if (openDays.Count > 0)
            {
                SetColVis(openDays);
            }
            else
            {
                Messages.Display(new Error(
                    1000,
                    "No Staff Availability Saved!"
                    ));
            }

        }
    }
}
