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
        public ObservableCollection<MedicalEvent> AllMedicalEvents { get; set; } = []; // Add this

        // Store original data for filtering
        private List<HealthCheckResult> _originalHealthResults = [];
        private List<VaccinationResult> _originalVaccinationResults = [];
        private List<MedicalEvent> _originalMedicalEvents = []; // Add this
        #endregion

        #region Private Fields
        private ContentControl _mainContent;
        private readonly UserService _userService;
        private readonly StudentService _studentService;
        private readonly HealthCheckResultService _healthCheckResultService;
        private readonly VaccinationResultService _vaccinationResultService;
        private readonly MedicalEventService _medicalEventService; // Add this
        private readonly User _currentUser;
        #endregion

        #region Constructor
        public HealthHistoryPage(
            UserService userService,
            StudentService studentService,
            HealthCheckResultService healthCheckResultService,
            VaccinationResultService vaccinationResultService,
            MedicalEventService medicalEventService, // Add this parameter
            User currentUser,
            ContentControl mainContent)
        {
            InitializeComponent();
            _userService = userService;
            _studentService = studentService;
            _healthCheckResultService = healthCheckResultService;
            _vaccinationResultService = vaccinationResultService;
            _medicalEventService = medicalEventService; // Add this
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
            LoadAllMedicalEvents(); // Add this
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

        #region Medical Events Tab Events - ADD THESE
        private void EventFromDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterMedicalEvents();
        }

        private void EventToDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterMedicalEvents();
        }

        private void EventTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterMedicalEvents();
        }

        private void EventStudentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterMedicalEvents();
        }

        private void EventSearchButton_Click(object sender, RoutedEventArgs e)
        {
            FilterMedicalEvents();
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
            else if (selectedTab == 2) // Medical Events Tab - ADD THIS
            {
                var selectedEvent = MedicalEventsDataGrid.SelectedItem as MedicalEvent;
                if (selectedEvent == null)
                {
                    MessageBox.Show("Vui lòng chọn một sự kiện để xem chi tiết.", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                ShowMedicalEventDetails(selectedEvent);
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
            else if (selectedTab == 2) // Medical Events Tab - ADD THIS
            {
                ExportMedicalEventsReport();
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

                // Set default selection for all ComboBoxes
                HealthStudentComboBox.ItemsSource = Students;
                VaccinationStudentComboBox.ItemsSource = Students;
                EventStudentComboBox.ItemsSource = Students; // Add this

                HealthStudentComboBox.SelectedIndex = 0;
                VaccinationStudentComboBox.SelectedIndex = 0;
                EventStudentComboBox.SelectedIndex = 0; // Add this
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

                        // Ensure navigation properties are populated
                        foreach (var result in studentResults)
                        {
                            if (result.HealthProfile == null)
                            {
                                result.HealthProfile = student.HealthProfiles.First();
                            }
                            if (result.HealthProfile.Student == null)
                            {
                                result.HealthProfile.Student = student;
                            }
                        }

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

                        // Ensure navigation properties are populated
                        foreach (var result in studentResults)
                        {
                            if (result.HealthProfile == null)
                            {
                                result.HealthProfile = student.HealthProfiles.First();
                            }
                            if (result.HealthProfile.Student == null)
                            {
                                result.HealthProfile.Student = student;
                            }
                        }

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

        // ADD THIS NEW METHOD
        private void LoadAllMedicalEvents()
        {
            try
            {
                var allEvents = _medicalEventService.GetAllMedicalEvents();
                var userStudentIds = Students.Where(s => s.StudentId != Guid.Empty).Select(s => s.StudentId).ToList();

                // Get events for all user's students
                var userEvents = allEvents
                    .Where(e => userStudentIds.Contains((Guid)e.StudentId))
                    .ToList();

                // Ensure Student data is populated
                foreach (var medEvent in userEvents)
                {
                    if (medEvent.Student == null)
                    {
                        medEvent.Student = Students.FirstOrDefault(s => s.StudentId == medEvent.StudentId);
                    }
                }

                _originalMedicalEvents = userEvents.OrderByDescending(e => e.EventDate).ToList();
                AllMedicalEvents = new ObservableCollection<MedicalEvent>(_originalMedicalEvents);
                MedicalEventsDataGrid.ItemsSource = AllMedicalEvents;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải sự kiện y tế: {ex.Message}", "Lỗi",
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
                UpdateHealthSummary();
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
                UpdateVaccinationSummary();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lọc dữ liệu tiêm chủng: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ADD THIS NEW METHOD
        private void FilterMedicalEvents()
        {
            try
            {
                if (!_originalMedicalEvents.Any()) return;

                var filteredEvents = _originalMedicalEvents.AsEnumerable();

                // Filter by date range
                var fromDate = EventFromDatePicker.SelectedDate;
                var toDate = EventToDatePicker.SelectedDate;

                if (fromDate.HasValue)
                {
                    filteredEvents = filteredEvents.Where(e => e.EventDate >= DateOnly.FromDateTime(fromDate.Value));
                }

                if (toDate.HasValue)
                {
                    filteredEvents = filteredEvents.Where(e => e.EventDate <= DateOnly.FromDateTime(toDate.Value));
                }

                // Filter by event type
                var selectedEventType = (EventTypeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                if (!string.IsNullOrEmpty(selectedEventType) && selectedEventType != "Tất cả")
                {
                    filteredEvents = filteredEvents.Where(e => e.EventType == selectedEventType);
                }

                // Filter by student
                var selectedStudent = EventStudentComboBox.SelectedItem as Student;
                if (selectedStudent != null && selectedStudent.StudentId != Guid.Empty)
                {
                    filteredEvents = filteredEvents.Where(e => e.StudentId == selectedStudent.StudentId);
                }

                var finalEvents = filteredEvents
                    .OrderByDescending(e => e.EventDate)
                    .ToList();

                AllMedicalEvents.Clear();
                foreach (var medEvent in finalEvents)
                {
                    AllMedicalEvents.Add(medEvent);
                }

                // Update summary
                UpdateMedicalEventSummary();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lọc sự kiện y tế: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ADD THESE SUMMARY METHODS
        private void UpdateHealthSummary()
        {
            HealthRecordCountTextBlock.Text = AllHealthResults.Count.ToString();
            var uniqueStudents = AllHealthResults.Select(r => r.HealthProfile?.Student?.StudentId).Distinct().Where(id => id.HasValue).Count();
            HealthStudentCountTextBlock.Text = uniqueStudents.ToString();
        }

        private void UpdateVaccinationSummary()
        {
            VaccinationRecordCountTextBlock.Text = AllVaccinations.Count.ToString();
            var uniqueStudents = AllVaccinations.Select(r => r.HealthProfile?.Student?.StudentId).Distinct().Where(id => id.HasValue).Count();
            VaccinationStudentCountTextBlock.Text = uniqueStudents.ToString();
        }

        private void UpdateMedicalEventSummary()
        {
            EventRecordCountTextBlock.Text = AllMedicalEvents.Count.ToString();
            var uniqueStudents = AllMedicalEvents.Select(e => e.StudentId).Distinct().Count();
            EventStudentCountTextBlock.Text = uniqueStudents.ToString();
            var severeEvents = AllMedicalEvents.Count(e => e.SeverityLevel == "Nghiêm trọng");
            EventSevereCountTextBlock.Text = severeEvents.ToString();
        }

        private void UpdateSummaryInfo()
        {
            UpdateHealthSummary();
            UpdateVaccinationSummary();
            UpdateMedicalEventSummary(); // Add this
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
                         $"Số lô: {vaccinationResult.Schedule?.Vaccine?.BatchNumber ?? "N/A"}\n" +
                         $"Vị trí tiêm: {vaccinationResult.InjectionSite}\n" +
                         $"Ghi chú: {vaccinationResult.Notes}";

            MessageBox.Show(details, "Chi tiết tiêm chủng",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ADD THIS NEW METHOD
        private void ShowMedicalEventDetails(MedicalEvent medicalEvent)
        {
            var studentName = medicalEvent.Student?.FullName ?? "N/A";
            var studentClass = medicalEvent.Student?.Grade ?? "N/A";

            var details = $"Chi tiết sự kiện y tế\n\n" +
                         $"Học sinh: {studentName} - Lớp: {studentClass}\n" +
                         $"Ngày giờ: {medicalEvent.EventDate:dd/MM/yyyy HH:mm}\n" +
                         $"Loại sự kiện: {medicalEvent.EventType}\n" +
                         $"Mô tả: {medicalEvent.EventDescription}\n" +
                         $"Địa điểm: {medicalEvent.Location}\n" +
                         $"Mức độ nghiêm trọng: {medicalEvent.SeverityLevel}\n" +
                         $"Y tá xử lý: {medicalEvent.StaffNurse?.FullName ?? "N/A"}\n" +
                         $"Ghi chú: {medicalEvent.Notes}";

            MessageBox.Show(details, "Chi tiết sự kiện y tế",
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

        // ADD THIS NEW METHOD
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

        // ADD THIS NEW METHOD
        private void ExportMedicalEventsReport()
        {
            try
            {
                var recordCount = AllMedicalEvents.Count;
                var studentCount = AllMedicalEvents.Select(e => e.StudentId).Distinct().Count();
                var severeCount = AllMedicalEvents.Count(e => e.SeverityLevel == "Nghiêm trọng");

                var summary = $"Báo cáo sự kiện y tế\n\n" +
                             $"Tổng số sự kiện: {recordCount}\n" +
                             $"Số học sinh: {studentCount}\n" +
                             $"Sự kiện nghiêm trọng: {severeCount}\n" +
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