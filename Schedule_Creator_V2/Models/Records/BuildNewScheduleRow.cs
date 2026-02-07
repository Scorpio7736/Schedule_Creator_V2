using System;
using System.Collections.Generic;

namespace Schedule_Creator_V2.Models.Records
{
    public record BuildNewScheduleRow(
        string scheduleName, 
        StaffNameAndAvail mondayStaff, 
        StaffNameAndAvail tuesdayStaff, 
        StaffNameAndAvail wednesdayStaff, 
        StaffNameAndAvail thursdayStaff, 
        StaffNameAndAvail fridayStaff, 
        StaffNameAndAvail saturdayStaff, 
        StaffNameAndAvail sundayStaff
        );


    public static class BuildNewScheduleRowExtensions
    {
        public static List<StaffNameAndAvail> GetRowsItems(this BuildNewScheduleRow row)
        {
            if (row is null)
                throw new ArgumentNullException(nameof(row));

            return new List<StaffNameAndAvail>()
            {
                row.mondayStaff,
                row.tuesdayStaff,
                row.wednesdayStaff,
                row.thursdayStaff,
                row.fridayStaff,
                row.saturdayStaff,
                row.sundayStaff
            };
        }

        public static List<(DayOfWeek Day, StaffNameAndAvail Staff)> GetRowsItemsWithDay(this BuildNewScheduleRow row)
        {
            if (row is null)
                throw new ArgumentNullException(nameof(row));

            var list = new List<(DayOfWeek, StaffNameAndAvail)>();

            if (row.mondayStaff is not null)    list.Add((DayOfWeek.Monday,    row.mondayStaff));
            if (row.tuesdayStaff is not null)   list.Add((DayOfWeek.Tuesday,   row.tuesdayStaff));
            if (row.wednesdayStaff is not null) list.Add((DayOfWeek.Wednesday, row.wednesdayStaff));
            if (row.thursdayStaff is not null)  list.Add((DayOfWeek.Thursday,  row.thursdayStaff));
            if (row.fridayStaff is not null)    list.Add((DayOfWeek.Friday,    row.fridayStaff));
            if (row.saturdayStaff is not null)  list.Add((DayOfWeek.Saturday,  row.saturdayStaff));
            if (row.sundayStaff is not null)    list.Add((DayOfWeek.Sunday,    row.sundayStaff));

            return list;
        }
    }
}
