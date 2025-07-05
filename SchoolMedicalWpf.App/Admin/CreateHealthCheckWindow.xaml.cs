using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;
using SchoolMedicalWpf.Dal.Repositories;
using System.Windows;

namespace SchoolMedicalWpf.App.Admin
{
    public partial class CreateHealthCheckWindow : Window
    {
        private HealthCheckSchedule _existingSchedule;
        private bool _isEditMode;

        private readonly HealthCheckScheduleService _healthCheckSchedule;
        private readonly HealthCheckResultService _healthCheckResult;
        private readonly StudentService _studentService;
        private readonly HealthProfileService _healthProfileService;

        public CreateHealthCheckWindow(HealthCheckScheduleService healthCheckSchedule, HealthCheckResultService healthCheckResultService, StudentService studentService, HealthProfileService healthProfileService)
        {
            InitializeComponent();
            _healthCheckSchedule = healthCheckSchedule;
            _healthCheckResult = healthCheckResultService;
            _studentService = studentService;
            _healthProfileService = healthProfileService;
            dpStartDate.SelectedDate = DateTime.Now;
            dpEndDate.SelectedDate = DateTime.Now.AddDays(7);
        }

        public CreateHealthCheckWindow(HealthCheckSchedule existingSchedule, HealthCheckScheduleService healthCheckSchedule, HealthCheckResultService healthCheckResultService, StudentService studentService, HealthProfileService healthProfileService)
            : this(healthCheckSchedule, healthCheckResultService, studentService, healthProfileService)
        {
            _existingSchedule = existingSchedule;
            _isEditMode = true;
            LoadExistingData();
        }


        private async Task LoadGradesAsync()
        {
            try
            {
                var allStudents = await _studentService.GetAllStudents();
                var distinctGrades = allStudents
                    .Where(s => !string.IsNullOrEmpty(s.Grade))
                    .Select(s => s.Grade)
                    .Distinct()
                    .OrderBy(g => g)
                    .ToList();

                cmbTargetGrade.ItemsSource = distinctGrades;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách khối/lớp: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LoadExistingData()
        {
            txtHeader.Text = "Sửa Lịch Khám Sức Khỏe";
            Title = "Sửa Lịch Khám Sức Khỏe";

            txtTitle.Text = _existingSchedule.Title;
            txtDescription.Text = _existingSchedule.Description;
            dpStartDate.SelectedDate = _existingSchedule.StartDate?.ToDateTime(TimeOnly.MinValue);
            dpEndDate.SelectedDate = _existingSchedule.EndDate?.ToDateTime(TimeOnly.MinValue);
            cmbTargetGrade.Text = _existingSchedule.TargetGrade;
            cmbHealthCheckType.Text = _existingSchedule.HealthCheckType;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin bắt buộc.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var schedule = _existingSchedule ?? new HealthCheckSchedule
                {
                    ScheduleId = Guid.NewGuid()
                };

                schedule.Title = txtTitle.Text.Trim();
                schedule.Description = txtDescription.Text.Trim();
                schedule.StartDate = DateOnly.FromDateTime(dpStartDate.SelectedDate!.Value);
                schedule.EndDate = DateOnly.FromDateTime(dpEndDate.SelectedDate!.Value);
                schedule.TargetGrade = cmbTargetGrade.Text.Trim();
                schedule.HealthCheckType = cmbHealthCheckType.Text.Trim();

                // TODO: Save to database
                if (_isEditMode)
                {
                    _healthCheckSchedule.Update(schedule);
                    MessageBox.Show("Cập nhật lịch khám thành công!", "Thông báo");
                }
                else
                {
                    _healthCheckSchedule.Add(schedule);
                    MessageBox.Show("Tạo lịch khám thành công!", "Thông báo");

                    // Auto generate results if checked
                    if (chkAutoGenerate.IsChecked == true)
                    {
                        var students = await _studentService.GetAllStudents();
                        students = students.Where(s => schedule.TargetGrade.Trim().ToLower() == s.Grade!.Trim().ToLower()).ToList();
                        foreach (var student in students)
                        {
                            var healthProfile = _healthProfileService.GetHealthProfileByStudentId(student.StudentId);
                            var result = new HealthCheckResult
                            {
                                ResultId = Guid.NewGuid(),
                                ScheduleId = schedule.ScheduleId,
                                HealthProfileId = healthProfile!.HealthProfileId,
                            };
                            _healthCheckResult.Add(result);
                        }
                    }
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text)) return false;
            if (!dpStartDate.SelectedDate.HasValue) return false;
            if (!dpEndDate.SelectedDate.HasValue) return false;
            if (string.IsNullOrWhiteSpace(cmbTargetGrade.Text)) return false;
            if (string.IsNullOrWhiteSpace(cmbHealthCheckType.Text)) return false;
            if (dpStartDate.SelectedDate > dpEndDate.SelectedDate) return false;

            return true;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadGradesAsync();
        }
    }
}