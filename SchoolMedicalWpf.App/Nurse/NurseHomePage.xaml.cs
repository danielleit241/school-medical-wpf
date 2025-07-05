using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Nurse
{
    /// <summary>
    /// Interaction logic for NurseHomePage.xaml
    /// </summary>
    public partial class NurseHomePage : UserControl
    {
        private User _currentUser;
        public NurseHomePage(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
        }

        private void ScheduleBorder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as NurseMainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = ActivatorUtilities.CreateInstance<HealthSchedulePage>(App.Services, _currentUser);
            }
        }

        private void MedicalRegistrationBorder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as NurseMainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = ActivatorUtilities.CreateInstance<MedicalRegistrationPage>(App.Services, _currentUser);
            }
        }

        private void MedicalEventsBorder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as NurseMainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = ActivatorUtilities.CreateInstance<MedicalEventPage>(App.Services, _currentUser);
            }
        }

        private void ProfileBorder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as NurseMainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContent.Content = ActivatorUtilities.CreateInstance<ProfilePage>(App.Services, _currentUser);
            }
        }
    }
}
