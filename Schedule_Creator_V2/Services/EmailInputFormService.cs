using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Interfaces;
using Schedule_Creator_V2.Models.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Schedule_Creator_V2.Services
{
    public static class EmailInputFormService
    {
        public static void BuildEmailInputControls(
            StackPanel container,
            EmailType emailType)
        {
            container.Children.Clear();

            if (emailType.inputs == null ||
                emailType.inputs.Count == 0)
            {
                ShowNoInputsConfiguredMessage(container);
                return;
            }

            foreach (IEmailInputs inputGroup in emailType.inputs)
            {
                Border inputGroupControl =
                    CreateInputGroup(inputGroup);

                container.Children.Add(inputGroupControl);
            }
        }

        public static void ApplyInputValues(
            StackPanel container)
        {
            foreach (Border border in
                     container.Children.OfType<Border>())
            {
                if (border.Child is not Panel groupPanel)
                {
                    continue;
                }

                foreach (TextBox textBox in
                         groupPanel.Children.OfType<TextBox>())
                {
                    if (textBox.Tag is not EmailInputControlInfo controlInfo)
                    {
                        continue;
                    }

                    SetPropertyValue(
                        controlInfo,
                        textBox.Text);
                }
            }
        }

        public static void ShowNoEmailTypeSelectedMessage(
            StackPanel container)
        {
            ShowPlaceholderMessage(
                container,
                heading: "No Email Type Selected",
                message:
                "Select an email type to display its input fields.");
        }

        private static void ShowNoInputsConfiguredMessage(
            StackPanel container)
        {
            ShowPlaceholderMessage(
                container,
                heading: "No Inputs Configured",
                message:
                "This email type does not contain any input sections.");
        }

        private static Border CreateInputGroup(
            IEmailInputs inputGroup)
        {
            StackPanel groupPanel = new StackPanel();

            TextBlock groupHeading = new TextBlock
            {
                Text = inputGroup.GetEmailTypeName(),
                FontSize = 17,
                FontWeight = FontWeights.SemiBold,
                Foreground = CreateBrush(31, 41, 55),
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
                else if (property.PropertyType ==
                         typeof(List<string>))
                {
                    AddStringListInput(
                        groupPanel,
                        inputGroup,
                        property);
                }
            }

            return new Border
            {
                Margin = new Thickness(0, 0, 0, 16),
                Padding = new Thickness(16),
                Background = Brushes.White,
                BorderBrush = CreateBrush(209, 213, 219),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Child = groupPanel
            };
        }

        private static void AddStringInput(
            Panel parent,
            IEmailInputs inputGroup,
            PropertyInfo property)
        {
            bool isMultiline =
                ShouldUseMultilineTextBox(property.Name);

            TextBlock label =
                CreateLabel(property.Name);

            string currentValue =
                property.GetValue(inputGroup)?.ToString()
                ?? string.Empty;

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

        private static void AddStringListInput(
            Panel parent,
            IEmailInputs inputGroup,
            PropertyInfo property)
        {
            TextBlock label =
                CreateLabel(property.Name);

            TextBlock instructions = new TextBlock
            {
                Text = "Enter one item per line.",
                FontSize = 12,
                Foreground = CreateBrush(107, 114, 128),
                Margin = new Thickness(0, 0, 0, 5)
            };

            List<string> currentItems =
                property.GetValue(inputGroup)
                    as List<string>
                ?? new List<string>();

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
                VerticalContentAlignment =
                    VerticalAlignment.Top,
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

        private static TextBlock CreateLabel(
            string propertyName)
        {
            return new TextBlock
            {
                Text = ConvertPropertyNameToLabel(
                    propertyName),

                FontWeight = FontWeights.SemiBold,
                Foreground = CreateBrush(55, 65, 81),
                Margin = new Thickness(0, 0, 0, 5)
            };
        }

        private static void ShowPlaceholderMessage(
            StackPanel container,
            string heading,
            string message)
        {
            container.Children.Clear();

            StackPanel messagePanel = new StackPanel
            {
                HorizontalAlignment =
                    HorizontalAlignment.Center
            };

            TextBlock headingText = new TextBlock
            {
                Text = heading,
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = CreateBrush(75, 85, 99),
                HorizontalAlignment =
                    HorizontalAlignment.Center
            };

            TextBlock messageText = new TextBlock
            {
                Text = message,
                Margin = new Thickness(0, 8, 0, 0),
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Foreground = CreateBrush(107, 114, 128),
                HorizontalAlignment =
                    HorizontalAlignment.Center
            };

            messagePanel.Children.Add(headingText);
            messagePanel.Children.Add(messageText);

            Border placeholder = new Border
            {
                Padding = new Thickness(30),
                HorizontalAlignment =
                    HorizontalAlignment.Stretch,
                Background = Brushes.White,
                BorderBrush = CreateBrush(209, 213, 219),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Child = messagePanel
            };

            container.Children.Add(placeholder);
        }

        private static void SetPropertyValue(
            EmailInputControlInfo controlInfo,
            string text)
        {
            if (!controlInfo.Property.CanWrite)
            {
                return;
            }

            if (controlInfo.Property.PropertyType ==
                typeof(string))
            {
                controlInfo.Property.SetValue(
                    controlInfo.InputGroup,
                    text);

                return;
            }

            if (controlInfo.Property.PropertyType ==
                typeof(List<string>))
            {
                List<string> values = text
                    .Replace("\r\n", "\n")
                    .Split(
                        '\n',
                        StringSplitOptions.RemoveEmptyEntries)
                    .Select(value => value.Trim())
                    .Where(value =>
                        !string.IsNullOrWhiteSpace(value))
                    .ToList();

                controlInfo.Property.SetValue(
                    controlInfo.InputGroup,
                    values);
            }
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

            return label.Replace("_", " ");
        }

        private static bool ShouldUseMultilineTextBox(
            string propertyName)
        {
            string[] multilinePropertyWords =
            {
                "Body",
                "Intro",
                "Text",
                "Subtitle",
                "Message"
            };

            return multilinePropertyWords.Any(word =>
                propertyName.Contains(
                    word,
                    StringComparison.OrdinalIgnoreCase));
        }

        private static SolidColorBrush CreateBrush(
            byte red,
            byte green,
            byte blue)
        {
            return new SolidColorBrush(
                Color.FromRgb(red, green, blue));
        }

        private sealed record EmailInputControlInfo(
            IEmailInputs InputGroup,
            PropertyInfo Property
        );
    }
}