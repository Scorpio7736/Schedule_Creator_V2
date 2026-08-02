using Schedule_Creator_V2.Models;
using System.Collections.Generic;

namespace Schedule_Creator_V2.Services
{
    public static class EmailStaffService
    {
        public static List<Staff> LoadStaff()
        {
            return DatabaseRead.ReadStaff() ?? new List<Staff>();
        }
    }
}