using System.ComponentModel;

namespace Schedule_Creator_V2.Models.Enums
{
    public enum Positions
    {
        [Description("Program Coordinator")]
        Program_Coordinator,
        [Description("Graduate Assistant")]
        Graduate_Assistant,
        [Description("Lead Supervisor")]
        Lead_Supervisor,
        [Description("Lead Route Setter")]
        Lead_Route_Setter,
        [Description("Shift Lead")]
        Shift_Lead,
        [Description("Attendant")]
        Attendant,
        [Description("Sub")]
        SUB,
        [Description("Unknown")]
        Unknown
    }
}
