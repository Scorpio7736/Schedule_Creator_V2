using Schedule_Creator_V2.Models.Interfaces;
using Schedule_Creator_V2.Models.Records;

namespace Schedule_Creator_V2.Services.Email
{
    public sealed record EmailSectionOption(
        string DisplayName,
        Type InputType,
        Func<IEmailInputs> CreateInput
    );

    public static class EmailSectionService
    {
        private static readonly IReadOnlyList<EmailSectionOption>
            OptionalSections =
            new List<EmailSectionOption>
            {
                new EmailSectionOption(
                    DisplayName: "Announcements",
                    InputType:
                        typeof(CustomAnnouncementsInputs),
                    CreateInput: () =>
                        new CustomAnnouncementsInputs(
                            AnnouncementsLabel:
                                "ANNOUNCEMENTS",
                            AnnouncementsIntro: "",
                            AnnouncementsList:
                                new List<string>()
                        )
                ),

                new EmailSectionOption(
                    DisplayName: "Request",
                    InputType:
                        typeof(CustomRequestInputs),
                    CreateInput: () =>
                        new CustomRequestInputs(
                            RequestLabel: "REQUEST",
                            RequestTitle: "",
                            RequestBody: "",
                            RequestButton: "",
                            RequestLink: ""
                        )
                ),

                new EmailSectionOption(
                    DisplayName: "Attachments",
                    InputType:
                        typeof(CustomAttachmentsInputs),
                    CreateInput: () =>
                        new CustomAttachmentsInputs(
                            AttachmentsLabel:
                                "ATTACHMENTS",
                            AttachmentsIntro: "",
                            AttachmentsList:
                                new List<string>()
                        )
                )
            };

        public static IReadOnlyList<EmailSectionOption>
            GetAvailableSections(
                EmailType emailType)
        {
            ArgumentNullException.ThrowIfNull(emailType);

            return OptionalSections
                .Where(option =>
                    !emailType.inputs.Any(input =>
                        option.InputType.IsInstanceOfType(
                            input)))
                .ToList();
        }

        public static bool AddSection(
            EmailType emailType,
            EmailSectionOption option)
        {
            ArgumentNullException.ThrowIfNull(emailType);
            ArgumentNullException.ThrowIfNull(option);

            bool alreadyExists =
                emailType.inputs.Any(input =>
                    option.InputType.IsInstanceOfType(
                        input));

            if (alreadyExists)
            {
                return false;
            }

            emailType.inputs.Add(
                option.CreateInput());

            SortSections(emailType);

            return true;
        }

        public static bool CanRemove(
            IEmailInputs input)
        {
            return input is
                CustomAnnouncementsInputs or
                CustomRequestInputs or
                CustomAttachmentsInputs;
        }

        public static bool RemoveSection(
            EmailType emailType,
            IEmailInputs input)
        {
            ArgumentNullException.ThrowIfNull(emailType);
            ArgumentNullException.ThrowIfNull(input);

            if (!CanRemove(input))
            {
                return false;
            }

            return emailType.inputs.Remove(input);
        }

        private static void SortSections(
            EmailType emailType)
        {
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
                EmailDetailsInputs => 0,
                CustomHeaderInputs => 10,
                CustomBodyInputs => 20,
                CustomAnnouncementsInputs => 30,
                CustomRequestInputs => 40,
                CustomAttachmentsInputs => 50,
                CustomSignatureInputs => 60,
                CustomFooterInputs => 70,
                _ => int.MaxValue
            };
        }
    }
}