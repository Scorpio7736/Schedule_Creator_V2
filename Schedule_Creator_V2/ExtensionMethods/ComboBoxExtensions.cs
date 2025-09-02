using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Schedule_Creator_V2.ExtensionMethods
{
    internal static class ComboBoxExtensions
    {

        public static bool HasValue(this ComboBox combobox)
        {
            if (combobox == null)
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
