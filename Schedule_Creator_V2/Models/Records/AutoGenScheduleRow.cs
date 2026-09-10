using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schedule_Creator_V2.Models.Enums;

namespace Schedule_Creator_V2.Models.Records
{
    public record AutoGenScheduleRow(int id, Positions position, DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime);
    
}
