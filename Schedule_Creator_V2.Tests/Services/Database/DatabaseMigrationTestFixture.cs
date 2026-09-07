using Microsoft.Data.SqlClient;

namespace Schedule_Creator_V2.Tests.Services.Database
{
    public sealed class DatabaseMigrationTestFixture :
        IDisposable
    {
        // =========================================================
        // DATABASE
        // =========================================================

        internal const string DatabaseName =
            "Schedule_Creator_V2_MigrationTests";

        private const string MasterConnectionString =
            @"Data Source=(localdb)\MSSQLLocalDB;" +
            @"Initial Catalog=master;" +
            @"Integrated Security=True";

        internal const string TestConnectionString =
            @"Data Source=(localdb)\MSSQLLocalDB;" +
            @"Initial Catalog=Schedule_Creator_V2_MigrationTests;" +
            @"Integrated Security=True";


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public DatabaseMigrationTestFixture()
        {
            RecreateTestDatabase();

            ApplyMigration();
        }


        // =========================================================
        // RECREATE DATABASE
        // =========================================================

        private static void RecreateTestDatabase()
        {
            using SqlConnection connection =
                new SqlConnection(
                    MasterConnectionString);

            connection.Open();

            const string commandText =
                """
                IF DB_ID('Schedule_Creator_V2_MigrationTests') IS NOT NULL
                BEGIN
                    ALTER DATABASE
                        [Schedule_Creator_V2_MigrationTests]
                    SET SINGLE_USER
                    WITH ROLLBACK IMMEDIATE;

                    DROP DATABASE
                        [Schedule_Creator_V2_MigrationTests];
                END;

                CREATE DATABASE
                    [Schedule_Creator_V2_MigrationTests];
                """;

            using SqlCommand command =
                new SqlCommand(
                    commandText,
                    connection);

            command.ExecuteNonQuery();
        }


        // =========================================================
        // APPLY MIGRATION
        // =========================================================

        private static void ApplyMigration()
        {
            using SqlConnection connection =
                new SqlConnection(
                    TestConnectionString);

            connection.Open();

            ExecuteScript(
                connection,
                Queries.CreateSchemas);

            ExecuteScript(
                connection,
                Queries.CreateTables);
        }


        private static void ExecuteScript(
            SqlConnection connection,
            string commandText)
        {
            using SqlCommand command =
                new SqlCommand(
                    commandText,
                    connection);

            command.ExecuteNonQuery();
        }


        // =========================================================
        // OPEN TEST CONNECTION
        // =========================================================

        internal static SqlConnection OpenConnection()
        {
            SqlConnection connection =
                new SqlConnection(
                    TestConnectionString);

            connection.Open();

            return connection;
        }


        // =========================================================
        // CLEANUP
        // =========================================================

        public void Dispose()
        {
            SqlConnection.ClearAllPools();

            using SqlConnection connection =
                new SqlConnection(
                    MasterConnectionString);

            connection.Open();

            const string commandText =
                """
                IF DB_ID('Schedule_Creator_V2_MigrationTests') IS NOT NULL
                BEGIN
                    ALTER DATABASE
                        [Schedule_Creator_V2_MigrationTests]
                    SET SINGLE_USER
                    WITH ROLLBACK IMMEDIATE;

                    DROP DATABASE
                        [Schedule_Creator_V2_MigrationTests];
                END;
                """;

            using SqlCommand command =
                new SqlCommand(
                    commandText,
                    connection);

            command.ExecuteNonQuery();

            GC.SuppressFinalize(
                this);
        }
    }
}