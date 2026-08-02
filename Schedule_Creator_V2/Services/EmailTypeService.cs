using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Constants;
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
                new EmailType(
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
                            SenderTitle: "UREC-O Climbing Tower Supervisor",
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
                                FooterLogoSource: EmailImageSources.FooterImage
                        )

                    }
                )
            };
        }
    }
}