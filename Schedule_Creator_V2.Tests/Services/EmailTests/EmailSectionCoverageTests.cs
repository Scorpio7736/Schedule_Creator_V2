using Schedule_Creator_V2.Models.Interfaces;
using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Services.Email;

namespace Schedule_Creator_V2.Tests.Services.Email;

public sealed class EmailSectionCoverageTests
{
    private static EmailType Editable() => new("Custom", new(), allowSectionEditing: true);

    [Fact]
    public void OptionalSections_CanBeAddedListedAndRemovedWithoutLosingRequiredSections()
    {
        var email = Editable();
        EmailSectionService.EnsureRequiredSections(email);
        var required = email.inputs.ToArray();
        var options = EmailSectionService.GetAvailableSections(email).ToArray();

        Assert.Equal(4, options.Length);
        foreach (var option in options)
            Assert.True(EmailSectionService.AddSection(email, option));
        Assert.Empty(EmailSectionService.GetAvailableSections(email));

        var removable = EmailSectionService.GetRemovableSections(email);
        Assert.Equal(4, removable.Count);
        foreach (var section in removable)
        {
            Assert.Contains(section.Input, email.inputs);
            Assert.False(string.IsNullOrWhiteSpace(section.DisplayName));
            Assert.True(EmailSectionService.RemoveSection(email, section.Input));
        }
        Assert.Equal(required, email.inputs);
        Assert.Equal(4, EmailSectionService.GetAvailableSections(email).Count);
    }

    [Fact]
    public void RemoveSection_NotPresent_ReturnsFalseAndPreservesInputs()
    {
        var email = Editable();
        EmailSectionService.EnsureRequiredSections(email);
        var before = email.inputs.ToArray();
        Assert.False(EmailSectionService.RemoveSection(email, new CustomImageInputs("x", "y")));
        Assert.Equal(before, email.inputs);
    }

    private sealed class AdditionalInput : IEmailInputs
    {
        public string GetEmailTypeName() => "Additional section";
    }

    [Fact]
    public void UnknownSection_RetainsNameAndSortsAfterKnownSections()
    {
        var extra = new AdditionalInput();
        var email = Editable();
        email.inputs.Add(extra);
        EmailSectionService.EnsureRequiredSections(email);
        Assert.Equal("Additional section", EmailSectionService.GetDisplayName(extra));
        Assert.Same(extra, email.inputs.Last());
        Assert.False(EmailSectionService.CanRemove(extra));
    }

    [Fact]
    public void PublicSectionOperations_RejectNullArguments()
    {
        var email = Editable();
        var option = EmailSectionService.GetAvailableSections(email).First();
        Assert.Throws<ArgumentNullException>(() => EmailSectionService.GetAvailableSections(null!));
        Assert.Throws<ArgumentNullException>(() => EmailSectionService.GetRemovableSections(null!));
        Assert.Throws<ArgumentNullException>(() => EmailSectionService.AddSection(null!, option));
        Assert.Throws<ArgumentNullException>(() => EmailSectionService.AddSection(email, null!));
        Assert.Throws<ArgumentNullException>(() => EmailSectionService.RemoveSection(null!, new AdditionalInput()));
        Assert.Throws<ArgumentNullException>(() => EmailSectionService.RemoveSection(email, null!));
        Assert.Throws<ArgumentNullException>(() => EmailSectionService.CanRemove(null!));
        Assert.Throws<ArgumentNullException>(() => EmailSectionService.GetDisplayName(null!));
        Assert.Throws<ArgumentNullException>(() => EmailSectionService.SortSections(null!));
    }
}