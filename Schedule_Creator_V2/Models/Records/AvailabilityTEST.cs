using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Creator_V2.Models
{
    public record AvailabilityTEST(int id, bool availMon, bool availTue, bool availWed, bool availThu, bool availFri, bool availSat, bool availSun, bool availEveryDay);
}
