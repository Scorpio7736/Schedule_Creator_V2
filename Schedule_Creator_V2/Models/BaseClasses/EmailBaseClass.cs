using Schedule_Creator_V2.Models;
using System.Collections.Generic;

namespace Schedule_Creator_V2.Models.BaseClasses
{
    internal class EmailBaseClass
    {
        // EMAIL VALUES

        public List<Staff> SendTo { get; set; }

        public List<Staff> SendCc { get; set; }

        public string Subject { get; set; }

        public bool IsFlagged { get; set; }

        public bool IsMarkedImportant { get; set; }

        // HEADER VALUES

        public string PreHeaderText { get; set; }

        public string OrganizationLabel { get; set; }

        public string HeaderLabel { get; set; }

        public string EmailHeading { get; set; }

        public string HeaderSubtitle { get; set; }

        // FOOTER VALUES

        public string FooterOrganization { get; set; }

        public string FooterWebsiteLink { get; set; }

        public string FooterText { get; set; }

        public EmailBaseClass(
            List<Staff>? sendTo,
            List<Staff>? sendCc,
            string? subject,
            bool? isFlagged = false,
            bool? isMarkedImportant = false)
        {
            SendTo =
                sendTo ?? new List<Staff>();

            SendCc =
                sendCc ?? new List<Staff>();

            Subject =
                subject ?? string.Empty;

            IsFlagged =
                isFlagged ?? false;

            IsMarkedImportant =
                isMarkedImportant ?? false;

            PreHeaderText =
                string.Empty;

            OrganizationLabel =
                "University Recreation";

            HeaderLabel =
                "UWGB Climbing Tower";

            EmailHeading =
                string.Empty;

            HeaderSubtitle =
                string.Empty;

            FooterOrganization =
                "University of Wisconsin–Green Bay";

            FooterWebsiteLink =
                string.Empty;

            FooterText =
                string.Empty;
        }
    }
}