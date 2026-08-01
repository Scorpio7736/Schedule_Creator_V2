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
            bool isFlagged = false, 
            bool isMarkedImportant = false
        )
        {
            sendTo = sendTo;
            sendCc = sendCc;
            subject = subject;
            this.isFlagged = isFlagged;
            this.isMarkedImportant = isMarkedImportant;
            _preHeaderText = "";
            _organizationLabel = "University Recreation";
            _headerLabel = "UWGB Climbing Tower";
            _emailHeading = "Email Heading";
            _headerSubtitle = "A short Summary or the subject of the email";
            _footerOrganization = "University of Wisconsin Green Bay";
            _footerWebsiteLink = "https://www.uwgb.edu/urec/adventure/climbing/";
            _footerText = "You are receiving this message because you are affiliated with the UWGB Climbing Tower.";
        }

        public List<Staff> SendTo
        {
            get { return sendTo; }
            set { sendTo = value; }
        }

        public List<Staff> SendCc
        {
            get { return sendCc; }
            set { sendCc = value; }
        }

        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }

        public bool IsFlagged
        {
            get { return isFlagged; }
            set { isFlagged = value; }
        }

        public bool IsMarkedImportant
        {
            get { return isMarkedImportant; }
            set { isMarkedImportant = value; }
        }

        public string preHeaderText
        {
            get { return preHeaderText; }
            set { preHeaderText = value; }
        }

        public string organizationLabel
        {
            get { return organizationLabel; }
            set { organizationLabel = value; }
        }

        public string headerLabel
        {
            get { return headerLabel; }
            set { headerLabel = value; }
        }

        public string emailHeading
        {
            get { return emailHeading; }
            set { emailHeading = value; }
        }

        public string headerSubtitle
        {
            get { return headerSubtitle; }
            set { headerSubtitle = value; }
        }

        public string footerOrganization
        {
            get { return footerOrganization; }
            set { footerOrganization = value; }
        }

        public string footerWebsiteLink
        {
            get { return footerWebsiteLink; }
            set { footerWebsiteLink = value; }
        } 

        public string footerText
        {
            get { return footerText; }
            set { footerText = value; }
        }


    }
}
