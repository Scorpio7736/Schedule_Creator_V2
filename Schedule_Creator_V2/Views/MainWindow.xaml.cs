using Schedule_Creator_V2.Services.Database;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Schedule_Creator_V2
{
    public partial class MainWindow : Window
    {
        private bool _isNavigating = false;

        public MainWindow()
        {
            InitializeComponent();

            DataMigragtion.EnsureDatabaseExists();

            ShowHome();

            Loaded += MainWindow_Loaded;
        }

        // =========================================================
        // HOME
        // =========================================================

        private void ShowHome()
        {
            DisplayScreen.Content = null;
            DisplayScreen.Visibility = Visibility.Collapsed;

            HomeDashboard.Visibility = Visibility.Visible;
            HomeDashboard.Opacity = 1;
        }

        // =========================================================
        // PAGE DISPLAY
        // =========================================================

        private void ShowPage(Page page)
        {
            ArgumentNullException.ThrowIfNull(page);

            HomeDashboard.Visibility = Visibility.Collapsed;

            DisplayScreen.Content = page;
            DisplayScreen.Opacity = 1;
            DisplayScreen.Visibility = Visibility.Visible;
        }

        // =========================================================
        // NAVIGATION
        // =========================================================

        private async Task NavigateToPageAsync(Page page)
        {
            ArgumentNullException.ThrowIfNull(page);

            if (_isNavigating)
            {
                return;
            }

            _isNavigating = true;

            try
            {
                await FadeOutCurrentPageAsync();

                ShowPage(page);
            }
            finally
            {
                _isNavigating = false;
            }
        }

        private async Task NavigateHomeAsync()
        {
            if (_isNavigating)
            {
                return;
            }

            _isNavigating = true;

            try
            {
                await FadeOutCurrentPageAsync();

                ShowHome();

                PlayHomeAnimations();
            }
            finally
            {
                _isNavigating = false;
            }
        }

        // =========================================================
        // PAGE FADE OUT
        // =========================================================

        private async Task FadeOutCurrentPageAsync()
        {
            FrameworkElement? currentPage = null;

            if (HomeDashboard.Visibility == Visibility.Visible)
            {
                currentPage = HomeDashboard;
            }
            else if (DisplayScreen.Visibility == Visibility.Visible)
            {
                currentPage = DisplayScreen;
            }

            if (currentPage == null)
            {
                return;
            }

            TaskCompletionSource<bool> animationFinished =
                new TaskCompletionSource<bool>();

            DoubleAnimation fadeOutAnimation =
                new DoubleAnimation
                {
                    From = currentPage.Opacity,
                    To = 0,

                    Duration =
                        TimeSpan.FromMilliseconds(450),

                    EasingFunction =
                        new QuadraticEase
                        {
                            EasingMode =
                                EasingMode.EaseIn
                        },

                    FillBehavior =
                        FillBehavior.HoldEnd
                };

            fadeOutAnimation.Completed +=
                (sender, e) =>
                {
                    animationFinished.TrySetResult(true);
                };

            currentPage.BeginAnimation(
                OpacityProperty,
                fadeOutAnimation);

            await animationFinished.Task;

            currentPage.BeginAnimation(
                OpacityProperty,
                null);

            currentPage.Opacity = 0;
        }

        // =========================================================
        // HOME ANIMATIONS
        // =========================================================

        private async void MainWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            // Wait until the full window is visibly rendered.
            await Task.Delay(400);

            PlayHomeAnimations();
        }

        private async void PlayHomeAnimations()
        {
            // =====================================================
            // RESET EXISTING ANIMATIONS
            // =====================================================

            WelcomeContent.BeginAnimation(
                OpacityProperty,
                null);

            WelcomeContentTransform.BeginAnimation(
                TranslateTransform.XProperty,
                null);

            WelcomeLogo.BeginAnimation(
                OpacityProperty,
                null);

            HomeLowerContent.BeginAnimation(
                OpacityProperty,
                null);


            // =====================================================
            // RESET STARTING VALUES
            // =====================================================

            HomeDashboard.Opacity = 1;

            WelcomeContent.Opacity = 0;
            WelcomeContentTransform.X = -60;

            WelcomeLogo.Opacity = 0;

            HomeLowerContent.Opacity = 0;


            // Let WPF render the reset state first.
            await Task.Delay(100);


            // =====================================================
            // WELCOME CONTENT FADE
            // =====================================================

            DoubleAnimation contentFadeAnimation =
                new DoubleAnimation
                {
                    From = 0,
                    To = 1,

                    Duration =
                        TimeSpan.FromMilliseconds(1100),

                    EasingFunction =
                        new QuadraticEase
                        {
                            EasingMode =
                                EasingMode.EaseOut
                        },

                    FillBehavior =
                        FillBehavior.HoldEnd
                };


            // =====================================================
            // WELCOME CONTENT SLIDE
            // =====================================================

            DoubleAnimation contentSlideAnimation =
                new DoubleAnimation
                {
                    From = -60,
                    To = 0,

                    Duration =
                        TimeSpan.FromMilliseconds(1300),

                    EasingFunction =
                        new CubicEase
                        {
                            EasingMode =
                                EasingMode.EaseOut
                        },

                    FillBehavior =
                        FillBehavior.HoldEnd
                };


            // =====================================================
            // LOGO FADE
            // =====================================================

            DoubleAnimation logoFadeAnimation =
                new DoubleAnimation
                {
                    From = 0,
                    To = 1,

                    BeginTime =
                        TimeSpan.FromMilliseconds(200),

                    Duration =
                        TimeSpan.FromMilliseconds(1200),

                    EasingFunction =
                        new QuadraticEase
                        {
                            EasingMode =
                                EasingMode.EaseOut
                        },

                    FillBehavior =
                        FillBehavior.HoldEnd
                };


            // =====================================================
            // LOWER DASHBOARD FADE
            // =====================================================

            DoubleAnimation lowerContentFadeAnimation =
                new DoubleAnimation
                {
                    From = 0,
                    To = 1,

                    Duration =
                        TimeSpan.FromMilliseconds(850),

                    EasingFunction =
                        new QuadraticEase
                        {
                            EasingMode =
                                EasingMode.EaseOut
                        },

                    FillBehavior =
                        FillBehavior.HoldEnd
                };


            // =====================================================
            // WHEN INTRO FINISHES
            // =====================================================

            contentSlideAnimation.Completed +=
                (sender, e) =>
                {
                    HomeLowerContent.BeginAnimation(
                        OpacityProperty,
                        lowerContentFadeAnimation);
                };


            // =====================================================
            // START INTRO ANIMATIONS
            // =====================================================

            WelcomeContent.BeginAnimation(
                OpacityProperty,
                contentFadeAnimation);

            WelcomeContentTransform.BeginAnimation(
                TranslateTransform.XProperty,
                contentSlideAnimation);

            WelcomeLogo.BeginAnimation(
                OpacityProperty,
                logoFadeAnimation);
        }

        // =========================================================
        // NAVIGATION BUTTONS
        // =========================================================

        private async void Home_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (HomeDashboard.Visibility == Visibility.Visible)
            {
                PlayHomeAnimations();
                return;
            }

            await NavigateHomeAsync();
        }

        private async void Send_Email_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            await NavigateToPageAsync(
                new Send_Email());
        }

        private async void Build_Schedule_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            await NavigateToPageAsync(
                new Build_Schedule());
        }

        private async void View_Schedule_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            await NavigateToPageAsync(
                new View_Schedule());
        }

        private async void View_Email_List_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            await NavigateToPageAsync(
                new View_Email_List());
        }

        private async void View_Days_Off_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            await NavigateToPageAsync(
                new View_Days_Off());
        }

        private async void Add_Belay_Cert_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            await NavigateToPageAsync(
                new Add_Belay_Cert());
        }

        private async void Remove_Staff_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            await NavigateToPageAsync(
                new Remove_Staff());
        }

        private async void Remove_Days_Off_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            await NavigateToPageAsync(
                new Remove_Days_Off());
        }

        private async void Add_Days_Off_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            await NavigateToPageAsync(
                new Add_Days_Off());
        }

        private async void Add_Staff_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            await NavigateToPageAsync(
                new Add_Staff());
        }

        private async void Add_Avail_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            await NavigateToPageAsync(
                new Add_Avail());
        }

        private async void Add_Collection_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            await NavigateToPageAsync(
                new Add_Job_Settings());
        }

        private async void Staff_Lookup_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            await NavigateToPageAsync(
                new View_Staff());
        }

        private async void Edit_Staff_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            await NavigateToPageAsync(
                new Edit_Staff());
        }
    }
}