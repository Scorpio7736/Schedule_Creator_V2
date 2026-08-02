using Schedule_Creator_V2.Models.Interfaces;

namespace Schedule_Creator_V2.Models.Records
{
    public class EmailDetailsInputs : IEmailInputs
    {
        public string Subject { get; set; }

        public EmailDetailsInputs(string Subject)
        {
            this.Subject = Subject;
        }

        public string GetEmailTypeName()
        {
            return "Email Details";
        }
    }
}