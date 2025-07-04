using System.Windows;
using System.Windows.Controls;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Nurse
{
    public partial class VaccinationResultWindow : Window
    {
        private readonly Guid _healthProfileId;
        private readonly Guid? _scheduleId;
        private readonly User _currentUser;
        private readonly VaccinationResultService _vaccinationResultService;
        private readonly DateTime _currentDateTime = DateTime.Now;

        // Track edit mode and existing result
        private bool _isEditMode = false;
        private Guid? _existingResultId = null;
        private VaccinationResult _originalResult = null;

        public VaccinationResult Result { get; private set; }
        public bool IsSaved { get; private set; } = false;

        // Fixed constructor to match the calling pattern
        public VaccinationResultWindow(Guid healthProfileId,
                                      Guid? scheduleId,
                                      User currentUser,
                                      VaccinationResultService vaccinationResultService,
                                      string studentCode = "",
                                      string studentName = "",
                                      string grade = "")
        {
            InitializeComponent();
            _healthProfileId = healthProfileId;
            _scheduleId = scheduleId;
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _vaccinationResultService = vaccinationResultService ?? throw new ArgumentNullException(nameof(vaccinationResultService));

            InitializeWindow(studentCode, studentName, grade);
            SetupEventHandlers();
        }

        private void InitializeWindow(string studentCode, string studentName, string grade)
        {
            txtDateTime.Text = $"{_currentDateTime:yyyy-MM-dd HH:mm:ss} UTC - Y tá: {_currentUser.FullName} ({_currentUser.FullName})";

            // Fill student information
            txtStudentCode.Text = studentCode;
            txtStudentName.Text = studentName;
            txtGrade.Text = grade;
            txtHealthProfileId.Text = _healthProfileId.ToString();

            // Set default date
            dpVaccinationDate.SelectedDate = _currentDateTime.Date;

            // Hide reaction time panel initially
            pnlReactionTime.Visibility = Visibility.Collapsed;

            // Focus on first input
            dpVaccinationDate.Focus();
        }

        // Added LoadExistingData method
        public void LoadExistingData(VaccinationResult existingResult)
        {
            if (existingResult == null) return;

            try
            {
                // Set edit mode flags
                _isEditMode = true;
                _existingResultId = existingResult.VaccinationResultId;
                _originalResult = existingResult;

                // Load existing data into controls
                if (FindName("txtDoseNumber") is TextBox txtDoseNumber)
                    txtDoseNumber.Text = existingResult.DoseNumber?.ToString() ?? "";

                if (FindName("dpVaccinationDate") is DatePicker dpVaccinationDate && existingResult.VaccinationDate.HasValue)
                    dpVaccinationDate.SelectedDate = existingResult.VaccinationDate.Value.ToDateTime(TimeOnly.MinValue);

                if (FindName("cmbInjectionSite") is ComboBox cmbInjectionSite)
                {
                    foreach (ComboBoxItem item in cmbInjectionSite.Items)
                    {
                        if (item.Content.ToString() == existingResult.InjectionSite)
                        {
                            cmbInjectionSite.SelectedItem = item;
                            break;
                        }
                    }
                }

                if (FindName("cmbImmediateReaction") is ComboBox cmbImmediateReaction)
                {
                    foreach (ComboBoxItem item in cmbImmediateReaction.Items)
                    {
                        if (item.Content.ToString() == existingResult.ImmediateReaction)
                        {
                            cmbImmediateReaction.SelectedItem = item;
                            break;
                        }
                    }
                }

                if (FindName("dtpReactionStartTime") is DatePicker dtpReactionStartTime && existingResult.ReactionStartTime.HasValue)
                    dtpReactionStartTime.SelectedDate = existingResult.ReactionStartTime.Value;

                if (FindName("cmbReactionType") is ComboBox cmbReactionType)
                {
                    foreach (ComboBoxItem item in cmbReactionType.Items)
                    {
                        if (item.Content.ToString() == existingResult.ReactionType)
                        {
                            cmbReactionType.SelectedItem = item;
                            break;
                        }
                    }
                }

                if (FindName("cmbSeverityLevel") is ComboBox cmbSeverityLevel)
                {
                    foreach (ComboBoxItem item in cmbSeverityLevel.Items)
                    {
                        if (item.Content.ToString() == existingResult.SeverityLevel)
                        {
                            cmbSeverityLevel.SelectedItem = item;
                            break;
                        }
                    }
                }

                if (FindName("txtNotes") is TextBox txtNotes)
                    txtNotes.Text = existingResult.Notes ?? "";

                // Update UI for edit mode
                Title = Title.Replace("Ghi nhận", "Chỉnh sửa");
                if (FindName("btnSave") is Button btnSave)
                    btnSave.Content = "💾 Cập nhật kết quả";

                // Show reaction panel if there are reactions
                if (!string.IsNullOrEmpty(existingResult.ImmediateReaction) &&
                    existingResult.ImmediateReaction != "Không có")
                {
                    pnlReactionTime.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu hiện có: {ex.Message}", "Lỗi",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SetupEventHandlers()
        {
            // Handle immediate reaction selection
            if (FindName("cmbImmediateReaction") is ComboBox cmbImmediateReaction)
            {
                cmbImmediateReaction.SelectionChanged += (s, e) =>
                {
                    if (cmbImmediateReaction.SelectedItem is ComboBoxItem item)
                    {
                        string reaction = item.Content.ToString();
                        pnlReactionTime.Visibility = (reaction != "Không có") ? Visibility.Visible : Visibility.Collapsed;
                    }
                };
            }
        }

        private async void SaveResult_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                try
                {
                    Result = CreateVaccinationResult();

                    if (_isEditMode && _existingResultId.HasValue)
                    {
                        // UPDATE existing result
                        await Task.Run(() => _vaccinationResultService.UpdateVaccinationResult(Result));
                    }
                    else
                    {
                        // CREATE new result
                        await Task.Run(() => _vaccinationResultService.AddVaccinationResult(Result));
                    }

                    IsSaved = true;

                    string action = _isEditMode ? "cập nhật" : "tạo mới";
                    MessageBox.Show($"✅ Kết quả tiêm chủng đã được {action} thành công!",
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

        // *** FIX CHÍNH: Logic tạo VaccinationResultId đúng ***
        private VaccinationResult CreateVaccinationResult()
        {
            // QUAN TRỌNG: Sử dụng existing VaccinationResultId nếu đang edit
            Guid resultId = _isEditMode && _existingResultId.HasValue
                ? _existingResultId.Value
                : Guid.NewGuid();

            var result = new VaccinationResult
            {
                VaccinationResultId = resultId, // *** SỬA CHÍNH Ở ĐÂY ***
                ScheduleId = _scheduleId,
                HealthProfileId = _healthProfileId,
                VaccinationDate = DateOnly.FromDateTime(dpVaccinationDate.SelectedDate!.Value),
                Notes = string.IsNullOrWhiteSpace(txtNotes.Text) ? null : txtNotes.Text.Trim(),
                RecordedId = _currentUser.UserId
            };

            // Parse dose number
            if (FindName("cmbDoseNumber") is ComboBox cmbDoseNumber && cmbDoseNumber.SelectedItem is ComboBoxItem doseItem)
            {
                if (int.TryParse(doseItem.Content.ToString(), out int dose))
                {
                    result.DoseNumber = dose;
                }
            }

            // Parse injection site
            if (FindName("cmbInjectionSite") is ComboBox cmbInjectionSite && cmbInjectionSite.SelectedItem is ComboBoxItem siteItem)
            {
                result.InjectionSite = siteItem.Content.ToString();
            }

            // Parse immediate reaction
            if (FindName("cmbImmediateReaction") is ComboBox cmbImmediateReaction && cmbImmediateReaction.SelectedItem is ComboBoxItem reactionItem)
            {
                result.ImmediateReaction = reactionItem.Content.ToString();
            }

            // Parse reaction start time
            if (FindName("dtpReactionStartTime") is DatePicker dtpReactionStartTime && dtpReactionStartTime.SelectedDate.HasValue)
            {
                result.ReactionStartTime = dtpReactionStartTime.SelectedDate.Value;
            }

            // Parse reaction type
            if (FindName("cmbReactionType") is ComboBox cmbReactionType && cmbReactionType.SelectedItem is ComboBoxItem typeItem)
            {
                result.ReactionType = typeItem.Content.ToString();
            }

            // Parse severity level
            if (FindName("cmbSeverityLevel") is ComboBox cmbSeverityLevel && cmbSeverityLevel.SelectedItem is ComboBoxItem severityItem)
            {
                result.SeverityLevel = severityItem.Content.ToString();
            }

            return result;
        }

        private bool ValidateInput()
        {
            var errors = new List<string>();

            if (!dpVaccinationDate.SelectedDate.HasValue)
            {
                errors.Add("Vui lòng chọn ngày tiêm chủng");
            }

            if (FindName("cmbDoseNumber") is ComboBox cmbDoseNumber && cmbDoseNumber.SelectedItem == null)
            {
                errors.Add("Vui lòng chọn liều tiêm");
            }

            if (FindName("cmbInjectionSite") is ComboBox cmbInjectionSite && cmbInjectionSite.SelectedItem == null)
            {
                errors.Add("Vui lòng chọn vị trí tiêm");
            }

            if (errors.Any())
            {
                MessageBox.Show($"Vui lòng sửa các lỗi sau:\n\n{string.Join("\n", errors)}",
                              "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
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
    }
}