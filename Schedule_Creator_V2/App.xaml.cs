using System.Configuration;
using System.Data;
using System.Windows;

namespace Schedule_Creator_V2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var splash = new SplashScreen("/Images/Logo.jpg");
            splash.Show(true);
            base.OnStartup(e);
        }
    }
}
