using Microsoft.Data.SqlClient;
using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Enums;
using Schedule_Creator_V2.Models.Records;

namespace Schedule_Creator_V2.Services
{
    internal class DatabaseCreate : Database
    {
        public static void AddSchedule(ScheduleRow row)
        {
            ExecuteNonQuery(
                """
                DELETE FROM
                    [UWGB].[Schedule]
                WHERE
                    scheduleName = @scheduleName
                """,
                new SqlParameter("@scheduleName", row.scheduleName
                ));

            ExecuteNonQuery(
                """
                INSERT INTO
                    [UWGB].[Schedule]
                    (dayOfWeek, staffID, scheduleName)
                VALUES
                    (@dayOfWeek, @staffID, @scheduleName)
                """,
                new SqlParameter("@dayOfWeek", row.dayOfWeek.ToString()),
                new SqlParameter("@staffID", row.staffID),
                new SqlParameter("@scheduleName", row.scheduleName)
                );
        }

        public static void AddJobSettings(JobSettings settings)
        {
            ExecuteNonQuery(
                """
                INSERT INTO
                    [UWGB].[JobSettings]
                    (DayOfWeek, OpeningTime, ClosingTime)
                VALUES
                    (@day, @openingTime, @closingTime)
                """,
                new SqlParameter("@day", settings.dayOfWeek.ToString()),
                new SqlParameter("@openingTime", settings.openingTime),
                new SqlParameter("@closingTime", settings.closingTime)
                );
        }
        public static void AddStaff(string fName, string mName, string lName, Positions position, byte[]? profilePicture, string email, bool isBelayCertified)
        {
            ExecuteNonQuery(
                """
                INSERT INTO 
                    [UWGB].[Staff] 
                    (fName, mName, lName, position, email, belayCert) 
                VALUES 
                    (@fName, @mName, @lName, @position, @email, @belayCert);
                """,
                
                new SqlParameter("@fName", fName),
                new SqlParameter("@mName", mName),
                new SqlParameter("@lName", lName),
                new SqlParameter("@position", position.ToString()),
                new SqlParameter("@email", email),
                new SqlParameter("@belayCert", isBelayCertified.ToString())
                );
        }
        public static void AddDaysOff(int id, List<DateOnly> dates, string reason)
        {
            foreach (DateOnly date in dates)
            {
                ExecuteNonQuery(
                """
                INSERT INTO 
                    [UWGB].[DaysOff] 
                    (id, Date, reason) 
                VALUES 
                    (@id, @date, @reason)
                """,
                new SqlParameter("@id", id),
                new SqlParameter("@date", date),
                new SqlParameter("@reason", reason));
            }
        }
        public static void RemoveDaysOff(int id, List<DateOnly> dates)
        {
            foreach (DateOnly date in dates)
            {
                ExecuteNonQuery(
                    """
                    DELETE FROM 
                        [UWGB].[DaysOff] 
                    WHERE 
                        id = @id AND Date = @date;
                    """,
                    new SqlParameter("@id", id),
                    new SqlParameter("@date", date));
            }
        }

        public static void AddAvailability( Availability availability)
        {
                ExecuteNonQuery(
                    "INSERT INTO [UWGB].[Availability] (id, dayOfTheWeek, startTime, endTime) VALUES (@id, @dayOfTheWeek, @startTime, @endTime)",
                    new SqlParameter("@id", availability.id),
                    new SqlParameter("@dayOfTheWeek", availability.dayOfTheWeek),
                    new SqlParameter("@startTime", availability.startTime),
                    new SqlParameter("@endTime", availability.endTime)
                    );
            
        }

        public static void RemoveAllAvailability(int id)
        {
            ExecuteNonQuery(
                "DELETE FROM [UWGB].[Availability] WHERE id = @id",
                new SqlParameter("@id", id)
                );
        }

        public static void RemoveJobSetting(DayOfWeek day)
        {
            ExecuteNonQuery(
                "DELETE FROM [UWGB].[JobSettings] WHERE DayOfWeek = @day",
                new SqlParameter("@day", day.ToString())
                );
        }

        public static void RemoveJobSettings()
        {
            ExecuteNonQuery("DELETE FROM [UWGB].[JobSettings]");
        }

        public static void RemoveAllFromAll(int id)
        {
            ExecuteNonQuery(
            """
            DELETE FROM [UWGB].[Staff] WHERE id = @id;
            DELETE FROM [UWGB].[Availability] WHERE id = @id;
            DELETE FROM [UWGB].[DaysOff] WHERE id = @id;
            DELETE FROM [UWGB].[JobSettings];
            """,
            new SqlParameter("@id", id));
        }
    }
}
