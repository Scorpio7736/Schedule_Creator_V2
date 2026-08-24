namespace Schedule_Creator_V2.ExtensionMethods
{
    internal static class DateOnlyExtensions
    {
        public static List<DateOnly> GetRangeOfDates(
            this DateOnly startDate,
            DateOnly endDate)
        {
            List<DateOnly> returnList = new List<DateOnly>();

            for (DateOnly date = startDate;
                 date <= endDate;
                 date = date.AddDays(1))
            {
                returnList.Add(date);
            }

            return returnList;
        }

        public static bool StartIsBeforeEnd(
            this DateOnly startDate,
            DateOnly endDate)
        {
            return startDate <= endDate;
        }
    }
}