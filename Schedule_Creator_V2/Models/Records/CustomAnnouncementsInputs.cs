using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Creator_V2.Models.Records
{
    using Schedule_Creator_V2.Models.Interfaces;
    using System.Collections.Generic;

    public record CustomAnnouncementsInputs(
        string AnnouncementsLabel,
        string AnnouncementsIntro,
        List<string> AnnouncementsList
    ) : IEmailInputs
    {
        public string GetEmailTypeName() => "Custom Announcements";
    }
}
