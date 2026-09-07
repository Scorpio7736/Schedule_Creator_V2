using Microsoft.Data.SqlClient;
using Schedule_Creator_V2.Models.Enums;
using System.Data;

using DatabaseBase =
    Schedule_Creator_V2.Services.Database.Database;

using DatabaseUpdateService =
    Schedule_Creator_V2.Services.Database.DatabaseUpdate;

namespace Schedule_Creator_V2.Tests.Services.Database
{
    public sealed class DatabaseUpdateTests :
        IClassFixture<DatabaseCreateTestFixture>
    {
        public DatabaseUpdateTests(
            DatabaseCreateTestFixture fixture)
        {
            /*
             * DatabaseCreateTestFixture creates the isolated
             * LocalDB database, UWGB schema, and Staff table.
             *
             * These tests create their own staff rows before
             * testing update operations.
             */
        }


        // =========================================================
        // UPDATE STAFF
        // =========================================================

        [Fact]
        public void UpdateStaff_ExistingStaff_UpdatesValues()
        {
            // Arrange

            int staffId =
                InsertTestStaff(
                    "Original",
                    "M",
                    "Person",
                    Positions.Attendant,
                    CreateUniqueEmail(),
                    false);

            const string expectedFirstName =
                "Updated";

            const string expectedMiddleName =
                "T";

            const string expectedLastName =
                "Tester";

            const Positions expectedPosition =
                Positions.Shift_Lead;

            string expectedEmail =
                CreateUniqueEmail();


            // Act

            DatabaseUpdateService.UpdateStaff(
                staffId,
                expectedFirstName,
                expectedMiddleName,
                expectedLastName,
                expectedPosition,
                expectedEmail);


            // Assert

            const string selectSql =
                """
                SELECT
                    fName,
                    mName,
                    lName,
                    position,
                    email
                FROM [UWGB].[Staff]
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
                expectedFirstName,
                reader["fName"]);

            Assert.Equal(
                expectedMiddleName,
                reader["mName"]);

            Assert.Equal(
                expectedLastName,
                reader["lName"]);

            Assert.Equal(
                expectedPosition.ToString(),
                reader["position"]);

            Assert.Equal(
                expectedEmail,
                reader["email"]);
        }


        [Fact]
        public void UpdateStaff_ExistingStaff_DoesNotChangeBelayCertification()
        {
            // Arrange

            int staffId =
                InsertTestStaff(
                    "Original",
                    "M",
                    "Person",
                    Positions.Attendant,
                    CreateUniqueEmail(),
                    true);

            DateOnly certifiedOn =
                new DateOnly(
                    2026,
                    1,
                    15);

            DateOnly expiresOn =
                new DateOnly(
                    2027,
                    1,
                    15);

            SetBelayCertificationDirectly(
                staffId,
                true,
                certifiedOn,
                expiresOn);


            // Act

            DatabaseUpdateService.UpdateStaff(
                staffId,
                "Changed",
                "X",
                "Name",
                Positions.Lead_Supervisor,
                CreateUniqueEmail());


            // Assert

            const string selectSql =
                """
                SELECT
                    belayCert,
                    certifiedOn,
                    expiresOn
                FROM [UWGB].[Staff]
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
                true.ToString(),
                reader["belayCert"]);

            Assert.Equal(
                certifiedOn.ToDateTime(
                    TimeOnly.MinValue),
                reader["certifiedOn"]);

            Assert.Equal(
                expiresOn.ToDateTime(
                    TimeOnly.MinValue),
                reader["expiresOn"]);
        }


        [Fact]
        public void UpdateStaff_NonExistingId_DoesNotCreateStaff()
        {
            // Arrange

            const int missingStaffId =
                -999999;


            // Act

            DatabaseUpdateService.UpdateStaff(
                missingStaffId,
                "Nobody",
                "N",
                "Here",
                Positions.Attendant,
                "nobody@test.local");


            // Assert

            const string countSql =
                """
                SELECT COUNT(*)
                FROM [UWGB].[Staff]
                WHERE id = @id;
                """;

            int count =
                DatabaseBase.ExecuteScalar<int>(
                    countSql,

                    new SqlParameter(
                        "@id",
                        missingStaffId));

            Assert.Equal(
                0,
                count);
        }


        // =========================================================
        // UPDATE BELAY CERTIFICATION
        // =========================================================

        [Fact]
        public void UpdateBelayCert_Certified_UpdatesStatusAndDates()
        {
            // Arrange

            int staffId =
                InsertTestStaff(
                    "Belay",
                    "C",
                    "Tester",
                    Positions.Attendant,
                    CreateUniqueEmail(),
                    false);

            DateOnly certifiedOn =
                new DateOnly(
                    2026,
                    9,
                    7);

            DateOnly expiresOn =
                new DateOnly(
                    2027,
                    9,
                    7);


            // Act

            DatabaseUpdateService.UpdateBelayCert(
                staffId,
                true,
                certifiedOn,
                expiresOn);


            // Assert

            const string selectSql =
                """
                SELECT
                    belayCert,
                    certifiedOn,
                    expiresOn
                FROM [UWGB].[Staff]
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
                true.ToString(),
                reader["belayCert"]);

            Assert.Equal(
                certifiedOn.ToDateTime(
                    TimeOnly.MinValue),
                reader["certifiedOn"]);

            Assert.Equal(
                expiresOn.ToDateTime(
                    TimeOnly.MinValue),
                reader["expiresOn"]);
        }


        [Fact]
        public void UpdateBelayCert_NotCertified_WithNullDates_SavesDatabaseNulls()
        {
            // Arrange

            int staffId =
                InsertTestStaff(
                    "Belay",
                    "N",
                    "Tester",
                    Positions.Attendant,
                    CreateUniqueEmail(),
                    true);

            SetBelayCertificationDirectly(
                staffId,
                true,
                new DateOnly(
                    2026,
                    5,
                    1),
                new DateOnly(
                    2027,
                    5,
                    1));


            // Act

            DatabaseUpdateService.UpdateBelayCert(
                staffId,
                false,
                null,
                null);


            // Assert

            const string selectSql =
                """
                SELECT
                    belayCert,
                    certifiedOn,
                    expiresOn
                FROM [UWGB].[Staff]
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
                false.ToString(),
                reader["belayCert"]);

            Assert.Equal(
                DBNull.Value,
                reader["certifiedOn"]);

            Assert.Equal(
                DBNull.Value,
                reader["expiresOn"]);
        }


        [Fact]
        public void UpdateBelayCert_CertifiedWithNoDates_SavesNullDates()
        {
            // Arrange

            int staffId =
                InsertTestStaff(
                    "No",
                    "D",
                    "Dates",
                    Positions.Attendant,
                    CreateUniqueEmail(),
                    false);


            // Act

            DatabaseUpdateService.UpdateBelayCert(
                staffId,
                true);


            // Assert

            const string selectSql =
                """
                SELECT
                    belayCert,
                    certifiedOn,
                    expiresOn
                FROM [UWGB].[Staff]
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
                true.ToString(),
                reader["belayCert"]);

            Assert.Equal(
                DBNull.Value,
                reader["certifiedOn"]);

            Assert.Equal(
                DBNull.Value,
                reader["expiresOn"]);
        }


        [Fact]
        public void UpdateBelayCert_NonExistingId_DoesNotCreateStaff()
        {
            // Arrange

            const int missingStaffId =
                -888888;


            // Act

            DatabaseUpdateService.UpdateBelayCert(
                missingStaffId,
                true,
                new DateOnly(
                    2026,
                    9,
                    7),
                new DateOnly(
                    2027,
                    9,
                    7));


            // Assert

            const string countSql =
                """
                SELECT COUNT(*)
                FROM [UWGB].[Staff]
                WHERE id = @id;
                """;

            int count =
                DatabaseBase.ExecuteScalar<int>(
                    countSql,

                    new SqlParameter(
                        "@id",
                        missingStaffId));

            Assert.Equal(
                0,
                count);
        }


        // =========================================================
        // HELPERS
        // =========================================================

        private static int InsertTestStaff(
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


        private static void SetBelayCertificationDirectly(
            int staffId,
            bool isBelayCertified,
            DateOnly? certifiedOn,
            DateOnly? expiresOn)
        {
            const string commandText =
                """
                UPDATE [UWGB].[Staff]
                SET
                    belayCert = @belayCert,
                    certifiedOn = @certifiedOn,
                    expiresOn = @expiresOn
                WHERE id = @id;
                """;

            DatabaseBase.ExecuteNonQuery(
                commandText,

                new SqlParameter(
                    "@id",
                    staffId),

                new SqlParameter(
                    "@belayCert",
                    isBelayCertified.ToString()),

                new SqlParameter(
                    "@certifiedOn",
                    SqlDbType.Date)
                {
                    Value =
                        certifiedOn.HasValue
                            ? certifiedOn.Value.ToDateTime(
                                TimeOnly.MinValue)
                            : DBNull.Value
                },

                new SqlParameter(
                    "@expiresOn",
                    SqlDbType.Date)
                {
                    Value =
                        expiresOn.HasValue
                            ? expiresOn.Value.ToDateTime(
                                TimeOnly.MinValue)
                            : DBNull.Value
                });
        }


        private static string CreateUniqueEmail()
        {
            return
                $"{Guid.NewGuid():N}@test.local";
        }
    }
}