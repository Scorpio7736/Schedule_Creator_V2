using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Schedule_Creator_V2
{
    public partial class Send_Email : Page
    {
        public List<EmailRecipientSelection> RecipientSelections
        {
            get;
            private set;
        }

        public List<EmailType> EmailTypes
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
                        new EmailRecipientSelection(staff))
                    .ToList();

            EmailTypes =
                EmailTypeService.CreateEmailTypes();

            DataContext = this;

            EmailInputFormService
                .ShowNoEmailTypeSelectedMessage(
                    EmailInputFieldsPanel);
        }

        private void EmailTypeComboBox_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            if (EmailTypeComboBox.SelectedItem
                is not EmailType selectedEmailType)
            {
                EmailInputFormService
                    .ShowNoEmailTypeSelectedMessage(
                        EmailInputFieldsPanel);

                return;
            }

            EmailInputFormService.BuildEmailInputControls(
                EmailInputFieldsPanel,
                selectedEmailType);
        }

        private void SelectAllStaffButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            foreach (EmailRecipientSelection recipient
                     in RecipientSelections)
            {
                recipient.IsTo = true;
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

        private void PreviewEmailButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            bool hasValidData =
                TryGetEmailData(
                    actionDescription: "previewing",
                    out EmailType selectedEmailType,
                    out _,
                    out _);

            if (!hasValidData)
            {
                return;
            }

            if (!EmailValidationService.ValidateRequiredFields(
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
                EmailInputFormService.ApplyInputValues(
                    EmailInputFieldsPanel);

                string subject =
                    EmailContentService.BuildSubject(
                        selectedEmailType);

                string htmlBody =
                    EmailContentService.BuildHtmlBody(
                        selectedEmailType);

                EmailPreviewWindow previewWindow =
                    new EmailPreviewWindow(
                        subject,
                        htmlBody)
                    {
                        Owner = Window.GetWindow(this)
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

        private void GenerateEmailButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            bool hasValidData =
                TryGetEmailData(
                    actionDescription: "generating",
                    out EmailType selectedEmailType,
                    out List<Staff> toRecipients,
                    out List<Staff> ccRecipients);

            if (!hasValidData)
            {
                return;
            }

            if (!EmailValidationService.ValidateRequiredFields(
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

            EmailInputFormService.ApplyInputValues(
                EmailInputFieldsPanel);

            Button? generateButton =
                sender as Button;

            try
            {
                if (generateButton is not null)
                {
                    generateButton.IsEnabled = false;
                }

                string subject =
                    EmailContentService.BuildSubject(
                        selectedEmailType);

                string htmlBody =
                    EmailContentService.BuildHtmlBody(
                        selectedEmailType);

                EmlEmailService.CreateAndOpenEmail(
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
                    generateButton.IsEnabled = true;
                }
            }
        }

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
                EmailTypeComboBox.IsDropDownOpen = true;

                emailType = null!;

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

                emailType = null!;

                return false;
            }

            emailType =
                selectedEmailType;

            return true;
        }

        private static List<Staff> LoadStaffSafely()
        {
            try
            {
                return EmailStaffService.LoadStaff();
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

                isTo = value;

                OnPropertyChanged();

                if (value && isCc)
                {
                    isCc = false;

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

                isCc = value;

                OnPropertyChanged();

                if (value && isTo)
                {
                    isTo = false;

                    OnPropertyChanged(
                        nameof(IsTo));
                }
            }
        }

        public EmailRecipientSelection(
            Staff staff)
        {
            ArgumentNullException.ThrowIfNull(staff);

            Staff = staff;
        }

        public void Clear()
        {
            IsTo = false;
            IsCc = false;
        }

        public event PropertyChangedEventHandler?
            PropertyChanged;

        private void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(
                    propertyName));
        }
    }
}