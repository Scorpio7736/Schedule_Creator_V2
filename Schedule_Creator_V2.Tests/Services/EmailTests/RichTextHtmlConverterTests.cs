using Schedule_Creator_V2.Services.Email;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Windows;
using System.Windows.Documents;

using DocumentList =
    System.Windows.Documents.List;

namespace Schedule_Creator_V2.Tests.Services.Email
{
    public sealed class RichTextHtmlConverterTests
    {
        // =========================================================
        // PARAGRAPHS
        // =========================================================

        [Fact]
        public void ToHtml_Paragraph_ReturnsParagraphHtml()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    FlowDocument document =
                        new FlowDocument();

                    document.Blocks.Add(
                        new Paragraph(
                            new Run(
                                "Hello World")));


                    // Act

                    string html =
                        RichTextHtmlConverter.ToHtml(
                            document);


                    // Assert

                    Assert.Equal(
                        "<p style=\"margin:0 0 16px 0;\">" +
                        "Hello World" +
                        "</p>",
                        html);
                });
        }


        [Fact]
        public void ToHtml_MultipleParagraphs_ReturnsAllParagraphs()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    FlowDocument document =
                        new FlowDocument();

                    document.Blocks.Add(
                        new Paragraph(
                            new Run(
                                "First")));

                    document.Blocks.Add(
                        new Paragraph(
                            new Run(
                                "Second")));


                    // Act

                    string html =
                        RichTextHtmlConverter.ToHtml(
                            document);


                    // Assert

                    Assert.Contains(
                        "First",
                        html);

                    Assert.Contains(
                        "Second",
                        html);

                    Assert.Equal(
                        2,
                        CountOccurrences(
                            html,
                            "<p style="));
                });
        }


        // =========================================================
        // HTML ENCODING
        // =========================================================

        [Fact]
        public void ToHtml_TextContainsHtmlCharacters_EncodesText()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    FlowDocument document =
                        new FlowDocument();

                    document.Blocks.Add(
                        new Paragraph(
                            new Run(
                                "<Tower> & \"Staff\"")));


                    // Act

                    string html =
                        RichTextHtmlConverter.ToHtml(
                            document);


                    // Assert

                    Assert.Contains(
                        "&lt;Tower&gt;",
                        html);

                    Assert.Contains(
                        "&amp;",
                        html);

                    Assert.Contains(
                        "&quot;Staff&quot;",
                        html);

                    Assert.DoesNotContain(
                        "<Tower>",
                        html);
                });
        }


        // =========================================================
        // LINE BREAKS
        // =========================================================

        [Fact]
        public void ToHtml_LineBreak_ReturnsBrElement()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    FlowDocument document =
                        new FlowDocument();

                    Paragraph paragraph =
                        new Paragraph();

                    paragraph.Inlines.Add(
                        new Run(
                            "First Line"));

                    paragraph.Inlines.Add(
                        new LineBreak());

                    paragraph.Inlines.Add(
                        new Run(
                            "Second Line"));

                    document.Blocks.Add(
                        paragraph);


                    // Act

                    string html =
                        RichTextHtmlConverter.ToHtml(
                            document);


                    // Assert

                    Assert.Contains(
                        "First Line<br />Second Line",
                        html);
                });
        }


        // =========================================================
        // BOLD
        // =========================================================

        [Fact]
        public void ToHtml_BoldRun_ReturnsStrongElement()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    FlowDocument document =
                        new FlowDocument();

                    Run run =
                        new Run(
                            "Important")
                        {
                            FontWeight =
                                FontWeights.Bold
                        };

                    document.Blocks.Add(
                        new Paragraph(
                            run));


                    // Act

                    string html =
                        RichTextHtmlConverter.ToHtml(
                            document);


                    // Assert

                    Assert.Contains(
                        "<strong>Important</strong>",
                        html);
                });
        }


        // =========================================================
        // ITALIC
        // =========================================================

        [Fact]
        public void ToHtml_ItalicRun_ReturnsEmElement()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    FlowDocument document =
                        new FlowDocument();

                    Run run =
                        new Run(
                            "Important")
                        {
                            FontStyle =
                                FontStyles.Italic
                        };

                    document.Blocks.Add(
                        new Paragraph(
                            run));


                    // Act

                    string html =
                        RichTextHtmlConverter.ToHtml(
                            document);


                    // Assert

                    Assert.Contains(
                        "<em>Important</em>",
                        html);
                });
        }


        // =========================================================
        // UNDERLINE
        // =========================================================

        [Fact]
        public void ToHtml_UnderlinedRun_ReturnsUnderlineElement()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    FlowDocument document =
                        new FlowDocument();

                    Run run =
                        new Run(
                            "Important")
                        {
                            TextDecorations =
                                TextDecorations.Underline
                        };

                    document.Blocks.Add(
                        new Paragraph(
                            run));


                    // Act

                    string html =
                        RichTextHtmlConverter.ToHtml(
                            document);


                    // Assert

                    Assert.Contains(
                        "<u>Important</u>",
                        html);
                });
        }


        // =========================================================
        // COMBINED FORMATTING
        // =========================================================

        [Fact]
        public void ToHtml_BoldItalicUnderline_AppliesAllFormatting()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    FlowDocument document =
                        new FlowDocument();

                    Run run =
                        new Run(
                            "Formatted")
                        {
                            FontWeight =
                                FontWeights.Bold,

                            FontStyle =
                                FontStyles.Italic,

                            TextDecorations =
                                TextDecorations.Underline
                        };

                    document.Blocks.Add(
                        new Paragraph(
                            run));


                    // Act

                    string html =
                        RichTextHtmlConverter.ToHtml(
                            document);


                    // Assert

                    Assert.Contains(
                        "<u><em><strong>" +
                        "Formatted" +
                        "</strong></em></u>",
                        html);
                });
        }


        // =========================================================
        // SPANS
        // =========================================================

        [Fact]
        public void ToHtml_FormattedSpan_FormatsChildContent()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    FlowDocument document =
                        new FlowDocument();

                    Span span =
                        new Span
                        {
                            FontWeight =
                                FontWeights.Bold
                        };

                    span.Inlines.Add(
                        new Run(
                            "First "));

                    span.Inlines.Add(
                        new Run(
                            "Second"));

                    document.Blocks.Add(
                        new Paragraph(
                            span));


                    // Act

                    string html =
                        RichTextHtmlConverter.ToHtml(
                            document);


                    // Assert

                    Assert.Contains(
                        "<strong>First Second</strong>",
                        html);
                });
        }


        // =========================================================
        // BULLETED LIST
        // =========================================================

        [Fact]
        public void ToHtml_BulletedList_ReturnsUnorderedList()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    FlowDocument document =
                        new FlowDocument();

                    DocumentList list =
                        new DocumentList
                        {
                            MarkerStyle =
                                TextMarkerStyle.Disc
                        };

                    list.ListItems.Add(
                        CreateListItem(
                            "First"));

                    list.ListItems.Add(
                        CreateListItem(
                            "Second"));

                    document.Blocks.Add(
                        list);


                    // Act

                    string html =
                        RichTextHtmlConverter.ToHtml(
                            document);


                    // Assert

                    Assert.Contains(
                        "<ul style=",
                        html);

                    Assert.Contains(
                        "</ul>",
                        html);

                    Assert.Contains(
                        "<li style=",
                        html);

                    Assert.Contains(
                        "First",
                        html);

                    Assert.Contains(
                        "Second",
                        html);

                    Assert.DoesNotContain(
                        "<ol style=",
                        html);
                });
        }


        // =========================================================
        // NUMBERED LIST
        // =========================================================

        [Fact]
        public void ToHtml_DecimalList_ReturnsOrderedList()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    FlowDocument document =
                        new FlowDocument();

                    DocumentList list =
                        new DocumentList
                        {
                            MarkerStyle =
                                TextMarkerStyle.Decimal
                        };

                    list.ListItems.Add(
                        CreateListItem(
                            "One"));

                    list.ListItems.Add(
                        CreateListItem(
                            "Two"));

                    document.Blocks.Add(
                        list);


                    // Act

                    string html =
                        RichTextHtmlConverter.ToHtml(
                            document);


                    // Assert

                    Assert.Contains(
                        "<ol style=",
                        html);

                    Assert.Contains(
                        "</ol>",
                        html);

                    Assert.Contains(
                        "One",
                        html);

                    Assert.Contains(
                        "Two",
                        html);
                });
        }


        [Theory]
        [InlineData(
            TextMarkerStyle.Decimal)]
        [InlineData(
            TextMarkerStyle.LowerLatin)]
        [InlineData(
            TextMarkerStyle.UpperLatin)]
        [InlineData(
            TextMarkerStyle.LowerRoman)]
        [InlineData(
            TextMarkerStyle.UpperRoman)]
        public void ToHtml_NumberedMarkerStyle_ReturnsOrderedList(
            TextMarkerStyle markerStyle)
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    FlowDocument document =
                        new FlowDocument();

                    DocumentList list =
                        new DocumentList
                        {
                            MarkerStyle =
                                markerStyle
                        };

                    list.ListItems.Add(
                        CreateListItem(
                            "Item"));

                    document.Blocks.Add(
                        list);


                    // Act

                    string html =
                        RichTextHtmlConverter.ToHtml(
                            document);


                    // Assert

                    Assert.Contains(
                        "<ol style=",
                        html);
                });
        }


        // =========================================================
        // LIST ITEM PARAGRAPHS
        // =========================================================

        [Fact]
        public void ToHtml_ListItem_DoesNotAddParagraphInsideListItem()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    FlowDocument document =
                        new FlowDocument();

                    DocumentList list =
                        new DocumentList
                        {
                            MarkerStyle =
                                TextMarkerStyle.Disc
                        };

                    list.ListItems.Add(
                        CreateListItem(
                            "List Text"));

                    document.Blocks.Add(
                        list);


                    // Act

                    string html =
                        RichTextHtmlConverter.ToHtml(
                            document);


                    // Assert

                    Assert.Contains(
                        "List Text",
                        html);

                    Assert.DoesNotContain(
                        "<p style=",
                        html);
                });
        }


        // =========================================================
        // SECTION
        // =========================================================

        [Fact]
        public void ToHtml_Section_ConvertsBlocksInsideSection()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    FlowDocument document =
                        new FlowDocument();

                    Section section =
                        new Section();

                    section.Blocks.Add(
                        new Paragraph(
                            new Run(
                                "Section One")));

                    section.Blocks.Add(
                        new Paragraph(
                            new Run(
                                "Section Two")));

                    document.Blocks.Add(
                        section);


                    // Act

                    string html =
                        RichTextHtmlConverter.ToHtml(
                            document);


                    // Assert

                    Assert.Contains(
                        "Section One",
                        html);

                    Assert.Contains(
                        "Section Two",
                        html);

                    Assert.Equal(
                        2,
                        CountOccurrences(
                            html,
                            "<p style="));
                });
        }


        // =========================================================
        // EMPTY DOCUMENT
        // =========================================================

        [Fact]
        public void ToHtml_EmptyDocument_ReturnsEmptyString()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    FlowDocument document =
                        new FlowDocument();


                    // Act

                    string html =
                        RichTextHtmlConverter.ToHtml(
                            document);


                    // Assert

                    Assert.Equal(
                        string.Empty,
                        html);
                });
        }


        // =========================================================
        // NULL DOCUMENT
        // =========================================================

        [Fact]
        public void ToHtml_NullDocument_ThrowsArgumentNullException()
        {
            RunOnSta(
                () =>
                {
                    Assert.Throws<ArgumentNullException>(
                        () =>
                            RichTextHtmlConverter.ToHtml(
                                null!));
                });
        }


        // =========================================================
        // SET PLAIN TEXT
        // =========================================================

        [Fact]
        public void SetPlainText_SimpleText_CreatesParagraph()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    FlowDocument document =
                        new FlowDocument();


                    // Act

                    RichTextHtmlConverter.SetPlainText(
                        document,
                        "Hello World");


                    // Assert

                    Assert.Single(
                        document.Blocks);

                    Paragraph paragraph =
                        Assert.IsType<Paragraph>(
                            document.Blocks.FirstBlock);

                    Assert.Equal(
                        "Hello World",
                        new TextRange(
                            paragraph.ContentStart,
                            paragraph.ContentEnd)
                            .Text
                            .TrimEnd(
                                '\r',
                                '\n'));
                });
        }


        [Fact]
        public void SetPlainText_EmptyText_CreatesEmptyParagraph()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    FlowDocument document =
                        new FlowDocument();


                    // Act

                    RichTextHtmlConverter.SetPlainText(
                        document,
                        string.Empty);


                    // Assert

                    Assert.Single(
                        document.Blocks);

                    Assert.IsType<Paragraph>(
                        document.Blocks.FirstBlock);
                });
        }


        [Fact]
        public void SetPlainText_NullText_CreatesEmptyParagraph()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    FlowDocument document =
                        new FlowDocument();


                    // Act

                    RichTextHtmlConverter.SetPlainText(
                        document,
                        null);


                    // Assert

                    Assert.Single(
                        document.Blocks);

                    Assert.IsType<Paragraph>(
                        document.Blocks.FirstBlock);
                });
        }


        [Fact]
        public void SetPlainText_ReplacesExistingBlocks()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    FlowDocument document =
                        new FlowDocument();

                    document.Blocks.Add(
                        new Paragraph(
                            new Run(
                                "Old Text")));


                    // Act

                    RichTextHtmlConverter.SetPlainText(
                        document,
                        "New Text");


                    // Assert

                    Assert.Single(
                        document.Blocks);

                    string html =
                        RichTextHtmlConverter.ToHtml(
                            document);

                    Assert.Contains(
                        "New Text",
                        html);

                    Assert.DoesNotContain(
                        "Old Text",
                        html);
                });
        }


        // =========================================================
        // PLAIN TEXT LINE BREAK
        // =========================================================

        [Fact]
        public void SetPlainText_SingleNewline_CreatesLineBreak()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    FlowDocument document =
                        new FlowDocument();


                    // Act

                    RichTextHtmlConverter.SetPlainText(
                        document,
                        "First Line\nSecond Line");

                    string html =
                        RichTextHtmlConverter.ToHtml(
                            document);


                    // Assert

                    Assert.Single(
                        document.Blocks);

                    Assert.Contains(
                        "First Line<br />Second Line",
                        html);
                });
        }


        // =========================================================
        // PLAIN TEXT PARAGRAPHS
        // =========================================================

        [Fact]
        public void SetPlainText_BlankLine_CreatesSeparateParagraphs()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    FlowDocument document =
                        new FlowDocument();


                    // Act

                    RichTextHtmlConverter.SetPlainText(
                        document,
                        "First Paragraph\n\nSecond Paragraph");


                    // Assert

                    Assert.Equal(
                        2,
                        document.Blocks.Count);

                    string html =
                        RichTextHtmlConverter.ToHtml(
                            document);

                    Assert.Contains(
                        "First Paragraph",
                        html);

                    Assert.Contains(
                        "Second Paragraph",
                        html);

                    Assert.Equal(
                        2,
                        CountOccurrences(
                            html,
                            "<p style="));
                });
        }


        // =========================================================
        // WINDOWS LINE ENDINGS
        // =========================================================

        [Fact]
        public void SetPlainText_WindowsLineEnding_NormalizesToLineBreak()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    FlowDocument document =
                        new FlowDocument();


                    // Act

                    RichTextHtmlConverter.SetPlainText(
                        document,
                        "First\r\nSecond");

                    string html =
                        RichTextHtmlConverter.ToHtml(
                            document);


                    // Assert

                    Assert.Contains(
                        "First<br />Second",
                        html);
                });
        }


        [Fact]
        public void SetPlainText_OldMacLineEnding_NormalizesToLineBreak()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    FlowDocument document =
                        new FlowDocument();


                    // Act

                    RichTextHtmlConverter.SetPlainText(
                        document,
                        "First\rSecond");

                    string html =
                        RichTextHtmlConverter.ToHtml(
                            document);


                    // Assert

                    Assert.Contains(
                        "First<br />Second",
                        html);
                });
        }


        // =========================================================
        // ROUND TRIP
        // =========================================================

        [Fact]
        public void SetPlainText_ThenToHtml_PreservesTextStructure()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    FlowDocument document =
                        new FlowDocument();

                    string text =
                        "Hello Tower Team\n" +
                        "Please review the schedule.\n\n" +
                        "Thank you.";


                    // Act

                    RichTextHtmlConverter.SetPlainText(
                        document,
                        text);

                    string html =
                        RichTextHtmlConverter.ToHtml(
                            document);


                    // Assert

                    Assert.Contains(
                        "Hello Tower Team<br />" +
                        "Please review the schedule.",
                        html);

                    Assert.Contains(
                        "Thank you.",
                        html);

                    Assert.Equal(
                        2,
                        CountOccurrences(
                            html,
                            "<p style="));
                });
        }


        // =========================================================
        // SET PLAIN TEXT NULL DOCUMENT
        // =========================================================

        [Fact]
        public void SetPlainText_NullDocument_ThrowsArgumentNullException()
        {
            RunOnSta(
                () =>
                {
                    Assert.Throws<ArgumentNullException>(
                        () =>
                            RichTextHtmlConverter.SetPlainText(
                                null!,
                                "Test"));
                });
        }


        // =========================================================
        // HELPERS
        // =========================================================

        private static ListItem CreateListItem(
            string text)
        {
            return new ListItem(
                new Paragraph(
                    new Run(
                        text)));
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