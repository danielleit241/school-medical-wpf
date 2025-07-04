using System.Windows;
using System.Windows.Controls;
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
        private readonly DateTime _currentDateTime = DateTime.Now;

        // Track edit mode and existing result
        private bool _isEditMode = false;
        private Guid? _existingResultId = null;
        private HealthCheckResult _originalResult = null!;

        public HealthCheckResult Result { get; private set; }
        public bool IsSaved { get; private set; } = false;

        public HealthCheckResultWindow(Guid healthProfileId,
                                      Guid? scheduleId,
                                      User currentUser,
                                      HealthCheckResultService healthCheckResultService,
                                      string studentCode = "",
                                      string studentName = "",
                                      string grade = "")
        {
            InitializeComponent();
            _healthProfileId = healthProfileId;
            _scheduleId = scheduleId;
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _healthCheckResultService = healthCheckResultService ?? throw new ArgumentNullException(nameof(healthCheckResultService));

            InitializeWindow(studentCode, studentName, grade);
        }

        private void InitializeWindow(string studentCode, string studentName, string grade)
        {
            txtDateTime.Text = $"{_currentDateTime:yyyy-MM-dd HH:mm:ss} UTC - Y tá: {_currentUser.FullName} ({_currentUser.FullName})";

            // Fill student information
            txtStudentCode.Text = studentCode;
            txtStudentName.Text = studentName;
            txtGrade.Text = grade;
            txtHealthProfileId.Text = _healthProfileId.ToString();

            dpDatePerformed.SelectedDate = _currentDateTime.Date;

            txtHeight.Focus();
        }

        public void LoadExistingData(HealthCheckResult existingResult)
        {
            if (existingResult == null) return;

            try
            {
                _isEditMode = true;
                _existingResultId = existingResult.ResultId;
                _originalResult = existingResult;

                if (FindName("txtHeight") is TextBox txtHeight)
                    txtHeight.Text = existingResult.Height?.ToString() ?? "";

                if (FindName("txtWeight") is TextBox txtWeight)
                    txtWeight.Text = existingResult.Weight?.ToString() ?? "";

                if (FindName("txtVisionLeft") is TextBox txtVisionLeft)
                    txtVisionLeft.Text = existingResult.VisionLeft?.ToString() ?? "";

                if (FindName("txtVisionRight") is TextBox txtVisionRight)
                    txtVisionRight.Text = existingResult.VisionRight?.ToString() ?? "";

                if (FindName("cmbHearing") is ComboBox cmbHearing)
                {
                    foreach (ComboBoxItem item in cmbHearing.Items)
                    {
                        if (item.Content.ToString() == existingResult.Hearing)
                        {
                            cmbHearing.SelectedItem = item;
                            break;
                        }
                    }
                }

                if (FindName("cmbNose") is ComboBox cmbNose)
                {
                    foreach (ComboBoxItem item in cmbNose.Items)
                    {
                        if (item.Content.ToString() == existingResult.Nose)
                        {
                            cmbNose.SelectedItem = item;
                            break;
                        }
                    }
                }

                if (FindName("txtBloodPressure") is TextBox txtBloodPressure)
                    txtBloodPressure.Text = existingResult.BloodPressure ?? "";

                if (FindName("dpDatePerformed") is DatePicker dpDatePerformed && existingResult.DatePerformed.HasValue)
                    dpDatePerformed.SelectedDate = existingResult.DatePerformed.Value.ToDateTime(TimeOnly.MinValue);

                if (FindName("txtNotes") is TextBox txtNotes)
                    txtNotes.Text = existingResult.Notes ?? "";

                Title = Title.Replace("Ghi nhận", "Chỉnh sửa");
                if (FindName("btnSave") is Button btnSave)
                    btnSave.Content = "💾 Cập nhật kết quả";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu hiện có: {ex.Message}", "Lỗi",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void SaveResult_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                try
                {
                    Result = CreateHealthCheckResult();

                    if (_isEditMode && _existingResultId.HasValue)
                    {
                        await Task.Run(() => _healthCheckResultService.Update(Result));
                    }
                    else
                    {
                        await Task.Run(() => _healthCheckResultService.Add(Result));
                    }

                    IsSaved = true;

                    string action = _isEditMode ? "cập nhật" : "tạo mới";
                    MessageBox.Show($"✅ Kết quả khám sức khỏe đã được {action} thành công!",
                                    "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    DialogResult = true;
                    Close();
                }
                catch (Exception ex)
                {
                    string action = _isEditMode ? "cập nhật" : "tạo mới";

                    if (ex.Message.Contains("optimistic concurrency") || ex.Message.Contains("database operation was expected"))
                    {
                        MessageBox.Show(
                            $"❌ Xung đột dữ liệu!\n\n" +
                            $"Kết quả này đã được thay đổi bởi người dùng khác.\n" +
                            $"Vui lòng đóng cửa sổ này và thử lại.",
                            "Xung đột dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        MessageBox.Show($"❌ Lỗi khi {action} kết quả:\n{ex.Message}",
                                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        // *** FIX CHÍNH: Logic tạo ResultId đúng ***
        private HealthCheckResult CreateHealthCheckResult()
        {
            // QUAN TRỌNG: Sử dụng existing ResultId nếu đang edit, tạo mới nếu create
            Guid resultId = _isEditMode && _existingResultId.HasValue
                ? _existingResultId.Value
                : Guid.NewGuid();

            var result = new HealthCheckResult
            {
                ResultId = resultId,
                ScheduleId = _scheduleId,
                HealthProfileId = _healthProfileId,
                DatePerformed = DateOnly.FromDateTime(dpDatePerformed.SelectedDate!.Value),
                BloodPressure = string.IsNullOrWhiteSpace(txtBloodPressure.Text) ? null : txtBloodPressure.Text.Trim(),
                Notes = string.IsNullOrWhiteSpace(txtNotes.Text) ? null : txtNotes.Text.Trim(),
                RecordedId = _currentUser.UserId
            };
            if (!string.IsNullOrWhiteSpace(txtHeight.Text) && double.TryParse(txtHeight.Text, out double height))
            {
                result.Height = height;
            }

            if (!string.IsNullOrWhiteSpace(txtWeight.Text) && double.TryParse(txtWeight.Text, out double weight))
            {
                result.Weight = weight;
            }

            if (!string.IsNullOrWhiteSpace(txtVisionLeft.Text))
            {
                result.VisionLeft = ParseVision(txtVisionLeft.Text);
            }

            if (!string.IsNullOrWhiteSpace(txtVisionRight.Text))
            {
                result.VisionRight = ParseVision(txtVisionRight.Text);
            }

            if (cmbHearing.SelectedItem is ComboBoxItem hearingItem)
            {
                result.Hearing = hearingItem.Content.ToString();
            }

            if (cmbNose.SelectedItem is ComboBoxItem noseItem)
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

        private bool ValidateInput()
        {
            var errors = new List<string>();

            if (!dpDatePerformed.SelectedDate.HasValue)
            {
                errors.Add("Vui lòng chọn ngày khám");
            }

            if (errors.Any())
            {
                MessageBox.Show($"Vui lòng sửa các lỗi sau:\n\n{string.Join("\n", errors)}",
                              "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!IsSaved)
            {
                var result = MessageBox.Show(
                    "Bạn có những thay đổi chưa được lưu. Bạn có chắc chắn muốn đóng không?",
                    "Xác nhận đóng", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            base.OnClosing(e);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSaved)
            {
                var result = MessageBox.Show(
                    "Bạn có những thay đổi chưa được lưu. Bạn có chắc chắn muốn hủy không?",
                    "Xác nhận hủy", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }

            DialogResult = false;
            Close();
        }
    }
}