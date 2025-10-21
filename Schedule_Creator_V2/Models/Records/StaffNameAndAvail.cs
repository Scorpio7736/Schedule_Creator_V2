namespace Schedule_Creator_V2.Models.Records
{
    public record StaffNameAndAvail(int id, string fName, string lName, TimeOnly startTime, TimeOnly endTime)
    {
       public string displayString = fName + " " + lName + "(" + startTime.ToShortTimeString() + " - " + endTime.ToShortTimeString() + ")";
    }
}
