using Microsoft.Data.SqlClient;

using DatabaseBase =
    Schedule_Creator_V2.Services.Database.Database;

namespace Schedule_Creator_V2.Tests.Services.Database
{
    public sealed class DatabaseCreateTestFixture
    {
        // =========================================================
        // CONNECTION STRINGS
        // =========================================================

        private const string MasterConnectionString =
            @"Data Source=(localdb)\MSSQLLocalDB;" +
            @"Initial Catalog=master;" +
            @"Integrated Security=True";

        internal const string TestConnectionString =
            @"Data Source=(localdb)\MSSQLLocalDB;" +
            @"Initial Catalog=Schedule_Creator_V2_Tests;" +
            @"Integrated Security=True";


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public DatabaseCreateTestFixture()
        {
            CreateTestDatabase();

            DatabaseBase.SetConnectionString(
                TestConnectionString);

            CreateSchema();

            CreateTables();
        }


        // =========================================================
        // CREATE DATABASE
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
        // CREATE SCHEMA
        // =========================================================

        private static void CreateSchema()
        {
            using SqlConnection connection =
                new SqlConnection(
                    TestConnectionString);

            connection.Open();

            const string commandText =
                """
                IF SCHEMA_ID('UWGB') IS NULL
                BEGIN
                    EXEC('CREATE SCHEMA [UWGB]');
                END;
                """;

            using SqlCommand command =
                new SqlCommand(
                    commandText,
                    connection);

            command.ExecuteNonQuery();
        }


        // =========================================================
        // CREATE TABLES
        // =========================================================

        private static void CreateTables()
        {
            using SqlConnection connection =
                new SqlConnection(
                    TestConnectionString);

            connection.Open();

            const string commandText =
                """
                IF OBJECT_ID(
                    'UWGB.Availability',
                    'U'
                ) IS NULL
                BEGIN
                    CREATE TABLE [UWGB].[Availability]
                    (
                        [id] INT
                            NOT NULL,

                        [dayOfTheWeek] INT
                            NOT NULL,

                        [startTime] TIME(2)
                            NOT NULL,

                        [endTime] TIME(2)
                            NOT NULL
                    );
                END;


                IF OBJECT_ID(
                    'UWGB.DaysOff',
                    'U'
                ) IS NULL
                BEGIN
                    CREATE TABLE [UWGB].[DaysOff]
                    (
                        [id] INT
                            NOT NULL,

                        [Date] DATE
                            NOT NULL,

                        [reason] NVARCHAR(50)
                            NOT NULL
                    );
                END;


                IF OBJECT_ID(
                    'UWGB.JobSettings',
                    'U'
                ) IS NULL
                BEGIN
                    CREATE TABLE [UWGB].[JobSettings]
                    (
                        [DayOfWeek] NVARCHAR(50)
                            NOT NULL
                            PRIMARY KEY,

                        [OpeningTime] TIME(2)
                            NOT NULL,

                        [ClosingTime] TIME(2)
                            NOT NULL
                    );
                END;


                IF OBJECT_ID(
                    'UWGB.Staff',
                    'U'
                ) IS NULL
                BEGIN
                    CREATE TABLE [UWGB].[Staff]
                    (
                        [id] INT
                            IDENTITY(1, 1)
                            NOT NULL,

                        [fName] NVARCHAR(50)
                            NOT NULL,

                        [mName] NVARCHAR(50)
                            NULL,

                        [lName] NVARCHAR(50)
                            NOT NULL,

                        [position] NVARCHAR(50)
                            NOT NULL,

                        [email] NVARCHAR(50)
                            NOT NULL,

                        [belayCert] NVARCHAR(50)
                            NOT NULL,

                        [certifiedOn] DATE
                            NULL,

                        [expiresOn] DATE
                            NULL,

                        PRIMARY KEY CLUSTERED
                        (
                            [id] ASC
                        )
                    );
                END;


                IF OBJECT_ID(
                    'UWGB.Schedule',
                    'U'
                ) IS NULL
                BEGIN
                    CREATE TABLE [UWGB].[Schedule]
                    (
                        [dayOfWeek] NVARCHAR(50)
                            NOT NULL,

                        [staffID] INT
                            NULL,

                        [scheduleName] NVARCHAR(50)
                            NOT NULL,

                        [startTime] TIME(2)
                            NOT NULL,

                        [endTime] TIME(2)
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