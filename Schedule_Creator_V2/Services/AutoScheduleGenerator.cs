using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Enums;
using Schedule_Creator_V2.Models.Records;

namespace Schedule_Creator_V2.Services
{
    internal sealed class AutoScheduleGenerator
    {
        internal sealed record AutoScheduleResult(
            bool Success,
            Dictionary<DayOfWeek, List<int>> DayAssignments,
            string Message,
            List<int> UnassignedStaffIds
            );

        private static readonly HashSet<Positions> LeadPositions = new()
        {
            Positions.Lead_Supervisor,
            Positions.Lead_Route_Setter,
            Positions.Shift_Lead
        };

        public AutoScheduleResult Generate()
        {
            List<JobSettings> jobSettings = DatabaseRead.ReadJobSettings();
            if (jobSettings.Count == 0)
            {
                return new AutoScheduleResult(false, new Dictionary<DayOfWeek, List<int>>(), "No job settings found.", new List<int>());
            }

            Dictionary<DayOfWeek, JobSettings> settingsByDay = jobSettings
                .GroupBy(setting => setting.dayOfWeek)
                .ToDictionary(group => group.Key, group => group.First());

            List<Staff> staff = DatabaseRead.ReadStaff();
            Dictionary<int, Staff> staffById = staff.ToDictionary(member => member.id, member => member);

            Dictionary<DayOfWeek, List<StaffNameAndAvail>> availByDay = settingsByDay.Keys.ToDictionary(
                day => day,
                day => DatabaseRead.ReadStaffNamesAndAvailOnDay(day)
                    .Where(avail => CoversWholeShift(avail, settingsByDay[day]))
                    .ToList());

            Dictionary<int, int> assignmentCount = staffById.Keys.ToDictionary(id => id, _ => 0);
            Dictionary<DayOfWeek, List<int>> dayAssignments = new Dictionary<DayOfWeek, List<int>>();

            foreach (DayOfWeek day in settingsByDay.Keys.OrderBy(day => day))
            {
                List<int> dayStaffIds = availByDay[day].Select(avail => avail.id).Distinct().ToList();
                List<int> dayLeadIds = dayStaffIds.Where(id => IsLead(staffById[id])).ToList();

                if (dayStaffIds.Count < 3)
                {
                    return new AutoScheduleResult(
                        false,
                        new Dictionary<DayOfWeek, List<int>>(),
                        $"{day}: fewer than 3 staff can cover the full shift hours.",
                        new List<int>());
                }

                if (dayLeadIds.Count == 0)
                {
                    return new AutoScheduleResult(
                        false,
                        new Dictionary<DayOfWeek, List<int>>(),
                        $"{day}: no lead-qualified staff can cover the full shift hours.",
                        new List<int>());
                }

                int selectedLeadId = PickBest(dayLeadIds, assignmentCount, staffById);
                List<int> selectedIds = new List<int> { selectedLeadId };

                List<int> nonLeadIds = dayStaffIds.Where(id => !IsLead(staffById[id])).ToList();
                IEnumerable<int> supportPool = nonLeadIds.Count >= 2
                    ? nonLeadIds
                    : dayStaffIds.Where(id => id != selectedLeadId);

                List<int> supportIds = supportPool
                    .OrderBy(id => assignmentCount[id])
                    .ThenBy(id => staffById[id].displayName)
                    .Take(2)
                    .ToList();

                if (supportIds.Count < 2)
                {
                    return new AutoScheduleResult(
                        false,
                        new Dictionary<DayOfWeek, List<int>>(),
                        $"{day}: could not find enough staff to build a 3-person shift.",
                        new List<int>());
                }

                selectedIds.AddRange(supportIds);

                foreach (int id in selectedIds)
                {
                    assignmentCount[id]++;
                }

                dayAssignments[day] = selectedIds;
            }

            SpreadShiftsAcrossStaff(dayAssignments, assignmentCount, staffById, availByDay);

            List<int> unassignedStaffIds = assignmentCount
                .Where(entry => entry.Value == 0)
                .Select(entry => entry.Key)
                .OrderBy(id => staffById[id].displayName)
                .ToList();

            string message = unassignedStaffIds.Count == 0
                ? "Schedule generated successfully."
                : "Schedule generated, but some staff could not be assigned to a full shift.";

            return new AutoScheduleResult(true, dayAssignments, message, unassignedStaffIds);
        }

        private static void SpreadShiftsAcrossStaff(
            Dictionary<DayOfWeek, List<int>> dayAssignments,
            Dictionary<int, int> assignmentCount,
            Dictionary<int, Staff> staffById,
            Dictionary<DayOfWeek, List<StaffNameAndAvail>> availByDay)
        {
            List<int> zeroAssigned = assignmentCount.Where(entry => entry.Value == 0).Select(entry => entry.Key).ToList();

            foreach (int unassignedId in zeroAssigned)
            {
                Staff unassignedStaff = staffById[unassignedId];
                bool isLead = IsLead(unassignedStaff);

                foreach (DayOfWeek day in dayAssignments.Keys.OrderBy(day => day))
                {
                    bool available = availByDay[day].Any(avail => avail.id == unassignedId);
                    if (!available)
                    {
                        continue;
                    }

                    List<int> currentDay = dayAssignments[day];
                    int leadCount = currentDay.Count(id => IsLead(staffById[id]));

                    int replacementIndex = -1;
                    for (int i = 0; i < currentDay.Count; i++)
                    {
                        int currentId = currentDay[i];
                        if (assignmentCount[currentId] <= 1)
                        {
                            continue;
                        }

                        bool currentIsLead = IsLead(staffById[currentId]);
                        if (isLead)
                        {
                            if (currentIsLead)
                            {
                                replacementIndex = i;
                                break;
                            }
                        }
                        else
                        {
                            if (!currentIsLead)
                            {
                                replacementIndex = i;
                                break;
                            }
                        }
                    }

                    if (replacementIndex < 0)
                    {
                        continue;
                    }

                    if (isLead && leadCount > 1)
                    {
                        continue;
                    }

                    int removedId = currentDay[replacementIndex];
                    currentDay[replacementIndex] = unassignedId;
                    assignmentCount[removedId]--;
                    assignmentCount[unassignedId]++;
                    break;
                }
            }
        }

        private static bool IsLead(Staff staff)
        {
            return LeadPositions.Contains(staff.position);
        }

        private static int PickBest(List<int> staffIds, Dictionary<int, int> assignmentCount, Dictionary<int, Staff> staffById)
        {
            return staffIds
                .OrderBy(id => assignmentCount[id])
                .ThenBy(id => staffById[id].displayName)
                .First();
        }

        private static bool CoversWholeShift(StaffNameAndAvail avail, JobSettings settings)
        {
            return avail.startTime <= settings.openingTime && avail.endTime >= settings.closingTime;
        }
    }
}
