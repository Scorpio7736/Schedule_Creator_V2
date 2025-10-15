using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Creator_V2.Models
{
    public record DaysOff(int id, DateOnly date, string reason);
}
