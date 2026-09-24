using Schedule_Creator_V2.Models.Records;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Schedule_Creator_V2.Services.Email
{
    public static class ThemedEmailContentService
    {
        private const string RichTextPrefix =
            "[[RICH_TEXT_HTML]]";


        // =========================================================
        // BUILD HTML
        // =========================================================

        public static string BuildHtmlBody(
            EmailType emailType)
        {
            ArgumentNullException.ThrowIfNull(
                emailType);

            string html =
                LoadTemplate(
                    emailType);

            string subject =
                EmailContentService
                    .BuildSubject(
                        emailType);


            // =====================================================
            // METADATA
            // =====================================================

            html =
                ReplaceElementContent(
                    html,
                    "emailTitle",
                    Encode(subject));

            html =
                ReplaceElementContent(
                    html,
                    "preheaderText",
                    Encode(subject));


            // =====================================================
            // HEADER
            // =====================================================

            CustomHeaderInputs? headerInputs =
                emailType.inputs
                    .OfType<CustomHeaderInputs>()
                    .FirstOrDefault();

            if (headerInputs is not null)
            {
                html =
                    ReplaceElementContent(
                        html,
                        "organizationLabel",
                        Encode(
                            headerInputs
                                .OrganizationName));

                html =
                    ReplaceElementContent(
                        html,
                        "headerLabel",
                        Encode(
                            headerInputs
                                .HeaderLabel));

                html =
                    ReplaceElementContent(
                        html,
                        "emailHeading",
                        Encode(
                            headerInputs
                                .EmailHeading));

                html =
                    ReplaceElementContent(
                        html,
                        "headerSubtitle",
                        RenderRichText(
                            headerInputs
                                .HeaderSubtitle));

                if (!string.IsNullOrWhiteSpace(
                        headerInputs.HeaderImageUrl))
                {
                    string headerImageSource =
                        ResolveImageSource(
                            headerInputs
                                .HeaderImageUrl);

                    html =
                        html.Replace(
                            "{{HEADER_IMAGE_SOURCE}}",
                            EncodeAttribute(
                                headerImageSource),
                            StringComparison.Ordinal);

                    html =
                        ReplaceAttributeById(
                            html,
                            "headerImage",
                            "src",
                            headerImageSource);
                }
            }


            // =====================================================
            // BODY
            // =====================================================

            CustomBodyInputs? bodyInputs =
                emailType.inputs
                    .OfType<CustomBodyInputs>()
                    .FirstOrDefault();

            if (bodyInputs is not null)
            {
                html =
                    ReplaceElementContent(
                        html,
                        "bodyHeading",
                        RenderPlainText(
                            bodyInputs
                                .RecipientGreeting));

                html =
                    ReplaceElementContent(
                        html,
                        "bodyContent",
                        RenderRichText(
                            bodyInputs
                                .EmailBody));
            }


            // =====================================================
            // IMAGE
            // =====================================================

            CustomImageInputs? imageInputs =
                emailType.inputs
                    .OfType<CustomImageInputs>()
                    .FirstOrDefault();

            bool hasImage =
                imageInputs is not null &&
                !string.IsNullOrWhiteSpace(
                    imageInputs.ImageSource);

            if (hasImage)
            {
                string imageSource =
                    ResolveImageSource(
                        imageInputs!.ImageSource);

                html =
                    html.Replace(
                        "{{CONTENT_IMAGE_SOURCE}}",
                        EncodeAttribute(
                            imageSource),
                        StringComparison.Ordinal);

                html =
                    ReplaceAttributeById(
                        html,
                        "contentImage",
                        "src",
                        imageSource);

                if (!string.IsNullOrWhiteSpace(
                        imageInputs.ImageAltText))
                {
                    html =
                        ReplaceAttributeById(
                            html,
                            "contentImage",
                            "alt",
                            imageInputs
                                .ImageAltText);
                }
            }
            else
            {
                html =
                    RemoveElementById(
                        html,
                        "imageSection");

                html =
                    RemoveElementById(
                        html,
                        "imageDividerSection");
            }


            // =====================================================
            // ANNOUNCEMENTS
            // =====================================================

            CustomAnnouncementsInputs?
                announcementsInputs =
                    emailType.inputs
                        .OfType<
                            CustomAnnouncementsInputs>()
                        .FirstOrDefault();

            if (announcementsInputs is not null)
            {
                string heading =
                    string.IsNullOrWhiteSpace(
                        announcementsInputs
                            .AnnouncementsLabel)
                        ? "Announcements"
                        : announcementsInputs
                            .AnnouncementsLabel;

                html =
                    ReplaceElementContent(
                        html,
                        "announcementsHeading",
                        Encode(
                            heading));

                html =
                    ReplaceElementContent(
                        html,
                        "announcementsContent",
                        BuildAnnouncementsContent(
                            announcementsInputs));
            }
            else
            {
                html =
                    RemoveElementById(
                        html,
                        "announcementsSection");
            }


            // =====================================================
            // REQUEST
            // =====================================================

            CustomRequestInputs? requestInputs =
                emailType.inputs
                    .OfType<CustomRequestInputs>()
                    .FirstOrDefault();

            if (requestInputs is not null)
            {
                string heading =
                    !string.IsNullOrWhiteSpace(
                        requestInputs.RequestTitle)
                        ? requestInputs.RequestTitle
                        : requestInputs.RequestLabel;

                html =
                    ReplaceElementContent(
                        html,
                        "requestHeading",
                        Encode(
                            heading));

                html =
                    ReplaceElementContent(
                        html,
                        "requestContent",
                        RenderRichText(
                            requestInputs
                                .RequestBody));

                bool hasButton =
                    !string.IsNullOrWhiteSpace(
                        requestInputs.RequestButton) &&
                    !string.IsNullOrWhiteSpace(
                        requestInputs.RequestLink);

                if (hasButton)
                {
                    html =
                        ReplaceElementContent(
                            html,
                            "requestButton",
                            Encode(
                                requestInputs
                                    .RequestButton));

                    html =
                        ReplaceAttributeById(
                            html,
                            "requestButton",
                            "href",
                            requestInputs
                                .RequestLink);
                }
                else
                {
                    html =
                        RemoveElementById(
                            html,
                            "requestButtonSection");
                }
            }
            else
            {
                html =
                    RemoveElementById(
                        html,
                        "requestSection");
            }


            // =====================================================
            // ATTACHMENTS
            // =====================================================

            CustomAttachmentsInputs?
                attachmentsInputs =
                    emailType.inputs
                        .OfType<
                            CustomAttachmentsInputs>()
                        .FirstOrDefault();

            if (attachmentsInputs is not null)
            {
                string heading =
                    string.IsNullOrWhiteSpace(
                        attachmentsInputs
                            .AttachmentsLabel)
                        ? "Attachments"
                        : attachmentsInputs
                            .AttachmentsLabel;

                html =
                    ReplaceElementContent(
                        html,
                        "attachmentsHeading",
                        Encode(
                            heading));

                html =
                    ReplaceElementContent(
                        html,
                        "attachmentsContent",
                        BuildAttachmentsContent(
                            attachmentsInputs));
            }
            else
            {
                html =
                    RemoveElementById(
                        html,
                        "attachmentsSection");
            }


            // =====================================================
            // SIGNATURE
            // =====================================================

            CustomSignatureInputs? signatureInputs =
                emailType.inputs
                    .OfType<CustomSignatureInputs>()
                    .FirstOrDefault();

            if (signatureInputs is not null)
            {
                html =
                    ReplaceElementContent(
                        html,
                        "signatureContent",
                        BuildSignatureContent(
                            signatureInputs));
            }
            else
            {
                html =
                    RemoveElementById(
                        html,
                        "signatureSection");
            }


            // =====================================================
            // FOOTER
            // =====================================================

            CustomFooterInputs? footerInputs =
                emailType.inputs
                    .OfType<CustomFooterInputs>()
                    .FirstOrDefault();

            if (footerInputs is not null)
            {
                html =
                    ReplaceElementContent(
                        html,
                        "footerOrganization",
                        Encode(
                            footerInputs
                                .FooterOrganization));

                html =
                    ReplaceElementContent(
                        html,
                        "footerText",
                        RenderPlainText(
                            footerInputs
                                .FooterText));

                html =
                    ReplaceElementContent(
                        html,
                        "footerWebsiteLink",
                        Encode(
                            footerInputs
                                .FooterWebsiteLink));

                html =
                    ReplaceAttributeById(
                        html,
                        "footerWebsiteLink",
                        "href",
                        footerInputs
                            .FooterWebsiteUrl);
            }


            // =====================================================
            // CLEAN PLACEHOLDERS
            // =====================================================

            html =
                html.Replace(
                    "{{HEADER_IMAGE_SOURCE}}",
                    string.Empty,
                    StringComparison.Ordinal);

            html =
                html.Replace(
                    "{{CONTENT_IMAGE_SOURCE}}",
                    string.Empty,
                    StringComparison.Ordinal);

            html =
                html.Replace(
                    "{{FOOTER_LOGO_SOURCE}}",
                    string.Empty,
                    StringComparison.Ordinal);


            return html;
        }


        // =========================================================
        // TEMPLATE
        // =========================================================

        private static string LoadTemplate(
            EmailType emailType)
        {
            string templateFileName =
                string.IsNullOrWhiteSpace(
                    emailType.templateFileName)
                    ? "Email.html"
                    : emailType.templateFileName;

            string templatePath =
                Path.Combine(
                    AppContext.BaseDirectory,
                    "Models",
                    "Objects",
                    templateFileName);

            if (!File.Exists(
                    templatePath))
            {
                throw new FileNotFoundException(
                    $"The HTML email template " +
                    $"'{templateFileName}' could not be found.",
                    templatePath);
            }

            return File.ReadAllText(
                templatePath);
        }


        // =========================================================
        // REMOVE EXACT ELEMENT
        // =========================================================

        private static string RemoveElementById(
            string html,
            string elementId)
        {
            string escapedId =
                Regex.Escape(
                    elementId);

            string pattern =
                $"""
                <
                (?<tag>[a-zA-Z0-9]+)
                \b
                (?=[^>]*\bid\s*=\s*["']{escapedId}["'])
                [^>]*
                >
                .*?
                </
                \k<tag>
                \s*
                >
                """;

            return Regex.Replace(
                html,
                pattern,
                string.Empty,
                RegexOptions.IgnoreCase |
                RegexOptions.Singleline |
                RegexOptions.IgnorePatternWhitespace);
        }


        // =========================================================
        // ANNOUNCEMENTS
        // =========================================================

        private static string
            BuildAnnouncementsContent(
                CustomAnnouncementsInputs inputs)
        {
            List<string> announcements =
                inputs.AnnouncementsList?
                    .Where(item =>
                        !string.IsNullOrWhiteSpace(
                            item))
                    .Select(item =>
                        item.Trim())
                    .ToList()
                ?? new List<string>();

            string intro =
                string.IsNullOrWhiteSpace(
                    inputs.AnnouncementsIntro)
                    ? string.Empty
                    : RenderRichText(
                        inputs.AnnouncementsIntro);

            if (announcements.Count == 0)
            {
                return intro;
            }

            string items =
                string.Join(
                    Environment.NewLine,
                    announcements.Select(
                        announcement =>
                            $"""
                            <li style="
                                margin:0 0 10px 0;
                                padding:0;">
                                {Encode(announcement)}
                            </li>
                            """));

            return
                intro +
                $"""
                <ul style="
                    margin:14px 0 0 0;
                    padding:0 0 0 22px;">
                    {items}
                </ul>
                """;
        }


        // =========================================================
        // ATTACHMENTS
        // =========================================================

        private static string
            BuildAttachmentsContent(
                CustomAttachmentsInputs inputs)
        {
            List<string> attachments =
                inputs.AttachmentsList?
                    .Where(item =>
                        !string.IsNullOrWhiteSpace(
                            item))
                    .Select(item =>
                        item.Trim())
                    .ToList()
                ?? new List<string>();

            string intro =
                string.IsNullOrWhiteSpace(
                    inputs.AttachmentsIntro)
                    ? string.Empty
                    : RenderRichText(
                        inputs.AttachmentsIntro);

            if (attachments.Count == 0)
            {
                return intro;
            }

            string items =
                string.Join(
                    Environment.NewLine,
                    attachments.Select(
                        attachment =>
                            $"""
                            <div style="
                                margin:0 0 8px 0;
                                font-weight:bold;">
                                &#128206;&nbsp;
                                {Encode(
                                    GetAttachmentDisplayName(
                                        attachment))}
                            </div>
                            """));

            return
                intro +
                $"""
                <div style="margin-top:14px;">
                    {items}
                </div>
                """;
        }


        private static string
            GetAttachmentDisplayName(
                string attachment)
        {
            if (string.IsNullOrWhiteSpace(
                    attachment))
            {
                return string.Empty;
            }

            try
            {
                string fileName =
                    Path.GetFileName(
                        attachment.Trim());

                return string.IsNullOrWhiteSpace(
                    fileName)
                    ? attachment.Trim()
                    : fileName;
            }
            catch
            {
                return attachment.Trim();
            }
        }


        // =========================================================
        // SIGNATURE
        // =========================================================

        private static string
            BuildSignatureContent(
                CustomSignatureInputs inputs)
        {
            List<string> lines =
                new List<string>();

            if (!string.IsNullOrWhiteSpace(
                    inputs.SenderName))
            {
                lines.Add(
                    $"<strong>{Encode(inputs.SenderName)}</strong>");
            }

            if (!string.IsNullOrWhiteSpace(
                    inputs.SenderTitle))
            {
                lines.Add(
                    Encode(
                        inputs.SenderTitle));
            }

            if (!string.IsNullOrWhiteSpace(
                    inputs.SenderOrganization))
            {
                lines.Add(
                    Encode(
                        inputs.SenderOrganization));
            }

            if (!string.IsNullOrWhiteSpace(
                    inputs.SenderEmail))
            {
                lines.Add(
                    $"<a href=\"mailto:{EncodeAttribute(inputs.SenderEmail)}\" " +
                    $"style=\"color:inherit;text-decoration:none;\">" +
                    $"{Encode(inputs.SenderEmail)}</a>");
            }

            if (!string.IsNullOrWhiteSpace(
                    inputs.SenderPhone))
            {
                lines.Add(
                    Encode(
                        inputs.SenderPhone));
            }

            string closing =
                string.IsNullOrWhiteSpace(
                    inputs.SignatureClosing)
                    ? string.Empty
                    : RenderRichText(
                        inputs.SignatureClosing);

            string sender =
                lines.Count == 0
                    ? string.Empty
                    : $"""
                      <div style="margin-top:12px;">
                          {string.Join("<br>", lines)}
                      </div>
                      """;

            return
                closing +
                sender;
        }


        // =========================================================
        // REPLACE ELEMENT CONTENT
        // =========================================================

        private static string ReplaceElementContent(
            string html,
            string elementId,
            string replacementHtml)
        {
            string escapedId =
                Regex.Escape(
                    elementId);

            string pattern =
                $"""
                (?<open>
                    <
                    (?<tag>[a-zA-Z0-9]+)
                    \b
                    (?=[^>]*\bid\s*=\s*["']{escapedId}["'])
                    [^>]*
                    >
                )
                .*?
                (?<close>
                    </
                    \k<tag>
                    \s*
                    >
                )
                """;

            return Regex.Replace(
                html,
                pattern,
                match =>
                    match.Groups["open"].Value +
                    replacementHtml +
                    match.Groups["close"].Value,
                RegexOptions.IgnoreCase |
                RegexOptions.Singleline |
                RegexOptions.IgnorePatternWhitespace);
        }


        // =========================================================
        // REPLACE ATTRIBUTE
        // =========================================================

        private static string ReplaceAttributeById(
            string html,
            string elementId,
            string attributeName,
            string attributeValue)
        {
            string escapedId =
                Regex.Escape(
                    elementId);

            string pattern =
                $"""
                <
                (?<tag>[a-zA-Z0-9]+)
                \b
                (?=[^>]*\bid\s*=\s*["']{escapedId}["'])
                [^>]*
                >
                """;

            return Regex.Replace(
                html,
                pattern,
                match =>
                {
                    string tag =
                        match.Value;

                    string attributePattern =
                        $"""
                        \b{Regex.Escape(attributeName)}
                        \s*=\s*
                        ["']
                        [^"']*
                        ["']
                        """;

                    string replacement =
                        $"{attributeName}=\"" +
                        $"{EncodeAttribute(attributeValue)}\"";

                    if (Regex.IsMatch(
                            tag,
                            attributePattern,
                            RegexOptions.IgnoreCase |
                            RegexOptions.IgnorePatternWhitespace))
                    {
                        return Regex.Replace(
                            tag,
                            attributePattern,
                            replacement,
                            RegexOptions.IgnoreCase |
                            RegexOptions.IgnorePatternWhitespace);
                    }

                    int index =
                        tag.LastIndexOf(
                            '>');

                    if (index < 0)
                    {
                        return tag;
                    }

                    return tag.Insert(
                        index,
                        " " +
                        replacement);
                },
                RegexOptions.IgnoreCase |
                RegexOptions.Singleline |
                RegexOptions.IgnorePatternWhitespace);
        }


        // =========================================================
        // IMAGE SOURCE
        // =========================================================

        private static string ResolveImageSource(
            string imageSource)
        {
            string trimmed =
                imageSource?.Trim()
                ?? string.Empty;

            if (string.IsNullOrWhiteSpace(
                    trimmed))
            {
                return string.Empty;
            }

            if (trimmed.StartsWith(
                    "data:image/",
                    StringComparison.OrdinalIgnoreCase))
            {
                return trimmed;
            }

            if (File.Exists(
                    trimmed))
            {
                return ConvertImageFileToDataUri(
                    trimmed);
            }

            if (Uri.TryCreate(
                    trimmed,
                    UriKind.Absolute,
                    out Uri? uri))
            {
                bool isWeb =
                    uri.Scheme.Equals(
                        Uri.UriSchemeHttp,
                        StringComparison.OrdinalIgnoreCase) ||
                    uri.Scheme.Equals(
                        Uri.UriSchemeHttps,
                        StringComparison.OrdinalIgnoreCase);

                if (!isWeb)
                {
                    throw new InvalidOperationException(
                        "The image URL must use HTTP or HTTPS.");
                }

                return uri.AbsoluteUri;
            }

            string httpsSource =
                "https://" +
                trimmed;

            if (Uri.TryCreate(
                    httpsSource,
                    UriKind.Absolute,
                    out Uri? httpsUri))
            {
                return httpsUri.AbsoluteUri;
            }

            throw new InvalidOperationException(
                "The image source is invalid.");
        }


        private static string
            ConvertImageFileToDataUri(
                string imageFilePath)
        {
            string mimeType =
                GetImageMimeType(
                    imageFilePath);

            byte[] imageBytes =
                File.ReadAllBytes(
                    imageFilePath);

            return
                $"data:{mimeType};base64," +
                Convert.ToBase64String(
                    imageBytes);
        }


        private static string GetImageMimeType(
            string imageFilePath)
        {
            return Path.GetExtension(
                    imageFilePath)
                .ToLowerInvariant() switch
            {
                ".jpg" =>
                    "image/jpeg",

                ".jpeg" =>
                    "image/jpeg",

                ".png" =>
                    "image/png",

                ".gif" =>
                    "image/gif",

                ".bmp" =>
                    "image/bmp",

                _ =>
                    throw new InvalidOperationException(
                        "Unsupported image format.")
            };
        }


        // =========================================================
        // TEXT
        // =========================================================

        private static string RenderPlainText(
            string? value)
        {
            if (string.IsNullOrWhiteSpace(
                    value))
            {
                return string.Empty;
            }

            return Encode(
                    value.Trim())
                .Replace(
                    "\r\n",
                    "<br>")
                .Replace(
                    "\n",
                    "<br>")
                .Replace(
                    "\r",
                    "<br>");
        }


        private static string RenderRichText(
            string? value)
        {
            if (string.IsNullOrWhiteSpace(
                    value))
            {
                return string.Empty;
            }

            string trimmed =
                value.Trim();

            if (trimmed.StartsWith(
                    RichTextPrefix,
                    StringComparison.Ordinal))
            {
                return SanitizeRichHtml(
                    trimmed[
                        RichTextPrefix.Length..]);
            }

            bool containsHtml =
                Regex.IsMatch(
                    trimmed,
                    @"<\s*(p|strong|b|em|i|u|ul|ol|li|br)\b",
                    RegexOptions.IgnoreCase);

            if (containsHtml)
            {
                return SanitizeRichHtml(
                    trimmed);
            }

            return RenderPlainText(
                trimmed);
        }


        private static string SanitizeRichHtml(
            string html)
        {
            try
            {
                string xmlHtml =
                    html.Replace(
                        "&nbsp;",
                        "&#160;",
                        StringComparison.OrdinalIgnoreCase);

                XElement root =
                    XElement.Parse(
                        "<root>" +
                        xmlHtml +
                        "</root>",
                        LoadOptions.PreserveWhitespace);

                return string.Concat(
                    root.Nodes()
                        .Select(
                            SanitizeNode));
            }
            catch
            {
                return RenderPlainText(
                    html);
            }
        }


        private static string SanitizeNode(
            XNode node)
        {
            if (node is XText text)
            {
                return Encode(
                    text.Value);
            }

            if (node is not XElement element)
            {
                return string.Empty;
            }

            string tag =
                element.Name
                    .LocalName
                    .ToLowerInvariant();

            string children =
                string.Concat(
                    element.Nodes()
                        .Select(
                            SanitizeNode));

            return tag switch
            {
                "p" =>
                    $"<p>{children}</p>",

                "strong" =>
                    $"<strong>{children}</strong>",

                "b" =>
                    $"<strong>{children}</strong>",

                "em" =>
                    $"<em>{children}</em>",

                "i" =>
                    $"<em>{children}</em>",

                "u" =>
                    $"<u>{children}</u>",

                "ul" =>
                    $"<ul>{children}</ul>",

                "ol" =>
                    $"<ol>{children}</ol>",

                "li" =>
                    $"<li>{children}</li>",

                "br" =>
                    "<br />",

                _ =>
                    children
            };
        }


        // =========================================================
        // ENCODING
        // =========================================================

        private static string Encode(
            string? value)
        {
            return WebUtility.HtmlEncode(
                value ?? string.Empty);
        }


        private static string EncodeAttribute(
            string? value)
        {
            return WebUtility.HtmlEncode(
                value ?? string.Empty);
        }
    }
}