using System.Text.RegularExpressions;
using Schedule_Creator_V2.Models.Constants;

namespace Schedule_Creator_V2.Services.Email;

/// <summary>Let’s Glow-only inline fallbacks for clients that discard stylesheet rules.</summary>
internal static class LetsGlowThemeService
{
    internal static string Apply(string html)
    {
        // Replace the standard daytime photo with a live-text neon hero.
        // User-selected header images are retained.
        html = Regex.Replace(html, @"<tr\b[^>]*\bid=""headerImageSection""[^>]*>.*?</tr>",
            match => match.Value.Contains(EmailImageSources.Default_HeaderImage, StringComparison.Ordinal)
                ? string.Empty : match.Value, RegexOptions.Singleline | RegexOptions.IgnoreCase);

        html = Regex.Replace(html, @"<[a-z][^>]*>", match =>
        {
            string tag = match.Value;
            string id = Attribute(tag, "id");
            string classes = Attribute(tag, "class");
            string style = Attribute(tag, "style");
            bool button = id == "requestButton";

            // Change only CSS values, never URLs, user text, or image data.
            style = Regex.Replace(style, @"(?<property>[\w-]+)\s*:\s*(?<value>[^;]+);?", declaration =>
            {
                string property = declaration.Groups["property"].Value;
                bool text = property.Equals("color", StringComparison.OrdinalIgnoreCase)
                    || property.Equals("-webkit-text-fill-color", StringComparison.OrdinalIgnoreCase);
                string value = Regex.Replace(declaration.Groups["value"].Value, @"#[a-fA-F0-9]{6}\b",
                    color => Map(color.Value, text, button));
                return property + ":" + value.Trim() + ";";
            });

            string? background = classes.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c switch
                {
                    "email-outer" => "#080615",
                    "background-white" => "#11102b",
                    "background-brand-green" => "#17133b",
                    "background-mint" => "#17133b",
                    "background-request" => "#080615",
                    "background-action" => "#9be447",
                    _ => null
                }).FirstOrDefault(c => c != null);
            if (background != null)
            {
                style += $"background-color:{background} !important;background-image:linear-gradient({background},{background}) !important;";
                if (!tag.StartsWith("<body", StringComparison.OrdinalIgnoreCase))
                    tag = SetAttribute(tag, "bgcolor", background);
            }
            else
            {
                string bgcolor = Attribute(tag, "bgcolor");
                if (bgcolor.Length > 0) tag = SetAttribute(tag, "bgcolor", Map(bgcolor, false, false));
            }

            if (id == "emailHeading")
                style += "font-size:48px;line-height:52px;letter-spacing:-1.5px;color:#f7f8ff !important;-webkit-text-fill-color:#f7f8ff !important;text-shadow:0 0 22px rgba(54,233,239,.35);";
            if (id == "headerLabel")
                style += "letter-spacing:3px;color:#f34fc1 !important;-webkit-text-fill-color:#f34fc1 !important;";
            if (id == "organizationName")
                style += "font-size:18px;line-height:24px;letter-spacing:.2px;";
            if (id == "headerSubtitle")
                style += "font-size:18px;line-height:28px;";
            if (button)
                style += "color:#11102b !important;-webkit-text-fill-color:#11102b !important;padding:16px 26px;";

            if (Regex.IsMatch(style, @"(?:^|;)\s*color\s*:\s*#", RegexOptions.IgnoreCase))
                tag = SetAttribute(tag, "class", (classes + (button ? " glow-button" : " glow-text")).Trim());
            if (style.Length > 0) tag = SetAttribute(tag, "style", style);
            return tag;
        }, RegexOptions.IgnoreCase);

        // Word-based Outlook does not use prefers-color-scheme. Its text gradient
        // provides the light-mode color, with an inverse fallback for dark mode.
        // Other clients never see these overrides.
        const string outlook = """
            <!--[if mso]>
            <style>.email-container{width:640px !important;} table,td{font-family:Arial,Helvetica,sans-serif !important;}</style>
            <![endif]-->
            <!--[if gte mso 16]>
            <style>
            .glow-text{color:#000000 !important;mso-style-textfill-type:gradient;mso-style-textfill-fill-gradientfill-stoplist:"0 #FFFFFF 0 100000,100000 #FFFFFF 0 100000";}
            .glow-button{color:#ffffff !important;mso-style-textfill-type:gradient;mso-style-textfill-fill-gradientfill-stoplist:"0 #11102B 1 100000,99000 #11102B 1 100000";}
            </style>
            <![endif]-->
            """;
        return html.Replace("</head>", outlook + "</head>", StringComparison.OrdinalIgnoreCase);
    }

    private static string Map(string value, bool text, bool button)
    {
        string color = value.ToLowerInvariant();
        if (text)
        {
            if (button) return "#11102b";
            return color switch
            {
                "#ffffff" or "#111111" => "#f7f8ff",
                "#0f5640" or "#397665" => "#36e9ef",
                "#bfdbd4" or "#54b948" => "#9be447",
                "#eef7f4" or "#303936" or "#26312d" or "#4b5551" => "#d9d8eb",
                "#f28c18" => "#f34fc1",
                _ => value
            };
        }
        return color switch
        {
            "#eef3f1" or "#f5f7f6" => "#080615",
            "#ffffff" => "#11102b",
            "#0f5640" or "#bfdbd4" => "#17133b",
            "#54b948" or "#f28c18" => "#9be447",
            "#397665" => "#37305b",
            _ => value
        };
    }

    private static string Attribute(string tag, string name) =>
        Regex.Match(tag, $@"\b{Regex.Escape(name)}\s*=\s*([""'])(?<value>.*?)\1",
            RegexOptions.Singleline | RegexOptions.IgnoreCase).Groups["value"].Value;

    private static string SetAttribute(string tag, string name, string value)
    {
        string pattern = $@"\b{Regex.Escape(name)}\s*=\s*([""']).*?\1";
        string attribute = name + "=\"" + value.Replace("\"", "&quot;") + "\"";
        return Regex.IsMatch(tag, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase)
            ? Regex.Replace(tag, pattern, _ => attribute, RegexOptions.Singleline | RegexOptions.IgnoreCase)
            : tag.Insert(tag.Length - (tag.EndsWith("/>") ? 2 : 1), " " + attribute);
    }
}