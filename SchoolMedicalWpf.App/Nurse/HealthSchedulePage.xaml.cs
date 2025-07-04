using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;
using SchoolMedicalWpf.Dal.Repositories;

namespace SchoolMedicalWpf.App.Nurse
{
    public partial class HealthSchedulePage : UserControl
    {
        private ObservableCollection<StudentResultItem> _studentResults;
        private ScheduleItem _currentSchedule;
        private readonly User _currentUser;
        private readonly DateTime _currentDateTime = DateTime.UtcNow;

        private readonly UserService _userService;
        private readonly StudentService _studentService;
        private readonly HealthProfileService _healthProfileService;
        private readonly HealthCheckScheduleService _healthCheckScheduleService;
        private readonly VaccinationScheduleService _vaccinationScheduleService;
        private readonly HealthCheckResultService _healthCheckResultService;
        private readonly VaccinationResultService _vaccinationResultService;

        public HealthSchedulePage(User currentUser,
                                 UserService userService,
                                 StudentService studentService,
                                 HealthProfileService healthProfileService,
                                 HealthCheckScheduleService healthCheckScheduleService,
                                 VaccinationScheduleService vaccinationScheduleService,
                                 HealthCheckResultService healthCheckResultService,
                                 VaccinationResultService vaccinationResultService)
        {
            InitializeComponent();
            _currentUser = currentUser;

            // Inject services
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
            _healthProfileService = healthProfileService ?? throw new ArgumentNullException(nameof(healthProfileService));
            _healthCheckScheduleService = healthCheckScheduleService ?? throw new ArgumentNullException(nameof(healthCheckScheduleService));
            _vaccinationScheduleService = vaccinationScheduleService ?? throw new ArgumentNullException(nameof(vaccinationScheduleService));
            _healthCheckResultService = healthCheckResultService ?? throw new ArgumentNullException(nameof(healthCheckResultService));
            _vaccinationResultService = vaccinationResultService ?? throw new ArgumentNullException(nameof(vaccinationResultService));

            _studentResults = new ObservableCollection<StudentResultItem>();
            dgStudents.ItemsSource = _studentResults;
            dgResultsSummary.ItemsSource = _studentResults;

            InitializePage();
        }

        private void InitializePage()
        {
            txtCurrentDateTime.Text = $"{_currentDateTime:yyyy-MM-dd HH:mm:ss} UTC - User: {_currentUser.FullName}";
            UpdateLastUpdateTime("Khởi tạo trang");
            LoadActiveSchedulesCount();
        }

        private async void LoadActiveSchedulesCount()
        {
            try
            {
                var activeSchedules = await GetActiveSchedules();
                txtActiveSchedulesCount.Text = $"({activeSchedules.Count} schedule đang diễn ra)";
            }
            catch (Exception ex)
            {
                txtActiveSchedulesCount.Text = "(Lỗi tải dữ liệu)";
                Console.WriteLine($"Error loading active schedules count: {ex.Message}");
            }
        }

        private async void SelectSchedule_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var activeSchedules = await GetActiveSchedules();

                if (!activeSchedules.Any())
                {
                    MessageBox.Show("Hiện tại không có schedule nào đang diễn ra!", "Thông báo");
                    return;
                }

                var dialog = new ScheduleSelectionDialog(activeSchedules);
                if (dialog.ShowDialog() == true)
                {
                    _currentSchedule = dialog.SelectedSchedule;
                    ShowSelectedScheduleInfo();
                    ClearStudentData();
                    UpdateLastUpdateTime("Đã chọn schedule");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách schedule: {ex.Message}", "Lỗi");
            }
        }

        private void ShowSelectedScheduleInfo()
        {
            if (_currentSchedule != null)
            {
                txtSelectedScheduleTitle.Text = $"{_currentSchedule.Title} ({_currentSchedule.Type})";
                txtSelectedScheduleInfo.Text = $"Từ {_currentSchedule.StartDate:dd/MM/yyyy} đến {_currentSchedule.EndDate:dd/MM/yyyy} | " +
                                             $"Đối tượng: {_currentSchedule.TargetGrade}";

                pnlSelectedSchedule.Visibility = Visibility.Visible;
                txtNoScheduleMessage.Visibility = Visibility.Collapsed;
            }
        }

        private async void LoadStudents_Click(object sender, RoutedEventArgs e)
        {
            if (_currentSchedule == null) return;

            try
            {
                btnLoadStudents.IsEnabled = false;
                btnLoadStudents.Content = "⏳ Đang tải...";

                await LoadStudentsForSchedule();

                btnLoadStudents.Content = "✅ Đã tải xong";
                btnSaveAll.IsEnabled = true;
                UpdateLastUpdateTime($"Đã tải {_studentResults.Count} sinh viên");

                await Task.Delay(2000);
                btnLoadStudents.Content = "👥 Tải lại danh sách";
                btnLoadStudents.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách sinh viên: {ex.Message}", "Lỗi");
                btnLoadStudents.Content = "👥 Tải danh sách sinh viên";
                btnLoadStudents.IsEnabled = true;
            }
        }

        private async Task LoadStudentsForSchedule()
        {
            _studentResults.Clear();

            // Get students based on schedule target grade
            var students = await GetStudentsForSchedule(_currentSchedule);

            foreach (var student in students)
            {
                // Get health profile for student
                var healthProfile = Task.Run(() => _healthProfileService.GetHealthProfileByStudentId(student.StudentId)).Result;
                if (healthProfile == null)
                {
                    Console.WriteLine($"Warning: No health profile found for student {student.StudentCode}");
                    continue;
                }

                var resultItem = new StudentResultItem
                {
                    StudentId = student.StudentId,
                    StudentCode = student.StudentCode!,
                    StudentName = $"{student.FullName}",
                    Grade = student.Gender!,
                    HealthProfileId = healthProfile.HealthProfileId,
                    ScheduleId = _currentSchedule.Id,
                    ScheduleType = _currentSchedule.Type,
                    IsCompleted = false,
                    DatePerformed = _currentDateTime.Date,
                    Notes = "",
                    Status = "Chưa thực hiện"
                };
                await LoadExistingResults(resultItem);

                _studentResults.Add(resultItem);
            }

            ShowDataGrids();
            UpdateStatistics();
        }

        private async Task LoadExistingResults(StudentResultItem item)
        {
            try
            {
                if (item.ScheduleType == "HealthCheck")
                {
                    var existingResults = await Task.Run(() => _healthCheckResultService.GetAll());
                    existingResults = existingResults.Where(r => r.ScheduleId == item.ScheduleId &&
                                                                 r.HealthProfileId == item.HealthProfileId).ToList();

                    if (existingResults.Any())
                    {
                        var latestResult = existingResults.OrderByDescending(r => r.DatePerformed).First();
                        item.IsCompleted = true;
                        item.Status = "Đã có kết quả";
                        item.DatePerformed = latestResult.DatePerformed?.ToDateTime(TimeOnly.MinValue) ?? _currentDateTime.Date;
                        item.Notes = latestResult.Notes ?? "";
                        item.HasExistingHealthData = true;

                        // Load health check specific data
                        item.Height = latestResult.Height;
                        item.Weight = latestResult.Weight;
                        item.VisionLeft = latestResult.VisionLeft;
                        item.VisionRight = latestResult.VisionRight;
                        item.Hearing = latestResult.Hearing!;
                        item.Nose = latestResult.Nose!;
                        item.BloodPressure = latestResult.BloodPressure!;
                    }
                }
                else if (item.ScheduleType == "Vaccination")
                {
                    var existingResults = await Task.Run(() => _vaccinationResultService.GetAllVaccinationResults());
                    existingResults = existingResults.Where(r => r.ScheduleId == item.ScheduleId &&
                                                                 r.HealthProfileId == item.HealthProfileId).ToList();

                    if (existingResults.Any())
                    {
                        var latestResult = existingResults.OrderByDescending(r => r.VaccinationDate).First();
                        item.IsCompleted = true;
                        item.Status = "Đã có kết quả";
                        item.DatePerformed = latestResult.VaccinationDate?.ToDateTime(TimeOnly.MinValue) ?? _currentDateTime.Date;
                        item.Notes = latestResult.Notes ?? "";
                        item.HasExistingVaccinationData = true;

                        // Load vaccination specific data
                        item.DoseNumber = latestResult.DoseNumber;
                        item.InjectionSite = latestResult.InjectionSite!;
                        item.ImmediateReaction = latestResult.ImmediateReaction!;
                        item.ReactionStartTime = latestResult.ReactionStartTime!;
                        item.ReactionType = latestResult.ReactionType!;
                        item.SeverityLevel = latestResult.SeverityLevel!;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading existing results for {item.StudentName}: {ex.Message}");
            }
        }

        private void EnterResults_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var resultItem = button?.DataContext as StudentResultItem;

            if (resultItem != null && resultItem.IsCompleted)
            {
                OpenResultWindow(resultItem);
            }
            else
            {
                MessageBox.Show("Vui lòng đánh dấu 'Đã thực hiện' trước khi nhập kết quả!", "Thông báo");
            }
        }

        private void OpenResultWindow(StudentResultItem resultItem)
        {
            try
            {
                if (_currentSchedule.Type == "HealthCheck")
                {
                    var healthCheckWindow = new HealthCheckResultWindow(
                        healthProfileId: resultItem.HealthProfileId,
                        scheduleId: resultItem.ScheduleId,
                        studentCode: resultItem.StudentCode,
                        studentName: resultItem.StudentName,
                        grade: resultItem.Grade,
                        currentUser: _currentUser,
                        healthCheckResultService: _healthCheckResultService
                    );

                    if (healthCheckWindow.ShowDialog() == true)
                    {
                        var savedResult = healthCheckWindow.Result;
                        UpdateStudentResultFromHealthCheck(resultItem, savedResult);

                        resultItem.Status = "Đã lưu kết quả khám";
                        UpdateStatistics();
                        UpdateLastUpdateTime($"Đã lưu kết quả khám cho {resultItem.StudentName}");

                        MessageBox.Show($"✅ Đã lưu kết quả khám sức khỏe cho {resultItem.StudentName}!",
                                      "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else if (_currentSchedule.Type == "Vaccination")
                {
                    var vaccinationWindow = new VaccinationResultWindow(
                        healthProfileId: resultItem.HealthProfileId,
                        scheduleId: resultItem.ScheduleId,
                        studentCode: resultItem.StudentCode,
                        studentName: resultItem.StudentName,
                        grade: resultItem.Grade,
                        currentUser: _currentUser,
                        vaccinationResultService: _vaccinationResultService
                    );

                    if (vaccinationWindow.ShowDialog() == true)
                    {
                        var savedResult = vaccinationWindow.Result;
                        UpdateStudentResultFromVaccination(resultItem, savedResult);

                        resultItem.Status = "Đã lưu kết quả tiêm";
                        UpdateStatistics();
                        UpdateLastUpdateTime($"Đã lưu kết quả tiêm cho {resultItem.StudentName}");

                        MessageBox.Show($"✅ Đã lưu kết quả tiêm chủng cho {resultItem.StudentName}!",
                                      "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi mở cửa sổ nhập kết quả: {ex.Message}",
                              "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateStudentResultFromHealthCheck(StudentResultItem item, HealthCheckResult result)
        {
            item.DatePerformed = result.DatePerformed?.ToDateTime(TimeOnly.MinValue) ?? _currentDateTime.Date;
            item.Notes = result.Notes ?? "";

            // Update additional health check specific data
            item.Height = result.Height;
            item.Weight = result.Weight;
            item.VisionLeft = result.VisionLeft;
            item.VisionRight = result.VisionRight;
            item.Hearing = result.Hearing;
            item.Nose = result.Nose;
            item.BloodPressure = result.BloodPressure;
            item.HasExistingHealthData = true;
        }

        private void UpdateStudentResultFromVaccination(StudentResultItem item, VaccinationResult result)
        {
            item.DatePerformed = result.VaccinationDate?.ToDateTime(TimeOnly.MinValue) ?? _currentDateTime.Date;
            item.Notes = result.Notes ?? "";

            // Update additional vaccination specific data
            item.DoseNumber = result.DoseNumber;
            item.InjectionSite = result.InjectionSite;
            item.ImmediateReaction = result.ImmediateReaction;
            item.ReactionStartTime = result.ReactionStartTime;
            item.ReactionType = result.ReactionType;
            item.SeverityLevel = result.SeverityLevel;
            item.HasExistingVaccinationData = true;
        }

        private async void SaveAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var completedResults = _studentResults.Where(x => x.IsCompleted &&
                    x.Status != "Đã lưu kết quả khám" &&
                    x.Status != "Đã lưu kết quả tiêm" &&
                    x.Status != "Đã có kết quả").ToList();

                if (!completedResults.Any())
                {
                    MessageBox.Show("Không có kết quả mới nào để lưu!", "Thông báo");
                    return;
                }

                var result = MessageBox.Show(
                    $"Bạn có {completedResults.Count} kết quả chưa được nhập chi tiết.\n" +
                    $"Bạn có muốn mở từng cửa sổ để nhập kết quả chi tiết không?\n\n" +
                    $"Chọn 'Yes' để nhập từng kết quả\n" +
                    $"Chọn 'No' để lưu với thông tin cơ bản",
                    "Nhập kết quả chi tiết", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.Cancel)
                    return;

                if (result == MessageBoxResult.Yes)
                {
                    // Open detail windows for each result
                    foreach (var item in completedResults)
                    {
                        var confirmResult = MessageBox.Show(
                            $"Nhập kết quả chi tiết cho {item.StudentName} ({item.StudentCode})?",
                            "Xác nhận", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                        if (confirmResult == MessageBoxResult.Cancel)
                            break;

                        if (confirmResult == MessageBoxResult.Yes)
                        {
                            OpenResultWindow(item);
                        }
                        else
                        {
                            // Save basic info only
                            await SaveBasicResultItem(item);
                            item.Status = "Đã lưu cơ bản";
                        }
                    }
                }
                else
                {
                    // Save basic info for all
                    btnSaveAll.IsEnabled = false;
                    btnSaveAll.Content = "⏳ Đang lưu...";

                    int savedCount = 0;
                    foreach (var item in completedResults)
                    {
                        await SaveBasicResultItem(item);
                        item.Status = "Đá lưu cơ bản";
                        savedCount++;
                    }

                    MessageBox.Show($"Đã lưu thông tin cơ bản cho {savedCount} sinh viên!", "Thành công");
                    btnSaveAll.Content = "💾 Lưu tất cả";
                    btnSaveAll.IsEnabled = true;
                }

                UpdateStatistics();
                UpdateLastUpdateTime($"Đã xử lý {completedResults.Count} kết quả");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu: {ex.Message}", "Lỗi");
                btnSaveAll.Content = "💾 Lưu tất cả";
                btnSaveAll.IsEnabled = true;
            }
        }

        private async Task SaveBasicResultItem(StudentResultItem item)
        {
            try
            {
                var recordedId = _currentUser.UserId;

                if (item.ScheduleType == "HealthCheck")
                {
                    var healthResult = new HealthCheckResult
                    {
                        ResultId = Guid.NewGuid(),
                        ScheduleId = item.ScheduleId,
                        HealthProfileId = item.HealthProfileId,
                        DatePerformed = DateOnly.FromDateTime(item.DatePerformed),
                        Notes = $"[Lưu cơ bản từ HealthSchedulePage lúc {_currentDateTime:yyyy-MM-dd HH:mm:ss}] {item.Notes}",
                        RecordedId = recordedId
                    };

                    await Task.Run(() => _healthCheckResultService.Add(healthResult));
                    Console.WriteLine($"[{_currentDateTime:yyyy-MM-dd HH:mm:ss}] Saved basic health check for {item.StudentName} by {_currentUser}");
                }
                else if (item.ScheduleType == "Vaccination")
                {
                    var vaccinationResult = new VaccinationResult
                    {
                        VaccinationResultId = Guid.NewGuid(),
                        ScheduleId = item.ScheduleId,
                        HealthProfileId = item.HealthProfileId,
                        VaccinationDate = DateOnly.FromDateTime(item.DatePerformed),
                        Notes = $"[Lưu cơ bản từ HealthSchedulePage lúc {_currentDateTime:yyyy-MM-dd HH:mm:ss}] {item.Notes}",
                        RecordedId = recordedId
                    };

                    await Task.Run(() => _vaccinationResultService.AddVaccinationResult(vaccinationResult)); // Wrap in Task.Run to avoid CS4008
                    Console.WriteLine($"[{_currentDateTime:yyyy-MM-dd HH:mm:ss}] Saved basic vaccination for {item.StudentName} by {_currentUser}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving basic result for {item.StudentName}: {ex.Message}");
                throw;
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (_currentSchedule != null)
            {
                LoadStudents_Click(null, null);
            }
            else
            {
                LoadActiveSchedulesCount();
                UpdateLastUpdateTime("Đã làm mới");
            }
        }

        private void ShowDataGrids()
        {
            tcMain.Visibility = Visibility.Visible;
            pnlNoData.Visibility = Visibility.Collapsed;
        }

        private void ClearStudentData()
        {
            _studentResults.Clear();
            tcMain.Visibility = Visibility.Collapsed;
            pnlNoData.Visibility = Visibility.Visible;
            btnSaveAll.IsEnabled = false;
        }

        private void UpdateStatistics()
        {
            var total = _studentResults.Count;
            var completed = _studentResults.Count(x => x.IsCompleted);
            var pending = total - completed;

            txtTotalStudents.Text = total.ToString();
            txtCompletedStudents.Text = completed.ToString();
            txtPendingStudents.Text = pending.ToString();
        }

        private void UpdateLastUpdateTime(string action)
        {
            txtLastUpdate.Text = $"{action} lúc {_currentDateTime:HH:mm:ss dd/MM/yyyy}";
        }

        #region Data Access Methods

        private async Task<List<ScheduleItem>> GetActiveSchedules()
        {
            var activeSchedules = new List<ScheduleItem>();
            var currentDate = _currentDateTime.Date;

            try
            {
                // Get active health check schedules
                var healthCheckSchedules = await Task.Run(() => _healthCheckScheduleService.GetAllHealthCheckSchedules());
                healthCheckSchedules = healthCheckSchedules
                    .Where(s => s.StartDate?.ToDateTime(TimeOnly.MinValue) <= currentDate &&
                                s.EndDate?.ToDateTime(TimeOnly.MinValue) >= currentDate)
                    .ToList();
                foreach (var schedule in healthCheckSchedules)
                {
                    activeSchedules.Add(new ScheduleItem
                    {
                        Id = schedule.ScheduleId,
                        Title = schedule.Title,
                        Type = "HealthCheck",
                        StartDate = schedule.StartDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue,
                        EndDate = schedule.EndDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MaxValue,
                        TargetGrade = schedule.TargetGrade ?? "Tất cả",
                        Description = schedule.Description ?? ""
                    });
                }

                // Get active vaccination schedules
                var vaccinationSchedules = await Task.Run(() => _vaccinationScheduleService.GetAllVaccinationSchedules());
                vaccinationSchedules = vaccinationSchedules
                    .Where(s => s.StartDate?.ToDateTime(TimeOnly.MinValue) <= currentDate &&
                                s.EndDate?.ToDateTime(TimeOnly.MinValue) >= currentDate)
                    .ToList();
                foreach (var schedule in vaccinationSchedules)
                {
                    activeSchedules.Add(new ScheduleItem
                    {
                        Id = schedule.ScheduleId,
                        Title = schedule.Title,
                        Type = "Vaccination",
                        StartDate = schedule.StartDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue,
                        EndDate = schedule.EndDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MaxValue,
                        TargetGrade = schedule.TargetGrade ?? "Tất cả",
                        Description = schedule.Description ?? ""
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting active schedules: {ex.Message}");
            }

            return activeSchedules;
        }

        private async Task<List<Student>> GetStudentsForSchedule(ScheduleItem schedule)
        {
            try
            {
                if (schedule.TargetGrade == "Tất cả" || schedule.TargetGrade == "Tất cả các lớp")
                {
                    return await _studentService.GetAllStudents();
                }
                else
                {
                    var students = await _studentService.GetAllStudents();
                    return students.Where(s => s.Grade == schedule.TargetGrade).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting students for schedule: {ex.Message}");
                return new List<Student>();
            }
        }

        #endregion
    }

    #region Supporting Classes

    public class ScheduleItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TargetGrade { get; set; }
        public string Description { get; set; }
    }

    public class StudentResultItem : INotifyPropertyChanged
    {
        private bool _isCompleted;
        private DateTime _datePerformed;
        private string _notes;
        private string _status;

        // Basic Info
        public Guid StudentId { get; set; }
        public string StudentCode { get; set; }
        public string StudentName { get; set; }
        public string Grade { get; set; }
        public Guid HealthProfileId { get; set; }
        public Guid ScheduleId { get; set; }
        public string ScheduleType { get; set; }

        public bool IsCompleted
        {
            get => _isCompleted;
            set { _isCompleted = value; OnPropertyChanged(); }
        }

        public DateTime DatePerformed
        {
            get => _datePerformed;
            set { _datePerformed = value; OnPropertyChanged(); }
        }

        public string Notes
        {
            get => _notes;
            set { _notes = value; OnPropertyChanged(); }
        }

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        // Health Check Results
        public double? Height { get; set; }
        public double? Weight { get; set; }
        public double? VisionLeft { get; set; }
        public double? VisionRight { get; set; }
        public string Hearing { get; set; }
        public string Nose { get; set; }
        public string BloodPressure { get; set; }
        public bool HasExistingHealthData { get; set; }

        // Vaccination Results
        public int? DoseNumber { get; set; }
        public string InjectionSite { get; set; }
        public string ImmediateReaction { get; set; }
        public DateTime? ReactionStartTime { get; set; }
        public string ReactionType { get; set; }
        public string SeverityLevel { get; set; }
        public bool HasExistingVaccinationData { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    #endregion
}