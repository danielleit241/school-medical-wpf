using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;
using SchoolMedicalWpf.Dal.Repositories;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace SchoolMedicalWpf.App.Nurse
{
    public partial class HealthSchedulePage : UserControl
    {
        private ObservableCollection<StudentResultItem> _studentResults;
        private ScheduleItem _currentSchedule;
        private readonly User _currentUser;
        private readonly DateTime _currentDateTime = DateTime.Now;

        private readonly UserService _userService;
        private readonly StudentService _studentService;
        private readonly HealthProfileService _healthProfileService;
        private readonly HealthCheckScheduleService _healthCheckScheduleService;
        private readonly VaccinationScheduleService _vaccinationScheduleService;
        private readonly HealthCheckResultService _healthCheckResultService;
        private readonly VaccinationResultService _vaccinationResultService;

        public HealthSchedulePage(UserService userService,
                                 StudentService studentService,
                                 HealthProfileService healthProfileService,
                                 HealthCheckScheduleService healthCheckScheduleService,
                                 VaccinationScheduleService vaccinationScheduleService,
                                 HealthCheckResultService healthCheckResultService,
                                 VaccinationResultService vaccinationResultService,
                                 User currentUser)
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
            txtCurrentDateTime.Text = $"{_currentDateTime:yyyy-MM-dd HH:mm:ss} UTC - User: {_currentUser.FullName} (danielleit241)";
            UpdateLastUpdateTime("Khởi tạo trang");
            LoadActiveSchedulesCount();
        }

        // ... (other code unchanged)

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
                // MessageBox removed
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
                // MessageBox removed
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
                // MessageBox removed
                btnLoadStudents.Content = "👥 Tải danh sách sinh viên";
                btnLoadStudents.IsEnabled = true;
            }
        }

        // *** UPDATED: LoadStudentsForSchedule với bool Status ***
        private async Task LoadStudentsForSchedule()
        {
            _studentResults.Clear();
            try
            {
                if (_currentSchedule.Type == "HealthCheck")
                {
                    await LoadStudentsFromHealthCheckResults();
                }
                else if (_currentSchedule.Type == "Vaccination")
                {
                    await LoadStudentsFromVaccinationResults();
                }

                ShowDataGrids();
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{_currentDateTime:yyyy-MM-dd HH:mm:ss}] Error loading students from results: {ex.Message}");
                throw;
            }
        }

        // *** UPDATED: Load students từ HealthCheckResults với bool Status ***
        private async Task LoadStudentsFromHealthCheckResults()
        {
            var allHealthResults = await Task.Run(() => _healthCheckResultService.GetAll());
            var scheduleResults = allHealthResults.Where(r => r.ScheduleId == _currentSchedule.Id).ToList();
            var healthProfileIds = scheduleResults.Select(r => r.HealthProfileId).Distinct().ToList();

            foreach (var healthProfileId in healthProfileIds)
            {
                try
                {
                    // Get health profile
                    var healthProfile = await Task.Run(() => _healthProfileService.GetHealthProfileById(healthProfileId));
                    if (healthProfile == null) continue;

                    // Get student from health profile
                    var student = await Task.Run(() => _studentService.GetStudentById((Guid)healthProfile.StudentId!));
                    if (student == null) continue;

                    var studentResults = scheduleResults
                        .Where(r => r.HealthProfileId == healthProfileId)
                        .OrderByDescending(r => r.DatePerformed)
                        .ToList();

                    var latestResult = studentResults.FirstOrDefault();
                    if (latestResult == null) continue;

                    var resultItem = new StudentResultItem
                    {
                        StudentId = student.StudentId,
                        StudentCode = student.StudentCode!,
                        StudentName = student.FullName!,
                        Grade = student.Grade!,
                        HealthProfileId = healthProfile.HealthProfileId,
                        ScheduleId = _currentSchedule.Id,
                        ScheduleType = _currentSchedule.Type,

                        IsCompleted = true,
                        HasExistingResults = true,
                        HasExistingHealthData = latestResult.Status, // true = có kết quả
                        ExistingResultId = latestResult.ResultId,
                        DatabaseStatus = latestResult.Status, // Store database status

                        DatePerformed = latestResult.DatePerformed?.ToDateTime(TimeOnly.MinValue) ?? _currentDateTime.Date,
                        Notes = latestResult.Notes ?? "",

                        Height = latestResult.Height,
                        Weight = latestResult.Weight,
                        VisionLeft = latestResult.VisionLeft,
                        VisionRight = latestResult.VisionRight,
                        Hearing = latestResult.Hearing ?? "",
                        Nose = latestResult.Nose ?? "",
                        BloodPressure = latestResult.BloodPressure ?? ""
                    };

                    // Set status based on database Status column (bool)
                    resultItem.Status = latestResult.Status switch
                    {
                        false => "Chưa có kết quả",
                        true => HasComprehensiveHealthData(latestResult) ? "Đã có kết quả đầy đủ" : "Đã có kết quả cơ bản"
                    };

                    _studentResults.Add(resultItem);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{_currentDateTime:yyyy-MM-dd HH:mm:ss}] Error processing health profile {healthProfileId}: {ex.Message}");
                }
            }
        }

        // *** UPDATED: Load students từ VaccinationResults với bool Status ***
        private async Task LoadStudentsFromVaccinationResults()
        {
            var allVaccinationResults = await Task.Run(() => _vaccinationResultService.GetAllVaccinationResults());
            var scheduleResults = allVaccinationResults.Where(r => r.ScheduleId == _currentSchedule.Id).ToList();

            var healthProfileIds = scheduleResults.Select(r => r.HealthProfileId).Distinct().ToList();
            foreach (var healthProfileId in healthProfileIds)
            {
                try
                {
                    var healthProfile = await Task.Run(() => _healthProfileService.GetHealthProfileById(healthProfileId));
                    if (healthProfile == null) continue;

                    var student = await Task.Run(() => _studentService.GetStudentById((Guid)healthProfile.StudentId!));
                    if (student == null) continue;

                    var studentResults = scheduleResults
                        .Where(r => r.HealthProfileId == healthProfileId)
                        .OrderByDescending(r => r.VaccinationDate)
                        .ToList();

                    var latestResult = studentResults.FirstOrDefault();
                    if (latestResult == null) continue;

                    var resultItem = new StudentResultItem
                    {
                        StudentId = student.StudentId,
                        StudentCode = student.StudentCode!,
                        StudentName = student.FullName!,
                        Grade = student.Grade!,
                        HealthProfileId = healthProfile.HealthProfileId,
                        ScheduleId = _currentSchedule.Id,
                        ScheduleType = _currentSchedule.Type,

                        IsCompleted = true,
                        HasExistingResults = true,
                        HasExistingVaccinationData = latestResult.Status, // true = có kết quả
                        ExistingResultId = latestResult.VaccinationResultId,
                        DatabaseStatus = latestResult.Status, // Store database status

                        DatePerformed = latestResult.VaccinationDate?.ToDateTime(TimeOnly.MinValue) ?? _currentDateTime.Date,
                        Notes = latestResult.Notes ?? "",

                        DoseNumber = latestResult.DoseNumber,
                        InjectionSite = latestResult.InjectionSite ?? "",
                        ImmediateReaction = latestResult.ImmediateReaction ?? "",
                        ReactionStartTime = latestResult.ReactionStartTime,
                        ReactionType = latestResult.ReactionType ?? "",
                        SeverityLevel = latestResult.SeverityLevel ?? ""
                    };

                    // Set status based on database Status column (bool) and reaction data
                    resultItem.Status = latestResult.Status switch
                    {
                        false => "Chưa có kết quả",
                        true when HasSignificantReaction(latestResult) => "Đã tiêm - Có phản ứng",
                        true when HasComprehensiveVaccinationData(latestResult) => "Đã tiêm - Hoàn tất",
                        true => "Đã tiêm - Cơ bản"
                    };

                    _studentResults.Add(resultItem);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{_currentDateTime:yyyy-MM-dd HH:mm:ss}] Error processing vaccination health profile {healthProfileId}: {ex.Message}");
                }
            }
        }

        private bool HasComprehensiveHealthData(HealthCheckResult result)
        {
            return result.Height.HasValue &&
                   result.Weight.HasValue &&
                   !string.IsNullOrEmpty(result.BloodPressure) &&
                   (result.VisionLeft.HasValue || result.VisionRight.HasValue) &&
                   !string.IsNullOrEmpty(result.Hearing);
        }

        private bool HasComprehensiveVaccinationData(VaccinationResult result)
        {
            return result.DoseNumber.HasValue &&
                   !string.IsNullOrEmpty(result.InjectionSite) &&
                   !string.IsNullOrEmpty(result.ImmediateReaction);
        }

        private bool HasSignificantReaction(VaccinationResult result)
        {
            return !string.IsNullOrEmpty(result.ImmediateReaction) &&
                   result.ImmediateReaction != "Không có" &&
                   (!string.IsNullOrEmpty(result.SeverityLevel) &&
                    (result.SeverityLevel.Contains("Vừa") || result.SeverityLevel.Contains("Nặng")));
        }

        private void ViewResult_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var resultItem = button?.DataContext as StudentResultItem;

            if (resultItem != null && resultItem.HasExistingResults)
            {
                ViewExistingResult(resultItem);
            }
            // MessageBox removed
        }

        private void EditResult_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var resultItem = button?.DataContext as StudentResultItem;

            if (resultItem != null && resultItem.HasExistingResults)
            {
                var confirmResult = MessageBox.Show(
                    $"Bạn có chắc chắn muốn chỉnh sửa kết quả cho {resultItem.StudentName}?\n" +
                    "Việc chỉnh sửa sẽ cập nhật kết quả hiện có.",
                    "Xác nhận chỉnh sửa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirmResult == MessageBoxResult.Yes)
                {
                    EditExistingResult(resultItem);
                }
            }
            // MessageBox removed
        }

        private void EnterResults_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var resultItem = button?.DataContext as StudentResultItem;

            if (resultItem != null && resultItem.IsCompleted)
            {
                OpenResultWindow(resultItem, false);
            }
            // MessageBox removed
        }

        private void ViewExistingResult(StudentResultItem resultItem)
        {
            try
            {
                if (_currentSchedule.Type == "HealthCheck")
                {
                    var viewWindow = new HealthCheckResultViewWindow(
                        healthProfileId: resultItem.HealthProfileId,
                        scheduleId: resultItem.ScheduleId,
                        studentCode: resultItem.StudentCode,
                        studentName: resultItem.StudentName,
                        grade: resultItem.Grade,
                        existingResult: CreateHealthCheckResultFromItem(resultItem),
                        currentUser: _currentUser
                    );

                    viewWindow.ShowDialog();
                }
                else if (_currentSchedule.Type == "Vaccination")
                {
                    var viewWindow = new VaccinationResultViewWindow(
                        healthProfileId: resultItem.HealthProfileId,
                        scheduleId: resultItem.ScheduleId,
                        studentCode: resultItem.StudentCode,
                        studentName: resultItem.StudentName,
                        grade: resultItem.Grade,
                        existingResult: CreateVaccinationResultFromItem(resultItem),
                        currentUser: _currentUser
                    );

                    viewWindow.ShowDialog();
                }

                UpdateLastUpdateTime($"Đã xem kết quả của {resultItem.StudentName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi hiển thị kết quả: {ex.Message}",
                              "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditExistingResult(StudentResultItem resultItem)
        {
            try
            {
                OpenResultWindow(resultItem, true); // true = edit mode
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi mở cửa sổ chỉnh sửa: {ex.Message}",
                              "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenResultWindow(StudentResultItem resultItem, bool isEditMode = false)
        {
            try
            {
                if (_currentSchedule.Type == "HealthCheck")
                {
                    var healthCheckWindow = new HealthCheckResultWindow(
                        healthProfileId: resultItem.HealthProfileId,
                        scheduleId: resultItem.ScheduleId,
                        currentUser: _currentUser,
                        healthCheckResultService: _healthCheckResultService,
                        studentCode: resultItem.StudentCode,
                        studentName: resultItem.StudentName,
                        grade: resultItem.Grade
                    );

                    if (isEditMode && resultItem.HasExistingResults)
                    {
                        healthCheckWindow.LoadExistingData(CreateHealthCheckResultFromItem(resultItem));
                    }

                    if (healthCheckWindow.ShowDialog() == true)
                    {
                        var savedResult = healthCheckWindow.Result;
                        UpdateStudentResultFromHealthCheck(resultItem, savedResult);

                        resultItem.Status = isEditMode ? "Đã cập nhật kết quả" : "Đã lưu kết quả";
                        resultItem.HasExistingResults = true;
                        resultItem.DatabaseStatus = true; // Set status to true when saved with details
                        UpdateStatistics();
                        UpdateLastUpdateTime($"Đã {(isEditMode ? "cập nhật" : "lưu")} kết quả khám cho {resultItem.StudentName}");

                        MessageBox.Show($"✅ Đã {(isEditMode ? "cập nhật" : "lưu")} kết quả khám sức khỏe cho {resultItem.StudentName}!\nStatus = true (Có kết quả)",
                                      "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else if (_currentSchedule.Type == "Vaccination")
                {
                    var vaccinationWindow = new VaccinationResultWindow(
                        healthProfileId: resultItem.HealthProfileId,
                        scheduleId: resultItem.ScheduleId,
                        currentUser: _currentUser,
                        vaccinationResultService: _vaccinationResultService,
                        studentCode: resultItem.StudentCode,
                        studentName: resultItem.StudentName,
                        grade: resultItem.Grade
                    );

                    if (isEditMode && resultItem.HasExistingResults)
                    {
                        vaccinationWindow.LoadExistingData(CreateVaccinationResultFromItem(resultItem));
                    }

                    if (vaccinationWindow.ShowDialog() == true)
                    {
                        var savedResult = vaccinationWindow.Result;
                        UpdateStudentResultFromVaccination(resultItem, savedResult);

                        resultItem.Status = isEditMode ? "Đã cập nhật kết quả (Status=true)" : "Đã lưu kết quả tiêm (Status=true)";
                        resultItem.HasExistingResults = true;
                        resultItem.DatabaseStatus = true; // Set status to true when saved with details
                        UpdateStatistics();
                        UpdateLastUpdateTime($"Đã {(isEditMode ? "cập nhật" : "lưu")} kết quả tiêm cho {resultItem.StudentName}");

                        MessageBox.Show($"✅ Đã {(isEditMode ? "cập nhật" : "lưu")} kết quả tiêm chủng cho {resultItem.StudentName}!\nStatus = true (Có kết quả)",
                                      "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi mở cửa sổ kết quả: {ex.Message}",
                              "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private HealthCheckResult CreateHealthCheckResultFromItem(StudentResultItem item)
        {
            return new HealthCheckResult
            {
                ResultId = item.ExistingResultId,
                ScheduleId = item.ScheduleId,
                HealthProfileId = item.HealthProfileId,
                DatePerformed = DateOnly.FromDateTime(item.DatePerformed),
                Status = item.DatabaseStatus, // Use stored database status (bool)
                Height = item.Height,
                Weight = item.Weight,
                VisionLeft = item.VisionLeft,
                VisionRight = item.VisionRight,
                Hearing = item.Hearing,
                Nose = item.Nose,
                BloodPressure = item.BloodPressure,
                Notes = item.Notes,
                RecordedId = _currentUser.UserId
            };
        }

        private VaccinationResult CreateVaccinationResultFromItem(StudentResultItem item)
        {
            return new VaccinationResult
            {
                VaccinationResultId = item.ExistingResultId,
                ScheduleId = item.ScheduleId,
                HealthProfileId = item.HealthProfileId,
                VaccinationDate = DateOnly.FromDateTime(item.DatePerformed),
                Status = item.DatabaseStatus, // Use stored database status (bool)
                DoseNumber = item.DoseNumber,
                InjectionSite = item.InjectionSite,
                ImmediateReaction = item.ImmediateReaction,
                ReactionStartTime = item.ReactionStartTime,
                ReactionType = item.ReactionType,
                SeverityLevel = item.SeverityLevel,
                Notes = item.Notes,
                RecordedId = _currentUser.UserId
            };
        }

        // *** UPDATED: UpdateStudentResultFromHealthCheck với bool Status ***
        private void UpdateStudentResultFromHealthCheck(StudentResultItem item, HealthCheckResult result)
        {
            item.DatePerformed = result.DatePerformed?.ToDateTime(TimeOnly.MinValue) ?? _currentDateTime.Date;
            item.Notes = result.Notes ?? "";
            item.ExistingResultId = result.ResultId;
            item.DatabaseStatus = result.Status; // Update database status (bool)

            item.Height = result.Height;
            item.Weight = result.Weight;
            item.VisionLeft = result.VisionLeft;
            item.VisionRight = result.VisionRight;
            item.Hearing = result.Hearing ?? "";
            item.Nose = result.Nose ?? "";
            item.BloodPressure = result.BloodPressure ?? "";

            // Update based on Status column (bool)
            item.HasExistingHealthData = result.Status;

            // Update status display
            item.Status = result.Status switch
            {
                false => "Chưa có kết quả",
                true => HasComprehensiveHealthData(result) ? "Đã có kết quả đầy đủ" : "Đã có kết quả cơ bản"
            };
        }

        // *** UPDATED: UpdateStudentResultFromVaccination với bool Status ***
        private void UpdateStudentResultFromVaccination(StudentResultItem item, VaccinationResult result)
        {
            item.DatePerformed = result.VaccinationDate?.ToDateTime(TimeOnly.MinValue) ?? _currentDateTime.Date;
            item.Notes = result.Notes ?? "";
            item.ExistingResultId = result.VaccinationResultId;
            item.DatabaseStatus = result.Status; // Update database status (bool)

            // Update vaccination specific data
            item.DoseNumber = result.DoseNumber;
            item.InjectionSite = result.InjectionSite ?? "";
            item.ImmediateReaction = result.ImmediateReaction ?? "";
            item.ReactionStartTime = result.ReactionStartTime;
            item.ReactionType = result.ReactionType ?? "";
            item.SeverityLevel = result.SeverityLevel ?? "";

            // Update based on Status column (bool)
            item.HasExistingVaccinationData = result.Status;

            // Update status display
            item.Status = result.Status switch
            {
                false => "Chưa có kết quả",
                true when HasSignificantReaction(result) => "Đã tiêm - Có phản ứng",
                true when HasComprehensiveVaccinationData(result) => "Đã tiêm - Hoàn tất",
                true => "Đã tiêm - Cơ bản"
            };
        }

        // *** UPDATED: SaveAll_Click với bool Status ***
        private async void SaveAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Lọc những kết quả đã completed nhưng chưa có kết quả chi tiết (Status = false hoặc chưa có)
                var completedResults = _studentResults.Where(x => x.IsCompleted &&
                    (!x.HasExistingResults || // Chưa có record
                     x.DatabaseStatus == false) && // Hoặc có record nhưng Status = false
                    !x.Status.Contains("Đã lưu") &&
                    !x.Status.Contains("Đã cập nhật")).ToList();

                if (!completedResults.Any())
                {
                    MessageBox.Show("Không có kết quả mới nào để lưu!", "Thông báo");
                    return;
                }

                var result = MessageBox.Show(
                    $"Bạn có {completedResults.Count} kết quả chưa được nhập chi tiết.\n" +
                    $"Bạn có muốn mở từng cửa sổ để nhập kết quả chi tiết không?\n\n" +
                    $"Chọn 'Yes' để nhập từng kết quả (Status = true)\n" +
                    $"Chọn 'No' để lưu với thông tin cơ bản (Status = false)",
                    "Nhập kết quả chi tiết", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.Cancel)
                    return;

                if (result == MessageBoxResult.Yes)
                {
                    // Open detail windows for each result - this will set Status = true
                    foreach (var item in completedResults)
                    {
                        var confirmResult = MessageBox.Show(
                            $"Nhập kết quả chi tiết cho {item.StudentName} ({item.StudentCode})?",
                            "Xác nhận", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                        if (confirmResult == MessageBoxResult.Cancel)
                            break;

                        if (confirmResult == MessageBoxResult.Yes)
                        {
                            OpenResultWindow(item, false); // This will save with Status = true
                        }
                        else
                        {
                            // Save basic info only with Status = false
                            await SaveBasicResultItem(item);
                            item.Status = "Đã lưu cơ bản (Status=false)";
                            item.HasExistingResults = true;
                            item.DatabaseStatus = false;
                        }
                    }
                }
                else
                {
                    // Save basic info for all with Status = false
                    btnSaveAll.IsEnabled = false;
                    btnSaveAll.Content = "⏳ Đang lưu...";

                    int savedCount = 0;
                    foreach (var item in completedResults)
                    {
                        await SaveBasicResultItem(item); // Saves with Status = false
                        item.Status = "Đã lưu cơ bản (Status=false)";
                        item.HasExistingResults = true;
                        item.DatabaseStatus = false;
                        savedCount++;
                    }

                    MessageBox.Show($"Đã lưu thông tin cơ bản (Status=false) cho {savedCount} sinh viên!\n" +
                                  "Y tá có thể cập nhật chi tiết sau để chuyển Status=true.", "Thành công");
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

        // *** UPDATED: SaveBasicResultItem với Status = false ***
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
                        Status = false, // false = chưa có kết quả (chỉ lưu thông tin cơ bản)
                        Notes = $"[Lưu cơ bản từ HealthSchedulePage lúc {_currentDateTime:yyyy-MM-dd HH:mm:ss}] {item.Notes}",
                        RecordedId = recordedId
                    };

                    await Task.Run(() => _healthCheckResultService.Add(healthResult));
                    item.ExistingResultId = healthResult.ResultId;
                    Console.WriteLine($"[{_currentDateTime:yyyy-MM-dd HH:mm:ss}] Saved basic health check (Status=false) for {item.StudentName} by {_currentUser.FullName}");
                }
                else if (item.ScheduleType == "Vaccination")
                {
                    var vaccinationResult = new VaccinationResult
                    {
                        VaccinationResultId = Guid.NewGuid(),
                        ScheduleId = item.ScheduleId,
                        HealthProfileId = item.HealthProfileId,
                        VaccinationDate = DateOnly.FromDateTime(item.DatePerformed),
                        Status = false, // false = chưa có kết quả (chỉ lưu thông tin cơ bản)
                        Notes = $"[Lưu cơ bản từ HealthSchedulePage lúc {_currentDateTime:yyyy-MM-dd HH:mm:ss}] {item.Notes}",
                        RecordedId = recordedId
                    };

                    await Task.Run(() => _vaccinationResultService.AddVaccinationResult(vaccinationResult));
                    item.ExistingResultId = vaccinationResult.VaccinationResultId;
                    Console.WriteLine($"[{_currentDateTime:yyyy-MM-dd HH:mm:ss}] Saved basic vaccination (Status=false) for {item.StudentName} by {_currentUser.FullName}");
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
                LoadStudents_Click(null!, null!);
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
            var hasResults = _studentResults.Count(x => x.DatabaseStatus == true); // Status = true
            var noResults = _studentResults.Count(x => x.DatabaseStatus == false);   // Status = false

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
                        Title = schedule.Title!,
                        Type = "HealthCheck",
                        StartDate = schedule.StartDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue,
                        EndDate = schedule.EndDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MaxValue,
                        TargetGrade = schedule.TargetGrade ?? "Tất cả",
                        Description = schedule.Description ?? ""
                    });
                }

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
                        Title = schedule.Title!,
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

        #endregion
    }

    #region Supporting Classes

    public class ScheduleItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string Type { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TargetGrade { get; set; } = "";
        public string Description { get; set; } = "";
    }

    public class StudentResultItem : INotifyPropertyChanged
    {
        private bool _isCompleted;
        private DateTime _datePerformed;
        private string _notes = "";
        private string _status = "";

        // Basic Info
        public Guid StudentId { get; set; }
        public string StudentCode { get; set; } = "";
        public string StudentName { get; set; } = "";
        public string Grade { get; set; } = "";
        public Guid HealthProfileId { get; set; }
        public Guid ScheduleId { get; set; }
        public string ScheduleType { get; set; } = "";

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

        // UPDATED: Database Status tracking với bool
        public bool DatabaseStatus { get; set; } // false: chưa có kết quả, true: có kết quả

        // Validation and tracking properties
        public bool HasExistingResults { get; set; }
        public Guid ExistingResultId { get; set; }

        // Health Check Results
        public double? Height { get; set; }
        public double? Weight { get; set; }
        public double? VisionLeft { get; set; }
        public double? VisionRight { get; set; }
        public string Hearing { get; set; } = "";
        public string Nose { get; set; } = "";
        public string BloodPressure { get; set; } = "";
        public bool HasExistingHealthData { get; set; }

        // Vaccination Results
        public int? DoseNumber { get; set; }
        public string InjectionSite { get; set; } = "";
        public string ImmediateReaction { get; set; } = "";
        public DateTime? ReactionStartTime { get; set; }
        public string ReactionType { get; set; } = "";
        public string SeverityLevel { get; set; } = "";
        public bool HasExistingVaccinationData { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    #endregion
}