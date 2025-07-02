using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Nurse
{
    public partial class NurseMainWindow : Window
    {
        private readonly User _currentUser;
        public NurseMainWindow(User currentUser)
        {
            InitializeComponent();
            LoadNurseHomePage();
            _currentUser = currentUser;
        }

        private void LoadNurseHomePage()
        {
            try
            {
                var homePage = new NurseHomePage();
                MainContent.Content = homePage;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải trang chủ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SidebarButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null)
            {
                string pageTag = button.Tag.ToString();
                LoadUserControl(pageTag);
            }
        }

        private void LoadUserControl(string controlName)
        {
            try
            {
                UserControl userControl = null;

                switch (controlName)
                {
                    case "Home":
                        userControl = new NurseHomePage();
                        break;
                    case "HealthSchedule":
                        userControl = new HealthSchedulePage();
                        break;
                    case "MedicalEvent":
                        userControl = ActivatorUtilities.CreateInstance<MedicalEventPage>(App.Services, _currentUser);
                        break;
                    case "MedicalRegistration":
                        userControl = ActivatorUtilities.CreateInstance<MedicalRegistrationPage>(App.Services, _currentUser);
                        break;
                    //case "Medicine":
                    //    userControl = new MedicalRegistrationPage();
                    //    break;
                    case "Profile":
                        userControl = ActivatorUtilities.CreateInstance<ProfilePage>(App.Services, _currentUser);
                        break;
                    default:
                        userControl = new NurseHomePage();
                        break;
                }

                MainContent.Content = userControl;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải trang {controlName}: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}