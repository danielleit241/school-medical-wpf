using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;
using System.Windows;
using System.Windows.Controls;

namespace SchoolMedicalWpf.App.Admin
{
    public partial class CreateVaccinationWindow : Window
    {
        private VaccinationSchedule _existingSchedule;
        private bool _isEditMode;
        private List<VaccineDetail> _vaccines;
        private readonly VaccinationScheduleService _vaccinationScheduleService;
        private readonly VaccinationResultService _vaccinationResultService;
        private readonly VaccineDetailService _vaccineDetailService;
        private readonly StudentService _studentService;
        private readonly HealthProfileService _healthProfileService;

        public CreateVaccinationWindow(VaccinationScheduleService vaccinationScheduleService, VaccinationResultService vaccinationResultService, VaccineDetailService vaccineDetailService, StudentService studentService, HealthProfileService healthProfileService)
        {
            InitializeComponent();
            _vaccinationScheduleService = vaccinationScheduleService;
            _vaccinationResultService = vaccinationResultService;
            _vaccineDetailService = vaccineDetailService;
            _studentService = studentService;
            _healthProfileService = healthProfileService;

            dpStartDate.SelectedDate = DateTime.Now;
            dpEndDate.SelectedDate = DateTime.Now.AddDays(7);

            Loaded += CreateVaccinationWindow_Loaded;
        }

        public CreateVaccinationWindow(VaccinationSchedule existingSchedule, VaccinationScheduleService vaccinationScheduleService, VaccinationResultService vaccinationResultService, VaccineDetailService vaccineDetailService, StudentService studentService, HealthProfileService healthProfileService)
            : this(vaccinationScheduleService, vaccinationResultService, vaccineDetailService, studentService, healthProfileService)
        {
            _existingSchedule = existingSchedule;
            _isEditMode = true;
        }

        private async void CreateVaccinationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadGradesAsync();
            await LoadVaccines();
            if (_isEditMode)
                LoadExistingData();
        }

        private async Task LoadVaccines()
        {
            _vaccines = await Task.Run(() => _vaccineDetailService.GetAllVaccineDetailsAsync());

            cmbVaccine.ItemsSource = _vaccines;
            cmbVaccine.DisplayMemberPath = "VaccineName";
            cmbVaccine.SelectedValuePath = "VaccineId";
        }

        private async Task LoadGradesAsync()
        {
            try
            {
                // Lấy tất cả students và extract distinct grades
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
            txtHeader.Text = "Sửa Lịch Tiêm Chủng";
            Title = "Sửa Lịch Tiêm Chủng";

            txtTitle.Text = _existingSchedule.Title;
            cmbVaccine.SelectedValue = _existingSchedule.VaccineId;
            cmbRound.Text = _existingSchedule.Round;
            txtDescription.Text = _existingSchedule.Description;
            dpStartDate.SelectedDate = _existingSchedule.StartDate?.ToDateTime(TimeOnly.MinValue);
            dpEndDate.SelectedDate = _existingSchedule.EndDate?.ToDateTime(TimeOnly.MinValue);
            cmbTargetGrade.Text = _existingSchedule.TargetGrade;
            cmbTargetGrade.IsEnabled = false;
        }

        private void cmbVaccine_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbVaccine.SelectedItem is VaccineDetail selectedVaccine)
            {
                txtVaccineInfo.Text = $"Nhà sản xuất: {selectedVaccine.Manufacturer}\n" +
                                     $"Độ tuổi: {selectedVaccine.AgeRecommendation}\n" +
                                     $"Mô tả: {selectedVaccine.Description}";
                vaccineInfoPanel.Visibility = Visibility.Visible;
            }
            else
            {
                vaccineInfoPanel.Visibility = Visibility.Collapsed;
            }
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
                var schedule = _existingSchedule ?? new VaccinationSchedule
                {
                    ScheduleId = Guid.NewGuid()
                };

                schedule.Title = txtTitle.Text.Trim();
                schedule.VaccineId = (Guid)cmbVaccine.SelectedValue;
                schedule.Round = cmbRound.Text.Trim();
                schedule.Description = txtDescription.Text.Trim();
                schedule.StartDate = DateOnly.FromDateTime(dpStartDate.SelectedDate!.Value);
                schedule.EndDate = DateOnly.FromDateTime(dpEndDate.SelectedDate!.Value);
                schedule.TargetGrade = cmbTargetGrade.Text.Trim();

                if (_isEditMode)
                {
                    _vaccinationScheduleService.UpdateVaccinationSchedule(schedule);
                    MessageBox.Show("Cập nhật lịch tiêm thành công!", "Thông báo");
                }
                else
                {
                    _vaccinationScheduleService.AddVaccinationSchedule(schedule);
                    MessageBox.Show("Tạo lịch tiêm thành công!", "Thông báo");

                    if (chkAutoGenerate.IsChecked == true)
                    {
                        var students = await _studentService.GetAllStudents();
                        students = students.Where(s => schedule.TargetGrade.Trim().ToLower() == s.Grade!.Trim().ToLower()).ToList();
                        foreach (var student in students)
                        {
                            var healthProfile = _healthProfileService.GetHealthProfileByStudentId(student.StudentId);

                            var result = new VaccinationResult
                            {
                                VaccinationResultId = Guid.NewGuid(),
                                ScheduleId = schedule.ScheduleId,
                                HealthProfileId = healthProfile!.HealthProfileId,
                            };
                            _vaccinationResultService.AddVaccinationResult(result);
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
            if (cmbVaccine.SelectedValue == null) return false;
            if (!dpStartDate.SelectedDate.HasValue) return false;
            if (!dpEndDate.SelectedDate.HasValue) return false;
            if (string.IsNullOrWhiteSpace(cmbTargetGrade.Text)) return false;
            if (dpStartDate.SelectedDate > dpEndDate.SelectedDate) return false;

            return true;
        }
    }
}