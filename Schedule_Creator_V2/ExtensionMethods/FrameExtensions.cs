using System.Windows.Controls;
using System.Drawing;
using System.Threading;

namespace Schedule_Creator_V2.ExtensionMethods
{
    internal static class FrameExtensions
    {
        public static void FadePageIn(this Frame frame, int fadeInTime, int fadeOutTime, Color fadeInColor)
        {
            frame.Content = fadeInColor;
            Thread.Sleep(fadeInTime);

        }
    }
}
