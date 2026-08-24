using Schedule_Creator_V2.Models.Constants;
using Schedule_Creator_V2.Models.Defaults;
using Schedule_Creator_V2.Models.Records;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Schedule_Creator_V2.Services
{
    public static class EmailContentService
    {
        private const string HeaderMarker =
            "<!-- HEADER -->";

        private const string BodyMarker =
            "<!-- BODY -->";

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


        public static string BuildSubject(
    EmailType emailType)
        {
            ArgumentNullException.ThrowIfNull(emailType);

            EmailDetailsInputs? emailDetails =
                emailType.inputs?
                    .OfType<EmailDetailsInputs>()
                    .FirstOrDefault();

            return emailDetails?.Subject?.Trim()
                ?? string.Empty;
        }

        public static string BuildHtmlBody(
    EmailType emailType)
        {
            ArgumentNullException.ThrowIfNull(
                emailType);

            string html =
                LoadEmailTemplate();

            string subject =
                BuildSubject(emailType);

            html = ReplaceElementContent(
                html,
                "emailTitle",
                Encode(subject));

            html = ReplaceElementContent(
                html,
                "preheaderText",
                Encode(subject));

            html = ReplaceRequiredMarker(
                html,
                HeaderMarker,
                BuildHeaderSection(emailType));

            html = ReplaceRequiredMarker(
                html,
                BodyMarker,
                BuildBodySection(emailType));

            html = ReplaceRequiredMarker(
                html,
                ImageMarker,
                BuildImageSection(emailType));

            html = ReplaceRequiredMarker(
                html,
                AnnouncementsMarker,
                BuildAnnouncementsSection(emailType));

            html = ReplaceRequiredMarker(
                html,
                RequestMarker,
                BuildRequestSection(emailType));

            html = ReplaceRequiredMarker(
                    html,
                    AttachmentsMarker,
                    BuildAttachmentsSection(emailType));

            html = ReplaceRequiredMarker(
                html,
                SignatureMarker,
                BuildSignatureSection(emailType));

            html = ReplaceRequiredMarker(
                html,
                FooterMarker,
                BuildFooterSection(emailType));

            return html;
        }

        private static string LoadEmailTemplate()
        {
            string templatePath =
                Path.Combine(
                    AppContext.BaseDirectory,
                    "Models",
                    "Objects",
                    "Email.html");

            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException(
                    "The HTML email template could not be found.",
                    templatePath);
            }

            return File.ReadAllText(
                templatePath);
        }

        private static string BuildImageSection(
    EmailType emailType)
        {
            CustomImageInputs? imageInputs =
                emailType.inputs?
                    .OfType<CustomImageInputs>()
                    .FirstOrDefault();

            if (imageInputs is null ||
                string.IsNullOrWhiteSpace(
                    imageInputs.ImageSource))
            {
                return string.Empty;
            }

            string resolvedImageSource =
                ResolveImageSource(
                    imageInputs.ImageSource);

            string imageAltText =
                string.IsNullOrWhiteSpace(
                    imageInputs.ImageAltText)
                    ? "Email image"
                    : imageInputs.ImageAltText.Trim();

            return $$"""
        <tr id="imageSection">
            <td
                align="center"
                bgcolor="#ffffff"
                class="content-padding background-white"
                style="
                    padding-top:10px;
                    padding-bottom:34px;
                    background-color:#ffffff !important;
                    background-image:linear-gradient(
                        #ffffff,
                        #ffffff) !important;">

                <img
                    id="contentImage"
                    src="{{EncodeAttribute(resolvedImageSource)}}"
                    width="564"
                    alt="{{EncodeAttribute(imageAltText)}}"
                    style="
                        display:block;
                        width:100%;
                        max-width:564px;
                        height:auto;
                        margin:0 auto;
                        border:0;
                        outline:none;
                        text-decoration:none;
                        border-radius:6px;">
            </td>
        </tr>
        """;
        }

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

            /*
             * Already-converted Base64 image.
             */
            if (trimmedSource.StartsWith(
                    "data:image/",
                    StringComparison.OrdinalIgnoreCase))
            {
                return trimmedSource;
            }

            /*
             * Local image file.
             */
            if (File.Exists(trimmedSource))
            {
                return ConvertImageFileToDataUri(
                    trimmedSource);
            }

            /*
             * Fully qualified web URL.
             */
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

            /*
             * Allow www.example.com/image.jpg without requiring
             * the user to enter https://.
             */
            string sourceWithScheme =
                "https://" + trimmedSource;

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

        private static string ConvertImageFileToDataUri(
            string imageFilePath)
        {
            if (!File.Exists(imageFilePath))
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

            string base64Image =
                Convert.ToBase64String(
                    imageBytes);

            return
                $"data:{mimeType};base64,{base64Image}";
        }

        private static string GetImageMimeType(
            string imageFilePath)
        {
            string extension =
                Path.GetExtension(imageFilePath)
                    .ToLowerInvariant();

            return extension switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",

                _ => throw new InvalidOperationException(
                    "Unsupported image format. Use JPG, JPEG, PNG, " +
                    "GIF, or BMP.")
            };
        }

        private static string BuildAttachmentsSection(
    EmailType emailType)
        {
            CustomAttachmentsInputs? attachmentsInputs =
                emailType.inputs?
                    .OfType<CustomAttachmentsInputs>()
                    .FirstOrDefault();

            if (attachmentsInputs is null)
            {
                return string.Empty;
            }

            List<string> attachments =
                attachmentsInputs.AttachmentsList?
                    .Where(attachment =>
                        !string.IsNullOrWhiteSpace(attachment))
                    .Select(attachment =>
                        attachment.Trim())
                    .ToList()
                ?? new List<string>();

            bool hasLabel =
                !string.IsNullOrWhiteSpace(
                    attachmentsInputs.AttachmentsLabel);

            bool hasIntro =
                !string.IsNullOrWhiteSpace(
                    attachmentsInputs.AttachmentsIntro);

            if (!hasLabel &&
                !hasIntro &&
                attachments.Count == 0)
            {
                return string.Empty;
            }

            string labelHtml =
                hasLabel
                    ? $$"""
                <div
                    id="attachmentsLabel"
                    class="text-brand-green"
                    style="
                        font-family:Arial,Helvetica,sans-serif;
                        font-size:12px;
                        line-height:17px;
                        letter-spacing:1.5px;
                        text-transform:uppercase;
                        color:#0f5640 !important;
                        -webkit-text-fill-color:#0f5640 !important;
                        font-weight:bold;">

                    {{Encode(
                                attachmentsInputs
                                    .AttachmentsLabel
                                    .Trim())}}
                </div>
                """
                    : string.Empty;

            string introHtml =
                hasIntro
                    ? $$"""
                <div
                    id="attachmentsIntro"
                    class="text-body"
                    style="
                        margin-top:8px;
                        font-family:Arial,Helvetica,sans-serif;
                        font-size:16px;
                        line-height:25px;
                        color:#303936 !important;
                        -webkit-text-fill-color:#303936 !important;">

                    {{EncodeWithLineBreaks(
                                attachmentsInputs
                                    .AttachmentsIntro
                                    .Trim())}}
                </div>
                """
                    : string.Empty;

            string attachmentRowsHtml =
                string.Join(
                    Environment.NewLine,
                    attachments.Select(
                        (attachment, index) =>
                        {
                            string fileName =
                                GetAttachmentDisplayName(
                                    attachment);

                            return $$"""
                        <tr
                            id="attachmentItem{{index + 1}}">

                            <td
                                width="34"
                                valign="middle"
                                bgcolor="#ffffff"
                                style="
                                    width:34px;
                                    padding:12px 0 12px 14px;
                                    border-bottom:1px solid #bfdbd4;
                                    background-color:#ffffff !important;
                                    background-image:linear-gradient(
                                        #ffffff,
                                        #ffffff) !important;
                                    font-family:Arial,Helvetica,sans-serif;
                                    font-size:18px;
                                    line-height:22px;
                                    color:#0f5640 !important;
                                    -webkit-text-fill-color:#0f5640 !important;
                                    font-weight:bold;">

                                &#128206;
                            </td>

                            <td
                                valign="middle"
                                bgcolor="#ffffff"
                                class="text-body"
                                style="
                                    padding:12px 14px 12px 8px;
                                    border-bottom:1px solid #bfdbd4;
                                    background-color:#ffffff !important;
                                    background-image:linear-gradient(
                                        #ffffff,
                                        #ffffff) !important;
                                    font-family:Arial,Helvetica,sans-serif;
                                    font-size:15px;
                                    line-height:22px;
                                    color:#303936 !important;
                                    -webkit-text-fill-color:#303936 !important;
                                    font-weight:bold;">

                                {{Encode(fileName)}}
                            </td>
                        </tr>
                        """;
                        }));

            string attachmentsListHtml =
                attachments.Count == 0
                    ? string.Empty
                    : $$"""
                <table
                    id="attachmentsList"
                    role="presentation"
                    width="100%"
                    cellpadding="0"
                    cellspacing="0"
                    border="0"
                    bgcolor="#ffffff"
                    style="
                        width:100%;
                        margin-top:16px;
                        border-collapse:collapse;
                        border-left:5px solid #f28c18;
                        background-color:#ffffff !important;
                        background-image:linear-gradient(
                            #ffffff,
                            #ffffff) !important;">

                    {{attachmentRowsHtml}}
                </table>
                """;

            return $$"""
        <tr id="attachmentsSection">
            <td
                bgcolor="#bfdbd4"
                class="content-padding background-mint"
                style="
                    padding-top:28px;
                    padding-bottom:28px;
                    font-family:Arial,Helvetica,sans-serif;
                    background-color:#bfdbd4 !important;
                    background-image:linear-gradient(
                        #bfdbd4,
                        #bfdbd4) !important;">

                {{labelHtml}}

                {{introHtml}}

                {{attachmentsListHtml}}
            </td>
        </tr>
        """;
        }
        private static string GetAttachmentDisplayName(
    string attachment)
        {
            if (string.IsNullOrWhiteSpace(attachment))
            {
                return string.Empty;
            }

            string trimmedAttachment =
                attachment.Trim();

            try
            {
                string fileName =
                    Path.GetFileName(
                        trimmedAttachment);

                return string.IsNullOrWhiteSpace(fileName)
                    ? trimmedAttachment
                    : fileName;
            }
            catch
            {
                return trimmedAttachment;
            }
        }
        private static string BuildAnnouncementsSection(
    EmailType emailType)
        {
            CustomAnnouncementsInputs? announcementsInputs =
                emailType.inputs?
                    .OfType<CustomAnnouncementsInputs>()
                    .FirstOrDefault();

            if (announcementsInputs is null)
            {
                return string.Empty;
            }

            List<string> announcements =
                announcementsInputs.AnnouncementsList?
                    .Where(announcement =>
                        !string.IsNullOrWhiteSpace(announcement))
                    .Select(announcement =>
                        announcement.Trim())
                    .ToList()
                ?? new List<string>();

            bool hasLabel =
                !string.IsNullOrWhiteSpace(
                    announcementsInputs.AnnouncementsLabel);

            bool hasIntro =
                !string.IsNullOrWhiteSpace(
                    announcementsInputs.AnnouncementsIntro);

            if (!hasLabel &&
                !hasIntro &&
                announcements.Count == 0)
            {
                return string.Empty;
            }

            string labelHtml =
                hasLabel
                    ? $$"""
                <div
                    id="announcementsLabel"
                    class="text-brand-green"
                    style="
                        font-family:Arial,Helvetica,sans-serif;
                        font-size:12px;
                        line-height:17px;
                        letter-spacing:1.5px;
                        text-transform:uppercase;
                        color:#0f5640 !important;
                        -webkit-text-fill-color:#0f5640 !important;
                        font-weight:bold;">

                    {{Encode(
                                announcementsInputs
                                    .AnnouncementsLabel
                                    .Trim())}}
                </div>
                """
                    : string.Empty;

            string introHtml =
                hasIntro
                    ? $$"""
                <div
                    id="announcementsIntro"
                    class="text-body"
                    style="
                        margin-top:8px;
                        font-family:Arial,Helvetica,sans-serif;
                        font-size:16px;
                        line-height:25px;
                        color:#303936 !important;
                        -webkit-text-fill-color:#303936 !important;">

                    {{EncodeWithLineBreaks(
                                announcementsInputs
                                    .AnnouncementsIntro
                                    .Trim())}}
                </div>
                """
                    : string.Empty;

            string announcementItemsHtml =
                string.Join(
                    Environment.NewLine,
                    announcements.Select(
                        (announcement, index) =>
                            $$"""
                    <li
                        id="announcementItem{{index + 1}}"
                        bgcolor="#ffffff"
                        style="
                            margin:0 0 12px 0;
                            padding:14px 16px;
                            list-style:none;
                            border-left:5px solid #f28c18;
                            background-color:#ffffff !important;
                            background-image:linear-gradient(
                                #ffffff,
                                #ffffff) !important;
                            font-family:Arial,Helvetica,sans-serif;
                            font-size:15px;
                            line-height:23px;
                            color:#303936 !important;
                            -webkit-text-fill-color:#303936 !important;">

                        {{EncodeWithLineBreaks(announcement)}}
                    </li>
                    """));

            string listHtml =
                announcements.Count == 0
                    ? string.Empty
                    : $$"""
                <ul
                    id="announcementsList"
                    style="
                        margin:16px 0 0 0;
                        padding:0;
                        list-style:none;">

                    {{announcementItemsHtml}}
                </ul>
                """;

            return $$"""
        <tr id="announcementsSection">
            <td
                bgcolor="#f5f7f6"
                class="content-padding background-request"
                style="
                    padding-top:28px;
                    padding-bottom:28px;
                    font-family:Arial,Helvetica,sans-serif;
                    background-color:#f5f7f6 !important;
                    background-image:linear-gradient(
                        #f5f7f6,
                        #f5f7f6) !important;">

                {{labelHtml}}

                {{introHtml}}

                {{listHtml}}
            </td>
        </tr>
        """;
        }
        private static string BuildRequestSection(
    EmailType emailType)
        {
            CustomRequestInputs? requestInputs =
                emailType.inputs?
                    .OfType<CustomRequestInputs>()
                    .FirstOrDefault();

            if (requestInputs is null)
            {
                return string.Empty;
            }

            bool hasLabel =
                !string.IsNullOrWhiteSpace(
                    requestInputs.RequestLabel);

            bool hasTitle =
                !string.IsNullOrWhiteSpace(
                    requestInputs.RequestTitle);

            bool hasBody =
                !string.IsNullOrWhiteSpace(
                    requestInputs.RequestBody);

            bool hasButtonText =
                !string.IsNullOrWhiteSpace(
                    requestInputs.RequestButton);

            bool hasLink =
                !string.IsNullOrWhiteSpace(
                    requestInputs.RequestLink);

            if (!hasLabel &&
                !hasTitle &&
                !hasBody &&
                !hasButtonText &&
                !hasLink)
            {
                return string.Empty;
            }

            string labelHtml =
                hasLabel
                    ? $$"""
                
                <div
                    id="requestLabel"
                    class="text-brand-green"
                    style="
                        font-family:Arial,Helvetica,sans-serif;
                        font-size:12px;
                        line-height:17px;
                        letter-spacing:1.5px;
                        text-transform:uppercase;
                        color:#0f5640 !important;
                        -webkit-text-fill-color:#0f5640 !important;
                        font-weight:bold;">

                    {{Encode(
                                requestInputs
                                    .RequestLabel
                                    .Trim())}}
                </div>
                """
                    : string.Empty;

            string titleHtml =
                hasTitle
                    ? $$"""
                <div
                    id="requestTitle"
                    class="text-black"
                    style="
                        margin-top:6px;
                        font-family:Arial,Helvetica,sans-serif;
                        font-size:23px;
                        line-height:29px;
                        color:#111111 !important;
                        -webkit-text-fill-color:#111111 !important;
                        font-weight:bold;">

                    {{Encode(
                                requestInputs
                                    .RequestTitle
                                    .Trim())}}
                </div>
                """
                    : string.Empty;

            string bodyHtml =
                hasBody
                    ? $$"""
                <div
                    id="requestBody"
                    class="text-request"
                    style="
                        margin-top:10px;
                        font-family:Arial,Helvetica,sans-serif;
                        font-size:15px;
                        line-height:24px;
                        color:#4b5551 !important;
                        -webkit-text-fill-color:#4b5551 !important;">

                    {{EncodeWithLineBreaks(
                                requestInputs
                                    .RequestBody
                                    .Trim())}}
                </div>
                """
                    : string.Empty;

            string buttonHtml =
                hasButtonText && hasLink
                    ? BuildRequestButton(
                        requestInputs.RequestButton,
                        requestInputs.RequestLink)
                    : string.Empty;

            return $$"""
        <tr id="requestSection">
            <td
                bgcolor="#ffffff"
                class="content-padding background-white"
                style="
                    padding-top:34px;
                    padding-bottom:34px;
                    font-family:Arial,Helvetica,sans-serif;
                    background-color:#ffffff !important;
                    background-image:linear-gradient(
                        #ffffff,
                        #ffffff) !important;">

                <table
                    role="presentation"
                    width="100%"
                    cellpadding="0"
                    cellspacing="0"
                    border="0"
                    bgcolor="#f5f7f6"
                    class="background-request"
                    style="
                        width:100%;
                        border-collapse:collapse;
                        background-color:#f5f7f6 !important;
                        background-image:linear-gradient(
                            #f5f7f6,
                            #f5f7f6) !important;">

                    <tr>
                        <td
                            style="
                                padding:24px;
                                border-left:7px solid #54b948;">

                            {{labelHtml}}

                            {{titleHtml}}

                            {{bodyHtml}}

                            {{buttonHtml}}
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        """;
        }

        private static string BuildRequestButton(
    string buttonText,
    string requestLink)
        {
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
                margin-top:20px;
                border-collapse:collapse;">

            <tr>
                <td
                    bgcolor="#f28c18"
                    style="
                        border-radius:3px;
                        background-color:#f28c18 !important;
                        background-image:linear-gradient(
                            #f28c18,
                            #f28c18) !important;">

                    <a
                        id="requestButton"
                        href="{{EncodeAttribute(normalizedLink)}}"
                        target="_blank"
                        class="text-black"
                        style="
                            display:inline-block;
                            padding:13px 23px;
                            font-family:Arial,Helvetica,sans-serif;
                            font-size:16px;
                            line-height:21px;
                            font-weight:bold;
                            color:#111111 !important;
                            -webkit-text-fill-color:#111111 !important;
                            text-decoration:none;">

                        {{Encode(buttonText.Trim())}}
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

            if (string.IsNullOrWhiteSpace(trimmedLink))
            {
                return string.Empty;
            }

            /*
             * Allow users to enter:
             *
             * www.example.com
             *
             * instead of requiring:
             *
             * https://www.example.com
             */
            if (!trimmedLink.Contains(
                    "://",
                    StringComparison.Ordinal) &&
                !trimmedLink.StartsWith(
                    "mailto:",
                    StringComparison.OrdinalIgnoreCase))
            {
                trimmedLink =
                    "https://" + trimmedLink;
            }

            if (!Uri.TryCreate(
                    trimmedLink,
                    UriKind.Absolute,
                    out Uri? parsedUri))
            {
                throw new InvalidOperationException(
                    $"The request link is invalid: {requestLink}");
            }

            bool isAllowedScheme =
                parsedUri.Scheme.Equals(
                    Uri.UriSchemeHttp,
                    StringComparison.OrdinalIgnoreCase) ||
                parsedUri.Scheme.Equals(
                    Uri.UriSchemeHttps,
                    StringComparison.OrdinalIgnoreCase) ||
                parsedUri.Scheme.Equals(
                    "mailto",
                    StringComparison.OrdinalIgnoreCase);

            if (!isAllowedScheme)
            {
                throw new InvalidOperationException(
                    "The request link must use HTTP, HTTPS, or MAILTO.");
            }

            return parsedUri.AbsoluteUri;
        }

        private static string BuildSignatureSection(
    EmailType emailType)
        {
            CustomSignatureInputs? signatureInputs =
                emailType.inputs?
                    .OfType<CustomSignatureInputs>()
                    .FirstOrDefault();

            if (signatureInputs is null)
            {
                return string.Empty;
            }

            string phoneHtml =
                string.IsNullOrWhiteSpace(
                    signatureInputs.SenderPhone)
                    ? string.Empty
                    : $$"""
                <div
                    id="senderPhone"
                    class="text-secondary"
                    style="
                        font-size:14px;
                        line-height:22px;
                        color:#26312d !important;
                        -webkit-text-fill-color:#26312d !important;">

                    {{Encode(signatureInputs.SenderPhone)}}
                </div>
                """;

            string emailHtml =
                string.IsNullOrWhiteSpace(
                    signatureInputs.SenderEmail)
                    ? string.Empty
                    : $$"""
                <div
                    style="
                        margin-top:8px;
                        font-size:14px;
                        line-height:22px;">

                    <a
                        id="senderEmail"
                        href="mailto:{{EncodeAttribute(signatureInputs.SenderEmail)}}"
                        class="text-brand-green"
                        style="
                            color:#0f5640 !important;
                            -webkit-text-fill-color:#0f5640 !important;
                            text-decoration:underline;">

                        {{Encode(signatureInputs.SenderEmail)}}
                    </a>
                </div>
                """;

            return $$"""
        <tr id="signatureSection">
            <td
                bgcolor="#ffffff"
                class="content-padding background-white"
                style="
                    padding-top:30px;
                    padding-bottom:30px;
                    font-family:Arial,Helvetica,sans-serif;
                    background-color:#ffffff !important;
                    background-image:linear-gradient(#ffffff,#ffffff) !important;">

                <div
                    id="signatureClosing"
                    class="text-secondary"
                    style="
                        font-size:16px;
                        line-height:25px;
                        color:#26312d !important;
                        -webkit-text-fill-color:#26312d !important;">

                    {{EncodeWithLineBreaks(signatureInputs.SignatureClosing)}}
                </div>

                <div
                    id="senderName"
                    class="text-brand-green"
                    style="
                        margin-top:10px;
                        font-size:19px;
                        line-height:25px;
                        color:#0f5640 !important;
                        -webkit-text-fill-color:#0f5640 !important;
                        font-weight:bold;">

                    {{Encode(signatureInputs.SenderName)}}
                </div>

                <div
                    id="senderTitle"
                    class="text-secondary"
                    style="
                        font-size:14px;
                        line-height:22px;
                        color:#26312d !important;
                        -webkit-text-fill-color:#26312d !important;">

                    {{Encode(signatureInputs.SenderTitle)}}
                </div>

                <div
                    id="senderOrganization"
                    class="text-secondary"
                    style="
                        font-size:14px;
                        line-height:22px;
                        color:#26312d !important;
                        -webkit-text-fill-color:#26312d !important;">

                    {{Encode(signatureInputs.SenderOrganization)}}
                </div>

                {{emailHtml}}

                {{phoneHtml}}
            </td>
        </tr>
        """;
        }

        private static string BuildHeaderSection(
    EmailType emailType)
        {
            CustomHeaderInputs? headerInputs =
                emailType.inputs?
                    .OfType<CustomHeaderInputs>()
                    .FirstOrDefault();

            string organizationLabel =
                headerInputs?.OrganizationName
                ?? string.Empty;

            string headerLabel =
                headerInputs?.HeaderLabel
                ?? string.Empty;

            string emailHeading =
                headerInputs?.EmailHeading
                ?? string.Empty;

            string headerSubtitle =
                headerInputs?.HeaderSubtitle
                ?? string.Empty;

            string headerImageUrl =
    string.IsNullOrWhiteSpace(
        headerInputs?.HeaderImageUrl)
            ? EmailImageSources.Default_HeaderImage
            : headerInputs.HeaderImageUrl;

            return $$"""
                <tr id="headerOrganizationSection">
                    <td
                        bgcolor="#bfdbd4"
                        class="content-padding background-mint"
                        style="
                            padding-top:20px;
                            padding-bottom:18px;
                            border-top:8px solid #0f5640;
                            background-color:#bfdbd4 !important;
                            background-image:linear-gradient(#bfdbd4,#bfdbd4) !important;">

                        <div
                            id="organizationLabel"
                            class="text-brand-green"
                            style="
                                font-family:Arial,Helvetica,sans-serif;
                                font-size:12px;
                                line-height:16px;
                                letter-spacing:1.5px;
                                text-transform:uppercase;
                                color:#0f5640 !important;
                                -webkit-text-fill-color:#0f5640 !important;
                                font-weight:bold;">

                            {{Encode(organizationLabel)}}
                        </div>

                        <div
                            id="organizationName"
                            class="text-black"
                            style="
                                font-family:Arial,Helvetica,sans-serif;
                                font-size:24px;
                                line-height:29px;
                                color:#111111 !important;
                                -webkit-text-fill-color:#111111 !important;
                                font-weight:bold;">

                            UWGB Climbing Tower
                        </div>
                    </td>
                </tr>

                <tr id="headerImageSection">
                    <td
                        bgcolor="#0f5640"
                        class="background-brand-green"
                        style="
                            padding:0;
                            border-bottom:6px solid #f28c18;
                            background-color:#0f5640 !important;
                            background-image:linear-gradient(#0f5640,#0f5640) !important;">

                        <img
                            id="headerImage"
                            src="{{EncodeAttribute(headerImageUrl)}}"
                            width="640"
                            alt="UWGB Climbing Tower"
                            style="
                                display:block;
                                width:100%;
                                max-width:640px;
                                height:auto;
                                border:0;
                                outline:none;
                                text-decoration:none;">
                    </td>
                </tr>

                <tr id="headerHeadingSection">
                    <td
                        bgcolor="#0f5640"
                        class="content-padding background-brand-green text-white"
                        style="
                            padding-top:30px;
                            padding-bottom:34px;
                            font-family:Arial,Helvetica,sans-serif;
                            color:#ffffff !important;
                            -webkit-text-fill-color:#ffffff !important;
                            background-color:#0f5640 !important;
                            background-image:linear-gradient(#0f5640,#0f5640) !important;">

                        <div
                            id="headerLabel"
                            class="text-mint"
                            style="
                                font-size:12px;
                                line-height:17px;
                                letter-spacing:1.7px;
                                text-transform:uppercase;
                                color:#bfdbd4 !important;
                                -webkit-text-fill-color:#bfdbd4 !important;
                                font-weight:bold;">

                            {{Encode(headerLabel)}}
                        </div>

                        <div
                            id="emailHeading"
                            class="headline text-white"
                            style="
                                margin-top:9px;
                                font-size:40px;
                                line-height:46px;
                                font-weight:800;
                                color:#ffffff !important;
                                -webkit-text-fill-color:#ffffff !important;">

                            {{Encode(emailHeading)}}
                        </div>

                        <div
                            id="headerSubtitle"
                            class="text-header-light"
                            style="
                                margin-top:13px;
                                font-size:17px;
                                line-height:26px;
                                color:#eef7f4 !important;
                                -webkit-text-fill-color:#eef7f4 !important;">

                            {{EncodeWithLineBreaks(headerSubtitle)}}
                        </div>
                    </td>
                </tr>
                """;
        }

        private static string BuildBodySection(
            EmailType emailType)
        {
            string recipientGreeting =
                GetInputValue(
                    emailType,
                    "RecipientGreeting");

            string emailBody =
                GetInputValue(
                    emailType,
                    "EmailBody");

            return $$"""
                <tr id="bodySection">
                    <td
                        bgcolor="#ffffff"
                        class="content-padding background-white text-body"
                        style="
                            padding-top:36px;
                            padding-bottom:34px;
                            font-family:Arial,Helvetica,sans-serif;
                            color:#303936 !important;
                            -webkit-text-fill-color:#303936 !important;
                            background-color:#ffffff !important;
                            background-image:linear-gradient(#ffffff,#ffffff) !important;">

                        <div
                            id="recipientGreeting"
                            class="text-body"
                            style="
                                font-size:17px;
                                line-height:27px;
                                color:#303936 !important;
                                -webkit-text-fill-color:#303936 !important;">

                            {{EncodeWithLineBreaks(recipientGreeting)}}
                        </div>

                        <div
                            id="emailBody"
                            class="text-body"
                            style="
                                margin-top:18px;
                                font-size:16px;
                                line-height:27px;
                                color:#303936 !important;
                                -webkit-text-fill-color:#303936 !important;">

                            {{EncodeWithParagraphs(emailBody)}}
                        </div>
                    </td>
                </tr>
                """;
        }

        private static string BuildFooterSection(
    EmailType emailType)
        {
            CustomFooterInputs footerInputs =
                emailType.inputs
                    .OfType<CustomFooterInputs>()
                    .FirstOrDefault()
                ?? EmailInputDefaults.DefaultFooterInputs;

            return $$"""
                <tr id="footerSection">
                    <td
                        align="center"
                        bgcolor="#0f5640"
                        class="content-padding background-brand-green"
                        style="
                            padding-top:30px;
                            padding-bottom:24px;
                            font-family:Arial,Helvetica,sans-serif;
                            background-color:#0f5640 !important;
                            background-image:linear-gradient(#0f5640,#0f5640) !important;">

                        <img
                            id="footerLogo"
                            src="{{EncodeAttribute(EmailImageSources.Default_FooterImage)}}"
                            width="145"
                            alt="UWGB UREC Outdoors"
                            style="
                                display:block;
                                width:145px;
                                max-width:145px;
                                height:auto;
                                margin:0 auto;
                                border:0;
                                outline:none;
                                text-decoration:none;">

                        <div
                            id="footerOrganization"
                            class="text-mint"
                            style="
                                margin-top:15px;
                                font-size:13px;
                                line-height:20px;
                                color:#bfdbd4 !important;
                                -webkit-text-fill-color:#bfdbd4 !important;">

                            University of Wisconsin–Green Bay
                        </div>

                        <div
                            style="
                                margin-top:2px;
                                font-size:13px;
                                line-height:20px;">

                            <a
                                id="footerWebsiteLink"
                                href="https://www.uwgb.edu/urec/adventure/climbing/"
                                class="text-white"
                                style="
                                    color:#ffffff !important;
                                    -webkit-text-fill-color:#ffffff !important;
                                    text-decoration:underline;">

                                urec.uwgb.edu
                            </a>
                        </div>

                        <div
                            class="footer-divider"
                            style="
                                height:1px;
                                margin:20px 0 15px 0;
                                background-color:#397665 !important;
                                background-image:linear-gradient(#397665,#397665) !important;">
                        </div>

                        <div
                            id="footerText"
                            class="text-mint"
                            style="
                                font-size:11px;
                                line-height:17px;
                                color:#bfdbd4 !important;
                                -webkit-text-fill-color:#bfdbd4 !important;">

                            You are receiving this message because you are
                            affiliated with the UWGB Climbing Tower.
                        </div>
                    </td>
                </tr>
                """;
        }

        private static string ReplaceRequiredMarker(
            string html,
            string marker,
            string replacement)
        {
            if (!html.Contains(
                    marker,
                    StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"The email template is missing the marker: {marker}");
            }

            return html.Replace(
                marker,
                replacement,
                StringComparison.Ordinal);
        }

        private static string ReplaceElementContent(
            string html,
            string elementId,
            string replacementHtml)
        {
            string escapedElementId =
                Regex.Escape(elementId);

            string pattern =
                $"""
                (?<opening>
                    <
                    (?<tag>[a-zA-Z][a-zA-Z0-9]*)
                    \b
                    [^>]*
                    \bid\s*=\s*["']{escapedElementId}["']
                    [^>]*
                    >
                )
                (?<content>.*?)
                (?<closing>
                    </\k<tag>\s*>
                )
                """;

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
                    $"The email template does not contain an element with the ID '{elementId}'.");
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

        private static string GetInputValue(
            EmailType emailType,
            string propertyName)
        {
            if (emailType.inputs is null)
            {
                return string.Empty;
            }

            foreach (object inputGroup in emailType.inputs)
            {
                PropertyInfo? property =
                    inputGroup
                        .GetType()
                        .GetProperties(
                            BindingFlags.Public |
                            BindingFlags.Instance)
                        .FirstOrDefault(currentProperty =>
                            string.Equals(
                                currentProperty.Name,
                                propertyName,
                                StringComparison.OrdinalIgnoreCase));

                if (property is null ||
                    property.PropertyType != typeof(string))
                {
                    continue;
                }

                return property.GetValue(inputGroup)
                           ?.ToString()
                       ?? string.Empty;
            }

            return string.Empty;
        }

        private static string Encode(
            string value)
        {
            return WebUtility.HtmlEncode(
                value ?? string.Empty);
        }

        private static string EncodeAttribute(
            string value)
        {
            return WebUtility.HtmlEncode(
                value?.Trim() ?? string.Empty);
        }

        private static string EncodeWithLineBreaks(
            string value)
        {
            return Encode(value)
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
            if (string.IsNullOrWhiteSpace(value))
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
                paragraphs.Select(paragraph =>
                    $$"""
                    <p
                        class="text-body"
                        style="
                            margin:0 0 16px 0;
                            color:#303936 !important;
                            -webkit-text-fill-color:#303936 !important;">

                        {{EncodeWithLineBreaks(paragraph.Trim())}}
                    </p>
                    """));
        }
    }
}