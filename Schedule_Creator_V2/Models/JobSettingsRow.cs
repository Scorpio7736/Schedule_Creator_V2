using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TimePickerControl = Xceed.Wpf.Toolkit.TimePicker;

namespace Schedule_Creator_V2.Models
{
    public record JobSettingsRow(ComboBox dayOfWeek, TimePickerControl openTime, TimePickerControl closeTime);
}
