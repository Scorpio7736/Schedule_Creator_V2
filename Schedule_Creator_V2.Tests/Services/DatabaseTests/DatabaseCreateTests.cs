using Microsoft.Data.SqlClient;
using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Enums;
using Schedule_Creator_V2.Models.Records;
using System.Data;

using DatabaseBase =
    Schedule_Creator_V2.Services.Database.Database;

using DatabaseCreateService =
    Schedule_Creator_V2.Services.Database.DatabaseCreate;

namespace Schedule_Creator_V2.Tests.Services.Database
{
    public sealed class DatabaseCreateTests :
        IClassFixture<DatabaseCreateTestFixture>
    {
        public DatabaseCreateTests(
            DatabaseCreateTestFixture fixture)
        {
            /*
             * The fixture creates the isolated LocalDB database,
             * UWGB schema, and required tables.
             */
        }


        // =========================================================
        // CREATE SCHEDULE
        // =========================================================

        [Fact]
        public void CreateSchedule_ValidRow_InsertsSchedule()
        {
            // Arrange

            string scheduleName =
                $"Test_{Guid.NewGuid():N}";

            ScheduleRow row =
                new ScheduleRow(
                    DayOfWeek.Monday,
                    123,
                    new TimeOnly(
                        14,
                        45),
                    new TimeOnly(
                        18,
                        15),
                    scheduleName);


            // Act

            DatabaseCreateService.CreateSchedule(
                row);


            // Assert

            const string selectSql =
                """
                SELECT
                    dayOfWeek,
                    staffID,
                    scheduleName,
                    startTime,
                    endTime
                FROM [UWGB].[Schedule]
                WHERE scheduleName = @scheduleName;
                """;

            using SqlDataReader reader =
                DatabaseBase.ExecuteReader(
                    selectSql,

                    new SqlParameter(
                        "@scheduleName",
                        scheduleName));

            Assert.True(
                reader.Read());

            Assert.Equal(
                DayOfWeek.Monday.ToString(),
                reader["dayOfWeek"]);

            Assert.Equal(
                123,
                reader["staffID"]);

            Assert.Equal(
                scheduleName,
                reader["scheduleName"]);

            Assert.Equal(
                new TimeSpan(
                    14,
                    45,
                    0),
                reader["startTime"]);

            Assert.Equal(
                new TimeSpan(
                    18,
                    15,
                    0),
                reader["endTime"]);
        }


        [Fact]
        public void CreateSchedule_NullStaffId_InsertsDatabaseNull()
        {
            // Arrange

            string scheduleName =
                $"Test_{Guid.NewGuid():N}";

            ScheduleRow row =
                new ScheduleRow(
                    DayOfWeek.Tuesday,
                    null,
                    new TimeOnly(
                        15,
                        0),
                    new TimeOnly(
                        19,
                        0),
                    scheduleName);


            // Act

            DatabaseCreateService.CreateSchedule(
                row);


            // Assert

            const string selectSql =
                """
                SELECT staffID
                FROM [UWGB].[Schedule]
                WHERE scheduleName = @scheduleName;
                """;

            using SqlDataReader reader =
                DatabaseBase.ExecuteReader(
                    selectSql,

                    new SqlParameter(
                        "@scheduleName",
                        scheduleName));

            Assert.True(
                reader.Read());

            Assert.Equal(
                DBNull.Value,
                reader["staffID"]);
        }


        // =========================================================
        // CREATE JOB SETTINGS
        // =========================================================

        [Fact]
        public void CreateJobSettings_ValidSettings_InsertsRow()
        {
            // Arrange

            const DayOfWeek day =
                DayOfWeek.Sunday;

            DeleteJobSettings(
                day);

            JobSettings settings =
                new JobSettings(
                    day,
                    new TimeOnly(
                        10,
                        30),
                    new TimeOnly(
                        20,
                        15));


            // Act

            DatabaseCreateService.CreateJobSettings(
                settings);


            // Assert

            const string selectSql =
                """
                SELECT
                    DayOfWeek,
                    OpeningTime,
                    ClosingTime
                FROM [UWGB].[JobSettings]
                WHERE DayOfWeek = @day;
                """;

            using SqlDataReader reader =
                DatabaseBase.ExecuteReader(
                    selectSql,

                    new SqlParameter(
                        "@day",
                        day.ToString()));

            Assert.True(
                reader.Read());

            Assert.Equal(
                day.ToString(),
                reader["DayOfWeek"]);

            Assert.Equal(
                new TimeSpan(
                    10,
                    30,
                    0),
                reader["OpeningTime"]);

            Assert.Equal(
                new TimeSpan(
                    20,
                    15,
                    0),
                reader["ClosingTime"]);
        }


        // =========================================================
        // CREATE STAFF
        // =========================================================

        [Fact]
        public void CreateStaff_ValidStaff_InsertsCorrectValues()
        {
            // Arrange

            string email =
                $"{Guid.NewGuid():N}@test.local";

            const string firstName =
                "Unit";

            const string middleName =
                "T";

            const string lastName =
                "Tester";

            const Positions position =
                Positions.Attendant;

            const bool isBelayCertified =
                true;


            // Act

            DatabaseCreateService.CreateStaff(
                firstName,
                middleName,
                lastName,
                position,
                null,
                email,
                isBelayCertified);


            // Assert

            const string selectSql =
                """
                SELECT
                    fName,
                    mName,
                    lName,
                    position,
                    email,
                    belayCert
                FROM [UWGB].[Staff]
                WHERE email = @email;
                """;

            using SqlDataReader reader =
                DatabaseBase.ExecuteReader(
                    selectSql,

                    new SqlParameter(
                        "@email",
                        email));

            Assert.True(
                reader.Read());

            Assert.Equal(
                firstName,
                reader["fName"]);

            Assert.Equal(
                middleName,
                reader["mName"]);

            Assert.Equal(
                lastName,
                reader["lName"]);

            Assert.Equal(
                position.ToString(),
                reader["position"]);

            Assert.Equal(
                email,
                reader["email"]);

            Assert.Equal(
                isBelayCertified.ToString(),
                reader["belayCert"]);
        }


        // =========================================================
        // CREATE DAYS OFF
        // =========================================================

        [Fact]
        public void CreateDaysOff_MultipleDates_InsertsAllDates()
        {
            // Arrange

            int staffId =
                Random.Shared.Next(
                    100000,
                    999999);

            string reason =
                $"Test_{Guid.NewGuid():N}"
                    .Substring(
                        0,
                        30);

            List<DateOnly> dates =
                new List<DateOnly>
                {
                    new DateOnly(
                        2030,
                        1,
                        10),

                    new DateOnly(
                        2030,
                        1,
                        11),

                    new DateOnly(
                        2030,
                        1,
                        12)
                };


            // Act

            DatabaseCreateService.CreateDaysOff(
                staffId,
                dates,
                reason);


            // Assert

            const string countSql =
                """
                SELECT COUNT(*)
                FROM [UWGB].[DaysOff]
                WHERE id = @id
                    AND reason = @reason;
                """;

            int count =
                DatabaseBase.ExecuteScalar<int>(
                    countSql,

                    new SqlParameter(
                        "@id",
                        SqlDbType.Int)
                    {
                        Value =
                            staffId
                    },

                    new SqlParameter(
                        "@reason",
                        reason));

            Assert.Equal(
                dates.Count,
                count);
        }


        // =========================================================
        // CREATE AVAILABILITY
        // =========================================================

        [Fact]
        public void CreateAvailability_ValidAvailability_InsertsRow()
        {
            // Arrange

            int staffId =
                Random.Shared.Next(
                    100000,
                    999999);

            Availability availability =
                new Availability(
                    staffId,
                    DayOfWeek.Wednesday,
                    new TimeOnly(
                        14,
                        30),
                    new TimeOnly(
                        20,
                        0));


            // Act

            DatabaseCreateService.CreateAvailability(
                availability);


            // Assert

            const string selectSql =
                """
                SELECT
                    id,
                    dayOfTheWeek,
                    startTime,
                    endTime
                FROM [UWGB].[Availability]
                WHERE id = @id;
                """;

            using SqlDataReader reader =
                DatabaseBase.ExecuteReader(
                    selectSql,

                    new SqlParameter(
                        "@id",
                        staffId));

            Assert.True(
                reader.Read());

            Assert.Equal(
                staffId,
                reader["id"]);

            Assert.Equal(
                (int)DayOfWeek.Wednesday,
                reader["dayOfTheWeek"]);

            Assert.Equal(
                new TimeSpan(
                    14,
                    30,
                    0),
                reader["startTime"]);

            Assert.Equal(
                new TimeSpan(
                    20,
                    0,
                    0),
                reader["endTime"]);
        }


        // =========================================================
        // HELPERS
        // =========================================================

        private static void DeleteJobSettings(
            DayOfWeek day)
        {
            const string deleteSql =
                """
                DELETE FROM [UWGB].[JobSettings]
                WHERE DayOfWeek = @day;
                """;

            DatabaseBase.ExecuteNonQuery(
                deleteSql,

                new SqlParameter(
                    "@day",
                    day.ToString()));
        }
    }
}