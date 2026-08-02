using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Schedule_Creator_V2
{
    public partial class Send_Email : Page
    {
        public List<Staff> StaffMembers { get; private set; }

        public List<EmailType> EmailTypes { get; private set; }

        public Send_Email()
        {
            InitializeComponent();

            StaffMembers =
                LoadStaffSafely();

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
            StaffListBox.SelectAll();
        }

        private void ClearStaffSelectionButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            StaffListBox.UnselectAll();
        }

        private void PreviewEmailButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            bool hasValidData =
                TryGetEmailData(
                    actionDescription: "previewing",
                    out EmailType selectedEmailType,
                    out List<Staff> selectedStaff);

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

            string subject =
                EmailContentService.BuildSubject(
                    selectedEmailType);

            MessageBox.Show(
                $"Subject: {subject}\n" +
                $"Selected staff: {selectedStaff.Count}\n\n" +
                "The email is ready to generate.",
                "Preview Email",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void GenerateEmailButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            bool hasValidData =
                TryGetEmailData(
                    actionDescription: "generating",
                    out EmailType selectedEmailType,
                    out List<Staff> selectedStaff);

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

            string subject =
                EmailContentService.BuildSubject(
                    selectedEmailType);

            string emailBody =
                EmailContentService.BuildPlainTextBody(
                    selectedEmailType);

            try
            {
                OutlookEmailService.OpenNewEmail(
                    selectedStaff,
                    subject,
                    emailBody);
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    $"The Outlook email could not be created.\n\n" +
                    exception.Message,
                    "Outlook Email Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private bool TryGetEmailData(
            string actionDescription,
            out EmailType emailType,
            out List<Staff> selectedStaff)
        {
            selectedStaff =
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

            selectedStaff =
                StaffListBox.SelectedItems
                    .Cast<Staff>()
                    .ToList();

            if (selectedStaff.Count == 0)
            {
                MessageBox.Show(
                    $"Please select at least one staff member before {actionDescription} the email.",
                    "No Staff Selected",
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
                    $"The staff list could not be loaded.\n\n" +
                    exception.Message,
                    "Staff Loading Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return new List<Staff>();
            }
        }
    }
}