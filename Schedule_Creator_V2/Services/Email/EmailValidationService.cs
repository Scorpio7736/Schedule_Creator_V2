using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Schedule_Creator_V2.Services.Email
{
    public static class EmailValidationService
    {
        private static readonly SolidColorBrush DefaultBorderBrush =
            new SolidColorBrush(
                Color.FromRgb(171, 173, 179));

        private static readonly SolidColorBrush ErrorBorderBrush =
            new SolidColorBrush(
                Color.FromRgb(220, 38, 38));

        public static bool ValidateRequiredFields(
            StackPanel container,
            out string errorMessage)
        {
            List<TextBox> textBoxes =
                FindVisualChildren<TextBox>(container)
                    .ToList();

            TextBox? firstInvalidTextBox = null;

            foreach (TextBox textBox in textBoxes)
            {
                ResetTextBoxBorder(textBox);

                if (!string.IsNullOrWhiteSpace(textBox.Text))
                {
                    continue;
                }

                textBox.BorderBrush = ErrorBorderBrush;
                textBox.BorderThickness = new Thickness(2);

                firstInvalidTextBox ??= textBox;
            }

            if (firstInvalidTextBox is null)
            {
                errorMessage = string.Empty;
                return true;
            }

            firstInvalidTextBox.BringIntoView();
            firstInvalidTextBox.Focus();

            errorMessage =
                "Please complete all required email fields before generating the email.";

            return false;
        }

        private static void ResetTextBoxBorder(
            TextBox textBox)
        {
            textBox.BorderBrush = DefaultBorderBrush;
            textBox.BorderThickness = new Thickness(1);
        }

        private static IEnumerable<T> FindVisualChildren<T>(
            DependencyObject parent)
            where T : DependencyObject
        {
            int childCount =
                VisualTreeHelper.GetChildrenCount(parent);

            for (int index = 0;
                 index < childCount;
                 index++)
            {
                DependencyObject child =
                    VisualTreeHelper.GetChild(
                        parent,
                        index);

                if (child is T matchingChild)
                {
                    yield return matchingChild;
                }

                foreach (T descendant in
                         FindVisualChildren<T>(child))
                {
                    yield return descendant;
                }
            }
        }
    }
}