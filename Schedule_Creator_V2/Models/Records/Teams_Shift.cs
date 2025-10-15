using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Schedule_Creator_V2.Models
{
    public record TeamsShift(
        string workArea,
        string shiftColor,
        DateOnly shiftDate,
        TimeOnly startTime,
        TimeOnly endTime,
        string staffEmail
        );


}
