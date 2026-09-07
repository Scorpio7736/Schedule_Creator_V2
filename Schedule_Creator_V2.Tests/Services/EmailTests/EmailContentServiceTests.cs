using Schedule_Creator_V2.Models.Interfaces;
using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Services.Email;

namespace Schedule_Creator_V2.Tests.Services.Email
{
    public sealed class EmailContentServiceTests
    {
        // =========================================================
        // BUILD SUBJECT
        // =========================================================

        [Fact]
        public void BuildSubject_ValidSubject_ReturnsSubject()
        {
            // Arrange

            EmailType emailType =
                CreateEmail(
                    "Tower Schedule");


            // Act

            string result =
                EmailContentService.BuildSubject(
                    emailType);


            // Assert

            Assert.Equal(
                "Tower Schedule",
                result);
        }


        [Fact]
        public void BuildSubject_SubjectHasWhitespace_TrimsSubject()
        {
            // Arrange

            EmailType emailType =
                CreateEmail(
                    "   Tower Schedule   ");


            // Act

            string result =
                EmailContentService.BuildSubject(
                    emailType);


            // Assert

            Assert.Equal(
                "Tower Schedule",
                result);
        }


        [Fact]
        public void BuildSubject_NoEmailDetails_ReturnsEmptyString()
        {
            // Arrange

            EmailType emailType =
                new EmailType(
                    "Test Email",
                    new List<IEmailInputs>(),
                    true);


            // Act

            string result =
                EmailContentService.BuildSubject(
                    emailType);


            // Assert

            Assert.Equal(
                string.Empty,
                result);
        }


        [Fact]
        public void BuildSubject_NullEmailType_ThrowsArgumentNullException()
        {
            // Act / Assert

            Assert.Throws<ArgumentNullException>(
                () =>
                    EmailContentService.BuildSubject(
                        null!));
        }


        // =========================================================
        // BUILD HTML BODY
        // =========================================================

        [Fact]
        public void BuildHtmlBody_ValidEmail_ReturnsHtml()
        {
            // Arrange

            EmailType emailType =
                CreateEmail(
                    "Tower Test");


            // Act

            string html =
                EmailContentService.BuildHtmlBody(
                    emailType);


            // Assert

            Assert.False(
                string.IsNullOrWhiteSpace(
                    html));

            Assert.Contains(
                "Tower Test",
                html);
        }


        [Fact]
        public void BuildHtmlBody_NullEmailType_ThrowsArgumentNullException()
        {
            // Act / Assert

            Assert.Throws<ArgumentNullException>(
                () =>
                    EmailContentService.BuildHtmlBody(
                        null!));
        }


        // =========================================================
        // HEADER
        // =========================================================

        [Fact]
        public void BuildHtmlBody_HeaderInputs_RendersHeaderValues()
        {
            // Arrange

            EmailType emailType =
                CreateEmail(
                    "Header Test",
                    organizationName:
                        "Test Organization",
                    headerLabel:
                        "TEST LABEL",
                    heading:
                        "Important Heading",
                    subtitle:
                        "Important Subtitle");


            // Act

            string html =
                EmailContentService.BuildHtmlBody(
                    emailType);


            // Assert

            Assert.Contains(
                "Test Organization",
                html);

            Assert.Contains(
                "TEST LABEL",
                html);

            Assert.Contains(
                "Important Heading",
                html);

            Assert.Contains(
                "Important Subtitle",
                html);

            Assert.Contains(
                "id=\"organizationLabel\"",
                html);

            Assert.Contains(
                "id=\"headerLabel\"",
                html);
        }


        // =========================================================
        // BODY
        // =========================================================

        [Fact]
        public void BuildHtmlBody_BodyInputs_RendersGreetingAndBody()
        {
            // Arrange

            EmailType emailType =
                CreateEmail(
                    subject:
                        "Body Test",
                    greeting:
                        "Hello Tower Team,",
                    body:
                        "Please review the new schedule.");


            // Act

            string html =
                EmailContentService.BuildHtmlBody(
                    emailType);


            // Assert

            Assert.Contains(
                "Hello Tower Team,",
                html);

            Assert.Contains(
                "Please review the new schedule.",
                html);

            Assert.Contains(
                "id=\"bodySection\"",
                html);

            Assert.Contains(
                "id=\"recipientGreeting\"",
                html);
        }


        // =========================================================
        // HTML ENCODING
        // =========================================================

        [Fact]
        public void BuildHtmlBody_PlainTextContainsHtmlCharacters_EncodesCharacters()
        {
            // Arrange

            EmailType emailType =
                CreateEmail(
                    subject:
                        "A & B < Test >",
                    greeting:
                        "Hello <Team>",
                    body:
                        "5 < 10 & 10 > 5");


            // Act

            string html =
                EmailContentService.BuildHtmlBody(
                    emailType);


            // Assert

            Assert.Contains(
                "A &amp; B &lt; Test &gt;",
                html);

            Assert.Contains(
                "Hello &lt;Team&gt;",
                html);

            Assert.Contains(
                "5 &lt; 10 &amp; 10 &gt; 5",
                html);
        }


        // =========================================================
        // OPTIONAL IMAGE
        // =========================================================

        [Fact]
        public void BuildHtmlBody_NoImageSection_DoesNotRenderImageSection()
        {
            // Arrange

            EmailType emailType =
                CreateEmail(
                    "No Image");


            // Act

            string html =
                EmailContentService.BuildHtmlBody(
                    emailType);


            // Assert

            Assert.DoesNotContain(
                "id=\"imageSection\"",
                html);
        }


        [Fact]
        public void BuildHtmlBody_ValidHttpsImage_RendersImage()
        {
            // Arrange

            EmailType emailType =
                CreateEmail(
                    "Image Test");

            emailType.inputs.Add(
                new CustomImageInputs(
                    "https://example.com/tower.png",
                    "Tower Photo"));


            // Act

            string html =
                EmailContentService.BuildHtmlBody(
                    emailType);


            // Assert

            Assert.Contains(
                "id=\"imageSection\"",
                html);

            Assert.Contains(
                "src=\"https://example.com/tower.png\"",
                html);

            Assert.Contains(
                "alt=\"Tower Photo\"",
                html);
        }


        [Fact]
        public void BuildHtmlBody_ImageUrlWithoutScheme_AddsHttps()
        {
            // Arrange

            EmailType emailType =
                CreateEmail(
                    "Image URL Test");

            emailType.inputs.Add(
                new CustomImageInputs(
                    "example.com/tower.png",
                    "Tower"));


            // Act

            string html =
                EmailContentService.BuildHtmlBody(
                    emailType);


            // Assert

            Assert.Contains(
                "https://example.com/tower.png",
                html);
        }


        [Fact]
        public void BuildHtmlBody_ImageUsesUnsupportedUrlScheme_ThrowsInvalidOperationException()
        {
            // Arrange

            EmailType emailType =
                CreateEmail(
                    "Bad Image");

            emailType.inputs.Add(
                new CustomImageInputs(
                    "ftp://example.com/tower.png",
                    "Tower"));


            // Act

            Action action =
                () =>
                    EmailContentService.BuildHtmlBody(
                        emailType);


            // Assert

            Assert.Throws<InvalidOperationException>(
                action);
        }


        // =========================================================
        // LOCAL IMAGE
        // =========================================================

        [Fact]
        public void BuildHtmlBody_LocalPngImage_ConvertsImageToDataUri()
        {
            // Arrange

            string imagePath =
                Path.Combine(
                    Path.GetTempPath(),
                    $"{Guid.NewGuid():N}.png");

            byte[] imageBytes =
            {
                1,
                2,
                3,
                4,
                5
            };

            File.WriteAllBytes(
                imagePath,
                imageBytes);

            try
            {
                EmailType emailType =
                    CreateEmail(
                        "Local Image");

                emailType.inputs.Add(
                    new CustomImageInputs(
                        imagePath,
                        "Local Image"));

                string expectedBase64 =
                    Convert.ToBase64String(
                        imageBytes);


                // Act

                string html =
                    EmailContentService.BuildHtmlBody(
                        emailType);


                // Assert

                Assert.Contains(
                    "data:image/png;base64," +
                    expectedBase64,
                    html);
            }
            finally
            {
                if (File.Exists(
                        imagePath))
                {
                    File.Delete(
                        imagePath);
                }
            }
        }


        // =========================================================
        // ANNOUNCEMENTS
        // =========================================================

        [Fact]
        public void BuildHtmlBody_Announcements_RendersAnnouncementSection()
        {
            // Arrange

            EmailType emailType =
                CreateEmail(
                    "Announcements");

            emailType.inputs.Add(
                new CustomAnnouncementsInputs(
                    "ANNOUNCEMENTS",
                    "Please review these updates.",
                    new List<string>
                    {
                        "Tower closes early Friday.",
                        "Staff training is Saturday."
                    }));


            // Act

            string html =
                EmailContentService.BuildHtmlBody(
                    emailType);


            // Assert

            Assert.Contains(
                "id=\"announcementsSection\"",
                html);

            Assert.Contains(
                "ANNOUNCEMENTS",
                html);

            Assert.Contains(
                "Please review these updates.",
                html);

            Assert.Contains(
                "Tower closes early Friday.",
                html);

            Assert.Contains(
                "Staff training is Saturday.",
                html);
        }


        [Fact]
        public void BuildHtmlBody_EmptyAnnouncements_DoesNotRenderAnnouncementSection()
        {
            // Arrange

            EmailType emailType =
                CreateEmail(
                    "Empty Announcements");

            emailType.inputs.Add(
                new CustomAnnouncementsInputs(
                    "",
                    "",
                    new List<string>()));


            // Act

            string html =
                EmailContentService.BuildHtmlBody(
                    emailType);


            // Assert

            Assert.DoesNotContain(
                "id=\"announcementsSection\"",
                html);
        }


        // =========================================================
        // REQUEST
        // =========================================================

        [Fact]
        public void BuildHtmlBody_Request_RendersRequestSection()
        {
            // Arrange

            EmailType emailType =
                CreateEmail(
                    "Request Test");

            emailType.inputs.Add(
                new CustomRequestInputs(
                    "REQUEST",
                    "Review Schedule",
                    "Please review your assigned shifts.",
                    "View Schedule",
                    "https://example.com/schedule"));


            // Act

            string html =
                EmailContentService.BuildHtmlBody(
                    emailType);


            // Assert

            Assert.Contains(
                "id=\"requestSection\"",
                html);

            Assert.Contains(
                "Review Schedule",
                html);

            Assert.Contains(
                "Please review your assigned shifts.",
                html);

            Assert.Contains(
                "View Schedule",
                html);

            Assert.Contains(
                "https://example.com/schedule",
                html);
        }


        [Fact]
        public void BuildHtmlBody_RequestLinkWithoutScheme_AddsHttps()
        {
            // Arrange

            EmailType emailType =
                CreateEmail(
                    "Request Link Test");

            emailType.inputs.Add(
                new CustomRequestInputs(
                    "REQUEST",
                    "Review Schedule",
                    "Open the schedule.",
                    "View Schedule",
                    "example.com/schedule"));


            // Act

            string html =
                EmailContentService.BuildHtmlBody(
                    emailType);


            // Assert

            Assert.Contains(
                "https://example.com/schedule",
                html);
        }


        // =========================================================
        // ATTACHMENTS
        // =========================================================

        [Fact]
        public void BuildHtmlBody_Attachments_RendersAttachmentNames()
        {
            // Arrange

            EmailType emailType =
                CreateEmail(
                    "Attachments Test");

            emailType.inputs.Add(
                new CustomAttachmentsInputs(
                    "ATTACHMENTS",
                    "Files included with this message.",
                    new List<string>
                    {
                        "TowerSchedule.pdf",
                        "TrainingNotes.docx"
                    }));


            // Act

            string html =
                EmailContentService.BuildHtmlBody(
                    emailType);


            // Assert

            Assert.Contains(
                "id=\"attachmentsSection\"",
                html);

            Assert.Contains(
                "ATTACHMENTS",
                html);

            Assert.Contains(
                "Files included with this message.",
                html);

            Assert.Contains(
                "TowerSchedule.pdf",
                html);

            Assert.Contains(
                "TrainingNotes.docx",
                html);
        }


        [Fact]
        public void BuildHtmlBody_EmptyAttachments_DoesNotRenderAttachmentSection()
        {
            // Arrange

            EmailType emailType =
                CreateEmail(
                    "No Attachments");

            emailType.inputs.Add(
                new CustomAttachmentsInputs(
                    "",
                    "",
                    new List<string>()));


            // Act

            string html =
                EmailContentService.BuildHtmlBody(
                    emailType);


            // Assert

            Assert.DoesNotContain(
                "id=\"attachmentsSection\"",
                html);
        }


        // =========================================================
        // TEMPLATE MARKERS
        // =========================================================

        [Fact]
        public void BuildHtmlBody_FinishedHtml_RemovesSectionMarkers()
        {
            // Arrange

            EmailType emailType =
                CreateEmail(
                    "Marker Test");


            // Act

            string html =
                EmailContentService.BuildHtmlBody(
                    emailType);


            // Assert

            Assert.DoesNotContain(
                "<!-- HEADER -->",
                html);

            Assert.DoesNotContain(
                "<!-- BODY -->",
                html);

            Assert.DoesNotContain(
                "<!-- IMAGE -->",
                html);

            Assert.DoesNotContain(
                "<!-- ANNOUNCEMENTS -->",
                html);

            Assert.DoesNotContain(
                "<!-- REQUEST -->",
                html);

            Assert.DoesNotContain(
                "<!-- ATTACHMENTS -->",
                html);

            Assert.DoesNotContain(
                "<!-- SIGNATURE -->",
                html);

            Assert.DoesNotContain(
                "<!-- FOOTER -->",
                html);
        }


        // =========================================================
        // HELPERS
        // =========================================================

        private static EmailType CreateEmail(
            string subject,
            string organizationName =
                "UWGB UREC",
            string headerLabel =
                "CLIMBING TOWER",
            string heading =
                "Test Email",
            string subtitle =
                "Test Subtitle",
            string greeting =
                "Hello Tower Team,",
            string body =
                "This is a test email.")
        {
            List<IEmailInputs> inputs =
                new List<IEmailInputs>
                {
                    new EmailDetailsInputs(
                        subject),

                    new CustomHeaderInputs(
                        organizationName,
                        headerLabel,
                        heading,
                        subtitle,
                        "https://example.com/header.png"),

                    new CustomBodyInputs(
                        greeting,
                        body)
                };

            return new EmailType(
                "Test Email",
                inputs,
                true);
        }
    }
}