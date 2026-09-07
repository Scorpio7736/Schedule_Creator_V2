using Microsoft.Data.SqlClient;

namespace Schedule_Creator_V2.Services.Database
{
    internal class DatabaseDelete : Database
    {
        // =========================================================
        // DELETE DAYS OFF
        // =========================================================

        public static void DeleteDaysOff(
            int id,
            List<DateOnly> dates)
        {
            foreach (DateOnly date in dates)
            {
                ExecuteNonQuery(
                    Queries.DeleteDaysOff,

                    new SqlParameter(
                        "@id",
                        id),

                    new SqlParameter(
                        "@date",
                        date));
            }
        }


        // =========================================================
        // DELETE AVAILABILITY
        // =========================================================

        public static void DeleteAllAvailability(
            int id)
        {
            ExecuteNonQuery(
                Queries.DeleteAllAvailability,

                new SqlParameter(
                    "@id",
                    id));
        }


        // =========================================================
        // DELETE JOB SETTINGS
        // =========================================================

        public static void DeleteJobSettingsOnDay(
            DayOfWeek dayOfWeek)
        {
            ExecuteNonQuery(
                Queries.DeleteJobSettingsOnDay,

                new SqlParameter(
                    "@DayOfWeek",
                    dayOfWeek.ToString()));
        }


        public static void DeleteAllJobSettings()
        {
            ExecuteNonQuery(
                Queries.DeleteAllJobSettings);
        }


        // =========================================================
        // DELETE STAFF AND RELATED DATA
        // =========================================================

        public static void DeleteAllByID(
            int id)
        {
            /*
             * Delete all records that belong to the staff member.
             *
             * Job settings are intentionally NOT deleted because
             * they are application-wide settings and are not tied
             * to an individual staff member.
             */

            ExecuteNonQuery(
                """
                DELETE FROM [UWGB].[Schedule]
                WHERE staffID = @id;

                DELETE FROM [UWGB].[Availability]
                WHERE id = @id;

                DELETE FROM [UWGB].[DaysOff]
                WHERE id = @id;

                DELETE FROM [UWGB].[Staff]
                WHERE id = @id;
                """,

                new SqlParameter(
                    "@id",
                    id));
        }
    }
}