using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Parent
{
    /// <summary>
    /// Interaction logic for HealthHistoryPage.xaml
    /// </summary>
    public partial class HealthHistoryPage : UserControl
    {
        #region Properties
        public List<Student> Students { get; set; } = [];
        public ObservableCollection<HealthCheckResult> AllHealthResults { get; set; } = [];
        public ObservableCollection<VaccinationResult> AllVaccinations { get; set; } = [];

        // Store original data for filtering
        private List<HealthCheckResult> _originalHealthResults = [];
        private List<VaccinationResult> _originalVaccinationResults = [];
        #endregion

        #region Private Fields
        private ContentControl _mainContent;
        private readonly UserService _userService;
        private readonly StudentService _studentService;
        private readonly HealthCheckResultService _healthCheckResultService;
        private readonly VaccinationResultService _vaccinationResultService;
        private readonly User _currentUser;
        #endregion

        #region Constructor
        public HealthHistoryPage(
            UserService userService,
            StudentService studentService,
            HealthCheckResultService healthCheckResultService,
            VaccinationResultService vaccinationResultService,
            User currentUser,
            ContentControl mainContent)
        {
            InitializeComponent();
            _userService = userService;
            _studentService = studentService;
            _healthCheckResultService = healthCheckResultService;
            _vaccinationResultService = vaccinationResultService;
            _currentUser = currentUser;
            _mainContent = mainContent;

            DataContext = this;
            Loaded += UserControl_Loaded;
        }
        #endregion

        #region Event Handlers
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStudents();
            LoadAllHealthHistory();
            LoadAllVaccinationHistory();
            UpdateSummaryInfo();
        }

        #region Health Check Tab Events
        private void HealthSearchButton_Click(object sender, RoutedEventArgs e)
        {
            FilterHealthResults();
        }

        private void HealthFromDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterHealthResults();
        }

        private void HealthToDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterHealthResults();
        }

        private void HealthStudentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterHealthResults();
        }
        #endregion

        #region Vaccination Tab Events
        private void VaccinationSearchButton_Click(object sender, RoutedEventArgs e)
        {
            FilterVaccinationResults();
        }

        private void SearchVaccineNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterVaccinationResults();
        }

        private void SearchManufacturerTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterVaccinationResults();
        }

        private void VaccinationStudentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterVaccinationResults();
        }
        #endregion

        #region Action Button Events
        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTab = HistoryTabControl.SelectedIndex;

            if (selectedTab == 0) // Health Check Tab
            {
                var selectedHealth = HealthHistoryDataGrid.SelectedItem as HealthCheckResult;
                if (selectedHealth == null)
                {
                    MessageBox.Show("Vui lòng chọn một bản ghi để xem chi tiết.", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                ShowHealthDetails(selectedHealth);
            }
            else if (selectedTab == 1) // Vaccination Tab
            {
                var selectedVaccination = VaccinationHistoryDataGrid.SelectedItem as VaccinationResult;
                if (selectedVaccination == null)
                {
                    MessageBox.Show("Vui lòng chọn một bản ghi để xem chi tiết.", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                ShowVaccinationDetails(selectedVaccination);
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTab = HistoryTabControl.SelectedIndex;

            if (selectedTab == 0) // Health Check Tab
            {
                ExportHealthReport();
            }
            else if (selectedTab == 1) // Vaccination Tab
            {
                ExportVaccinationReport();
            }
        }
        #endregion
        #endregion

        #region Private Methods
        private void LoadStudents()
        {
            try
            {
                var students = _studentService.GetStudents();
                Students = students.Where(s => s.UserId == _currentUser.UserId).ToList();

                // Add "Tất cả" option
                var allStudentsOption = new Student { StudentId = Guid.Empty, FullName = "Tất cả học sinh" };
                Students.Insert(0, allStudentsOption);

                // Set default selection
                HealthStudentComboBox.ItemsSource = Students;
                VaccinationStudentComboBox.ItemsSource = Students;



                HealthStudentComboBox.SelectedIndex = 0;
                VaccinationStudentComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách học sinh: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAllHealthHistory()
        {
            try
            {
                var allResults = _healthCheckResultService.GetAll();
                var userStudentIds = Students.Where(s => s.StudentId != Guid.Empty).Select(s => s.StudentId).ToList();

                // Get results for all user's students
                var userResults = new List<HealthCheckResult>();
                foreach (var studentId in userStudentIds)
                {
                    var student = _studentService.GetStudent(studentId);
                    if (student?.HealthProfiles != null && student.HealthProfiles.Any())
                    {
                        var healthProfileId = student.HealthProfiles.First().HealthProfileId;
                        var studentResults = allResults
                            .Where(r => r.HealthProfileId == healthProfileId)
                            .ToList();
                        userResults.AddRange(studentResults);
                    }
                }

                _originalHealthResults = userResults.OrderByDescending(r => r.DatePerformed).ToList();
                AllHealthResults = new ObservableCollection<HealthCheckResult>(_originalHealthResults);
                HealthHistoryDataGrid.ItemsSource = AllHealthResults;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải lịch sử khám sức khỏe: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAllVaccinationHistory()
        {
            try
            {
                var allResults = _vaccinationResultService.GetAllVaccinationResults();
                var userStudentIds = Students.Where(s => s.StudentId != Guid.Empty).Select(s => s.StudentId).ToList();

                // Get results for all user's students
                var userResults = new List<VaccinationResult>();
                foreach (var studentId in userStudentIds)
                {
                    var student = _studentService.GetStudent(studentId);
                    if (student?.HealthProfiles != null && student.HealthProfiles.Any())
                    {
                        var healthProfileId = student.HealthProfiles.First().HealthProfileId;
                        var studentResults = allResults
                            .Where(r => r.HealthProfileId == healthProfileId)
                            .ToList();
                        userResults.AddRange(studentResults);
                    }
                }

                _originalVaccinationResults = userResults.OrderByDescending(r => r.VaccinationDate).ToList();
                AllVaccinations = new ObservableCollection<VaccinationResult>(_originalVaccinationResults);
                VaccinationHistoryDataGrid.ItemsSource = AllVaccinations;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải lịch sử tiêm chủng: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterHealthResults()
        {
            try
            {
                if (!_originalHealthResults.Any()) return;

                var filteredResults = _originalHealthResults.AsEnumerable();

                // Filter by date range
                var fromDate = HealthFromDatePicker.SelectedDate;
                var toDate = HealthToDatePicker.SelectedDate;

                if (fromDate.HasValue)
                {
                    filteredResults = filteredResults.Where(r => r.DatePerformed >= DateOnly.FromDateTime(fromDate.Value));
                }

                if (toDate.HasValue)
                {
                    filteredResults = filteredResults.Where(r => r.DatePerformed <= DateOnly.FromDateTime(toDate.Value));
                }

                // Filter by student
                var selectedStudent = HealthStudentComboBox.SelectedItem as Student;
                if (selectedStudent != null && selectedStudent.StudentId != Guid.Empty)
                {
                    filteredResults = filteredResults.Where(r =>
                        r.HealthProfile?.Student?.StudentId == selectedStudent.StudentId);
                }

                var finalResults = filteredResults
                    .OrderByDescending(r => r.DatePerformed)
                    .ToList();

                AllHealthResults.Clear();
                foreach (var result in finalResults)
                {
                    AllHealthResults.Add(result);
                }

                // Update summary
                HealthRecordCountTextBlock.Text = AllHealthResults.Count.ToString();
                var uniqueStudents = AllHealthResults.Select(r => r.HealthProfile?.Student?.StudentId).Distinct().Count();
                HealthStudentCountTextBlock.Text = uniqueStudents.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lọc dữ liệu khám sức khỏe: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterVaccinationResults()
        {
            try
            {
                if (!_originalVaccinationResults.Any()) return;

                var filteredResults = _originalVaccinationResults.AsEnumerable();

                // Filter by vaccine name
                var vaccineName = SearchVaccineNameTextBox.Text?.Trim();
                if (!string.IsNullOrEmpty(vaccineName))
                {
                    filteredResults = filteredResults.Where(r =>
                        r.Schedule?.Vaccine?.VaccineName?.Contains(vaccineName, StringComparison.OrdinalIgnoreCase) == true);
                }

                // Filter by manufacturer
                var manufacturer = SearchManufacturerTextBox.Text?.Trim();
                if (!string.IsNullOrEmpty(manufacturer))
                {
                    filteredResults = filteredResults.Where(r =>
                        r.Schedule?.Vaccine?.Manufacturer?.Contains(manufacturer, StringComparison.OrdinalIgnoreCase) == true);
                }

                // Filter by student
                var selectedStudent = VaccinationStudentComboBox.SelectedItem as Student;
                if (selectedStudent != null && selectedStudent.StudentId != Guid.Empty)
                {
                    filteredResults = filteredResults.Where(r =>
                        r.HealthProfile?.Student?.StudentId == selectedStudent.StudentId);
                }

                var finalResults = filteredResults
                    .OrderByDescending(r => r.VaccinationDate)
                    .ToList();

                AllVaccinations.Clear();
                foreach (var result in finalResults)
                {
                    AllVaccinations.Add(result);
                }

                // Update summary
                VaccinationRecordCountTextBlock.Text = AllVaccinations.Count.ToString();
                var uniqueStudents = AllVaccinations.Select(r => r.HealthProfile?.Student?.StudentId).Distinct().Count();
                VaccinationStudentCountTextBlock.Text = uniqueStudents.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lọc dữ liệu tiêm chủng: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateSummaryInfo()
        {
            // Update health summary
            HealthRecordCountTextBlock.Text = AllHealthResults.Count.ToString();
            var uniqueHealthStudents = AllHealthResults.Select(r => r.HealthProfile?.Student?.StudentId).Distinct().Where(id => id.HasValue).Count();
            HealthStudentCountTextBlock.Text = uniqueHealthStudents.ToString();

            // Update vaccination summary
            VaccinationRecordCountTextBlock.Text = AllVaccinations.Count.ToString();
            var uniqueVaccinationStudents = AllVaccinations.Select(r => r.HealthProfile?.Student?.StudentId).Distinct().Where(id => id.HasValue).Count();
            VaccinationStudentCountTextBlock.Text = uniqueVaccinationStudents.ToString();
        }

        private void ShowHealthDetails(HealthCheckResult healthResult)
        {
            var studentName = healthResult.HealthProfile?.Student?.FullName ?? "N/A";
            var studentClass = healthResult.HealthProfile?.Student?.Grade ?? "N/A";

            var details = $"Chi tiết khám sức khỏe\n\n" +
                         $"Học sinh: {studentName} - Lớp: {studentClass}\n" +
                         $"Ngày khám: {healthResult.DatePerformed:dd/MM/yyyy}\n" +
                         $"Chiều cao: {healthResult.Height} cm\n" +
                         $"Cân nặng: {healthResult.Weight} kg\n" +
                         $"Thị lực trái: {healthResult.VisionLeft}\n" +
                         $"Thị lực phải: {healthResult.VisionRight}\n" +
                         $"Thính lực: {healthResult.Hearing}\n" +
                         $"Mũi họng: {healthResult.Nose}\n" +
                         $"Huyết áp: {healthResult.BloodPressure}\n" +
                         $"Ghi chú: {healthResult.Notes}";

            MessageBox.Show(details, "Chi tiết khám sức khỏe",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowVaccinationDetails(VaccinationResult vaccinationResult)
        {
            var studentName = vaccinationResult.HealthProfile?.Student?.FullName ?? "N/A";
            var studentClass = vaccinationResult.HealthProfile?.Student?.Grade ?? "N/A";
            var vaccineName = vaccinationResult.Schedule?.Vaccine?.VaccineName ?? "N/A";
            var manufacturer = vaccinationResult.Schedule?.Vaccine?.Manufacturer ?? "N/A";

            var details = $"Chi tiết tiêm chủng\n\n" +
                         $"Học sinh: {studentName} - Lớp: {studentClass}\n" +
                         $"Ngày tiêm: {vaccinationResult.VaccinationDate:dd/MM/yyyy}\n" +
                         $"Tên vaccine: {vaccineName}\n" +
                         $"Liều số: {vaccinationResult.DoseNumber}\n" +
                         $"Nhà sản xuất: {manufacturer}\n" +
                         $"Số lô: {vaccinationResult.Schedule.Vaccine.BatchNumber}\n" +
                         $"Vị trí tiêm: {vaccinationResult.InjectionSite}\n" +
                         //$"Phản ứng: {vaccinationResult.Reaction ?? "Không có"}\n" +
                         $"Ghi chú: {vaccinationResult.Notes}";

            MessageBox.Show(details, "Chi tiết tiêm chủng",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExportHealthReport()
        {
            try
            {
                var recordCount = AllHealthResults.Count;
                var studentCount = AllHealthResults.Select(r => r.HealthProfile?.Student?.StudentId).Distinct().Where(id => id.HasValue).Count();

                var summary = $"Báo cáo lịch sử khám sức khỏe\n\n" +
                             $"Tổng số bản ghi: {recordCount}\n" +
                             $"Số học sinh: {studentCount}\n" +
                             $"Ngày xuất báo cáo: {DateTime.Now:dd/MM/yyyy HH:mm}";

                MessageBox.Show(summary, "Xuất báo cáo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất báo cáo: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportVaccinationReport()
        {
            try
            {
                var recordCount = AllVaccinations.Count;
                var studentCount = AllVaccinations.Select(r => r.HealthProfile?.Student?.StudentId).Distinct().Where(id => id.HasValue).Count();

                var summary = $"Báo cáo lịch sử tiêm chủng\n\n" +
                             $"Tổng số mũi tiêm: {recordCount}\n" +
                             $"Số học sinh: {studentCount}\n" +
                             $"Ngày xuất báo cáo: {DateTime.Now:dd/MM/yyyy HH:mm}";

                MessageBox.Show(summary, "Xuất báo cáo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất báo cáo: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}