using Schedule_Creator_V2.Models.Constants;
using Schedule_Creator_V2.Models.Records;

namespace Schedule_Creator_V2.Models.Defaults
{
    public static class EmailInputDefaults
    {
        public static CustomSignatureInputs DefaultSignatureInputs =>
            new CustomSignatureInputs(
                SignatureClosing: "Thanks,",
                SenderName: "Jack London",
                SenderTitle: "Climbing Tower Supervisor",
                SenderOrganization: "Urec Outdoors",
                SenderEmail: "londjc22@uwgb.edu",
                SenderPhone: "(414) 425 - 4022"
            );

        public static CustomFooterInputs DefaultFooterInputs =>
            new CustomFooterInputs(
                FooterOrganization:
                    EmailInputConstants.OrganizationName,

                FooterWebsiteLink:
                    "University Recreation",

                FooterWebsiteUrl:
                    "https://www.uwgb.edu/urec/",

                FooterText:
                    "University of Wisconsin-Green Bay",

                FooterLogoSource:
                    EmailImageSources.Default_FooterImage
            );
    }
}