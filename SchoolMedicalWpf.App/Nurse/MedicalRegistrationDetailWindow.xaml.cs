using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Nurse
{
    public partial class MedicalRegistrationDetailWindow : Window
    {
        private MedicalRegistration _registration;
        private readonly MedicalRegistrationService _medicalRegistrationService;

        // Simple event for notifying parent window
        public event Action RegistrationUpdated;

        public MedicalRegistrationDetailWindow(MedicalRegistration registration, MedicalRegistrationService medicalRegistrationService)
        {
            InitializeComponent();
            _registration = registration;
            _medicalRegistrationService = medicalRegistrationService;
            LoadRegistrationDetails();
        }

        private void LoadRegistrationDetails()
        {
            try
            {
                // Student Information  
                StudentNameText.Text = _registration.Student?.FullName ?? "Không xác định";
                StudentIdText.Text = _registration.Student?.StudentCode ?? "N/A";

                // Parent Information  
                ParentNameText.Text = _registration.User?.FullName ?? "Không xác định";

                // Medication Information  
                MedicationNameText.Text = _registration.MedicationName;
                DosageText.Text = _registration.TotalDosages;
                NotesText.Text = string.IsNullOrEmpty(_registration.Notes) ? "Không có ghi chú" : _registration.Notes;

                // Submission Information  
                DateSubmittedText.Text = _registration.DateSubmitted?.ToString("dd/MM/yyyy") ?? "Không xác định";
                ConsentText.Text = (bool)_registration.ParentalConsent! ? "✓ Đã đồng ý" : "✗ Chưa đồng ý";

                // Status Badge  
                StatusBadge.Child = CreateStatusBadge(_registration.Status);

                // Processing Information - only show if you have these fields  
                // if (_registration.ProcessedDate.HasValue)  
                // {  
                //     ProcessingInfoPanel.Visibility = Visibility.Visible;  
                //     ProcessedDateText.Text = _registration.ProcessedDate.Value.ToString("dd/MM/yyyy HH:mm");  
                //     ProcessedByText.Text = _registration.ProcessedBy ?? "Không xác định";  
                // }  

                // Quick Actions for Pending status (false = pending)  
                if (_registration.Status == false)
                {
                    QuickActionsPanel.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải chi tiết: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private TextBlock CreateStatusBadge(bool status)
        {
            var statusText = new TextBlock
            {
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            var badge = new Border
            {
                CornerRadius = new CornerRadius(10),
                HorizontalAlignment = HorizontalAlignment.Left,
                Child = statusText
            };

            if (status)
            {
                badge.Background = new SolidColorBrush(Color.FromRgb(39, 174, 96));
                statusText.Text = "Đã duyệt";
            }
            else
            {
                badge.Background = new SolidColorBrush(Color.FromRgb(243, 156, 18));
                statusText.Text = "Chờ duyệt";
            }

            return new TextBlock
            {
                Inlines = { new System.Windows.Documents.InlineUIContainer(badge) }
            };
        }

        private async void QuickApproveButton_Click(object sender, RoutedEventArgs e)
        {
            var studentName = _registration.Student?.FullName ?? "Không xác định";
            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn duyệt đơn thuốc của {studentName}?",
                "Xác nhận duyệt",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await UpdateStatus(true);
            }
        }

        private async void QuickRejectButton_Click(object sender, RoutedEventArgs e)
        {
            var studentName = _registration.Student?.FullName ?? "Không xác định";
            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn từ chối đơn thuốc của {studentName}?",
                "Xác nhận từ chối",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await UpdateStatus(false);
            }
        }

        private async Task UpdateStatus(bool newStatus)
        {
            try
            {
                _registration.Status = newStatus;
                // If you have these fields, uncomment:
                // _registration.ProcessedDate = DateTime.Now;
                // _registration.ProcessedBy = "danielleit241";

                // Save to database
                await Task.Run(() => _medicalRegistrationService.UpdateRegistration(_registration));

                // Notify parent window
                RegistrationUpdated?.Invoke();

                // Refresh display
                LoadRegistrationDetails();

                string statusText = newStatus ? "duyệt" : "từ chối";

                MessageBox.Show(
                    $"Đã {statusText} đơn thuốc thành công!",
                    "Thành công",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // Hide quick actions if approved
                if (newStatus)
                {
                    QuickActionsPanel.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật trạng thái: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // TODO: Implement print functionality
                MessageBox.Show("Chức năng in đơn sẽ được triển khai sau!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi in đơn: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}