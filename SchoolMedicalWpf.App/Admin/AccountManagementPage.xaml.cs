using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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

        public AccountManagementPage(UserService userService, RoleService roleService)
        {
            InitializeComponent();
            _userService = userService;
            _roleService = roleService;
            Loaded += AccountManagementPage_Loaded;
        }

        private async void AccountManagementPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadRoles();
            await LoadUsers();
        }

        private async Task LoadRoles()
        {
            try
            {
                // Lấy tất cả các role từ RoleService
                var roles = await _roleService.GetAllRoles();

                if (roles == null || roles.Count == 0)
                {
                    MessageBox.Show("No roles were found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _roles = new ObservableCollection<Role>(roles);

                // Gán roles cho ComboBox
                RoleComboBox.ItemsSource = _roles;
                FilterRoleComboBox.ItemsSource = _roles;

                // Đặt chỉ mục mặc định cho ComboBox lọc
                FilterRoleComboBox.SelectedIndex = 0; // Hoặc có thể để -1 để không chọn gì mặc định
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading roles: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async Task LoadUsers(int? roleId = null)
        {
            var users = roleId.HasValue
                ? await _userService.GetUsersByRoleId(roleId.Value) // Lọc người dùng theo RoleId
                : await _userService.GetAllUsers(); // Lấy tất cả người dùng

            // Chỉ hiển thị người dùng có trạng thái "active"
            _users = new ObservableCollection<User>(users.Where(u => u.Status != false));
            AccountDataGrid.ItemsSource = _users;
        }

        private async void FilterRoleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Kiểm tra nếu có role được chọn
            if (FilterRoleComboBox.SelectedItem is Role selectedRole)
            {
                // Nếu có role được chọn, lọc người dùng theo RoleId
                await LoadUsers(selectedRole.RoleId);
            }
            else
            {
                // Nếu không có role nào được chọn, tải tất cả người dùng
                await LoadUsers();
            }
        }

        private async void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            // Xóa lựa chọn trong ComboBox lọc và tải lại tất cả người dùng
            FilterRoleComboBox.SelectedIndex = -1;
            await LoadUsers();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRole = RoleComboBox.SelectedItem as Role;
                if (selectedRole == null)
                {
                    MessageBox.Show("Vui lòng chọn quyền cho tài khoản.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var user = new User
                {
                    FullName = FullNameTextBox.Text,
                    PhoneNumber = PhoneNumberTextBox.Text,
                    EmailAddress = EmailTextBox.Text,
                    DayOfBirth = DateOnly.TryParse(DayOfBirthTextBox.Text, out DateOnly parsedDate) ? parsedDate : null,
                    Address = AddressTextBox.Text,
                    RoleId = selectedRole.RoleId, // Lưu RoleId vào User
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
            // Implement Edit logic here if needed
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Implement Delete logic here if needed
        }

        private void AccountDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AccountDataGrid.SelectedItem is User selectedUser)
            {
                FullNameTextBox.Text = selectedUser.FullName;
                PhoneNumberTextBox.Text = selectedUser.PhoneNumber;
                EmailTextBox.Text = selectedUser.EmailAddress;
                DayOfBirthTextBox.Text = selectedUser.DayOfBirth?.ToString() ?? "";
                AddressTextBox.Text = selectedUser.Address;
                RoleComboBox.SelectedItem = _roles.FirstOrDefault(r => r.RoleId == selectedUser.RoleId); // Chọn role cho người dùng
            }
        }
    }
}