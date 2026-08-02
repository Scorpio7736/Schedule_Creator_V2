using Schedule_Creator_V2.Models.Records;

namespace Schedule_Creator_V2.Models.Defaults
{
    public class EmailInputDefaults
    {
        public static CustomSignatureInputs DefaultSignatureInputs => new CustomSignatureInputs(
                            SignatureClosing: "Thanks,",
                            SenderName: "Jack London",
                            SenderTitle: "Climbing Tower Supervisor",
                            SenderOrganization: "Urec Outdoors",
                            SenderEmail: "londjc22@uwgb.edu",
                            SenderPhone: "(414) 425 - 4022"
                        );
    }
}
