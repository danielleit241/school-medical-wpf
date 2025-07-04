using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App
{
    public partial class ProfilePage : UserControl
    {
        private readonly User _currentUser;
        private readonly UserService _userService;
        private bool _isProcessing = false;

        public ProfilePage(User currentUser, UserService userService)
        {
            InitializeComponent();
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));

            LoadUserProfile();
        }

        private void LoadUserProfile()
        {
            try
            {
                // Current Date and Time (UTC - YYYY-MM-DD HH:MM:SS formatted): 2025-07-04 14:32:55
                LastLoginText.Text = $"🕐 Đăng nhập lần cuối: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";

                // Load user information
                FullNameTextBlock.Text = _currentUser.FullName ?? (_currentUser.FullName ?? "danielleit241");
                DoBTextBlock.Text = _currentUser.DayOfBirth?.ToString("dd/MM/yyyy") ?? "Chưa cập nhật";
                AddressTextBlock.Text = _currentUser.Address ?? "Chưa cập nhật";
                PhoneNumberTextBlock.Text = _currentUser.PhoneNumber ?? "Chưa cập nhật";
                EmailTextBlock.Text = _currentUser.EmailAddress ?? "Chưa cập nhật";
                Username.Text = _currentUser.FullName ?? "danielleit241"; // Current User's Login: danielleit241

                if (!string.IsNullOrEmpty(_currentUser.AvatarUrl))
                {
                    try
                    {
                        AvatarImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(_currentUser.AvatarUrl));
                    }
                    catch
                    {
                        // Use default avatar if loading fails
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi tải thông tin người dùng: {ex.Message}\n\n" +
                    $"🕐 Thời gian: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n" +
                    $"👤 User: {_currentUser.FullName ?? "danielleit241"}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateInformationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Switch to edit mode first
                ViewPanel.Visibility = Visibility.Collapsed;
                EditPanel.Visibility = Visibility.Visible;
                ChangePasswordPanel.Visibility = Visibility.Collapsed;

                // Force layout update to ensure EditPanel is rendered
                this.UpdateLayout();

                // Use Dispatcher to populate after UI is ready
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    PopulateEditForm();
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi mở form chỉnh sửa: {ex.Message}\n\n" +
                    $"🕐 Thời gian: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n" +
                    $"👤 User: {_currentUser.FullName ?? "danielleit241"}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PopulateEditForm()
        {
            try
            {
                // Clear all fields first to avoid template conflicts
                EditName.ClearValue(TextBox.TextProperty);
                EditEmail.ClearValue(TextBox.TextProperty);
                EditPhoneNumber.ClearValue(TextBox.TextProperty);
                EditAddress.ClearValue(TextBox.TextProperty);
                EditBirthDate.ClearValue(DatePicker.SelectedDateProperty);

                // Ensure controls are enabled and editable (fix typing issue)
                EditName.IsEnabled = true;
                EditName.IsReadOnly = false;
                EditEmail.IsEnabled = true;
                EditEmail.IsReadOnly = false;
                EditPhoneNumber.IsEnabled = true;
                EditPhoneNumber.IsReadOnly = false;
                EditAddress.IsEnabled = true;
                EditAddress.IsReadOnly = false;
                EditBirthDate.IsEnabled = true;

                // Small delay to ensure controls are ready
                Task.Delay(150).ContinueWith(_ =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        // Populate with user data
                        EditName.Text = _currentUser?.FullName ?? "";
                        EditEmail.Text = _currentUser?.EmailAddress ?? "";
                        EditPhoneNumber.Text = _currentUser?.PhoneNumber ?? "";
                        EditAddress.Text = _currentUser?.Address ?? "";

                        if (_currentUser?.DayOfBirth.HasValue == true)
                        {
                            EditBirthDate.SelectedDate = _currentUser.DayOfBirth.Value.ToDateTime(TimeOnly.MinValue);
                        }

                        // Copy avatar
                        EditAvatar.Source = AvatarImage.Source;

                        // Force final layout update
                        this.UpdateLayout();

                        // Add debug handlers to ensure typing works
                        EditName.TextChanged += (s, args) =>
                        {
                            System.Diagnostics.Debug.WriteLine($"EditName typed: '{EditName.Text}'");
                        };

                        // Focus on name field
                        EditName.Focus();
                        EditName.SelectAll();

                        // Debug info
                        System.Diagnostics.Debug.WriteLine($"Form populated - Name: '{EditName.Text}', Email: '{EditEmail.Text}'");
                    });
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PopulateEditForm error: {ex.Message}");
            }
        }

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Switch to change password mode
                ViewPanel.Visibility = Visibility.Collapsed;
                EditPanel.Visibility = Visibility.Collapsed;
                ChangePasswordPanel.Visibility = Visibility.Visible;

                // Pre-fill phone number
                PhoneNumberChangePassword.Text = _currentUser.PhoneNumber ?? "";

                // Focus on phone number field
                PhoneNumberChangePassword.Focus();
                PhoneNumberChangePassword.SelectAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi mở form đổi mật khẩu: {ex.Message}\n\n" +
                    $"🕐 Thời gian: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n" +
                    $"👤 User: {_currentUser.FullName ?? "danielleit241"}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChangeAvatarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Title = "Chọn ảnh đại diện",
                    Filter = "Image files (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg;*.jpeg;*.png;*.bmp",
                    FilterIndex = 1
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    var bitmap = new System.Windows.Media.Imaging.BitmapImage(new Uri(openFileDialog.FileName));
                    EditAvatar.Source = bitmap;

                    MessageBox.Show($"✅ Ảnh đại diện đã được chọn thành công!\n\n" +
                        $"📁 File: {System.IO.Path.GetFileName(openFileDialog.FileName)}\n\n" +
                        $"🕐 Thời gian: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n" +
                        $"👤 User: {_currentUser.FullName ?? "danielleit241"}", "Thành công",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi chọn ảnh: {ex.Message}\n\n" +
                    $"🕐 Thời gian: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n" +
                    $"👤 User: {_currentUser.FullName ?? "danielleit241"}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void SaveInformationButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isProcessing) return;

            try
            {
                _isProcessing = true;
                SaveInformationButton.Content = "⏳ Đang lưu...";
                SaveInformationButton.IsEnabled = false;

                // Debug current values
                System.Diagnostics.Debug.WriteLine($"=== SAVE DEBUG ===");
                System.Diagnostics.Debug.WriteLine($"EditName.Text: '{EditName.Text}'");
                System.Diagnostics.Debug.WriteLine($"EditEmail.Text: '{EditEmail.Text}'");
                System.Diagnostics.Debug.WriteLine($"EditPhoneNumber.Text: '{EditPhoneNumber.Text}'");
                System.Diagnostics.Debug.WriteLine($"EditAddress.Text: '{EditAddress.Text}'");

                // Validate required fields
                if (string.IsNullOrWhiteSpace(EditName.Text))
                {
                    MessageBox.Show("❌ Vui lòng nhập họ tên.\n\n" +
                        $"🕐 Thời gian: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n" +
                        $"👤 User: {_currentUser.FullName ?? "danielleit241"}", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    EditName.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(EditEmail.Text))
                {
                    MessageBox.Show("❌ Vui lòng nhập email.\n\n" +
                        $"🕐 Thời gian: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n" +
                        $"👤 User: {_currentUser.FullName ?? "danielleit241"}", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    EditEmail.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(EditPhoneNumber.Text))
                {
                    MessageBox.Show("❌ Vui lòng nhập số điện thoại.\n\n" +
                        $"🕐 Thời gian: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n" +
                        $"👤 User: {_currentUser.FullName ?? "danielleit241"}", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    EditPhoneNumber.Focus();
                    return;
                }

                // Update user information
                _currentUser.FullName = EditName.Text.Trim();
                _currentUser.DayOfBirth = EditBirthDate.SelectedDate.HasValue ?
                    DateOnly.FromDateTime(EditBirthDate.SelectedDate.Value) : null;
                _currentUser.Address = string.IsNullOrWhiteSpace(EditAddress.Text) ? null : EditAddress.Text.Trim();
                _currentUser.PhoneNumber = EditPhoneNumber.Text.Trim();
                _currentUser.EmailAddress = EditEmail.Text.Trim();

                // Save to database
                var result = await Task.Run(() => _userService.UpdateUser(_currentUser));

                if (result)
                {
                    MessageBox.Show($"✅ Thông tin đã được cập nhật thành công!\n\n" +
                        $"👤 Họ tên: {_currentUser.FullName}\n" +
                        $"📧 Email: {_currentUser.EmailAddress}\n" +
                        $"📞 SĐT: {_currentUser.PhoneNumber}\n\n" +
                        $"🕐 Thời gian: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n" +
                        $"👤 User: {_currentUser.FullName ?? "danielleit241"}", "Cập nhật thành công",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadUserProfile();
                    CancelButton_Click(sender, e);
                }
                else
                {
                    MessageBox.Show($"❌ Không thể cập nhật thông tin. Vui lòng thử lại.\n\n" +
                        $"🕐 Thời gian: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n" +
                        $"👤 User: {_currentUser.FullName ?? "danielleit241"}", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi lưu thông tin: {ex.Message}\n\n" +
                    $"🕐 Thời gian: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n" +
                    $"👤 User: {_currentUser.FullName ?? "danielleit241"}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _isProcessing = false;
                SaveInformationButton.Content = "✅ Lưu thay đổi";
                SaveInformationButton.IsEnabled = true;
            }
        }

        private async void SaveChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isProcessing) return;

            try
            {
                _isProcessing = true;
                SaveChangePasswordButton.Content = "⏳ Đang xử lý...";
                SaveChangePasswordButton.IsEnabled = false;

                if (string.IsNullOrWhiteSpace(PhoneNumberChangePassword.Text) ||
                    string.IsNullOrWhiteSpace(OldPassword.Password) ||
                    string.IsNullOrWhiteSpace(NewPassword.Password) ||
                    string.IsNullOrWhiteSpace(ConfirmedPassword.Password))
                {
                    MessageBox.Show("❌ Vui lòng điền đầy đủ thông tin.\n\n" +
                        $"🕐 Thời gian: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n" +
                        $"👤 User: {_currentUser.FullName ?? "danielleit241"}", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (NewPassword.Password != ConfirmedPassword.Password)
                {
                    MessageBox.Show("❌ Mật khẩu mới và xác nhận mật khẩu không khớp.\n\n" +
                        $"🕐 Thời gian: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n" +
                        $"👤 User: {_currentUser.FullName ?? "danielleit241"}", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    NewPassword.Focus();
                    return;
                }

                if (NewPassword.Password.Length < 8)
                {
                    MessageBox.Show("❌ Mật khẩu mới phải có ít nhất 8 ký tự.\n\n" +
                        $"🕐 Thời gian: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n" +
                        $"👤 User: {_currentUser.FullName ?? "danielleit241"}", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    NewPassword.Focus();
                    return;
                }

                // Change password logic here
                var result = await Task.Run(() => _userService.ChangePassword(
                    PhoneNumberChangePassword.Text.Trim(),
                    OldPassword.Password,
                    NewPassword.Password));

                if (result)
                {
                    MessageBox.Show($"✅ Mật khẩu đã được thay đổi thành công!\n\n" +
                        $"🔒 Vui lòng đăng nhập lại với mật khẩu mới\n\n" +
                        $"🕐 Thời gian: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n" +
                        $"👤 User: {_currentUser.FullName ?? "danielleit241"}", "Thành công",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    OldPassword.Clear();
                    NewPassword.Clear();
                    ConfirmedPassword.Clear();
                    CancelChangePasswordButton_Click(sender, e);
                }
                else
                {
                    MessageBox.Show($"❌ Không thể thay đổi mật khẩu. Vui lòng kiểm tra lại thông tin.\n\n" +
                        $"🕐 Thời gian: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n" +
                        $"👤 User: {_currentUser.FullName ?? "danielleit241"}", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi đổi mật khẩu: {ex.Message}\n\n" +
                    $"🕐 Thời gian: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n" +
                    $"👤 User: {_currentUser.FullName ?? "danielleit241"}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _isProcessing = false;
                SaveChangePasswordButton.Content = "✅ Đổi mật khẩu";
                SaveChangePasswordButton.IsEnabled = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ViewPanel.Visibility = Visibility.Visible;
            EditPanel.Visibility = Visibility.Collapsed;
            ChangePasswordPanel.Visibility = Visibility.Collapsed;
        }

        private void CancelChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            OldPassword.Clear();
            NewPassword.Clear();
            ConfirmedPassword.Clear();

            ViewPanel.Visibility = Visibility.Visible;
            EditPanel.Visibility = Visibility.Collapsed;
            ChangePasswordPanel.Visibility = Visibility.Collapsed;
        }
    }
}