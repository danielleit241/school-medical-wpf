using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;
using System.Windows;
using System.Windows.Media;

namespace SchoolMedicalWpf.App.Nurse
{
    public partial class MedicalEventDetailWindow : Window
    {
        private MedicalEvent _medicalEvent;
        private readonly MedicalEventService _medicalEventService;
        private readonly UserService _userService;
        private readonly User _currentUser;

        public event Action EventUpdated;

        public MedicalEventDetailWindow(MedicalEventService medicalEventService, UserService userService, MedicalEvent medicalEvent, User currentUser)
        {
            InitializeComponent();
            _medicalEvent = medicalEvent;
            _medicalEventService = medicalEventService;
            _userService = userService;
            _currentUser = currentUser;
            Loaded += async (s, e) => await LoadEventDetailsAsync();
        }

        private async Task LoadEventDetailsAsync()
        {
            try
            {
                StudentNameText.Text = _medicalEvent.Student?.FullName ?? "Không xác định";
                StudentClassText.Text = _medicalEvent.Student?.Grade ?? "N/A";
                EventTypeText.Text = _medicalEvent.EventType ?? "Không xác định";
                EventDescriptionText.Text = _medicalEvent.EventDescription ?? "Không có mô tả";
                LocationText.Text = _medicalEvent.Location ?? "Không xác định";
                EventDateText.Text = _medicalEvent.EventDate?.ToString("dd/MM/yyyy") ?? "N/A";
                SetSeverityBadge(_medicalEvent.SeverityLevel ?? "Medium");
                User? staffNurse = null;
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

                StaffNurseText.Text = staffNurse?.FullName ?? _currentUser?.FullName ?? "Không xác định";
                var isNotified = _medicalEvent.ParentNotified == true;
                ParentNotifiedText.Text = isNotified ? "✅ Đã thông báo" : "❌ Chưa thông báo";
                ParentNotifiedText.Foreground = isNotified
                    ? new SolidColorBrush(Color.FromRgb(39, 174, 96))
                    : new SolidColorBrush(Color.FromRgb(231, 76, 60));
                NotesText.Text = string.IsNullOrEmpty(_medicalEvent.Notes) ? "Không có ghi chú" : _medicalEvent.Notes;
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

        public async Task RefreshEventDetails()
        {
            await LoadEventDetailsAsync();
        }
    }
}