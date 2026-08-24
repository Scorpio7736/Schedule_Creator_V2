using Schedule_Creator_V2.Models.Interfaces;

namespace Schedule_Creator_V2.Models.Records
{
    public record EmailType(
        string displayName,
        List<IEmailInputs> inputs,
        bool allowSectionEditing = false
    );
}