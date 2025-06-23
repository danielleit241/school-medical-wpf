using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
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
            MainContent.Content = new ParentHomePage(_currentUser);
        }

        private void SidebarButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string tag = button?.Tag?.ToString()!;
            switch (tag)
            {
                case "Home":
                    MainContent.Content = ActivatorUtilities.CreateInstance<ParentHomePage>(App.Services, _currentUser);
                    break;
                case "Profile":
                    MainContent.Content = ActivatorUtilities.CreateInstance<ProfilePage>(App.Services, _currentUser);
                    break;
                case "Medicine":
                    MainContent.Content = new MedicalRegistrationHistoryPage();
                    break;
                case "Health":
                    MainContent.Content = ActivatorUtilities.CreateInstance<ParentHealthDeclarationPage>(App.Services, _currentUser); ;
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