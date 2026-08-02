using Schedule_Creator_V2.Models;
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
                            Subject: ""
                        ),

                        new CustomHeaderInputs(
                            OrganizationName: "",
                            HeaderLabel: "",
                            EmailHeading: "",
                            HeaderSubtitle: ""
                        ),

                        new CustomBodyInputs(
                            RecipientGreeting: "",
                            EmailBody: ""
                        )
                    }
                )
            };
        }
    }
}