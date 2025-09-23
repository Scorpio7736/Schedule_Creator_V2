using Schedule_Creator_V2.Models;   // CoordanatePair
using System.Windows;               // Point
using System.Windows.Input;         // MouseEventArgs, IInputElement

namespace Schedule_Creator_V2.Animations
{
    internal class Fireworks
    {
        private readonly int NUMBER_OF_PARTICLES = 35;
        private readonly int FIREWORK_DIAMETER = 100;
        private int FIREWORK_COUNT;
        private const double OPACITY_HIDDEN = 0.0;
        private const double OPACITY_VISIBLE = 1.0;
        private readonly double SCREEN_WIDTH;
        private readonly double SCREEN_HEIGHT;
        private readonly double FIREWORK_DELAY;



        // host is the element you want coordinates relative to (Window, Canvas, etc.)
        private readonly FrameworkElement root;

        public Fireworks(FrameworkElement root, int fireworkCount)
        {
            this.root = root;
            FIREWORK_COUNT = fireworkCount;
        }
        private void Launch(object sender, MouseButtonEventArgs e)
        {
            Point local = e.GetPosition(root);
            Point screen = root.PointToScreen(local);
            CoordanatePair mouseLocation = new CoordanatePair((int)local.X, (int)local.Y);
            

        }
    }
}
