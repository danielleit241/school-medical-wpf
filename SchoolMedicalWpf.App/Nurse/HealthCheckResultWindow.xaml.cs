using System.Windows;
using System.Windows.Media;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Nurse
{
    public partial class HealthCheckResultWindow : Window
    {
        private readonly Guid _healthProfileId;
        private readonly Guid? _scheduleId;
        private readonly User _currentUser;
        private readonly HealthCheckResultService _healthCheckResultService;
        private readonly DateTime _currentDateTime = new DateTime(2025, 7, 4, 2, 38, 7, DateTimeKind.Utc);

        public HealthCheckResult Result { get; private set; }
        public bool IsSaved { get; private set; } = false;

        public HealthCheckResultWindow(Guid healthProfileId, Guid? scheduleId, User currentUser,
                                       HealthCheckResultService healthCheckResultService,
                                       string studentCode = "", string studentName = "",
                                       string grade = "")
        {
            InitializeComponent();
            _healthProfileId = healthProfileId;
            _scheduleId = scheduleId;
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser), "Current user cannot be null");
            _healthCheckResultService = healthCheckResultService ?? throw new ArgumentNullException(nameof(healthCheckResultService), "Health check result service cannot be null");
            InitializeWindow(studentCode, studentName, grade);
        }

        private void InitializeWindow(string studentCode, string studentName, string grade)
        {
            txtDateTime.Text = $"{_currentDateTime:dd/MM/yyyy HH:mm:ss} UTC - Y tá: {_currentUser}";

            // Fill student information
            txtStudentCode.Text = studentCode;
            txtStudentName.Text = studentName;
            txtGrade.Text = grade;
            txtHealthProfileId.Text = _healthProfileId.ToString();

            // Set default date
            dpDatePerformed.SelectedDate = _currentDateTime.Date;

            // Focus on first input
            txtHeight.Focus();
        }

        private void SaveResult_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                try
                {
                    Result = CreateHealthCheckResult();

                    // TODO: Save to database here
                    // await _healthCheckService.SaveResultAsync(Result);

                    IsSaved = true;

                    MessageBox.Show("✅ Kết quả khám sức khỏe đã được lưu thành công!",
                                    "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    DialogResult = true;
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ Lỗi khi lưu kết quả: {ex.Message}",
                                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool ValidateInput()
        {
            txtValidationMessage.Visibility = Visibility.Collapsed;

            if (!dpDatePerformed.SelectedDate.HasValue)
            {
                ShowValidationError("Vui lòng chọn ngày khám!");
                dpDatePerformed.Focus();
                return false;
            }

            if (dpDatePerformed.SelectedDate.Value > _currentDateTime.Date)
            {
                ShowValidationError("Ngày khám không thể là ngày tương lai!");
                dpDatePerformed.Focus();
                return false;
            }

            // Validate height
            if (!string.IsNullOrWhiteSpace(txtHeight.Text))
            {
                if (!double.TryParse(txtHeight.Text, out double height) || height <= 0 || height > 300)
                {
                    ShowValidationError("Chiều cao không hợp lệ! (0-300 cm)");
                    txtHeight.Focus();
                    return false;
                }
            }

            // Validate weight
            if (!string.IsNullOrWhiteSpace(txtWeight.Text))
            {
                if (!double.TryParse(txtWeight.Text, out double weight) || weight <= 0 || weight > 500)
                {
                    ShowValidationError("Cân nặng không hợp lệ! (0-500 kg)");
                    txtWeight.Focus();
                    return false;
                }
            }

            // Validate vision
            if (!string.IsNullOrWhiteSpace(txtVisionLeft.Text))
            {
                if (!IsValidVision(txtVisionLeft.Text))
                {
                    ShowValidationError("Thị lực mắt trái không hợp lệ! VD: 10/10, 1.0, 0.8");
                    txtVisionLeft.Focus();
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtVisionRight.Text))
            {
                if (!IsValidVision(txtVisionRight.Text))
                {
                    ShowValidationError("Thị lực mắt phải không hợp lệ! VD: 10/10, 1.0, 0.8");
                    txtVisionRight.Focus();
                    return false;
                }
            }

            return true;
        }

        private bool IsValidVision(string vision)
        {
            // Accept formats: "10/10", "1.0", "0.8", etc.
            if (vision.Contains("/"))
            {
                var parts = vision.Split('/');
                return parts.Length == 2 &&
                       double.TryParse(parts[0], out _) &&
                       double.TryParse(parts[1], out _);
            }

            return double.TryParse(vision, out double val) && val >= 0 && val <= 2.0;
        }

        private void ShowValidationError(string message)
        {
            txtValidationMessage.Text = $"⚠️ {message}";
            txtValidationMessage.Visibility = Visibility.Visible;

            // Highlight the window border
            BorderBrush = Brushes.Red;
            BorderThickness = new Thickness(2);

            // Reset after 3 seconds
            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            timer.Tick += (s, e) =>
            {
                BorderBrush = SystemColors.ActiveBorderBrush;
                BorderThickness = new Thickness(1);
                timer.Stop();
            };
            timer.Start();
        }

        private HealthCheckResult CreateHealthCheckResult()
        {
            var result = new HealthCheckResult
            {
                ResultId = Guid.NewGuid(),
                ScheduleId = _scheduleId,
                HealthProfileId = _healthProfileId,
                DatePerformed = DateOnly.FromDateTime(dpDatePerformed.SelectedDate.Value),
                BloodPressure = string.IsNullOrWhiteSpace(txtBloodPressure.Text) ? null : txtBloodPressure.Text.Trim(),
                Notes = string.IsNullOrWhiteSpace(txtNotes.Text) ? null : txtNotes.Text.Trim(),
                RecordedId = Guid.Parse("12345678-1234-1234-1234-123456789012") // danielleit241's ID
            };

            // Parse height
            if (!string.IsNullOrWhiteSpace(txtHeight.Text) && double.TryParse(txtHeight.Text, out double height))
            {
                result.Height = height;
            }

            // Parse weight
            if (!string.IsNullOrWhiteSpace(txtWeight.Text) && double.TryParse(txtWeight.Text, out double weight))
            {
                result.Weight = weight;
            }

            // Parse vision left
            if (!string.IsNullOrWhiteSpace(txtVisionLeft.Text))
            {
                result.VisionLeft = ParseVision(txtVisionLeft.Text);
            }

            // Parse vision right
            if (!string.IsNullOrWhiteSpace(txtVisionRight.Text))
            {
                result.VisionRight = ParseVision(txtVisionRight.Text);
            }

            // Get hearing
            if (cmbHearing.SelectedItem is System.Windows.Controls.ComboBoxItem hearingItem)
            {
                result.Hearing = hearingItem.Content.ToString();
            }

            // Get nose
            if (cmbNose.SelectedItem is System.Windows.Controls.ComboBoxItem noseItem)
            {
                result.Nose = noseItem.Content.ToString();
            }

            return result;
        }

        private double? ParseVision(string visionText)
        {
            if (visionText.Contains("/"))
            {
                var parts = visionText.Split('/');
                if (parts.Length == 2 &&
                    double.TryParse(parts[0], out double numerator) &&
                    double.TryParse(parts[1], out double denominator) &&
                    denominator != 0)
                {
                    return numerator / denominator;
                }
            }
            else if (double.TryParse(visionText, out double vision))
            {
                return vision;
            }

            return null;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn hủy? Dữ liệu đã nhập sẽ bị mất.",
                                         "Xác nhận hủy", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                DialogResult = false;
                Close();
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!IsSaved && !DialogResult.HasValue)
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn đóng? Dữ liệu đã nhập sẽ bị mất.",
                                             "Xác nhận đóng", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }

            base.OnClosing(e);
        }
    }
}