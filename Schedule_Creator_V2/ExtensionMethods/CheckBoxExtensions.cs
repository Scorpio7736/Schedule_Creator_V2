using System.Windows.Controls;

namespace Schedule_Creator_V2.ExtensionMethods
{
    internal static class CheckBoxExtensions
    {

        public static string GetTag(this CheckBox checkBox)
        {
            if (checkBox.Tag != null)
            {
                return checkBox.Tag.ToString();
            }
            return string.Empty;

        }

    }
}
