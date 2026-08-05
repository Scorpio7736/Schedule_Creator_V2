namespace Schedule_Creator_V2.Models.Records
{
    public record StaffNameAndAvail(
        int id,
        string fName,
        string lName,
        TimeOnly startTime,
        TimeOnly endTime)
    {
        public const int MissingStaffId = -1;

        public bool isMissing =>
            id == MissingStaffId;

        public string displayName =>
            isMissing
                ? "Missing"
                : $"{fName} {lName} " +
                  $"({startTime.ToShortTimeString()} - " +
                  $"{endTime.ToShortTimeString()})";

        public static StaffNameAndAvail MissingOption =>
            new(
                MissingStaffId,
                "Missing",
                string.Empty,
                TimeOnly.MinValue,
                TimeOnly.MinValue);
    }
}