namespace Schedule_Creator_V2.Models.BaseClasses
{
    internal class EmailBaseClass
    {
        private List<Staff> sendTo;
        public List<Staff> sendCc;
        public string subject;
        public bool isFlagged;
        public bool isMarkedImportant;

        // HEADER VARIABLES
        protected string _preHeaderText;
        protected string _organizationLabel;
        protected string _headerLabel;
        protected string _emailHeading;
        protected string _headerSubtitle;
        // FOOTER VARIABLES
        protected string _footerOrganization;
        protected string _footerWebsiteLink;
        protected string _footerText;

        public EmailBaseClass(
            List<Staff> sendTo, 
            List<Staff> sendCc, 
            string subject, 
            bool? isFlagged = false, 
            bool? isMarkedImportant = false
        )
        {
            sendTo = sendTo;
            sendCc = sendCc;
            subject = subject;
            this.isFlagged = isFlagged ?? false;
            this.isMarkedImportant = isMarkedImportant ?? false;
            _preHeaderText = "";
            _organizationLabel = "University Recreation";
            _headerLabel = "UWGB Climbing Tower";
            _emailHeading = "Email Heading";
            _headerSubtitle = "A short Summary or the subject of the email";
            _footerOrganization = "University of Wisconsin Green Bay";
            _footerWebsiteLink = "https://www.uwgb.edu/urec/adventure/climbing/";
            _footerText = "You are receiving this message because you are affiliated with the UWGB Climbing Tower.";
        }
        public List<Staff> SendTo { get; set; }
        public List<Staff> SendCc { get; set; }
        public string Subject { get; set; }
        public bool IsFlagged { get; set; }
        public bool IsMarkedImportant { get; set; }
        public string preHeaderText { get; set; }
        public string organizationLabel { get; set; }
        public string headerLabel { get; set; }
        public string emailHeading { get; set; }
        public string headerSubtitle { get; set; }
        public string footerOrganization { get; set; }
        public string footerWebsiteLink { get; set; }
        public string footerText { get; set; }


    }
}
