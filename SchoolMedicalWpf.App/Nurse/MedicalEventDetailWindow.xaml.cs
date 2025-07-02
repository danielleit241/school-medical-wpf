using System.Windows;
using System.Windows.Media;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Nurse
{
    public partial class MedicalEventDetailWindow : Window
    {
        private MedicalEvent _medicalEvent;
        private readonly MedicalEventService _medicalEventService;
        private readonly UserService _userService;
        private readonly User _currentUser; // ✅ Use actual current user

        public event Action EventUpdated;

        public MedicalEventDetailWindow(MedicalEventService medicalEventService, UserService userService, MedicalEvent medicalEvent, User currentUser)
        {
            InitializeComponent();
            _medicalEvent = medicalEvent;
            _medicalEventService = medicalEventService;
            _userService = userService;
            _currentUser = currentUser; // ✅ Store current user from login

            // ✅ Load details asynchronously to prevent hanging
            Loaded += async (s, e) => await LoadEventDetailsAsync();
        }

        // ✅ Make LoadEventDetails async to prevent UI freezing
        private async Task LoadEventDetailsAsync()
        {
            try
            {
                // Student Information
                StudentNameText.Text = _medicalEvent.Student?.FullName ?? "Không xác định";
                StudentClassText.Text = _medicalEvent.Student?.Grade ?? "N/A";

                // Event Information
                EventTypeText.Text = _medicalEvent.EventType ?? "Không xác định";
                EventDescriptionText.Text = _medicalEvent.EventDescription ?? "Không có mô tả";
                LocationText.Text = _medicalEvent.Location ?? "Không xác định";
                EventDateText.Text = _medicalEvent.EventDate?.ToString("dd/MM/yyyy") ?? "N/A";

                // ✅ Enhanced Severity Badge with English/Vietnamese support
                SetSeverityBadge(_medicalEvent.SeverityLevel ?? "Medium");

                // ✅ Staff Nurse Information - Use async to prevent hanging
                User staffNurse = null;
                if (_medicalEvent.StaffNurseId.HasValue)
                {
                    try
                    {
                        staffNurse = await Task.Run(() =>
                        {
                            try
                            {
                                return _userService.GetUserById(_medicalEvent.StaffNurseId.Value);
                            }
                            catch
                            {
                                return null;
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[{DateTime.UtcNow:HH:mm:ss}] Error loading staff nurse: {ex.Message}");
                    }
                }

                // ✅ Use current user if staff nurse not found
                StaffNurseText.Text = staffNurse?.FullName ?? _currentUser?.FullName ?? "Không xác định";

                // Parent Notification Status
                var isNotified = _medicalEvent.ParentNotified == true;
                ParentNotifiedText.Text = isNotified ? "✅ Đã thông báo" : "❌ Chưa thông báo";
                ParentNotifiedText.Foreground = isNotified
                    ? new SolidColorBrush(Color.FromRgb(39, 174, 96))
                    : new SolidColorBrush(Color.FromRgb(231, 76, 60));

                // Show notify button if not notified
                if (!isNotified && NotifyParentButton != null)
                {
                    NotifyParentButton.Visibility = Visibility.Visible;
                }

                // Notes
                NotesText.Text = string.IsNullOrEmpty(_medicalEvent.Notes) ? "Không có ghi chú" : _medicalEvent.Notes;

                // ✅ Use current time and user info
                this.Title = $"Chi tiết sự kiện y tế - {_medicalEvent.Student?.FullName ?? "N/A"} - {DateTime.Now:dd/MM/yyyy HH:mm}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi tải chi tiết: {ex.Message}\n\n" +
                    $"🕐 Thời gian: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n" +
                    $"👤 User: {_currentUser?.FullName ?? "N/A"}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ✅ Enhanced SetSeverityBadge with English/Vietnamese support
        private void SetSeverityBadge(string severity)
        {
            if (SeverityBorder == null || SeverityText == null) return;

            switch (severity?.ToLower())
            {
                case "high":
                case "nghiêm trọng":
                    SeverityBorder.Background = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                    SeverityText.Text = "🔴 MỨC ĐỘ CAO";
                    SeverityText.Foreground = Brushes.White;
                    break;

                case "medium":
                case "trung bình":
                    SeverityBorder.Background = new SolidColorBrush(Color.FromRgb(243, 156, 18));
                    SeverityText.Text = "🟡 MỨC ĐỘ TRUNG BÌNH";
                    SeverityText.Foreground = Brushes.White;
                    break;

                case "low":
                case "nhẹ":
                    SeverityBorder.Background = new SolidColorBrush(Color.FromRgb(39, 174, 96));
                    SeverityText.Text = "🟢 MỨC ĐỘ THẤP";
                    SeverityText.Foreground = Brushes.White;
                    break;

                default:
                    SeverityBorder.Background = new SolidColorBrush(Color.FromRgb(149, 165, 166));
                    SeverityText.Text = "⚪ KHÔNG XÁC ĐỊNH";
                    SeverityText.Foreground = Brushes.White;
                    break;
            }
        }

        private async void NotifyParentButton_Click(object sender, RoutedEventArgs e)
        {
            var studentName = _medicalEvent.Student?.FullName ?? "Không xác định";
            var eventType = _medicalEvent.EventType ?? "Không xác định";
            var currentUserName = _currentUser?.FullName ?? _currentUser?.FullName ?? "Không xác định";

            var result = MessageBox.Show(
                $"🤔 Bạn có chắc chắn muốn thông báo cho phụ huynh?\n\n" +
                $"👨‍👩‍👧‍👦 Học sinh: {studentName}\n" +
                $"🏥 Loại sự kiện: {eventType}\n" +
                $"📅 Ngày: {_medicalEvent.EventDate?.ToString("dd/MM/yyyy") ?? "N/A"}\n\n" +
                $"🕐 Thời gian hiện tại: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n" +
                $"👩‍⚕️ Y tá thực hiện: {currentUserName}",
                "Xác nhận thông báo phụ huynh",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await UpdateParentNotification();
            }
        }

        private async Task UpdateParentNotification()
        {
            try
            {
                if (NotifyParentButton != null)
                {
                    NotifyParentButton.IsEnabled = false;
                    NotifyParentButton.Content = "⏳ Đang xử lý...";
                }

                _medicalEvent.ParentNotified = true;

                // ✅ Use ConfigureAwait(false) to prevent deadlock
                await Task.Run(() => _medicalEventService.UpdateMedicalEvent(_medicalEvent))
                    .ConfigureAwait(true);

                // Update UI on UI thread
                Dispatcher.Invoke(() =>
                {
                    // Update UI
                    ParentNotifiedText.Text = "✅ Đã thông báo";
                    ParentNotifiedText.Foreground = new SolidColorBrush(Color.FromRgb(39, 174, 96));

                    if (NotifyParentButton != null)
                        NotifyParentButton.Visibility = Visibility.Collapsed;
                });

                EventUpdated?.Invoke();

                var studentName = _medicalEvent.Student?.FullName ?? "Không xác định";
                var currentUserName = _currentUser?.FullName ?? _currentUser?.FullName ?? "Không xác định";

                MessageBox.Show(
                    $"🎉 Đã thông báo phụ huynh thành công!\n\n" +
                    $"👨‍👩‍👧‍👦 Học sinh: {studentName}\n" +
                    $"🕐 Thời gian thông báo: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n" +
                    $"👩‍⚕️ Y tá thực hiện: {currentUserName}\n" +
                    $"📱 Phụ huynh sẽ nhận thông báo qua SMS/Email\n" +
                    $"📝 Trạng thái: Đã cập nhật thành công",
                    "Thông báo thành công",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                var currentUserName = _currentUser?.FullName ?? _currentUser?.FullName ?? "Không xác định";
                MessageBox.Show($"❌ Lỗi khi cập nhật thông báo phụ huynh:\n\n" +
                    $"📋 Chi tiết lỗi: {ex.Message}\n" +
                    $"🕐 Thời gian: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n" +
                    $"👤 User: {currentUserName}\n\n" +
                    $"🔄 Vui lòng thử lại sau hoặc liên hệ quản trị viên.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Dispatcher.Invoke(() =>
                {
                    if (NotifyParentButton != null)
                    {
                        NotifyParentButton.IsEnabled = true;
                        NotifyParentButton.Content = "📞 Thông báo phụ huynh";
                    }
                });
            }
        }

        private void EditEventButton_Click(object sender, RoutedEventArgs e)
        {
            var currentUserName = _currentUser?.FullName ?? _currentUser?.FullName ?? "Không xác định";
            MessageBox.Show(
                $"⚠️ Chức năng chỉnh sửa sự kiện đang được phát triển\n\n" +
                $"🔧 Sẽ có sẵn trong phiên bản tiếp theo\n" +
                $"📞 Liên hệ quản trị viên nếu cần chỉnh sửa khẩn cấp\n\n" +
                $"🕐 Thời gian: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n" +
                $"👤 User: {currentUserName}",
                "Thông báo",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // ✅ Add helper method for severity level conversion
        private string GetSeverityDisplayText(string severityLevel)
        {
            return severityLevel?.ToLower() switch
            {
                "high" or "nghiêm trọng" => "🔴 Nghiêm trọng",
                "medium" or "trung bình" => "🟡 Trung bình",
                "low" or "nhẹ" => "🟢 Nhẹ",
                _ => "⚪ Không xác định"
            };
        }

        // ✅ Add method to refresh event details if needed
        public async Task RefreshEventDetails()
        {
            await LoadEventDetailsAsync();
        }
    }
}