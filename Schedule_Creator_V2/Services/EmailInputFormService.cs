using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Interfaces;
using Schedule_Creator_V2.Models.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace Schedule_Creator_V2.Services
{
    public static class EmailInputFormService
    {
        private const string RichTextPrefix =
            "[[RICH_TEXT_HTML]]";

        private static readonly HashSet<string> RichTextPropertyNames =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase)
            {
                "EmailBody",
                "RequestBody",
                "AnnouncementsIntro",
                "AttachmentsIntro",
                "HeaderSubtitle",
                "SignatureClosing"
            };

        public static void BuildEmailInputControls(
            StackPanel container,
            EmailType emailType)
        {
            container.Children.Clear();

            if (emailType.inputs == null ||
                emailType.inputs.Count == 0)
            {
                ShowNoInputsConfiguredMessage(container);
                return;
            }

            foreach (IEmailInputs inputGroup in emailType.inputs)
            {
                Border inputGroupControl =
                    CreateInputGroup(inputGroup);

                container.Children.Add(inputGroupControl);
            }
        }

        public static void ApplyInputValues(
            StackPanel container)
        {
            foreach (Border border in
                     container.Children.OfType<Border>())
            {
                if (border.Child is not Panel groupPanel)
                {
                    continue;
                }

                foreach (FrameworkElement element in
                         EnumerateInputElements(groupPanel))
                {
                    if (element.Tag is not
                        EmailInputControlInfo controlInfo)
                    {
                        continue;
                    }

                    if (element is TextBox textBox)
                    {
                        SetPropertyValue(
                            controlInfo,
                            textBox.Text);

                        continue;
                    }

                    if (element is RichTextBox richTextBox)
                    {
                        string richTextValue =
                            GetRichTextValue(richTextBox);

                        SetPropertyValue(
                            controlInfo,
                            richTextValue);
                    }
                }
            }
        }

        public static void ShowNoEmailTypeSelectedMessage(
            StackPanel container)
        {
            ShowPlaceholderMessage(
                container,
                heading: "No Email Type Selected",
                message:
                "Select an email type to display its input fields.");
        }

        private static void ShowNoInputsConfiguredMessage(
            StackPanel container)
        {
            ShowPlaceholderMessage(
                container,
                heading: "No Inputs Configured",
                message:
                "This email type does not contain any input sections.");
        }

        private static Border CreateInputGroup(
            IEmailInputs inputGroup)
        {
            StackPanel groupPanel =
                new StackPanel();

            TextBlock groupHeading =
                new TextBlock
                {
                    Text = inputGroup.GetEmailTypeName(),
                    FontSize = 17,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = CreateBrush(31, 41, 55),
                    Margin = new Thickness(0, 0, 0, 12)
                };

            groupPanel.Children.Add(groupHeading);

            PropertyInfo[] properties = inputGroup
                .GetType()
                .GetProperties(
                    BindingFlags.Public |
                    BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType == typeof(string))
                {
                    AddStringInput(
                        groupPanel,
                        inputGroup,
                        property);
                }
                else if (property.PropertyType ==
                         typeof(List<string>))
                {
                    AddStringListInput(
                        groupPanel,
                        inputGroup,
                        property);
                }
            }

            return new Border
            {
                Margin = new Thickness(0, 0, 0, 16),
                Padding = new Thickness(16),
                Background = Brushes.White,
                BorderBrush = CreateBrush(209, 213, 219),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Child = groupPanel
            };
        }

        private static void AddStringInput(
            Panel parent,
            IEmailInputs inputGroup,
            PropertyInfo property)
        {
            if (ShouldUseRichTextEditor(property.Name))
            {
                AddRichTextInput(
                    parent,
                    inputGroup,
                    property);

                return;
            }

            bool isMultiline =
                ShouldUseMultilineTextBox(property.Name);

            TextBlock label =
                CreateLabel(property.Name);

            string currentValue =
                property.GetValue(inputGroup)?.ToString()
                ?? string.Empty;

            TextBox textBox =
                new TextBox
                {
                    Text = currentValue,
                    MinHeight = isMultiline ? 100 : 36,
                    Margin = new Thickness(0, 0, 0, 14),
                    Padding = new Thickness(8),
                    AcceptsReturn = isMultiline,

                    TextWrapping = isMultiline
                        ? TextWrapping.Wrap
                        : TextWrapping.NoWrap,

                    VerticalContentAlignment = isMultiline
                        ? VerticalAlignment.Top
                        : VerticalAlignment.Center,

                    VerticalScrollBarVisibility = isMultiline
                        ? ScrollBarVisibility.Auto
                        : ScrollBarVisibility.Disabled,

                    Tag = new EmailInputControlInfo(
                        inputGroup,
                        property)
                };

            parent.Children.Add(label);
            parent.Children.Add(textBox);
        }

        private static void AddRichTextInput(
            Panel parent,
            IEmailInputs inputGroup,
            PropertyInfo property)
        {
            TextBlock label =
                CreateLabel(property.Name);

            TextBlock instructions =
                new TextBlock
                {
                    Text =
                        "Select text and use the formatting toolbar.",
                    FontSize = 12,
                    Foreground = CreateBrush(107, 114, 128),
                    Margin = new Thickness(0, 0, 0, 5)
                };

            RichTextBox editor =
                new RichTextBox
                {
                    MinHeight = 130,
                    Padding = new Thickness(8),
                    BorderThickness = new Thickness(0),
                    AcceptsReturn = true,
                    VerticalScrollBarVisibility =
                        ScrollBarVisibility.Auto,
                    Background = Brushes.White,
                    Tag = new EmailInputControlInfo(
                        inputGroup,
                        property)
                };

            SpellCheck.SetIsEnabled(
                editor,
                true);

            string currentValue =
                property.GetValue(inputGroup)?.ToString()
                ?? string.Empty;

            LoadRichTextValue(
                editor,
                currentValue);

            StackPanel toolbar =
                new StackPanel
                {
                    Orientation =
                        Orientation.Horizontal
                };

            toolbar.Children.Add(
                CreateFormatButton(
                    CreateBoldButtonContent(),
                    "Bold",
                    EditingCommands.ToggleBold,
                    editor,
                    new Thickness(0, 0, 4, 0)));

            toolbar.Children.Add(
                CreateFormatButton(
                    CreateItalicButtonContent(),
                    "Italic",
                    EditingCommands.ToggleItalic,
                    editor,
                    new Thickness(0, 0, 4, 0)));

            toolbar.Children.Add(
                CreateFormatButton(
                    CreateUnderlineButtonContent(),
                    "Underline",
                    EditingCommands.ToggleUnderline,
                    editor,
                    new Thickness(0, 0, 12, 0)));

            toolbar.Children.Add(
                CreateFormatButton(
                    "• List",
                    "Bulleted List",
                    EditingCommands.ToggleBullets,
                    editor,
                    new Thickness(0, 0, 4, 0),
                    minWidth: 58));

            toolbar.Children.Add(
                CreateFormatButton(
                    "1. List",
                    "Numbered List",
                    EditingCommands.ToggleNumbering,
                    editor,
                    new Thickness(0),
                    minWidth: 64));

            Border toolbarBorder =
                new Border
                {
                    Padding = new Thickness(6),
                    Background =
                        CreateBrush(249, 250, 251),
                    BorderBrush =
                        CreateBrush(229, 231, 235),
                    BorderThickness =
                        new Thickness(0, 0, 0, 1),
                    Child = toolbar
                };

            Grid editorGrid =
                new Grid();

            editorGrid.RowDefinitions.Add(
                new RowDefinition
                {
                    Height = GridLength.Auto
                });

            editorGrid.RowDefinitions.Add(
                new RowDefinition
                {
                    Height =
                        new GridLength(
                            1,
                            GridUnitType.Star)
                });

            Grid.SetRow(
                toolbarBorder,
                0);

            Grid.SetRow(
                editor,
                1);

            editorGrid.Children.Add(toolbarBorder);
            editorGrid.Children.Add(editor);

            Border editorBorder =
                new Border
                {
                    Margin =
                        new Thickness(0, 0, 0, 14),
                    BorderBrush =
                        CreateBrush(209, 213, 219),
                    BorderThickness =
                        new Thickness(1),
                    CornerRadius =
                        new CornerRadius(4),
                    Background = Brushes.White,
                    ClipToBounds = true,
                    Child = editorGrid
                };

            parent.Children.Add(label);
            parent.Children.Add(instructions);
            parent.Children.Add(editorBorder);
        }

        private static Button CreateFormatButton(
            object content,
            string toolTip,
            ICommand command,
            RichTextBox commandTarget,
            Thickness margin,
            double minWidth = 32)
        {
            return new Button
            {
                Content = content,
                ToolTip = toolTip,
                Command = command,
                CommandTarget = commandTarget,
                Focusable = false,
                MinWidth = minWidth,
                Height = 28,
                Padding = new Thickness(7, 2, 7, 2),
                Margin = margin,
                VerticalContentAlignment =
                    VerticalAlignment.Center,
                HorizontalContentAlignment =
                    HorizontalAlignment.Center
            };
        }

        private static TextBlock CreateBoldButtonContent()
        {
            return new TextBlock
            {
                Text = "B",
                FontWeight = FontWeights.Bold
            };
        }

        private static TextBlock CreateItalicButtonContent()
        {
            return new TextBlock
            {
                Text = "I",
                FontStyle = FontStyles.Italic
            };
        }

        private static TextBlock CreateUnderlineButtonContent()
        {
            return new TextBlock
            {
                Text = "U",
                TextDecorations =
                    TextDecorations.Underline
            };
        }

        private static void AddStringListInput(
            Panel parent,
            IEmailInputs inputGroup,
            PropertyInfo property)
        {
            TextBlock label =
                CreateLabel(property.Name);

            TextBlock instructions =
                new TextBlock
                {
                    Text = "Enter one item per line.",
                    FontSize = 12,
                    Foreground =
                        CreateBrush(107, 114, 128),
                    Margin =
                        new Thickness(0, 0, 0, 5)
                };

            List<string> currentItems =
                property.GetValue(inputGroup)
                    as List<string>
                ?? new List<string>();

            TextBox textBox =
                new TextBox
                {
                    Text = string.Join(
                        Environment.NewLine,
                        currentItems),

                    MinHeight = 120,
                    Margin =
                        new Thickness(0, 0, 0, 14),
                    Padding = new Thickness(8),
                    AcceptsReturn = true,
                    TextWrapping = TextWrapping.Wrap,
                    VerticalContentAlignment =
                        VerticalAlignment.Top,
                    VerticalScrollBarVisibility =
                        ScrollBarVisibility.Auto,

                    Tag = new EmailInputControlInfo(
                        inputGroup,
                        property)
                };

            parent.Children.Add(label);
            parent.Children.Add(instructions);
            parent.Children.Add(textBox);
        }

        private static TextBlock CreateLabel(
            string propertyName)
        {
            return new TextBlock
            {
                Text = ConvertPropertyNameToLabel(
                    propertyName),

                FontWeight = FontWeights.SemiBold,
                Foreground =
                    CreateBrush(55, 65, 81),
                Margin =
                    new Thickness(0, 0, 0, 5)
            };
        }

        private static void ShowPlaceholderMessage(
            StackPanel container,
            string heading,
            string message)
        {
            container.Children.Clear();

            StackPanel messagePanel =
                new StackPanel
                {
                    HorizontalAlignment =
                        HorizontalAlignment.Center
                };

            TextBlock headingText =
                new TextBlock
                {
                    Text = heading,
                    FontSize = 18,
                    FontWeight =
                        FontWeights.SemiBold,
                    Foreground =
                        CreateBrush(75, 85, 99),
                    HorizontalAlignment =
                        HorizontalAlignment.Center
                };

            TextBlock messageText =
                new TextBlock
                {
                    Text = message,
                    Margin =
                        new Thickness(0, 8, 0, 0),
                    TextAlignment =
                        TextAlignment.Center,
                    TextWrapping =
                        TextWrapping.Wrap,
                    Foreground =
                        CreateBrush(107, 114, 128),
                    HorizontalAlignment =
                        HorizontalAlignment.Center
                };

            messagePanel.Children.Add(headingText);
            messagePanel.Children.Add(messageText);

            Border placeholder =
                new Border
                {
                    Padding = new Thickness(30),
                    HorizontalAlignment =
                        HorizontalAlignment.Stretch,
                    Background = Brushes.White,
                    BorderBrush =
                        CreateBrush(209, 213, 219),
                    BorderThickness =
                        new Thickness(1),
                    CornerRadius =
                        new CornerRadius(6),
                    Child = messagePanel
                };

            container.Children.Add(placeholder);
        }

        private static void SetPropertyValue(
            EmailInputControlInfo controlInfo,
            string text)
        {
            if (!controlInfo.Property.CanWrite)
            {
                return;
            }

            if (controlInfo.Property.PropertyType ==
                typeof(string))
            {
                controlInfo.Property.SetValue(
                    controlInfo.InputGroup,
                    text);

                return;
            }

            if (controlInfo.Property.PropertyType ==
                typeof(List<string>))
            {
                List<string> values = text
                    .Replace("\r\n", "\n")
                    .Split(
                        '\n',
                        StringSplitOptions.RemoveEmptyEntries)
                    .Select(value => value.Trim())
                    .Where(value =>
                        !string.IsNullOrWhiteSpace(value))
                    .ToList();

                controlInfo.Property.SetValue(
                    controlInfo.InputGroup,
                    values);
            }
        }

        private static IEnumerable<FrameworkElement>
            EnumerateInputElements(
                DependencyObject root)
        {
            if (root is FrameworkElement element)
            {
                yield return element;
            }

            if (root is Panel panel)
            {
                foreach (UIElement child in panel.Children)
                {
                    foreach (FrameworkElement descendant in
                             EnumerateInputElements(child))
                    {
                        yield return descendant;
                    }
                }

                yield break;
            }

            if (root is Border border &&
                border.Child is not null)
            {
                foreach (FrameworkElement descendant in
                         EnumerateInputElements(
                             border.Child))
                {
                    yield return descendant;
                }
            }
        }

        private static bool ShouldUseRichTextEditor(
            string propertyName)
        {
            return RichTextPropertyNames.Contains(
                propertyName);
        }

        private static string GetRichTextValue(
            RichTextBox editor)
        {
            TextRange textRange =
                new TextRange(
                    editor.Document.ContentStart,
                    editor.Document.ContentEnd);

            if (string.IsNullOrWhiteSpace(
                    textRange.Text))
            {
                return string.Empty;
            }

            return RichTextPrefix +
                   SerializeFlowDocument(
                       editor.Document);
        }

        private static string SerializeFlowDocument(
            FlowDocument document)
        {
            StringBuilder html =
                new StringBuilder();

            foreach (Block block in document.Blocks)
            {
                html.Append(
                    SerializeBlock(block));
            }

            return html.ToString();
        }

        private static string SerializeBlock(
            Block block)
        {
            if (block is Paragraph paragraph)
            {
                string content =
                    string.Concat(
                        paragraph.Inlines
                            .Select(SerializeInline));

                if (string.IsNullOrEmpty(content))
                {
                    content = "<br>";
                }

                content =
                    ApplyTextElementFormatting(
                        content,
                        paragraph);

                return
                    $"<p>{content}</p>";
            }

            if (block is
                System.Windows.Documents.List list)
            {
                string tag =
                    IsOrderedList(list)
                        ? "ol"
                        : "ul";

                StringBuilder html =
                    new StringBuilder();

                html.Append('<')
                    .Append(tag)
                    .Append('>');

                foreach (ListItem listItem in
                         list.ListItems)
                {
                    html.Append("<li>");

                    foreach (Block itemBlock in
                             listItem.Blocks)
                    {
                        html.Append(
                            SerializeBlock(itemBlock));
                    }

                    html.Append("</li>");
                }

                html.Append("</")
                    .Append(tag)
                    .Append('>');

                return html.ToString();
            }

            return string.Empty;
        }

        private static string SerializeInline(
            Inline inline)
        {
            if (inline is LineBreak)
            {
                return "<br>";
            }

            if (inline is Run run)
            {
                string content =
                    WebUtility.HtmlEncode(
                        run.Text);

                return ApplyTextElementFormatting(
                    content,
                    run);
            }

            if (inline is Span span)
            {
                string content =
                    string.Concat(
                        span.Inlines
                            .Select(SerializeInline));

                return ApplyTextElementFormatting(
                    content,
                    span);
            }

            return string.Empty;
        }

        private static string ApplyTextElementFormatting(
            string content,
            TextElement element)
        {
            if (element.FontWeight.ToOpenTypeWeight() >=
                FontWeights.Bold.ToOpenTypeWeight())
            {
                content =
                    $"<strong>{content}</strong>";
            }

            if (element.FontStyle ==
                FontStyles.Italic)
            {
                content =
                    $"<em>{content}</em>";
            }

            if (element is Inline inline &&
                HasUnderline(inline))
            {
                content =
                    $"<u>{content}</u>";
            }

            return content;
        }

        private static bool HasUnderline(
            Inline inline)
        {
            if (inline.TextDecorations is null)
            {
                return false;
            }

            return inline.TextDecorations.Any(
                decoration =>
                    decoration.Location ==
                    TextDecorationLocation.Underline);
        }

        private static bool IsOrderedList(
            System.Windows.Documents.List list)
        {
            return list.MarkerStyle ==
                   TextMarkerStyle.Decimal ||
                   list.MarkerStyle ==
                   TextMarkerStyle.LowerLatin ||
                   list.MarkerStyle ==
                   TextMarkerStyle.UpperLatin ||
                   list.MarkerStyle ==
                   TextMarkerStyle.LowerRoman ||
                   list.MarkerStyle ==
                   TextMarkerStyle.UpperRoman;
        }

        private static void LoadRichTextValue(
            RichTextBox editor,
            string value)
        {
            editor.Document.Blocks.Clear();

            if (string.IsNullOrWhiteSpace(value))
            {
                editor.Document.Blocks.Add(
                    new Paragraph());

                return;
            }

            if (!value.StartsWith(
                    RichTextPrefix,
                    StringComparison.Ordinal))
            {
                LoadPlainText(
                    editor.Document,
                    value);

                return;
            }

            string html =
                value.Substring(
                    RichTextPrefix.Length);

            try
            {
                LoadHtml(
                    editor.Document,
                    html);
            }
            catch
            {
                LoadPlainText(
                    editor.Document,
                    html);
            }

            if (editor.Document.Blocks.Count == 0)
            {
                editor.Document.Blocks.Add(
                    new Paragraph());
            }
        }

        private static void LoadPlainText(
            FlowDocument document,
            string value)
        {
            document.Blocks.Clear();

            string normalizedValue =
                value
                    .Replace(
                        "\r\n",
                        "\n",
                        StringComparison.Ordinal);

            string[] paragraphs =
                normalizedValue.Split(
                    "\n\n",
                    StringSplitOptions.None);

            foreach (string paragraphText in
                     paragraphs)
            {
                Paragraph paragraph =
                    new Paragraph();

                string[] lines =
                    paragraphText.Split(
                        '\n');

                for (int index = 0;
                     index < lines.Length;
                     index++)
                {
                    if (index > 0)
                    {
                        paragraph.Inlines.Add(
                            new LineBreak());
                    }

                    paragraph.Inlines.Add(
                        new Run(lines[index]));
                }

                document.Blocks.Add(paragraph);
            }

            if (document.Blocks.Count == 0)
            {
                document.Blocks.Add(
                    new Paragraph());
            }
        }

        private static void LoadHtml(
            FlowDocument document,
            string html)
        {
            document.Blocks.Clear();

            XElement root =
                XElement.Parse(
                    $"<root>{html}</root>",
                    LoadOptions.PreserveWhitespace);

            foreach (XNode node in root.Nodes())
            {
                if (node is XElement element)
                {
                    Block? block =
                        CreateBlockFromHtmlElement(
                            element);

                    if (block is not null)
                    {
                        document.Blocks.Add(block);
                    }

                    continue;
                }

                if (node is XText text &&
                    !string.IsNullOrWhiteSpace(
                        text.Value))
                {
                    document.Blocks.Add(
                        new Paragraph(
                            new Run(text.Value)));
                }
            }
        }

        private static Block? CreateBlockFromHtmlElement(
            XElement element)
        {
            string tag =
                element.Name.LocalName
                    .ToLowerInvariant();

            if (tag == "p")
            {
                return CreateParagraph(
                    element.Nodes());
            }

            if (tag == "ul" ||
                tag == "ol")
            {
                return CreateList(
                    element,
                    ordered: tag == "ol");
            }

            Paragraph fallbackParagraph =
                CreateParagraph(
                    element.Nodes());

            return fallbackParagraph;
        }

        private static Paragraph CreateParagraph(
            IEnumerable<XNode> nodes)
        {
            Paragraph paragraph =
                new Paragraph();

            foreach (XNode node in nodes)
            {
                foreach (Inline inline in
                         CreateInlinesFromHtmlNode(
                             node))
                {
                    paragraph.Inlines.Add(inline);
                }
            }

            return paragraph;
        }

        private static System.Windows.Documents.List
            CreateList(
                XElement element,
                bool ordered)
        {
            System.Windows.Documents.List list =
                new System.Windows.Documents.List
                {
                    MarkerStyle = ordered
                        ? TextMarkerStyle.Decimal
                        : TextMarkerStyle.Disc
                };

            foreach (XElement itemElement in
                     element.Elements()
                         .Where(currentElement =>
                             string.Equals(
                                 currentElement.Name.LocalName,
                                 "li",
                                 StringComparison.OrdinalIgnoreCase)))
            {
                ListItem listItem =
                    new ListItem();

                List<XElement> blockElements =
                    itemElement.Elements()
                        .Where(currentElement =>
                            IsBlockHtmlElement(
                                currentElement.Name
                                    .LocalName))
                        .ToList();

                if (blockElements.Count > 0)
                {
                    foreach (XElement blockElement in
                             blockElements)
                    {
                        Block? block =
                            CreateBlockFromHtmlElement(
                                blockElement);

                        if (block is not null)
                        {
                            listItem.Blocks.Add(block);
                        }
                    }
                }
                else
                {
                    listItem.Blocks.Add(
                        CreateParagraph(
                            itemElement.Nodes()));
                }

                if (listItem.Blocks.Count == 0)
                {
                    listItem.Blocks.Add(
                        new Paragraph());
                }

                list.ListItems.Add(listItem);
            }

            return list;
        }

        private static IEnumerable<Inline>
            CreateInlinesFromHtmlNode(
                XNode node)
        {
            if (node is XText text)
            {
                yield return
                    new Run(text.Value);

                yield break;
            }

            if (node is not XElement element)
            {
                yield break;
            }

            string tag =
                element.Name.LocalName
                    .ToLowerInvariant();

            if (tag == "br")
            {
                yield return new LineBreak();
                yield break;
            }

            Span span =
                new Span();

            if (tag == "strong" ||
                tag == "b")
            {
                span.FontWeight =
                    FontWeights.Bold;
            }

            if (tag == "em" ||
                tag == "i")
            {
                span.FontStyle =
                    FontStyles.Italic;
            }

            if (tag == "u")
            {
                span.TextDecorations =
                    TextDecorations.Underline;
            }

            foreach (XNode childNode in
                     element.Nodes())
            {
                foreach (Inline childInline in
                         CreateInlinesFromHtmlNode(
                             childNode))
                {
                    span.Inlines.Add(childInline);
                }
            }

            yield return span;
        }

        private static bool IsBlockHtmlElement(
            string tagName)
        {
            return tagName.Equals(
                       "p",
                       StringComparison.OrdinalIgnoreCase) ||
                   tagName.Equals(
                       "ul",
                       StringComparison.OrdinalIgnoreCase) ||
                   tagName.Equals(
                       "ol",
                       StringComparison.OrdinalIgnoreCase);
        }

        private static string ConvertPropertyNameToLabel(
            string propertyName)
        {
            if (string.IsNullOrWhiteSpace(
                    propertyName))
            {
                return string.Empty;
            }

            string label =
                Regex.Replace(
                    propertyName,
                    "([a-z0-9])([A-Z])",
                    "$1 $2");

            return label.Replace(
                "_",
                " ");
        }

        private static bool ShouldUseMultilineTextBox(
            string propertyName)
        {
            string[] multilinePropertyWords =
            {
                "Body",
                "Intro",
                "Text",
                "Subtitle",
                "Message"
            };

            return multilinePropertyWords.Any(word =>
                propertyName.Contains(
                    word,
                    StringComparison.OrdinalIgnoreCase));
        }

        private static SolidColorBrush CreateBrush(
            byte red,
            byte green,
            byte blue)
        {
            return new SolidColorBrush(
                Color.FromRgb(
                    red,
                    green,
                    blue));
        }

        private sealed record EmailInputControlInfo(
            IEmailInputs InputGroup,
            PropertyInfo Property
        );
    }
}