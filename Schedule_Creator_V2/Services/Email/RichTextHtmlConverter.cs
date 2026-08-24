using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Documents;

namespace Schedule_Creator_V2.Services.Email
{
    public static class RichTextHtmlConverter
    {
        /// <summary>
        /// Converts a WPF FlowDocument into a limited,
        /// email-safe HTML representation.
        ///
        /// Supported formatting:
        /// - Paragraphs
        /// - Bold
        /// - Italic
        /// - Underline
        /// - Bulleted lists
        /// - Numbered lists
        /// - Line breaks
        /// </summary>
        public static string ToHtml(
            FlowDocument document)
        {
            ArgumentNullException.ThrowIfNull(document);

            StringBuilder html = new StringBuilder();

            foreach (Block block in document.Blocks)
            {
                html.Append(
                    ConvertBlock(block));
            }

            return html.ToString();
        }

        /// <summary>
        /// Loads ordinary plain text into a FlowDocument.
        /// </summary>
        public static void SetPlainText(
            FlowDocument document,
            string? text)
        {
            ArgumentNullException.ThrowIfNull(document);

            document.Blocks.Clear();

            string normalizedText =
                (text ?? string.Empty)
                    .Replace(
                        "\r\n",
                        "\n",
                        StringComparison.Ordinal)
                    .Replace(
                        "\r",
                        "\n",
                        StringComparison.Ordinal);

            if (string.IsNullOrEmpty(normalizedText))
            {
                document.Blocks.Add(
                    new Paragraph());

                return;
            }

            /*
             * Split blank lines into separate paragraphs.
             */
            string[] paragraphs =
                normalizedText.Split(
                    new[] { "\n\n" },
                    StringSplitOptions.None);

            foreach (string paragraphText in paragraphs)
            {
                Paragraph paragraph =
                    new Paragraph();

                string[] lines =
                    paragraphText.Split('\n');

                for (int i = 0;
                     i < lines.Length;
                     i++)
                {
                    paragraph.Inlines.Add(
                        new Run(lines[i]));

                    if (i < lines.Length - 1)
                    {
                        paragraph.Inlines.Add(
                            new LineBreak());
                    }
                }

                document.Blocks.Add(paragraph);
            }
        }

        /// <summary>
        /// Converts an individual WPF document Block
        /// into HTML.
        /// </summary>
        private static string ConvertBlock(
            Block block)
        {
            return block switch
            {
                Paragraph paragraph =>
                    ConvertParagraph(paragraph),

                List list =>
                    ConvertList(list),

                Section section =>
                    ConvertSection(section),

                _ =>
                    string.Empty
            };
        }

        /// <summary>
        /// Converts a Section and all blocks inside it.
        /// </summary>
        private static string ConvertSection(
            Section section)
        {
            StringBuilder html =
                new StringBuilder();

            foreach (Block block in section.Blocks)
            {
                html.Append(
                    ConvertBlock(block));
            }

            return html.ToString();
        }

        /// <summary>
        /// Converts a paragraph.
        /// </summary>
        private static string ConvertParagraph(
            Paragraph paragraph)
        {
            StringBuilder html =
                new StringBuilder();

            html.Append(
                "<p style=\"margin:0 0 16px 0;\">");

            foreach (Inline inline in paragraph.Inlines)
            {
                html.Append(
                    ConvertInline(inline));
            }

            html.Append("</p>");

            return html.ToString();
        }

        /// <summary>
        /// Converts either a numbered or bulleted list.
        /// </summary>
        private static string ConvertList(
            List list)
        {
            bool isOrdered =
                IsNumberedList(list);

            string tag =
                isOrdered
                    ? "ol"
                    : "ul";

            StringBuilder html =
                new StringBuilder();

            html.Append(
                $"<{tag} style=\"margin:0 0 16px 22px;padding:0;\">");

            foreach (ListItem listItem in list.ListItems)
            {
                html.Append(
                    ConvertListItem(listItem));
            }

            html.Append(
                $"</{tag}>");

            return html.ToString();
        }

        /// <summary>
        /// Converts one WPF ListItem.
        /// </summary>
        private static string ConvertListItem(
            ListItem listItem)
        {
            StringBuilder html =
                new StringBuilder();

            html.Append(
                "<li style=\"margin:0 0 6px 0;\">");

            foreach (Block block in listItem.Blocks)
            {
                /*
                 * Avoid placing a full <p> with large
                 * margins inside every list item.
                 */
                if (block is Paragraph paragraph)
                {
                    foreach (Inline inline in paragraph.Inlines)
                    {
                        html.Append(
                            ConvertInline(inline));
                    }
                }
                else
                {
                    html.Append(
                        ConvertBlock(block));
                }
            }

            html.Append("</li>");

            return html.ToString();
        }

        /// <summary>
        /// Determines whether a WPF list should become
        /// an OL or UL element.
        /// </summary>
        private static bool IsNumberedList(
            List list)
        {
            return list.MarkerStyle switch
            {
                TextMarkerStyle.Decimal => true,
                TextMarkerStyle.LowerLatin => true,
                TextMarkerStyle.UpperLatin => true,
                TextMarkerStyle.LowerRoman => true,
                TextMarkerStyle.UpperRoman => true,

                _ => false
            };
        }

        /// <summary>
        /// Converts an Inline element into HTML.
        /// </summary>
        private static string ConvertInline(
            Inline inline)
        {
            if (inline is LineBreak)
            {
                return "<br />";
            }

            if (inline is Run run)
            {
                string encodedText =
                    WebUtility.HtmlEncode(
                        run.Text ?? string.Empty);

                return ApplyFormatting(
                    run,
                    encodedText);
            }

            if (inline is Span span)
            {
                StringBuilder contents =
                    new StringBuilder();

                foreach (Inline child in span.Inlines)
                {
                    contents.Append(
                        ConvertInline(child));
                }

                return ApplyFormatting(
                    span,
                    contents.ToString());
            }

            return string.Empty;
        }

        /// <summary>
        /// Applies supported formatting tags to
        /// converted inline content.
        /// </summary>
        private static string ApplyFormatting(
            Inline inline,
            string html)
        {
            string result = html;

            if (IsBold(inline))
            {
                result =
                    $"<strong>{result}</strong>";
            }

            if (IsItalic(inline))
            {
                result =
                    $"<em>{result}</em>";
            }

            if (IsUnderlined(inline))
            {
                result =
                    $"<u>{result}</u>";
            }

            return result;
        }

        private static bool IsBold(
            Inline inline)
        {
            return inline.FontWeight ==
                   FontWeights.Bold;
        }

        private static bool IsItalic(
            Inline inline)
        {
            return inline.FontStyle ==
                   FontStyles.Italic;
        }

        private static bool IsUnderlined(
            Inline inline)
        {
            if (inline.TextDecorations == null)
            {
                return false;
            }

            return inline.TextDecorations.Any(
                decoration =>
                    decoration.Location ==
                    TextDecorationLocation.Underline);
        }
    }
}