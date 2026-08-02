using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Records;
using System;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Schedule_Creator_V2.Services
{

    public static class EmailContentService
    {
        public static string BuildPlainTextBody(
    EmailType emailType)
        {
            string organizationName =
                GetInputValue(
                    emailType,
                    "OrganizationName");

            string headerLabel =
                GetInputValue(
                    emailType,
                    "HeaderLabel");

            string emailHeading =
                GetInputValue(
                    emailType,
                    "EmailHeading");

            string headerSubtitle =
                GetInputValue(
                    emailType,
                    "HeaderSubtitle");

            string recipientGreeting =
                GetInputValue(
                    emailType,
                    "RecipientGreeting");

            string emailBody =
                GetInputValue(
                    emailType,
                    "EmailBody");

            List<string> sections =
                new List<string>();

            if (!string.IsNullOrWhiteSpace(organizationName))
            {
                sections.Add(organizationName.Trim());
            }

            if (!string.IsNullOrWhiteSpace(headerLabel))
            {
                sections.Add(headerLabel.Trim());
            }

            if (!string.IsNullOrWhiteSpace(emailHeading))
            {
                sections.Add(emailHeading.Trim());
            }

            if (!string.IsNullOrWhiteSpace(headerSubtitle))
            {
                sections.Add(headerSubtitle.Trim());
            }

            if (!string.IsNullOrWhiteSpace(recipientGreeting))
            {
                sections.Add(recipientGreeting.Trim());
            }

            if (!string.IsNullOrWhiteSpace(emailBody))
            {
                sections.Add(emailBody.Trim());
            }

            return string.Join(
                Environment.NewLine + Environment.NewLine,
                sections);
        }
        public static string BuildSubject(
            EmailType emailType)
        {
            string subject =
                GetInputValue(
                    emailType,
                    "Subject");

            if (!string.IsNullOrWhiteSpace(subject))
            {
                return subject.Trim();
            }

            return emailType.displayName;
        }

        public static string BuildHtmlBody(
            EmailType emailType)
        {
            string organizationName =
                GetInputValue(
                    emailType,
                    "OrganizationName");

            string headerLabel =
                GetInputValue(
                    emailType,
                    "HeaderLabel");

            string emailHeading =
                GetInputValue(
                    emailType,
                    "EmailHeading");

            string headerSubtitle =
                GetInputValue(
                    emailType,
                    "HeaderSubtitle");

            string recipientGreeting =
                GetInputValue(
                    emailType,
                    "RecipientGreeting");

            string emailBody =
                GetInputValue(
                    emailType,
                    "EmailBody");

            return $$"""
                <!doctype html>
                <html lang="en">
                <head>
                    <meta charset="utf-8">
                    <meta name="viewport"
                          content="width=device-width, initial-scale=1">
                </head>

                <body style="
                    margin:0;
                    padding:0;
                    background-color:#eef3f1;
                    font-family:Arial, Helvetica, sans-serif;">

                    <table role="presentation"
                           width="100%"
                           cellpadding="0"
                           cellspacing="0"
                           border="0"
                           style="background-color:#eef3f1;">

                        <tr>
                            <td align="center"
                                style="padding:30px 15px;">

                                <table role="presentation"
                                       width="640"
                                       cellpadding="0"
                                       cellspacing="0"
                                       border="0"
                                       style="
                                           width:100%;
                                           max-width:640px;
                                           background-color:#ffffff;
                                           border-collapse:collapse;">

                                    <tr>
                                        <td style="
                                            padding:20px 36px;
                                            background-color:#bfdbd4;
                                            border-top:8px solid #0f5640;">

                                            <div style="
                                                color:#0f5640;
                                                font-size:12px;
                                                font-weight:bold;
                                                letter-spacing:1.5px;
                                                text-transform:uppercase;">

                                                {{Encode(organizationName)}}
                                            </div>

                                            <div style="
                                                margin-top:4px;
                                                color:#111111;
                                                font-size:24px;
                                                font-weight:bold;">

                                                UWGB Climbing Tower
                                            </div>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td style="
                                            padding:34px 36px;
                                            background-color:#0f5640;
                                            border-bottom:6px solid #f28c18;
                                            color:#ffffff;">

                                            <div style="
                                                color:#bfdbd4;
                                                font-size:13px;
                                                font-weight:bold;
                                                letter-spacing:2px;
                                                text-transform:uppercase;">

                                                {{Encode(headerLabel)}}
                                            </div>

                                            <div style="
                                                margin-top:8px;
                                                font-size:40px;
                                                line-height:46px;
                                                font-weight:800;">

                                                {{Encode(emailHeading)}}
                                            </div>

                                            <div style="
                                                margin-top:14px;
                                                color:#eef7f4;
                                                font-size:17px;
                                                line-height:26px;">

                                                {{Encode(headerSubtitle)}}
                                            </div>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td style="
                                            padding:36px;
                                            color:#303936;">

                                            <div style="
                                                font-size:17px;
                                                line-height:27px;">

                                                {{Encode(recipientGreeting)}}
                                            </div>

                                            <div style="
                                                margin-top:18px;
                                                font-size:16px;
                                                line-height:27px;">

                                                {{EncodeWithLineBreaks(emailBody)}}
                                            </div>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td align="center"
                                            style="
                                                padding:24px 36px;
                                                background-color:#0f5640;
                                                color:#bfdbd4;
                                                font-size:12px;
                                                line-height:18px;">

                                            University of Wisconsin–Green Bay<br>
                                            UWGB UREC Climbing Tower
                                        </td>
                                    </tr>

                                </table>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>
                """;
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

        private static string EncodeWithLineBreaks(
            string value)
        {
            string encodedValue =
                Encode(value);

            return encodedValue
                .Replace(
                    "\r\n",
                    "<br>")
                .Replace(
                    "\n",
                    "<br>");
        }
    }
}