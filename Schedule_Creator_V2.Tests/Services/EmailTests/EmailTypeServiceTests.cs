using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Services.Email;

namespace Schedule_Creator_V2.Tests.Services.Email
{
    public sealed class EmailTypeServiceTests
    {
        // =========================================================
        // EMAIL TYPES
        // =========================================================

        [Fact]
        public void CreateEmailTypes_ReturnsFourEmailTypes()
        {
            // Act

            List<EmailType> result =
                EmailTypeService
                    .CreateEmailTypes();


            // Assert

            Assert.Equal(
                4,
                result.Count);
        }


        [Fact]
        public void CreateEmailTypes_ReturnsExpectedNames()
        {
            // Act

            List<EmailType> result =
                EmailTypeService
                    .CreateEmailTypes();


            // Assert

            Assert.Equal(
                "Custom",
                result[0].displayName);

            Assert.Equal(
                "Announcements",
                result[1].displayName);

            Assert.Equal(
                "(No Request) Welcome to the Climbing Tower Team",
                result[2].displayName);

            Assert.Equal(
                "Schedule Announcement",
                result[3].displayName);
        }


        // =========================================================
        // CUSTOM
        // =========================================================

        [Fact]
        public void CreateEmailTypes_Custom_AllowsSectionEditing()
        {
            // Arrange

            EmailType custom =
                GetEmailType(
                    "Custom");


            // Assert

            Assert.True(
                custom.allowSectionEditing);
        }


        [Fact]
        public void CreateEmailTypes_Custom_ContainsRequiredBaseInputs()
        {
            // Arrange

            EmailType custom =
                GetEmailType(
                    "Custom");


            // Assert

            Assert.Contains(
                custom.inputs,
                input =>
                    input is EmailDetailsInputs);

            Assert.Contains(
                custom.inputs,
                input =>
                    input is CustomHeaderInputs);

            Assert.Contains(
                custom.inputs,
                input =>
                    input is CustomBodyInputs);

            Assert.Contains(
                custom.inputs,
                input =>
                    input is CustomSignatureInputs);

            Assert.Contains(
                custom.inputs,
                input =>
                    input is CustomFooterInputs);
        }


        // =========================================================
        // ANNOUNCEMENTS
        // =========================================================

        [Fact]
        public void CreateEmailTypes_Announcements_ContainsAnnouncementSection()
        {
            // Arrange

            EmailType announcements =
                GetEmailType(
                    "Announcements");


            // Assert

            Assert.Contains(
                announcements.inputs,
                input =>
                    input is
                        CustomAnnouncementsInputs);
        }


        [Fact]
        public void CreateEmailTypes_Announcements_DoesNotAllowSectionEditing()
        {
            // Arrange

            EmailType announcements =
                GetEmailType(
                    "Announcements");


            // Assert

            Assert.False(
                announcements
                    .allowSectionEditing);
        }


        // =========================================================
        // WELCOME
        // =========================================================

        [Fact]
        public void CreateEmailTypes_Welcome_HasExpectedSubject()
        {
            // Arrange

            EmailType welcome =
                GetEmailType(
                    "(No Request) Welcome to the Climbing Tower Team");

            EmailDetailsInputs details =
                welcome.inputs
                    .OfType<EmailDetailsInputs>()
                    .Single();


            // Assert

            Assert.Equal(
                "Welcome to the Climbing Tower Team",
                details.Subject);
        }


        // =========================================================
        // SCHEDULE ANNOUNCEMENT
        // =========================================================

        [Fact]
        public void CreateEmailTypes_ScheduleAnnouncement_ContainsImageSection()
        {
            // Arrange

            EmailType schedule =
                GetEmailType(
                    "Schedule Announcement");


            // Assert

            Assert.Contains(
                schedule.inputs,
                input =>
                    input is CustomImageInputs);
        }


        [Fact]
        public void CreateEmailTypes_ScheduleAnnouncement_ContainsRequestSection()
        {
            // Arrange

            EmailType schedule =
                GetEmailType(
                    "Schedule Announcement");


            // Assert

            Assert.Contains(
                schedule.inputs,
                input =>
                    input is CustomRequestInputs);
        }


        [Fact]
        public void CreateEmailTypes_ScheduleAnnouncement_SubjectContainsSchedulePlaceholder()
        {
            // Arrange

            EmailType schedule =
                GetEmailType(
                    "Schedule Announcement");

            EmailDetailsInputs details =
                schedule.inputs
                    .OfType<EmailDetailsInputs>()
                    .Single();


            // Assert

            Assert.Contains(
                "[SCHEDULE NAME]",
                details.Subject);
        }


        // =========================================================
        // NEW INSTANCES
        // =========================================================

        [Fact]
        public void CreateEmailTypes_CalledTwice_ReturnsDifferentLists()
        {
            // Act

            List<EmailType> first =
                EmailTypeService
                    .CreateEmailTypes();

            List<EmailType> second =
                EmailTypeService
                    .CreateEmailTypes();


            // Assert

            Assert.NotSame(
                first,
                second);
        }


        [Fact]
        public void CreateEmailTypes_CalledTwice_EmailTypesAreDifferentInstances()
        {
            // Act

            List<EmailType> first =
                EmailTypeService
                    .CreateEmailTypes();

            List<EmailType> second =
                EmailTypeService
                    .CreateEmailTypes();


            // Assert

            Assert.NotSame(
                first[0],
                second[0]);
        }


        // =========================================================
        // HELPERS
        // =========================================================

        private static EmailType GetEmailType(
            string displayName)
        {
            return EmailTypeService
                .CreateEmailTypes()
                .Single(
                    emailType =>
                        emailType.displayName ==
                        displayName);
        }
    }
}