using System.Windows;
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
        private User _currentUser;

        // Inject UserService qua constructor
        public LoginWindow(UserService userService)
        {
            InitializeComponent();
            _userService = userService;
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

                if (user.RoleId == 4)
                {
                    _currentUser = user;
                    var prmw = new ParentMainWindow(_currentUser, _userService);
                    prmw.Show();
                    this.Close();
                }
                if(user.RoleId == 1)
                {
                    var amw = new Admin.AdminMainWindow(user, _userService);
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
