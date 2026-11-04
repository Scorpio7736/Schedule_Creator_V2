using Schedule_Creator_V2.Models.Interfaces;
using System.Collections.Generic;

namespace Schedule_Creator_V2.Models.Records
{
    public record CustomPositionDescriptionInputs(
        string Position,
        string Description,
        List<string> Details
    ) : IEmailInputs
    {
        public string GetEmailTypeName() => "Position Description";
    }
}
