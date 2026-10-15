using System.IO;
using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Services.Email;

namespace Schedule_Creator_V2.Tests.Services.Email;

public sealed class EmailThemeServiceTests
{
    private static EmailType Email(string? template = "Email.html") =>
        new("Test", new(), templateFileName: template!);

    [Theory]
    [InlineData("Email.html", "DefaultEmailStyles.css", "light")]
    [InlineData(null, "DefaultEmailStyles.css", "light")]
    [InlineData("", "DefaultEmailStyles.css", "light")]
    [InlineData("unknown.html", "DefaultEmailStyles.css", "light")]
    [InlineData("Lets_Glow_Email_Template.html", "LetsGlowStyles.css", "dark")]
    [InlineData("LETS_GLOW_EMAIL_TEMPLATE.HTML", "LetsGlowStyles.css", "dark")]
    [InlineData("Tower_of_Power_Email_Template.html", "TowerOfPowerStyles.css", "light")]
    [InlineData("TOWER_OF_POWER_EMAIL_TEMPLATE.HTML", "TowerOfPowerStyles.css", "light")]
    public void Template_SelectsExpectedStylesAndColorScheme(string? template, string css, string scheme)
    {
        Assert.Equal(css, EmailThemeService.GetStyleFile(Email(template)));
        Assert.Equal(scheme, EmailThemeService.GetColorScheme(Email(template)));
    }

    [Fact]
    public void NullArguments_AreRejected()
    {
        Assert.Equal("emailType", Assert.Throws<ArgumentNullException>(
            () => EmailThemeService.GetStyleFile(null!)).ParamName);
        Assert.Throws<ArgumentNullException>(() => EmailThemeService.GetColorScheme(null!));
        Assert.Equal("html", Assert.Throws<ArgumentNullException>(
            () => EmailThemeService.ApplyTheme(null!, Email())).ParamName);
        Assert.Equal("emailType", Assert.Throws<ArgumentNullException>(
            () => EmailThemeService.ApplyTheme("", null!)).ParamName);
    }

    [Theory]
    [InlineData("Email.html", "DefaultEmailStyles.css", "light")]
    [InlineData("Lets_Glow_Email_Template.html", "LetsGlowStyles.css", "dark")]
    [InlineData("Tower_of_Power_Email_Template.html", "TowerOfPowerStyles.css", "light")]
    public void ApplyTheme_InsertsPackagedStylesAndReplacesAllMarkers(string template, string css, string scheme)
    {
        string styles = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
            "Models", "Objects", "Emails", "Styles", css));
        const string html = "<style>{{EMAIL_STYLES}}</style><meta content='{{COLOR_SCHEME}}'>" +
                            "<p>Unchanged content</p><style>{{EMAIL_STYLES}}</style>";
        string result = EmailThemeService.ApplyTheme(html, Email(template));
        Assert.Equal($"<style>{styles}</style><meta content='{scheme}'>" +
                     $"<p>Unchanged content</p><style>{styles}</style>", result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("<html><body>No styles marker</body></html>")]
    [InlineData("{{email_styles}}")]
    public void ApplyTheme_MissingExactStyleMarker_ThrowsHelpfulError(string html)
    {
        var error = Assert.Throws<InvalidOperationException>(() =>
            EmailThemeService.ApplyTheme(html, Email()));
        Assert.Contains("{{EMAIL_STYLES}}", error.Message);
    }

    [Theory]
    [InlineData("Lets_Glow_Email_Template.html", "#FFFFFF", "#11102b")]
    [InlineData("Lets_Glow_Email_Template.html", "#111111", "#ffffff")]
    [InlineData("Lets_Glow_Email_Template.html", "#0F5640", "#36e9ef")]
    [InlineData("Tower_of_Power_Email_Template.html", "#f5f7f6", "#ffffff")]
    [InlineData("Tower_of_Power_Email_Template.html", "#ffffff", "#fff9f4")]
    [InlineData("Tower_of_Power_Email_Template.html", "#0f5640", "#f15a24")]
    [InlineData("Email.html", "#FFFFFF", "#FFFFFF")]
    public void ApplyTheme_RecolorsHtmlWithoutCascadingReplacements(string template, string original, string expected)
    {
        string result = EmailThemeService.ApplyTheme(
            $"<div style='color:{original}'>Text</div>{{{{EMAIL_STYLES}}}}", Email(template));
        Assert.StartsWith($"<div style='color:{expected}'>Text</div>", result);
    }

    [Theory]
    [InlineData("Lets_Glow_Email_Template.html")]
    [InlineData("Tower_of_Power_Email_Template.html")]
    public void ApplyTheme_PreservesUnknownColorsAndLongerHexTokens(string template)
    {
        const string content = "<p>#123abc #ffffff00 #fff ordinary text</p>";
        Assert.StartsWith(content, EmailThemeService.ApplyTheme(
            content + "{{EMAIL_STYLES}}", Email(template)));
    }
}