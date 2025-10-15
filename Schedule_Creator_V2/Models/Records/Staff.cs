using Schedule_Creator_V2.Models.Enums;

namespace Schedule_Creator_V2.Models
{
    public record Staff(int id, string fName, string mName, string lName, Positions position, string email, bool isBelayCertified, DateOnly? certifiedOn, DateOnly? expiresOn)
    {
        public string displayName => $"{fName} {lName}";
        public string certRange => $"{certifiedOn} - {expiresOn}";
    };
}
