using System.IO;
using System.Net;
using Schedule_Creator_V2.Models.Interfaces;
using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Services.Email;

namespace Schedule_Creator_V2.Tests.Services.Email;

public sealed class EmailRenderingCoverageTests
{
    [Fact]
    public void ActiveRenderer_UnknownTheme_UsesMasterTemplateAndDefaultStyles()
    {
        var email = Email(new CustomBodyInputs("Hello", "Fallback body")) with
        {
            templateFileName = "unknown-theme.html"
        };
        string html = EmailContentService.BuildHtmlBody(email);
        Assert.Contains("Fallback body", html);
        Assert.DoesNotContain("{{EMAIL_STYLES}}", html);
        Assert.Equal("DefaultEmailStyles.css", EmailThemeService.GetStyleFile(email));
    }
    [Fact]
    public void ActiveRenderer_CustomFooter_EncodesTextAndNormalizesWebsite()
    {
        string html = EmailContentService.BuildHtmlBody(Email(
            new CustomFooterInputs("Club <Team>", "Visit & learn",
                "example.com/about?a=1&b=2", "First <line>\nSecond line", "")));
        Assert.Contains("Club &lt;Team&gt;", html);
        Assert.Contains("Visit &amp; learn", html);
        Assert.Contains("href=\"https://example.com/about?a=1&amp;b=2\"", html);
        Assert.Contains("First &lt;line&gt;<br>Second line", html);
    }
    private static EmailType Email(params IEmailInputs[] inputs) =>
        new("Test", inputs.ToList(), templateFileName: "Lets_Glow_Email_Template.html");

    private static string Render(string renderer, EmailType email) => renderer switch
    {
        "standard" => EmailContentService.BuildHtmlBody(email),
        "themed" => ThemedEmailContentService.BuildHtmlBody(email),
        "glow" => LetsGlowEmailContentService.BuildHtmlBody(email),
        _ => throw new ArgumentOutOfRangeException(nameof(renderer))
    };

    [Theory]
    [InlineData("standard")]
    [InlineData("themed")]
    [InlineData("glow")]
    public void Render_NullEmail_Throws(string renderer) =>
        Assert.Throws<ArgumentNullException>(() => Render(renderer, null!));

    [Theory]
    [InlineData("standard")]
    [InlineData("themed")]
    [InlineData("glow")]
    public void Render_MetadataAndBody_EncodePlainText(string renderer)
    {
        string result = Render(renderer, Email(
            new EmailDetailsInputs("Review <draft> & approve"),
            new CustomBodyInputs("Hello <team>", "Climb safely & have fun")));
        Assert.Contains("Review &lt;draft&gt; &amp; approve", result);
        Assert.Contains("Hello &lt;team&gt;", result);
        Assert.Contains("Climb safely &amp; have fun", result);
        Assert.DoesNotContain("<team>", result);
    }

    [Theory]
    [InlineData("standard")]
    [InlineData("themed")]
    [InlineData("glow")]
    public void Render_Header_EncodesTextAndPreservesImageSource(string renderer)
    {
        string result = Render(renderer, Email(new CustomHeaderInputs(
            "Club & Team", "Notice <one>", "Event <two>", "Subtitle",
            "https://example.com/header.png")));
        Assert.Contains("Club &amp; Team", result);
        Assert.Contains("Notice &lt;one&gt;", result);
        Assert.Contains("Event &lt;two&gt;", result);
        Assert.Contains("https://example.com/header.png", result);
    }

    [Theory]
    [InlineData("standard")]
    [InlineData("themed")]
    [InlineData("glow")]
    public void Render_RichText_PreservesFormattingAndRemovesEventAttributes(string renderer)
    {
        string result = Render(renderer, Email(new CustomBodyInputs("Hello",
            "[[RICH_TEXT_HTML]]<p onclick=\"bad()\"><strong>Bold</strong> &amp; <em>italic</em></p>")));
        Assert.Contains("<strong>Bold</strong>", result);
        Assert.Contains("<em>italic</em>", result);
        Assert.DoesNotContain("onclick", result);
        Assert.DoesNotContain("[[RICH_TEXT_HTML]]", result);
    }

    [Theory]
    [InlineData("standard")]
    [InlineData("themed")]
    [InlineData("glow")]
    public void Render_MalformedRichText_EncodesInsteadOfEmittingBrokenMarkup(string renderer)
    {
        string result = Render(renderer, Email(new CustomBodyInputs("Hello",
            "[[RICH_TEXT_HTML]]<strong>Unclosed")));
        Assert.Contains("&lt;strong&gt;Unclosed", result);
        Assert.DoesNotContain("[[RICH_TEXT_HTML]]", result);
    }

    [Theory]
    [InlineData("standard", "https://example.com/photo.png")]
    [InlineData("themed", "https://example.com/photo.png")]
    [InlineData("glow", "https://example.com/photo.png")]
    [InlineData("standard", "example.com/photo.png")]
    [InlineData("themed", "example.com/photo.png")]
    [InlineData("glow", "example.com/photo.png")]
    public void Render_WebImage_NormalizesUrlAndEncodesAltText(string renderer, string source)
    {
        string result = Render(renderer, Email(new CustomImageInputs(source, "A < B & C")));
        Assert.Contains("https://example.com/photo.png", result);
        Assert.Contains("A &lt; B &amp; C", result);
    }

    [Theory]
    [InlineData("standard")]
    [InlineData("themed")]
    [InlineData("glow")]
    public void Render_DataImage_PreservesEmbeddedBytes(string renderer)
    {
        const string source = "data:image/png;base64,AQID";
        Assert.Contains(source, Render(renderer, Email(new CustomImageInputs(source, "Picture"))));
    }

    [Theory]
    [InlineData("standard", "javascript:alert(1)")]
    [InlineData("themed", "javascript:alert(1)")]
    [InlineData("glow", "javascript:alert(1)")]
    [InlineData("standard", "ftp://example.com/photo.png")]
    [InlineData("themed", "ftp://example.com/photo.png")]
    [InlineData("glow", "ftp://example.com/photo.png")]
    public void Render_UnsupportedImageScheme_Throws(string renderer, string source) =>
        Assert.Throws<InvalidOperationException>(() =>
            Render(renderer, Email(new CustomImageInputs(source, "Picture"))));

    [Theory]
    [InlineData("standard")]
    [InlineData("themed")]
    [InlineData("glow")]
    public void Render_LocalImage_EmbedsBytesWithoutChangingSourceFile(string renderer)
    {
        string path = Path.Combine(Path.GetTempPath(), $"email-test-{Guid.NewGuid():N}.png");
        byte[] bytes = Convert.FromBase64String(
            "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+jRZkAAAAASUVORK5CYII=");
        try
        {
            File.WriteAllBytes(path, bytes);
            Assert.Contains("data:image/png;base64," + Convert.ToBase64String(bytes),
                Render(renderer, Email(new CustomImageInputs(path, "Pixel"))));
            Assert.Equal(bytes, File.ReadAllBytes(path));
        }
        finally { File.Delete(path); }
    }

    [Theory]
    [InlineData("standard")]
    [InlineData("themed")]
    [InlineData("glow")]
    public void Render_AnnouncementsAndRequest_RendersUserValues(string renderer)
    {
        string result = Render(renderer, Email(
            new CustomAnnouncementsInputs("News & updates", "Read carefully", new() { "Bring shoes", "Arrive early" }),
            new CustomRequestInputs("Action", "Please respond", "Choose your shift",
                "Respond now", "https://example.com/respond")));
        foreach (string value in new[] { "News &amp; updates", "Read carefully", "Bring shoes",
            "Arrive early", "Please respond", "Choose your shift", "Respond now", "https://example.com/respond" })
            Assert.Contains(value, result);
    }

    [Theory]
    [InlineData("standard")]
    [InlineData("themed")]
    [InlineData("glow")]
    public void Render_SignatureAndFooter_PreserveContactDetails(string renderer)
    {
        string result = Render(renderer, Email(
            new CustomSignatureInputs("Thanks", "Alex & Sam", "Coordinator", "Climbing Club",
                "alex@example.com", "555-0100"),
            new CustomFooterInputs("Campus Recreation", "Visit website",
                "https://example.com", "See you soon", "")));
        foreach (string value in new[] { "Alex &amp; Sam", "Coordinator", "Climbing Club",
            "alex@example.com", "555-0100", "Campus Recreation", "Visit website", "See you soon" })
            Assert.Contains(value, result);
    }

    [Theory]
    [InlineData("themed")]
    public void Render_MissingTemplate_ReportsFileName(string renderer)
    {
        string name = $"missing-{Guid.NewGuid():N}.html";
        var error = Assert.Throws<FileNotFoundException>(() =>
            Render(renderer, Email() with { templateFileName = name }));
        Assert.EndsWith(name, error.FileName);
    }

    [Theory]
    [InlineData("standard")]
    [InlineData("themed")]
    [InlineData("glow")]
    public void Render_NoOptionalSections_RemovesTheirContainers(string renderer)
    {
        string result = Render(renderer, Email(new CustomBodyInputs("Hello", "Body")));
        foreach (string id in new[] { "imageSection", "announcementsSection", "requestSection",
            "attachmentsSection", "signatureSection" })
            Assert.DoesNotContain($"id=\"{id}\"", result);
        Assert.Contains("Body", result);
    }

    [Theory]
    [InlineData("standard")]
    [InlineData("themed")]
    public void Render_Attachments_EncodesNames(string renderer)
    {
        string result = Render(renderer, Email(new CustomAttachmentsInputs(
            "Documents", "Please read", new() { "Rules & policies.pdf" })));
        Assert.Contains("Rules &amp; policies.pdf", result);
    }
}