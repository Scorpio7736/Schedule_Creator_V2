using Schedule_Creator_V2.Models.Interfaces;
using System.Collections.Generic;

namespace Schedule_Creator_V2.Models.Records
{
    public record EmailType(
        string displayName,
        List<IEmailInputs> inputs
    );
}