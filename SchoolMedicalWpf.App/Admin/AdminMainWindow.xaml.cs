using Microsoft.Extensions.DependencyInjection;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;
using System.Windows;
using System.Windows.Controls;

namespace SchoolMedicalWpf.App.Admin
{
    /// <summary>
    /// Interaction logic for AdminMainWindow.xaml
    /// </summary>
    public partial class AdminMainWindow : Window
    {
        private User _currentUser;
        private UserService _userService;
        private RoleService _roleService;
        private StudentService _studentService; // Add StudentService field

        // Sử dụng DI để khởi tạo UserService, RoleService, và StudentService
        public AdminMainWindow(User user, UserService userService, RoleService roleService, StudentService studentService)
        {
            InitializeComponent();
            _currentUser = user;
            _userService = userService;
            _roleService = roleService;
            _studentService = studentService; // Initialize StudentService
            MainContent.Content = new AdminHomePage(_currentUser); // Khởi tạo AdminHomePage mặc định
        }

        public void SidebarButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string tag = button?.Tag?.ToString()!;
            switch (tag)
            {
                case "Home":
                    MainContent.Content = new AdminHomePage(_currentUser);
                    break;
                case "Campaign":
                    MainContent.Content = ActivatorUtilities.CreateInstance<CampaignPage>(App.Services, _currentUser);
                    break;
                case "Account":
                    MainContent.Content = new AccountManagementPage(_userService, _roleService);
                    break;
                case "Student":
                    MainContent.Content = ActivatorUtilities.CreateInstance<StudentManagementPage>(App.Services, _currentUser);
                    break;
                case "Profile":
                    MainContent.Content = ActivatorUtilities.CreateInstance<ProfilePage>(App.Services, _currentUser);
                    break;
                case "Quit":
                    var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        Application.Current.Shutdown();
                    }
                    break;
                default:
                    MainContent.Content = new AdminHomePage(_currentUser);
                    break;
            }
        }
    }

}
