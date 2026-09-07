using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Schedule_Creator_V2.Services.Email
{
    public static class EmailValidationService
    {
        private static readonly SolidColorBrush
    DefaultBorderBrush =
        CreateFrozenBrush(
            Color.FromRgb(
                171,
                173,
                179));

        private static readonly SolidColorBrush
            ErrorBorderBrush =
                CreateFrozenBrush(
                    Color.FromRgb(
                        220,
                        38,
                        38));


        // =========================================================
        // BRUSH CREATION
        // =========================================================

        private static SolidColorBrush CreateFrozenBrush(
            Color color)
        {
            SolidColorBrush brush =
                new SolidColorBrush(
                    color);

            /*
             * SolidColorBrush derives from Freezable.
             *
             * Freezing the brush removes its thread affinity,
             * allowing these shared static brushes to be safely
             * used by WPF controls created on different STA threads.
             */
            brush.Freeze();

            return brush;
        }

        // =========================================================
        // REQUIRED FIELD ATTACHED PROPERTY
        // =========================================================

        public static readonly DependencyProperty
            IsRequiredProperty =
                DependencyProperty.RegisterAttached(
                    "IsRequired",
                    typeof(bool),
                    typeof(EmailValidationService),
                    new PropertyMetadata(true));


        public static void SetIsRequired(
            DependencyObject element,
            bool value)
        {
            element.SetValue(
                IsRequiredProperty,
                value);
        }


        public static bool GetIsRequired(
            DependencyObject element)
        {
            return (bool)element.GetValue(
                IsRequiredProperty);
        }


        // =========================================================
        // VALIDATION
        // =========================================================

        public static bool ValidateRequiredFields(
            StackPanel container,
            out string errorMessage)
        {
            List<TextBox> textBoxes =
                FindVisualChildren<TextBox>(
                    container)
                .ToList();

            TextBox? firstInvalidTextBox =
                null;

            foreach (TextBox textBox
                     in textBoxes)
            {
                ResetTextBoxBorder(
                    textBox);

                /*
                 * Optional fields are ignored by required-field
                 * validation.
                 *
                 * This is used for fields such as Header Image,
                 * which already has a configured default.
                 */
                if (!GetIsRequired(
                        textBox))
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(
                        textBox.Text))
                {
                    continue;
                }

                textBox.BorderBrush =
                    ErrorBorderBrush;

                textBox.BorderThickness =
                    new Thickness(2);

                firstInvalidTextBox ??=
                    textBox;
            }

            if (firstInvalidTextBox
                is null)
            {
                errorMessage =
                    string.Empty;

                return true;
            }

            firstInvalidTextBox
                .BringIntoView();

            firstInvalidTextBox
                .Focus();

            errorMessage =
                "Please complete all required email fields " +
                "before generating the email.";

            return false;
        }


        // =========================================================
        // RESET BORDER
        // =========================================================

        private static void ResetTextBoxBorder(
            TextBox textBox)
        {
            textBox.BorderBrush =
                DefaultBorderBrush;

            textBox.BorderThickness =
                new Thickness(1);
        }


        // =========================================================
        // FIND CONTROLS
        // =========================================================

        private static IEnumerable<T>
            FindVisualChildren<T>(
                DependencyObject parent)
            where T : DependencyObject
        {
            int childCount =
                VisualTreeHelper
                    .GetChildrenCount(
                        parent);

            for (int index = 0;
                 index < childCount;
                 index++)
            {
                DependencyObject child =
                    VisualTreeHelper
                        .GetChild(
                            parent,
                            index);

                if (child is T matchingChild)
                {
                    yield return
                        matchingChild;
                }

                foreach (T descendant in
                         FindVisualChildren<T>(
                             child))
                {
                    yield return
                        descendant;
                }
            }
        }
    }
}