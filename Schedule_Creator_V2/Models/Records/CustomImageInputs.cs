using Schedule_Creator_V2.Models.Interfaces;

namespace Schedule_Creator_V2.Models.Records
{
    public record CustomImageInputs(
        string ImageSource,
        string ImageAltText
    ) : IEmailInputs
    {
        public string GetEmailTypeName()
        {
            return "Custom Image";
        }
    }
}