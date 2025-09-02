using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Schedule_Creator_V2
{

    

    
    internal class UniFunc
    {
        public static List<DateOnly> GetRangeOfDates(DateOnly startDate, DateOnly endDate)
        {
            List<DateOnly> returnList = new List<DateOnly>();

            for (DateOnly date = startDate; date <= endDate; date = date.AddDays(1))
            {
                returnList.Add(date);
            }

            return returnList;

        }
        public static bool StartIsBeforeEnd(DateOnly startDate, DateOnly endDate)
        {
            if (startDate <= endDate)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }



}
