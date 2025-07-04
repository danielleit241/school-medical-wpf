using System.Collections.ObjectModel;
using System.Windows;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Admin
{
    /// <summary>
    /// Interaction logic for CreateUpdateAccount.xaml
    /// </summary>
    public partial class CreateUpdateAccount : Window
    {
        public User EditedUser { get; set; } = null!;

        private readonly UserService _userService;
        private readonly RoleService _roleService;

        private ObservableCollection<Role> _roles = new();

        private bool ValidateForm()
        {
            // Họ tên
            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text))
            {
                MessageBox.Show("Vui lòng nhập họ tên.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                FullNameTextBox.Focus();
                return false;
            }

            // Số điện thoại
            if (string.IsNullOrWhiteSpace(PhoneNumberTextBox.Text))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                PhoneNumberTextBox.Focus();
                return false;
            }
            // Kiểm tra số điện thoại là số và có độ dài hợp lệ (ví dụ 9-11 số)
            if (!PhoneNumberTextBox.Text.All(char.IsDigit) || PhoneNumberTextBox.Text.Length < 9 || PhoneNumberTextBox.Text.Length > 11)
            {
                MessageBox.Show("Số điện thoại không hợp lệ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                PhoneNumberTextBox.Focus();
                return false;
            }

            // Email (có thể bỏ qua nếu không bắt buộc)
            if (!string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(EmailTextBox.Text);
                }
                catch
                {
                    MessageBox.Show("Email không hợp lệ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    EmailTextBox.Focus();
                    return false;
                }
            }

            // Quyền
            if (RoleComboBox.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn quyền cho tài khoản.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                RoleComboBox.Focus();
                return false;
            }

            return true;
        }

        public CreateUpdateAccount(UserService userService, RoleService roleService, User? editedUser = null)
        {
            _userService = userService;
            _roleService = roleService;
            EditedUser = editedUser!;
            InitializeComponent();

            // Xử lý loaded ở đây luôn
            this.Loaded += CreateUpdateAccount_Loaded;
        }


        private async void CreateUpdateAccount_Loaded(object sender, RoutedEventArgs e)
        {
            await FillRolesAsync();
            FillElement(EditedUser);

            if (EditedUser != null)
                DetailModelTextBlock.Text = "Cập nhật tài khoản";
            else
                DetailModelTextBlock.Text = "Tạo tài khoản mới";
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            User u = EditedUser ?? new User();
            u.FullName = FullNameTextBox.Text.Trim();
            u.PhoneNumber = PhoneNumberTextBox.Text.Trim();
            u.DayOfBirth = DayOfBirthTextBox.SelectedDate.HasValue ? DateOnly.FromDateTime(DayOfBirthTextBox.SelectedDate.Value) : null;
            u.EmailAddress = EmailTextBox.Text.Trim();
            u.Address = AddressTextBox.Text.Trim();
            u.Status = (StatusComboBox.SelectedIndex == 0);
            u.RoleId = (RoleComboBox.SelectedItem as Role)?.RoleId;

            if (EditedUser == null)
                await _userService.AddUser(u);
            else
                await _userService.UpdateUser(u);

            this.Close();
        }


        private async Task FillRolesAsync()
        {
            _roles.Clear();
            var roles = await _roleService.GetAllRoles();
            foreach (var role in roles)
            {
                _roles.Add(role);
            }
            RoleComboBox.ItemsSource = _roles;
            RoleComboBox.SelectedIndex = -1; // Đặt không chọn mặc định
            RoleComboBox.DisplayMemberPath = "RoleName"; // Hiển thị tên quyền trong ComboBox
            RoleComboBox.SelectedValuePath = "RoleId"; // Lưu ID quyền khi chọn
        }

        public void FillElement(User u)
        {
            if (u == null) return;

            FullNameTextBox.Text = u.FullName ?? string.Empty;
            PhoneNumberTextBox.Text = u.PhoneNumber ?? string.Empty;
            EmailTextBox.Text = u.EmailAddress ?? string.Empty;
            AddressTextBox.Text = u.Address ?? string.Empty;
            StatusComboBox.Text = u.Status.HasValue && u.Status.Value ? "Hoạt động" : "Không hoạt động";
            // Ngày sinh
            if (u.DayOfBirth.HasValue)
                DayOfBirthTextBox.SelectedDate = u.DayOfBirth.Value.ToDateTime(TimeOnly.MinValue);
            else
                DayOfBirthTextBox.SelectedDate = null;
            // Quyền
            if (u.RoleId.HasValue)
            {
                // Lúc này đã có _roles rồi nên sẽ set được
                RoleComboBox.SelectedItem = _roles.FirstOrDefault(r => r.RoleId == u.RoleId.Value);
            }
            else
            {
                RoleComboBox.SelectedIndex = -1;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
