using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Admin
{
    public partial class AccountManagementPage : UserControl
    {
        private readonly UserService _userService;
        private readonly RoleService _roleService;
        private ObservableCollection<User> _users = new();
        private ObservableCollection<Role> _roles = new();
        private User EditedUser;

        public AccountManagementPage(UserService userService, RoleService roleService)
        {
            InitializeComponent();
            _userService = userService;
            _roleService = roleService;
            LoadData();
        }

        private async void LoadData()
        {
            await LoadRoles();
            await LoadUsers();
        }

        private async Task LoadRoles()
        {
            var roles = await _roleService.GetAllRoles();

            if (roles == null || roles.Count == 0)
            {
                MessageBox.Show("No roles were found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _roles = new ObservableCollection<Role>(roles);
            RoleComboBox.ItemsSource = _roles;
            FilterRoleComboBox.ItemsSource = _roles;

            if (_roles.Count > 0)
                FilterRoleComboBox.SelectedIndex = 0;
            else
                FilterRoleComboBox.SelectedIndex = -1;
        }

        private async Task LoadUsers(int? roleId = null)
        {
            var users = await _userService.GetAllUsers();

            _users = new ObservableCollection<User>(users.Where(u => u.Status != false));
            AccountDataGrid.ItemsSource = _users;
        }

        private async void FilterRoleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilterRoleComboBox.SelectedItem is Role selectedRole)
                await LoadUsers(selectedRole.RoleId);
            else
                await LoadUsers();
        }

        private async void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterRoleComboBox.SelectedIndex = -1;
            await LoadUsers();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RoleComboBox.SelectedItem is not Role selectedRole)
                {
                    MessageBox.Show("Vui lòng chọn quyền cho tài khoản.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var user = new User
                {
                    FullName = FullNameTextBox.Text,
                    PhoneNumber = PhoneNumberTextBox.Text,
                    EmailAddress = EmailTextBox.Text,
                    DayOfBirth = DayOfBirthTextBox.SelectedDate.HasValue
                        ? DateOnly.FromDateTime(DayOfBirthTextBox.SelectedDate.Value)
                        : null,
                    Address = AddressTextBox.Text,
                    RoleId = selectedRole.RoleId,
                    Status = true
                };

                await _userService.AddUser(user);
                await LoadUsers();
                MessageBox.Show("Thêm tài khoản thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // Implement Edit logic if needed
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Implement Delete logic if needed
        }

        private void AccountDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AccountDataGrid.SelectedItem is User selectedUser)
            {
                FullNameTextBox.Text = selectedUser.FullName;
                PhoneNumberTextBox.Text = selectedUser.PhoneNumber;
                EmailTextBox.Text = selectedUser.EmailAddress;
                DayOfBirthTextBox.SelectedDate = selectedUser.DayOfBirth?.ToDateTime(TimeOnly.MinValue);
                AddressTextBox.Text = selectedUser.Address;
                RoleComboBox.SelectedItem = _roles.FirstOrDefault(r => r.RoleId == selectedUser.RoleId);
            }
        }
    }
}
