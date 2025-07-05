using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Admin
{
    /// <summary>
    /// Interaction logic for AdminHomePage.xaml
    /// </summary>
    public partial class AdminHomePage : UserControl
    {

        private User _currentUser;
        public AdminHomePage(User user)
        {
            InitializeComponent();
            _currentUser = user;
        }

        private void VaccinationManagementBorder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as AdminMainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = ActivatorUtilities.CreateInstance<CampaignPage>(App.Services, _currentUser);
            }
        }
        private void UserManagementBorder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as AdminMainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = ActivatorUtilities.CreateInstance<AccountManagementPage>(App.Services, _currentUser);
            }
        }
        private void StudentManagementBorder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as AdminMainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = ActivatorUtilities.CreateInstance<StudentManagementPage>(App.Services, _currentUser);
            }
        }
        private void ProfileBorder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as AdminMainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = ActivatorUtilities.CreateInstance<ProfilePage>(App.Services, _currentUser);
            }
        }
    }
}
