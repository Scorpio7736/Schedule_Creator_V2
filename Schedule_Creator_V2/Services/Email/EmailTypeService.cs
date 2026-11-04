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
                                "UREC Outdoors - Climbing Tower | Interview Invitation"
                        ),

                        new CustomHeaderInputs(
                            OrganizationName:
                                EmailInputConstants.OrganizationName,

                            HeaderLabel:
                                "CLIMBING TOWER RECRUITMENT",

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
                                "employment as a Climbing Tower Attendant.\n\n" +

                                "Please let us know if you are still " +
                                "interested. You can find the Climbing " +
                                "Tower position description and " +
                                "employment details below, along with " +
                                "options to schedule your interview."
                        ),

                        new CustomAnnouncementsInputs(
                            AnnouncementsLabel:
                                "CLIMBING TOWER ATTENDANT",

                            AnnouncementsIntro:
                                "Promote an active and energetic " +
                                "environment by teaching and advocating " +
                                "safe climbing skills. Use customer " +
                                "service skills to interact with users " +
                                "and to communicate proper equipment " +
                                "usage and climbing techniques. Belay " +
                                "certification not required to begin, " +
                                "but will be offered during employment.",

                            AnnouncementsList:
                            [
                                "Pay and hours: Starting wage is $12.25/hour with annual raises; up to 25 hours per week. Regularly scheduled staff work at least 3 hours per week.",
                                "Substitute shifts: Substitutes must work at least one substitute shift per month when applicable.",
                                "Certifications and training: CPR/AED/First Aid and Top Rope Belay are required when applicable. UREC Outdoors provides training and certification opportunities; prior belay certification is not required to begin employment.",
                                "Climbing perks: Off-hours climbing with other UREC Outdoors staff and semester bonus opportunities (contingent on climbing at the tower).",
                                "Equipment benefits: Free personal equipment rentals through UREC Outdoors and Pro-Deal membership offering discounts on outdoor equipment."
                            ]
                        ),

                        new CustomRequestInputs(
                            RequestLabel:
                                "INTERVIEW AVAILABILITY",

                            RequestTitle:
                                "Schedule Your Interview",

                            RequestBody:
                                "Please let us know your availability " +
                                "for a virtual interview. Please choose " +
                                "one of the available 30-minute openings " +
                                "using the scheduling link below. If " +
                                "none of the listed times work for you, " +
                                "reply with a few dates and times " +
                                "that would be convenient.",

                            RequestButton:
                                "Schedule Your Interview",

                            // Paste your Microsoft Bookings meeting-type
                            // URL into this field in the email builder.
                            // An empty URL keeps the button hidden.
                            RequestLink:
                                ""
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
                                "Climbing Tower - Position Offer"
                        ),

                        new CustomHeaderInputs(
                            OrganizationName:
                                EmailInputConstants.OrganizationName,

                            HeaderLabel:
                                "CLIMBING TOWER RECRUITMENT",

                            EmailHeading:
                                "CONGRATULATIONS!",

                            HeaderSubtitle:
                                "WE WOULD LOVE TO WELCOME YOU TO THE TEAM!",

                            HeaderImageUrl:
                                EmailImageSources.WelcomToTheTowerTeamHeader
                        ),

                        new CustomBodyInputs(
                            RecipientGreeting:
                                "Hello [RECIPIENT NAME],",

                            EmailBody:
                                "Congratulations!\n\n" +

                                "We are excited to offer you a position " +
                                "as a Climbing Tower Attendant with " +
                                "UREC Outdoors at UW-Green Bay!\n\n" +

                                "Throughout the application and interview " +
                                "process, we were impressed by your " +
                                "qualifications, enthusiasm, and potential " +
                                "to contribute to our team.\n\n" +

                                "We believe you will be a great addition " +
                                "to the Climbing Tower, and we look " +
                                "forward to working with you.\n\n" +

                                "Next Steps:\n\n" +

                                "Please respond to this email by " +
                                "[RESPONSE DEADLINE] to let us know if " +
                                "you would like to accept the position.\n\n" +

                                "Once we receive your confirmation, we " +
                                "will provide additional information " +
                                "regarding the hiring process, required " +
                                "training, and onboarding.\n\n" +

                                "Congratulations again, and we hope to " +
                                "welcome you to the Climbing Tower " +
                                "Team soon!"
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