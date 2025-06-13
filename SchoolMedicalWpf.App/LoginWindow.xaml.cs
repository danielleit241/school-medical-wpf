using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SchoolMedicalWpf.App.Parent;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly UserService _userService;
        private readonly RoleService _roleService;
        private readonly StudentService _studentService;
        private User _currentUser;
        // Inject UserService và RoleService qua constructor
        public LoginWindow(UserService userService, RoleService roleService, StudentService studentService)
        {
            InitializeComponent();
            _userService = userService;
            _roleService = roleService;
            _studentService = studentService;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string phoneNumber = PhoneNumberBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both phone number and password.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var user = await _userService.Authenticate(phoneNumber, password);
            if (user != null)
            {
                _currentUser = user; // Save the current user

                if (user.RoleId == 4)
                {
                    var prmw = new ParentMainWindow(_currentUser, _userService, _studentService);
                    prmw.Show();
                    this.Close();
                }
                else if (user.RoleId == 1)
                {
                    var amw = new Admin.AdminMainWindow(user, _userService, _roleService);  // Tiêm RoleService vào AdminMainWindow
                    amw.Show();
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Invalid phone number or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
