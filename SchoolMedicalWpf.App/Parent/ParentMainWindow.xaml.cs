using System.Windows;
using System.Windows.Controls;

namespace SchoolMedicalWpf.App.Parent
{
    /// <summary>
    /// Interaction logic for ParentMainWindow.xaml
    /// </summary>
    public partial class ParentMainWindow : Window
    {
        public ParentMainWindow()
        {
            InitializeComponent();
            // Load homepage mặc định
            MainContent.Content = new ParentHomePage();
        }

        private void SidebarButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string tag = button?.Tag?.ToString();
            switch (tag)
            {
                case "Home":
                    MainContent.Content = new ParentHomePage();
                    break;
                case "Profile":
                    MainContent.Content = new ParentProfilePage();
                    break;
                //case "Medicine":
                //    MainContent.Content = new RegisterMedicinePage();
                //    break;
                //case "Health":
                //    MainContent.Content = new StudentHealthPage();
                //    break;
                //case "Exam":
                //    MainContent.Content = new HealthExamHistoryPage();
                //    break;
                //case "Notification":
                //    MainContent.Content = new NotificationPage();
                //    break;
                default:
                    MainContent.Content = new ParentHomePage();
                    break;
            }
        }
    }
}