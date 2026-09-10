using Schedule_Creator_V2.Models.Enums;

namespace Schedule_Creator_V2.Models.Records
{
    public record AutoGenShiftTemplate(
        TimeOnly startTime,
        TimeOnly endTime,
        AutoGenShiftType shiftType
    );
}