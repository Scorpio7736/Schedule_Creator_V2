using System;
using System.IO;

namespace Schedule_Creator_V2.Models.Constants
{
    public static class EmailImageSources
    {
        public static readonly string Default_HeaderImage =
            "data:image/jpeg;base64," +
            Convert.ToBase64String(
                File.ReadAllBytes(
                    Path.Combine(
                        AppContext.BaseDirectory,
                        "Images",
                        "EmailImages",
                        "Header_Default.jpg")));

        public static readonly string UpcomingClassHeader =
            "data:image/jpeg;base64," +
            Convert.ToBase64String(
                File.ReadAllBytes(
                    Path.Combine(
                        AppContext.BaseDirectory,
                        "Images",
                        "EmailImages",
                        "Header_UpcomingClass.jpg")));

        public static readonly string WelcomToTheTowerTeamHeader =
            "data:image/jpeg;base64," +
            Convert.ToBase64String(
                File.ReadAllBytes(
                    Path.Combine(
                        AppContext.BaseDirectory,
                        "Images",
                        "EmailImages",
                        "Header_WelcomeToTheTeam.jpeg")));

        public static readonly string Default_FooterImage =
            "data:image/png;base64," +
            Convert.ToBase64String(
                File.ReadAllBytes(
                    Path.Combine(
                        AppContext.BaseDirectory,
                        "Images",
                        "EmailImages",
                        "Footer_Default.png")));
    }
}