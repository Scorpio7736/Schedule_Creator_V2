using Schedule_Creator_V2.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Reflection;

namespace Schedule_Creator_V2.Services
{
    public static class OutlookEmailService
    {
        private static readonly string[] EmailMemberNames =
        {
            "email",
            "emailAddress",
            "email_address"
        };

        private static readonly string[] DisplayNameMemberNames =
        {
            "displayName",
            "name",
            "fullName",
            "firstName"
        };

        public static void OpenNewEmail(
            IEnumerable<Staff> staffMembers,
            string subject,
            string body)
        {
            List<string> recipientAddresses =
                GetRecipientAddresses(staffMembers);

            if (recipientAddresses.Count == 0)
            {
                throw new InvalidOperationException(
                    "No valid recipient email addresses were found.");
            }

            string recipients =
                string.Join(",", recipientAddresses);

            string mailToUrl =
                $"mailto:{recipients}" +
                $"?subject={Uri.EscapeDataString(subject ?? string.Empty)}" +
                $"&body={Uri.EscapeDataString(body ?? string.Empty)}";

            try
            {
                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName = mailToUrl,
                        UseShellExecute = true
                    });
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException(
                    "The new Outlook compose window could not be opened.\n\n" +
                    "Make sure Outlook (new) is assigned as the default " +
                    "application for MAILTO links in Windows.",
                    exception);
            }
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
                    GetStringMemberValue(
                        staffMember,
                        EmailMemberNames);

                if (!IsValidEmailAddress(emailAddress))
                {
                    string displayName =
                        GetStringMemberValue(
                            staffMember,
                            DisplayNameMemberNames);

                    invalidStaffMembers.Add(
                        string.IsNullOrWhiteSpace(displayName)
                            ? "Unknown staff member"
                            : displayName);

                    continue;
                }

                validAddresses.Add(
                    emailAddress.Trim());
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

        private static string GetStringMemberValue(
            object source,
            IEnumerable<string> memberNames)
        {
            Type sourceType =
                source.GetType();

            foreach (string memberName in memberNames)
            {
                PropertyInfo? property =
                    sourceType.GetProperty(
                        memberName,
                        BindingFlags.Public |
                        BindingFlags.Instance |
                        BindingFlags.IgnoreCase);

                if (property is not null &&
                    property.PropertyType == typeof(string))
                {
                    return property.GetValue(source)
                               ?.ToString()
                           ?? string.Empty;
                }

                FieldInfo? field =
                    sourceType.GetField(
                        memberName,
                        BindingFlags.Public |
                        BindingFlags.Instance |
                        BindingFlags.IgnoreCase);

                if (field is not null &&
                    field.FieldType == typeof(string))
                {
                    return field.GetValue(source)
                               ?.ToString()
                           ?? string.Empty;
                }
            }

            return string.Empty;
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
                    new MailAddress(emailAddress.Trim());

                return string.Equals(
                    parsedAddress.Address,
                    emailAddress.Trim(),
                    StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}