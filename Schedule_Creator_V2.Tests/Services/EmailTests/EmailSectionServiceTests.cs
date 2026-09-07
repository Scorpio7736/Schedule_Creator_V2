using Schedule_Creator_V2.Models.Interfaces;
using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Services.Email;

namespace Schedule_Creator_V2.Tests.Services.Email
{
    public sealed class EmailSectionServiceTests
    {
        // =========================================================
        // CAN EDIT
        // =========================================================

        [Fact]
        public void CanEditSections_EditingEnabled_ReturnsTrue()
        {
            // Arrange

            EmailType emailType =
                CreateEmailType(
                    true);


            // Act

            bool result =
                EmailSectionService
                    .CanEditSections(
                        emailType);


            // Assert

            Assert.True(
                result);
        }


        [Fact]
        public void CanEditSections_EditingDisabled_ReturnsFalse()
        {
            // Arrange

            EmailType emailType =
                CreateEmailType(
                    false);


            // Act

            bool result =
                EmailSectionService
                    .CanEditSections(
                        emailType);


            // Assert

            Assert.False(
                result);
        }


        // =========================================================
        // REQUIRED SECTIONS
        // =========================================================

        [Fact]
        public void EnsureRequiredSections_EmptyEmail_AddsRequiredSections()
        {
            // Arrange

            EmailType emailType =
                CreateEmailType(
                    true);


            // Act

            EmailSectionService
                .EnsureRequiredSections(
                    emailType);


            // Assert

            Assert.Contains(
                emailType.inputs,
                input =>
                    input is EmailDetailsInputs);

            Assert.Contains(
                emailType.inputs,
                input =>
                    input is CustomHeaderInputs);

            Assert.Contains(
                emailType.inputs,
                input =>
                    input is CustomBodyInputs);

            Assert.Contains(
                emailType.inputs,
                input =>
                    input is CustomSignatureInputs);

            Assert.Contains(
                emailType.inputs,
                input =>
                    input is CustomFooterInputs);
        }


        [Fact]
        public void EnsureRequiredSections_EmptyEmail_AddsExactlyFiveSections()
        {
            // Arrange

            EmailType emailType =
                CreateEmailType(
                    true);


            // Act

            EmailSectionService
                .EnsureRequiredSections(
                    emailType);


            // Assert

            Assert.Equal(
                5,
                emailType.inputs.Count);
        }


        [Fact]
        public void EnsureRequiredSections_CalledTwice_DoesNotDuplicateSections()
        {
            // Arrange

            EmailType emailType =
                CreateEmailType(
                    true);


            // Act

            EmailSectionService
                .EnsureRequiredSections(
                    emailType);

            EmailSectionService
                .EnsureRequiredSections(
                    emailType);


            // Assert

            Assert.Single(
                emailType.inputs
                    .OfType<EmailDetailsInputs>());

            Assert.Single(
                emailType.inputs
                    .OfType<CustomHeaderInputs>());

            Assert.Single(
                emailType.inputs
                    .OfType<CustomBodyInputs>());

            Assert.Single(
                emailType.inputs
                    .OfType<CustomSignatureInputs>());

            Assert.Single(
                emailType.inputs
                    .OfType<CustomFooterInputs>());
        }


        // =========================================================
        // AVAILABLE OPTIONAL SECTIONS
        // =========================================================

        [Fact]
        public void GetAvailableSections_NoOptionalSections_ReturnsFourOptions()
        {
            // Arrange

            EmailType emailType =
                CreateEmailType(
                    true);

            EmailSectionService
                .EnsureRequiredSections(
                    emailType);


            // Act

            IReadOnlyList<EmailSectionOption> result =
                EmailSectionService
                    .GetAvailableSections(
                        emailType);


            // Assert

            Assert.Equal(
                4,
                result.Count);

            Assert.Contains(
                result,
                option =>
                    option.DisplayName ==
                    "Image");

            Assert.Contains(
                result,
                option =>
                    option.DisplayName ==
                    "Announcements");

            Assert.Contains(
                result,
                option =>
                    option.DisplayName ==
                    "Request");

            Assert.Contains(
                result,
                option =>
                    option.DisplayName ==
                    "Attachments");
        }


        // =========================================================
        // ADD SECTION
        // =========================================================

        [Fact]
        public void AddSection_EditingEnabled_AddsSection()
        {
            // Arrange

            EmailType emailType =
                CreateEmailType(
                    true);

            EmailSectionService
                .EnsureRequiredSections(
                    emailType);

            EmailSectionOption imageOption =
                GetOption(
                    emailType,
                    "Image");


            // Act

            bool result =
                EmailSectionService
                    .AddSection(
                        emailType,
                        imageOption);


            // Assert

            Assert.True(
                result);

            Assert.Single(
                emailType.inputs
                    .OfType<CustomImageInputs>());
        }


        [Fact]
        public void AddSection_EditingDisabled_DoesNotAddSection()
        {
            // Arrange

            EmailType emailType =
                CreateEmailType(
                    false);

            EmailSectionService
                .EnsureRequiredSections(
                    emailType);

            EmailSectionOption imageOption =
                GetOption(
                    emailType,
                    "Image");


            // Act

            bool result =
                EmailSectionService
                    .AddSection(
                        emailType,
                        imageOption);


            // Assert

            Assert.False(
                result);

            Assert.Empty(
                emailType.inputs
                    .OfType<CustomImageInputs>());
        }


        [Fact]
        public void AddSection_SectionAlreadyExists_ReturnsFalse()
        {
            // Arrange

            EmailType emailType =
                CreateEmailType(
                    true);

            EmailSectionService
                .EnsureRequiredSections(
                    emailType);

            EmailSectionOption imageOption =
                GetOption(
                    emailType,
                    "Image");

            EmailSectionService
                .AddSection(
                    emailType,
                    imageOption);


            // Act

            bool result =
                EmailSectionService
                    .AddSection(
                        emailType,
                        imageOption);


            // Assert

            Assert.False(
                result);

            Assert.Single(
                emailType.inputs
                    .OfType<CustomImageInputs>());
        }


        [Fact]
        public void AddSection_AddedSection_NoLongerAppearsAsAvailable()
        {
            // Arrange

            EmailType emailType =
                CreateEmailType(
                    true);

            EmailSectionService
                .EnsureRequiredSections(
                    emailType);

            EmailSectionOption imageOption =
                GetOption(
                    emailType,
                    "Image");


            // Act

            EmailSectionService
                .AddSection(
                    emailType,
                    imageOption);

            IReadOnlyList<EmailSectionOption> available =
                EmailSectionService
                    .GetAvailableSections(
                        emailType);


            // Assert

            Assert.DoesNotContain(
                available,
                option =>
                    option.DisplayName ==
                    "Image");
        }


        // =========================================================
        // REMOVE SECTION
        // =========================================================

        [Fact]
        public void RemoveSection_OptionalSection_RemovesSection()
        {
            // Arrange

            EmailType emailType =
                CreateEmailType(
                    true);

            EmailSectionService
                .EnsureRequiredSections(
                    emailType);

            EmailSectionOption imageOption =
                GetOption(
                    emailType,
                    "Image");

            EmailSectionService
                .AddSection(
                    emailType,
                    imageOption);

            CustomImageInputs image =
                emailType.inputs
                    .OfType<CustomImageInputs>()
                    .Single();


            // Act

            bool result =
                EmailSectionService
                    .RemoveSection(
                        emailType,
                        image);


            // Assert

            Assert.True(
                result);

            Assert.Empty(
                emailType.inputs
                    .OfType<CustomImageInputs>());
        }


        [Fact]
        public void RemoveSection_RequiredSection_DoesNotRemoveSection()
        {
            // Arrange

            EmailType emailType =
                CreateEmailType(
                    true);

            EmailSectionService
                .EnsureRequiredSections(
                    emailType);

            CustomBodyInputs body =
                emailType.inputs
                    .OfType<CustomBodyInputs>()
                    .Single();


            // Act

            bool result =
                EmailSectionService
                    .RemoveSection(
                        emailType,
                        body);


            // Assert

            Assert.False(
                result);

            Assert.Single(
                emailType.inputs
                    .OfType<CustomBodyInputs>());
        }


        [Fact]
        public void RemoveSection_EditingDisabled_DoesNotRemoveOptionalSection()
        {
            // Arrange

            EmailType editableEmail =
                CreateEmailType(
                    true);

            EmailSectionService
                .EnsureRequiredSections(
                    editableEmail);

            EmailSectionOption imageOption =
                GetOption(
                    editableEmail,
                    "Image");

            EmailSectionService
                .AddSection(
                    editableEmail,
                    imageOption);

            CustomImageInputs image =
                editableEmail.inputs
                    .OfType<CustomImageInputs>()
                    .Single();

            EmailType nonEditableEmail =
                new EmailType(
                    "Test",
                    new List<IEmailInputs>
                    {
                        image
                    },
                    false);


            // Act

            bool result =
                EmailSectionService
                    .RemoveSection(
                        nonEditableEmail,
                        image);


            // Assert

            Assert.False(
                result);

            Assert.Contains(
                image,
                nonEditableEmail.inputs);
        }


        // =========================================================
        // CAN REMOVE
        // =========================================================

        [Fact]
        public void CanRemove_Image_ReturnsTrue()
        {
            // Arrange

            EmailType emailType =
                CreateEmailType(
                    true);

            EmailSectionOption option =
                GetOption(
                    emailType,
                    "Image");

            IEmailInputs input =
                option.CreateInput();


            // Act

            bool result =
                EmailSectionService
                    .CanRemove(
                        input);


            // Assert

            Assert.True(
                result);
        }


        [Fact]
        public void CanRemove_Body_ReturnsFalse()
        {
            // Arrange

            EmailType emailType =
                CreateEmailType(
                    true);

            EmailSectionService
                .EnsureRequiredSections(
                    emailType);

            CustomBodyInputs body =
                emailType.inputs
                    .OfType<CustomBodyInputs>()
                    .Single();


            // Act

            bool result =
                EmailSectionService
                    .CanRemove(
                        body);


            // Assert

            Assert.False(
                result);
        }


        // =========================================================
        // DISPLAY NAMES
        // =========================================================

        [Fact]
        public void GetDisplayName_KnownSection_ReturnsFriendlyName()
        {
            // Arrange

            EmailType emailType =
                CreateEmailType(
                    true);

            EmailSectionService
                .EnsureRequiredSections(
                    emailType);

            CustomHeaderInputs header =
                emailType.inputs
                    .OfType<CustomHeaderInputs>()
                    .Single();


            // Act

            string result =
                EmailSectionService
                    .GetDisplayName(
                        header);


            // Assert

            Assert.Equal(
                "Header",
                result);
        }


        // =========================================================
        // SORTING
        // =========================================================

        [Fact]
        public void EnsureRequiredSections_SortsRequiredSections()
        {
            // Arrange

            EmailType emailType =
                CreateEmailType(
                    true);


            // Act

            EmailSectionService
                .EnsureRequiredSections(
                    emailType);


            // Assert

            Assert.IsType<EmailDetailsInputs>(
                emailType.inputs[0]);

            Assert.IsType<CustomHeaderInputs>(
                emailType.inputs[1]);

            Assert.IsType<CustomBodyInputs>(
                emailType.inputs[2]);

            Assert.IsType<CustomSignatureInputs>(
                emailType.inputs[3]);

            Assert.IsType<CustomFooterInputs>(
                emailType.inputs[4]);
        }


        [Fact]
        public void AddSection_Image_IsPlacedBetweenBodyAndSignature()
        {
            // Arrange

            EmailType emailType =
                CreateEmailType(
                    true);

            EmailSectionService
                .EnsureRequiredSections(
                    emailType);

            EmailSectionOption imageOption =
                GetOption(
                    emailType,
                    "Image");


            // Act

            EmailSectionService
                .AddSection(
                    emailType,
                    imageOption);


            // Assert

            Assert.IsType<EmailDetailsInputs>(
                emailType.inputs[0]);

            Assert.IsType<CustomHeaderInputs>(
                emailType.inputs[1]);

            Assert.IsType<CustomBodyInputs>(
                emailType.inputs[2]);

            Assert.IsType<CustomImageInputs>(
                emailType.inputs[3]);

            Assert.IsType<CustomSignatureInputs>(
                emailType.inputs[4]);

            Assert.IsType<CustomFooterInputs>(
                emailType.inputs[5]);
        }


        // =========================================================
        // NULL ARGUMENTS
        // =========================================================

        [Fact]
        public void EnsureRequiredSections_NullEmailType_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                    EmailSectionService
                        .EnsureRequiredSections(
                            null!));
        }


        // =========================================================
        // HELPERS
        // =========================================================

        private static EmailType CreateEmailType(
            bool allowSectionEditing)
        {
            return new EmailType(
                displayName:
                    "Test Email",

                inputs:
                    new List<IEmailInputs>(),

                allowSectionEditing:
                    allowSectionEditing);
        }


        private static EmailSectionOption GetOption(
            EmailType emailType,
            string displayName)
        {
            return EmailSectionService
                .GetAvailableSections(
                    emailType)
                .Single(
                    option =>
                        option.DisplayName ==
                        displayName);
        }
    }
}