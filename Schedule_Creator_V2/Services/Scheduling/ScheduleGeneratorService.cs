using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Enums;
using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Services.Database;

namespace Schedule_Creator_V2.Services
{
    public static class ScheduleGeneratorService
    {
        private record GeneratedInterval(
            DayOfWeek Day,
            int StaffId,
            TimeOnly Start,
            TimeOnly End
        );

        /// <summary>
        /// Automatically generates a weekly schedule.
        ///
        /// Rules:
        /// - Only schedules employees within their availability.
        /// - Excluded employees are never scheduled.
        /// - Keeps the requested number of staff working at all times.
        /// - Requires a leadership-qualified employee at all times when
        ///   requireLeadership is true.
        /// - Tries to keep employees on continuous shifts.
        /// - Tries to distribute hours fairly.
        /// - Merges consecutive coverage intervals into full shifts.
        /// </summary>
        public static List<ScheduleRow> AutoGenerateSchedule(
            string scheduleName,
            int staffRequired,
            HashSet<int>? excludedStaffIds = null,
            bool requireLeadership = true)
        {
            if (string.IsNullOrWhiteSpace(scheduleName))
            {
                throw new ArgumentException(
                    "A schedule name is required.",
                    nameof(scheduleName));
            }

            if (staffRequired < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(staffRequired),
                    "At least one staff member must be required.");
            }

            excludedStaffIds ??= new HashSet<int>();

            List<AutoGenScheduleRow> staffData =
                DatabaseRead.ReadAutoGenScheduleData();

            List<JobSettings> jobSettings =
                DatabaseRead.ReadJobSettings();

            if (jobSettings.Count == 0)
            {
                throw new InvalidOperationException(
                    "No job settings were found.");
            }

            /*
             * Tracks how many minutes each employee has been assigned
             * throughout the generated schedule.
             *
             * This allows us to prefer employees with fewer scheduled
             * hours when several employees are equally valid choices.
             */
            Dictionary<int, int> assignedMinutes = new();

            /*
             * Temporary interval assignments.
             *
             * Example:
             *
             * Jack  Monday 2:45 - 4:45
             * Jack  Monday 4:45 - 5:30
             * Jack  Monday 5:30 - 6:15
             *
             * These will later be merged into:
             *
             * Jack  Monday 2:45 - 6:15
             */
            List<GeneratedInterval> assignments = new();

            foreach (JobSettings jobSetting in jobSettings)
            {
                GenerateDay(
                    jobSetting,
                    staffData,
                    staffRequired,
                    excludedStaffIds,
                    requireLeadership,
                    assignedMinutes,
                    assignments);
            }

            List<GeneratedInterval> mergedAssignments =
                MergeAssignments(assignments);

            return mergedAssignments
                .OrderBy(x => GetDaySortOrder(x.Day))
                .ThenBy(x => x.Start)
                .ThenBy(x => x.StaffId)
                .Select(x =>
                    new ScheduleRow(
                        x.Day,
                        x.StaffId,
                        x.Start,
                        x.End,
                        scheduleName.Trim()))
                .ToList();
        }

        /// <summary>
        /// Generates all required coverage for a single day.
        /// </summary>
        private static void GenerateDay(
            JobSettings jobSetting,
            List<AutoGenScheduleRow> allStaffData,
            int staffRequired,
            HashSet<int> excludedStaffIds,
            bool requireLeadership,
            Dictionary<int, int> assignedMinutes,
            List<GeneratedInterval> assignments)
        {
            if (jobSetting.closingTime <= jobSetting.openingTime)
            {
                throw new InvalidOperationException(
                    $"Invalid operating hours for {jobSetting.dayOfWeek}: " +
                    $"{jobSetting.openingTime:h:mm tt} - " +
                    $"{jobSetting.closingTime:h:mm tt}.");
            }

            /*
             * Get everyone who:
             *
             * 1. Has availability on this day.
             * 2. Has not been excluded.
             * 3. Has at least some availability during operating hours.
             */
            List<AutoGenScheduleRow> dayStaff =
                allStaffData
                    .Where(x =>
                        x.dayOfWeek == jobSetting.dayOfWeek)
                    .Where(x =>
                        !excludedStaffIds.Contains(x.id))
                    .Where(x =>
                        x.startTime < jobSetting.closingTime &&
                        x.endTime > jobSetting.openingTime)
                    .ToList();

            if (dayStaff.Count == 0)
            {
                throw new InvalidOperationException(
                    $"No eligible staff are available on " +
                    $"{jobSetting.dayOfWeek}.");
            }

            /*
             * Build every point in time where staffing availability changes.
             *
             * Example:
             *
             * Open:      2:45
             * Jack:      2:45 - 8:15
             * Sara:      2:45 - 6:15
             * Owen:      4:45 - 8:15
             *
             * Boundaries:
             *
             * 2:45
             * 4:45
             * 6:15
             * 8:15
             */
            List<TimeOnly> boundaries =
                GetTimeBoundaries(
                    jobSetting,
                    dayStaff);

            /*
             * Schedule each interval between boundaries.
             */
            for (int i = 0; i < boundaries.Count - 1; i++)
            {
                TimeOnly intervalStart = boundaries[i];
                TimeOnly intervalEnd = boundaries[i + 1];

                if (intervalEnd <= intervalStart)
                {
                    continue;
                }

                /*
                 * An employee must be available for the ENTIRE interval.
                 */
                List<AutoGenScheduleRow> candidates =
                    dayStaff
                        .Where(x =>
                            x.startTime <= intervalStart &&
                            x.endTime >= intervalEnd)
                        /*
                         * Prevent duplicate employee entries in case an
                         * employee has overlapping availability rows.
                         */
                        .GroupBy(x => x.id)
                        .Select(group => group.First())
                        .ToList();

                if (candidates.Count < staffRequired)
                {
                    throw new InvalidOperationException(
                        $"Not enough staff are available on " +
                        $"{jobSetting.dayOfWeek} from " +
                        $"{intervalStart:h:mm tt} to " +
                        $"{intervalEnd:h:mm tt}. " +
                        $"Required: {staffRequired}. " +
                        $"Available: {candidates.Count}.");
                }

                /*
                 * Find employees who were working immediately before this
                 * interval. They will receive a strong preference so the
                 * generator produces longer continuous shifts.
                 */
                HashSet<int> currentlyWorking =
                    assignments
                        .Where(x =>
                            x.Day == jobSetting.dayOfWeek &&
                            x.End == intervalStart)
                        .Select(x => x.StaffId)
                        .ToHashSet();

                List<AutoGenScheduleRow> selected =
                    SelectStaffForInterval(
                        candidates,
                        staffRequired,
                        requireLeadership,
                        currentlyWorking,
                        assignedMinutes,
                        intervalEnd);

                int intervalMinutes =
                    GetMinutesBetween(
                        intervalStart,
                        intervalEnd);

                foreach (AutoGenScheduleRow employee in selected)
                {
                    assignments.Add(
                        new GeneratedInterval(
                            jobSetting.dayOfWeek,
                            employee.id,
                            intervalStart,
                            intervalEnd));

                    if (!assignedMinutes.ContainsKey(employee.id))
                    {
                        assignedMinutes[employee.id] = 0;
                    }

                    assignedMinutes[employee.id] +=
                        intervalMinutes;
                }
            }
        }

        /// <summary>
        /// Creates all time points where employee availability changes.
        /// </summary>
        private static List<TimeOnly> GetTimeBoundaries(
            JobSettings jobSetting,
            List<AutoGenScheduleRow> dayStaff)
        {
            SortedSet<TimeOnly> boundaries = new()
            {
                jobSetting.openingTime,
                jobSetting.closingTime
            };

            foreach (AutoGenScheduleRow employee in dayStaff)
            {
                TimeOnly effectiveStart =
                    employee.startTime < jobSetting.openingTime
                        ? jobSetting.openingTime
                        : employee.startTime;

                TimeOnly effectiveEnd =
                    employee.endTime > jobSetting.closingTime
                        ? jobSetting.closingTime
                        : employee.endTime;

                if (effectiveStart >
                    jobSetting.openingTime &&
                    effectiveStart <
                    jobSetting.closingTime)
                {
                    boundaries.Add(effectiveStart);
                }

                if (effectiveEnd >
                    jobSetting.openingTime &&
                    effectiveEnd <
                    jobSetting.closingTime)
                {
                    boundaries.Add(effectiveEnd);
                }
            }

            return boundaries.ToList();
        }

        /// <summary>
        /// Chooses the employees who should work a particular interval.
        /// </summary>
        private static List<AutoGenScheduleRow> SelectStaffForInterval(
            List<AutoGenScheduleRow> candidates,
            int staffRequired,
            bool requireLeadership,
            HashSet<int> currentlyWorking,
            Dictionary<int, int> assignedMinutes,
            TimeOnly intervalEnd)
        {
            List<AutoGenScheduleRow> selected = new();

            /*
             * Leadership gets selected first so we know every interval
             * contains at least one qualified leader.
             */
            if (requireLeadership)
            {
                List<AutoGenScheduleRow> leaders =
                    candidates
                        .Where(x =>
                            IsLeadership(x.position))
                        .ToList();

                if (leaders.Count == 0)
                {
                    throw new InvalidOperationException(
                        "Leadership coverage is required, but no " +
                        "leadership-qualified employee is available " +
                        $"through {intervalEnd:h:mm tt}.");
                }

                AutoGenScheduleRow selectedLeader =
                    leaders
                        .OrderBy(x =>
                            GetCandidateScore(
                                x,
                                currentlyWorking,
                                assignedMinutes))
                        .ThenByDescending(x =>
                            GetMinutesBetween(
                                intervalEnd,
                                x.endTime))
                        .First();

                selected.Add(selectedLeader);
            }

            /*
             * Fill all remaining positions.
             */
            while (selected.Count < staffRequired)
            {
                AutoGenScheduleRow? nextEmployee =
                    candidates
                        .Where(candidate =>
                            selected.All(selectedEmployee =>
                                selectedEmployee.id != candidate.id))
                        .OrderBy(candidate =>
                            GetCandidateScore(
                                candidate,
                                currentlyWorking,
                                assignedMinutes))
                        /*
                         * If two employees have equal scores, prefer the
                         * employee who remains available longer.
                         */
                        .ThenByDescending(candidate =>
                            GetMinutesBetween(
                                intervalEnd,
                                candidate.endTime))
                        .FirstOrDefault();

                if (nextEmployee == null)
                {
                    throw new InvalidOperationException(
                        "Unable to find enough eligible staff " +
                        "for this scheduling interval.");
                }

                selected.Add(nextEmployee);
            }

            return selected;
        }

        /// <summary>
        /// Produces a scheduling priority score.
        ///
        /// Lower scores are preferred.
        /// </summary>
        private static int GetCandidateScore(
            AutoGenScheduleRow employee,
            HashSet<int> currentlyWorking,
            Dictionary<int, int> assignedMinutes)
        {
            int score =
                assignedMinutes.GetValueOrDefault(
                    employee.id,
                    0);

            /*
             * Strongly prefer employees who are already working.
             *
             * This prevents schedules such as:
             *
             * Jack 2:45-3:00
             * Sara 3:00-3:15
             * Jack 3:15-3:30
             *
             * and instead creates longer shifts.
             */
            if (currentlyWorking.Contains(employee.id))
            {
                score -= 100_000;
            }

            return score;
        }

        /// <summary>
        /// Determines whether a position can satisfy the leadership
        /// requirement.
        /// </summary>
        private static bool IsLeadership(
            Positions position)
        {
            return position is
                Positions.Program_Coordinator or
                Positions.Graduate_Assistant or
                Positions.Lead_Supervisor or
                Positions.Lead_Route_Setter or
                Positions.Shift_Lead;
        }

        /// <summary>
        /// Combines consecutive intervals belonging to the same employee.
        /// </summary>
        private static List<GeneratedInterval> MergeAssignments(
            List<GeneratedInterval> assignments)
        {
            List<GeneratedInterval> merged = new();

            var groups =
                assignments
                    .GroupBy(x =>
                        new
                        {
                            x.Day,
                            x.StaffId
                        });

            foreach (var group in groups)
            {
                List<GeneratedInterval> employeeIntervals =
                    group
                        .OrderBy(x => x.Start)
                        .ToList();

                if (employeeIntervals.Count == 0)
                {
                    continue;
                }

                GeneratedInterval current =
                    employeeIntervals[0];

                for (int i = 1;
                     i < employeeIntervals.Count;
                     i++)
                {
                    GeneratedInterval next =
                        employeeIntervals[i];

                    /*
                     * The next interval starts exactly where the current
                     * one ends, so they form one continuous shift.
                     */
                    if (current.End == next.Start)
                    {
                        current =
                            current with
                            {
                                End = next.End
                            };
                    }
                    else
                    {
                        merged.Add(current);

                        current = next;
                    }
                }

                merged.Add(current);
            }

            return merged;
        }

        /// <summary>
        /// Returns the number of minutes between two TimeOnly values.
        /// </summary>
        private static int GetMinutesBetween(
            TimeOnly start,
            TimeOnly end)
        {
            if (end <= start)
            {
                return 0;
            }

            TimeSpan difference =
                end.ToTimeSpan() -
                start.ToTimeSpan();

            return (int)difference.TotalMinutes;
        }

        /// <summary>
        /// Makes Monday the first day when sorting schedules.
        /// </summary>
        private static int GetDaySortOrder(
            DayOfWeek day)
        {
            return day switch
            {
                DayOfWeek.Monday => 1,
                DayOfWeek.Tuesday => 2,
                DayOfWeek.Wednesday => 3,
                DayOfWeek.Thursday => 4,
                DayOfWeek.Friday => 5,
                DayOfWeek.Saturday => 6,
                DayOfWeek.Sunday => 7,
                _ => 8
            };
        }
    }
}