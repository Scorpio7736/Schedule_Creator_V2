using Schedule_Creator_V2.Models.Constants;
using Schedule_Creator_V2.Models.Defaults;
using Schedule_Creator_V2.Models.Interfaces;
using Schedule_Creator_V2.Models.Records;

namespace Schedule_Creator_V2.Services.Email
{
    public sealed record EmailSectionOption(
        string DisplayName,
        Type InputType,
        Func<IEmailInputs> CreateInput
    );

    public sealed record ActiveEmailSection(
        string DisplayName,
        IEmailInputs Input
    );

    public static class EmailSectionService
    {
        // =========================================================
        // OPTIONAL SECTIONS
        // =========================================================

        private static readonly
            IReadOnlyList<EmailSectionOption>
            OptionalSections =
                new List<EmailSectionOption>
                {
                    new EmailSectionOption(
                        DisplayName:
                            "Image",

                        InputType:
                            typeof(CustomImageInputs),

                        CreateInput:
                            () =>
                                new CustomImageInputs(
                                    ImageSource:
                                        "",

                                    ImageAltText:
                                        ""
                                )
                    ),

                    new EmailSectionOption(
                        DisplayName:
                            "Announcements",

                        InputType:
                            typeof(
                                CustomAnnouncementsInputs),

                        CreateInput:
                            () =>
                                new CustomAnnouncementsInputs(
                                    AnnouncementsLabel:
                                        "ANNOUNCEMENTS",

                                    AnnouncementsIntro:
                                        "",

                                    AnnouncementsList:
                                        new List<string>()
                                )
                    ),

                    new EmailSectionOption(
                        DisplayName:
                            "Request",

                        InputType:
                            typeof(
                                CustomRequestInputs),

                        CreateInput:
                            () =>
                                new CustomRequestInputs(
                                    RequestLabel:
                                        "REQUEST",

                                    RequestTitle:
                                        "",

                                    RequestBody:
                                        "",

                                    RequestButton:
                                        "",

                                    RequestLink:
                                        ""
                                )
                    ),

                    new EmailSectionOption(
                        DisplayName:
                            "Attachments",

                        InputType:
                            typeof(
                                CustomAttachmentsInputs),

                        CreateInput:
                            () =>
                                new CustomAttachmentsInputs(
                                    AttachmentsLabel:
                                        "ATTACHMENTS",

                                    AttachmentsIntro:
                                        "",

                                    AttachmentsList:
                                        new List<string>()
                                )
                    )
                };


        // =========================================================
        // SECTION EDITING
        // =========================================================

        public static bool CanEditSections(
            EmailType emailType)
        {
            ArgumentNullException.ThrowIfNull(
                emailType);

            return emailType.allowSectionEditing;
        }


        // =========================================================
        // REQUIRED SECTIONS
        // =========================================================

        public static void EnsureRequiredSections(
            EmailType emailType)
        {
            ArgumentNullException.ThrowIfNull(
                emailType);

            EnsureEmailDetails(
                emailType);

            EnsureHeader(
                emailType);

            EnsureBody(
                emailType);

            EnsureSignature(
                emailType);

            EnsureFooter(
                emailType);

            SortSections(
                emailType);
        }

        private static void EnsureEmailDetails(
            EmailType emailType)
        {
            if (emailType.inputs
                .OfType<EmailDetailsInputs>()
                .Any())
            {
                return;
            }

            emailType.inputs.Insert(
                0,
                new EmailDetailsInputs(
                    Subject:
                        ""));
        }

        private static void EnsureHeader(
            EmailType emailType)
        {
            if (emailType.inputs
                .OfType<CustomHeaderInputs>()
                .Any())
            {
                return;
            }

            emailType.inputs.Add(
                new CustomHeaderInputs(
                    OrganizationName:
                        EmailInputConstants
                            .OrganizationName,

                    HeaderLabel:
                        "",

                    EmailHeading:
                        "",

                    HeaderSubtitle:
                        "",

                    HeaderImageUrl:
                        EmailImageSources
                            .Default_HeaderImage
                ));
        }

        private static void EnsureBody(
            EmailType emailType)
        {
            if (emailType.inputs
                .OfType<CustomBodyInputs>()
                .Any())
            {
                return;
            }

            emailType.inputs.Add(
                new CustomBodyInputs(
                    RecipientGreeting:
                        EmailInputConstants
                            .TowerTeamGreeting,

                    EmailBody:
                        ""
                ));
        }

        private static void EnsureSignature(
            EmailType emailType)
        {
            if (emailType.inputs
                .OfType<CustomSignatureInputs>()
                .Any())
            {
                return;
            }

            emailType.inputs.Add(
                EmailInputDefaults
                    .DefaultSignatureInputs);
        }

        private static void EnsureFooter(
            EmailType emailType)
        {
            if (emailType.inputs
                .OfType<CustomFooterInputs>()
                .Any())
            {
                return;
            }

            emailType.inputs.Add(
                EmailInputDefaults
                    .DefaultFooterInputs);
        }


        // =========================================================
        // AVAILABLE OPTIONAL SECTIONS
        // =========================================================

        public static IReadOnlyList<EmailSectionOption>
            GetAvailableSections(
                EmailType emailType)
        {
            ArgumentNullException.ThrowIfNull(
                emailType);

            return OptionalSections
                .Where(option =>
                    !emailType.inputs.Any(input =>
                        option.InputType
                            .IsInstanceOfType(
                                input)))
                .ToList();
        }


        // =========================================================
        // REMOVABLE SECTIONS
        // =========================================================

        public static IReadOnlyList<ActiveEmailSection>
            GetRemovableSections(
                EmailType emailType)
        {
            ArgumentNullException.ThrowIfNull(
                emailType);

            return emailType.inputs
                .Where(
                    CanRemove)
                .Select(input =>
                    new ActiveEmailSection(
                        GetDisplayName(
                            input),

                        input))
                .ToList();
        }

        public static bool CanRemove(
            IEmailInputs input)
        {
            ArgumentNullException.ThrowIfNull(
                input);

            return input is
                CustomImageInputs or
                CustomAnnouncementsInputs or
                CustomRequestInputs or
                CustomAttachmentsInputs;
        }


        // =========================================================
        // ADD SECTION
        // =========================================================

        public static bool AddSection(
            EmailType emailType,
            EmailSectionOption option)
        {
            ArgumentNullException.ThrowIfNull(
                emailType);

            ArgumentNullException.ThrowIfNull(
                option);

            if (!CanEditSections(
                    emailType))
            {
                return false;
            }

            bool alreadyExists =
                emailType.inputs.Any(input =>
                    option.InputType
                        .IsInstanceOfType(
                            input));

            if (alreadyExists)
            {
                return false;
            }

            emailType.inputs.Add(
                option.CreateInput());

            SortSections(
                emailType);

            return true;
        }


        // =========================================================
        // REMOVE SECTION
        // =========================================================

        public static bool RemoveSection(
            EmailType emailType,
            IEmailInputs input)
        {
            ArgumentNullException.ThrowIfNull(
                emailType);

            ArgumentNullException.ThrowIfNull(
                input);

            if (!CanEditSections(
                    emailType))
            {
                return false;
            }

            if (!CanRemove(
                    input))
            {
                return false;
            }

            bool removed =
                emailType.inputs.Remove(
                    input);

            if (removed)
            {
                SortSections(
                    emailType);
            }

            return removed;
        }


        // =========================================================
        // DISPLAY NAME
        // =========================================================

        public static string GetDisplayName(
            IEmailInputs input)
        {
            ArgumentNullException.ThrowIfNull(
                input);

            return input switch
            {
                EmailDetailsInputs =>
                    "Email Details",

                CustomHeaderInputs =>
                    "Header",

                CustomBodyInputs =>
                    "Body",

                CustomImageInputs =>
                    "Image",

                CustomAnnouncementsInputs =>
                    "Announcements",

                CustomRequestInputs =>
                    "Request",

                CustomAttachmentsInputs =>
                    "Attachments",

                CustomSignatureInputs =>
                    "Signature",

                CustomFooterInputs =>
                    "Footer",

                _ =>
                    input.GetEmailTypeName()
            };
        }


        // =========================================================
        // SORTING
        // =========================================================

        public static void SortSections(
            EmailType emailType)
        {
            ArgumentNullException.ThrowIfNull(
                emailType);

            emailType.inputs.Sort(
                (left, right) =>
                    GetSectionOrder(left)
                        .CompareTo(
                            GetSectionOrder(right)));
        }

        private static int GetSectionOrder(
            IEmailInputs input)
        {
            return input switch
            {
                EmailDetailsInputs =>
                    0,

                CustomHeaderInputs =>
                    10,

                CustomBodyInputs =>
                    20,

                CustomImageInputs =>
                    30,

                CustomAnnouncementsInputs =>
                    40,

                CustomRequestInputs =>
                    50,

                CustomAttachmentsInputs =>
                    60,

                CustomSignatureInputs =>
                    70,

                CustomFooterInputs =>
                    80,

                _ =>
                    100
            };
        }
    }
}