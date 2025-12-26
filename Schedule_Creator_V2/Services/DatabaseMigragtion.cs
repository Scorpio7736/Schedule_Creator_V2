using Microsoft.Data.SqlClient;
using Schedule_Creator_V2.Services;

namespace Schedule_Creator_V2
{
    internal class DatabaseMigragtion : Database
    {
        public static void TestDatabaseExistence()
        {
            List<string> dbNames = new List<string> 
            { 
                "Availability",
                "DaysOff",
                "JobSettings",
                "Schedule",
                "Staff"
            };

            foreach (string databaseName in dbNames)
            {
                if ( ! DatabaseExists(databaseName))
                {
                    CreateDatabaseAndTables();
                }
            }

        }

        private static void CreateDatabaseAndTables()
        {

        }

        public static bool DatabaseExists(string databaseName)
        {
            int exists = ExecuteScalar<int>(
                @"""
                SELECT
                    CASE
                        WHEN DB_ID(@dbName) IS NOT NULL THEN 1
                        ELSE 0
                    END
                """,
                new SqlParameter("@dbName", databaseName)
            );

            return exists == 1;
        }
    }
}
