using Microsoft.Data.SqlClient;

namespace Schedule_Creator_V2.Services
{
    internal class DatabaseDelete : Database
    {

        public static void DeleteDaysOff(int id, List<DateOnly> dates)
        {
            foreach (DateOnly date in dates)
            {
                ExecuteNonQuery(
                    Queries.DeleteDaysOff,
                    new SqlParameter("@id", id),
                    new SqlParameter("@date", date));
            }
        }

        public static void DeleteAllAvailability(int id)
        {
            ExecuteNonQuery(
                Queries.DeleteAllAvailability,
                new SqlParameter("@id", id)
                );
        }

        public static void DeleteJobSettingsOnDay(DayOfWeek dayOfWeek)
        {
            ExecuteNonQuery(
                Queries.DeleteJobSettingsOnDay,
                new SqlParameter("@DayOfWeek", dayOfWeek.ToString())
                );
        }

        public static void DeleteAllJobSettings()
        {
            ExecuteNonQuery(Queries.DeleteAllJobSettings);
        }

        public static void DeleteAllByID(int id)
        {
            /* ERROR THAT NEEDS TO BE FIXED LATER:
             *  Delete any schedule line with the user id saved or else it will crash the page.
             */
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