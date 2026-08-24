using System;
using System.Windows;

namespace Schedule_Creator_V2
{
    public partial class EmailPreviewWindow : Window
    {
        private readonly string htmlContent;

        public EmailPreviewWindow(
            string subject,
            string htmlContent)
        {
            InitializeComponent();

            ArgumentNullException.ThrowIfNull(htmlContent);

            this.htmlContent = htmlContent;

            SubjectTextBlock.Text =
                string.IsNullOrWhiteSpace(subject)
                    ? "No subject"
                    : subject;

            Title =
                string.IsNullOrWhiteSpace(subject)
                    ? "Email Preview"
                    : $"Email Preview - {subject}";

            Loaded += EmailPreviewWindow_Loaded;
        }

        private void EmailPreviewWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            EmailPreviewBrowser.NavigateToString(
                htmlContent);
        }

        private void CloseButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            Close();
        }
    }
}