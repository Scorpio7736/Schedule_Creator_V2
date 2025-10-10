using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Schedule_Creator_V2.ExtensionMethods
{
    static class DataGridExtensions
    {

        public static int GetRowIndexFromButton(this DataGrid dataGrid, object sender)
        {

            if (sender is not DependencyObject source)
            {
                return -1;
            }

            var row = source.FindParent<DataGridRow>();
            return row?.GetIndex() ?? -1;
        }

        public static T? FindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            DependencyObject? parent = VisualTreeHelper.GetParent(child);

            while (parent is not null && parent is not T)
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as T;
        }
    }

}
