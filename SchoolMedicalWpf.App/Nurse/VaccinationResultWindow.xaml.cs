using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
        private readonly DateTime _currentDateTime = new DateTime(2025, 7, 4, 2, 38, 7, DateTimeKind.Utc);

        public VaccinationResult Result { get; private set; }
        public bool IsSaved { get; private set; } = false;

        public VaccinationResultWindow(Guid healthProfileId, Guid? scheduleId, User currentUser,
                                       VaccinationResultService vaccinationResultService,
                                       string studentCode, string studentName,
                                       string grade)
        {
            InitializeComponent();
            _healthProfileId = healthProfileId;
            _scheduleId = scheduleId;
            _currentUser = currentUser;
            _vaccinationResultService = vaccinationResultService;

            InitializeWindow(studentCode, studentName, grade);
            SetupEventHandlers();
        }

        // Overload constructor to handle optional parameters
        public VaccinationResultWindow(Guid healthProfileId, Guid? scheduleId, User currentUser,
                                       VaccinationResultService vaccinationResultService)
            : this(healthProfileId, scheduleId, currentUser, vaccinationResultService, "", "", "")
        {
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
            dpVaccinationDate.SelectedDate = _currentDateTime.Date;

            // Hide reaction time panel initially
            pnlReactionTime.Visibility = Visibility.Collapsed;

            // Focus on first input
            dpVaccinationDate.Focus();
        }

        private void SetupEventHandlers()
        {
            // Show/hide reaction time based on immediate reaction selection
            cmbImmediateReaction.SelectionChanged += (s, e) =>
            {
                if (cmbImmediateReaction.SelectedItem is ComboBoxItem item)
                {
                    var hasReaction = item.Content.ToString() != "Không có phản ứng";
                    pnlReactionTime.Visibility = hasReaction ? Visibility.Visible : Visibility.Collapsed;

                    if (hasReaction)
                    {
                        dtpReactionStartTime.SelectedDate = _currentDateTime;
                    }
                }
            };
        }

        private void SaveResult_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                try
                {
                    Result = CreateVaccinationResult();

                    // TODO: Save to database here
                    // await _vaccinationService.SaveResultAsync(Result);

                    IsSaved = true;

                    MessageBox.Show("✅ Kết quả tiêm chủng đã được lưu thành công!",
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

            if (!dpVaccinationDate.SelectedDate.HasValue)
            {
                ShowValidationError("Vui lòng chọn ngày tiêm!");
                dpVaccinationDate.Focus();
                return false;
            }

            if (dpVaccinationDate.SelectedDate.Value > _currentDateTime.Date)
            {
                ShowValidationError("Ngày tiêm không thể là ngày tương lai!");
                dpVaccinationDate.Focus();
                return false;
            }

            if (cmbDoseNumber.SelectedItem == null)
            {
                ShowValidationError("Vui lòng chọn liều thứ!");
                cmbDoseNumber.Focus();
                return false;
            }

            if (cmbInjectionSite.SelectedItem == null)
            {
                ShowValidationError("Vui lòng chọn vị trí tiêm!");
                cmbInjectionSite.Focus();
                return false;
            }

            // Validate reaction start time if reaction exists
            if (pnlReactionTime.Visibility == Visibility.Visible)
            {
                if (dtpReactionStartTime.SelectedDate.HasValue)
                {
                    var reactionTime = dtpReactionStartTime.SelectedDate.Value;
                    var vaccinationTime = dpVaccinationDate.SelectedDate.Value;

                    if (reactionTime < vaccinationTime)
                    {
                        ShowValidationError("Thời gian phản ứng không thể trước thời gian tiêm!");
                        dtpReactionStartTime.Focus();
                        return false;
                    }
                }
            }

            return true;
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

        private VaccinationResult CreateVaccinationResult()
        {
            var result = new VaccinationResult
            {
                VaccinationResultId = Guid.NewGuid(),
                ScheduleId = _scheduleId,
                HealthProfileId = _healthProfileId,
                VaccinationDate = DateOnly.FromDateTime(dpVaccinationDate.SelectedDate.Value),
                Notes = string.IsNullOrWhiteSpace(txtNotes.Text) ? null : txtNotes.Text.Trim(),
                RecordedId = Guid.Parse("12345678-1234-1234-1234-123456789012") // danielleit241's ID
            };

            // Get dose number
            if (cmbDoseNumber.SelectedItem is ComboBoxItem doseItem)
            {
                if (int.TryParse(doseItem.Content.ToString(), out int dose))
                {
                    result.DoseNumber = dose;
                }
                else
                {
                    result.DoseNumber = 0; // For "Nhắc lại"
                }
            }

            // Get injection site
            if (cmbInjectionSite.SelectedItem is ComboBoxItem siteItem)
            {
                result.InjectionSite = siteItem.Content.ToString();
            }

            // Get immediate reaction
            if (cmbImmediateReaction.SelectedItem is ComboBoxItem reactionItem)
            {
                result.ImmediateReaction = reactionItem.Content.ToString();
            }

            // Get reaction start time
            if (dtpReactionStartTime.SelectedDate.HasValue)
            {
                result.ReactionStartTime = dtpReactionStartTime.SelectedDate.Value;
            }

            // Get reaction type
            if (cmbReactionType.SelectedItem is ComboBoxItem typeItem)
            {
                result.ReactionType = typeItem.Content.ToString();
            }

            // Get severity level
            if (cmbSeverityLevel.SelectedItem is ComboBoxItem severityItem)
            {
                result.SeverityLevel = severityItem.Content.ToString();
            }

            return result;
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