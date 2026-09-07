using Schedule_Creator_V2.Models.Interfaces;
using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Services.Email;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Schedule_Creator_V2.Tests.Services.Email
{
    public sealed class EmailInputFormServiceTests
    {
        // =========================================================
        // NO EMAIL TYPE SELECTED
        // =========================================================

        [Fact]
        public void ShowNoEmailTypeSelectedMessage_CreatesPlaceholder()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    StackPanel container =
                        new StackPanel();


                    // Act

                    EmailInputFormService
                        .ShowNoEmailTypeSelectedMessage(
                            container);


                    // Assert

                    Assert.Single(
                        container.Children);

                    Assert.IsType<Border>(
                        container.Children[0]);

                    List<TextBlock> textBlocks =
                        FindDescendants<TextBlock>(
                            container)
                        .ToList();

                    Assert.Contains(
                        textBlocks,
                        textBlock =>
                            textBlock.Text ==
                            "No Email Type Selected");

                    Assert.Contains(
                        textBlocks,
                        textBlock =>
                            textBlock.Text ==
                            "Select an email type to display its input fields.");
                });
        }


        // =========================================================
        // NO INPUTS CONFIGURED
        // =========================================================

        [Fact]
        public void BuildEmailInputControls_NoInputs_CreatesNoInputsPlaceholder()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    StackPanel container =
                        new StackPanel();

                    EmailType emailType =
                        new EmailType(
                            "Empty",
                            new List<IEmailInputs>(),
                            true);


                    // Act

                    EmailInputFormService
                        .BuildEmailInputControls(
                            container,
                            emailType);


                    // Assert

                    Assert.Single(
                        container.Children);

                    List<TextBlock> textBlocks =
                        FindDescendants<TextBlock>(
                            container)
                        .ToList();

                    Assert.Contains(
                        textBlocks,
                        textBlock =>
                            textBlock.Text ==
                            "No Inputs Configured");

                    Assert.Contains(
                        textBlocks,
                        textBlock =>
                            textBlock.Text ==
                            "This email type does not contain any input sections.");
                });
        }


        // =========================================================
        // CLEAR EXISTING CONTROLS
        // =========================================================

        [Fact]
        public void BuildEmailInputControls_ExistingControls_ClearsOldControls()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    StackPanel container =
                        new StackPanel();

                    TextBlock oldControl =
                        new TextBlock
                        {
                            Text =
                                "Old Control"
                        };

                    container.Children.Add(
                        oldControl);

                    EmailType emailType =
                        CreateDetailsEmail(
                            "New Subject");


                    // Act

                    EmailInputFormService
                        .BuildEmailInputControls(
                            container,
                            emailType);


                    // Assert

                    Assert.DoesNotContain(
                        oldControl,
                        container.Children
                            .Cast<UIElement>());
                });
        }


        // =========================================================
        // GROUP CREATION
        // =========================================================

        [Fact]
        public void BuildEmailInputControls_MultipleInputGroups_CreatesBorderForEachGroup()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    EmailDetailsInputs details =
                        new EmailDetailsInputs(
                            "Test");

                    CustomHeaderInputs header =
                        new CustomHeaderInputs(
                            "Test Organization",
                            "HEADER",
                            "Heading",
                            "Subtitle",
                            "https://example.com/header.png");

                    CustomBodyInputs body =
                        new CustomBodyInputs(
                            "Hello,",
                            "Body");

                    EmailType emailType =
                        new EmailType(
                            "Test",
                            new List<IEmailInputs>
                            {
                                details,
                                header,
                                body
                            },
                            true);

                    StackPanel container =
                        new StackPanel();


                    // Act

                    EmailInputFormService
                        .BuildEmailInputControls(
                            container,
                            emailType);


                    // Assert

                    Assert.Equal(
                        3,
                        container.Children.Count);

                    Assert.All(
                        container.Children
                            .Cast<UIElement>(),
                        child =>
                            Assert.IsType<Border>(
                                child));
                });
        }


        [Fact]
        public void BuildEmailInputControls_CreatesInputGroupHeadings()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    EmailDetailsInputs details =
                        new EmailDetailsInputs(
                            "Test");

                    CustomBodyInputs body =
                        new CustomBodyInputs(
                            "Hello,",
                            "Body");

                    EmailType emailType =
                        new EmailType(
                            "Test",
                            new List<IEmailInputs>
                            {
                                details,
                                body
                            },
                            true);

                    StackPanel container =
                        new StackPanel();


                    // Act

                    EmailInputFormService
                        .BuildEmailInputControls(
                            container,
                            emailType);


                    // Assert

                    List<TextBlock> textBlocks =
                        FindDescendants<TextBlock>(
                            container)
                        .ToList();

                    Assert.Contains(
                        textBlocks,
                        textBlock =>
                            textBlock.Text ==
                            "Email Details");

                    Assert.Contains(
                        textBlocks,
                        textBlock =>
                            textBlock.Text ==
                            "Custom Body");
                });
        }


        // =========================================================
        // STRING INPUT
        // =========================================================

        [Fact]
        public void BuildEmailInputControls_EmailDetails_LoadsSubjectIntoTextBox()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    EmailType emailType =
                        CreateDetailsEmail(
                            "Tower Schedule");

                    StackPanel container =
                        new StackPanel();


                    // Act

                    EmailInputFormService
                        .BuildEmailInputControls(
                            container,
                            emailType);


                    // Assert

                    TextBox textBox =
                        Assert.Single(
                            FindDescendants<TextBox>(
                                container));

                    Assert.Equal(
                        "Tower Schedule",
                        textBox.Text);
                });
        }


        // =========================================================
        // RICH TEXT INPUT
        // =========================================================

        [Fact]
        public void BuildEmailInputControls_EmailBody_CreatesRichTextBox()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    CustomBodyInputs body =
                        new CustomBodyInputs(
                            "Hello Tower Team,",
                            "Please review the schedule.");

                    EmailType emailType =
                        new EmailType(
                            "Test",
                            new List<IEmailInputs>
                            {
                                body
                            });

                    StackPanel container =
                        new StackPanel();


                    // Act

                    EmailInputFormService
                        .BuildEmailInputControls(
                            container,
                            emailType);


                    // Assert

                    RichTextBox richTextBox =
                        Assert.Single(
                            FindDescendants<RichTextBox>(
                                container));

                    string text =
                        GetDocumentText(
                            richTextBox.Document);

                    Assert.Contains(
                        "Please review the schedule.",
                        text);
                });
        }


        [Fact]
        public void BuildEmailInputControls_EmailBody_LoadsPlainTextWithLineBreaks()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    CustomBodyInputs body =
                        new CustomBodyInputs(
                            "Hello,",
                            "First Line\nSecond Line");

                    EmailType emailType =
                        new EmailType(
                            "Test",
                            new List<IEmailInputs>
                            {
                                body
                            });

                    StackPanel container =
                        new StackPanel();


                    // Act

                    EmailInputFormService
                        .BuildEmailInputControls(
                            container,
                            emailType);


                    // Assert

                    RichTextBox editor =
                        Assert.Single(
                            FindDescendants<RichTextBox>(
                                container));

                    string text =
                        GetDocumentText(
                            editor.Document);

                    Assert.Contains(
                        "First Line",
                        text);

                    Assert.Contains(
                        "Second Line",
                        text);
                });
        }


        // =========================================================
        // STRING LIST INPUT
        // =========================================================

        [Fact]
        public void BuildEmailInputControls_AnnouncementList_LoadsItemsOnePerLine()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    CustomAnnouncementsInputs announcements =
                        new CustomAnnouncementsInputs(
                            "ANNOUNCEMENTS",
                            "Latest updates.",
                            new List<string>
                            {
                                "First Announcement",
                                "Second Announcement"
                            });

                    EmailType emailType =
                        new EmailType(
                            "Test",
                            new List<IEmailInputs>
                            {
                                announcements
                            });

                    StackPanel container =
                        new StackPanel();


                    // Act

                    EmailInputFormService
                        .BuildEmailInputControls(
                            container,
                            emailType);


                    // Assert

                    TextBox listTextBox =
                        FindDescendants<TextBox>(
                            container)
                        .Single(
                            textBox =>
                                textBox.AcceptsReturn);

                    Assert.Equal(
                        "First Announcement" +
                        Environment.NewLine +
                        "Second Announcement",
                        listTextBox.Text);
                });
        }


        // =========================================================
        // IMAGE REQUIRED STATE
        // =========================================================

        [Fact]
        public void BuildEmailInputControls_HeaderImage_IsOptional()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    CustomHeaderInputs header =
                        new CustomHeaderInputs(
                            "Test Organization",
                            "HEADER",
                            "Heading",
                            "Subtitle",
                            "https://example.com/default-header.png");

                    EmailType emailType =
                        new EmailType(
                            "Test",
                            new List<IEmailInputs>
                            {
                                header
                            });

                    StackPanel container =
                        new StackPanel();


                    // Act

                    EmailInputFormService
                        .BuildEmailInputControls(
                            container,
                            emailType);


                    // Assert

                    TextBox imageTextBox =
                        FindDescendants<TextBox>(
                            container)
                        .Single(
                            textBox =>
                                !EmailValidationService
                                    .GetIsRequired(
                                        textBox));

                    Assert.False(
                        EmailValidationService
                            .GetIsRequired(
                                imageTextBox));
                });
        }


        [Fact]
        public void BuildEmailInputControls_ContentImage_IsRequired()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    CustomImageInputs image =
                        new CustomImageInputs(
                            "",
                            "Tower image");

                    EmailType emailType =
                        new EmailType(
                            "Test",
                            new List<IEmailInputs>
                            {
                                image
                            });

                    StackPanel container =
                        new StackPanel();


                    // Act

                    EmailInputFormService
                        .BuildEmailInputControls(
                            container,
                            emailType);


                    // Assert

                    TextBox imageTextBox =
                        FindDescendants<TextBox>(
                            container)
                        .Single(
                            textBox =>
                                textBox.Height ==
                                38);

                    Assert.True(
                        EmailValidationService
                            .GetIsRequired(
                                imageTextBox));
                });
        }


        // =========================================================
        // HEADER DEFAULT IMAGE
        // =========================================================

        [Fact]
        public void BuildEmailInputControls_DefaultHeaderImage_DoesNotDisplayImageSource()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    const string defaultImage =
                        "https://example.com/default-header.png";

                    CustomHeaderInputs header =
                        new CustomHeaderInputs(
                            "Test Organization",
                            "HEADER",
                            "Heading",
                            "Subtitle",
                            defaultImage);

                    EmailType emailType =
                        new EmailType(
                            "Test",
                            new List<IEmailInputs>
                            {
                                header
                            });

                    StackPanel container =
                        new StackPanel();


                    // Act

                    EmailInputFormService
                        .BuildEmailInputControls(
                            container,
                            emailType);


                    // Assert

                    TextBox imageTextBox =
                        FindDescendants<TextBox>(
                            container)
                        .Single(
                            textBox =>
                                !EmailValidationService
                                    .GetIsRequired(
                                        textBox));

                    Assert.Equal(
                        string.Empty,
                        imageTextBox.Text);

                    Assert.Equal(
                        defaultImage,
                        header.HeaderImageUrl);
                });
        }


        // =========================================================
        // APPLY NORMAL STRING
        // =========================================================

        [Fact]
        public void ApplyInputValues_ChangedSubject_UpdatesEmailDetails()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    EmailDetailsInputs details =
                        new EmailDetailsInputs(
                            "Original Subject");

                    EmailType emailType =
                        new EmailType(
                            "Test",
                            new List<IEmailInputs>
                            {
                                details
                            });

                    StackPanel container =
                        new StackPanel();

                    EmailInputFormService
                        .BuildEmailInputControls(
                            container,
                            emailType);

                    TextBox textBox =
                        Assert.Single(
                            FindDescendants<TextBox>(
                                container));

                    textBox.Text =
                        "Updated Subject";


                    // Act

                    EmailInputFormService
                        .ApplyInputValues(
                            container);


                    // Assert

                    Assert.Equal(
                        "Updated Subject",
                        details.Subject);
                });
        }


        // =========================================================
        // APPLY STRING LIST
        // =========================================================

        [Fact]
        public void ApplyInputValues_AnnouncementList_SplitsAndTrimsItems()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    CustomAnnouncementsInputs announcements =
                        new CustomAnnouncementsInputs(
                            "ANNOUNCEMENTS",
                            "Intro",
                            new List<string>
                            {
                                "Old Item"
                            });

                    EmailType emailType =
                        new EmailType(
                            "Test",
                            new List<IEmailInputs>
                            {
                                announcements
                            });

                    StackPanel container =
                        new StackPanel();

                    EmailInputFormService
                        .BuildEmailInputControls(
                            container,
                            emailType);

                    TextBox listTextBox =
                        FindDescendants<TextBox>(
                            container)
                        .Single(
                            textBox =>
                                textBox.AcceptsReturn);

                    listTextBox.Text =
                        "  First Item  " +
                        Environment.NewLine +
                        Environment.NewLine +
                        " Second Item " +
                        Environment.NewLine +
                        "   " +
                        Environment.NewLine +
                        "Third Item";


                    // Act

                    EmailInputFormService
                        .ApplyInputValues(
                            container);


                    // Assert

                    Assert.Equal(
                        3,
                        announcements
                            .AnnouncementsList
                            .Count);

                    Assert.Equal(
                        "First Item",
                        announcements
                            .AnnouncementsList[0]);

                    Assert.Equal(
                        "Second Item",
                        announcements
                            .AnnouncementsList[1]);

                    Assert.Equal(
                        "Third Item",
                        announcements
                            .AnnouncementsList[2]);
                });
        }


        // =========================================================
        // APPLY RICH TEXT
        // =========================================================

        [Fact]
        public void ApplyInputValues_RichTextBody_StoresRichTextPrefix()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    CustomBodyInputs body =
                        new CustomBodyInputs(
                            "Hello,",
                            "Original Body");

                    EmailType emailType =
                        new EmailType(
                            "Test",
                            new List<IEmailInputs>
                            {
                                body
                            });

                    StackPanel container =
                        new StackPanel();

                    EmailInputFormService
                        .BuildEmailInputControls(
                            container,
                            emailType);

                    RichTextBox editor =
                        Assert.Single(
                            FindDescendants<RichTextBox>(
                                container));

                    editor.Document
                        .Blocks
                        .Clear();

                    editor.Document
                        .Blocks
                        .Add(
                            new Paragraph(
                                new Run(
                                    "Updated Body")));


                    // Act

                    EmailInputFormService
                        .ApplyInputValues(
                            container);


                    // Assert

                    Assert.StartsWith(
                        "[[RICH_TEXT_HTML]]",
                        body.EmailBody);

                    Assert.Contains(
                        "<p>Updated Body</p>",
                        body.EmailBody);
                });
        }


        [Fact]
        public void ApplyInputValues_BoldRichText_PreservesBoldFormatting()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    CustomBodyInputs body =
                        new CustomBodyInputs(
                            "Hello,",
                            "");

                    EmailType emailType =
                        new EmailType(
                            "Test",
                            new List<IEmailInputs>
                            {
                                body
                            });

                    StackPanel container =
                        new StackPanel();

                    EmailInputFormService
                        .BuildEmailInputControls(
                            container,
                            emailType);

                    RichTextBox editor =
                        Assert.Single(
                            FindDescendants<RichTextBox>(
                                container));

                    Run boldRun =
                        new Run(
                            "Important")
                        {
                            FontWeight =
                                FontWeights.Bold
                        };

                    editor.Document
                        .Blocks
                        .Clear();

                    editor.Document
                        .Blocks
                        .Add(
                            new Paragraph(
                                boldRun));


                    // Act

                    EmailInputFormService
                        .ApplyInputValues(
                            container);


                    // Assert

                    Assert.Contains(
                        "<strong>Important</strong>",
                        body.EmailBody);
                });
        }


        [Fact]
        public void ApplyInputValues_EmptyRichText_StoresEmptyString()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    CustomBodyInputs body =
                        new CustomBodyInputs(
                            "Hello,",
                            "Existing Body");

                    EmailType emailType =
                        new EmailType(
                            "Test",
                            new List<IEmailInputs>
                            {
                                body
                            });

                    StackPanel container =
                        new StackPanel();

                    EmailInputFormService
                        .BuildEmailInputControls(
                            container,
                            emailType);

                    RichTextBox editor =
                        Assert.Single(
                            FindDescendants<RichTextBox>(
                                container));

                    editor.Document
                        .Blocks
                        .Clear();

                    editor.Document
                        .Blocks
                        .Add(
                            new Paragraph());


                    // Act

                    EmailInputFormService
                        .ApplyInputValues(
                            container);


                    // Assert

                    Assert.Equal(
                        string.Empty,
                        body.EmailBody);
                });
        }


        // =========================================================
        // APPLY IMAGE URL
        // =========================================================

        [Fact]
        public void ApplyInputValues_ContentImageUrl_UpdatesImageSource()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    CustomImageInputs image =
                        new CustomImageInputs(
                            "",
                            "Tower");

                    EmailType emailType =
                        new EmailType(
                            "Test",
                            new List<IEmailInputs>
                            {
                                image
                            });

                    StackPanel container =
                        new StackPanel();

                    EmailInputFormService
                        .BuildEmailInputControls(
                            container,
                            emailType);

                    TextBox imageTextBox =
                        FindDescendants<TextBox>(
                            container)
                        .Single(
                            textBox =>
                                textBox.Height ==
                                38);

                    imageTextBox.Text =
                        "  https://example.com/tower.png  ";


                    // Act

                    EmailInputFormService
                        .ApplyInputValues(
                            container);


                    // Assert

                    Assert.Equal(
                        "https://example.com/tower.png",
                        image.ImageSource);
                });
        }


        // =========================================================
        // LOCAL IMAGE -> BASE64
        // =========================================================

        [Fact]
        public void ApplyInputValues_LocalPngPath_ConvertsImageToDataUri()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    string imagePath =
                        Path.Combine(
                            Path.GetTempPath(),
                            $"{Guid.NewGuid():N}.png");

                    byte[] imageBytes =
                    {
                        1,
                        2,
                        3,
                        4,
                        5
                    };

                    File.WriteAllBytes(
                        imagePath,
                        imageBytes);

                    try
                    {
                        CustomImageInputs image =
                            new CustomImageInputs(
                                "",
                                "Tower");

                        EmailType emailType =
                            new EmailType(
                                "Test",
                                new List<IEmailInputs>
                                {
                                    image
                                });

                        StackPanel container =
                            new StackPanel();

                        EmailInputFormService
                            .BuildEmailInputControls(
                                container,
                                emailType);

                        TextBox imageTextBox =
                            FindDescendants<TextBox>(
                                container)
                            .Single(
                                textBox =>
                                    textBox.Height ==
                                    38);

                        imageTextBox.Text =
                            imagePath;

                        string expected =
                            "data:image/png;base64," +
                            Convert.ToBase64String(
                                imageBytes);


                        // Act

                        EmailInputFormService
                            .ApplyInputValues(
                                container);


                        // Assert

                        Assert.Equal(
                            expected,
                            image.ImageSource);
                    }
                    finally
                    {
                        if (File.Exists(
                                imagePath))
                        {
                            File.Delete(
                                imagePath);
                        }
                    }
                });
        }


        // =========================================================
        // HEADER DEFAULT RESTORATION
        // =========================================================

        [Fact]
        public void ApplyInputValues_EmptyHeaderImage_RestoresConfiguredDefault()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    const string defaultImage =
                        "https://example.com/default-header.png";

                    CustomHeaderInputs header =
                        new CustomHeaderInputs(
                            "Organization",
                            "HEADER",
                            "Heading",
                            "Subtitle",
                            defaultImage);

                    EmailType emailType =
                        new EmailType(
                            "Test",
                            new List<IEmailInputs>
                            {
                                header
                            });

                    StackPanel container =
                        new StackPanel();

                    EmailInputFormService
                        .BuildEmailInputControls(
                            container,
                            emailType);

                    TextBox imageTextBox =
                        FindDescendants<TextBox>(
                            container)
                        .Single(
                            textBox =>
                                !EmailValidationService
                                    .GetIsRequired(
                                        textBox));

                    /*
                     * Simulate some other code changing the
                     * model while the UI is still configured
                     * to use the default-image state.
                     */

                    header.HeaderImageUrl =
                        "https://example.com/custom.png";

                    imageTextBox.Text =
                        string.Empty;


                    // Act

                    EmailInputFormService
                        .ApplyInputValues(
                            container);


                    // Assert

                    Assert.Equal(
                        defaultImage,
                        header.HeaderImageUrl);
                });
        }


        // =========================================================
        // RICH HTML LOAD
        // =========================================================

        [Fact]
        public void BuildEmailInputControls_SavedRichText_LoadsFormattedContent()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    CustomBodyInputs body =
                        new CustomBodyInputs(
                            "Hello,",
                            "[[RICH_TEXT_HTML]]" +
                            "<p><strong>Important</strong> Message</p>");

                    EmailType emailType =
                        new EmailType(
                            "Test",
                            new List<IEmailInputs>
                            {
                                body
                            });

                    StackPanel container =
                        new StackPanel();


                    // Act

                    EmailInputFormService
                        .BuildEmailInputControls(
                            container,
                            emailType);


                    // Assert

                    RichTextBox editor =
                        Assert.Single(
                            FindDescendants<RichTextBox>(
                                container));

                    string text =
                        GetDocumentText(
                            editor.Document);

                    Assert.Contains(
                        "Important Message",
                        NormalizeWhitespace(
                            text));
                });
        }


        // =========================================================
        // REBUILD
        // =========================================================

        [Fact]
        public void BuildEmailInputControls_CalledTwice_DoesNotDuplicateControls()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    EmailType emailType =
                        CreateDetailsEmail(
                            "Test");

                    StackPanel container =
                        new StackPanel();


                    // Act

                    EmailInputFormService
                        .BuildEmailInputControls(
                            container,
                            emailType);

                    EmailInputFormService
                        .BuildEmailInputControls(
                            container,
                            emailType);


                    // Assert

                    Assert.Single(
                        container.Children);

                    Assert.Single(
                        FindDescendants<TextBox>(
                            container));
                });
        }


        // =========================================================
        // HELPERS
        // =========================================================

        private static EmailType CreateDetailsEmail(
            string subject)
        {
            return new EmailType(
                "Test Email",
                new List<IEmailInputs>
                {
                    new EmailDetailsInputs(
                        subject)
                },
                true);
        }


        private static string GetDocumentText(
            FlowDocument document)
        {
            TextRange range =
                new TextRange(
                    document.ContentStart,
                    document.ContentEnd);

            return range.Text;
        }


        private static string NormalizeWhitespace(
            string value)
        {
            return string.Join(
                " ",
                value
                    .Split(
                        (char[]?)null,
                        StringSplitOptions
                            .RemoveEmptyEntries));
        }


        // =========================================================
        // CONTROL ENUMERATION
        // =========================================================

        private static IEnumerable<T>
            FindDescendants<T>(
                DependencyObject root)
            where T : DependencyObject
        {
            if (root is T match)
            {
                yield return match;
            }

            if (root is Panel panel)
            {
                foreach (UIElement child
                         in panel.Children)
                {
                    foreach (T descendant
                             in FindDescendants<T>(
                                 child))
                    {
                        yield return
                            descendant;
                    }
                }

                yield break;
            }

            if (root is Border border &&
                border.Child is not null)
            {
                foreach (T descendant
                         in FindDescendants<T>(
                             border.Child))
                {
                    yield return
                        descendant;
                }

                yield break;
            }

            if (root is ContentControl contentControl &&
                contentControl.Content
                    is DependencyObject content)
            {
                foreach (T descendant
                         in FindDescendants<T>(
                             content))
                {
                    yield return
                        descendant;
                }
            }
        }


        // =========================================================
        // STA TEST RUNNER
        // =========================================================

        private static void RunOnSta(
            Action action)
        {
            Exception? capturedException =
                null;

            Thread thread =
                new Thread(
                    () =>
                    {
                        try
                        {
                            action();
                        }
                        catch (Exception exception)
                        {
                            capturedException =
                                exception;
                        }
                    });

            thread.SetApartmentState(
                ApartmentState.STA);

            thread.Start();

            thread.Join();

            if (capturedException is not null)
            {
                ExceptionDispatchInfo
                    .Capture(
                        capturedException)
                    .Throw();
            }
        }
    }
}