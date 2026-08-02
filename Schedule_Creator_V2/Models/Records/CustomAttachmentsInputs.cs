using Schedule_Creator_V2.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Creator_V2.Models.Records
{
    public record CustomAttachmentsInputs(
    string AttachmentsLabel,
    string AttachmentsIntro,
    List<string> AttachmentsList
) : IEmailInputs
    {
        public string GetEmailTypeName() => "Custom Attachments";
    }
}
