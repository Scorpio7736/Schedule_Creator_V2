using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Constants;
using Schedule_Creator_V2.Models.Defaults;
using Schedule_Creator_V2.Models.Interfaces;
using Schedule_Creator_V2.Models.Records;
using System.Collections.Generic;

namespace Schedule_Creator_V2.Services
{
    public static class EmailTypeService
    {
        public static List<EmailType> CreateEmailTypes()
        {
            return new List<EmailType>
            {
                /* TEST */new EmailType(
                    displayName: "Test",
                    inputs: new List<IEmailInputs>
                    {
                        new EmailDetailsInputs(
                            Subject: "Test Email"
                        ),

                        new CustomHeaderInputs(
                            OrganizationName: "UNIVERSITY RECREATION",
                            HeaderLabel: "TEST MESSAGE FOR TOWER TEAM",
                            EmailHeading: "THIS IS A TEST",
                            HeaderSubtitle: "TESTING OF THE NEW FORMAT"
                        ),

                        new CustomBodyInputs(
                            RecipientGreeting: "Hello,",
                            EmailBody: "Please ignore this."
                        ),
                        new CustomSignatureInputs(
                            SignatureClosing: "Thanks,",
                            SenderName: "Jack London",
                            SenderTitle: "Climbing Tower Supervisor",
                            SenderOrganization: "Urec Outdoors",
                            SenderEmail: "londjc22@uwgb.edu",
                            SenderPhone: "(414) 425 - 4022"
                        ),
                        new CustomAnnouncementsInputs(
                            AnnouncementsLabel: " ANNOUNCEMENTS lbl",
                            AnnouncementsIntro: "TESTING ANNOUNCEMENTS INTRO",
                            AnnouncementsList: [
                                "TEST 1",
                                "TEST 2",
                                "TEST 3"
                                ]
                        ),
                        new CustomRequestInputs(
                            RequestLabel: " REQUESTS lbl",
                            RequestTitle: "TESTING REQUESTS TITLE",
                            RequestBody: "TESTING REQUESTS BODY",
                            RequestButton: "Testing request button",
                            RequestLink: "https://www.uwgb.edu/urec/adventure/climbing/"
                        ),
                        new CustomAttachmentsInputs(
                            AttachmentsLabel: " ATTACHMENTS lbl",
                            AttachmentsIntro: "TESTING ATTACHMENTS INTRO",
                            AttachmentsList: [
                                "TEST 1",
                                "TEST 2",
                                "TEST 3"
                                ]
                        ),
                        new CustomFooterInputs(
                                FooterOrganization: "UNIVERSITY RECREATION",
                                FooterWebsiteLink: "https://www.uwgb.edu/urec/",
                                FooterWebsiteUrl: "https://www.uwgb.edu/urec/",
                                FooterText: "TESTING FOOTER TEXT",
                                FooterLogoSource: EmailImageSources.Default_FooterImage
                        )

                    }
                ),
                /* NR WELCOME */new EmailType(
                        displayName: "(No Request)Welcome to the Climbing Tower Team",
                        inputs: new List<IEmailInputs>
                        {
                            new EmailDetailsInputs(
                                Subject: "Welcome to the Climbing Tower Team"
                            ),

                            new CustomHeaderInputs(
                            OrganizationName: "UNIVERSITY RECREATION",
                            HeaderLabel: "WELCOME TO THE CLIMBING TOWER TEAM",
                            EmailHeading: "WELCOME TO THE CLIMBING TOWER TEAM",
                            HeaderSubtitle: "WE ARE EXCITED TO HAVE YOU ON BOARD!",
                            HeaderImageUrl: EmailImageSources.WelcomToTheTowerTeamHeader
                        ),
                        new CustomBodyInputs(
                                EmailInputConstants.TowerTeamGreeting,
                                "We are thrilled to welcome you to the Climbing Tower Team! Your dedication and enthusiasm are invaluable to our mission of providing a safe and enjoyable climbing experience for all. We look forward to working with you and seeing the positive impact you'll make on our team and the climbing community."
                        ),
                        EmailInputDefaults.DefaultSignatureInputs,
                    }
                ),
            };
        }
    }
}