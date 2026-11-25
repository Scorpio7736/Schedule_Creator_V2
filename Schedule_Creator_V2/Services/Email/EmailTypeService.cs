using Schedule_Creator_V2.Models.Constants;
using Schedule_Creator_V2.Models.Defaults;
using Schedule_Creator_V2.Models.Interfaces;
using Schedule_Creator_V2.Models.Records;

namespace Schedule_Creator_V2.Services.Email
{
    public static class EmailTypeService
    {
        public static List<EmailType> CreateEmailTypes()
        {
            return new List<EmailType>
            {
                // =================================================
                // CUSTOM
                // =================================================

                new EmailType(
                    displayName:
                        "Custom",

                    inputs: new List<IEmailInputs>
                    {
                        new EmailDetailsInputs(
                            Subject:
                                ""
                        ),

                        new CustomHeaderInputs(
                            OrganizationName:
                                EmailInputConstants.OrganizationName,

                            HeaderLabel:
                                "",

                            EmailHeading:
                                "",

                            HeaderSubtitle:
                                "",

                            HeaderImageUrl:
                                EmailImageSources.Default_HeaderImage
                        ),

                        new CustomBodyInputs(
                            RecipientGreeting:
                                EmailInputConstants.TowerTeamGreeting,

                            EmailBody:
                                ""
                        ),

                        EmailInputDefaults
                            .DefaultSignatureInputs,

                        EmailInputDefaults
                            .DefaultFooterInputs
                    },

                    allowSectionEditing:
                        true
                ),

                
                // =================================================
                // HIRING & RECRUITMENT
                // =================================================

                // -------------------------------------------------
                // INTERVIEW OFFER
                // -------------------------------------------------

                new EmailType(
                    displayName:
                        "Interview Offer",

                    inputs: new List<IEmailInputs>
                    {
                        new EmailDetailsInputs(
                            Subject:
                                "UREC Outdoors | Interview Invitation"
                        ),

                        new CustomHeaderInputs(
                            OrganizationName:
                                EmailInputConstants.OrganizationName,

                            HeaderLabel:
                                "UREC OUTDOORS RECRUITMENT",

                            EmailHeading:
                                "INTERVIEW INVITATION",

                            HeaderSubtitle:
                                "JOIN US FOR AN INTERVIEW!",

                            HeaderImageUrl:
                                EmailImageSources.WelcomToTheTowerTeamHeader
                        ),

                        new CustomBodyInputs(
                            RecipientGreeting:
                                "Hi [RECIPIENT NAME],",

                            EmailBody:
                                "Thank you for applying for employment " +
                                "with UREC Outdoors! We have reviewed " +
                                "your application, and we would like to " +
                                "offer you an interview regarding " +
                                "employment for the position(s) listed below.\n\n" +

                                "Please let us know if you are still " +
                                "interested in a 30-minute virtual " +
                                "interview. You may reply to this email " +
                                "with one of the available times below. " +
                                "If none of these options work for you, " +
                                "feel free to send us a few dates and " +
                                "times that would be convenient.\n\n" +

                                "Interview Availability ():\n" +
                                "• Monday, : \n" +
                                "• Tuesday, : \n" +
                                "• Wednesday, : \n" +
                                "• Thursday, : \n" +
                                "• Friday, : \n\n" +

                                "Please choose any 30-minute opening " +
                                "within these windows. All times are " +
                                "Central Time. You can find the " +
                                "selected position descriptions " +
                                "and employment details below."
                        ),

                        new InterviewJobSelectionInputs(
                            SelectedJobs: new List<string>
                            {
                                InterviewJobCatalog.ClimbingTowerAttendant
                            }
                        ),

                        EmailInputDefaults
                            .DefaultSignatureInputs
                    }
                ),


                // -------------------------------------------------
                // POSITION OFFER
                // -------------------------------------------------

                new EmailType(
                    displayName:
                        "Position Offer",

                    inputs: new List<IEmailInputs>
                    {
                        new EmailDetailsInputs(
                            Subject:
                                "UREC Outdoors | Join Our Team!"
                        ),

                        new CustomHeaderInputs(
                            OrganizationName:
                                EmailInputConstants.OrganizationName,

                            HeaderLabel:
                                "UREC OUTDOORS RECRUITMENT",

                            EmailHeading:
                                "POSITION OFFER",

                            HeaderSubtitle:
                                "WE WOULD LOVE TO WELCOME YOU TO THE TEAM!",

                            HeaderImageUrl:
                                EmailImageSources.WelcomToTheTowerTeamHeader
                        ),

                        new CustomBodyInputs(
                            RecipientGreeting:
                                "Hi [RECIPIENT NAME],",

                            EmailBody:
                                "Thank you for taking the time to interview " +
                                "with UREC Outdoors! After reviewing our " +
                                "applicants, we are excited to offer you " +
                                "employment in the position(s) listed below.\n\n" +

                                "We enjoyed learning more about you and " +
                                "your interest in joining our team. We " +
                                "believe you would make a great addition " +
                                "to UREC Outdoors!\n\n" +

                                "Accepting Your Offer:\n" +
                                "If you are interested in accepting this " +
                                "offer, please respond to this email to " +
                                "confirm. After receiving your response, " +
                                "we will begin onboarding and share " +
                                "information about your next steps, " +
                                "required training, and certifications.\n\n" +

                                "Employment Information:\n" +
                                "• Starting Wage: $12.25/hour, with annual " +
                                "merit-based raises.\n" +
                                "• Maximum Hours: 25 per week.\n" +
                                "• Minimum Hours: 3 per week for regularly " +
                                "scheduled staff.\n" +
                                "• Substitute Staff: At least one shift " +
                                "per month, when applicable.\n" +
                                "• Certifications: CPR/AED/First Aid " +
                                "(provided by UREC Outdoors).\n\n" +

                                "Additional position-specific benefits, " +
                                "requirements, and descriptions are " +
                                "included below.\n\n" +

                                "Thank you again for your interest in " +
                                "UREC Outdoors. We look forward to hearing " +
                                "from you and hope to welcome you to " +
                                "our team!"
                        ),

                        new InterviewJobSelectionInputs(
                            SelectedJobs: new List<string>
                            {
                                InterviewJobCatalog.ClimbingTowerAttendant
                            }
                        ),

                        EmailInputDefaults
                            .DefaultSignatureInputs
                    }
                ),


                // -------------------------------------------------
                // POSITION REJECTION
                // -------------------------------------------------

                new EmailType(
                    displayName:
                        "Position Rejection",

                    inputs: new List<IEmailInputs>
                    {
                        new EmailDetailsInputs(
                            Subject:
                                "Climbing Tower - Application Update"
                        ),

                        new CustomHeaderInputs(
                            OrganizationName:
                                EmailInputConstants.OrganizationName,

                            HeaderLabel:
                                "CLIMBING TOWER RECRUITMENT",

                            EmailHeading:
                                "APPLICATION UPDATE",

                            HeaderSubtitle:
                                "THANK YOU FOR YOUR INTEREST",

                            HeaderImageUrl:
                                EmailImageSources.Default_HeaderImage
                        ),

                        new CustomBodyInputs(
                            RecipientGreeting:
                                "Hello [RECIPIENT NAME],",

                            EmailBody:
                                "Thank you for your interest in joining " +
                                "the UREC Outdoors Climbing Tower Team " +
                                "and for taking the time to participate " +
                                "in our hiring process.\n\n" +

                                "After careful consideration, we have " +
                                "decided to move forward with other " +
                                "candidates for the Climbing Tower " +
                                "Attendant position.\n\n" +

                                "We sincerely appreciate the time and " +
                                "effort you put into your application " +
                                "and interview. We had the opportunity " +
                                "to meet several great candidates, " +
                                "making this a difficult decision.\n\n" +

                                "We encourage you to consider applying " +
                                "for future opportunities with " +
                                "UREC Outdoors.\n\n" +

                                "Thank you again for your interest, " +
                                "and we wish you the best in your " +
                                "future endeavors!"
                        ),

                        EmailInputDefaults
                            .DefaultSignatureInputs
                    }
                ),



                // =================================================
                // ANNOUNCEMENTS
                // =================================================

                new EmailType(
                    displayName:
                        "Announcements",

                    inputs: new List<IEmailInputs>
                    {
                        new EmailDetailsInputs(
                            Subject:
                                ""
                        ),

                        new CustomHeaderInputs(
                            OrganizationName:
                                EmailInputConstants.OrganizationName,

                            HeaderLabel:
                                "TOWER TEAM ANNOUNCEMENTS",

                            EmailHeading:
                                "",

                            HeaderSubtitle:
                                "",

                            HeaderImageUrl:
                                EmailImageSources.UpcomingClassHeader
                        ),

                        new CustomBodyInputs(
                            RecipientGreeting:
                                EmailInputConstants.TowerTeamGreeting,

                            EmailBody:
                                "Here are the latest announcements for the " +
                                "Tower Team. Please review the information " +
                                "below and stay up to date with our " +
                                "activities and events."
                        ),

                        new CustomAnnouncementsInputs(
                            AnnouncementsLabel:
                                "ANNOUNCEMENTS",

                            AnnouncementsIntro:
                                "Please find the latest updates and " +
                                "important information for the Tower Team below.",

                            AnnouncementsList:
                            [
                                "Announcement 1: Details about the first announcement.",
                                "Announcement 2: Details about the second announcement.",
                                "Announcement 3: Details about the third announcement."
                            ]
                        ),

                        EmailInputDefaults
                            .DefaultSignatureInputs
                    }
                ),


                // =================================================
                // TOWER BALL SCORE UPDATE
                // =================================================

                new EmailType(
                    displayName:
                        "Tower Ball Score Update",

                    inputs: new List<IEmailInputs>
                    {
                        new EmailDetailsInputs(
                            Subject:
                                "Climbing Tower | Tower Ball Score Update"
                        ),

                        new CustomHeaderInputs(
                            OrganizationName:
                                EmailInputConstants.OrganizationName,

                            HeaderLabel:
                                "CLIMBING TOWER",

                            EmailHeading:
                                "TOWER BALL SCORE UPDATE",

                            HeaderSubtitle:
                                "LATEST SCORES AND STANDINGS",

                            HeaderImageUrl:
                                EmailImageSources.TowerBallHeader
                        ),

                        new CustomBodyInputs(
                            RecipientGreeting:
                                EmailInputConstants.TowerTeamGreeting,

                            EmailBody:
                                "Here is the latest Tower Ball score update!\n\n" +
                                "Keep up the great work!"
                        ),

                        new TowerBallScoreInputs(new Dictionary<int, TowerBallScoreEntry>()),

                        EmailInputDefaults
                            .DefaultSignatureInputs
                    }
                ),


                // =================================================
                // NO REQUEST WELCOME
                // =================================================

                new EmailType(
                    displayName:
                        "(No Request) Welcome to the Climbing Tower Team",

                    inputs: new List<IEmailInputs>
                    {
                        new EmailDetailsInputs(
                            Subject:
                                "Welcome to the Climbing Tower Team"
                        ),

                        new CustomHeaderInputs(
                            OrganizationName:
                                EmailInputConstants.OrganizationName,

                            HeaderLabel:
                                "WELCOME TO THE CLIMBING TOWER TEAM",

                            EmailHeading:
                                "WELCOME TO THE CLIMBING TOWER TEAM",

                            HeaderSubtitle:
                                "WE ARE EXCITED TO HAVE YOU ON BOARD!",

                            HeaderImageUrl:
                                EmailImageSources.WelcomToTheTowerTeamHeader
                        ),

                        new CustomBodyInputs(
                            RecipientGreeting:
                                "Hello [RECIPIENT NAME],",

                            EmailBody:
                                "We are thrilled to welcome you to the " +
                                "Climbing Tower Team! Your dedication and " +
                                "enthusiasm are invaluable to our mission " +
                                "of providing a safe and enjoyable climbing " +
                                "experience for all. We look forward to " +
                                "working with you and seeing the positive " +
                                "impact you'll make on our team and the " +
                                "climbing community."
                        ),

                        EmailInputDefaults
                            .DefaultSignatureInputs
                    }
                ),


                // =================================================
                // SCHEDULE ANNOUNCEMENT
                // =================================================

                new EmailType(
                    displayName:
                        "Schedule Announcement",

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

                            EmailBody:
                                ""
                        ),

                        new CustomImageInputs(
                            ImageSource:
                                "",

                            ImageAltText:
                                ""
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

                        EmailInputDefaults
                            .DefaultSignatureInputs
                    }
                ),


                // =================================================
                // LET'S GLOW SCHEDULE
                // =================================================

                new EmailType(
                    displayName:
                        "Let's Glow Schedule",

                    inputs: new List<IEmailInputs>
                    {
                        new EmailDetailsInputs(
                            Subject:
                                "Let's Glow Schedule"
                        ),

                        new CustomHeaderInputs(
                            OrganizationName:
                                "UREC OUTDOORS",

                            HeaderLabel:
                                "GLOW CLIMB",

                            EmailHeading:
                                "LET'S GLOW!",

                            HeaderSubtitle:
                                "Same tower. Blacklights on. Everything glows.",

                            HeaderImageUrl:
                                EmailImageSources.Default_HeaderImage
                        ),

                        new CustomBodyInputs(
                            RecipientGreeting:
                                EmailInputConstants.TowerTeamGreeting,

                            EmailBody:
                                ""
                        ),

                        new CustomImageInputs(
                            ImageSource:
                                "",

                            ImageAltText:
                                "Let's Glow Schedule"
                        ),

                        EmailInputDefaults
                            .DefaultSignatureInputs
                    },

                    templateFileName:
                        "Lets_Glow_Email_Template.html"
                ),


                // =================================================
                // LET'S GLOW ANNOUNCEMENTS
                // =================================================

                new EmailType(
                    displayName:
                        "Let's Glow Announcements",

                    inputs: new List<IEmailInputs>
                    {
                        new EmailDetailsInputs(
                            Subject:
                                "Let's Glow Announcements"
                        ),

                        new CustomHeaderInputs(
                            OrganizationName:
                                "UREC OUTDOORS",

                            HeaderLabel:
                                "GLOW CLIMB",

                            EmailHeading:
                                "LET'S GLOW!",

                            HeaderSubtitle:
                                "Same tower. Blacklights on. Everything glows.",

                            HeaderImageUrl:
                                EmailImageSources.Default_HeaderImage
                        ),

                        new CustomBodyInputs(
                            RecipientGreeting:
                                EmailInputConstants.TowerTeamGreeting,

                            EmailBody:
                                ""
                        ),

                        new CustomAnnouncementsInputs(
                            AnnouncementsLabel:
                                "ANNOUNCEMENTS",

                            AnnouncementsIntro:
                                "",

                            AnnouncementsList:
                            [
                                ""
                            ]
                        ),

                        EmailInputDefaults
                            .DefaultSignatureInputs
                    },

                    templateFileName:
                        "Lets_Glow_Email_Template.html"
                ),


                // =================================================
                // LET'S GLOW REQUESTS
                // =================================================

                new EmailType(
                    displayName:
                        "Let's Glow Requests",

                    inputs: new List<IEmailInputs>
                    {
                        new EmailDetailsInputs(
                            Subject:
                                "Let's Glow Requests"
                        ),

                        new CustomHeaderInputs(
                            OrganizationName:
                                "UREC OUTDOORS",

                            HeaderLabel:
                                "GLOW CLIMB",

                            EmailHeading:
                                "LET'S GLOW!",

                            HeaderSubtitle:
                                "Same tower. Blacklights on. Everything glows.",

                            HeaderImageUrl:
                                EmailImageSources.Default_HeaderImage
                        ),

                        new CustomBodyInputs(
                            RecipientGreeting:
                                EmailInputConstants.TowerTeamGreeting,

                            EmailBody:
                                ""
                        ),

                        new CustomRequestInputs(
                            RequestLabel:
                                "REQUEST",

                            RequestTitle:
                                "",

                            RequestBody:
                                "",

                            RequestButton:
                                "",

                            RequestLink:
                                ""
                        ),

                        EmailInputDefaults
                            .DefaultSignatureInputs
                    },

                    templateFileName:
                        "Lets_Glow_Email_Template.html"
                ),


                // =================================================
                // TOWER OF POWER SCHEDULE
                // =================================================

                new EmailType(
                    displayName:
                        "Tower of Power Schedule",

                    inputs: new List<IEmailInputs>
                    {
                        new EmailDetailsInputs(
                            Subject:
                                "Tower of Power Schedule"
                        ),

                        new CustomHeaderInputs(
                            OrganizationName:
                                "UREC OUTDOORS",

                            HeaderLabel:
                                "CLIMBING COMPETITION",

                            EmailHeading:
                                "TOWER OF POWER",

                            HeaderSubtitle:
                                "Climb hard. Rack up points. Take the tower.",

                            HeaderImageUrl:
                                EmailImageSources.Default_HeaderImage
                        ),

                        new CustomBodyInputs(
                            RecipientGreeting:
                                EmailInputConstants.TowerTeamGreeting,

                            EmailBody:
                                ""
                        ),

                        new CustomImageInputs(
                            ImageSource:
                                "",

                            ImageAltText:
                                "Tower of Power Schedule"
                        ),

                        EmailInputDefaults
                            .DefaultSignatureInputs
                    },

                    templateFileName:
                        "Tower_of_Power_Email_Template.html"
                ),


                // =================================================
                // TOWER OF POWER ANNOUNCEMENTS
                // =================================================

                new EmailType(
                    displayName:
                        "Tower of Power Announcements",

                    inputs: new List<IEmailInputs>
                    {
                        new EmailDetailsInputs(
                            Subject:
                                "Tower of Power Announcements"
                        ),

                        new CustomHeaderInputs(
                            OrganizationName:
                                "UREC OUTDOORS",

                            HeaderLabel:
                                "CLIMBING COMPETITION",

                            EmailHeading:
                                "TOWER OF POWER",

                            HeaderSubtitle:
                                "Climb hard. Rack up points. Take the tower.",

                            HeaderImageUrl:
                                EmailImageSources.Default_HeaderImage
                        ),

                        new CustomBodyInputs(
                            RecipientGreeting:
                                EmailInputConstants.TowerTeamGreeting,

                            EmailBody:
                                ""
                        ),

                        new CustomAnnouncementsInputs(
                            AnnouncementsLabel:
                                "ANNOUNCEMENTS",

                            AnnouncementsIntro:
                                "",

                            AnnouncementsList:
                            [
                                ""
                            ]
                        ),

                        EmailInputDefaults
                            .DefaultSignatureInputs
                    },

                    templateFileName:
                        "Tower_of_Power_Email_Template.html"
                ),


                // =================================================
                // TOWER OF POWER REQUESTS
                // =================================================

                new EmailType(
                    displayName:
                        "Tower of Power Requests",

                    inputs: new List<IEmailInputs>
                    {
                        new EmailDetailsInputs(
                            Subject:
                                "Tower of Power Requests"
                        ),

                        new CustomHeaderInputs(
                            OrganizationName:
                                "UREC OUTDOORS",

                            HeaderLabel:
                                "CLIMBING COMPETITION",

                            EmailHeading:
                                "TOWER OF POWER",

                            HeaderSubtitle:
                                "Climb hard. Rack up points. Take the tower.",

                            HeaderImageUrl:
                                EmailImageSources.Default_HeaderImage
                        ),

                        new CustomBodyInputs(
                            RecipientGreeting:
                                EmailInputConstants.TowerTeamGreeting,

                            EmailBody:
                                ""
                        ),

                        new CustomRequestInputs(
                            RequestLabel:
                                "REQUEST",

                            RequestTitle:
                                "",

                            RequestBody:
                                "",

                            RequestButton:
                                "",

                            RequestLink:
                                ""
                        ),

                        EmailInputDefaults
                            .DefaultSignatureInputs
                    },

                    templateFileName:
                        "Tower_of_Power_Email_Template.html"
                )
            };
        }
    }
}