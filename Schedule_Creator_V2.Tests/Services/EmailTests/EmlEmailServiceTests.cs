using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Enums;
using Schedule_Creator_V2.Services.Email;
using System.Reflection;
using System.Text;

namespace Schedule_Creator_V2.Tests.Services.Email
{
    public sealed class EmlEmailServiceTests
    {
        // =========================================================
        // PUBLIC VALIDATION
        // =========================================================

        [Fact]
        public void CreateAndOpenEmail_NullToRecipients_ThrowsArgumentNullException()
        {
            // Arrange

            List<Staff> cc =
                new List<Staff>();


            // Act

            Action action =
                () =>
                    EmlEmailService.CreateAndOpenEmail(
                        null!,
                        cc,
                        "Test",
                        "<p>Test</p>");


            // Assert

            Assert.Throws<ArgumentNullException>(
                action);
        }


        [Fact]
        public void CreateAndOpenEmail_NullCcRecipients_ThrowsArgumentNullException()
        {
            // Arrange

            List<Staff> to =
                new List<Staff>
                {
                    CreateStaff(
                        1,
                        "Test",
                        "Person",
                        "test@example.com")
                };


            // Act

            Action action =
                () =>
                    EmlEmailService.CreateAndOpenEmail(
                        to,
                        null!,
                        "Test",
                        "<p>Test</p>");


            // Assert

            Assert.Throws<ArgumentNullException>(
                action);
        }


        [Fact]
        public void CreateAndOpenEmail_EmptyToRecipients_ThrowsInvalidOperationException()
        {
            // Arrange

            List<Staff> to =
                new List<Staff>();

            List<Staff> cc =
                new List<Staff>();


            // Act

            InvalidOperationException exception =
                Assert.Throws<InvalidOperationException>(
                    () =>
                        EmlEmailService.CreateAndOpenEmail(
                            to,
                            cc,
                            "Test",
                            "<p>Test</p>"));


            // Assert

            Assert.Contains(
                "No valid To recipient",
                exception.Message);
        }


        [Fact]
        public void CreateAndOpenEmail_InvalidToEmail_ThrowsInvalidOperationException()
        {
            // Arrange

            List<Staff> to =
                new List<Staff>
                {
                    CreateStaff(
                        1,
                        "Invalid",
                        "Person",
                        "not-an-email")
                };

            List<Staff> cc =
                new List<Staff>();


            // Act

            InvalidOperationException exception =
                Assert.Throws<InvalidOperationException>(
                    () =>
                        EmlEmailService.CreateAndOpenEmail(
                            to,
                            cc,
                            "Test",
                            "<p>Test</p>"));


            // Assert

            Assert.Contains(
                "Invalid Person",
                exception.Message);
        }


        // =========================================================
        // RECIPIENT ADDRESSES
        // =========================================================

        [Fact]
        public void GetRecipientAddresses_ValidAddresses_ReturnsAddresses()
        {
            // Arrange

            List<Staff> staff =
                new List<Staff>
                {
                    CreateStaff(
                        1,
                        "First",
                        "Person",
                        "first@example.com"),

                    CreateStaff(
                        2,
                        "Second",
                        "Person",
                        "second@example.com")
                };


            // Act

            List<string> result =
                InvokePrivate<List<string>>(
                    "GetRecipientAddresses",
                    staff);


            // Assert

            Assert.Equal(
                2,
                result.Count);

            Assert.Contains(
                "first@example.com",
                result);

            Assert.Contains(
                "second@example.com",
                result);
        }


        [Fact]
        public void GetRecipientAddresses_AddressHasWhitespace_TrimsAddress()
        {
            // Arrange

            List<Staff> staff =
                new List<Staff>
                {
                    CreateStaff(
                        1,
                        "Test",
                        "Person",
                        "  test@example.com  ")
                };


            // Act

            List<string> result =
                InvokePrivate<List<string>>(
                    "GetRecipientAddresses",
                    staff);


            // Assert

            Assert.Single(
                result);

            Assert.Equal(
                "test@example.com",
                result[0]);
        }


        [Fact]
        public void GetRecipientAddresses_DuplicateAddresses_RemovesDuplicates()
        {
            // Arrange

            List<Staff> staff =
                new List<Staff>
                {
                    CreateStaff(
                        1,
                        "First",
                        "Person",
                        "staff@example.com"),

                    CreateStaff(
                        2,
                        "Second",
                        "Person",
                        "STAFF@example.com")
                };


            // Act

            List<string> result =
                InvokePrivate<List<string>>(
                    "GetRecipientAddresses",
                    staff);


            // Assert

            Assert.Single(
                result);
        }


        // =========================================================
        // SUBJECT ENCODING
        // =========================================================

        [Fact]
        public void EncodeHeaderValue_AsciiSubject_ReturnsMimeEncodedSubject()
        {
            // Arrange

            const string subject =
                "Tower Schedule";

            string expectedBase64 =
                Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(
                        subject));


            // Act

            string result =
                InvokePrivate<string>(
                    "EncodeHeaderValue",
                    subject);


            // Assert

            Assert.Equal(
                $"=?utf-8?B?{expectedBase64}?=",
                result);
        }


        [Fact]
        public void EncodeHeaderValue_UnicodeSubject_PreservesUnicodeThroughBase64()
        {
            // Arrange

            const string subject =
                "Tower Schedule – Fall 2026";

            string expectedBase64 =
                Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(
                        subject));


            // Act

            string result =
                InvokePrivate<string>(
                    "EncodeHeaderValue",
                    subject);


            // Assert

            Assert.Equal(
                $"=?utf-8?B?{expectedBase64}?=",
                result);
        }


        [Fact]
        public void EncodeHeaderValue_NullValue_EncodesEmptyString()
        {
            // Act

            string result =
    InvokePrivate<string>(
        "EncodeHeaderValue",
        new object?[]
        {
            null
        });


            // Assert

            Assert.Equal(
                "=?utf-8?B??=",
                result);
        }


        // =========================================================
        // BASE64
        // =========================================================

        [Fact]
        public void ConvertToBase64WithLineBreaks_Text_CanBeDecoded()
        {
            // Arrange

            const string original =
                "Hello Tower Team";


            // Act

            string encoded =
                InvokePrivate<string>(
                    "ConvertToBase64WithLineBreaks",
                    original);

            byte[] decodedBytes =
                Convert.FromBase64String(
                    encoded);

            string decoded =
                Encoding.UTF8.GetString(
                    decodedBytes);


            // Assert

            Assert.Equal(
                original,
                decoded);
        }


        [Fact]
        public void ConvertToBase64WithLineBreaks_Unicode_CanBeDecoded()
        {
            // Arrange

            const string original =
                "Schedule – September ✓";


            // Act

            string encoded =
                InvokePrivate<string>(
                    "ConvertToBase64WithLineBreaks",
                    original);

            string decoded =
                Encoding.UTF8.GetString(
                    Convert.FromBase64String(
                        encoded));


            // Assert

            Assert.Equal(
                original,
                decoded);
        }


        // =========================================================
        // HTML TO PLAIN TEXT
        // =========================================================

        [Fact]
        public void ConvertHtmlToPlainText_Paragraph_RemovesHtmlTags()
        {
            // Arrange

            const string html =
                "<p>Hello Tower Team</p>";


            // Act

            string result =
                InvokePrivate<string>(
                    "ConvertHtmlToPlainText",
                    html);


            // Assert

            Assert.Equal(
                "Hello Tower Team",
                result);
        }


        [Fact]
        public void ConvertHtmlToPlainText_BrTag_ProducesNewLine()
        {
            // Arrange

            const string html =
                "First Line<br />Second Line";


            // Act

            string result =
                InvokePrivate<string>(
                    "ConvertHtmlToPlainText",
                    html);


            // Assert

            Assert.Equal(
                "First Line" +
                Environment.NewLine +
                "Second Line",
                result);
        }


        [Fact]
        public void ConvertHtmlToPlainText_HtmlEntities_DecodesEntities()
        {
            // Arrange

            const string html =
                "<p>A &amp; B &lt; C</p>";


            // Act

            string result =
                InvokePrivate<string>(
                    "ConvertHtmlToPlainText",
                    html);


            // Assert

            Assert.Equal(
                "A & B < C",
                result);
        }


        [Fact]
        public void ConvertHtmlToPlainText_StyleElement_RemovesStyleContent()
        {
            // Arrange

            const string html =
                """
                <style>
                    .test { color:red; }
                </style>
                <p>Hello</p>
                """;


            // Act

            string result =
                InvokePrivate<string>(
                    "ConvertHtmlToPlainText",
                    html);


            // Assert

            Assert.DoesNotContain(
                "color:red",
                result);

            Assert.Contains(
                "Hello",
                result);
        }


        [Fact]
        public void ConvertHtmlToPlainText_ScriptElement_RemovesScriptContent()
        {
            // Arrange

            const string html =
                """
                <script>
                    alert('test');
                </script>
                <p>Hello</p>
                """;


            // Act

            string result =
                InvokePrivate<string>(
                    "ConvertHtmlToPlainText",
                    html);


            // Assert

            Assert.DoesNotContain(
                "alert",
                result);

            Assert.Contains(
                "Hello",
                result);
        }


        [Fact]
        public void ConvertHtmlToPlainText_EmptyHtml_ReturnsEmptyString()
        {
            // Act

            string result =
                InvokePrivate<string>(
                    "ConvertHtmlToPlainText",
                    string.Empty);


            // Assert

            Assert.Equal(
                string.Empty,
                result);
        }


        // =========================================================
        // SAFE FILE NAME
        // =========================================================

        [Fact]
        public void CreateSafeFileName_NormalSubject_ReturnsSubject()
        {
            // Act

            string result =
                InvokePrivate<string>(
                    "CreateSafeFileName",
                    "TowerSchedule");


            // Assert

            Assert.Equal(
                "TowerSchedule",
                result);
        }


        [Fact]
        public void CreateSafeFileName_SubjectHasSpaces_ReplacesSpacesWithUnderscores()
        {
            // Act

            string result =
                InvokePrivate<string>(
                    "CreateSafeFileName",
                    "Fall Tower Schedule");


            // Assert

            Assert.Equal(
                "Fall_Tower_Schedule",
                result);
        }


        [Fact]
        public void CreateSafeFileName_MultipleWhitespace_CollapsesToSingleUnderscore()
        {
            // Act

            string result =
                InvokePrivate<string>(
                    "CreateSafeFileName",
                    "Fall    Tower     Schedule");


            // Assert

            Assert.Equal(
                "Fall_Tower_Schedule",
                result);
        }


        [Fact]
        public void CreateSafeFileName_EmptySubject_ReturnsGeneratedEmail()
        {
            // Act

            string result =
                InvokePrivate<string>(
                    "CreateSafeFileName",
                    string.Empty);


            // Assert

            Assert.Equal(
                "Generated_Email",
                result);
        }


        [Fact]
        public void CreateSafeFileName_LongSubject_TruncatesTo75Characters()
        {
            // Arrange

            string subject =
                new string(
                    'A',
                    100);


            // Act

            string result =
                InvokePrivate<string>(
                    "CreateSafeFileName",
                    subject);


            // Assert

            Assert.Equal(
                75,
                result.Length);
        }


        // =========================================================
        // BUILD EML CONTENT
        // =========================================================

        [Fact]
        public void BuildEmlContent_ValidMessage_StartsWithXUnsent()
        {
            // Arrange

            List<string> to =
                new List<string>
                {
                    "to@example.com"
                };

            List<string> cc =
                new List<string>();


            // Act

            string result =
                InvokePrivate<string>(
                    "BuildEmlContent",
                    to,
                    cc,
                    "Test Subject",
                    "<p>Hello</p>");


            // Assert

            Assert.StartsWith(
                "X-Unsent: 1\r\n",
                result);
        }


        [Fact]
        public void BuildEmlContent_ToRecipients_WritesToHeader()
        {
            // Arrange

            List<string> to =
                new List<string>
                {
                    "first@example.com",
                    "second@example.com"
                };


            // Act

            string result =
                InvokePrivate<string>(
                    "BuildEmlContent",
                    to,
                    new List<string>(),
                    "Test",
                    "<p>Hello</p>");


            // Assert

            Assert.Contains(
                "To: first@example.com, second@example.com\r\n",
                result);
        }


        [Fact]
        public void BuildEmlContent_CcRecipients_WritesCcHeader()
        {
            // Arrange

            List<string> cc =
                new List<string>
                {
                    "cc@example.com"
                };


            // Act

            string result =
                InvokePrivate<string>(
                    "BuildEmlContent",
                    new List<string>
                    {
                        "to@example.com"
                    },
                    cc,
                    "Test",
                    "<p>Hello</p>");


            // Assert

            Assert.Contains(
                "Cc: cc@example.com\r\n",
                result);
        }


        [Fact]
        public void BuildEmlContent_NoCcRecipients_DoesNotWriteCcHeader()
        {
            // Act

            string result =
                InvokePrivate<string>(
                    "BuildEmlContent",
                    new List<string>
                    {
                        "to@example.com"
                    },
                    new List<string>(),
                    "Test",
                    "<p>Hello</p>");


            // Assert

            Assert.DoesNotContain(
                "\r\nCc:",
                result);
        }


        [Fact]
        public void BuildEmlContent_Subject_WritesEncodedSubjectHeader()
        {
            // Arrange

            const string subject =
                "Tower Schedule";

            string expected =
                "=?utf-8?B?" +
                Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(
                        subject)) +
                "?=";


            // Act

            string result =
                InvokePrivate<string>(
                    "BuildEmlContent",
                    new List<string>
                    {
                        "to@example.com"
                    },
                    new List<string>(),
                    subject,
                    "<p>Hello</p>");


            // Assert

            Assert.Contains(
                $"Subject: {expected}\r\n",
                result);
        }


        [Fact]
        public void BuildEmlContent_Message_IsMultipartAlternative()
        {
            // Act

            string result =
                InvokePrivate<string>(
                    "BuildEmlContent",
                    new List<string>
                    {
                        "to@example.com"
                    },
                    new List<string>(),
                    "Test",
                    "<p>Hello</p>");


            // Assert

            Assert.Contains(
                "MIME-Version: 1.0",
                result);

            Assert.Contains(
                "Content-Type: multipart/alternative;",
                result);

            Assert.Contains(
                "Content-Type: text/plain; charset=\"utf-8\"",
                result);

            Assert.Contains(
                "Content-Type: text/html; charset=\"utf-8\"",
                result);
        }


        [Fact]
        public void BuildEmlContent_BodiesUseBase64Encoding()
        {
            // Act

            string result =
                InvokePrivate<string>(
                    "BuildEmlContent",
                    new List<string>
                    {
                        "to@example.com"
                    },
                    new List<string>(),
                    "Test",
                    "<p>Hello</p>");


            // Assert

            Assert.Equal(
                2,
                CountOccurrences(
                    result,
                    "Content-Transfer-Encoding: base64"));
        }


        [Fact]
        public void BuildEmlContent_HtmlBody_ContainsBase64EncodedHtml()
        {
            // Arrange

            const string html =
                "<p>Hello Tower Team</p>";

            string expectedHtml =
                Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(
                        html),
                    Base64FormattingOptions
                        .InsertLineBreaks);


            // Act

            string result =
                InvokePrivate<string>(
                    "BuildEmlContent",
                    new List<string>
                    {
                        "to@example.com"
                    },
                    new List<string>(),
                    "Test",
                    html);


            // Assert

            Assert.Contains(
                expectedHtml,
                result);
        }


        [Fact]
        public void BuildEmlContent_HasClosingMimeBoundary()
        {
            // Act

            string result =
                InvokePrivate<string>(
                    "BuildEmlContent",
                    new List<string>
                    {
                        "to@example.com"
                    },
                    new List<string>(),
                    "Test",
                    "<p>Hello</p>");


            // Assert

            string boundary =
                GetBoundary(
                    result);

            Assert.EndsWith(
                $"--{boundary}--\r\n",
                result);
        }


        // =========================================================
        // HELPERS
        // =========================================================

        private static Staff CreateStaff(
            int id,
            string firstName,
            string lastName,
            string email)
        {
            return new Staff(
                id,
                firstName,
                "",
                lastName,
                Positions.Attendant,
                email,
                false,
                null,
                null);
        }


        private static T InvokePrivate<T>(
            string methodName,
            params object?[] arguments)
        {
            MethodInfo? method =
                typeof(EmlEmailService)
                    .GetMethod(
                        methodName,
                        BindingFlags.NonPublic |
                        BindingFlags.Static);

            Assert.NotNull(
                method);

            try
            {
                object? result =
                    method.Invoke(
                        null,
                        arguments);

                Assert.NotNull(
                    result);

                return (T)result;
            }
            catch (TargetInvocationException exception)
                when (exception.InnerException is not null)
            {
                /*
                 * Reflection normally wraps exceptions thrown by
                 * the tested method in TargetInvocationException.
                 * Re-throw the real exception so xUnit reports the
                 * production failure rather than the reflection
                 * wrapper.
                 */

                System.Runtime.ExceptionServices
                    .ExceptionDispatchInfo
                    .Capture(
                        exception.InnerException)
                    .Throw();

                throw;
            }
        }


        private static string GetBoundary(
            string eml)
        {
            const string marker =
                "boundary=\"";

            int start =
                eml.IndexOf(
                    marker,
                    StringComparison.Ordinal);

            Assert.True(
                start >= 0);

            start +=
                marker.Length;

            int end =
                eml.IndexOf(
                    '"',
                    start);

            Assert.True(
                end > start);

            return eml.Substring(
                start,
                end - start);
        }


        private static int CountOccurrences(
            string source,
            string value)
        {
            int count =
                0;

            int index =
                0;

            while (
                (index =
                    source.IndexOf(
                        value,
                        index,
                        StringComparison.Ordinal))
                >= 0)
            {
                count++;

                index +=
                    value.Length;
            }

            return count;
        }
    }
}