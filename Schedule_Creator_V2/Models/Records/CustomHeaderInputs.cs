using Schedule_Creator_V2.Models.Constants;
using Schedule_Creator_V2.Models.Interfaces;

namespace Schedule_Creator_V2.Models.Records
{
    public record CustomHeaderInputs : IEmailInputs
    {
        public string OrganizationName { get; set; }

        public string HeaderLabel { get; set; }

        public string EmailHeading { get; set; }

        public string HeaderSubtitle { get; set; }

        public string HeaderImageUrl { get; set; }

        public CustomHeaderInputs(
            string OrganizationName,
            string HeaderLabel,
            string EmailHeading,
            string HeaderSubtitle,
            string? HeaderImageUrl = null)
        {
            this.OrganizationName = OrganizationName;
            this.HeaderLabel = HeaderLabel;
            this.EmailHeading = EmailHeading;
            this.HeaderSubtitle = HeaderSubtitle;

            this.HeaderImageUrl =
                string.IsNullOrWhiteSpace(HeaderImageUrl)
                    ? EmailImageSources.Default_HeaderImage
                    : HeaderImageUrl;
        }

        public string GetEmailTypeName()
        {
            return "Custom Header";
        }
    }
}