using Microsoft.Data.SqlClient;
using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Enums;
using Schedule_Creator_V2.Models.Records;
using System.Data;

using DatabaseBase =
    Schedule_Creator_V2.Services.Database.Database;

using DatabaseReadService =
    Schedule_Creator_V2.Services.Database.DatabaseRead;

namespace Schedule_Creator_V2.Tests.Services.Database
{
    public sealed class DatabaseReadTests :
        IClassFixture<DatabaseCreateTestFixture>
    {
        public DatabaseReadTests(
            DatabaseCreateTestFixture fixture)
        {
            /*
             * DatabaseCreateTestFixture creates the isolated
             * LocalDB test database, UWGB schema, and tables.
             *
             * Each test creates its own identifiable test data.
             */
        }


        // =========================================================
        // READ STAFF
        // =========================================================

        [Fact]
        public void ReadStaff_ExistingStaff_ReturnsStaff()
        {
            // Arrange

            string email =
                CreateUniqueEmail();

            int staffId =
                InsertStaff(
                    "Read",
                    "A",
                    "Tester",
                    Positions.Attendant,
                    email,
                    true);


            // Act

            List<Staff> staff =
                DatabaseReadService.ReadStaff();


            // Assert

            Staff? result =
                staff.FirstOrDefault(
                    item =>
                        item.id == staffId);

            Assert.NotNull(
                result);

            Assert.Equal(
                "Read",
                result.fName);

            Assert.Equal(
                "A",
                result.mName);

            Assert.Equal(
                "Tester",
                result.lName);

            Assert.Equal(
                Positions.Attendant,
                result.position);

            Assert.Equal(
                email,
                result.email);

            Assert.True(
                result.isBelayCertified);
        }


        // =========================================================
        // READ STAFF BY ID
        // =========================================================

        [Fact]
        public void ReadStaffByID_ExistingId_ReturnsCorrectStaff()
        {
            // Arrange

            string email =
                CreateUniqueEmail();

            int staffId =
                InsertStaff(
                    "Specific",
                    "B",
                    "Person",
                    Positions.Shift_Lead,
                    email,
                    false);


            // Act

            Staff result =
                DatabaseReadService.ReadStaffByID(
                    staffId);


            // Assert

            Assert.Equal(
                staffId,
                result.id);

            Assert.Equal(
                "Specific",
                result.fName);

            Assert.Equal(
                "B",
                result.mName);

            Assert.Equal(
                "Person",
                result.lName);

            Assert.Equal(
                Positions.Shift_Lead,
                result.position);

            Assert.Equal(
                email,
                result.email);

            Assert.False(
                result.isBelayCertified);
        }


        [Fact]
        public void ReadStaffByID_NonExistingId_ReturnsUnknownStaff()
        {
            // Arrange

            const int missingId =
                -999999;


            // Act

            Staff result =
                DatabaseReadService.ReadStaffByID(
                    missingId);


            // Assert

            Assert.Equal(
                Positions.Unknown,
                result.position);

            Assert.Equal(
                string.Empty,
                result.fName);

            Assert.Equal(
                string.Empty,
                result.lName);
        }


        // =========================================================
        // READ STAFF WITH NO AVAILABILITY
        // =========================================================

        [Fact]
        public void ReadStaffWithNoAvail_StaffWithoutAvailability_ReturnsStaff()
        {
            // Arrange

            int staffId =
                InsertStaff(
                    "No",
                    "A",
                    "Availability",
                    Positions.Attendant,
                    CreateUniqueEmail(),
                    false);


            // Act

            List<Staff> staff =
                DatabaseReadService
                    .ReadStaffWithNoAvail();


            // Assert

            Assert.Contains(
                staff,
                item =>
                    item.id == staffId);
        }


        [Fact]
        public void ReadStaffWithNoAvail_StaffWithAvailability_DoesNotReturnStaff()
        {
            // Arrange

            int staffId =
                InsertStaff(
                    "Has",
                    "A",
                    "Availability",
                    Positions.Attendant,
                    CreateUniqueEmail(),
                    false);

            InsertAvailability(
                staffId,
                DayOfWeek.Monday,
                new TimeOnly(
                    14,
                    0),
                new TimeOnly(
                    18,
                    0));


            // Act

            List<Staff> staff =
                DatabaseReadService
                    .ReadStaffWithNoAvail();


            // Assert

            Assert.DoesNotContain(
                staff,
                item =>
                    item.id == staffId);
        }


        // =========================================================
        // READ JOB SETTINGS DAYS
        // =========================================================

        [Fact]
        public void ReadJobSettingsDays_ExistingDay_ReturnsDay()
        {
            // Arrange

            DayOfWeek day =
                DayOfWeek.Friday;

            DeleteJobSetting(
                day);

            InsertJobSetting(
                day,
                new TimeOnly(
                    12,
                    0),
                new TimeOnly(
                    20,
                    0));


            // Act

            List<DayOfWeek> days =
                DatabaseReadService
                    .ReadJobSettingsDays();


            // Assert

            Assert.Contains(
                day,
                days);
        }


        // =========================================================
        // READ JOB SETTINGS
        // =========================================================

        [Fact]
        public void ReadJobSettings_ExistingSetting_ReturnsCorrectValues()
        {
            // Arrange

            DayOfWeek day =
                DayOfWeek.Saturday;

            DeleteJobSetting(
                day);

            TimeOnly openingTime =
                new TimeOnly(
                    9,
                    30);

            TimeOnly closingTime =
                new TimeOnly(
                    17,
                    45);

            InsertJobSetting(
                day,
                openingTime,
                closingTime);


            // Act

            List<JobSettings> settings =
                DatabaseReadService
                    .ReadJobSettings();


            // Assert

            JobSettings? result =
                settings.FirstOrDefault(
                    item =>
                        item.dayOfWeek ==
                        day);

            Assert.NotNull(
                result);

            Assert.Equal(
                openingTime,
                result.openingTime);

            Assert.Equal(
                closingTime,
                result.closingTime);
        }


        // =========================================================
        // READ SCHEDULE BY NAME
        // =========================================================

        [Fact]
        public void ReadScheduleByScheduleName_ExistingSchedule_ReturnsRows()
        {
            // Arrange

            string scheduleName =
                $"Read_{Guid.NewGuid():N}";

            InsertSchedule(
                DayOfWeek.Monday,
                null,
                new TimeOnly(
                    14,
                    45),
                new TimeOnly(
                    18,
                    15),
                scheduleName);

            InsertSchedule(
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

            List<ScheduleRow> rows =
                DatabaseReadService
                    .ReadScheduleByScheduleName(
                        scheduleName);


            // Assert

            Assert.Equal(
                2,
                rows.Count);

            Assert.All(
                rows,
                row =>
                    Assert.Equal(
                        scheduleName,
                        row.scheduleName));

            Assert.Contains(
                rows,
                row =>
                    row.dayOfWeek ==
                    DayOfWeek.Monday);

            Assert.Contains(
                rows,
                row =>
                    row.dayOfWeek ==
                    DayOfWeek.Tuesday);
        }


        [Fact]
        public void ReadScheduleByScheduleName_NonExistingSchedule_ReturnsEmptyList()
        {
            // Arrange

            string scheduleName =
                $"Missing_{Guid.NewGuid():N}";


            // Act

            List<ScheduleRow> rows =
                DatabaseReadService
                    .ReadScheduleByScheduleName(
                        scheduleName);


            // Assert

            Assert.Empty(
                rows);
        }


        // =========================================================
        // READ ALL SCHEDULE NAMES
        // =========================================================

        [Fact]
        public void ReadAllScheduleNames_ExistingSchedule_ReturnsScheduleName()
        {
            // Arrange

            string scheduleName =
                $"Names_{Guid.NewGuid():N}";

            InsertSchedule(
                DayOfWeek.Wednesday,
                null,
                new TimeOnly(
                    14,
                    0),
                new TimeOnly(
                    18,
                    0),
                scheduleName);


            // Act

            List<string> names =
                DatabaseReadService
                    .ReadAllScheduleNames();


            // Assert

            Assert.Contains(
                scheduleName,
                names);
        }


        // =========================================================
        // READ STAFF NAMES AND AVAILABILITY ON DAY
        // =========================================================

        [Fact]
        public void ReadStaffNamesAndAvailOnDay_MatchingDay_ReturnsStaff()
        {
            // Arrange

            int staffId =
                InsertStaff(
                    "Available",
                    "C",
                    "Person",
                    Positions.Attendant,
                    CreateUniqueEmail(),
                    false);

            TimeOnly startTime =
                new TimeOnly(
                    13,
                    30);

            TimeOnly endTime =
                new TimeOnly(
                    19,
                    15);

            InsertAvailability(
                staffId,
                DayOfWeek.Wednesday,
                startTime,
                endTime);


            // Act

            List<StaffNameAndAvail> results =
                DatabaseReadService
                    .ReadStaffNamesAndAvailOnDay(
                        DayOfWeek.Wednesday);


            // Assert

            StaffNameAndAvail? result =
                results.FirstOrDefault(
                    item =>
                        item.id == staffId);

            Assert.NotNull(
                result);

            Assert.Equal(
                "Available",
                result.fName);

            Assert.Equal(
                "Person",
                result.lName);

            Assert.Equal(
                startTime,
                result.startTime);

            Assert.Equal(
                endTime,
                result.endTime);
        }


        // =========================================================
        // READ STAFF AVAILABLE ON DAY
        // =========================================================

        [Fact]
        public void ReadStaffAvailOnDay_MatchingAvailability_ReturnsStaff()
        {
            // Arrange

            int staffId =
                InsertStaff(
                    "Day",
                    "D",
                    "Available",
                    Positions.Lead_Supervisor,
                    CreateUniqueEmail(),
                    true);

            InsertAvailability(
                staffId,
                DayOfWeek.Thursday,
                new TimeOnly(
                    14,
                    0),
                new TimeOnly(
                    20,
                    0));


            // Act

            List<Staff> staff =
                DatabaseReadService
                    .ReadStaffAvailOnDay(
                        DayOfWeek.Thursday);


            // Assert

            Staff? result =
                staff.FirstOrDefault(
                    item =>
                        item.id == staffId);

            Assert.NotNull(
                result);

            Assert.Equal(
                Positions.Lead_Supervisor,
                result.position);
        }


        [Fact]
        public void ReadStaffAvailOnDay_DifferentDay_DoesNotReturnStaff()
        {
            // Arrange

            int staffId =
                InsertStaff(
                    "Wrong",
                    "D",
                    "Day",
                    Positions.Attendant,
                    CreateUniqueEmail(),
                    false);

            InsertAvailability(
                staffId,
                DayOfWeek.Monday,
                new TimeOnly(
                    14,
                    0),
                new TimeOnly(
                    18,
                    0));


            // Act

            List<Staff> staff =
                DatabaseReadService
                    .ReadStaffAvailOnDay(
                        DayOfWeek.Sunday);


            // Assert

            Assert.DoesNotContain(
                staff,
                item =>
                    item.id == staffId);
        }


        // =========================================================
        // READ AVAILABILITY FOR STAFF
        // =========================================================

        [Fact]
        public void ReadAvailForStaffByID_ExistingAvailability_ReturnsOrderedDays()
        {
            // Arrange

            int staffId =
                Random.Shared.Next(
                    100000,
                    999999);

            InsertAvailability(
                staffId,
                DayOfWeek.Friday,
                new TimeOnly(
                    12,
                    0),
                new TimeOnly(
                    18,
                    0));

            InsertAvailability(
                staffId,
                DayOfWeek.Monday,
                new TimeOnly(
                    14,
                    0),
                new TimeOnly(
                    20,
                    0));

            InsertAvailability(
                staffId,
                DayOfWeek.Wednesday,
                new TimeOnly(
                    13,
                    0),
                new TimeOnly(
                    19,
                    0));


            // Act

            List<Availability> availability =
                DatabaseReadService
                    .ReadAvailForStaffByID(
                        staffId);


            // Assert

            Assert.Equal(
                3,
                availability.Count);

            Assert.Equal(
                DayOfWeek.Monday,
                availability[0]
                    .dayOfTheWeek);

            Assert.Equal(
                DayOfWeek.Wednesday,
                availability[1]
                    .dayOfTheWeek);

            Assert.Equal(
                DayOfWeek.Friday,
                availability[2]
                    .dayOfTheWeek);
        }


        // =========================================================
        // READ AVAILABILITY BY ID
        // =========================================================

        [Fact]
        public void ReadAvailByID_ExistingAvailability_ReturnsAvailability()
        {
            // Arrange

            int staffId =
                Random.Shared.Next(
                    100000,
                    999999);

            TimeOnly startTime =
                new TimeOnly(
                    10,
                    15);

            TimeOnly endTime =
                new TimeOnly(
                    16,
                    45);

            InsertAvailability(
                staffId,
                DayOfWeek.Tuesday,
                startTime,
                endTime);


            // Act

            List<Availability> availability =
                DatabaseReadService
                    .ReadAvailByID(
                        staffId);


            // Assert

            Assert.Contains(
                availability,
                item =>
                    item.id == staffId &&
                    item.dayOfTheWeek ==
                        DayOfWeek.Tuesday &&
                    item.startTime ==
                        startTime &&
                    item.endTime ==
                        endTime);
        }


        [Fact]
        public void ReadAvailByID_NonExistingId_ReturnsEmptyList()
        {
            // Arrange

            const int missingId =
                -777777;


            // Act

            List<Availability> availability =
                DatabaseReadService
                    .ReadAvailByID(
                        missingId);


            // Assert

            Assert.Empty(
                availability);
        }


        // =========================================================
        // READ DAYS OFF
        // =========================================================

        [Fact]
        public void ReadDaysOff_ExistingDayOff_ReturnsDayOff()
        {
            // Arrange

            int staffId =
                Random.Shared.Next(
                    100000,
                    999999);

            DateOnly date =
                new DateOnly(
                    2035,
                    6,
                    15);

            string reason =
                $"ReadTest_{Guid.NewGuid():N}"
                    .Substring(
                        0,
                        30);

            InsertDayOff(
                staffId,
                date,
                reason);


            // Act

            List<DaysOff> daysOff =
                DatabaseReadService
                    .ReadDaysOff();


            // Assert

            Assert.Contains(
                daysOff,
                item =>
                    item.id == staffId &&
                    item.date == date &&
                    item.reason == reason);
        }


        // =========================================================
        // HELPERS
        // =========================================================

        private static int InsertStaff(
            string firstName,
            string middleName,
            string lastName,
            Positions position,
            string email,
            bool isBelayCertified)
        {
            const string commandText =
                """
                INSERT INTO [UWGB].[Staff]
                (
                    fName,
                    mName,
                    lName,
                    position,
                    email,
                    belayCert
                )
                OUTPUT INSERTED.id
                VALUES
                (
                    @fName,
                    @mName,
                    @lName,
                    @position,
                    @email,
                    @belayCert
                );
                """;

            return DatabaseBase.ExecuteScalar<int>(
                commandText,

                new SqlParameter(
                    "@fName",
                    firstName),

                new SqlParameter(
                    "@mName",
                    middleName),

                new SqlParameter(
                    "@lName",
                    lastName),

                new SqlParameter(
                    "@position",
                    position.ToString()),

                new SqlParameter(
                    "@email",
                    email),

                new SqlParameter(
                    "@belayCert",
                    isBelayCertified.ToString()));
        }


        private static void InsertAvailability(
            int staffId,
            DayOfWeek day,
            TimeOnly startTime,
            TimeOnly endTime)
        {
            const string commandText =
                """
                INSERT INTO [UWGB].[Availability]
                (
                    id,
                    dayOfTheWeek,
                    startTime,
                    endTime
                )
                VALUES
                (
                    @id,
                    @day,
                    @startTime,
                    @endTime
                );
                """;

            DatabaseBase.ExecuteNonQuery(
                commandText,

                new SqlParameter(
                    "@id",
                    SqlDbType.Int)
                {
                    Value =
                        staffId
                },

                new SqlParameter(
                    "@day",
                    SqlDbType.Int)
                {
                    Value =
                        (int)day
                },

                new SqlParameter(
                    "@startTime",
                    SqlDbType.Time)
                {
                    Value =
                        startTime.ToTimeSpan()
                },

                new SqlParameter(
                    "@endTime",
                    SqlDbType.Time)
                {
                    Value =
                        endTime.ToTimeSpan()
                });
        }


        private static void InsertSchedule(
            DayOfWeek day,
            int? staffId,
            TimeOnly startTime,
            TimeOnly endTime,
            string scheduleName)
        {
            const string commandText =
                """
                INSERT INTO [UWGB].[Schedule]
                (
                    dayOfWeek,
                    staffID,
                    startTime,
                    endTime,
                    scheduleName
                )
                VALUES
                (
                    @day,
                    @staffId,
                    @startTime,
                    @endTime,
                    @scheduleName
                );
                """;

            DatabaseBase.ExecuteNonQuery(
                commandText,

                new SqlParameter(
                    "@day",
                    day.ToString()),

                new SqlParameter(
                    "@staffId",
                    SqlDbType.Int)
                {
                    Value =
                        staffId.HasValue
                            ? staffId.Value
                            : DBNull.Value
                },

                new SqlParameter(
                    "@startTime",
                    SqlDbType.Time)
                {
                    Value =
                        startTime.ToTimeSpan()
                },

                new SqlParameter(
                    "@endTime",
                    SqlDbType.Time)
                {
                    Value =
                        endTime.ToTimeSpan()
                },

                new SqlParameter(
                    "@scheduleName",
                    scheduleName));
        }


        private static void InsertJobSetting(
            DayOfWeek day,
            TimeOnly openingTime,
            TimeOnly closingTime)
        {
            const string commandText =
                """
                INSERT INTO [UWGB].[JobSettings]
                (
                    DayOfWeek,
                    OpeningTime,
                    ClosingTime
                )
                VALUES
                (
                    @day,
                    @openingTime,
                    @closingTime
                );
                """;

            DatabaseBase.ExecuteNonQuery(
                commandText,

                new SqlParameter(
                    "@day",
                    day.ToString()),

                new SqlParameter(
                    "@openingTime",
                    SqlDbType.Time)
                {
                    Value =
                        openingTime.ToTimeSpan()
                },

                new SqlParameter(
                    "@closingTime",
                    SqlDbType.Time)
                {
                    Value =
                        closingTime.ToTimeSpan()
                });
        }


        private static void DeleteJobSetting(
            DayOfWeek day)
        {
            const string commandText =
                """
                DELETE FROM [UWGB].[JobSettings]
                WHERE DayOfWeek = @day;
                """;

            DatabaseBase.ExecuteNonQuery(
                commandText,

                new SqlParameter(
                    "@day",
                    day.ToString()));
        }


        private static void InsertDayOff(
            int staffId,
            DateOnly date,
            string reason)
        {
            const string commandText =
                """
                INSERT INTO [UWGB].[DaysOff]
                (
                    id,
                    Date,
                    reason
                )
                VALUES
                (
                    @id,
                    @date,
                    @reason
                );
                """;

            DatabaseBase.ExecuteNonQuery(
                commandText,

                new SqlParameter(
                    "@id",
                    staffId),

                new SqlParameter(
                    "@date",
                    SqlDbType.Date)
                {
                    Value =
                        date.ToDateTime(
                            TimeOnly.MinValue)
                },

                new SqlParameter(
                    "@reason",
                    reason));
        }


        private static string CreateUniqueEmail()
        {
            return
                $"{Guid.NewGuid():N}@test.local";
        }
    }
}