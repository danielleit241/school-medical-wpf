using Microsoft.Extensions.DependencyInjection;
using SchoolMedicalWpf.Dal.Entities;
using System.Windows;
using System.Windows.Controls;

namespace SchoolMedicalWpf.App.Nurse
{
    public partial class NurseMainWindow : Window
    {
        private readonly User _currentUser;
        public NurseMainWindow(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            LoadNurseHomePage();

        }

        private void LoadNurseHomePage()
        {
            try
            {
                var homePage = new NurseHomePage(_currentUser);
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
                string pageTag = button.Tag.ToString()!;
                LoadUserControl(pageTag);
            }
        }

        private void LoadUserControl(string controlName)
        {
            try
            {
                UserControl userControl = null!;

                switch (controlName)
                {
                    case "Home":
                        userControl = new NurseHomePage(_currentUser);
                        break;
                    case "HealthSchedule":
                        userControl = ActivatorUtilities.CreateInstance<HealthSchedulePage>(App.Services, _currentUser);
                        break;
                    case "MedicalEvent":
                        userControl = ActivatorUtilities.CreateInstance<MedicalEventPage>(App.Services, _currentUser);
                        break;
                    case "MedicalRegistration":
                        userControl = ActivatorUtilities.CreateInstance<MedicalRegistrationPage>(App.Services, _currentUser);
                        break;
                    case "Profile":
                        userControl = ActivatorUtilities.CreateInstance<ProfilePage>(App.Services, _currentUser);
                        break;
                    case "Quit":
                        var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            Application.Current.Shutdown();
                        }
                        break;
                    default:
                        userControl = new NurseHomePage(_currentUser);
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