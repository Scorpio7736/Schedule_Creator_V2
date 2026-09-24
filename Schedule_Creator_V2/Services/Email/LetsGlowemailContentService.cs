using Schedule_Creator_V2.Models.Constants;
using Schedule_Creator_V2.Models.Defaults;
using Schedule_Creator_V2.Models.Records;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Schedule_Creator_V2.Services.Email
{
    public static class LetsGlowEmailContentService
    {
        public const string TemplateFileName =
            "Lets_Glow_Email_Template.html";

        private const string ImageMarker =
            "<!-- IMAGE -->";

        private const string AnnouncementsMarker =
            "<!-- ANNOUNCEMENTS -->";

        private const string RequestMarker =
            "<!-- REQUEST -->";

        private const string AttachmentsMarker =
            "<!-- ATTACHMENTS -->";

        private const string SignatureMarker =
            "<!-- SIGNATURE -->";

        private const string FooterMarker =
            "<!-- FOOTER -->";

        private const string RichTextPrefix =
            "[[RICH_TEXT_HTML]]";


        // =========================================================
        // TEMPLATE CHECK
        // =========================================================

        public static bool UsesLetsGlowTemplate(
            EmailType emailType)
        {
            ArgumentNullException.ThrowIfNull(
                emailType);

            return string.Equals(
                emailType.templateFileName,
                TemplateFileName,
                StringComparison.OrdinalIgnoreCase);
        }


        // =========================================================
        // BUILD EMAIL
        // =========================================================

        public static string BuildHtmlBody(
            EmailType emailType)
        {
            ArgumentNullException.ThrowIfNull(
                emailType);

            string html =
                LoadTemplate();

            string subject =
                EmailContentService
                    .BuildSubject(
                        emailType);

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

            html =
                ApplyHeader(
                    html,
                    emailType);

            html =
                ApplyBody(
                    html,
                    emailType);

            html =
                ApplyImage(
                    html,
                    emailType);

            html =
                ApplyAnnouncements(
                    html,
                    emailType);

            html =
                ApplyRequest(
                    html,
                    emailType);

            /*
             * These three Let's Glow presets currently do not use
             * the attachments section.
             */
            html =
                RemoveBetweenMarkers(
                    html,
                    AttachmentsMarker,
                    SignatureMarker);

            html =
                ApplySignature(
                    html,
                    emailType);

            html =
                ApplyFooter(
                    html,
                    emailType);

            /*
             * The original HTML template contains placeholder
             * tokens. They should never survive into a generated
             * email.
             */
            html =
                Regex.Replace(
                    html,
                    @"\{\{[A-Z0-9_]+\}\}",
                    string.Empty,
                    RegexOptions.IgnoreCase);

            return html;
        }


        // =========================================================
        // TEMPLATE
        // =========================================================

        private static string LoadTemplate()
        {
            string templatePath =
                Path.Combine(
                    AppContext.BaseDirectory,
                    "Models",
                    "Objects",
                    TemplateFileName);

            if (!File.Exists(
                    templatePath))
            {
                throw new FileNotFoundException(
                    $"The Let's Glow email template " +
                    $"'{TemplateFileName}' could not be found.",
                    templatePath);
            }

            return File.ReadAllText(
                templatePath);
        }


        // =========================================================
        // HEADER
        // =========================================================

        private static string ApplyHeader(
            string html,
            EmailType emailType)
        {
            CustomHeaderInputs? headerInputs =
                emailType.inputs?
                    .OfType<CustomHeaderInputs>()
                    .FirstOrDefault();

            string organizationLabel =
                string.IsNullOrWhiteSpace(
                    headerInputs?.OrganizationName)
                    ? "UREC OUTDOORS"
                    : headerInputs.OrganizationName.Trim();

            string headerLabel =
                string.IsNullOrWhiteSpace(
                    headerInputs?.HeaderLabel)
                    ? "GLOW CLIMB"
                    : headerInputs.HeaderLabel.Trim();

            string emailHeading =
                string.IsNullOrWhiteSpace(
                    headerInputs?.EmailHeading)
                    ? "LET'S GLOW!"
                    : headerInputs.EmailHeading.Trim();

            string headerSubtitle =
                headerInputs?.HeaderSubtitle
                ?? string.Empty;

            string headerImageSource =
                string.IsNullOrWhiteSpace(
                    headerInputs?.HeaderImageUrl)
                    ? EmailImageSources.Default_HeaderImage
                    : headerInputs.HeaderImageUrl;

            headerImageSource =
                ResolveImageSource(
                    headerImageSource);

            html =
                ReplaceElementAttribute(
                    html,
                    "headerImage",
                    "src",
                    headerImageSource);

            html =
                ReplaceElementAttribute(
                    html,
                    "headerImage",
                    "alt",
                    "Let's Glow at the UREC Climbing Tower");

            html =
                ReplaceElementContent(
                    html,
                    "organizationLabel",
                    Encode(
                        organizationLabel));

            html =
                ReplaceElementContent(
                    html,
                    "headerLabel",
                    Encode(
                        headerLabel));

            html =
                ReplaceElementContent(
                    html,
                    "emailHeading",
                    RenderGlowHeading(
                        emailHeading));

            html =
                ReplaceElementContent(
                    html,
                    "headerSubtitle",
                    RenderRichText(
                        headerSubtitle));

            return html;
        }


        private static string RenderGlowHeading(
            string heading)
        {
            if (string.IsNullOrWhiteSpace(
                    heading))
            {
                return string.Empty;
            }

            const string glowText =
                "GLOW!";

            int glowIndex =
                heading.LastIndexOf(
                    glowText,
                    StringComparison.OrdinalIgnoreCase);

            if (glowIndex < 0)
            {
                return Encode(
                    heading);
            }

            string beforeGlow =
                heading[..glowIndex];

            string actualGlowText =
                heading.Substring(
                    glowIndex,
                    glowText.Length);

            string afterGlow =
                heading[
                    (glowIndex + glowText.Length)..];

            return
                Encode(beforeGlow) +
                $$"""
                <span
                    class="text-lime"
                    style="
                        color:#9be447 !important;
                        -webkit-text-fill-color:#9be447 !important;">
                    {{Encode(actualGlowText)}}
                </span>
                """ +
                Encode(afterGlow);
        }


        // =========================================================
        // BODY
        // =========================================================

        private static string ApplyBody(
            string html,
            EmailType emailType)
        {
            CustomBodyInputs? bodyInputs =
                emailType.inputs?
                    .OfType<CustomBodyInputs>()
                    .FirstOrDefault();

            if (bodyInputs is null)
            {
                return html;
            }

            string greeting =
                bodyInputs.RecipientGreeting
                ?? string.Empty;

            string body =
                bodyInputs.EmailBody
                ?? string.Empty;

            html =
                ReplaceElementContent(
                    html,
                    "bodyHeading",
                    EncodeWithLineBreaks(
                        greeting));

            html =
                ReplaceElementContent(
                    html,
                    "bodyContent",
                    RenderRichText(
                        body,
                        useParagraphsForPlainText: true));

            /*
             * The base Let's Glow HTML contains a sample
             * REGISTER / LEARN MORE button.
             *
             * That button is not part of these email presets,
             * so remove it.
             */
            html =
                Regex.Replace(
                    html,
                    @"\s*<!-- Optional CTA -->\s*<table\b.*?</table>",
                    string.Empty,
                    RegexOptions.IgnoreCase |
                    RegexOptions.Singleline);

            return html;
        }


        // =========================================================
        // IMAGE
        // =========================================================

        private static string ApplyImage(
            string html,
            EmailType emailType)
        {
            CustomImageInputs? imageInputs =
                emailType.inputs?
                    .OfType<CustomImageInputs>()
                    .FirstOrDefault();

            /*
             * Announcements and Requests do not contain
             * CustomImageInputs, so remove the image area from
             * those templates completely.
             */
            if (imageInputs is null ||
                string.IsNullOrWhiteSpace(
                    imageInputs.ImageSource))
            {
                return RemoveBetweenMarkers(
                    html,
                    ImageMarker,
                    AnnouncementsMarker);
            }

            string resolvedImageSource =
                ResolveImageSource(
                    imageInputs.ImageSource);

            string altText =
                string.IsNullOrWhiteSpace(
                    imageInputs.ImageAltText)
                    ? "Let's Glow"
                    : imageInputs.ImageAltText.Trim();

            html =
                ReplaceElementAttribute(
                    html,
                    "contentImage",
                    "src",
                    resolvedImageSource);

            html =
                ReplaceElementAttribute(
                    html,
                    "contentImage",
                    "alt",
                    altText);

            return html;
        }


        // =========================================================
        // ANNOUNCEMENTS
        // =========================================================

        private static string ApplyAnnouncements(
            string html,
            EmailType emailType)
        {
            CustomAnnouncementsInputs?
                announcementsInputs =
                    emailType.inputs?
                        .OfType<CustomAnnouncementsInputs>()
                        .FirstOrDefault();

            if (announcementsInputs is null)
            {
                return RemoveBetweenMarkers(
                    html,
                    AnnouncementsMarker,
                    RequestMarker);
            }

            string heading =
                string.IsNullOrWhiteSpace(
                    announcementsInputs
                        .AnnouncementsLabel)
                    ? "Announcements"
                    : announcementsInputs
                        .AnnouncementsLabel
                        .Trim();

            string content =
                BuildAnnouncementsContent(
                    announcementsInputs);

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
                    content);

            return html;
        }


        private static string BuildAnnouncementsContent(
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

            string introHtml =
                string.IsNullOrWhiteSpace(
                    inputs.AnnouncementsIntro)
                    ? string.Empty
                    : $$"""
                    <div
                        style="
                            margin:0 0 16px 0;
                            color:#d9d8eb !important;
                            -webkit-text-fill-color:#d9d8eb !important;">
                        {{RenderRichText(
                            inputs.AnnouncementsIntro)}}
                    </div>
                    """;

            string itemsHtml =
                string.Join(
                    Environment.NewLine,
                    announcements.Select(
                        (announcement, index) =>
                            $$"""
                            <li
                                id="glowAnnouncement{{index + 1}}"
                                style="
                                    margin:0 0 10px 0;
                                    padding:12px 14px;
                                    list-style:none;
                                    border-left:5px solid #f34fc1;
                                    background-color:#11102b !important;
                                    background-image:linear-gradient(
                                        #11102b,
                                        #11102b) !important;
                                    color:#d9d8eb !important;
                                    -webkit-text-fill-color:#d9d8eb !important;">
                                {{RenderRichText(
                                    announcement)}}
                            </li>
                            """));

            string listHtml =
                announcements.Count == 0
                    ? string.Empty
                    : $$"""
                    <ul
                        style="
                            margin:0;
                            padding:0;
                            list-style:none;">
                        {{itemsHtml}}
                    </ul>
                    """;

            return
                introHtml +
                listHtml;
        }


        // =========================================================
        // REQUEST
        // =========================================================

        private static string ApplyRequest(
            string html,
            EmailType emailType)
        {
            CustomRequestInputs? requestInputs =
                emailType.inputs?
                    .OfType<CustomRequestInputs>()
                    .FirstOrDefault();

            if (requestInputs is null)
            {
                return RemoveBetweenMarkers(
                    html,
                    RequestMarker,
                    AttachmentsMarker);
            }

            string heading;

            if (!string.IsNullOrWhiteSpace(
                    requestInputs.RequestTitle))
            {
                heading =
                    requestInputs
                        .RequestTitle
                        .Trim();
            }
            else if (!string.IsNullOrWhiteSpace(
                         requestInputs.RequestLabel))
            {
                heading =
                    requestInputs
                        .RequestLabel
                        .Trim();
            }
            else
            {
                heading =
                    "Quick Request";
            }

            string content =
                BuildRequestContent(
                    requestInputs);

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
                    content);

            return html;
        }


        private static string BuildRequestContent(
            CustomRequestInputs inputs)
        {
            bool hasTitle =
                !string.IsNullOrWhiteSpace(
                    inputs.RequestTitle);

            bool hasLabel =
                !string.IsNullOrWhiteSpace(
                    inputs.RequestLabel);

            string labelHtml =
                hasTitle && hasLabel
                    ? $$"""
                    <div
                        class="eyebrow text-pink"
                        style="
                            margin:0 0 8px 0;
                            color:#f34fc1 !important;
                            -webkit-text-fill-color:#f34fc1 !important;">
                        {{Encode(
                            inputs.RequestLabel.Trim())}}
                    </div>
                    """
                    : string.Empty;

            string bodyHtml =
                string.IsNullOrWhiteSpace(
                    inputs.RequestBody)
                    ? string.Empty
                    : $$"""
                    <div
                        style="
                            margin-top:8px;
                            color:#d9d8eb !important;
                            -webkit-text-fill-color:#d9d8eb !important;">
                        {{RenderRichText(
                            inputs.RequestBody)}}
                    </div>
                    """;

            string buttonHtml =
                BuildRequestButton(
                    inputs.RequestButton,
                    inputs.RequestLink);

            return
                labelHtml +
                bodyHtml +
                buttonHtml;
        }


        private static string BuildRequestButton(
            string buttonText,
            string requestLink)
        {
            if (string.IsNullOrWhiteSpace(
                    buttonText) ||
                string.IsNullOrWhiteSpace(
                    requestLink))
            {
                return string.Empty;
            }

            string normalizedLink =
                NormalizeRequestLink(
                    requestLink);

            return $$"""
            <table
                role="presentation"
                cellpadding="0"
                cellspacing="0"
                border="0"
                style="
                    margin-top:18px;
                    border-collapse:collapse;">
                <tr>
                    <td
                        bgcolor="#9be447"
                        class="bg-lime"
                        style="
                            border-radius:4px;
                            background-color:#9be447 !important;
                            background-image:linear-gradient(
                                #9be447,
                                #9be447) !important;">
                        <a
                            href="{{EncodeAttribute(
                                normalizedLink)}}"
                            target="_blank"
                            class="button-link text-night"
                            style="
                                display:inline-block;
                                padding:12px 20px;
                                font-family:Arial,Helvetica,sans-serif;
                                font-size:14px;
                                line-height:18px;
                                font-weight:800;
                                text-decoration:none;
                                color:#0d0b20 !important;
                                -webkit-text-fill-color:#0d0b20 !important;">
                            {{Encode(
                                buttonText.Trim())}}
                        </a>
                    </td>
                </tr>
            </table>
            """;
        }


        private static string NormalizeRequestLink(
            string requestLink)
        {
            string trimmedLink =
                requestLink?.Trim()
                ?? string.Empty;

            if (string.IsNullOrWhiteSpace(
                    trimmedLink))
            {
                return string.Empty;
            }

            if (!trimmedLink.Contains(
                    "://",
                    StringComparison.Ordinal) &&
                !trimmedLink.StartsWith(
                    "mailto:",
                    StringComparison.OrdinalIgnoreCase))
            {
                trimmedLink =
                    "https://" +
                    trimmedLink;
            }

            if (!Uri.TryCreate(
                    trimmedLink,
                    UriKind.Absolute,
                    out Uri? parsedUri))
            {
                throw new InvalidOperationException(
                    $"The request link is invalid: {requestLink}");
            }

            bool allowedScheme =
                parsedUri.Scheme.Equals(
                    Uri.UriSchemeHttp,
                    StringComparison.OrdinalIgnoreCase) ||

                parsedUri.Scheme.Equals(
                    Uri.UriSchemeHttps,
                    StringComparison.OrdinalIgnoreCase) ||

                parsedUri.Scheme.Equals(
                    "mailto",
                    StringComparison.OrdinalIgnoreCase);

            if (!allowedScheme)
            {
                throw new InvalidOperationException(
                    "The request link must use HTTP, HTTPS, or MAILTO.");
            }

            return parsedUri.AbsoluteUri;
        }


        // =========================================================
        // SIGNATURE
        // =========================================================

        private static string ApplySignature(
            string html,
            EmailType emailType)
        {
            CustomSignatureInputs? signatureInputs =
                emailType.inputs?
                    .OfType<CustomSignatureInputs>()
                    .FirstOrDefault();

            if (signatureInputs is null)
            {
                return RemoveBetweenMarkers(
                    html,
                    SignatureMarker,
                    FooterMarker);
            }

            string signatureHtml =
                BuildSignatureContent(
                    signatureInputs);

            return ReplaceElementContent(
                html,
                "signatureContent",
                signatureHtml);
        }


        private static string BuildSignatureContent(
            CustomSignatureInputs inputs)
        {
            string closingHtml =
                string.IsNullOrWhiteSpace(
                    inputs.SignatureClosing)
                    ? string.Empty
                    : RenderRichText(
                        inputs.SignatureClosing);

            string titleHtml =
                string.IsNullOrWhiteSpace(
                    inputs.SenderTitle)
                    ? string.Empty
                    : Encode(
                        inputs.SenderTitle) +
                      "<br>";

            string organizationHtml =
                string.IsNullOrWhiteSpace(
                    inputs.SenderOrganization)
                    ? string.Empty
                    : Encode(
                        inputs.SenderOrganization) +
                      "<br>";

            string emailHtml =
                string.IsNullOrWhiteSpace(
                    inputs.SenderEmail)
                    ? string.Empty
                    : $$"""
                    <a
                        href="mailto:{{EncodeAttribute(
                            inputs.SenderEmail)}}"
                        class="text-cyan"
                        style="
                            color:#36e9ef !important;
                            -webkit-text-fill-color:#36e9ef !important;
                            text-decoration:underline;">
                        {{Encode(
                            inputs.SenderEmail)}}
                    </a><br>
                    """;

            string phoneHtml =
                string.IsNullOrWhiteSpace(
                    inputs.SenderPhone)
                    ? string.Empty
                    : Encode(
                        inputs.SenderPhone);

            return $$"""
            {{closingHtml}}

            <div style="height:12px;line-height:12px;">
                &nbsp;
            </div>

            <strong
                class="text-white"
                style="
                    color:#ffffff !important;
                    -webkit-text-fill-color:#ffffff !important;">
                {{Encode(
                    inputs.SenderName)}}
            </strong>
            <br>

            {{titleHtml}}
            {{organizationHtml}}
            {{emailHtml}}
            {{phoneHtml}}
            """;
        }


        // =========================================================
        // FOOTER
        // =========================================================

        private static string ApplyFooter(
            string html,
            EmailType emailType)
        {
            CustomFooterInputs footerInputs =
                emailType.inputs?
                    .OfType<CustomFooterInputs>()
                    .FirstOrDefault()
                ?? EmailInputDefaults.DefaultFooterInputs;

            string glowFooterImage =
                GetLetsGlowFooterImageSource();

            string organizationHtml =
                $$"""
                <img
                    id="footerLogo"
                    src="{{EncodeAttribute(
                        glowFooterImage)}}"
                    width="190"
                    alt="UREC Outdoors Let's Glow"
                    style="
                        display:block;
                        width:190px;
                        max-width:100%;
                        height:auto;
                        margin:0 0 14px 0;
                        border:0;
                        outline:none;
                        text-decoration:none;">

                <div
                    style="
                        font-size:14px;
                        line-height:20px;
                        font-weight:800;
                        color:#ffffff !important;
                        -webkit-text-fill-color:#ffffff !important;">
                    {{Encode(
                        footerInputs.FooterOrganization)}}
                </div>
                """;

            html =
                ReplaceElementContent(
                    html,
                    "footerOrganization",
                    organizationHtml);

            html =
                ReplaceElementContent(
                    html,
                    "footerText",
                    EncodeWithLineBreaks(
                        footerInputs.FooterText));

            html =
                ReplaceElementContent(
                    html,
                    "footerWebsiteLink",
                    Encode(
                        footerInputs.FooterWebsiteLink));

            string footerUrl =
                string.IsNullOrWhiteSpace(
                    footerInputs.FooterWebsiteUrl)
                    ? "https://www.uwgb.edu/urec/"
                    : footerInputs.FooterWebsiteUrl.Trim();

            html =
                ReplaceElementAttribute(
                    html,
                    "footerWebsiteLink",
                    "href",
                    footerUrl);

            return html;
        }


        private static string
            GetLetsGlowFooterImageSource()
        {
            string imagePath =
                Path.Combine(
                    AppContext.BaseDirectory,
                    "Images",
                    "EmailImages",
                    "Footer_LetsGlow.png");

            if (!File.Exists(
                    imagePath))
            {
                throw new FileNotFoundException(
                    "The Let's Glow footer image could not be found.",
                    imagePath);
            }

            byte[] imageBytes =
                File.ReadAllBytes(
                    imagePath);

            return
                "data:image/png;base64," +
                Convert.ToBase64String(
                    imageBytes);
        }


        // =========================================================
        // IMAGE HELPERS
        // =========================================================

        private static string ResolveImageSource(
            string imageSource)
        {
            string trimmedSource =
                imageSource?.Trim()
                ?? string.Empty;

            if (string.IsNullOrWhiteSpace(
                    trimmedSource))
            {
                return string.Empty;
            }

            if (trimmedSource.StartsWith(
                    "data:image/",
                    StringComparison.OrdinalIgnoreCase))
            {
                return trimmedSource;
            }

            if (File.Exists(
                    trimmedSource))
            {
                return ConvertImageFileToDataUri(
                    trimmedSource);
            }

            if (Uri.TryCreate(
                    trimmedSource,
                    UriKind.Absolute,
                    out Uri? absoluteUri))
            {
                bool isWebImage =
                    absoluteUri.Scheme.Equals(
                        Uri.UriSchemeHttp,
                        StringComparison.OrdinalIgnoreCase) ||

                    absoluteUri.Scheme.Equals(
                        Uri.UriSchemeHttps,
                        StringComparison.OrdinalIgnoreCase);

                if (!isWebImage)
                {
                    throw new InvalidOperationException(
                        "The image URL must use HTTP or HTTPS.");
                }

                return absoluteUri.AbsoluteUri;
            }

            string sourceWithScheme =
                "https://" +
                trimmedSource;

            if (Uri.TryCreate(
                    sourceWithScheme,
                    UriKind.Absolute,
                    out Uri? webUri))
            {
                return webUri.AbsoluteUri;
            }

            throw new InvalidOperationException(
                "The image source is invalid. Enter a valid local " +
                "image file path, HTTPS image URL, or Base64 image.");
        }


        private static string
            ConvertImageFileToDataUri(
                string imageFilePath)
        {
            if (!File.Exists(
                    imageFilePath))
            {
                throw new FileNotFoundException(
                    "The selected image file could not be found.",
                    imageFilePath);
            }

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
            string extension =
                Path.GetExtension(
                        imageFilePath)
                    .ToLowerInvariant();

            return extension switch
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
                        "Unsupported image format. " +
                        "Use JPG, JPEG, PNG, GIF, or BMP.")
            };
        }


        // =========================================================
        // TEMPLATE MANIPULATION
        // =========================================================

        private static string RemoveBetweenMarkers(
            string html,
            string startMarker,
            string endMarker)
        {
            int startIndex =
                html.IndexOf(
                    startMarker,
                    StringComparison.Ordinal);

            if (startIndex < 0)
            {
                throw new InvalidOperationException(
                    $"The Let's Glow template is missing " +
                    $"the marker: {startMarker}");
            }

            int contentStart =
                startIndex +
                startMarker.Length;

            int endIndex =
                html.IndexOf(
                    endMarker,
                    contentStart,
                    StringComparison.Ordinal);

            if (endIndex < 0)
            {
                throw new InvalidOperationException(
                    $"The Let's Glow template is missing " +
                    $"the marker: {endMarker}");
            }

            return
                html[..contentStart] +
                Environment.NewLine +
                html[endIndex..];
        }


        private static string ReplaceElementContent(
            string html,
            string elementId,
            string replacementHtml)
        {
            string escapedElementId =
                Regex.Escape(
                    elementId);

            string pattern =
                $@"(?<opening>
                        <
                        (?<tag>[a-zA-Z][a-zA-Z0-9]*)
                        \b
                        [^>]*
                        \bid\s*=\s*[""']{escapedElementId}[""']
                        [^>]*
                        >
                    )
                    (?<content>.*?)
                    (?<closing>
                        </\k<tag>\s*>
                    )";

            Match match =
                Regex.Match(
                    html,
                    pattern,
                    RegexOptions.IgnoreCase |
                    RegexOptions.Singleline |
                    RegexOptions.IgnorePatternWhitespace);

            if (!match.Success)
            {
                throw new InvalidOperationException(
                    $"The Let's Glow template does not contain " +
                    $"an element with the ID '{elementId}'.");
            }

            return Regex.Replace(
                html,
                pattern,
                currentMatch =>
                    currentMatch.Groups["opening"].Value +
                    replacementHtml +
                    currentMatch.Groups["closing"].Value,
                RegexOptions.IgnoreCase |
                RegexOptions.Singleline |
                RegexOptions.IgnorePatternWhitespace);
        }


        private static string ReplaceElementAttribute(
            string html,
            string elementId,
            string attributeName,
            string attributeValue)
        {
            string escapedElementId =
                Regex.Escape(
                    elementId);

            string escapedAttributeName =
                Regex.Escape(
                    attributeName);

            string elementPattern =
                $@"<
                    (?<tag>[a-zA-Z][a-zA-Z0-9]*)
                    \b
                    [^>]*
                    \bid\s*=\s*[""']{escapedElementId}[""']
                    [^>]*
                    >";

            Match match =
                Regex.Match(
                    html,
                    elementPattern,
                    RegexOptions.IgnoreCase |
                    RegexOptions.Singleline |
                    RegexOptions.IgnorePatternWhitespace);

            if (!match.Success)
            {
                throw new InvalidOperationException(
                    $"The Let's Glow template does not contain " +
                    $"an element with the ID '{elementId}'.");
            }

            string encodedValue =
                EncodeAttribute(
                    attributeValue);

            return Regex.Replace(
                html,
                elementPattern,
                currentMatch =>
                {
                    string openingTag =
                        currentMatch.Value;

                    string attributePattern =
                        $@"\b{escapedAttributeName}\s*=\s*([""']).*?\1";

                    if (Regex.IsMatch(
                            openingTag,
                            attributePattern,
                            RegexOptions.IgnoreCase |
                            RegexOptions.Singleline))
                    {
                        return Regex.Replace(
                            openingTag,
                            attributePattern,
                            $"{attributeName}=\"{encodedValue}\"",
                            RegexOptions.IgnoreCase |
                            RegexOptions.Singleline);
                    }

                    int insertIndex =
                        openingTag.LastIndexOf(
                            '>',
                            openingTag.Length - 1);

                    if (insertIndex < 0)
                    {
                        return openingTag;
                    }

                    return
                        openingTag.Insert(
                            insertIndex,
                            $" {attributeName}=\"{encodedValue}\"");
                },
                RegexOptions.IgnoreCase |
                RegexOptions.Singleline |
                RegexOptions.IgnorePatternWhitespace);
        }


        // =========================================================
        // RICH TEXT
        // =========================================================

        private static string RenderRichText(
            string value,
            bool useParagraphsForPlainText = false)
        {
            if (string.IsNullOrWhiteSpace(
                    value))
            {
                return string.Empty;
            }

            string content =
                value.Trim();

            if (content.StartsWith(
                    RichTextPrefix,
                    StringComparison.Ordinal))
            {
                content =
                    content.Substring(
                        RichTextPrefix.Length);

                return SanitizeRichTextHtml(
                    content);
            }

            if (LooksLikeRichTextHtml(
                    content))
            {
                return SanitizeRichTextHtml(
                    content);
            }

            return useParagraphsForPlainText
                ? EncodeWithParagraphs(
                    content)
                : EncodeWithLineBreaks(
                    content);
        }


        private static bool LooksLikeRichTextHtml(
            string value)
        {
            if (string.IsNullOrWhiteSpace(
                    value))
            {
                return false;
            }

            return Regex.IsMatch(
                value,
                @"<\s*/?\s*(p|strong|b|em|i|u|ul|ol|li|br)\b",
                RegexOptions.IgnoreCase);
        }


        private static string
            SanitizeRichTextHtml(
                string html)
        {
            if (string.IsNullOrWhiteSpace(
                    html))
            {
                return string.Empty;
            }

            try
            {
                XElement root =
                    XElement.Parse(
                        $"<root>{html}</root>",
                        LoadOptions.PreserveWhitespace);

                return string.Concat(
                    root.Nodes()
                        .Select(
                            SanitizeRichTextNode));
            }
            catch
            {
                return EncodeWithParagraphs(
                    html);
            }
        }


        private static string
            SanitizeRichTextNode(
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

            if (tag == "br")
            {
                return "<br>";
            }

            string content =
                string.Concat(
                    element.Nodes()
                        .Select(
                            SanitizeRichTextNode));

            return tag switch
            {
                "p" =>
                    $$"""
                    <p
                        style="
                            margin:0 0 16px 0;
                            color:inherit;">
                        {{content}}
                    </p>
                    """,

                "strong" or "b" =>
                    $"<strong>{content}</strong>",

                "em" or "i" =>
                    $"<em>{content}</em>",

                "u" =>
                    $"<u>{content}</u>",

                "ul" =>
                    $$"""
                    <ul
                        style="
                            margin:8px 0 16px 22px;
                            padding:0;">
                        {{content}}
                    </ul>
                    """,

                "ol" =>
                    $$"""
                    <ol
                        style="
                            margin:8px 0 16px 22px;
                            padding:0;">
                        {{content}}
                    </ol>
                    """,

                "li" =>
                    $$"""
                    <li
                        style="
                            margin:0 0 7px 0;
                            padding:0;">
                        {{content}}
                    </li>
                    """,

                _ =>
                    content
            };
        }


        // =========================================================
        // ENCODING
        // =========================================================

        private static string Encode(
            string value)
        {
            return WebUtility.HtmlEncode(
                value ??
                string.Empty);
        }


        private static string EncodeAttribute(
            string value)
        {
            return WebUtility.HtmlEncode(
                value?.Trim()
                ?? string.Empty);
        }


        private static string EncodeWithLineBreaks(
            string value)
        {
            return Encode(
                    value)
                .Replace(
                    "\r\n",
                    "<br>",
                    StringComparison.Ordinal)
                .Replace(
                    "\n",
                    "<br>",
                    StringComparison.Ordinal);
        }


        private static string EncodeWithParagraphs(
            string value)
        {
            if (string.IsNullOrWhiteSpace(
                    value))
            {
                return string.Empty;
            }

            string normalizedValue =
                value
                    .Replace(
                        "\r\n",
                        "\n",
                        StringComparison.Ordinal)
                    .Trim();

            string[] paragraphs =
                normalizedValue.Split(
                    "\n\n",
                    StringSplitOptions.RemoveEmptyEntries);

            return string.Join(
                Environment.NewLine,
                paragraphs.Select(
                    paragraph =>
                        $$"""
                        <p
                            style="
                                margin:0 0 16px 0;
                                color:inherit;">
                            {{EncodeWithLineBreaks(
                                paragraph.Trim())}}
                        </p>
                        """));
        }
    }
}