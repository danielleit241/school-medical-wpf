using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SchoolMedicalWpf.App.Admin
{
    public partial class AccountManagementPage : UserControl
    {
        private readonly UserService _userService;
        private ObservableCollection<User> _users = new();
        private ObservableCollection<Role> _roles = new();

        public AccountManagementPage(UserService userService)
        {
            InitializeComponent();
            _userService = userService;

            Loaded += AccountManagementPage_Loaded;
            FilterRoleComboBox.SelectionChanged += FilterRoleComboBox_SelectionChanged;
        }

        private async void AccountManagementPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadRoles();
            await LoadUsers();
        }

        private async Task LoadRoles()
        {
            var roles = await _userService.GetAllRoles();
            _roles = new ObservableCollection<Role>(roles);
            RoleComboBox.ItemsSource = _roles;
            FilterRoleComboBox.ItemsSource = _roles;
            FilterRoleComboBox.SelectedIndex = 0;
        }

        private async Task LoadUsers(int? roleId = null)
        {
            var users = roleId.HasValue
                ? await _userService.GetUsersByRoleId(roleId.Value)
                : await _userService.GetAllUsers();

            // Chỉ lấy user còn hoạt động
            _users = new ObservableCollection<User>(users.Where(u => u.Status != false));
            AccountDataGrid.ItemsSource = _users;
        }

        private async void FilterRoleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Nếu không có quyền nào được chọn, tải lại toàn bộ người dùng
            if (FilterRoleComboBox.SelectedIndex == -1)
            {
                await LoadUsers();
            }
            else if (FilterRoleComboBox.SelectedItem is Role selectedRole)
            {
                // Nếu có quyền được chọn, tải lại người dùng theo quyền
                await LoadUsers(selectedRole.RoleId);
            }
        }
        private async void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            // Đặt lại giá trị ComboBox thành null hoặc giá trị mặc định để xóa quyền đang chọn
            FilterRoleComboBox.SelectedIndex = -1; // Hoặc có thể gán null nếu cần

            // Tải lại toàn bộ người dùng mà không lọc theo quyền
            await LoadUsers();
        }

        private void AccountDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AccountDataGrid.SelectedItem is User user)
            {
                FullNameTextBox.Text = user.FullName;
                PhoneNumberTextBox.Text = user.PhoneNumber;
                EmailTextBox.Text = user.EmailAddress;
                RoleComboBox.SelectedItem = _roles.FirstOrDefault(r => r.RoleId == user.RoleId);
            }
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
            if (AccountDataGrid.SelectedItem is not User selectedUser)
            {
                MessageBox.Show("Vui lòng chọn tài khoản để sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var selectedRole = RoleComboBox.SelectedItem as Role;
                if (selectedRole == null)
                {
                    MessageBox.Show("Vui lòng chọn quyền cho tài khoản.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                selectedUser.FullName = FullNameTextBox.Text;
                selectedUser.PhoneNumber = PhoneNumberTextBox.Text;
                selectedUser.EmailAddress = EmailTextBox.Text;
                selectedUser.RoleId = selectedRole.RoleId;

                await _userService.UpdateUser(selectedUser);
                await LoadUsers();
                MessageBox.Show("Cập nhật tài khoản thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (AccountDataGrid.SelectedItem is not User selectedUser)
            {
                MessageBox.Show("Vui lòng chọn tài khoản để cập nhật trạng thái.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra nếu tài khoản đã bị vô hiệu hóa rồi
            if (selectedUser.Status == false)
            {
                MessageBox.Show("Tài khoản này đã bị vô hiệu hóa trước đó.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (MessageBox.Show("Bạn chắc chắn muốn vô hiệu hóa tài khoản này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    // Gọi phương thức DeleteUser trong UserService để cập nhật trạng thái của người dùng thành false (vô hiệu hóa tài khoản)
                    await _userService.DeleteUser(selectedUser.UserId);
                    await LoadUsers(); // Tải lại danh sách người dùng
                    MessageBox.Show("Tài khoản đã được vô hiệu hóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
