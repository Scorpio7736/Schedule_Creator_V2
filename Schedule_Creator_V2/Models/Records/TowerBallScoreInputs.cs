using Schedule_Creator_V2.Models.Interfaces;
using System.Collections.Generic;

namespace Schedule_Creator_V2.Models.Records
{
    public record TowerBallScoreInputs(
        Dictionary<int, TowerBallScoreEntry> Entries
    ) : IEmailInputs
    {
        public string GetEmailTypeName() => "Tower Ball Scores";
    }

    public sealed class TowerBallScoreEntry
    {
        public TowerBallScoreEntry(string name) => Name = name;
        public string Name { get; set; }
        public bool Included { get; set; }
        public int DisplayOrder { get; set; }
        public string Score { get; set; } = string.Empty;
    }
}
