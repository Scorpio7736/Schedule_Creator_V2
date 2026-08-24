using Schedule_Creator_V2.Services.Database;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Schedule_Creator_V2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataMigragtion.EnsureDatabaseExists();

            ShowHome();
        }

        private void ShowHome()
        {
            DisplayScreen.Content = null;
            DisplayScreen.Visibility = Visibility.Collapsed;
            HomeDashboard.Visibility = Visibility.Visible;
        }

        private void ShowPage(Page page)
        {
            ArgumentNullException.ThrowIfNull(page);

            HomeDashboard.Visibility = Visibility.Collapsed;
            DisplayScreen.Visibility = Visibility.Visible;
            DisplayScreen.Content = page;
        }

        private void Home_Btn_Click(
            object sender,
            RoutedEventArgs e)
        {
            ShowHome();
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