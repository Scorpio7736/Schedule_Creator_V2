using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Schedule_Creator_V2.ExtensionMethods
{
    internal static class LabelExtensions
    {

        public static void ErrorOut(this Label label)
        {
            label.Foreground = new SolidColorBrush(Colors.Red);
            label.FontWeight = FontWeights.Bold;
        }

        public static void Reset(this Label label)
        {
            label.Foreground = new SolidColorBrush(Colors.Black);
            label.FontWeight = FontWeights.Normal;
        }



    }
}
