using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace Schedule_Creator_V2.Models
{
    internal class PageInput
    {
        private Object _input;
        private string _title;
        private bool _excluded;
        private readonly List<PageInput> _requiredInputs;
        private readonly List<PageInput> _missingRequiredInputs;

        public PageInput(Object input, string title, bool excluded = false, IEnumerable<PageInput> requiredInputs = null)
        {

            this._input = input;
            this._title = title;
            this._excluded = excluded;
            this._requiredInputs = requiredInputs != null ? new List<PageInput>(requiredInputs) : new List<PageInput>();
            this._missingRequiredInputs = new List<PageInput>();
        }


        public string GetTitle()
        {
            return this._title;
        }

        public bool IsExcluded => this._excluded;

        internal bool HasValue()
        {
            return InputHasValue(this._input);
        }

        public void AddRequiredInputs(IEnumerable<PageInput> requiredInputs)
        {
            if (requiredInputs == null)
            {
                return;
            }

            foreach (PageInput requiredInput in requiredInputs)
            {
                if (requiredInput == null)
                {
                    continue;
                }

                if (!this._requiredInputs.Contains(requiredInput))
                {
                    this._requiredInputs.Add(requiredInput);
                }
            }
        }

        public ReadOnlyCollection<PageInput> GetMissingRequiredInputs()
        {
            return this._missingRequiredInputs.AsReadOnly();
        }

        public bool CheckNull()
        {
            this._missingRequiredInputs.Clear();

            if (this._excluded != true)
            {
                if (!InputHasValue(this._input))
                {
                    return false;
                }

                if (this._requiredInputs.Count > 0)
                {
                    foreach (PageInput requiredInput in this._requiredInputs)
                    {
                        if (requiredInput == null || requiredInput.IsExcluded)
                        {
                            continue;
                        }

                        if (!requiredInput.HasValue())
                        {
                            this._missingRequiredInputs.Add(requiredInput);
                        }
                    }

                    if (this._missingRequiredInputs.Count > 0)
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return true; // returns true if excluded to pass statments
            }
        }

        public void ResetInput()
        {
            if (_input is TextBlock textBlock)
            {
                textBlock.Text = string.Empty;
            }

            if (_input is TextBox textBox)
            {
                textBox.Text = string.Empty;
            }

            if (_input is ComboBox comboBox)
            {
                comboBox.SelectedItem = null;
            }

            if (_input is CheckBox checkBox)
            {
                checkBox.IsChecked = false;
            }

            if (_input is DatePicker datePicker)
            {
                datePicker.SelectedDate = null;
            }

            if (_input is TimePicker timePicker)
            {
                timePicker.Value = null;
            }
        }

        private static bool InputHasValue(object input)
        {
            if (input is TextBlock textBlock)
            {
                return !string.IsNullOrEmpty(textBlock.Text);
            }

            if (input is TextBox textBox)
            {
                return !string.IsNullOrEmpty(textBox.Text);
            }

            if (input is ComboBox comboBox)
            {
                return comboBox.SelectedItem != null;
            }

            if (input is CheckBox checkBox)
            {
                return checkBox.IsChecked == true;
            }

            if (input is DatePicker datePicker)
            {
                return datePicker.SelectedDate != null;
            }

            if (input is TimePicker timePicker)
            {
                return timePicker.Value != null;
            }

            return input != null;
        }
    }
}
