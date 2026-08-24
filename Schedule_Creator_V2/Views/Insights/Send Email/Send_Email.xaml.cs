using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Constants;
using Schedule_Creator_V2.Models.Defaults;
using Schedule_Creator_V2.Models.Interfaces;
using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Services.Email;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Schedule_Creator_V2
{
    public partial class Send_Email : Page
    {
        private EmailType? currentEmailType;
        private static readonly List<EmailSectionOption>
            OptionalEmailSections =
            new List<EmailSectionOption>
            {
                new EmailSectionOption(
                    DisplayName: "Image",
                    InputType:
                        typeof(CustomImageInputs),
                    CreateInput: () =>
                        new CustomImageInputs(
                            ImageSource:
                                "",
                            ImageAltText:
                                ""
                        )
                ),
                new EmailSectionOption(
                    DisplayName: "Announcements",
                    InputType:
                        typeof(CustomAnnouncementsInputs),
                    CreateInput: () =>
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
                    DisplayName: "Request",
                    InputType:
                        typeof(CustomRequestInputs),
                    CreateInput: () =>
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
                    DisplayName: "Attachments",
                    InputType:
                        typeof(CustomAttachmentsInputs),
                    CreateInput: () =>
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

        public List<EmailRecipientSelection>
            RecipientSelections
        {
            get;
            private set;
        }

        public List<EmailType>
            EmailTypes
        {
            get;
            private set;
        }

        public Send_Email()
        {
            InitializeComponent();

            RecipientSelections =
                LoadStaffSafely()
                    .Select(staff =>
                        new EmailRecipientSelection(
                            staff))
                    .ToList();

            EmailTypes =
                EmailTypeService
                    .CreateEmailTypes();

            DataContext =
                this;

            EmailSectionManagerBorder.Visibility =
                Visibility.Collapsed;

            EmailInputFormService
                .ShowNoEmailTypeSelectedMessage(
                    EmailInputFieldsPanel);
        }

        // =========================================================
        // EMAIL TYPE SELECTION
        // =========================================================

        private void EmailTypeComboBox_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            /*
             * Save changes made to the previous email type
             * before replacing the generated controls.
             */
            if (currentEmailType is not null)
            {
                EmailInputFormService.ApplyInputValues(
                    EmailInputFieldsPanel);
            }

            if (EmailTypeComboBox.SelectedItem
                is not EmailType selectedEmailType)
            {
                currentEmailType =
                    null;

                EmailSectionManagerBorder.Visibility =
                    Visibility.Collapsed;

                EmailInputFormService
                    .ShowNoEmailTypeSelectedMessage(
                        EmailInputFieldsPanel);

                return;
            }

            currentEmailType =
                selectedEmailType;

            if (IsCustomEmailType(
                    currentEmailType))
            {
                EnsureRequiredCustomSections(
                    currentEmailType);
            }

            RebuildEmailEditor();
        }

        // =========================================================
        // CUSTOM EMAIL SECTION MANAGER
        // =========================================================

        private void RebuildEmailEditor()
        {
            if (currentEmailType is null)
            {
                EmailSectionManagerBorder.Visibility =
                    Visibility.Collapsed;

                EmailInputFormService
                    .ShowNoEmailTypeSelectedMessage(
                        EmailInputFieldsPanel);

                return;
            }

            SortEmailSections(
                currentEmailType);

            EmailInputFormService
                .BuildEmailInputControls(
                    EmailInputFieldsPanel,
                    currentEmailType);

            bool isCustomEmail =
                IsCustomEmailType(
                    currentEmailType);

            EmailSectionManagerBorder.Visibility =
                isCustomEmail
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            if (isCustomEmail)
            {
                RefreshSectionManager();
            }
            else
            {
                EmailSectionComboBox.ItemsSource =
                    null;

                RemoveEmailSectionComboBox.ItemsSource =
                    null;
            }
        }

        private void RefreshSectionManager()
        {
            if (currentEmailType is null ||
                !IsCustomEmailType(
                    currentEmailType))
            {
                EmailSectionComboBox.ItemsSource =
                    null;

                RemoveEmailSectionComboBox.ItemsSource =
                    null;

                AddEmailSectionButton.IsEnabled =
                    false;

                RemoveEmailSectionButton.IsEnabled =
                    false;

                return;
            }

            // ---------------------------------------------
            // Sections that are not currently in the email
            // ---------------------------------------------

            List<EmailSectionOption>
                availableSections =
                    OptionalEmailSections
                        .Where(option =>
                            !currentEmailType.inputs
                                .Any(input =>
                                    option.InputType
                                        .IsInstanceOfType(
                                            input)))
                        .ToList();

            EmailSectionComboBox.ItemsSource =
                availableSections;

            EmailSectionComboBox.SelectedIndex =
                availableSections.Count > 0
                    ? 0
                    : -1;

            AddEmailSectionButton.IsEnabled =
                availableSections.Count > 0;

            // ---------------------------------------------
            // Sections that may currently be removed
            // ---------------------------------------------

            List<ActiveEmailSection>
                removableSections =
                    currentEmailType.inputs
                        .Where(IsRemovableSection)
                        .Select(input =>
                            new ActiveEmailSection(
                                GetSectionDisplayName(
                                    input),
                                input))
                        .ToList();

            RemoveEmailSectionComboBox.ItemsSource =
                removableSections;

            RemoveEmailSectionComboBox.SelectedIndex =
                removableSections.Count > 0
                    ? 0
                    : -1;

            RemoveEmailSectionButton.IsEnabled =
                removableSections.Count > 0;
        }

        private void AddEmailSectionButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (currentEmailType is null ||
                !IsCustomEmailType(
                    currentEmailType))
            {
                return;
            }

            if (EmailSectionComboBox.SelectedItem
                is not EmailSectionOption selectedSection)
            {
                return;
            }

            /*
             * Save everything the user already typed before
             * destroying and rebuilding the form controls.
             */
            EmailInputFormService.ApplyInputValues(
                EmailInputFieldsPanel);

            bool alreadyExists =
                currentEmailType.inputs
                    .Any(input =>
                        selectedSection
                            .InputType
                            .IsInstanceOfType(
                                input));

            if (alreadyExists)
            {
                MessageBox.Show(
                    "That section is already included in this email.",
                    "Section Already Added",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                RefreshSectionManager();

                return;
            }

            IEmailInputs newSection =
                selectedSection
                    .CreateInput();

            currentEmailType.inputs.Add(
                newSection);

            SortEmailSections(
                currentEmailType);

            RebuildEmailEditor();
        }

        private void RemoveEmailSectionButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (currentEmailType is null ||
                !IsCustomEmailType(
                    currentEmailType))
            {
                return;
            }

            if (RemoveEmailSectionComboBox.SelectedItem
                is not ActiveEmailSection selectedSection)
            {
                return;
            }

            /*
             * Save current form values before rebuilding
             * the dynamic controls.
             */
            EmailInputFormService.ApplyInputValues(
                EmailInputFieldsPanel);

            if (!IsRemovableSection(
                    selectedSection.Input))
            {
                MessageBox.Show(
                    "Header, body, signature, and footer " +
                    "cannot be removed.",
                    "Required Email Section",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            currentEmailType.inputs.Remove(
                selectedSection.Input);

            SortEmailSections(
                currentEmailType);

            RebuildEmailEditor();
        }

        // =========================================================
        // REQUIRED CUSTOM SECTIONS
        // =========================================================

        private static void EnsureRequiredCustomSections(
            EmailType emailType)
        {
            ArgumentNullException.ThrowIfNull(
                emailType);

            /*
             * Email Details is kept because the subject field
             * belongs to the overall email rather than being an
             * optional content section.
             */
            if (!emailType.inputs
                    .OfType<EmailDetailsInputs>()
                    .Any())
            {
                emailType.inputs.Insert(
                    0,
                    new EmailDetailsInputs(
                        Subject: ""));
            }

            // Header
            if (!emailType.inputs
                    .OfType<CustomHeaderInputs>()
                    .Any())
            {
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

            // Body
            if (!emailType.inputs
                    .OfType<CustomBodyInputs>()
                    .Any())
            {
                emailType.inputs.Add(
                    new CustomBodyInputs(
                        RecipientGreeting:
                            EmailInputConstants
                                .TowerTeamGreeting,

                        EmailBody:
                            ""
                    ));
            }

            // Signature
            if (!emailType.inputs
                    .OfType<CustomSignatureInputs>()
                    .Any())
            {
                emailType.inputs.Add(
                    EmailInputDefaults
                        .DefaultSignatureInputs);
            }

            // Footer
            if (!emailType.inputs
                    .OfType<CustomFooterInputs>()
                    .Any())
            {
                emailType.inputs.Add(
                    CreateDefaultFooterInputs());
            }

            SortEmailSections(
                emailType);
        }

        private static CustomFooterInputs
            CreateDefaultFooterInputs()
        {
            return new CustomFooterInputs(
                FooterOrganization:
                    EmailInputConstants
                        .OrganizationName,

                FooterWebsiteLink:
                    "https://www.uwgb.edu/urec/",

                FooterWebsiteUrl:
                    "https://www.uwgb.edu/urec/",

                FooterText:
                    "University Recreation",

                FooterLogoSource:
                    EmailImageSources
                        .Default_FooterImage
            );
        }

        // =========================================================
        // SECTION RULES
        // =========================================================

        private static bool IsCustomEmailType(
            EmailType emailType)
        {
            return string.Equals(
                emailType.displayName,
                "Custom",
                StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsRemovableSection(IEmailInputs input)
        {
            return input is
                CustomImageInputs or
                CustomAnnouncementsInputs or
                CustomRequestInputs or
                CustomAttachmentsInputs;
        }

        private static string GetSectionDisplayName(
    IEmailInputs input)
        {
            return input switch
            {
                CustomImageInputs =>
                    "Image",

                CustomAnnouncementsInputs =>
                    "Announcements",

                CustomRequestInputs =>
                    "Request",

                CustomAttachmentsInputs =>
                    "Attachments",

                CustomHeaderInputs =>
                    "Header",

                CustomBodyInputs =>
                    "Body",

                CustomSignatureInputs =>
                    "Signature",

                CustomFooterInputs =>
                    "Footer",

                EmailDetailsInputs =>
                    "Email Details",

                _ =>
                    input.GetEmailTypeName()
            };
        }

        private static void SortEmailSections(
            EmailType emailType)
        {
            emailType.inputs.Sort(
                (left, right) =>
                    GetSectionOrder(left)
                        .CompareTo(
                            GetSectionOrder(right)));
        }

        private static int GetSectionOrder(IEmailInputs input)
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

        // =========================================================
        // RECIPIENT CONTROLS
        // =========================================================

        private void SelectAllStaffButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            foreach (EmailRecipientSelection recipient
                     in RecipientSelections)
            {
                recipient.IsTo =
                    true;
            }
        }

        private void ClearStaffSelectionButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            foreach (EmailRecipientSelection recipient
                     in RecipientSelections)
            {
                recipient.Clear();
            }
        }

        // =========================================================
        // PREVIEW
        // =========================================================

        private void PreviewEmailButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            bool hasValidData =
                TryGetEmailData(
                    actionDescription:
                        "previewing",

                    out EmailType selectedEmailType,
                    out _,
                    out _);

            if (!hasValidData)
            {
                return;
            }

            if (!EmailValidationService
                    .ValidateRequiredFields(
                        EmailInputFieldsPanel,
                        out string validationMessage))
            {
                MessageBox.Show(
                    validationMessage,
                    "Required Fields Missing",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            try
            {
                EmailInputFormService
                    .ApplyInputValues(
                        EmailInputFieldsPanel);

                string subject =
                    EmailContentService
                        .BuildSubject(
                            selectedEmailType);

                string htmlBody =
                    EmailContentService
                        .BuildHtmlBody(
                            selectedEmailType);

                EmailPreviewWindow previewWindow =
                    new EmailPreviewWindow(
                        subject,
                        htmlBody)
                    {
                        Owner =
                            Window.GetWindow(
                                this)
                    };

                previewWindow.Show();
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    "The email preview could not be opened.\n\n" +
                    exception.Message,
                    "Email Preview Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // =========================================================
        // GENERATE EMAIL
        // =========================================================

        private void GenerateEmailButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            bool hasValidData =
                TryGetEmailData(
                    actionDescription:
                        "generating",

                    out EmailType selectedEmailType,
                    out List<Staff> toRecipients,
                    out List<Staff> ccRecipients);

            if (!hasValidData)
            {
                return;
            }

            if (!EmailValidationService
                    .ValidateRequiredFields(
                        EmailInputFieldsPanel,
                        out string validationMessage))
            {
                MessageBox.Show(
                    validationMessage,
                    "Required Fields Missing",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            EmailInputFormService
                .ApplyInputValues(
                    EmailInputFieldsPanel);

            Button? generateButton =
                sender as Button;

            try
            {
                if (generateButton is not null)
                {
                    generateButton.IsEnabled =
                        false;
                }

                string subject =
                    EmailContentService
                        .BuildSubject(
                            selectedEmailType);

                string htmlBody =
                    EmailContentService
                        .BuildHtmlBody(
                            selectedEmailType);

                EmlEmailService
                    .CreateAndOpenEmail(
                        toRecipients,
                        ccRecipients,
                        subject,
                        htmlBody);
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    "The Outlook email could not be created.\n\n" +
                    exception.Message,
                    "Outlook Email Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                if (generateButton is not null)
                {
                    generateButton.IsEnabled =
                        true;
                }
            }
        }

        // =========================================================
        // EMAIL DATA / VALIDATION
        // =========================================================

        private bool TryGetEmailData(
            string actionDescription,
            out EmailType emailType,
            out List<Staff> toRecipients,
            out List<Staff> ccRecipients)
        {
            toRecipients =
                new List<Staff>();

            ccRecipients =
                new List<Staff>();

            if (EmailTypeComboBox.SelectedItem
                is not EmailType selectedEmailType)
            {
                MessageBox.Show(
                    "Please select an email type before continuing.",
                    "No Email Type Selected",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                EmailTypeComboBox.Focus();

                EmailTypeComboBox.IsDropDownOpen =
                    true;

                emailType =
                    null!;

                return false;
            }

            toRecipients =
                RecipientSelections
                    .Where(recipient =>
                        recipient.IsTo)
                    .Select(recipient =>
                        recipient.Staff)
                    .ToList();

            ccRecipients =
                RecipientSelections
                    .Where(recipient =>
                        recipient.IsCc)
                    .Select(recipient =>
                        recipient.Staff)
                    .ToList();

            if (toRecipients.Count == 0)
            {
                MessageBox.Show(
                    $"Please select at least one To recipient before " +
                    $"{actionDescription} the email.",
                    "No To Recipients Selected",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                StaffListBox.Focus();

                emailType =
                    null!;

                return false;
            }

            emailType =
                selectedEmailType;

            return true;
        }

        // =========================================================
        // STAFF
        // =========================================================

        private static List<Staff>
            LoadStaffSafely()
        {
            try
            {
                return EmailStaffService
                    .LoadStaff();
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    "The staff list could not be loaded.\n\n" +
                    exception.Message,
                    "Staff Loading Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return new List<Staff>();
            }
        }

        // =========================================================
        // INTERNAL SECTION MODELS
        // =========================================================

        private sealed record EmailSectionOption(
            string DisplayName,
            Type InputType,
            Func<IEmailInputs> CreateInput
        );

        private sealed record ActiveEmailSection(
            string DisplayName,
            IEmailInputs Input
        );
    }

    // =============================================================
    // RECIPIENT SELECTION MODEL
    // =============================================================

    public sealed class EmailRecipientSelection :
        INotifyPropertyChanged
    {
        private bool isTo;
        private bool isCc;

        public Staff Staff
        {
            get;
        }

        public bool IsTo
        {
            get => isTo;

            set
            {
                if (isTo == value)
                {
                    return;
                }

                isTo =
                    value;

                OnPropertyChanged();

                if (value &&
                    isCc)
                {
                    isCc =
                        false;

                    OnPropertyChanged(
                        nameof(IsCc));
                }
            }
        }

        public bool IsCc
        {
            get => isCc;

            set
            {
                if (isCc == value)
                {
                    return;
                }

                isCc =
                    value;

                OnPropertyChanged();

                if (value &&
                    isTo)
                {
                    isTo =
                        false;

                    OnPropertyChanged(
                        nameof(IsTo));
                }
            }
        }

        public EmailRecipientSelection(
            Staff staff)
        {
            ArgumentNullException.ThrowIfNull(
                staff);

            Staff =
                staff;
        }

        public void Clear()
        {
            IsTo =
                false;

            IsCc =
                false;
        }

        public event PropertyChangedEventHandler?
            PropertyChanged;

        private void OnPropertyChanged(
            [CallerMemberName]
            string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(
                    propertyName));
        }
    }
}