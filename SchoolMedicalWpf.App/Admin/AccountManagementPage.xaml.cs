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
        private readonly User _currentUser;
        private ObservableCollection<User> _users = new();
        private ObservableCollection<Role> _roles = new();

        public AccountManagementPage(UserService userService, RoleService roleService, User currentUser)
        {
            InitializeComponent();
            _userService = userService;
            _roleService = roleService;
            _currentUser = currentUser;
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

            // Clear existing roles and set new ones
            _roles = new ObservableCollection<Role>(roles);
            FilterRoleComboBox.ItemsSource = _roles;
            FilterRoleComboBox.SelectedIndex = -1;

        }

        private async Task LoadUsers(int? roleId = null)
        {
            var users = await _userService.GetAllUsers();
            if (roleId.HasValue)
                users = users.Where(u => u.RoleId == roleId.Value && u.Status != false).ToList();
            else
                users = users.Where(u => u.Status != false).ToList();

            _users = new ObservableCollection<User>(users);

            AccountDataGrid.ItemsSource = null; // Reset the ItemsSource to refresh the DataGrid
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

        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            CreateUpdateAccount createUpdateAccount = new CreateUpdateAccount(_userService, _roleService);
            createUpdateAccount.ShowDialog();

            // Sau khi tạo tài khoản mới, tải lại danh sách người dùng
            await LoadUsers();
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            User? selected = AccountDataGrid.SelectedItem as User;
            if (selected == null)
            {
                MessageBox.Show("Vui lòng chọn tài khoản để sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            CreateUpdateAccount createUpdateAccount = new CreateUpdateAccount(_userService, _roleService, selected);
            createUpdateAccount.EditedUser = selected;
            createUpdateAccount.ShowDialog();
            await LoadUsers();
        }


        // Nút "Xóa tài khoản"
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (AccountDataGrid.SelectedItem is not User selectedUser)
            {
                MessageBox.Show("Vui lòng chọn tài khoản để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Bạn có chắc muốn xóa tài khoản '{selectedUser.FullName}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _userService.DeleteUser(selectedUser.UserId);
                    await LoadUsers();
                    MessageBox.Show("Xóa tài khoản thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
