using System.Windows;
using System.Windows.Controls;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Parent
{
    /// <summary>
    /// Interaction logic for ParentMainWindow.xaml
    /// </summary>
    public partial class ParentMainWindow : Window
    {
        private User _currentUser;
        private UserService _userService;
        private StudentService _studentService;

        public ParentMainWindow(User user, UserService service, StudentService studentService)
        {
            InitializeComponent();
            _currentUser = user;
            _userService = service;
            // Load homepage mặc định
            MainContent.Content = new ParentHomePage(_currentUser);
            _studentService = studentService;
        }

        private void SidebarButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string tag = button?.Tag?.ToString()!;
            switch (tag)
            {
                case "Home":
                    MainContent.Content = new ParentHomePage(_currentUser);
                    break;
                case "Profile":
                    MainContent.Content = new ProfilePage(_currentUser, _userService);
                    break;
                case "Medicine":
                    MainContent.Content = new MedicalRegistrationHistoryPage();
                    break;
                case "Health":
                    MainContent.Content = new ParentHealthDeclarationPage(_currentUser, _studentService);
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