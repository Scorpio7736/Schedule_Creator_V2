namespace Schedule_Creator_V2.Models.Records
{
    public record StaffNameAndAvail(int id, string fName, string lName, TimeOnly startTime, TimeOnly endTime)
    {
        public string displayName => $"{fName} {lName} ({startTime.ToShortTimeString()} - {endTime.ToShortTimeString()})";
    }
}
