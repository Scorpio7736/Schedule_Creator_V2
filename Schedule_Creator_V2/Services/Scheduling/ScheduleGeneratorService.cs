using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Enums;
using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Services.Database;

namespace Schedule_Creator_V2.Services
{
    public static class ScheduleGeneratorService
    {
        // =========================================================
        // INTERNAL SHIFT REPRESENTATION
        // =========================================================

        private record GeneratedShift(
            DayOfWeek Day,
            int? StaffId,
            TimeOnly Start,
            TimeOnly End,
            AutoGenShiftType ShiftType
        );

        private record EffectiveShiftTemplate(
            int TemplateIndex,
            TimeOnly Start,
            TimeOnly End,
            AutoGenShiftType ShiftType
        );

        // =========================================================
        // AUTO GENERATE
        // =========================================================

        public static List<ScheduleRow> AutoGenerateSchedule(
            string scheduleName,
            IReadOnlyCollection<AutoGenShiftTemplate> shiftTemplates,
            HashSet<int>? excludedStaffIds = null)
        {
            if (string.IsNullOrWhiteSpace(scheduleName))
            {
                throw new ArgumentException(
                    "A schedule name is required.",
                    nameof(scheduleName));
            }

            if (shiftTemplates == null ||
                shiftTemplates.Count == 0)
            {
                throw new ArgumentException(
                    "At least one shift template is required.",
                    nameof(shiftTemplates));
            }

            excludedStaffIds ??=
                new HashSet<int>();

            // -----------------------------------------------------
            // LOAD DATA
            // -----------------------------------------------------

            List<AutoGenScheduleRow> staffData =
                DatabaseRead.ReadAutoGenScheduleData();

            List<JobSettings> jobSettings =
                DatabaseRead.ReadJobSettings();

            if (jobSettings.Count == 0)
            {
                throw new InvalidOperationException(
                    "No job settings were found.");
            }

            // -----------------------------------------------------
            // NORMALIZE STAFF TIMES
            // -----------------------------------------------------

            staffData =
                staffData
                    .Select(x =>
                        x with
                        {
                            startTime =
                                NormalizeTime(x.startTime),

                            endTime =
                                NormalizeTime(x.endTime)
                        })
                    .Where(x =>
                        x.endTime >
                        x.startTime)
                    .ToList();

            // -----------------------------------------------------
            // NORMALIZE JOB SETTINGS
            // -----------------------------------------------------

            jobSettings =
                jobSettings
                    .Select(x =>
                        new JobSettings(
                            x.dayOfWeek,
                            NormalizeTime(
                                x.openingTime),
                            NormalizeTime(
                                x.closingTime)))
                    .Where(x =>
                        x.closingTime >
                        x.openingTime)
                    .ToList();

            // -----------------------------------------------------
            // NORMALIZE TEMPLATES
            // -----------------------------------------------------

            List<AutoGenShiftTemplate> templates =
                shiftTemplates
                    .Select(x =>
                        new AutoGenShiftTemplate(
                            NormalizeTime(
                                x.startTime),
                            NormalizeTime(
                                x.endTime),
                            x.shiftType))
                    .Where(x =>
                        x.endTime >
                        x.startTime)
                    .ToList();

            if (templates.Count == 0)
            {
                throw new InvalidOperationException(
                    "No valid shift templates were provided.");
            }

            // -----------------------------------------------------
            // FAIRNESS TRACKING
            // -----------------------------------------------------

            Dictionary<int, int> assignedMinutes =
                new();

            /*
             * Globally tracks anyone who has already appeared on
             * the schedule.
             *
             * Unused staff receive priority.
             */
            HashSet<int> scheduledStaffIds =
                new();

            List<GeneratedShift> assignments =
                new();

            // -----------------------------------------------------
            // BUILD EACH DAY
            // -----------------------------------------------------

            foreach (
                JobSettings jobSetting
                in jobSettings
                    .OrderBy(x =>
                        GetDaySortOrder(
                            x.dayOfWeek)))
            {
                GenerateDay(
                    jobSetting,
                    templates,
                    staffData,
                    excludedStaffIds,
                    assignedMinutes,
                    scheduledStaffIds,
                    assignments);
            }

            // -----------------------------------------------------
            // RETURN FINAL ROWS
            // -----------------------------------------------------

            return assignments
                .Where(x =>
                    x.End > x.Start)

                .OrderBy(x =>
                    GetDaySortOrder(
                        x.Day))

                .ThenBy(x =>
                    x.Start)

                .ThenBy(x =>
                    x.ShiftType ==
                    AutoGenShiftType.Leadership
                        ? 0
                        : 1)

                .ThenBy(x =>
                    x.StaffId ??
                    int.MaxValue)

                .Select(x =>
                    new ScheduleRow(
                        x.Day,
                        x.StaffId,
                        x.Start,
                        x.End,
                        scheduleName.Trim()))

                .ToList();
        }

        // =========================================================
        // GENERATE DAY
        // =========================================================

        private static void GenerateDay(
            JobSettings jobSetting,
            List<AutoGenShiftTemplate> templates,
            List<AutoGenScheduleRow> allStaffData,
            HashSet<int> excludedStaffIds,
            Dictionary<int, int> assignedMinutes,
            HashSet<int> scheduledStaffIds,
            List<GeneratedShift> assignments)
        {
            // -----------------------------------------------------
            // STAFF FOR DAY
            // -----------------------------------------------------

            List<AutoGenScheduleRow> dayStaff =
                allStaffData
                    .Where(x =>
                        x.dayOfWeek ==
                        jobSetting.dayOfWeek)

                    .Where(x =>
                        !excludedStaffIds.Contains(
                            x.id))

                    .ToList();

            // -----------------------------------------------------
            // BUILD EFFECTIVE SHIFTS
            // -----------------------------------------------------

            List<EffectiveShiftTemplate> dayShifts =
                new();

            for (int i = 0;
                 i < templates.Count;
                 i++)
            {
                AutoGenShiftTemplate template =
                    templates[i];

                /*
                 * Never schedule outside configured operating hours.
                 *
                 * If a template extends beyond opening/closing,
                 * trim it to the actual operating day.
                 */
                TimeOnly effectiveStart =
                    template.startTime <
                    jobSetting.openingTime
                        ? jobSetting.openingTime
                        : template.startTime;

                TimeOnly effectiveEnd =
                    template.endTime >
                    jobSetting.closingTime
                        ? jobSetting.closingTime
                        : template.endTime;

                if (effectiveEnd <=
                    effectiveStart)
                {
                    continue;
                }

                dayShifts.Add(
                    new EffectiveShiftTemplate(
                        i,
                        effectiveStart,
                        effectiveEnd,
                        template.shiftType));
            }

            /*
             * Leadership is assigned first.
             *
             * This prevents an overlapping Any Staff shift from
             * consuming the only available leader.
             *
             * Within each type, fill the shifts with the fewest
             * possible candidates first.
             */
            dayShifts =
                dayShifts
                    .OrderBy(x =>
                        x.ShiftType ==
                        AutoGenShiftType.Leadership
                            ? 0
                            : 1)

                    .ThenBy(x =>
                        CountPotentialCandidates(
                            dayStaff,
                            x))

                    .ThenBy(x =>
                        x.Start)

                    .ThenBy(x =>
                        x.End)

                    .ThenBy(x =>
                        x.TemplateIndex)

                    .ToList();

            // -----------------------------------------------------
            // FILL EACH SHIFT
            // -----------------------------------------------------

            foreach (
                EffectiveShiftTemplate shift
                in dayShifts)
            {
                AutoGenScheduleRow? employee =
                    FindBestEmployeeForShift(
                        jobSetting.dayOfWeek,
                        shift,
                        dayShifts,
                        dayStaff,
                        assignments,
                        assignedMinutes,
                        scheduledStaffIds);

                // -------------------------------------------------
                // MISSING
                // -------------------------------------------------

                if (employee == null)
                {
                    assignments.Add(
                        new GeneratedShift(
                            jobSetting.dayOfWeek,
                            null,
                            shift.Start,
                            shift.End,
                            shift.ShiftType));

                    continue;
                }

                // -------------------------------------------------
                // REAL STAFF
                // -------------------------------------------------

                assignments.Add(
                    new GeneratedShift(
                        jobSetting.dayOfWeek,
                        employee.id,
                        shift.Start,
                        shift.End,
                        shift.ShiftType));

                scheduledStaffIds.Add(
                    employee.id);

                int minutes =
                    GetMinutesBetween(
                        shift.Start,
                        shift.End);

                assignedMinutes[employee.id] =
                    assignedMinutes
                        .GetValueOrDefault(
                            employee.id,
                            0)
                    + minutes;
            }
        }

        // =========================================================
        // FIND BEST EMPLOYEE
        // =========================================================

        private static AutoGenScheduleRow?
            FindBestEmployeeForShift(
                DayOfWeek day,
                EffectiveShiftTemplate shift,
                List<EffectiveShiftTemplate> allDayShifts,
                List<AutoGenScheduleRow> dayStaff,
                List<GeneratedShift> existingAssignments,
                Dictionary<int, int> assignedMinutes,
                HashSet<int> scheduledStaffIds)
        {
            List<AutoGenScheduleRow> candidates =
                dayStaff

                    /*
                     * Must cover the entire shift.
                     */
                    .Where(employee =>
                        employee.startTime <=
                        shift.Start &&
                        employee.endTime >=
                        shift.End)

                    /*
                     * Must meet shift qualification.
                     */
                    .Where(employee =>
                        CanFillShiftType(
                            employee.position,
                            shift.ShiftType))

                    /*
                     * One employee cannot work two overlapping
                     * template shifts.
                     */
                    .Where(employee =>
                        !HasOverlappingAssignment(
                            employee.id,
                            day,
                            shift.Start,
                            shift.End,
                            existingAssignments))

                    /*
                     * An employee may have multiple availability
                     * rows. Only consider them once.
                     */
                    .GroupBy(employee =>
                        employee.id)

                    .Select(group =>
                        group.First())

                    .ToList();

            if (candidates.Count == 0)
            {
                return null;
            }

            return candidates

                /*
                 * PRIORITY #1:
                 *
                 * Include staff who have not appeared anywhere
                 * on the weekly schedule.
                 */
                .OrderBy(employee =>
                    scheduledStaffIds.Contains(
                        employee.id)
                            ? 1
                            : 0)

                /*
                 * PRIORITY #2:
                 *
                 * Prefer staff with fewer opportunities to fit
                 * one of today's template shifts.
                 */
                .ThenBy(employee =>
                    CountCompatibleShifts(
                        employee,
                        allDayShifts))

                /*
                 * PRIORITY #3:
                 *
                 * Balance overall scheduled minutes.
                 */
                .ThenBy(employee =>
                    assignedMinutes
                        .GetValueOrDefault(
                            employee.id,
                            0))

                /*
                 * PRIORITY #4:
                 *
                 * Prefer the more constrained availability.
                 */
                .ThenBy(employee =>
                    GetMinutesBetween(
                        employee.startTime,
                        employee.endTime))

                .ThenBy(employee =>
                    employee.endTime)

                .ThenBy(employee =>
                    employee.id)

                .First();
        }

        // =========================================================
        // COUNT POTENTIAL CANDIDATES
        // =========================================================

        private static int CountPotentialCandidates(
            List<AutoGenScheduleRow> dayStaff,
            EffectiveShiftTemplate shift)
        {
            return dayStaff

                .Where(employee =>
                    employee.startTime <=
                    shift.Start &&
                    employee.endTime >=
                    shift.End)

                .Where(employee =>
                    CanFillShiftType(
                        employee.position,
                        shift.ShiftType))

                .Select(employee =>
                    employee.id)

                .Distinct()

                .Count();
        }

        // =========================================================
        // COUNT COMPATIBLE SHIFTS
        // =========================================================

        private static int CountCompatibleShifts(
            AutoGenScheduleRow employee,
            List<EffectiveShiftTemplate> shifts)
        {
            return shifts.Count(
                shift =>
                    employee.startTime <=
                    shift.Start &&

                    employee.endTime >=
                    shift.End &&

                    CanFillShiftType(
                        employee.position,
                        shift.ShiftType));
        }

        // =========================================================
        // SHIFT TYPE
        // =========================================================

        private static bool CanFillShiftType(
            Positions position,
            AutoGenShiftType shiftType)
        {
            return shiftType switch
            {
                AutoGenShiftType.AnyStaff =>
                    true,

                AutoGenShiftType.Leadership =>
                    IsLeadership(position),

                _ =>
                    false
            };
        }

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

        // =========================================================
        // OVERLAP
        // =========================================================

        private static bool HasOverlappingAssignment(
            int staffId,
            DayOfWeek day,
            TimeOnly start,
            TimeOnly end,
            List<GeneratedShift> assignments)
        {
            return assignments.Any(
                existing =>
                    existing.Day == day &&

                    existing.StaffId ==
                    staffId &&

                    TimesOverlap(
                        start,
                        end,
                        existing.Start,
                        existing.End));
        }

        private static bool TimesOverlap(
            TimeOnly start1,
            TimeOnly end1,
            TimeOnly start2,
            TimeOnly end2)
        {
            /*
             * Adjacent shifts are allowed.
             *
             * 2:45-5:45
             * 5:45-8:15
             *
             * do not overlap.
             */
            return start1 < end2 &&
                   end1 > start2;
        }

        // =========================================================
        // TIME HELPERS
        // =========================================================

        private static TimeOnly NormalizeTime(
            TimeOnly time)
        {
            return new TimeOnly(
                time.Hour,
                time.Minute);
        }

        private static int GetMinutesBetween(
            TimeOnly start,
            TimeOnly end)
        {
            if (end <= start)
            {
                return 0;
            }

            return (int)(
                end.ToTimeSpan() -
                start.ToTimeSpan())
                .TotalMinutes;
        }

        // =========================================================
        // DAY ORDER
        // =========================================================

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