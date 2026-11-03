using System.IO;
using System.Security.Cryptography;
using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Services.Email;

namespace Schedule_Creator_V2.Tests.Services.Email;

public sealed class LetsGlowDesignTests
{
    [Fact]
    public void Glow_SeparatesTextAndBackgroundColorsWithoutChangingUserContent()
    {
        string html = EmailThemeService.ApplyTheme(
            "<head>{{EMAIL_STYLES}}</head><div style=\"color:#ffffff;background-color:#ffffff;\">" +
            "Keep #ffffff in this message <a href=\"https://example.com/#ffffff\">Link</a></div>",
            new EmailType("Glow", new(), templateFileName: "Lets_Glow_Email_Template.html"));
        Assert.Contains("color:#f7f8ff", html);
        Assert.Contains("background-color:#11102b", html);
        Assert.Contains("Keep #ffffff in this message", html);
        Assert.Contains("href=\"https://example.com/#ffffff\"", html);
    }

    [Theory]
    [InlineData("Let's Glow Schedule")]
    [InlineData("Let's Glow Announcements")]
    [InlineData("Let's Glow Requests")]
    public void GlowPresets_IncludeOutlookFallbacksAndDarkModeRules(string name)
    {
        var email = EmailTypeService.CreateEmailTypes().Single(x => x.displayName == name);
        string html = EmailContentService.BuildHtmlBody(email);
        Assert.Contains("[if gte mso 16]", html);
        Assert.Contains("mso-style-textfill-type:gradient", html);
        Assert.Contains("[data-ogsc]", html);
        Assert.Contains("[data-ogsb]", html);
        Assert.Contains("prefers-color-scheme:dark", html);
        Assert.Contains("bgcolor=\"#11102b\"", html);
        Assert.DoesNotContain("id=\"headerImageSection\"", html);
        Assert.Contains("LET&#39;S GLOW!", html);
    }

    [Fact]
    public void Glow_CustomHeaderPhoto_IsPreserved()
    {
        var email = EmailTypeService.CreateEmailTypes().Single(x => x.displayName == "Let's Glow Requests");
        email.inputs.OfType<CustomHeaderInputs>().Single().HeaderImageUrl = "https://example.com/night.png";
        string html = EmailContentService.BuildHtmlBody(email);
        Assert.Contains("id=\"headerImageSection\"", html);
        Assert.Contains("https://example.com/night.png", html);
    }

    [Fact]
    public void Glow_RequestButton_HasDarkTextOnLime()
    {
        var email = EmailTypeService.CreateEmailTypes().Single(x => x.displayName == "Let's Glow Requests");
        email.inputs.RemoveAll(x => x is CustomRequestInputs);
        email.inputs.Add(new CustomRequestInputs("Action", "Respond", "Choose a shift", "Reply", "https://example.com"));
        string html = EmailContentService.BuildHtmlBody(email);
        Assert.Contains("bgcolor=\"#9be447\"", html);
        string button = System.Text.RegularExpressions.Regex.Match(html, "<a[^>]*id=\"requestButton\"[^>]*>").Value;
        Assert.Contains("color:#11102b", button);
        Assert.Contains("glow-button", button);
    }

    [Theory]
    [InlineData("Email.html")]
    [InlineData("Tower_of_Power_Email_Template.html")]
    public void OtherThemes_DoNotReceiveGlowMarkup(string template)
    {
        string html = EmailThemeService.ApplyTheme(
            "<head>{{EMAIL_STYLES}}</head><p>Other theme</p>",
            new EmailType("Other", new(), templateFileName: template));
        Assert.DoesNotContain("glow-text", html);
        Assert.DoesNotContain("mso-style-textfill", html);
        Assert.DoesNotContain("#f34fc1", html);
    }
    [Fact]
    public void ExportPresetPreviews_WhenRequested()
    {
        string? directory = Environment.GetEnvironmentVariable("SCHEDULE_EMAIL_PREVIEW_DIR");
        if (string.IsNullOrWhiteSpace(directory)) return;
        Directory.CreateDirectory(directory);
        foreach (var email in EmailTypeService.CreateEmailTypes())
        {
            string name = string.Concat(email.displayName.Select(c => char.IsLetterOrDigit(c) ? c : '_'));
            string html = EmailContentService.BuildHtmlBody(email);
            File.WriteAllText(Path.Combine(directory, name + ".html"), html);
            File.WriteAllText(Path.Combine(directory, name + ".sha256"),
                Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(html))));
        }
        var demo = EmailTypeService.CreateEmailTypes().Single(x => x.displayName == "Let's Glow Requests");
        demo.inputs.RemoveAll(x => x is CustomBodyInputs or CustomRequestInputs);
        demo.inputs.Add(new CustomBodyInputs("Hey Tower Team,",
            "Blacklights on. Energy up.\n\nHelp us make the next glow climb a night to remember. Check the details below and let us know where you can lend a hand."));
        demo.inputs.Add(new CustomAnnouncementsInputs("BEFORE YOU CLIMB", "A few things to keep the night running smoothly.",
            new() { "Wear something bright or neon.", "Bring your climbing shoes and a water bottle.", "Check in with the team before your shift." }));
        demo.inputs.Add(new CustomRequestInputs("YOUR NEXT MOVE", "Ready to light up the tower?",
            "Choose your preferred role and send us your availability.", "Count me in", "https://example.com/glow"));
        File.WriteAllText(Path.Combine(directory, "Lets_Glow_Demo.html"), EmailContentService.BuildHtmlBody(demo));
    }
}