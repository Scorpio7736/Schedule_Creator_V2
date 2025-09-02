using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Schedule_Creator_V2.ExtensionMethods
{
    internal static class DatePickerExtensions
    {

        public static bool HasValue(this DatePicker datePicker)
        {
            if (datePicker.SelectedDate == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
