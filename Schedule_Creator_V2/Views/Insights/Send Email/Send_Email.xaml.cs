using Schedule_Creator_V2.Models;
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


        // =========================================================
        // PUBLIC DATA
        // =========================================================

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


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

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
                EmailInputFormService
                    .ApplyInputValues(
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

            /*
             * Email types that allow section editing must
             * always contain their required core sections.
             */
            if (EmailSectionService
                    .CanEditSections(
                        currentEmailType))
            {
                EmailSectionService
                    .EnsureRequiredSections(
                        currentEmailType);
            }

            RebuildEmailEditor();
        }


        // =========================================================
        // EMAIL EDITOR
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

            /*
             * Keep every email section in the canonical order
             * defined by EmailSectionService.
             */
            EmailSectionService
                .SortSections(
                    currentEmailType);

            EmailInputFormService
                .BuildEmailInputControls(
                    EmailInputFieldsPanel,
                    currentEmailType);

            bool canEditSections =
                EmailSectionService
                    .CanEditSections(
                        currentEmailType);

            EmailSectionManagerBorder.Visibility =
                canEditSections
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            if (canEditSections)
            {
                RefreshSectionManager();
            }
            else
            {
                EmailSectionComboBox.ItemsSource =
                    null;

                RemoveEmailSectionComboBox.ItemsSource =
                    null;

                AddEmailSectionButton.IsEnabled =
                    false;

                RemoveEmailSectionButton.IsEnabled =
                    false;
            }
        }


        // =========================================================
        // CUSTOM EMAIL SECTION MANAGER
        // =========================================================

        private void RefreshSectionManager()
        {
            if (currentEmailType is null ||
                !EmailSectionService
                    .CanEditSections(
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

            // -----------------------------------------------------
            // Sections that are not currently in the email
            // -----------------------------------------------------

            IReadOnlyList<EmailSectionOption>
                availableSections =
                    EmailSectionService
                        .GetAvailableSections(
                            currentEmailType);

            EmailSectionComboBox.ItemsSource =
                availableSections;

            EmailSectionComboBox.SelectedIndex =
                availableSections.Count > 0
                    ? 0
                    : -1;

            AddEmailSectionButton.IsEnabled =
                availableSections.Count > 0;


            // -----------------------------------------------------
            // Sections that may currently be removed
            // -----------------------------------------------------

            IReadOnlyList<ActiveEmailSection>
                removableSections =
                    EmailSectionService
                        .GetRemovableSections(
                            currentEmailType);

            RemoveEmailSectionComboBox.ItemsSource =
                removableSections;

            RemoveEmailSectionComboBox.SelectedIndex =
                removableSections.Count > 0
                    ? 0
                    : -1;

            RemoveEmailSectionButton.IsEnabled =
                removableSections.Count > 0;
        }


        // =========================================================
        // ADD EMAIL SECTION
        // =========================================================

        private void AddEmailSectionButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (currentEmailType is null ||
                !EmailSectionService
                    .CanEditSections(
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
             * destroying and rebuilding the dynamic controls.
             */
            EmailInputFormService
                .ApplyInputValues(
                    EmailInputFieldsPanel);

            bool added =
                EmailSectionService
                    .AddSection(
                        currentEmailType,
                        selectedSection);

            if (!added)
            {
                MessageBox.Show(
                    "That section is already included in this email.",
                    "Section Already Added",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                RefreshSectionManager();

                return;
            }

            RebuildEmailEditor();
        }


        // =========================================================
        // REMOVE EMAIL SECTION
        // =========================================================

        private void RemoveEmailSectionButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (currentEmailType is null ||
                !EmailSectionService
                    .CanEditSections(
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
            EmailInputFormService
                .ApplyInputValues(
                    EmailInputFieldsPanel);

            bool removed =
                EmailSectionService
                    .RemoveSection(
                        currentEmailType,
                        selectedSection.Input);

            if (!removed)
            {
                MessageBox.Show(
                    "Header, body, signature, footer, and " +
                    "email details cannot be removed.",
                    "Required Email Section",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            RebuildEmailEditor();
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
    }


    // =============================================================
    // RECIPIENT SELECTION MODEL
    // =============================================================

    public sealed class EmailRecipientSelection :
        INotifyPropertyChanged
    {
        private bool isTo;
        private bool isCc;


        // =========================================================
        // STAFF
        // =========================================================

        public Staff Staff
        {
            get;
        }


        // =========================================================
        // TO
        // =========================================================

        public bool IsTo
        {
            get =>
                isTo;

            set
            {
                if (isTo == value)
                {
                    return;
                }

                isTo =
                    value;

                OnPropertyChanged();

                /*
                 * A recipient cannot simultaneously be marked
                 * as both To and Cc.
                 */
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


        // =========================================================
        // CC
        // =========================================================

        public bool IsCc
        {
            get =>
                isCc;

            set
            {
                if (isCc == value)
                {
                    return;
                }

                isCc =
                    value;

                OnPropertyChanged();

                /*
                 * A recipient cannot simultaneously be marked
                 * as both To and Cc.
                 */
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


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public EmailRecipientSelection(
            Staff staff)
        {
            ArgumentNullException.ThrowIfNull(
                staff);

            Staff =
                staff;
        }


        // =========================================================
        // CLEAR
        // =========================================================

        public void Clear()
        {
            IsTo =
                false;

            IsCc =
                false;
        }


        // =========================================================
        // PROPERTY CHANGED
        // =========================================================

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