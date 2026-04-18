using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Enums;
using Schedule_Creator_V2.Models.Records;

namespace Schedule_Creator_V2.Services
{
    internal sealed class AutoScheduleResult
    {
        public Dictionary<DayOfWeek, List<int>> DayAssignments { get; } = new Dictionary<DayOfWeek, List<int>>();

        public List<string> Warnings { get; } = new List<string>();
    }

    internal static class AutoScheduleService
    {
        private static readonly Random Randomizer = Random.Shared;
        private static readonly HashSet<Positions> LeadPositions = new HashSet<Positions>
        {
            Positions.Shift_Lead,
            Positions.Lead_Supervisor,
            Positions.Lead_Route_Setter
        };

        public static AutoScheduleResult Generate(int requestedStaffPerDay, HashSet<int> excludedStaffIds)
        {
            AutoScheduleResult result = new AutoScheduleResult();
            int minimumStaffPerDay = Math.Max(3, requestedStaffPerDay);

            List<JobSettings> settings = DatabaseRead.ReadJobSettings().ToList();
            ShuffleInPlace(settings);

            if (settings.Count == 0)
            {
                result.Warnings.Add("No job settings were found.");
                return result;
            }

            List<Staff> allStaff = DatabaseRead.ReadStaff()
                .Where(staff => !excludedStaffIds.Contains(staff.id))
                .ToList();

            ShuffleInPlace(allStaff);

            if (allStaff.Count == 0)
            {
                result.Warnings.Add("No available staff remain after exclusions.");
                return result;
            }

            Dictionary<int, List<Availability>> availabilityByStaff = allStaff.ToDictionary(
                staff => staff.id,
                staff => DatabaseRead.ReadAvailForStaffByID(staff.id));

            Dictionary<int, int> assignmentCounts = allStaff.ToDictionary(staff => staff.id, _ => 0);

            foreach (JobSettings setting in settings)
            {
                List<Staff> fullCoverage = allStaff
                    .Where(staff => CoversFullShift(staff.id, setting, availabilityByStaff))
                    .ToList();

                List<Staff> halfCoverage = allStaff
                    .Where(staff => !fullCoverage.Any(s => s.id == staff.id) && CoversHalfShift(staff.id, setting, availabilityByStaff))
                    .ToList();

                List<Staff> dayCandidates = fullCoverage.Concat(halfCoverage).ToList();

                List<int> selected = SelectDayStaff(dayCandidates, minimumStaffPerDay, assignmentCounts);

                if (selected.Count < 3)
                {
                    result.Warnings.Add($"{setting.dayOfWeek}: unable to assign the minimum of 3 staff with current availability.");
                }

                bool hasLead = selected.Any(id => allStaff.Any(staff => staff.id == id && LeadPositions.Contains(staff.position)));
                if (!hasLead)
                {
                    result.Warnings.Add($"{setting.dayOfWeek}: no lead was available for the generated shift.");
                }

                bool usedPartialCoverage = selected.Any(id => !fullCoverage.Any(staff => staff.id == id));
                if (usedPartialCoverage)
                {
                    result.Warnings.Add($"{setting.dayOfWeek}: used partial availability coverage for one or more staff.");
                }

                foreach (int staffId in selected)
                {
                    assignmentCounts[staffId]++;
                }

                result.DayAssignments[setting.dayOfWeek] = selected;
            }

            EnsureAllStaffAssignedAtLeastOnce(result, allStaff, settings, availabilityByStaff, assignmentCounts);

            return result;
        }

        private static List<int> SelectDayStaff(List<Staff> candidates, int minimumStaffPerDay, Dictionary<int, int> assignmentCounts)
        {
            List<int> selectedIds = new List<int>();

            List<Staff> randomizedCandidates = candidates.ToList();
            ShuffleInPlace(randomizedCandidates);

            Dictionary<int, int> randomTieBreakers = randomizedCandidates
                .Select((staff, index) => new { staff.id, index })
                .ToDictionary(item => item.id, item => item.index);

            List<Staff> leadCandidates = randomizedCandidates.Where(staff => LeadPositions.Contains(staff.position)).ToList();
            Staff? selectedLead = leadCandidates
                .OrderBy(staff => assignmentCounts[staff.id])
                .ThenBy(staff => randomTieBreakers[staff.id])
                .FirstOrDefault();

            if (selectedLead != null)
            {
                selectedIds.Add(selectedLead.id);
            }

            IEnumerable<Staff> nonLeads = randomizedCandidates.Where(staff => !LeadPositions.Contains(staff.position));
            IEnumerable<Staff> extraLeads = randomizedCandidates.Where(staff => LeadPositions.Contains(staff.position) && (selectedLead == null || staff.id != selectedLead.id));

            IEnumerable<Staff> orderedFill = nonLeads
                .Concat(extraLeads)
                .Where(staff => !selectedIds.Contains(staff.id))
                .OrderBy(staff => assignmentCounts[staff.id])
                .ThenBy(staff => randomTieBreakers[staff.id]);

            foreach (Staff staff in orderedFill)
            {
                if (selectedIds.Count >= minimumStaffPerDay)
                {
                    break;
                }

                selectedIds.Add(staff.id);
            }

            return selectedIds;
        }

        private static void EnsureAllStaffAssignedAtLeastOnce(
            AutoScheduleResult result,
            List<Staff> allStaff,
            List<JobSettings> settings,
            Dictionary<int, List<Availability>> availabilityByStaff,
            Dictionary<int, int> assignmentCounts)
        {
            foreach (Staff staff in allStaff.Where(s => assignmentCounts[s.id] == 0))
            {
                List<JobSettings> availableDays = settings
                    .Where(setting => CoversHalfShift(staff.id, setting, availabilityByStaff))
                    .ToList();

                ShuffleInPlace(availableDays);

                JobSettings? dayToUse = availableDays
                    .OrderBy(setting => result.DayAssignments.TryGetValue(setting.dayOfWeek, out List<int>? assigned) ? assigned.Count : 0)
                    .FirstOrDefault();

                if (dayToUse == null)
                {
                    continue;
                }

                if (!result.DayAssignments.TryGetValue(dayToUse.dayOfWeek, out List<int>? dayAssignments))
                {
                    dayAssignments = new List<int>();
                    result.DayAssignments[dayToUse.dayOfWeek] = dayAssignments;
                }

                if (!dayAssignments.Contains(staff.id))
                {
                    dayAssignments.Add(staff.id);
                    assignmentCounts[staff.id]++;
                }
            }
        }


        private static void ShuffleInPlace<T>(IList<T> items)
        {
            for (int i = items.Count - 1; i > 0; i--)
            {
                int swapIndex = Randomizer.Next(i + 1);
                (items[i], items[swapIndex]) = (items[swapIndex], items[i]);
            }
        }

        private static bool CoversFullShift(int staffId, JobSettings setting, Dictionary<int, List<Availability>> availabilityByStaff)
        {
            if (!availabilityByStaff.TryGetValue(staffId, out List<Availability>? availabilities))
            {
                return false;
            }

            return availabilities.Any(availability =>
                availability.dayOfTheWeek == setting.dayOfWeek &&
                availability.startTime <= setting.openingTime &&
                availability.endTime >= setting.closingTime);
        }

        private static bool CoversHalfShift(int staffId, JobSettings setting, Dictionary<int, List<Availability>> availabilityByStaff)
        {
            if (!availabilityByStaff.TryGetValue(staffId, out List<Availability>? availabilities))
            {
                return false;
            }

            double fullMinutes = (setting.closingTime - setting.openingTime).TotalMinutes;

            return availabilities.Any(availability =>
            {
                if (availability.dayOfTheWeek != setting.dayOfWeek)
                {
                    return false;
                }

                TimeOnly overlapStart = availability.startTime > setting.openingTime ? availability.startTime : setting.openingTime;
                TimeOnly overlapEnd = availability.endTime < setting.closingTime ? availability.endTime : setting.closingTime;
                double overlapMinutes = (overlapEnd - overlapStart).TotalMinutes;

                return overlapMinutes >= fullMinutes / 2;
            });
        }
    }
}
