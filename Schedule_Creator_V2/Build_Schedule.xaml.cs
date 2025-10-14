using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Services;
using System.Windows;
using System.Windows.Controls;

namespace Schedule_Creator_V2
{
    /// <summary>
    /// Interaction logic for Build_Schedule.xaml
    /// </summary>
    public partial class Build_Schedule : Page
    {
        public Build_Schedule()
        {
            InitializeComponent();

            SetAvailCol();

        }

        private void SetColVis(List<DayOfWeek> openDays)
        {
            //TODO: find a better way to do this PLEASE!!!!!!!!!!!!!!!!!

            int MON_COL = 0, TUE_COL = 1, WED_COL = 2, THU_COL = 3, FRI_COL = 4, SAT_COL = 5, SUN_COL = 6;

            foreach (DayOfWeek day in openDays)
            {
                if (day == DayOfWeek.Monday)
                {
                    ScheduleGrid.Columns[MON_COL].Visibility = System.Windows.Visibility.Visible;
                    continue;
                }

                if (day == DayOfWeek.Tuesday)
                {
                    ScheduleGrid.Columns[TUE_COL].Visibility = System.Windows.Visibility.Visible;
                    continue;
                }

                if (day == DayOfWeek.Wednesday)
                {
                    ScheduleGrid.Columns[WED_COL].Visibility = System.Windows.Visibility.Visible;
                    continue;
                }

                if (day == DayOfWeek.Thursday)
                {
                    ScheduleGrid.Columns[THU_COL].Visibility = System.Windows.Visibility.Visible;
                    continue;
                }

                if (day == DayOfWeek.Friday)
                {
                    ScheduleGrid.Columns[FRI_COL].Visibility = System.Windows.Visibility.Visible;
                    continue;
                }

                if (day == DayOfWeek.Saturday)
                {
                    ScheduleGrid.Columns[SAT_COL].Visibility = System.Windows.Visibility.Visible;
                    continue;
                }

                if (day == DayOfWeek.Sunday)
                {
                    ScheduleGrid.Columns[SUN_COL].Visibility = System.Windows.Visibility.Visible;
                    continue;
                }
            }
        }

        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            BuildScheduleRow newRow = new BuildScheduleRow();







            ScheduleGrid.Items.Add(newRow);
        }

        private void SetAvailCol()
        {
            List<JobSettings> settings = DatabaseRead.ReadJobSettings();
            List<DayOfWeek> openDays = new List<DayOfWeek>();

            if (settings.Count() > 0)
            {
                foreach (JobSettings setting in settings)
                {
                    if ( ! openDays.Contains(setting.dayOfWeek))
                    {
                        openDays.Add(setting.dayOfWeek);
                    }
                }

                SetColVis(openDays);
                
            }
            else
            {
                Messages.Display(new Error(
                    1000,
                    "No Job Hours Saved!"
                    ));
            }

        }
    }
}
