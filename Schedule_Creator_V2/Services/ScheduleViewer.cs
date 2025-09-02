using Schedule_Creator_V2.Services;
using Schedule_Creator_V2.Models;
using Microsoft.Identity.Client;

namespace Schedule_Creator_V2.Services
{
    internal class ScheduleViewer
    {

        public static List<Staff> BuildSchedule(DateOnly date)
        {
            List<Staff> returnList = new List<Staff>();
            List<Staff> staffList = DatabaseRead.ReadStaff();
            List<Availability> availList = DatabaseRead.ReadAvailability();
            List<DaysOff> daysOffList = DatabaseRead.ReadDaysOff();
            AvailDays availDay = Enum.Parse<AvailDays>(date.DayOfWeek.ToString());

            
            foreach(Staff staff in staffList)
            {
                // Add who is avail on day
                foreach (Availability avail in availList)
                {
                    if (staff.id == avail.id)
                    {
                        if (availDay == avail.dayOfTheWeek || avail.dayOfTheWeek == AvailDays.Every_Day)
                        {
                            if (staff.isBelayCertified == true && isBetweenDates(date, staff.certifiedOn, staff.expiresOn))
                            returnList.Add(staff);
                        }   
                    }
                }
                // Add who is avail on date
                foreach (DaysOff daysOff in daysOffList)
                {
                    if (staff.id == daysOff.id && daysOff.date == date)
                    {
                        returnList.Remove(staff);
                    }
                }
            }

            return OrganizeList(returnList);

        }

        private static bool isBetweenDates(DateOnly selectedDate, DateOnly? startDate, DateOnly? endDate)
        {
            if (selectedDate >= startDate && selectedDate <= endDate)
            {
                return true;
            }
            return false;
        }



        public static List<Staff> OrganizeList(List<Staff> list)
        {
            var returnList = new List<Staff>();

                foreach (Positions position in Enum.GetValues<Positions>())
                {
                    foreach (Staff staff in list)
                    {
                        if (staff.position == position)
                        {
                            returnList.Add(staff);
                        }
                    }
                }
            

            return returnList;
        }
    }
}
