using Schedule_Creator_V2.Models;

namespace Schedule_Creator_V2.ExtensionMethods
{
    internal static class AvailBuilderExtensions
    {
        public static void Reset(this AvailBuilder ab)
        {
            ab.checkBox.IsChecked = false;
            ab.label.Reset();
            ab.availTimes.Clear();
        }
    }
}
