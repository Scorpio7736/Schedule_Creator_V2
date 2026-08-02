using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Interfaces;
using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Schedule_Creator_V2
{
    public partial class Send_Email : Page
    {
        public List<Staff> StaffMembers { get; private set; }

        public List<EmailType> EmailTypes { get; private set; }

        public Send_Email()
        {
            InitializeComponent();

            StaffMembers = new List<Staff>();
            EmailTypes = CreateEmailTypes();

            LoadStaff();

            DataContext = this;

            ShowNoEmailTypeSelectedMessage();
        }

        private List<EmailType> CreateEmailTypes()
        {
            return new List<EmailType>
            {
                new EmailType(
                    displayName: "Test",
                    inputs: new List<IEmailInputs>
                    {
                        new CustomHeaderInputs(
                            OrganizationName: "",
                            HeaderLabel: "",
                            EmailHeading: "",
                            HeaderSubtitle: ""
                        ),

                        new CustomBodyInputs(
                            RecipientGreeting: "",
                            EmailBody: ""
                        )
                    }
                )
            };
        }

        private void LoadStaff()
        {
            try
            {
                StaffMembers =
                    DatabaseRead.ReadStaff() ??
                    new List<Staff>();
            }
            catch (Exception exception)
            {
                StaffMembers = new List<Staff>();

                MessageBox.Show(
                    $"The staff list could not be loaded.\n\n{exception.Message}",
                    "Staff Loading Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void EmailTypeComboBox_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            if (EmailTypeComboBox.SelectedItem is not EmailType selectedEmailType)
            {
                ShowNoEmailTypeSelectedMessage();
                return;
            }

            BuildEmailInputControls(selectedEmailType);
        }

        private void BuildEmailInputControls(EmailType emailType)
        {
            EmailInputFieldsPanel.Children.Clear();

            if (emailType.inputs is null || emailType.inputs.Count == 0)
            {
                ShowNoInputsConfiguredMessage();
                return;
            }

            foreach (IEmailInputs inputGroup in emailType.inputs)
            {
                AddInputGroup(inputGroup);
            }
        }

        private void AddInputGroup(IEmailInputs inputGroup)
        {
            Border groupBorder = new Border
            {
                Margin = new Thickness(0, 0, 0, 16),
                Padding = new Thickness(16),
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush(
                    Color.FromRgb(209, 213, 219)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Tag = inputGroup
            };

            StackPanel groupPanel = new StackPanel();

            TextBlock groupHeading = new TextBlock
            {
                Text = inputGroup.GetEmailTypeName(),
                FontSize = 17,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(
                    Color.FromRgb(31, 41, 55)),
                Margin = new Thickness(0, 0, 0, 12)
            };

            groupPanel.Children.Add(groupHeading);

            PropertyInfo[] properties = inputGroup
                .GetType()
                .GetProperties(
                    BindingFlags.Public |
                    BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType == typeof(string))
                {
                    AddStringInput(
                        groupPanel,
                        inputGroup,
                        property);
                }
                else if (property.PropertyType == typeof(List<string>))
                {
                    AddStringListInput(
                        groupPanel,
                        inputGroup,
                        property);
                }
            }

            groupBorder.Child = groupPanel;

            EmailInputFieldsPanel.Children.Add(groupBorder);
        }

        private void AddStringInput(
            StackPanel parent,
            IEmailInputs inputGroup,
            PropertyInfo property)
        {
            bool isMultiline =
                ShouldUseMultilineTextBox(property.Name);

            TextBlock label = new TextBlock
            {
                Text = ConvertPropertyNameToLabel(property.Name),
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(
                    Color.FromRgb(55, 65, 81)),
                Margin = new Thickness(0, 0, 0, 5)
            };

            string currentValue =
                property.GetValue(inputGroup)?.ToString() ??
                string.Empty;

            TextBox textBox = new TextBox
            {
                Text = currentValue,
                MinHeight = isMultiline ? 100 : 36,
                Margin = new Thickness(0, 0, 0, 14),
                Padding = new Thickness(8),
                AcceptsReturn = isMultiline,
                TextWrapping = isMultiline
                    ? TextWrapping.Wrap
                    : TextWrapping.NoWrap,
                VerticalContentAlignment = isMultiline
                    ? VerticalAlignment.Top
                    : VerticalAlignment.Center,
                VerticalScrollBarVisibility = isMultiline
                    ? ScrollBarVisibility.Auto
                    : ScrollBarVisibility.Disabled,
                Tag = new EmailInputControlInfo(
                    inputGroup,
                    property)
            };

            parent.Children.Add(label);
            parent.Children.Add(textBox);
        }

        private void AddStringListInput(
            StackPanel parent,
            IEmailInputs inputGroup,
            PropertyInfo property)
        {
            TextBlock label = new TextBlock
            {
                Text = ConvertPropertyNameToLabel(property.Name),
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(
                    Color.FromRgb(55, 65, 81)),
                Margin = new Thickness(0, 0, 0, 5)
            };

            TextBlock instructions = new TextBlock
            {
                Text = "Enter one item per line.",
                FontSize = 12,
                Foreground = new SolidColorBrush(
                    Color.FromRgb(107, 114, 128)),
                Margin = new Thickness(0, 0, 0, 5)
            };

            List<string> currentItems =
                property.GetValue(inputGroup) as List<string> ??
                new List<string>();

            TextBox textBox = new TextBox
            {
                Text = string.Join(
                    Environment.NewLine,
                    currentItems),
                MinHeight = 120,
                Margin = new Thickness(0, 0, 0, 14),
                Padding = new Thickness(8),
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap,
                VerticalContentAlignment = VerticalAlignment.Top,
                VerticalScrollBarVisibility =
                    ScrollBarVisibility.Auto,
                Tag = new EmailInputControlInfo(
                    inputGroup,
                    property)
            };

            parent.Children.Add(label);
            parent.Children.Add(instructions);
            parent.Children.Add(textBox);
        }

        private void ShowNoEmailTypeSelectedMessage()
        {
            EmailInputFieldsPanel.Children.Clear();

            Border placeholder = CreatePlaceholderBorder();

            StackPanel panel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center
            };

            panel.Children.Add(new TextBlock
            {
                Text = "No Email Type Selected",
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(
                    Color.FromRgb(75, 85, 99)),
                HorizontalAlignment =
                    HorizontalAlignment.Center
            });

            panel.Children.Add(new TextBlock
            {
                Text = "Select an email type to display its input fields.",
                Margin = new Thickness(0, 8, 0, 0),
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Foreground = new SolidColorBrush(
                    Color.FromRgb(107, 114, 128)),
                HorizontalAlignment =
                    HorizontalAlignment.Center
            });

            placeholder.Child = panel;

            EmailInputFieldsPanel.Children.Add(placeholder);
        }

        private void ShowNoInputsConfiguredMessage()
        {
            EmailInputFieldsPanel.Children.Clear();

            Border placeholder = CreatePlaceholderBorder();

            StackPanel panel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center
            };

            panel.Children.Add(new TextBlock
            {
                Text = "No Inputs Configured",
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(
                    Color.FromRgb(75, 85, 99)),
                HorizontalAlignment =
                    HorizontalAlignment.Center
            });

            panel.Children.Add(new TextBlock
            {
                Text = "This email type does not contain any input sections.",
                Margin = new Thickness(0, 8, 0, 0),
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Foreground = new SolidColorBrush(
                    Color.FromRgb(107, 114, 128)),
                HorizontalAlignment =
                    HorizontalAlignment.Center
            });

            placeholder.Child = panel;

            EmailInputFieldsPanel.Children.Add(placeholder);
        }

        private static Border CreatePlaceholderBorder()
        {
            return new Border
            {
                Padding = new Thickness(30),
                HorizontalAlignment =
                    HorizontalAlignment.Stretch,
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush(
                    Color.FromRgb(209, 213, 219)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6)
            };
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
            if (!ValidateEmailTypeSelection())
            {
                return;
            }

            List<Staff> selectedStaff = GetSelectedStaff();

            if (selectedStaff.Count == 0)
            {
                MessageBox.Show(
                    "Please select at least one staff member before previewing the email.",
                    "No Staff Selected",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                StaffListBox.Focus();
                return;
            }

            EmailType selectedEmailType =
                (EmailType)EmailTypeComboBox.SelectedItem;

            MessageBox.Show(
                $"Email type: {selectedEmailType.displayName}\n" +
                $"Selected staff: {selectedStaff.Count}\n\n" +
                "Email preview functionality will be added later.",
                "Preview Email",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void GenerateEmailButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (!ValidateEmailTypeSelection())
            {
                return;
            }

            List<Staff> selectedStaff = GetSelectedStaff();

            if (selectedStaff.Count == 0)
            {
                MessageBox.Show(
                    "Please select at least one staff member before generating the email.",
                    "No Staff Selected",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                StaffListBox.Focus();
                return;
            }

            EmailType selectedEmailType =
                (EmailType)EmailTypeComboBox.SelectedItem;

            MessageBox.Show(
                $"Email type: {selectedEmailType.displayName}\n" +
                $"Selected staff: {selectedStaff.Count}\n\n" +
                "Email generation functionality will be added later.",
                "Generate Email",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private bool ValidateEmailTypeSelection()
        {
            if (HasValidEmailTypeSelection())
            {
                return true;
            }

            MessageBox.Show(
                "Please select an email type before continuing.",
                "No Email Type Selected",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            EmailTypeComboBox.Focus();
            EmailTypeComboBox.IsDropDownOpen = true;

            return false;
        }

        private bool HasValidEmailTypeSelection()
        {
            return EmailTypeComboBox.SelectedItem is EmailType;
        }

        private string GetSelectedEmailType()
        {
            return EmailTypeComboBox.SelectedItem is EmailType emailType
                ? emailType.displayName
                : string.Empty;
        }

        private List<Staff> GetSelectedStaff()
        {
            return StaffListBox.SelectedItems
                .Cast<Staff>()
                .ToList();
        }

        private static string ConvertPropertyNameToLabel(
            string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return string.Empty;
            }

            string label = Regex.Replace(
                propertyName,
                "([a-z0-9])([A-Z])",
                "$1 $2");

            label = label.Replace(
                "_",
                " ");

            return label;
        }

        private static bool ShouldUseMultilineTextBox(
            string propertyName)
        {
            return propertyName.Contains(
                       "Body",
                       StringComparison.OrdinalIgnoreCase)
                   ||
                   propertyName.Contains(
                       "Intro",
                       StringComparison.OrdinalIgnoreCase)
                   ||
                   propertyName.Contains(
                       "Text",
                       StringComparison.OrdinalIgnoreCase)
                   ||
                   propertyName.Contains(
                       "Subtitle",
                       StringComparison.OrdinalIgnoreCase)
                   ||
                   propertyName.Contains(
                       "Message",
                       StringComparison.OrdinalIgnoreCase);
        }

        private sealed record EmailInputControlInfo(
            IEmailInputs InputGroup,
            PropertyInfo Property
        );
    }
}