using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Services.Database;

namespace Schedule_Creator_V2.Services.Scheduling
{
    public static class ScheduleService
    {
        // =========================================================
        // SCHEDULE NAMES
        // =========================================================

        public static IReadOnlyList<string>
            GetScheduleNames()
        {
            return DatabaseRead
                .ReadAllScheduleNames();
        }


        public static bool ScheduleNameExists(
            string scheduleName)
        {
            if (string.IsNullOrWhiteSpace(
                    scheduleName))
            {
                return false;
            }

            string normalizedName =
                scheduleName.Trim();

            return GetScheduleNames()
                .Any(existingName =>
                    string.Equals(
                        existingName,
                        normalizedName,
                        StringComparison.OrdinalIgnoreCase));
        }


        // =========================================================
        // READ SCHEDULE
        // =========================================================

        public static List<ScheduleRow>
            GetSchedule(
                string scheduleName)
        {
            if (string.IsNullOrWhiteSpace(
                    scheduleName))
            {
                return new List<ScheduleRow>();
            }

            return DatabaseRead
                .ReadScheduleByScheduleName(
                    scheduleName.Trim());
        }


        // =========================================================
        // JOB SETTING DAYS
        // =========================================================

        public static List<DayOfWeek>
            GetConfiguredScheduleDays()
        {
            return DatabaseRead
                .ReadJobSettingsDays();
        }


        // =========================================================
        // STAFF
        // =========================================================

        public static Staff GetStaff(
            int staffId)
        {
            return DatabaseRead
                .ReadStaffByID(
                    staffId);
        }


        public static string GetStaffDisplayName(
            int staffId)
        {
            Staff staff =
                GetStaff(
                    staffId);

            return string.IsNullOrWhiteSpace(
                    staff.displayName)
                ? $"Staff ID: {staffId}"
                : staff.displayName;
        }


        // =========================================================
        // SAVE SCHEDULE
        // =========================================================

        public static void SaveSchedule(
            IEnumerable<ScheduleRow> scheduleRows)
        {
            ArgumentNullException.ThrowIfNull(
                scheduleRows);

            List<ScheduleRow> rows =
                scheduleRows.ToList();

            if (rows.Count == 0)
            {
                throw new InvalidOperationException(
                    "Cannot save an empty schedule.");
            }

            foreach (ScheduleRow scheduleRow
                     in rows)
            {
                DatabaseCreate
                    .CreateSchedule(
                        scheduleRow);
            }
        }
    }
}