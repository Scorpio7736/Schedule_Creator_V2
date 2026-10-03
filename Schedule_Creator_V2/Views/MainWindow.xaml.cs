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
        }

        // =========================================================
        // PAGE NAVIGATION
        // =========================================================

        private void ShowPage(Page page)
        {
            ArgumentNullException.ThrowIfNull(page);

            HomeDashboard.Visibility = Visibility.Collapsed;
            DisplayScreen.Visibility = Visibility.Visible;
            DisplayScreen.Content = page;
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

        private void Home_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            ShowHome();

            PlayHomeAnimations();
        }

        private void Send_Email_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            ShowPage(
                new Send_Email());
        }

        private void Build_Schedule_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            ShowPage(
                new Build_Schedule());
        }

        private void View_Schedule_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            ShowPage(
                new View_Schedule());
        }

        private void View_Email_List_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            ShowPage(
                new View_Email_List());
        }

        private void View_Days_Off_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            ShowPage(
                new View_Days_Off());
        }

        private void Add_Belay_Cert_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            ShowPage(
                new Add_Belay_Cert());
        }

        private void Remove_Staff_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            ShowPage(
                new Remove_Staff());
        }

        private void Remove_Days_Off_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            ShowPage(
                new Remove_Days_Off());
        }

        private void Add_Days_Off_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            ShowPage(
                new Add_Days_Off());
        }

        private void Add_Staff_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            ShowPage(
                new Add_Staff());
        }

        private void Add_Avail_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            ShowPage(
                new Add_Avail());
        }

        private void Add_Collection_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            ShowPage(
                new Add_Job_Settings());
        }

        private void Staff_Lookup_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            ShowPage(
                new View_Staff());
        }

        private void Edit_Staff_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            ShowPage(
                new Edit_Staff());
        }
    }
}