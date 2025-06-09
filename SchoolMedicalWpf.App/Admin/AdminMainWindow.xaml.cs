using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Admin
{
    /// <summary>
    /// Interaction logic for AdminMainWindow.xaml
    /// </summary>
    /// 
    public partial class AdminMainWindow : Window
    {
        private User _currentUser;
        private UserService _userService;
        private RoleService _roleService;

        // Sử dụng DI để khởi tạo UserService và RoleService
        public AdminMainWindow(User user, UserService userService, RoleService roleService)
        {
            InitializeComponent();
            _currentUser = user;
            _userService = userService;
            _roleService = roleService;
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
                    MainContent.Content = new CampaignPage();
                    break;
                case "Account":
                    MainContent.Content = new AccountManagementPage(_userService, _roleService);
                    break;
                case "Student":
                    MainContent.Content = new StudentManagementPage();
                    break;
                case "Profile":
                    MainContent.Content = new ProfilePage(_currentUser, _userService);
                    break;
                default:
                    MainContent.Content = new AdminHomePage(_currentUser);
                    break;
            }
        }
    }

}
