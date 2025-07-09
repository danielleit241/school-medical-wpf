using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Parent
{
    /// <summary>
    /// Interaction logic for ParentHomePage.xaml
    /// </summary>
    public partial class ParentHomePage : UserControl
    {
        private User _currentUser;

        public ParentHomePage(User user)
        {
            InitializeComponent();
            _currentUser = user;

        }

        private void HealthDeclarationBorder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as ParentMainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = ActivatorUtilities.CreateInstance<ParentHealthDeclarationPage>(App.Services, _currentUser);
            }
        }

        private void HealthHistoryBorder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as ParentMainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = ActivatorUtilities.CreateInstance<HealthHistoryPage>(App.Services, _currentUser, mainWindow.MainContent);
            }
        }

        private void ProfileBorder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as ParentMainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = ActivatorUtilities.CreateInstance<ProfilePage>(App.Services, _currentUser);
            }
        }

        private void MedicalRegistrationBorder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as ParentMainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = ActivatorUtilities.CreateInstance<MedicalRegistrationHistoryPage>(App.Services, _currentUser);
            }
        }

        private void NotificationBorder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //var mainWindow = Window.GetWindow(this) as ParentMainWindow;
            //if (mainWindow != null)
            //{
            //    // Nếu có NotificationPage, bạn có thể truyền user hoặc không tùy constructor
            //    mainWindow.MainContent.Content = ActivatorUtilities.CreateInstance<NotificationPage>(App.Services, _currentUser);
            //}
        }
    }
}
