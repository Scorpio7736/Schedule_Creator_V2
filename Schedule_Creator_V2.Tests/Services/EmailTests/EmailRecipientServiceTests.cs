using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Enums;
using Schedule_Creator_V2.Services.Email;

namespace Schedule_Creator_V2.Tests.Services.Email
{
    public sealed class EmailRecipientServiceTests
    {
        // =========================================================
        // VALID EMAILS
        // =========================================================

        [Fact]
        public void GetRecipientAddresses_ValidStaff_ReturnsAddresses()
        {
            // Arrange

            List<Staff> staff =
                new List<Staff>
                {
                    CreateStaff(
                        1,
                        "Jack",
                        "London",
                        "jack@example.com"),

                    CreateStaff(
                        2,
                        "Alex",
                        "Smith",
                        "alex@example.com")
                };


            // Act

            List<string> result =
                EmailRecipientService
                    .GetRecipientAddresses(
                        staff);


            // Assert

            Assert.Equal(
                2,
                result.Count);

            Assert.Contains(
                "jack@example.com",
                result);

            Assert.Contains(
                "alex@example.com",
                result);
        }


        // =========================================================
        // TRIMMING
        // =========================================================

        [Fact]
        public void GetRecipientAddresses_EmailHasWhitespace_TrimsAddress()
        {
            // Arrange

            List<Staff> staff =
                new List<Staff>
                {
                    CreateStaff(
                        1,
                        "Jack",
                        "London",
                        "  jack@example.com  ")
                };


            // Act

            List<string> result =
                EmailRecipientService
                    .GetRecipientAddresses(
                        staff);


            // Assert

            Assert.Single(
                result);

            Assert.Equal(
                "jack@example.com",
                result[0]);
        }


        // =========================================================
        // DUPLICATES
        // =========================================================

        [Fact]
        public void GetRecipientAddresses_DuplicateEmails_RemovesDuplicates()
        {
            // Arrange

            List<Staff> staff =
                new List<Staff>
                {
                    CreateStaff(
                        1,
                        "First",
                        "Person",
                        "staff@example.com"),

                    CreateStaff(
                        2,
                        "Second",
                        "Person",
                        "staff@example.com")
                };


            // Act

            List<string> result =
                EmailRecipientService
                    .GetRecipientAddresses(
                        staff);


            // Assert

            Assert.Single(
                result);

            Assert.Equal(
                "staff@example.com",
                result[0]);
        }


        [Fact]
        public void GetRecipientAddresses_DuplicateEmailsDifferentCase_RemovesDuplicates()
        {
            // Arrange

            List<Staff> staff =
                new List<Staff>
                {
                    CreateStaff(
                        1,
                        "First",
                        "Person",
                        "STAFF@example.com"),

                    CreateStaff(
                        2,
                        "Second",
                        "Person",
                        "staff@example.com")
                };


            // Act

            List<string> result =
                EmailRecipientService
                    .GetRecipientAddresses(
                        staff);


            // Assert

            Assert.Single(
                result);
        }


        // =========================================================
        // INVALID EMAILS
        // =========================================================

        [Fact]
        public void GetRecipientAddresses_EmptyEmail_ThrowsInvalidOperationException()
        {
            // Arrange

            List<Staff> staff =
                new List<Staff>
                {
                    CreateStaff(
                        1,
                        "Jack",
                        "London",
                        "")
                };


            // Act

            Action action =
                () =>
                    EmailRecipientService
                        .GetRecipientAddresses(
                            staff);


            // Assert

            Assert.Throws<InvalidOperationException>(
                action);
        }


        [Fact]
        public void GetRecipientAddresses_InvalidEmail_ThrowsInvalidOperationException()
        {
            // Arrange

            List<Staff> staff =
                new List<Staff>
                {
                    CreateStaff(
                        1,
                        "Jack",
                        "London",
                        "not-an-email")
                };


            // Act

            Action action =
                () =>
                    EmailRecipientService
                        .GetRecipientAddresses(
                            staff);


            // Assert

            Assert.Throws<InvalidOperationException>(
                action);
        }


        [Fact]
        public void GetRecipientAddresses_InvalidEmail_ExceptionContainsStaffName()
        {
            // Arrange

            List<Staff> staff =
                new List<Staff>
                {
                    CreateStaff(
                        1,
                        "Jack",
                        "London",
                        "invalid")
                };


            // Act

            InvalidOperationException exception =
                Assert.Throws<InvalidOperationException>(
                    () =>
                        EmailRecipientService
                            .GetRecipientAddresses(
                                staff));


            // Assert

            Assert.Contains(
                "Jack London",
                exception.Message);
        }


        [Fact]
        public void GetRecipientAddresses_OneInvalidStaff_ThrowsEvenWhenOthersAreValid()
        {
            // Arrange

            List<Staff> staff =
                new List<Staff>
                {
                    CreateStaff(
                        1,
                        "Valid",
                        "Person",
                        "valid@example.com"),

                    CreateStaff(
                        2,
                        "Invalid",
                        "Person",
                        "bad-email")
                };


            // Act

            Action action =
                () =>
                    EmailRecipientService
                        .GetRecipientAddresses(
                            staff);


            // Assert

            Assert.Throws<InvalidOperationException>(
                action);
        }


        // =========================================================
        // EMPTY COLLECTION
        // =========================================================

        [Fact]
        public void GetRecipientAddresses_EmptyStaffList_ReturnsEmptyList()
        {
            // Arrange

            List<Staff> staff =
                new List<Staff>();


            // Act

            List<string> result =
                EmailRecipientService
                    .GetRecipientAddresses(
                        staff);


            // Assert

            Assert.Empty(
                result);
        }


        // =========================================================
        // HELPERS
        // =========================================================

        private static Staff CreateStaff(
            int id,
            string firstName,
            string lastName,
            string email)
        {
            return new Staff(
                id,
                firstName,
                "",
                lastName,
                Positions.Attendant,
                email,
                false,
                null,
                null);
        }
    }
}