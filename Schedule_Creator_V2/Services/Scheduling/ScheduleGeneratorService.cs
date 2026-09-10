using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Services.Database;
using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Models;

namespace Schedule_Creator_V2.Services
{
    public static class ScheduleGeneratorService
    {
        public static List<ScheduleRow> AutoGenerateSchedule()
        {
            var returnList = new List<ScheduleRow>();
            var data = DatabaseRead.ReadAutoGenScheduleData();
            var jobSettings = DatabaseRead.ReadJobSettings();

            var minRequiredAtAllTimes = 0;
            var leadershipRequiredAtAllTimes = true;
            var staffNotOnSchedule = new List<Staff>();

            foreach( var jobSetting in jobSettings)
            {

            }


            return returnList;
        }
    }
}
