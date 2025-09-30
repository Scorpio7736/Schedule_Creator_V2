using Microsoft.Data.SqlClient;
using Schedule_Creator_V2.Models;

namespace Schedule_Creator_V2.Services
{
    internal class DatabaseCreate : Database
    {
        public static void AddJobSettings(DayOfWeek day, TimeOnly openingTime, TimeOnly closingTime)
        {
            ExecuteNonQuery(
                """
                INSERT INTO
                    [UWGB].[JobSettings]
                    (DayOfWeek, OpeningTime, ClosingTime)
                VALUES
                    (@day, @openingTime, @closingTime)
                """,
                new SqlParameter("@day", day),
                new SqlParameter("@openingTime", openingTime),
                new SqlParameter("@closingTime", closingTime)
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

        

        public static void AddAvailability(int id, AvailDays day, string availTimes)
        {
                ExecuteNonQuery(
                    "INSERT INTO [UWGB].[Availability] (id, dayOfTheWeek, availTimes) VALUES (@id, @dayOfTheWeek, @availTimes)",
                    new SqlParameter("@id", id),
                    new SqlParameter("@dayOfTheWeek", day.ToString()),
                    new SqlParameter("@availTimes", availTimes)
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
                new SqlParameter("@day", day)
                );
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
