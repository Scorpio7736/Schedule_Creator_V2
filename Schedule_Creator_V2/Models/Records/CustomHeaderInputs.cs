using Schedule_Creator_V2.Models.Interfaces;

public record CustomHeaderInputs(
    string OrganizationName,
    string HeaderLabel,
    string EmailHeading,
    string HeaderSubtitle
) : IEmailInputs
{
    public string GetEmailTypeName() => "Custom Header";
}