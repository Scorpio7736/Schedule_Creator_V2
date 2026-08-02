using Schedule_Creator_V2.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace Schedule_Creator_V2.Services
{
    public static class EmlEmailService
    {
        private const int TemporaryFileLifetimeDays = 7;

        public static string CreateAndOpenEmail(
    IEnumerable<Staff> toStaffMembers,
    IEnumerable<Staff> ccStaffMembers,
    string subject,
    string htmlBody)
        {
            ArgumentNullException.ThrowIfNull(
                toStaffMembers);

            ArgumentNullException.ThrowIfNull(
                ccStaffMembers);

            List<string> toAddresses =
                GetRecipientAddresses(
                    toStaffMembers);

            if (toAddresses.Count == 0)
            {
                throw new InvalidOperationException(
                    "No valid To recipient email addresses were found.");
            }

            List<string> ccAddresses =
                GetRecipientAddresses(
                    ccStaffMembers)
                    .Where(ccAddress =>
                        !toAddresses.Contains(
                            ccAddress,
                            StringComparer.OrdinalIgnoreCase))
                    .ToList();

            string emailDirectory =
                GetEmailDirectory();

            DeleteOldEmailFiles(
                emailDirectory);

            string emlContent =
                BuildEmlContent(
                    toAddresses,
                    ccAddresses,
                    subject,
                    htmlBody);

            string emlFilePath =
                CreateEmlFilePath(
                    emailDirectory,
                    subject);

            WriteEmlFile(
                emlFilePath,
                emlContent);

            OpenEmlFile(
                emlFilePath);

            return emlFilePath;
        }

        private static string BuildEmlContent(
    List<string> toAddresses,
    List<string> ccAddresses,
    string subject,
    string htmlBody)
        {
            string boundary =
                "ScheduleCreatorBoundary_" +
                Guid.NewGuid().ToString("N");

            string encodedSubject =
                EncodeHeaderValue(subject);

            string plainTextBody =
                ConvertHtmlToPlainText(htmlBody);

            string encodedPlainText =
                ConvertToBase64WithLineBreaks(
                    plainTextBody);

            string encodedHtml =
                ConvertToBase64WithLineBreaks(
                    htmlBody);

            StringBuilder emlBuilder =
                new StringBuilder();

            /*
             * X-Unsent must be placed before the other headers.
             * Outlook uses this to identify the file as an unsent message.
             */
            emlBuilder.Append(
                "X-Unsent: 1\r\n");

            /*
             * Primary recipients.
             */
            emlBuilder.Append(
                "To: ");

            emlBuilder.Append(
                string.Join(
                    ", ",
                    toAddresses));

            emlBuilder.Append(
                "\r\n");

            /*
             * Carbon-copy recipients.
             * Do not write the header when no CC recipients were selected.
             */
            if (ccAddresses.Count > 0)
            {
                emlBuilder.Append(
                    "Cc: ");

                emlBuilder.Append(
                    string.Join(
                        ", ",
                        ccAddresses));

                emlBuilder.Append(
                    "\r\n");
            }

            emlBuilder.Append(
                "Subject: ");

            emlBuilder.Append(
                encodedSubject);

            emlBuilder.Append(
                "\r\n");

            emlBuilder.Append(
                "Date: ");

            emlBuilder.Append(
                DateTimeOffset.Now.ToString("r"));

            emlBuilder.Append(
                "\r\n");

            emlBuilder.Append(
                "MIME-Version: 1.0\r\n");

            emlBuilder.Append(
                "Content-Type: multipart/alternative; ");

            emlBuilder.Append(
                $"boundary=\"{boundary}\"\r\n");

            emlBuilder.Append(
                "\r\n");

            /*
             * Plain-text fallback.
             */
            emlBuilder.Append(
                $"--{boundary}\r\n");

            emlBuilder.Append(
                "Content-Type: text/plain; charset=\"utf-8\"\r\n");

            emlBuilder.Append(
                "Content-Transfer-Encoding: base64\r\n");

            emlBuilder.Append(
                "\r\n");

            emlBuilder.Append(
                encodedPlainText);

            emlBuilder.Append(
                "\r\n");

            /*
             * Full HTML email body.
             */
            emlBuilder.Append(
                $"--{boundary}\r\n");

            emlBuilder.Append(
                "Content-Type: text/html; charset=\"utf-8\"\r\n");

            emlBuilder.Append(
                "Content-Transfer-Encoding: base64\r\n");

            emlBuilder.Append(
                "\r\n");

            emlBuilder.Append(
                encodedHtml);

            emlBuilder.Append(
                "\r\n");

            /*
             * Close the MIME boundary.
             */
            emlBuilder.Append(
                $"--{boundary}--\r\n");

            return emlBuilder.ToString();
        }

        private static List<string> GetRecipientAddresses(
            IEnumerable<Staff> staffMembers)
        {
            List<string> validAddresses =
                new List<string>();

            List<string> invalidStaffMembers =
                new List<string>();

            foreach (Staff staffMember in staffMembers)
            {
                string emailAddress =
                    staffMember.email?.Trim()
                    ?? string.Empty;

                if (!IsValidEmailAddress(emailAddress))
                {
                    string displayName =
                        string.IsNullOrWhiteSpace(
                            staffMember.displayName)
                            ? "Unknown staff member"
                            : staffMember.displayName;

                    invalidStaffMembers.Add(
                        displayName);

                    continue;
                }

                validAddresses.Add(
                    emailAddress);
            }

            if (invalidStaffMembers.Count > 0)
            {
                throw new InvalidOperationException(
                    "The following selected staff members do not " +
                    "have valid email addresses:\n\n" +
                    string.Join(
                        Environment.NewLine,
                        invalidStaffMembers));
            }

            return validAddresses
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static bool IsValidEmailAddress(
            string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                return false;
            }

            try
            {
                MailAddress parsedAddress =
                    new MailAddress(emailAddress);

                return string.Equals(
                    parsedAddress.Address,
                    emailAddress,
                    StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private static string GetEmailDirectory()
        {
            string emailDirectory =
                Path.Combine(
                    Path.GetTempPath(),
                    "Schedule_Creator_V2",
                    "GeneratedEmails");

            Directory.CreateDirectory(
                emailDirectory);

            return emailDirectory;
        }

        private static string CreateEmlFilePath(
            string emailDirectory,
            string subject)
        {
            string safeSubject =
                CreateSafeFileName(subject);

            string timestamp =
                DateTime.Now.ToString(
                    "yyyy-MM-dd_HH-mm-ss");

            string fileName =
                $"{safeSubject}_{timestamp}.eml";

            return Path.Combine(
                emailDirectory,
                fileName);
        }

        private static string CreateSafeFileName(
            string subject)
        {
            string fileName =
                string.IsNullOrWhiteSpace(subject)
                    ? "Generated_Email"
                    : subject.Trim();

            foreach (char invalidCharacter in
                     Path.GetInvalidFileNameChars())
            {
                fileName =
                    fileName.Replace(
                        invalidCharacter,
                        '_');
            }

            fileName =
                Regex.Replace(
                    fileName,
                    @"\s+",
                    "_");

            const int maximumSubjectLength = 75;

            if (fileName.Length > maximumSubjectLength)
            {
                fileName =
                    fileName.Substring(
                        0,
                        maximumSubjectLength);
            }

            return fileName;
        }

        private static void WriteEmlFile(
            string emlFilePath,
            string emlContent)
        {
            /*
             * Do not include a UTF-8 byte-order mark.
             * The first bytes should be the X-Unsent header.
             */
            UTF8Encoding utf8WithoutBom =
                new UTF8Encoding(
                    encoderShouldEmitUTF8Identifier: false);

            File.WriteAllText(
                emlFilePath,
                emlContent,
                utf8WithoutBom);
        }

        private static void OpenEmlFile(
            string emlFilePath)
        {
            try
            {
                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName = emlFilePath,
                        UseShellExecute = true
                    });
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException(
                    "The generated email file could not be opened.\n\n" +
                    "Make sure Outlook (new) is assigned as the " +
                    "default application for .eml files.",
                    exception);
            }
        }

        private static string EncodeHeaderValue(
            string value)
        {
            string safeValue =
                value ?? string.Empty;

            byte[] valueBytes =
                Encoding.UTF8.GetBytes(
                    safeValue);

            string base64Value =
                Convert.ToBase64String(
                    valueBytes);

            return $"=?utf-8?B?{base64Value}?=";
        }

        private static string ConvertToBase64WithLineBreaks(
            string value)
        {
            byte[] contentBytes =
                Encoding.UTF8.GetBytes(
                    value ?? string.Empty);

            return Convert.ToBase64String(
                contentBytes,
                Base64FormattingOptions.InsertLineBreaks);
        }

        private static string ConvertHtmlToPlainText(
            string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return string.Empty;
            }

            string text =
                Regex.Replace(
                    html,
                    @"<\s*br\s*/?\s*>",
                    Environment.NewLine,
                    RegexOptions.IgnoreCase);

            text =
                Regex.Replace(
                    text,
                    @"</\s*p\s*>",
                    Environment.NewLine +
                    Environment.NewLine,
                    RegexOptions.IgnoreCase);

            text =
                Regex.Replace(
                    text,
                    @"</\s*div\s*>",
                    Environment.NewLine,
                    RegexOptions.IgnoreCase);

            text =
                Regex.Replace(
                    text,
                    @"<style\b[^>]*>.*?</style>",
                    string.Empty,
                    RegexOptions.IgnoreCase |
                    RegexOptions.Singleline);

            text =
                Regex.Replace(
                    text,
                    @"<script\b[^>]*>.*?</script>",
                    string.Empty,
                    RegexOptions.IgnoreCase |
                    RegexOptions.Singleline);

            text =
                Regex.Replace(
                    text,
                    @"<[^>]+>",
                    string.Empty,
                    RegexOptions.Singleline);

            text =
                WebUtility.HtmlDecode(text);

            text =
                Regex.Replace(
                    text,
                    @"[ \t]+\r?\n",
                    Environment.NewLine);

            text =
                Regex.Replace(
                    text,
                    @"(\r?\n){3,}",
                    Environment.NewLine +
                    Environment.NewLine);

            return text.Trim();
        }

        private static void DeleteOldEmailFiles(
            string emailDirectory)
        {
            try
            {
                DateTime expirationDate =
                    DateTime.Now.AddDays(
                        -TemporaryFileLifetimeDays);

                IEnumerable<string> oldEmailFiles =
                    Directory
                        .EnumerateFiles(
                            emailDirectory,
                            "*.eml")
                        .Where(filePath =>
                            File.GetLastWriteTime(filePath) <
                            expirationDate);

                foreach (string oldEmailFile in oldEmailFiles)
                {
                    try
                    {
                        File.Delete(oldEmailFile);
                    }
                    catch
                    {
                        /*
                         * A temporary file being used by Outlook
                         * should not prevent a new email from opening.
                         */
                    }
                }
            }
            catch
            {
                /*
                 * Cleanup failure should not prevent generation.
                 */
            }
        }
    }
}