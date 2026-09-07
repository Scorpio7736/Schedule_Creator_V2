using Microsoft.Data.SqlClient;
using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Enums;
using Schedule_Creator_V2.Services.Email;
using Schedule_Creator_V2.Tests.Services.Database;
using System.Data;

using DatabaseBase =
    Schedule_Creator_V2.Services.Database.Database;

namespace Schedule_Creator_V2.Tests.Services.Email
{
    public sealed class EmailStaffServiceTests :
        IClassFixture<DatabaseCreateTestFixture>
    {
        public EmailStaffServiceTests(
            DatabaseCreateTestFixture fixture)
        {
            /*
             * DatabaseCreateTestFixture creates the isolated
             * test database and points the Database service at it.
             */
        }


        // =========================================================
        // LOAD STAFF
        // =========================================================

        [Fact]
        public void LoadStaff_ExistingStaff_ReturnsStaffMember()
        {
            // Arrange

            string email =
                $"{Guid.NewGuid():N}@test.local";

            int staffId =
                InsertStaff(
                    firstName:
                        "Email",

                    middleName:
                        "T",

                    lastName:
                        "Tester",

                    position:
                        Positions.Attendant,

                    email:
                        email,

                    belayCertified:
                        false,

                    certifiedOn:
                        null,

                    expiresOn:
                        null);

            try
            {
                // Act

                List<Staff> result =
                    EmailStaffService.LoadStaff();


                // Assert

                Staff staff =
                    Assert.Single(
                        result.Where(
                            staff =>
                                staff.id ==
                                staffId));

                Assert.Equal(
                    staffId,
                    staff.id);

                Assert.Equal(
                    "Email",
                    staff.fName);

                Assert.Equal(
                    "T",
                    staff.mName);

                Assert.Equal(
                    "Tester",
                    staff.lName);

                Assert.Equal(
                    Positions.Attendant,
                    staff.position);

                Assert.Equal(
                    email,
                    staff.email);

                Assert.False(
                    staff.isBelayCertified);

                Assert.Null(
                    staff.certifiedOn);

                Assert.Null(
                    staff.expiresOn);
            }
            finally
            {
                DeleteStaff(
                    staffId);
            }
        }


        // =========================================================
        // BELAY CERTIFICATION
        // =========================================================

        [Fact]
        public void LoadStaff_CertifiedStaff_ReturnsCertificationData()
        {
            // Arrange

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

            string email =
                $"{Guid.NewGuid():N}@test.local";

            int staffId =
                InsertStaff(
                    firstName:
                        "Certified",

                    middleName:
                        "T",

                    lastName:
                        "Tester",

                    position:
                        Positions.Attendant,

                    email:
                        email,

                    belayCertified:
                        true,

                    certifiedOn:
                        certifiedOn,

                    expiresOn:
                        expiresOn);

            try
            {
                // Act

                List<Staff> result =
                    EmailStaffService.LoadStaff();


                // Assert

                Staff staff =
                    Assert.Single(
                        result.Where(
                            staff =>
                                staff.id ==
                                staffId));

                Assert.True(
                    staff.isBelayCertified);

                Assert.Equal(
                    certifiedOn,
                    staff.certifiedOn);

                Assert.Equal(
                    expiresOn,
                    staff.expiresOn);
            }
            finally
            {
                DeleteStaff(
                    staffId);
            }
        }


        // =========================================================
        // MULTIPLE STAFF
        // =========================================================

        [Fact]
        public void LoadStaff_MultipleStaff_ReturnsBothStaffMembers()
        {
            // Arrange

            string firstEmail =
                $"{Guid.NewGuid():N}@test.local";

            string secondEmail =
                $"{Guid.NewGuid():N}@test.local";

            int firstId =
                InsertStaff(
                    "First",
                    "T",
                    "Tester",
                    Positions.Attendant,
                    firstEmail,
                    false,
                    null,
                    null);

            int secondId =
                InsertStaff(
                    "Second",
                    "T",
                    "Tester",
                    Positions.Attendant,
                    secondEmail,
                    false,
                    null,
                    null);

            try
            {
                // Act

                List<Staff> result =
                    EmailStaffService.LoadStaff();


                // Assert

                Assert.Contains(
                    result,
                    staff =>
                        staff.id ==
                        firstId);

                Assert.Contains(
                    result,
                    staff =>
                        staff.id ==
                        secondId);
            }
            finally
            {
                DeleteStaff(
                    firstId);

                DeleteStaff(
                    secondId);
            }
        }


        // =========================================================
        // RETURN VALUE
        // =========================================================

        [Fact]
        public void LoadStaff_ReturnsNonNullList()
        {
            // Act

            List<Staff> result =
                EmailStaffService.LoadStaff();


            // Assert

            Assert.NotNull(
                result);
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
            bool belayCertified,
            DateOnly? certifiedOn,
            DateOnly? expiresOn)
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
                    belayCert,
                    certifiedOn,
                    expiresOn
                )
                OUTPUT INSERTED.id
                VALUES
                (
                    @fName,
                    @mName,
                    @lName,
                    @position,
                    @email,
                    @belayCert,
                    @certifiedOn,
                    @expiresOn
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
                    belayCertified.ToString()),

                new SqlParameter(
                    "@certifiedOn",
                    SqlDbType.Date)
                {
                    Value =
                        certifiedOn.HasValue
                            ? certifiedOn.Value
                                .ToDateTime(
                                    TimeOnly.MinValue)
                            : DBNull.Value
                },

                new SqlParameter(
                    "@expiresOn",
                    SqlDbType.Date)
                {
                    Value =
                        expiresOn.HasValue
                            ? expiresOn.Value
                                .ToDateTime(
                                    TimeOnly.MinValue)
                            : DBNull.Value
                });
        }


        private static void DeleteStaff(
            int staffId)
        {
            const string commandText =
                """
                DELETE FROM [UWGB].[Staff]
                WHERE id = @id;
                """;

            DatabaseBase.ExecuteNonQuery(
                commandText,

                new SqlParameter(
                    "@id",
                    staffId));
        }
    }
}