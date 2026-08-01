namespace Schedule_Creator_V2.Models.BaseClasses
{
    internal class EmailBaseClass
    {
        // EMAIL VALUES
        private List<Staff> sendTo;
        public List<Staff> sendCc;
        public string subject;
        public bool isFlagged;
        public bool isMarkedImportant;
        // HEADER VARIABLES
        public string preHeaderText;
        public string organizationLabel;
        public string headerLabel;
        public string emailHeading;
        public string headerSubtitle;
        // FOOTER VARIABLES
        public string footerOrganization;
        public string footerWebsiteLink;
        public string footerText;

        public EmailBaseClass(
            List<Staff> sendTo,
            List<Staff> sendCc,
            string subject,
            string emailHeading,
            string headerSubtitle,
            bool? isFlagged = false,
            bool? isMarkedImportant = false            
        )
        {
            sendTo = sendTo;
            sendCc = sendCc;
            subject = subject;
            emailHeading = emailHeading;
            headerSubtitle = headerSubtitle;
            this.isFlagged = isFlagged ?? false;
            this.isMarkedImportant = isMarkedImportant ?? false;
            preHeaderText = "";
            organizationLabel = "University Recreation";
            headerLabel = "UWGB Climbing Tower";
            footerOrganization = "University of Wisconsin Green Bay";
            footerWebsiteLink = "https://www.uwgb.edu/urec/adventure/climbing/";
            footerText = "You are receiving this message because you are affiliated with the UWGB Climbing Tower.";
        }
        public List<Staff> SendTo { get; set; }
        public List<Staff> SendCc { get; set; }
        public string Subject { get; set; }
        public bool IsFlagged { get; set; }
        public bool IsMarkedImportant { get; set; }
    }
}
