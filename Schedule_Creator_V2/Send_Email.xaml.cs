using System.Windows;
using System.Windows.Controls;

namespace Schedule_Creator_V2
    {
        public partial class Send_Email : Page
        {
            public Send_Email()
            {
                InitializeComponent();
            }

            private void EmailTypeComboBox_SelectionChanged(
                object sender,
                SelectionChangedEventArgs e)
            {
                if (!IsLoaded)
                {
                    return;
                }

                NoEmailTypeSelectedPanel.Visibility = Visibility.Collapsed;
                GeneralEmailInputPanel.Visibility = Visibility.Visible;
            }

            private void SelectAllStaffButton_Click(
                object sender,
                RoutedEventArgs e)
            {
                foreach (object child in StaffCheckboxPanel.Children)
                {
                    if (child is CheckBox checkBox)
                    {
                        checkBox.IsChecked = true;
                    }
                }
            }

            private void ClearStaffSelectionButton_Click(
                object sender,
                RoutedEventArgs e)
            {
                foreach (object child in StaffCheckboxPanel.Children)
                {
                    if (child is CheckBox checkBox)
                    {
                        checkBox.IsChecked = false;
                    }
                }
            }

            private void PreviewEmailButton_Click(
                object sender,
                RoutedEventArgs e)
            {
                // Preview functionality will be added later.
                MessageBox.Show(
                    "Email preview functionality has not been implemented yet.",
                    "Preview Email",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }

            private void GenerateEmailButton_Click(
                object sender,
                RoutedEventArgs e)
            {
                // Email generation functionality will be added later.
                MessageBox.Show(
                    "Email generation functionality has not been implemented yet.",
                    "Generate Email",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }
    }