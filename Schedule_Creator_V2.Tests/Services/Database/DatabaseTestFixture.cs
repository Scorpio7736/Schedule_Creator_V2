using Microsoft.Data.SqlClient;

using DatabaseBase =
    Schedule_Creator_V2.Services.Database.Database;

namespace Schedule_Creator_V2.Tests.Services.Database
{
    public sealed class DatabaseTestFixture
    {
        // =========================================================
        // CONNECTION STRINGS
        // =========================================================

        private const string MasterConnectionString =
            @"Data Source=(localdb)\MSSQLLocalDB;" +
            @"Initial Catalog=master;" +
            @"Integrated Security=True";

        private const string TestConnectionString =
            @"Data Source=(localdb)\MSSQLLocalDB;" +
            @"Initial Catalog=Schedule_Creator_V2_Tests;" +
            @"Integrated Security=True";


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public DatabaseTestFixture()
        {
            CreateTestDatabase();

            DatabaseBase.SetConnectionString(
                TestConnectionString);

            CreateTestTable();
        }


        // =========================================================
        // CREATE TEST DATABASE
        // =========================================================

        private static void CreateTestDatabase()
        {
            using SqlConnection connection =
                new SqlConnection(
                    MasterConnectionString);

            connection.Open();

            const string commandText =
                """
                IF DB_ID('Schedule_Creator_V2_Tests') IS NULL
                BEGIN
                    CREATE DATABASE [Schedule_Creator_V2_Tests];
                END;
                """;

            using SqlCommand command =
                new SqlCommand(
                    commandText,
                    connection);

            command.ExecuteNonQuery();
        }


        // =========================================================
        // CREATE TEST TABLE
        // =========================================================

        private static void CreateTestTable()
        {
            using SqlConnection connection =
                new SqlConnection(
                    TestConnectionString);

            connection.Open();

            const string commandText =
                """
                IF OBJECT_ID(
                    'dbo.DatabaseTests',
                    'U'
                ) IS NULL
                BEGIN
                    CREATE TABLE dbo.DatabaseTests
                    (
                        Id INT IDENTITY(1, 1)
                            NOT NULL
                            PRIMARY KEY,

                        TestGroup UNIQUEIDENTIFIER
                            NOT NULL,

                        TextValue NVARCHAR(100)
                            NOT NULL,

                        NumberValue INT
                            NOT NULL
                    );
                END;
                """;

            using SqlCommand command =
                new SqlCommand(
                    commandText,
                    connection);

            command.ExecuteNonQuery();
        }
    }
}