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
            List<DayOfWeek> daysWithoutLead = new List<DayOfWeek>();

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

                List<int> selectedIds = new List<int>();

                if (dayLeadIds.Count > 0)
                {
                    int selectedLeadId = dayLeadIds
                        .OrderBy(id => assignmentCount[id])
                        .ThenBy(id => staffById[id].displayName)
                        .First();

                    selectedIds.Add(selectedLeadId);

                    List<int> supportIds = dayStaffIds
                        .Where(id => id != selectedLeadId)
                        .OrderBy(id => IsLead(staffById[id]) ? 1 : 0)
                        .ThenBy(id => assignmentCount[id])
                        .ThenBy(id => staffById[id].displayName)
                        .Take(2)
                        .ToList();

                    selectedIds.AddRange(supportIds);
                }
                else
                {
                    daysWithoutLead.Add(day);

                    selectedIds = dayStaffIds
                        .OrderBy(id => assignmentCount[id])
                        .ThenBy(id => staffById[id].displayName)
                        .Take(3)
                        .ToList();
                }

                if (selectedIds.Count < 3)
                {
                    return new AutoScheduleResult(
                        false,
                        new Dictionary<DayOfWeek, List<int>>(),
                        $"{day}: could not find enough staff to build a 3-person shift.",
                        new List<int>());
                }

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
            if (daysWithoutLead.Count > 0)
            {
                string noLeadDays = string.Join(", ", daysWithoutLead.OrderBy(day => day));
                message += $" Warning: no lead-qualified staff were available for {noLeadDays}.";
            }

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

                    int replacementIndex = -1;
                    int bestPriority = int.MaxValue;
                    for (int i = 0; i < currentDay.Count; i++)
                    {
                        int currentId = currentDay[i];
                        if (assignmentCount[currentId] <= 1)
                        {
                            continue;
                        }

                        bool currentIsLead = IsLead(staffById[currentId]);
                        int priority;

                        if (isLead)
                        {
                            // Prefer lead-for-lead swaps first.
                            // Allow replacing a non-lead if needed so multiple leads are possible,
                            // but this remains a lower-priority choice.
                            priority = currentIsLead ? 0 : 1;
                        }
                        else
                        {
                            if (currentIsLead)
                            {
                                continue;
                            }

                            priority = 0;
                        }

                        if (priority < bestPriority)
                        {
                            bestPriority = priority;
                            replacementIndex = i;
                        }
                    }

                    if (replacementIndex < 0)
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

        private static bool CoversWholeShift(StaffNameAndAvail avail, JobSettings settings)
        {
            return avail.startTime <= settings.openingTime && avail.endTime >= settings.closingTime;
        }
    }
}
