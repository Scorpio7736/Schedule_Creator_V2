using System.Net;
using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Services.Email;

namespace Schedule_Creator_V2.Tests.Services.Email;

public sealed class EmailPresetCoverageTests
{
    public static IEnumerable<object[]> Presets() =>
        EmailTypeService.CreateEmailTypes().Select(email => new object[] { email.displayName });

    [Theory]
    [MemberData(nameof(Presets))]
    public void EveryPreset_RendersThroughActivePipeline(string name)
    {
        var email = EmailTypeService.CreateEmailTypes().Single(x => x.displayName == name);
        string html = EmailContentService.BuildHtmlBody(email);
        Assert.Contains("<html", html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("{{", html);
        Assert.DoesNotContain("[[RICH_TEXT_HTML]]", html);
        Assert.Contains(WebUtility.HtmlEncode(EmailContentService.BuildSubject(email)), html);
    }

    [Theory]
    [InlineData("Lets_Glow_Email_Template.html", true)]
    [InlineData("LETS_GLOW_EMAIL_TEMPLATE.HTML", true)]
    [InlineData("Email.html", false)]
    [InlineData("Tower_of_Power_Email_Template.html", false)]
    [InlineData(null, false)]
    [InlineData("", false)]
    public void UsesLetsGlowTemplate_RecognizesOnlyGlowTemplate(string? template, bool expected)
    {
        Assert.Equal(expected, LetsGlowEmailContentService.UsesLetsGlowTemplate(
            new EmailType("Test", new(), templateFileName: template!)));
    }

    [Fact]
    public void UsesLetsGlowTemplate_NullEmail_Throws() =>
        Assert.Throws<ArgumentNullException>(() => LetsGlowEmailContentService.UsesLetsGlowTemplate(null!));

    [Fact]
    public void CreateEmailTypes_ReturnsIndependentEditableInputs()
    {
        var first = EmailTypeService.CreateEmailTypes();
        var second = EmailTypeService.CreateEmailTypes();
        first[0].inputs.OfType<EmailDetailsInputs>().Single().Subject = "Changed subject";
        first[0].inputs.Clear();
        Assert.NotEmpty(second[0].inputs);
        Assert.DoesNotContain(second.SelectMany(x => x.inputs).OfType<EmailDetailsInputs>(),
            x => x.Subject == "Changed subject");
    }
}