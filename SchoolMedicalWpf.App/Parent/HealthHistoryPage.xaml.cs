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
        public ObservableCollection<MedicalEvent> AllMedicalEvents { get; set; } = [];
        private List<HealthCheckResult> _originalHealthResults = [];
        private List<VaccinationResult> _originalVaccinationResults = [];
        private List<MedicalEvent> _originalMedicalEvents = [];
        #endregion

        #region Private Fields
        private ContentControl _mainContent;
        private readonly UserService _userService;
        private readonly StudentService _studentService;
        private readonly HealthCheckResultService _healthCheckResultService;
        private readonly VaccinationResultService _vaccinationResultService;
        private readonly MedicalEventService _medicalEventService;
        private readonly User _currentUser;
        #endregion

        #region Constructor
        public HealthHistoryPage(
            UserService userService,
            StudentService studentService,
            HealthCheckResultService healthCheckResultService,
            VaccinationResultService vaccinationResultService,
            MedicalEventService medicalEventService,
            User currentUser,
            ContentControl mainContent)
        {
            InitializeComponent();
            _userService = userService;
            _studentService = studentService;
            _healthCheckResultService = healthCheckResultService;
            _vaccinationResultService = vaccinationResultService;
            _medicalEventService = medicalEventService;
            _currentUser = currentUser;
            _mainContent = mainContent;

            DataContext = this;
            Loaded += UserControl_Loaded;
        }
        #endregion

        #region Event Handlers
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await UserControl_LoadedAsync();
        }

        private async Task UserControl_LoadedAsync()
        {
            try
            {
                ShowLoadingIndicator(true);

                await LoadStudentsAsync();
                await LoadAllHealthHistoryAsync();
                await LoadAllVaccinationHistoryAsync();
                await LoadAllMedicalEventsAsync();
                UpdateSummaryInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi tải dữ liệu: {ex.Message}\n\n" +
                    $"🕐 Thời gian: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n" +
                    $"👤 User: {_currentUser?.FullName ?? "danielleit241"}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ShowLoadingIndicator(false);
            }
        }

        private void ShowLoadingIndicator(bool show)
        {
            Dispatcher.Invoke(() =>
            {
                this.IsEnabled = !show;
                this.Cursor = show ? System.Windows.Input.Cursors.Wait : System.Windows.Input.Cursors.Arrow;
            });
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

        // Thêm double-click event cho Health DataGrid
        private void HealthHistoryDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var selectedItem = HealthHistoryDataGrid.SelectedItem as HealthCheckResult;
            if (selectedItem != null)
            {
                ShowHealthDetails(selectedItem);
            }
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

        // Thêm double-click event cho Vaccination DataGrid
        private void VaccinationHistoryDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var selectedItem = VaccinationHistoryDataGrid.SelectedItem as VaccinationResult;
            if (selectedItem != null)
            {
                ShowVaccinationDetails(selectedItem);
            }
        }
        #endregion

        #region Medical Events Tab Events
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

        // Thêm double-click event cho Medical Events DataGrid
        private void MedicalEventsDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var selectedItem = MedicalEventsDataGrid.SelectedItem as MedicalEvent;
            if (selectedItem != null)
            {
                ShowMedicalEventDetailsAsync(selectedItem);
            }
        }
        #endregion
        #endregion

        #region Private Methods
        // ... (giữ nguyên tất cả các methods LoadStudentsAsync, LoadAllHealthHistoryAsync, etc.)

        private void ShowHealthDetails(HealthCheckResult healthResult)
        {
            try
            {
                var detailsWindow = new HealthResultDetailWindow(healthResult);
                detailsWindow.Owner = Window.GetWindow(this);
                detailsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi hiển thị chi tiết khám sức khỏe: {ex.Message}\n\n" +
                    $"🕐 Thời gian: {DateTime.Now}\n" +
                    $"👤 User: {_currentUser?.FullName ?? "danielleit241"}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowVaccinationDetails(VaccinationResult vaccinationResult)
        {
            try
            {
                var detailsWindow = new VaccinationResultDetailWindow(vaccinationResult);
                detailsWindow.Owner = Window.GetWindow(this);
                detailsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi hiển thị chi tiết tiêm chủng: {ex.Message}\n\n" +
                    $"🕐 Thời gian: {DateTime.Now}\n" +
                    $"👤 User: {_currentUser?.FullName ?? "danielleit241"}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ShowMedicalEventDetailsAsync(MedicalEvent medicalEvent)
        {
            try
            {
                var staffNurseName = "N/A";
                var currentUserName = _currentUser?.FullName ?? "danielleit241";

                if (medicalEvent.StaffNurseId.HasValue)
                {
                    try
                    {
                        var staffNurse = await _userService.GetUserById(medicalEvent.StaffNurseId.Value);
                        staffNurseName = staffNurse?.FullName ?? currentUserName;
                    }
                    catch
                    {
                        staffNurseName = currentUserName;
                    }
                }

                var detailsWindow = new MedicalEventDetailWindow(medicalEvent, staffNurseName, currentUserName);
                detailsWindow.Owner = Window.GetWindow(this);
                detailsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi hiển thị chi tiết sự kiện y tế: {ex.Message}\n\n" +
                    $"🕐 Thời gian: {DateTime.Now}\n" +
                    $"👤 User: {_currentUser?.FullName ?? "danielleit241"}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetSeverityDisplayText(string severityLevel)
        {
            return severityLevel?.ToLower() switch
            {
                "high" or "nghiêm trọng" => "Nghiêm trọng",
                "medium" or "trung bình" => "Trung bình",
                "low" or "nhẹ" => "Nhẹ",
                _ => "N/A"
            };
        }

        // ... (giữ nguyên tất cả các methods khác: LoadStudentsAsync, FilterHealthResults, etc.)
        private async Task LoadStudentsAsync()
        {
            try
            {
                var students = await Task.Run(async () =>
                {
                    try
                    {
                        return await _studentService.GetAllStudents();
                    }
                    catch (Exception)
                    {
                        return new List<Student>();
                    }
                });

                Students = students.Where(s => s.UserId == _currentUser.UserId).ToList();

                var allStudentsOption = new Student { StudentId = Guid.Empty, FullName = "Tất cả học sinh" };
                Students.Insert(0, allStudentsOption);

                Dispatcher.Invoke(() =>
                {
                    HealthStudentComboBox.ItemsSource = Students;
                    VaccinationStudentComboBox.ItemsSource = Students;
                    EventStudentComboBox.ItemsSource = Students;

                    HealthStudentComboBox.SelectedIndex = 0;
                    VaccinationStudentComboBox.SelectedIndex = 0;
                    EventStudentComboBox.SelectedIndex = 0;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi tải danh sách học sinh: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadAllHealthHistoryAsync()
        {
            try
            {
                var allResults = await Task.Run(() => _healthCheckResultService.GetAll());
                Console.WriteLine($"Total results from service: {allResults?.Count() ?? 0}");

                var userStudentIds = Students.Where(s => s.StudentId != Guid.Empty).Select(s => s.StudentId).ToList();
                Console.WriteLine($"User student IDs count: {userStudentIds.Count}");

                var userResults = new List<HealthCheckResult>();

                foreach (var result in allResults!)
                {
                    try
                    {
                        if (result.HealthProfile?.Student != null)
                        {
                            Console.WriteLine($"Checking student ID: {result.HealthProfile.Student.StudentId}");
                            if (userStudentIds.Contains(result.HealthProfile.Student.StudentId))
                            {
                                userResults.Add(result);
                                Console.WriteLine($"Added result for student: {result.HealthProfile.Student.StudentId}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing result: {ex.Message}");
                    }
                }

                Console.WriteLine($"Filtered results count: {userResults.Count}");

                _originalHealthResults = userResults.OrderByDescending(r => r.DatePerformed).ToList();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    AllHealthResults = new ObservableCollection<HealthCheckResult>(_originalHealthResults);
                    HealthHistoryDataGrid.ItemsSource = AllHealthResults;
                    Console.WriteLine($"UI updated with {AllHealthResults.Count} items");
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi tải lịch sử khám sức khỏe: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Full exception: {ex}");
            }
        }

        private async Task LoadAllVaccinationHistoryAsync()
        {
            try
            {
                var allResults = await Task.Run(() => _vaccinationResultService.GetAllVaccinationResults());

                var userStudentIds = Students.Where(s => s.StudentId != Guid.Empty).Select(s => s.StudentId).ToList();
                var userResults = new List<VaccinationResult>();

                foreach (var result in allResults)
                {
                    try
                    {
                        if (result.HealthProfile?.Student != null)
                        {
                            if (userStudentIds.Contains(result.HealthProfile.Student.StudentId))
                            {
                                userResults.Add(result);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                if (userResults.Count == 0)
                {
                    foreach (var result in allResults)
                    {
                        try
                        {
                            if (result.HealthProfileId == Guid.Empty)
                            {
                                foreach (var studentId in userStudentIds)
                                {
                                    var student = await Task.Run(async () => await _studentService.GetStudentById(studentId));
                                    if (student?.HealthProfiles != null && student.HealthProfiles.Any(hp => hp.HealthProfileId == result.HealthProfileId))
                                    {
                                        if (result.HealthProfile == null)
                                        {
                                            result.HealthProfile = student.HealthProfiles.First(hp => hp.HealthProfileId == result.HealthProfileId);
                                        }
                                        if (result.HealthProfile.Student == null)
                                        {
                                            result.HealthProfile.Student = student;
                                        }

                                        userResults.Add(result);
                                        break;
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                _originalVaccinationResults = userResults.OrderByDescending(r => r.VaccinationDate).ToList();

                Dispatcher.Invoke(() =>
                {
                    AllVaccinations = new ObservableCollection<VaccinationResult>(_originalVaccinationResults);
                    VaccinationHistoryDataGrid.ItemsSource = AllVaccinations;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi tải lịch sử tiêm chủng: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadAllMedicalEventsAsync()
        {
            try
            {
                var allEvents = await Task.Run(() => _medicalEventService.GetAllMedicalEvents());

                var userStudentIds = Students.Where(s => s.StudentId != Guid.Empty).Select(s => s.StudentId).ToList();

                var userEvents = allEvents
                    .Where(e => e.StudentId.HasValue && userStudentIds.Contains(e.StudentId.Value))
                    .ToList();

                foreach (var medEvent in userEvents)
                {
                    if (medEvent.Student == null)
                    {
                        medEvent.Student = Students.FirstOrDefault(s => s.StudentId == medEvent.StudentId);
                    }
                }

                _originalMedicalEvents = userEvents.OrderByDescending(e => e.EventDate).ToList();

                Dispatcher.Invoke(() =>
                {
                    AllMedicalEvents = new ObservableCollection<MedicalEvent>(_originalMedicalEvents);
                    MedicalEventsDataGrid.ItemsSource = AllMedicalEvents;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi tải sự kiện y tế: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterHealthResults()
        {
            try
            {
                if (!_originalHealthResults.Any()) return;

                var filteredResults = _originalHealthResults.AsEnumerable();

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

                UpdateHealthSummary();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi lọc dữ liệu khám sức khỏe: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterVaccinationResults()
        {
            try
            {
                if (!_originalVaccinationResults.Any()) return;

                var filteredResults = _originalVaccinationResults.AsEnumerable();

                var vaccineName = SearchVaccineNameTextBox.Text?.Trim();
                if (!string.IsNullOrEmpty(vaccineName))
                {
                    filteredResults = filteredResults.Where(r =>
                        r.Schedule?.Vaccine?.VaccineName?.Contains(vaccineName, StringComparison.OrdinalIgnoreCase) == true);
                }

                var manufacturer = SearchManufacturerTextBox.Text?.Trim();
                if (!string.IsNullOrEmpty(manufacturer))
                {
                    filteredResults = filteredResults.Where(r =>
                        r.Schedule?.Vaccine?.Manufacturer?.Contains(manufacturer, StringComparison.OrdinalIgnoreCase) == true);
                }

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

                UpdateVaccinationSummary();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi lọc dữ liệu tiêm chủng: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterMedicalEvents()
        {
            try
            {
                if (!_originalMedicalEvents.Any()) return;

                var filteredEvents = _originalMedicalEvents.AsEnumerable();

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

                var selectedEventType = (EventTypeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                if (!string.IsNullOrEmpty(selectedEventType) && selectedEventType != "Tất cả")
                {
                    filteredEvents = filteredEvents.Where(e => e.EventType == selectedEventType);
                }

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

                UpdateMedicalEventSummary();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi lọc sự kiện y tế: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateHealthSummary()
        {
            try
            {
                HealthRecordCountTextBlock.Text = AllHealthResults.Count.ToString();
                var uniqueStudents = AllHealthResults.Select(r => r.HealthProfile?.Student?.StudentId).Distinct().Where(id => id.HasValue).Count();
                HealthStudentCountTextBlock.Text = uniqueStudents.ToString();
            }
            catch (Exception)
            {
            }
        }

        private void UpdateVaccinationSummary()
        {
            try
            {
                VaccinationRecordCountTextBlock.Text = AllVaccinations.Count.ToString();
                var uniqueStudents = AllVaccinations.Select(r => r.HealthProfile?.Student?.StudentId).Distinct().Where(id => id.HasValue).Count();
                VaccinationStudentCountTextBlock.Text = uniqueStudents.ToString();
            }
            catch (Exception)
            {
            }
        }

        private void UpdateMedicalEventSummary()
        {
            try
            {
                EventRecordCountTextBlock.Text = AllMedicalEvents.Count.ToString();
                var uniqueStudents = AllMedicalEvents.Select(e => e.StudentId).Distinct().Count();
                EventStudentCountTextBlock.Text = uniqueStudents.ToString();

                var severeEvents = AllMedicalEvents.Count(e =>
                    e.SeverityLevel == "High" ||
                    e.SeverityLevel == "Nghiêm trọng");
                EventSevereCountTextBlock.Text = severeEvents.ToString();
            }
            catch (Exception)
            {
            }
        }

        private void UpdateSummaryInfo()
        {
            UpdateHealthSummary();
            UpdateVaccinationSummary();
            UpdateMedicalEventSummary();
        }

        #endregion
    }
}