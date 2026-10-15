using System.IO;
using System.Net;
using Schedule_Creator_V2.Models.Interfaces;
using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Services.Email;

namespace Schedule_Creator_V2.Tests.Services.Email;

/// <summary>
/// Exercises the public renderer using isolated templates and the packaged themes.
/// Each test instance owns its temporary files; shared application assets are never edited.
/// </summary>
public sealed class ThemedEmailContentServiceTests : IDisposable
{
    private readonly string directory = Path.Combine(Path.GetTempPath(), "themed-email-" + Guid.NewGuid().ToString("N"));
    private const string Template = """
        <html><head><title id='emailTitle'>old title</title></head><body>
        <span id='preheaderText'>old preview</span>
        <header>
        <span id='organizationLabel'>old organization</span>
        <span id='headerLabel'>old label</span>
        <h1 id='emailHeading'>old heading</h1>
        <div id='headerSubtitle'>old subtitle</div>
        <img id='headerImage' src='{{HEADER_IMAGE_SOURCE}}'>
        </header>
        <h2 id='bodyHeading'>old greeting</h2><div id='bodyContent'>old body</div>
        <section id='imageSection'><img id='contentImage' src='{{CONTENT_IMAGE_SOURCE}}' alt='Original alt'></section>
        <section id='imageDividerSection'>image divider</section>
        <section id='announcementsSection'><h2 id='announcementsHeading'>old news</h2><div id='announcementsContent'>old news body</div></section>
        <section id='requestSection'><h2 id='requestHeading'>old request</h2><div id='requestContent'>old request body</div>
        <nav id='requestButtonSection'><a id='requestButton' href='old'>old button</a></nav></section>
        <section id='attachmentsSection'><h2 id='attachmentsHeading'>old documents</h2><div id='attachmentsContent'>old documents body</div></section>
        <section id='signatureSection'><div id='signatureContent'>old signature</div></section>
        <footer><span id='footerOrganization'>default organization</span><div id='footerText'>default footer</div>
        <a id='footerWebsiteLink' href='original'>default site</a><img src='{{FOOTER_LOGO_SOURCE}}'></footer>
        </body></html>
        """;

    public ThemedEmailContentServiceTests() => Directory.CreateDirectory(directory);

    private string Render(string template, params IEmailInputs[] inputs)
    {
        string path = Path.Combine(directory, "template.html");
        File.WriteAllText(path, template);
        return ThemedEmailContentService.BuildHtmlBody(new("Test", inputs.ToList(), templateFileName: path));
    }

    private string Render(params IEmailInputs[] inputs) => Render(Template, inputs);

    public void Dispose()
    {
        // Only files created by this test instance, with no recursive directory deletion.
        foreach (string path in Directory.EnumerateFiles(directory))
            File.Delete(path);
        Directory.Delete(directory);
    }

    [Fact]
    public void NullEmail_ReportsParameterName() =>
        Assert.Equal("emailType", Assert.Throws<ArgumentNullException>(() =>
            ThemedEmailContentService.BuildHtmlBody(null!)).ParamName);

    [Fact]
    public void MissingTemplate_ReportsExactPath()
    {
        string path = Path.Combine(directory, "missing.html");
        var error = Assert.Throws<FileNotFoundException>(() =>
            ThemedEmailContentService.BuildHtmlBody(new("Test", new(), templateFileName: path)));
        Assert.Equal(path, error.FileName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void BlankTemplateName_FallsBackToMasterTemplate(string? template)
    {
        string html = ThemedEmailContentService.BuildHtmlBody(new("Test",
            new() { new EmailDetailsInputs("Fallback subject") }, templateFileName: template!));
        Assert.Contains("Fallback subject", html);
        Assert.Contains("<html", html, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("Lets_Glow_Email_Template.html")]
    [InlineData("Tower_of_Power_Email_Template.html")]
    public void PackagedTheme_RendersCustomBodyAndRemovesUnusedSections(string template)
    {
        string html = ThemedEmailContentService.BuildHtmlBody(new("Test",
            new() { new EmailDetailsInputs("Theme test"), new CustomBodyInputs("Hello team", "Custom body") },
            templateFileName: template));
        Assert.Contains("Theme test", html);
        Assert.Contains("Hello team", html);
        Assert.Contains("Custom body", html);
        Assert.DoesNotContain("id=\"requestSection\"", html);
        Assert.DoesNotContain("{{CONTENT_IMAGE_SOURCE}}", html);
    }

    [Fact]
    public void Subject_IsTrimmedEncodedAndUsedForTitleAndPreview()
    {
        string html = Render(new EmailDetailsInputs("  <Draft> & review  "));
        Assert.Contains("<title id='emailTitle'>&lt;Draft&gt; &amp; review</title>", html);
        Assert.Contains("<span id='preheaderText'>&lt;Draft&gt; &amp; review</span>", html);
    }

    [Fact]
    public void EmptyEmail_ClearsMetadataAndImageTokensAndPreservesDefaultFooter()
    {
        string html = Render();
        Assert.Contains("<title id='emailTitle'></title>", html);
        Assert.Contains("default organization", html);
        Assert.Contains("default footer", html);
        Assert.DoesNotContain("{{", html);
        foreach (string id in new[] { "imageSection", "imageDividerSection", "announcementsSection",
            "requestSection", "attachmentsSection", "signatureSection" })
            Assert.DoesNotContain($"id='{id}'", html);
    }

    [Fact]
    public void Header_EncodesFieldsAndRendersRichSubtitle()
    {
        string html = Render(new CustomHeaderInputs("Org & team", "<Label>", "<Heading>",
            "<strong>Subtitle</strong>", "https://example.com/header.png?a=1&b=2"));
        Assert.Contains("Org &amp; team", html);
        Assert.Contains("&lt;Label&gt;", html);
        Assert.Contains("&lt;Heading&gt;", html);
        Assert.Contains("<strong>Subtitle</strong>", html);
        Assert.Contains("src=\"https://example.com/header.png?a=1&amp;b=2\"", html);
    }

    [Fact]
    public void BlankHeaderImage_DoesNotResolveAnImage()
    {
        var header = new CustomHeaderInputs("Org", "Label", "Heading", "") { HeaderImageUrl = " " };
        Assert.Contains("id='headerImage' src=''", Render(header));
    }

    [Theory]
    [InlineData("first\r\nsecond")]
    [InlineData("first\nsecond")]
    [InlineData("first\rsecond")]
    public void PlainText_NormalizesLineEndings(string value)
    {
        string html = Render(new CustomBodyInputs(value, value));
        Assert.Contains("<h2 id='bodyHeading'>first<br>second</h2>", html);
        Assert.Contains("<div id='bodyContent'>first<br>second</div>", html);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" \r\n ")]
    public void BlankBodyAndGreeting_RenderEmptyElements(string? value)
    {
        string html = Render(new CustomBodyInputs(value!, value!));
        Assert.Contains("<h2 id='bodyHeading'></h2>", html);
        Assert.Contains("<div id='bodyContent'></div>", html);
    }

    [Theory]
    [InlineData("<p>Text</p>", "<p>Text</p>")]
    [InlineData("<b>Text</b>", "<strong>Text</strong>")]
    [InlineData("<i>Text</i>", "<em>Text</em>")]
    [InlineData("<u>Text</u>", "<u>Text</u>")]
    [InlineData("<strong>Text</strong>", "<strong>Text</strong>")]
    [InlineData("<em>Text</em>", "<em>Text</em>")]
    [InlineData("<ul><li>Item</li></ul>", "<ul><li>Item</li></ul>")]
    [InlineData("<ol><li>Item</li></ol>", "<ol><li>Item</li></ol>")]
    [InlineData("<p>A<br />B</p>", "<p>A<br />B</p>")]
    [InlineData("<p><span style='color:red'>Text</span></p>", "<p>Text</p>")]
    [InlineData("<p><!-- comment -->Text</p>", "<p>Text</p>")]
    [InlineData("<p onclick='bad()'>Text</p>", "<p>Text</p>")]
    public void RichText_UsesAllowedMarkupAndDiscardsAttributes(string input, string expected)
    {
        string html = Render(new CustomBodyInputs("", "[[RICH_TEXT_HTML]]" + input));
        Assert.Contains($"<div id='bodyContent'>{expected}</div>", html);
        Assert.DoesNotContain("[[RICH_TEXT_HTML]]", html);
    }

    [Fact]
    public void RichText_WithoutPrefix_IsRecognized()
    {
        Assert.Contains("<div id='bodyContent'><strong>Bold</strong></div>",
            Render(new CustomBodyInputs("", "<b>Bold</b>")));
    }

    [Fact]
    public void RichText_NonbreakingSpace_IsPreserved()
    {
        string html = Render(new CustomBodyInputs("", "<p>A&nbsp;B</p>"));
        Assert.Contains("A\u00a0B", WebUtility.HtmlDecode(html));
    }

    [Fact]
    public void MalformedRichText_IsEncoded()
    {
        Assert.Contains("&lt;strong&gt;Unclosed",
            Render(new CustomBodyInputs("", "[[RICH_TEXT_HTML]]<strong>Unclosed")));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void BlankImage_RemovesImageAndDivider(string? source)
    {
        string html = Render(new CustomImageInputs(source!, ""));
        Assert.DoesNotContain("id='imageSection'", html);
        Assert.DoesNotContain("id='imageDividerSection'", html);
    }

    [Theory]
    [InlineData("https://example.com/p.png", "https://example.com/p.png")]
    [InlineData("http://example.com/p.png", "http://example.com/p.png")]
    [InlineData("example.com/p.png", "https://example.com/p.png")]
    [InlineData(" data:image/png;base64,AQID ", "data:image/png;base64,AQID")]
    public void Image_ResolvesSupportedSourcesAndEncodesAlt(string source, string expected)
    {
        string html = Render(new CustomImageInputs(source, "A \"quote\" & text"));
        Assert.Contains($"src=\"{expected}\"", html);
        Assert.Contains("alt=\"A &quot;quote&quot; &amp; text\"", html);
        Assert.Contains("id='imageDividerSection'", html);
    }

    [Fact]
    public void BlankAltText_PreservesTemplateAlt()
    {
        Assert.Contains("alt='Original alt'", Render(new CustomImageInputs("https://example.com/a.png", " ")));
    }

    [Theory]
    [InlineData(".jpg", "image/jpeg")]
    [InlineData(".jpeg", "image/jpeg")]
    [InlineData(".PNG", "image/png")]
    [InlineData(".gif", "image/gif")]
    [InlineData(".bmp", "image/bmp")]
    public void LocalImage_UsesExpectedMimeAndExactFileBytes(string extension, string mime)
    {
        string path = Path.Combine(directory, "image" + extension);
        byte[] bytes = { 1, 2, 3, 4 }; // This service embeds bytes; it does not decode image pixels.
        File.WriteAllBytes(path, bytes);
        string html = Render(new CustomImageInputs(path, "Image"));
        Assert.Contains($"data:{mime};base64,{Convert.ToBase64String(bytes)}", html);
        Assert.Equal(bytes, File.ReadAllBytes(path));
    }

    [Fact]
    public void UnsupportedLocalImage_Throws()
    {
        string path = Path.Combine(directory, "image.svg");
        File.WriteAllText(path, "<svg/>");
        Assert.Throws<InvalidOperationException>(() => Render(new CustomImageInputs(path, "Image")));
    }

    [Theory]
    [InlineData("ftp://example.com/image.png")]
    [InlineData("javascript:alert(1)")]
    [InlineData("file:///missing/image.png")]
    [InlineData("http://")]
    public void InvalidImageSource_Throws(string source) =>
        Assert.Throws<InvalidOperationException>(() => Render(new CustomImageInputs(source, "Image")));

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Lists_TrimFilterAndEncodeEntries(bool announcements)
    {
        IEmailInputs input = announcements
            ? new CustomAnnouncementsInputs("", "Intro <one>", new() { " ", null!, "  <item> & text  " })
            : new CustomAttachmentsInputs("", "Intro <one>", new() { " ", null!, @"C:\files\<item> & text" });
        string html = Render(input);
        Assert.Contains(announcements ? ">Announcements</h2>" : ">Attachments</h2>", html);
        Assert.Contains("Intro &lt;one&gt;", html);
        Assert.Contains("&lt;item&gt; &amp; text", html);
        Assert.DoesNotContain(@"C:\files", html);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void EmptyOrNullLists_RenderIntroWithoutListMarkup(bool announcements, bool nullList)
    {
        List<string> list = nullList ? null! : new();
        IEmailInputs input = announcements
            ? new CustomAnnouncementsInputs("News", "<b>Intro</b>", list)
            : new CustomAttachmentsInputs("Files", "<b>Intro</b>", list);
        string html = Render(input);
        Assert.Contains("<strong>Intro</strong>", html);
        Assert.DoesNotContain("<ul", html);
        Assert.DoesNotContain("&#128206;", html);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("Reply", "")]
    [InlineData("", "https://example.com")]
    public void IncompleteRequestButton_RemovesButtonOnly(string label, string link)
    {
        string html = Render(new CustomRequestInputs("Fallback", "", "Request body", label, link));
        Assert.Contains("<h2 id='requestHeading'>Fallback</h2>", html);
        Assert.Contains("Request body", html);
        Assert.DoesNotContain("id='requestButtonSection'", html);
        Assert.Contains("id='requestSection'", html);
    }

    [Fact]
    public void CompleteRequest_UsesTitleAndEncodesLink()
    {
        string html = Render(new CustomRequestInputs("Fallback", "Title <one>", "<b>Respond</b>",
            "Click & go", "https://example.com/?a=1&b=2"));
        Assert.Contains("Title &lt;one&gt;", html);
        Assert.Contains("<strong>Respond</strong>", html);
        Assert.Contains("Click &amp; go", html);
        Assert.Contains("href=\"https://example.com/?a=1&amp;b=2\"", html);
    }

    [Fact]
    public void EmptySignature_DoesNotLeaveSampleTextOrEmptyContactBlock()
    {
        string html = Render(new CustomSignatureInputs("", "", "", "", "", ""));
        Assert.Contains("<div id='signatureContent'></div>", html);
        Assert.DoesNotContain("mailto:", html);
    }

    [Fact]
    public void Signature_AllContactFieldsAreEncoded()
    {
        string html = Render(new CustomSignatureInputs("<b>Thanks</b>", "A & B", "<Lead>", "<Club>",
            "a@example.com", "<555>"));
        foreach (string expected in new[] { "<strong>Thanks</strong>", "<strong>A &amp; B</strong>",
            "&lt;Lead&gt;", "&lt;Club&gt;", "mailto:a@example.com", "&lt;555&gt;" })
            Assert.Contains(expected, html);
    }

    [Fact]
    public void Footer_UsesEncodedCustomFieldsAndLineBreaks()
    {
        string html = Render(new CustomFooterInputs("<Club>", "Site & more",
            "https://example.com/?x=1&y=2", "First\nSecond", ""));
        Assert.Contains("&lt;Club&gt;", html);
        Assert.Contains("Site &amp; more", html);
        Assert.Contains("href=\"https://example.com/?x=1&amp;y=2\"", html);
        Assert.Contains("First<br>Second", html);
    }

    [Fact]
    public void MissingImageAttributes_AreInserted()
    {
        string template = Template.Replace("src='{{CONTENT_IMAGE_SOURCE}}' alt='Original alt'", "");
        string html = Render(template, new CustomImageInputs("https://example.com/p.png", "New alt"));
        Assert.Contains("src=\"https://example.com/p.png\"", html);
        Assert.Contains("alt=\"New alt\"", html);
    }

    [Fact]
    public void MissingOptionalElements_DoNotPreventOtherContentFromRendering()
    {
        string html = Render("<title id='emailTitle'></title><div id='bodyContent'></div>",
            new EmailDetailsInputs("Subject"), new CustomBodyInputs("Greeting", "Message"),
            new CustomImageInputs("https://example.com/p.png", "Image"));
        Assert.Equal("<title id='emailTitle'>Subject</title><div id='bodyContent'>Message</div>", html);
    }

    [Fact]
    public void Rendering_DoesNotModifyInputsOrTemplateFile()
    {
        var body = new CustomBodyInputs("Hi", "Message");
        string html = Render(body);
        Assert.Equal("Hi", body.RecipientGreeting);
        Assert.Equal("Message", body.EmailBody);
        Assert.Equal(Template, File.ReadAllText(Path.Combine(directory, "template.html")));
        Assert.Contains("Message", html);
    }
}