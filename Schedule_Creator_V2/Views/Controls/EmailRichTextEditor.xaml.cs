using System.Windows.Controls;
using Schedule_Creator_V2.Services;

namespace Schedule_Creator_V2
{
    /// <summary>
    /// Interaction logic for EmailRichTextEditor.xaml
    /// </summary>
    public partial class EmailRichTextEditor : UserControl
    {
        public EmailRichTextEditor()
        {
            InitializeComponent();
        }

        public string Html
        {
            get
            {
                return "[[RICH_TEXT_HTML]]" +
                       RichTextHtmlConverter.ToHtml(
                           Editor.Document);
            }

            set
            {
                RichTextHtmlConverter.SetPlainText(
                    Editor.Document,
                    value);
            }
        }
    }
}
