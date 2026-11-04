using System;
using System.Collections.Generic;
using System.Linq;

namespace Schedule_Creator_V2.Models.Records
{
    public record InterviewJobDefinition(
        string Title,
        string Description,
        IReadOnlyList<string> Details);

    public static class InterviewJobCatalog
    {
        public const string ClimbingTowerAttendant = "Climbing Tower Attendant";
        public const string ShorewoodAttendant = "Shorewood Outdoor Adventure Center Attendant";
        public const string TripCoordinator = "Student Adventure Trip Coordinator";

        public static readonly IReadOnlyList<InterviewJobDefinition> Jobs =
            new List<InterviewJobDefinition>
            {
                new(
                    ClimbingTowerAttendant,
                    "Promote an active and energetic environment by teaching and advocating " +
                    "safe climbing skills. Use customer service skills to interact with users " +
                    "and to communicate proper equipment usage and climbing techniques. " +
                    "Belay certification not required to begin, but will be offered during employment.",
                    new List<string>
                    {
                        "Pay and hours: Starting wage is $12.25/hour with annual raises; up to 25 hours per week. Regularly scheduled staff work at least 3 hours per week.",
                        "Substitute shifts: Substitutes must work at least one substitute shift per month when applicable.",
                        "Certifications and training: CPR/AED/First Aid and Top Rope Belay are required when applicable. UREC Outdoors provides training and certification opportunities; prior belay certification is not required to begin employment.",
                        "Climbing perks: Off-hours climbing with other UREC Outdoors staff and semester bonus opportunities (contingent on climbing at the tower).",
                        "Equipment benefits: Free personal equipment rentals through UREC Outdoors and Pro-Deal membership offering discounts on outdoor equipment."
                    }
                ),
                new(
                    ShorewoodAttendant,
                    "Responsible for the day-to-day operations of the UREC Outdoors Outdoor Adventure Center, " +
                    "including the organization, access, and security of the equipment room during each shift.",
                    new List<string>
                    {
                        "Conduct equipment rentals, sales, and trip/clinic registrations.",
                        "Manage point of sale, equipment use, and loading rental equipment.",
                        "Support program marketing and other duties related to the center's operations."
                    }
                ),
                new(
                    TripCoordinator,
                    "Responsible for providing a safe and enjoyable experience for patrons on outdoor " +
                    "adventure trips. The coordinator team works with other trip leaders and the " +
                    "program coordinator to plan trips and see them to fruition.",
                    new List<string>
                    {
                        "Research and study trip locations; check the condition of and pack equipment for trips.",
                        "Determine itineraries and complete risk management forms.",
                        "Conduct participant meetings covering food planning, gear needs, hygiene, Leave No Trace (LNT) principles, and other aspects of successful and safe backcountry trips.",
                        "Requires a minimum of one semester of training."
                    }
                )
            };

        public static IReadOnlyList<InterviewJobDefinition> Resolve(
            IEnumerable<string>? selectedTitles)
        {
            HashSet<string> selected = new HashSet<string>(
                selectedTitles ?? Enumerable.Empty<string>(),
                StringComparer.OrdinalIgnoreCase);

            return Jobs.Where(job => selected.Contains(job.Title)).ToList();
        }
    }
}
