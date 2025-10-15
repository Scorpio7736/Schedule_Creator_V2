namespace Schedule_Creator_V2.Models
{
    public record JobSettings(DayOfWeek dayOfWeek, TimeOnly openingTime, TimeOnly closingTime);
}
