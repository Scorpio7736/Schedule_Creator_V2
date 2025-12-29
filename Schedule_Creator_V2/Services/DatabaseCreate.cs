using Microsoft.Data.SqlClient;
using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Enums;
using Schedule_Creator_V2.Models.Records;

namespace Schedule_Creator_V2.Services
{
    internal class DatabaseCreate : Database
    {
        public static void CreateSchedule(ScheduleRow row)
        {
            ExecuteNonQuery(
                Queries.CreateSchedule,
                new SqlParameter("@dayOfWeek", row.dayOfWeek.ToString()),
                new SqlParameter("@staffID", row.staffID),
                new SqlParameter("@scheduleName", row.scheduleName));
        }

        public static void CreateJobSettings(JobSettings settings)
        {
            ExecuteNonQuery(
                Queries.CreateJobSettings,
                new SqlParameter("@day", settings.dayOfWeek.ToString()),
                new SqlParameter("@openingTime", settings.openingTime),
                new SqlParameter("@closingTime", settings.closingTime)
                );
        }
        public static void CreateStaff(string fName, string mName, string lName, Positions position, byte[]? profilePicture, string email, bool isBelayCertified)
        {
            ExecuteNonQuery(
                Queries.CreateStaff,
                new SqlParameter("@fName", fName),
                new SqlParameter("@mName", mName),
                new SqlParameter("@lName", lName),
                new SqlParameter("@position", position.ToString()),
                new SqlParameter("@email", email),
                new SqlParameter("@belayCert", isBelayCertified.ToString())
                );
        }
        public static void CreateDaysOff(int id, List<DateOnly> dates, string reason)
        {
            foreach (DateOnly date in dates)
            {
                ExecuteNonQuery(
                Queries.CreateDaysOff,
                new SqlParameter("@id", id),
                new SqlParameter("@date", date),
                new SqlParameter("@reason", reason));
            }
        }

        public static void CreateAvailability( Availability availability)
        {
                ExecuteNonQuery(
                    Queries.CreateAvailability,
                    new SqlParameter("@id", availability.id),
                    new SqlParameter("@dayOfTheWeek", availability.dayOfTheWeek),
                    new SqlParameter("@startTime", availability.startTime),
                    new SqlParameter("@endTime", availability.endTime)
                    );
            
        }
    }
}
