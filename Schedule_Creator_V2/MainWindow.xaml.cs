using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Schedule_Creator_V2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SemaphoreSlim _transitionLock = new(1, 1);

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void View_Email_List_Btn_Click(object sender, RoutedEventArgs e)
        {
            await SwitchContentAsync(new View_Email_List());
        }

        private async void View_Days_Off_Btn_Click(object sender, RoutedEventArgs e)
        {
            await SwitchContentAsync(new View_Days_Off());
        }

        private async void Add_Belay_Cert_Btn_Click(object sender, RoutedEventArgs e)
        {
            await SwitchContentAsync(new Add_Belay_Cert());
        }

        private async void Remove_Staff_Btn_Click(object sender, RoutedEventArgs e)
        {
            await SwitchContentAsync(new Remove_Staff());
        }

        private async void Remove_Days_Off_Btn_Click(object sender, RoutedEventArgs e)
        {
            await SwitchContentAsync(new Remove_Days_Off());
        }

        private async void Add_Days_Off_Btn_Click(object sender, RoutedEventArgs e)
        {
            await SwitchContentAsync(new Add_Days_Off());
        }

        private async void Home_Btn_Click(object sender, RoutedEventArgs e)
        {
            await SwitchContentAsync(null);
        }

        private async void Add_Staff_Btn_Click(object sender, RoutedEventArgs e)
        {
            await SwitchContentAsync(new Add_Staff());
        }

        private async void Add_Avail_Btn_Click(object sender, RoutedEventArgs e)
        {
            await SwitchContentAsync(new Add_Avail());
        }

        private async void Add_Collection_Btn_Click(object sender, RoutedEventArgs e)
        {
            await SwitchContentAsync(new Add_Job_Settings());
        }

        private async void Staff_Lookup_Btn_Click(object sender, RoutedEventArgs e)
        {
            await SwitchContentAsync(new View_Staff());
        }

        private async void Edit_Staff_Btn_Click(object sender, RoutedEventArgs e)
        {
            await SwitchContentAsync(new Edit_Staff());
        }

        private async Task SwitchContentAsync(Page? newPage)
        {
            await _transitionLock.WaitAsync();
            try
            {
                if (DisplayScreen.Content is FrameworkElement currentContent)
                {
                    await FadeAsync(currentContent, 1, 0);
                }

                DisplayScreen.Content = newPage;

                if (newPage is null)
                {
                    return;
                }

                newPage.Opacity = 0;

                if (!newPage.IsLoaded)
                {
                    await WaitForLoadedAsync(newPage);
                }

                await FadeAsync(newPage, 0, 1);
            }
            finally
            {
                _transitionLock.Release();
            }
        }

        private static Task WaitForLoadedAsync(FrameworkElement element)
        {
            if (element.IsLoaded)
            {
                return Task.CompletedTask;
            }

            var tcs = new TaskCompletionSource<object?>();
            RoutedEventHandler? handler = null;
            handler = (_, __) =>
            {
                element.Loaded -= handler;
                tcs.TrySetResult(null);
            };

            element.Loaded += handler;
            return tcs.Task;
        }

        private static Task FadeAsync(FrameworkElement element, double from, double to)
        {
            var tcs = new TaskCompletionSource<object?>();

            element.Opacity = from;

            var animation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromMilliseconds(250),
                FillBehavior = FillBehavior.Stop
            };

            animation.Completed += (_, __) =>
            {
                element.Opacity = to;
                tcs.TrySetResult(null);
            };

            element.BeginAnimation(UIElement.OpacityProperty, animation);

            return tcs.Task;
        }
    }
}

