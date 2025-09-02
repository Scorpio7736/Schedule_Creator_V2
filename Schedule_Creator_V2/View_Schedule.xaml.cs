using Schedule_Creator_V2.ExtensionMethods;
using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Services;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Schedule_Creator_V2
{
    /// <summary>
    /// Interaction logic for View_Schedule.xaml
    /// </summary>
    public partial class View_Schedule : Page
    {
        public View_Schedule()
        {
            InitializeComponent();
        }

        private void FillGrid(object sender, RoutedEventArgs e)
        {
            DateOnly selectedDate = DateOnly.FromDateTime(ScheduleDatePicker.SelectedDate.Value);
            var staffList = ScheduleViewer.BuildSchedule(selectedDate);
            var availabilities = DatabaseRead.ReadAvailability();

            var availabilityLookup = availabilities.ToLookup(a => a.id);
            var selectedDay = Enum.Parse<AvailDays>(selectedDate.DayOfWeek.ToString());


            // TODO: find logic about this stream. Add clause where if there is a staff member who is a sub, they will be added to every day with avail times as UPON REQUEST.
            var scheduleLines = staffList
                .SelectMany(staff =>
                    availabilityLookup[staff.id]
                        .Where(
                        ava => ava.dayOfTheWeek == selectedDay 
                        || 
                        ava.dayOfTheWeek == AvailDays.Every_Day
                        )
                        .Select(ava => new ScheduleEntry(
                            staff.fName,
                            staff.lName,
                            ava.availTimes,
                            (staff.position.ToString()),
                            staff.certRange
                        ))
                )
                .ToList();

            ScheduleDataGrid.ItemsSource = scheduleLines;
            CheckSupervisorStaff(scheduleLines);
        }

        private void CheckSupervisorStaff(List<ScheduleEntry> scheduleLines)
        {
            int supCount = 0;
            List<string> warningMSG = new List<string>();

            foreach (ScheduleEntry line in scheduleLines)
            {
                if (
                    Enum.Parse<Positions>(line.position) == Positions.Shift_Lead
                    ||
                    Enum.Parse<Positions>(line.position) == Positions.Lead_Supervisor
                    )
                {
                    supCount++;
                }
            }

            
            if (supCount < 1)
            {
                warningMSG.Add("not enough supervisory staff");
            }

            if (scheduleLines.Count < 3)
            {
                warningMSG.Add("not enough staff to schedule a full shift");
            }

            if (warningMSG.Count > 0)
            {
                Messages.Display(new Warning(string.Join(", ", warningMSG) + "."));
            }
            
            
        }
    }
}
