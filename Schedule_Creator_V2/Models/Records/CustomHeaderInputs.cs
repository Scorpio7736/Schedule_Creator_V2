using Schedule_Creator_V2.Models.Constants;
using Schedule_Creator_V2.Models.Interfaces;

public record CustomHeaderInputs(
    string OrganizationName,
    string HeaderLabel,
    string EmailHeading,
    string HeaderSubtitle,
    string? CustomHeaderImageUrl = null
) : IEmailInputs
{
    public string HeaderImageUrl =>
        CustomHeaderImageUrl ?? EmailImageSources.Default_HeaderImage;

    public string GetEmailTypeName() => "Custom Header";
}