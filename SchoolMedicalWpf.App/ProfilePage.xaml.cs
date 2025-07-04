using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.AspNetCore.Identity;
using Microsoft.Win32;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App
{
    /// <summary>
    /// Interaction logic for ParentProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : UserControl
    {
        private User _currentUser;
        private UserService _userService;

        public ProfilePage(UserService service, User user)
        {
            InitializeComponent();
            _currentUser = user;
            _userService = service;
            LoadUserProfile();
        }

        private void GoBack()
        {
            ViewPanel.Visibility = Visibility.Visible;
            EditPanel.Visibility = Visibility.Collapsed;
            ChangePasswordPanel.Visibility = Visibility.Collapsed;
        }

        private void LoadUserProfile()
        {
            if (_currentUser != null)
            {
                FullNameTextBlock.Text = _currentUser.FullName ?? "Vui lòng cập nhật";
                PhoneNumberTextBlock.Text = _currentUser.PhoneNumber;
                EmailTextBlock.Text = _currentUser.EmailAddress ?? "Vui lòng cập nhật";
                AddressTextBlock.Text = _currentUser.Address ?? "Vui lòng cập nhật";
                DoBTextBlock.Text = _currentUser.DayOfBirth?.ToString("dd/MM/yyyy") ?? "Vui lòng cập nhật";
                string avatarPath = string.IsNullOrWhiteSpace(_currentUser.AvatarUrl)
                        ? "pack://application:,,,/Resources/default-avatar.png"
                        : _currentUser.AvatarUrl;

                try
                {
                    AvatarUrl.ImageSource = new BitmapImage(new Uri(avatarPath, UriKind.RelativeOrAbsolute));
                }
                catch
                {
                    AvatarUrl.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/avatar.png"));
                }
            }
        }

        private void UpdateInformationButton_Click(object sender, RoutedEventArgs e)
        {
            ViewPanel.Visibility = Visibility.Collapsed;
            EditPanel.Visibility = Visibility.Visible;
            LoadUserEditProfile();
        }

        private void LoadUserEditProfile()
        {
            if (_currentUser != null)
            {
                EditName.Text = _currentUser.FullName ?? string.Empty;
                EditPhoneNumber.Text = _currentUser.PhoneNumber ?? string.Empty;
                EditPhoneNumber.IsEnabled = false;

                EditEmail.Text = _currentUser.EmailAddress ?? string.Empty;
                EditAddress.Text = _currentUser.Address ?? string.Empty;

                EditBirthDate.SelectedDate = _currentUser.DayOfBirth.HasValue
                    ? _currentUser.DayOfBirth.Value.ToDateTime(TimeOnly.MinValue)
                    : null;

                string avatarPath = string.IsNullOrWhiteSpace(_currentUser.AvatarUrl)
                        ? "pack://application:,,,/Resources/default-avatar.png"
                        : _currentUser.AvatarUrl;

                try
                {
                    EditAvatar.ImageSource = new BitmapImage(new Uri(avatarPath, UriKind.RelativeOrAbsolute));
                }
                catch
                {
                    EditAvatar.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/avatar.png"));
                }
            }
        }

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            ViewPanel.Visibility = Visibility.Collapsed;
            ChangePasswordPanel.Visibility = Visibility.Visible;
            PhoneNumberChangePassword.Text = _currentUser.PhoneNumber ?? string.Empty;
            PhoneNumberChangePassword.IsEnabled = false;
        }

        private async void SaveInformationButton_Click(object sender, RoutedEventArgs e)
        {
            var user = await _userService.GetUserById(_currentUser.UserId);
            if (user != null)
            {
                user.FullName = EditName.Text.Trim();
                user.EmailAddress = EditEmail.Text.Trim();
                user.Address = EditAddress.Text.Trim();
                user.DayOfBirth = EditBirthDate.SelectedDate.HasValue
                    ? DateOnly.FromDateTime(EditBirthDate.SelectedDate.Value)
                    : null; // Correctly handle nullable DateOnly

                // Cập nhật thông tin người dùng
                await _userService.UpdateUser(user);
                // Cập nhật lại thông tin hiện tại
                _currentUser = user;
                LoadUserProfile();
                GoBack();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        private async void ChangeAvatarButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedPath = SelectImageFromDisk();
            if (selectedPath != null)
            {
                string savedPath = SaveImageToAssets(selectedPath);
                if (savedPath != null)
                {
                    SetAvatarImage(savedPath);
                    _currentUser.AvatarUrl = savedPath;

                    await _userService.UpdateUser(_currentUser);
                }
            }
        }

        private string SaveImageToAssets(string sourcePath)
        {
            if (string.IsNullOrEmpty(sourcePath) || !File.Exists(sourcePath))
                return null!;

            // Lưu vào thư mục Assets cùng với file .exe khi chạy
            string assetsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
            Directory.CreateDirectory(assetsFolder);

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(sourcePath);
            string destPath = Path.Combine(assetsFolder, fileName);

            File.Copy(sourcePath, destPath, overwrite: true);
            return destPath; // Trả về đường dẫn ảnh đã lưu
        }


        private string SelectImageFromDisk()
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Chọn ảnh đại diện",
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            return null!;
        }


        private void SetAvatarImage(string imagePath)
        {
            if (File.Exists(imagePath))
                EditAvatar.ImageSource = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
        }

        private void CancelChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        private async void SaveChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            var user = await _userService.GetUserById(_currentUser.UserId);
            if (user != null)
            {
                string newPassword = NewPassword.Password.Trim();
                string confirmPassword = ConfirmedPassword.Password.Trim();
                if (newPassword != confirmPassword)
                {
                    MessageBox.Show("Mật khẩu mới và xác nhận mật khẩu không khớp.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                // Mã hóa mật khẩu mới
                user.PasswordHash = new PasswordHasher<User>().HashPassword(user, newPassword);
                // Cập nhật thông tin người dùng
                await _userService.UpdateUser(user);
                MessageBox.Show("Đổi mật khẩu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                GoBack();
            }
        }
    }
}
