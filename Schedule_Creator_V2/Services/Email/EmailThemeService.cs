
using Schedule_Creator_V2.Models.Records;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Schedule_Creator_V2.Services.Email
{
    public static class EmailThemeService
    {
        private const string StyleMarker =
            "{{EMAIL_STYLES}}";

        private const string ColorSchemeMarker =
            "{{COLOR_SCHEME}}";

        private const string StylesDirectory =
            "Styles";

        public static string GetStyleFile(
            EmailType emailType)
        {
            ArgumentNullException.ThrowIfNull(
                emailType);

            string templateName =
                emailType.templateFileName ?? "";

            if (templateName.Equals(
                "Lets_Glow_Email_Template.html",
                StringComparison.OrdinalIgnoreCase))
            {
                return "LetsGlowStyles.css";
            }

            if (templateName.Equals(
                "Tower_of_Power_Email_Template.html",
                StringComparison.OrdinalIgnoreCase))
            {
                return "TowerOfPowerStyles.css";
            }

            return "DefaultEmailStyles.css";
        }

        public static string GetColorScheme(
            EmailType emailType)
        {
            return GetStyleFile(emailType) ==
                "LetsGlowStyles.css"
                    ? "dark"
                    : "light";
        }

        public static string ApplyTheme(
            string html,
            EmailType emailType)
        {
            ArgumentNullException.ThrowIfNull(html);
            ArgumentNullException.ThrowIfNull(emailType);

            string styleFile =
                GetStyleFile(emailType);

            string cssPath = Path.Combine(
                AppContext.BaseDirectory,
                "Models",
                "Objects",
                "Emails",
                StylesDirectory,
                styleFile
            );

            if (!File.Exists(cssPath))
            {
                throw new FileNotFoundException(
                    "Email stylesheet not found.",
                    cssPath);
            }

            string css =
                File.ReadAllText(cssPath);

            if (!html.Contains(StyleMarker))
            {
                throw new InvalidOperationException(
                    "Email.html is missing the " +
                    StyleMarker + " placeholder.");
            }

            // Update the static HTML color values
            // before inserting the selected CSS.
            html = styleFile == "LetsGlowStyles.css"
                ? LetsGlowThemeService.Apply(html)
                : ApplyPalette(html, styleFile);

            html = html.Replace(
                StyleMarker,
                css,
                StringComparison.Ordinal);

            html = html.Replace(
                ColorSchemeMarker,
                GetColorScheme(emailType),
                StringComparison.Ordinal);

            return html;
        }

        private static string ApplyPalette(
            string html,
            string styleFile)
        {
            if (styleFile == "DefaultEmailStyles.css")
            {
                return html;
            }

            // These are the existing color tokens in
            // the standard email layout and generated
            // section HTML. Replace only hexadecimal
            // color tokens, not unrelated text.

            (string Old, string New)[] colors;

            colors = new[]
            {
                ("#eef3f1", "#f7cdbd"),
                ("#ffffff", "#fff9f4"),
                ("#0f5640", "#f15a24"),
                ("#bfdbd4", "#f7cdbd"),
                ("#f5f7f6", "#ffffff"),
                ("#54b948", "#f15a24"),
                ("#303936", "#625650"),
                ("#26312d", "#292425"),
                ("#4b5551", "#625650"),
                ("#111111", "#292425"),
                ("#eef7f4", "#fff9f4"),
                ("#397665", "#f15a24")
            };

            // Single-pass replacement prevents new
            // colors from being replaced a second time.
            return Regex.Replace(
                html,
                @"#[0-9a-fA-F]{6}\b",
                match =>
                {
                    foreach (var color in colors)
                    {
                        if (match.Value.Equals(
                            color.Old,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            return color.New;
                        }
                    }

                    return match.Value;
                });
        }
    }
}
