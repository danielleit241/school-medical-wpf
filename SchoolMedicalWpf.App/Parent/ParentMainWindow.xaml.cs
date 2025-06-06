using System.Windows;
using System.Windows.Controls;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Parent
{
    /// <summary>
    /// Interaction logic for ParentMainWindow.xaml
    /// </summary>
    public partial class ParentMainWindow : Window
    {
        private User _currentUser;

        public ParentMainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            // Load homepage mặc định
            MainContent.Content = new ParentHomePage(_currentUser);
        }

        private void SidebarButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string tag = button?.Tag?.ToString();
            switch (tag)
            {
                case "Home":
                    MainContent.Content = new ParentHomePage(_currentUser);
                    break;
                case "Profile":
                    MainContent.Content = new ParentProfilePage(_currentUser);
                    break;
                //case "Medicine":
                //    MainContent.Content = new RegisterMedicinePage();
                //    break;
                case "Health":
                    MainContent.Content = new ParentHealthDeclarationPage();
                    break;
                //case "Exam":
                //    MainContent.Content = new HealthExamHistoryPage();
                //    break;
                //case "Notification":
                //    MainContent.Content = new NotificationPage();
                //    break;
                default:
                    MainContent.Content = new ParentHomePage(_currentUser);
                    break;
            }
        }

    }
}