namespace Schedule_Creator_V2.Models.Objects
{
    public class TestEmail : EmailBaseClass
    {
        protected string _recipientGreeting;
        protected string _emailBody;
        public TestEmail(

            ) : base(
            new List<Staff>(),
            new List<Staff>(),
            "Test Email Subject"
        )
        {
            
        }
    
    }
}
