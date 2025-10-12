namespace Schedule_Creator_V2.Models
{
    public record Availability(int id, DayOfWeek dayOfTheWeek, TimeOnly startTime, TimeOnly endTime);
}
