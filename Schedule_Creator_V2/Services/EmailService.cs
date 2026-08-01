using System.Text.RegularExpressions;

namespace Schedule_Creator_V2.Services
{
    internal class EmailService
    {

        private string _emailHtml;

        public EmailService(string emailHtml)
        {
            if (string.IsNullOrWhiteSpace(emailHtml))
            {
                throw new ArgumentException(
                    "Email HTML cannot be empty.",
                    nameof(emailHtml));
            }

            _emailHtml = emailHtml;
        }

        public void SetHeaderVisibility(bool isVisible)
        {
            SetSectionVisibility("header-section", isVisible);
        }

        public void SetBodyVisibility(bool isVisible)
        {
            SetSectionVisibility("body-section", isVisible);
        }

        public void SetRequestVisibility(bool isVisible)
        {
            SetSectionVisibility("request-section", isVisible);
        }

        public void SetAttachmentsVisibility(bool isVisible)
        {
            SetSectionVisibility("attachments-section", isVisible);
        }

        public void SetSignatureVisibility(bool isVisible)
        {
            SetSectionVisibility("signature-section", isVisible);
        }

        public void SetFooterVisibility(bool isVisible)
        {
            SetSectionVisibility("footer-section", isVisible);
        }

        public void SetAllSectionsVisibility(bool isVisible)
        {
            SetHeaderVisibility(isVisible);
            SetBodyVisibility(isVisible);
            SetRequestVisibility(isVisible);
            SetAttachmentsVisibility(isVisible);
            SetSignatureVisibility(isVisible);
            SetFooterVisibility(isVisible);
        }

        public string GetHtml()
        {
            return _emailHtml;
        }

        private void SetSectionVisibility(
            string sectionId,
            bool isVisible)
        {
            string displayValue = isVisible ? "block" : "none";

            string pattern =
                $@"(<[^>]+\bid\s*=\s*[""']{Regex.Escape(sectionId)}[""'][^>]*)(>)";

            Match match = Regex.Match(
                _emailHtml,
                pattern,
                RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                throw new InvalidOperationException(
                    $"Could not find an HTML element with the ID '{sectionId}'.");
            }

            string openingTag = match.Groups[1].Value;

            openingTag = SetDisplayStyle(openingTag, displayValue);

            _emailHtml =
                _emailHtml.Substring(0, match.Index) +
                openingTag +
                match.Groups[2].Value +
                _emailHtml.Substring(match.Index + match.Length);
        }

        private static string SetDisplayStyle(
            string openingTag,
            string displayValue)
        {
            Match styleMatch = Regex.Match(
                openingTag,
                @"\bstyle\s*=\s*([""'])(.*?)\1",
                RegexOptions.IgnoreCase);

            if (styleMatch.Success)
            {
                string existingStyles = styleMatch.Groups[2].Value;

                existingStyles = Regex.Replace(
                    existingStyles,
                    @"display\s*:\s*[^;]+;?",
                    string.Empty,
                    RegexOptions.IgnoreCase);

                existingStyles = existingStyles.Trim();

                if (existingStyles.Length > 0 &&
                    !existingStyles.EndsWith(";"))
                {
                    existingStyles += ";";
                }

                string updatedStyles =
                    $"{existingStyles} display: {displayValue};".Trim();

                string replacement =
                    $"style=\"{updatedStyles}\"";

                return openingTag.Remove(
                        styleMatch.Index,
                        styleMatch.Length)
                    .Insert(
                        styleMatch.Index,
                        replacement);
            }

            return openingTag +
                   $" style=\"display: {displayValue};\"";
        }

    }
}
