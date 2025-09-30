using System.Windows;

namespace Schedule_Creator_V2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void View_Build_Schedule_Btn_Click(object sender, RoutedEventArgs e)
        {
            DisplayScreen.Content = new Build_Schedule();
        }

        private void View_Email_List_Btn_Click(object sender, RoutedEventArgs e)
        {
            DisplayScreen.Content = new View_Email_List();
        }

        private void View_Days_Off_Btn_Click(object sender, RoutedEventArgs e)
        {
            DisplayScreen.Content = new View_Days_Off();
        }

        private void Add_Belay_Cert_Btn_Click(object sender, RoutedEventArgs e)
        {
            DisplayScreen.Content = new Add_Belay_Cert();
        }

        private void Remove_Staff_Btn_Click(object sender, RoutedEventArgs e)
        {
            DisplayScreen.Content = new Remove_Staff();
        }

        private void View_Schedule_Btn_Click(object sender, RoutedEventArgs e)
        {
            DisplayScreen.Content = new View_Schedule();
        }
        private void Remove_Days_Off_Btn_Click(object sender, RoutedEventArgs e)
        {
            DisplayScreen.Content = new Remove_Days_Off();
        }

        private void Add_Days_Off_Btn_Click(object sender, RoutedEventArgs e)
        {
            DisplayScreen.Content = new Add_Days_Off();
        }
        private void Add_PFP_Btn_Click(object sender, RoutedEventArgs e)
        {
            DisplayScreen.Content = new Add_PFP();
        }
        private void Home_Btn_Click(object sender, RoutedEventArgs e)
        {
            DisplayScreen.Content = null;
        }
        private void Add_Staff_Btn_Click(object sender, RoutedEventArgs e)
        {
            DisplayScreen.Content = null;
            DisplayScreen.Content = new Add_Staff(); 
        }

        private void Add_Avail_Btn_Click(Object sender, RoutedEventArgs e)
        {
            DisplayScreen.Content = new Add_Avail();
        }
        private void Add_Collection_Btn_Click(object sender, RoutedEventArgs e)
        {
            DisplayScreen.Content = new Add_Job_Settings();
        }
        private void Staff_Lookup_Btn_Click(object sender, RoutedEventArgs e)
        {
            DisplayScreen.Content = null;
            DisplayScreen.Content = new View_Staff();
        }

        private void Edit_Staff_Btn_Click(object sender, RoutedEventArgs e)
        {
            DisplayScreen.Content = new Edit_Staff();
        }

    }
}