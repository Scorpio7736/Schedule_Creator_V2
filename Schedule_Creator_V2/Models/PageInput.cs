using System.Windows.Controls;

namespace Schedule_Creator_V2.Models
{
    internal class PageInput
    {
        private Object _input;
        private string _title;
        private bool _excluded;

        public PageInput(Object input, string title, bool excluded = false)
        {

            this._input = input;
            this._title = title;
            this._excluded = excluded;
        }


        public string GetTitle()
        {
            return this._title;
        }

        public bool CheckNull()
        {
            if (this._excluded != true)
            {
                if (this._input is TextBlock tb && tb.Text == string.Empty)
                {
                    return false;
                }

                if (this._input is TextBox txt && txt.Text == string.Empty)
                {
                    return false;
                }

                if (this._input is ComboBox cb && cb.SelectedItem == null)
                {
                    return false;
                }

                if (this._input is DatePicker dp && dp.SelectedDate == null)
                {
                    return false;
                }

                if (this._input is CheckBox chk && chk.IsChecked == false)
                {
                    return false;
                }

                if (this._input is DatePicker datePicker && datePicker.SelectedDate == null)
                {
                    return false;
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
        }
    }
}