using Schedule_Creator_V2.Models.Constants;
using Schedule_Creator_V2.Models.Defaults;
using Schedule_Creator_V2.Models.Interfaces;
using Schedule_Creator_V2.Models.Records;

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
                /* ANNOUNCEMENTS */new EmailType(
                    displayName: "Announcements",
                    inputs: new List<IEmailInputs>
                    {
                        new EmailDetailsInputs(
                            ""
                            ),
                        new CustomHeaderInputs(
                            OrganizationName: EmailInputConstants.OrganizationName,
                            HeaderLabel: "TOWER TEAM ANNOUNCEMENTS",
                            EmailHeading: "",
                            HeaderSubtitle: "",
                            HeaderImageUrl: EmailImageSources.UpcomingClassHeader
                        ),
                        new CustomBodyInputs(
                                RecipientGreeting: EmailInputConstants.TowerTeamGreeting,
                                EmailBody: "Here are the latest announcements for the Tower Team. Please review the information below and stay up to date with our activities and events."
                        ),
                        new CustomAnnouncementsInputs(
                            AnnouncementsLabel: "ANNOUNCEMENTS",
                            AnnouncementsIntro: "Please find the latest updates and important information for the Tower Team below.",
                            AnnouncementsList: [
                                "Announcement 1: Details about the first announcement.",
                                "Announcement 2: Details about the second announcement.",
                                "Announcement 3: Details about the third announcement."
                                ]
                        ),
                        EmailInputDefaults.DefaultSignatureInputs,
                    }
                ),
                /* NR WELCOME */new EmailType(
                        displayName: "(No Request) Welcome to the Climbing Tower Team",
                        inputs: new List<IEmailInputs>
                        {
                            new EmailDetailsInputs(
                                Subject: "Welcome to the Climbing Tower Team"
                            ),

                            new CustomHeaderInputs(
                            OrganizationName: EmailInputConstants.OrganizationName,
                            HeaderLabel: "WELCOME TO THE CLIMBING TOWER TEAM",
                            EmailHeading: "WELCOME TO THE CLIMBING TOWER TEAM",
                            HeaderSubtitle: "WE ARE EXCITED TO HAVE YOU ON BOARD!",
                            HeaderImageUrl: EmailImageSources.WelcomToTheTowerTeamHeader
                        ),
                        new CustomBodyInputs(
                                "Hello [RECIPIENT NAME],",
                                "We are thrilled to welcome you to the Climbing Tower Team! Your dedication and enthusiasm are invaluable to our mission of providing a safe and enjoyable climbing experience for all. We look forward to working with you and seeing the positive impact you'll make on our team and the climbing community."
                        ),
                        EmailInputDefaults.DefaultSignatureInputs,
                    }
                ),
                /* SCHEDULE ANNOUNCEMENT */
                new EmailType(
                    displayName: "Schedule Announcement",
                    inputs: new List<IEmailInputs>
                    {
                        new EmailDetailsInputs(
                            Subject:
                                "Climbing Tower Schedule: [SCHEDULE NAME]"
                        ),

                        new CustomHeaderInputs(
                            OrganizationName:
                                EmailInputConstants.OrganizationName,
                            HeaderLabel:
                                "CLIMBING TOWER SCHEDULE",
                            EmailHeading:
                                "NEW SCHEDULE AVAILABLE",
                            HeaderSubtitle:
                                "[START DATE] THROUGH [END DATE]",
                            HeaderImageUrl:
                                EmailImageSources.Default_HeaderImage
                        ),

                        new CustomBodyInputs(
                            RecipientGreeting:
                                EmailInputConstants.TowerTeamGreeting,
                            EmailBody: ""
                        ),

                        new CustomImageInputs(
                            ImageSource: "",
                            ImageAltText: ""
                        ),

                        new CustomRequestInputs(
                            RequestLabel:
                                "SCHEDULE",
                            RequestTitle:
                                "Review the Full Schedule",
                            RequestBody:
                                "Use the button below to open the " +
                                "current schedule. Please review the " +
                                "entire schedule, including any shifts " +
                                "that are currently marked as missing.",
                            RequestButton:
                                "View Schedule",
                            RequestLink:
                                ""
                        ),

                        EmailInputDefaults.DefaultSignatureInputs
                    }
                ),
            };
        }
    }
}