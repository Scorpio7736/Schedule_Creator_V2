using Microsoft.Data.SqlClient;

namespace Schedule_Creator_V2.Tests.Services.Database
{
    public sealed class DatabaseMigrationTests :
        IClassFixture<DatabaseMigrationTestFixture>
    {
        public DatabaseMigrationTests(
            DatabaseMigrationTestFixture fixture)
        {
            /*
             * The fixture creates a completely separate LocalDB
             * database and applies the application's schema and
             * table migration resources before these tests run.
             */
        }


        // =========================================================
        // SCHEMA
        // =========================================================

        [Fact]
        public void CreateSchemas_CreatesUwgbSchema()
        {
            // Arrange

            const string commandText =
                """
                SELECT COUNT(*)
                FROM sys.schemas
                WHERE name = 'UWGB';
                """;


            // Act

            int schemaCount =
                ExecuteScalar<int>(
                    commandText);


            // Assert

            Assert.Equal(
                1,
                schemaCount);
        }


        // =========================================================
        // TABLES
        // =========================================================

        [Fact]
        public void CreateTables_CreatesExpectedTables()
        {
            // Arrange

            string[] expectedTables =
            {
                "Availability",
                "DaysOff",
                "JobSettings",
                "Schedule",
                "Staff"
            };

            const string commandText =
                """
                SELECT
                    table_name
                FROM information_schema.tables
                WHERE table_schema = 'UWGB'
                ORDER BY table_name;
                """;


            // Act

            List<string> actualTables =
                new();

            using SqlConnection connection =
                DatabaseMigrationTestFixture
                    .OpenConnection();

            using SqlCommand command =
                new SqlCommand(
                    commandText,
                    connection);

            using SqlDataReader reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                actualTables.Add(
                    reader.GetString(
                        0));
            }


            // Assert

            Assert.Equal(
                expectedTables,
                actualTables);
        }


        // =========================================================
        // AVAILABILITY
        // =========================================================

        [Fact]
        public void CreateTables_Availability_HasExpectedColumns()
        {
            // Arrange

            string[] expectedColumns =
            {
                "id",
                "dayOfTheWeek",
                "startTime",
                "endTime"
            };


            // Act

            List<string> actualColumns =
                GetColumnNames(
                    "Availability");


            // Assert

            Assert.Equal(
                expectedColumns,
                actualColumns);
        }


        // =========================================================
        // DAYS OFF
        // =========================================================

        [Fact]
        public void CreateTables_DaysOff_HasExpectedColumns()
        {
            // Arrange

            string[] expectedColumns =
            {
                "id",
                "Date",
                "reason"
            };


            // Act

            List<string> actualColumns =
                GetColumnNames(
                    "DaysOff");


            // Assert

            Assert.Equal(
                expectedColumns,
                actualColumns);
        }


        // =========================================================
        // JOB SETTINGS
        // =========================================================

        [Fact]
        public void CreateTables_JobSettings_HasExpectedColumns()
        {
            // Arrange

            string[] expectedColumns =
            {
                "DayOfWeek",
                "OpeningTime",
                "ClosingTime"
            };


            // Act

            List<string> actualColumns =
                GetColumnNames(
                    "JobSettings");


            // Assert

            Assert.Equal(
                expectedColumns,
                actualColumns);
        }


        [Fact]
        public void CreateTables_JobSettings_DayOfWeekIsPrimaryKey()
        {
            // Arrange

            const string commandText =
                """
                SELECT COUNT(*)
                FROM sys.indexes i
                INNER JOIN sys.index_columns ic
                    ON i.object_id = ic.object_id
                    AND i.index_id = ic.index_id
                INNER JOIN sys.columns c
                    ON ic.object_id = c.object_id
                    AND ic.column_id = c.column_id
                INNER JOIN sys.tables t
                    ON i.object_id = t.object_id
                INNER JOIN sys.schemas s
                    ON t.schema_id = s.schema_id
                WHERE
                    s.name = 'UWGB'
                    AND t.name = 'JobSettings'
                    AND c.name = 'DayOfWeek'
                    AND i.is_primary_key = 1;
                """;


            // Act

            int primaryKeyCount =
                ExecuteScalar<int>(
                    commandText);


            // Assert

            Assert.Equal(
                1,
                primaryKeyCount);
        }


        // =========================================================
        // STAFF
        // =========================================================

        [Fact]
        public void CreateTables_Staff_HasExpectedColumns()
        {
            // Arrange

            string[] expectedColumns =
            {
                "id",
                "fName",
                "mName",
                "lName",
                "position",
                "email",
                "belayCert",
                "certifiedOn",
                "expiresOn"
            };


            // Act

            List<string> actualColumns =
                GetColumnNames(
                    "Staff");


            // Assert

            Assert.Equal(
                expectedColumns,
                actualColumns);
        }


        [Fact]
        public void CreateTables_Staff_IdIsIdentity()
        {
            // Arrange

            const string commandText =
                """
                SELECT COLUMNPROPERTY(
                    OBJECT_ID('UWGB.Staff'),
                    'id',
                    'IsIdentity'
                );
                """;


            // Act

            int isIdentity =
                ExecuteScalar<int>(
                    commandText);


            // Assert

            Assert.Equal(
                1,
                isIdentity);
        }


        // =========================================================
        // SCHEDULE
        // =========================================================

        [Fact]
        public void CreateTables_Schedule_HasExpectedColumns()
        {
            // Arrange

            string[] expectedColumns =
            {
                "dayOfWeek",
                "staffID",
                "scheduleName",
                "startTime",
                "endTime"
            };


            // Act

            List<string> actualColumns =
                GetColumnNames(
                    "Schedule");


            // Assert

            Assert.Equal(
                expectedColumns,
                actualColumns);
        }


        [Fact]
        public void CreateTables_Schedule_StaffIdAllowsNull()
        {
            // Arrange

            const string commandText =
                """
                SELECT
                    IS_NULLABLE
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE
                    TABLE_SCHEMA = 'UWGB'
                    AND TABLE_NAME = 'Schedule'
                    AND COLUMN_NAME = 'staffID';
                """;


            // Act

            string isNullable =
                ExecuteScalar<string>(
                    commandText);


            // Assert

            Assert.Equal(
                "YES",
                isNullable);
        }


        // =========================================================
        // DATA TYPES
        // =========================================================

        [Theory]
        [InlineData(
            "Availability",
            "dayOfTheWeek",
            "int")]
        [InlineData(
            "Availability",
            "startTime",
            "time")]
        [InlineData(
            "Availability",
            "endTime",
            "time")]
        [InlineData(
            "DaysOff",
            "Date",
            "date")]
        [InlineData(
            "JobSettings",
            "DayOfWeek",
            "nvarchar")]
        [InlineData(
            "Schedule",
            "dayOfWeek",
            "nvarchar")]
        [InlineData(
            "Schedule",
            "staffID",
            "int")]
        [InlineData(
            "Schedule",
            "scheduleName",
            "nvarchar")]
        [InlineData(
            "Staff",
            "id",
            "int")]
        [InlineData(
            "Staff",
            "position",
            "nvarchar")]
        public void CreateTables_Column_HasExpectedDataType(
            string tableName,
            string columnName,
            string expectedType)
        {
            // Arrange

            const string commandText =
                """
                SELECT
                    DATA_TYPE
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE
                    TABLE_SCHEMA = 'UWGB'
                    AND TABLE_NAME = @tableName
                    AND COLUMN_NAME = @columnName;
                """;


            // Act

            string actualType;

            using (
                SqlConnection connection =
                    DatabaseMigrationTestFixture
                        .OpenConnection())
            {
                using SqlCommand command =
                    new SqlCommand(
                        commandText,
                        connection);

                command.Parameters.AddWithValue(
                    "@tableName",
                    tableName);

                command.Parameters.AddWithValue(
                    "@columnName",
                    columnName);

                actualType =
                    (string)command.ExecuteScalar();
            }


            // Assert

            Assert.Equal(
                expectedType,
                actualType);
        }


        // =========================================================
        // HELPERS
        // =========================================================

        private static List<string> GetColumnNames(
            string tableName)
        {
            const string commandText =
                """
                SELECT
                    COLUMN_NAME
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE
                    TABLE_SCHEMA = 'UWGB'
                    AND TABLE_NAME = @tableName
                ORDER BY ORDINAL_POSITION;
                """;

            List<string> columns =
                new();

            using SqlConnection connection =
                DatabaseMigrationTestFixture
                    .OpenConnection();

            using SqlCommand command =
                new SqlCommand(
                    commandText,
                    connection);

            command.Parameters.AddWithValue(
                "@tableName",
                tableName);

            using SqlDataReader reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                columns.Add(
                    reader.GetString(
                        0));
            }

            return columns;
        }


        private static T ExecuteScalar<T>(
            string commandText)
        {
            using SqlConnection connection =
                DatabaseMigrationTestFixture
                    .OpenConnection();

            using SqlCommand command =
                new SqlCommand(
                    commandText,
                    connection);

            object? result =
                command.ExecuteScalar();

            if (result is null ||
                result == DBNull.Value)
            {
                throw new InvalidOperationException(
                    "The migration test query " +
                    "did not return a value.");
            }

            return (T)result;
        }
    }
}