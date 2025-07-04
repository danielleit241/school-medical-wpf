using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Parent
{
    public partial class MedicalRegistrationHistoryPage : UserControl
    {
        private readonly MedicalRegistrationService _registrationService;
        private readonly User _currentUser;
        private readonly StudentService _studentService;
        private readonly UserService _userService;
        private bool _isLoading = false;

        public ObservableCollection<MedicalRegistrationViewModel> MedicalRegistrationList { get; set; } = new ObservableCollection<MedicalRegistrationViewModel>();

        public MedicalRegistrationHistoryPage(
            MedicalRegistrationService registrationService,
            StudentService studentService,
            UserService userService,
            User currentUser)
        {
            InitializeComponent();
            _registrationService = registrationService;
            _currentUser = currentUser;
            _studentService = studentService;
            _userService = userService;
            this.DataContext = this;
        }

        private async void CreateMedicalRegistration_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button)
                {
                    button.IsEnabled = false;
                }

                var userStudents = await Task.Run(() => _studentService.GetStudentsByUserId(_currentUser.UserId));

                if (userStudents.Count == 0)
                {
                    MessageBox.Show("❌ Bạn phải có ít nhất một học sinh để tạo đơn đăng ký thuốc.\n\n" +
                        $"🕐 Thời gian: {DateTime.Now}\n" +
                        $"👤 User: {_currentUser.FullName}", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var form = ActivatorUtilities.CreateInstance<MedicalRegistrationFormWindow>(App.Services, _currentUser);
                form.Owner = Window.GetWindow(this);

                form.Closed += async (s, args) => await LoadDataAsync();

                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi mở form đăng ký: {ex.Message}\n\n" +
                    $"🕐 Thời gian: {DateTime.Now}\n" +
                    $"👤 User: {_currentUser.FullName}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (sender is Button button)
                {
                    button.IsEnabled = true;
                }
            }
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            if (_isLoading) return;

            try
            {
                _isLoading = true;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    LoadingPanel.Visibility = Visibility.Visible;
                });

                var registrationsHistory = await Task.Run(() =>
                {
                    return _registrationService.GetAllRegistrations();
                });

                var userRegistrations = registrationsHistory
                    .Where(r => r.UserId == _currentUser.UserId)
                    .OrderByDescending(r => r.DateSubmitted)
                    .ToList();

                var viewModels = new List<MedicalRegistrationViewModel>();
                foreach (var registration in userRegistrations)
                {
                    var staffNurseName = "N/A";

                    if (registration.StaffNurseId.HasValue)
                    {
                        try
                        {
                            var staffNurse = await _userService.GetUserById(registration.StaffNurseId.Value);
                            staffNurseName = staffNurse?.FullName ?? "Nurse";
                        }
                        catch
                        {
                            staffNurseName = "Nurse";
                        }
                    }

                    viewModels.Add(new MedicalRegistrationViewModel
                    {
                        MedicationName = registration.MedicationName,
                        DateSubmitted = registration.DateSubmitted,
                        DateApproved = registration.DateApproved,
                        Student = registration.Student,
                        TotalDosages = registration.TotalDosages,
                        ParentalConsent = registration.ParentalConsent,
                        Status = registration.Status,
                        Notes = registration.Notes,
                        StaffNurseName = staffNurseName
                    });
                }

                // ✅ Update UI trên UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MedicalRegistrationList.Clear();
                    foreach (var viewModel in viewModels)
                    {
                        MedicalRegistrationList.Add(viewModel);
                    }
                });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"❌ Lỗi khi tải dữ liệu: {ex.Message}\n\n" +
                        $"🕐 Thời gian: {DateTime.Now}\n" +
                        $"👤 User: {_currentUser.FullName}", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                _isLoading = false;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    LoadingPanel.Visibility = Visibility.Collapsed;
                });
            }
        }

        public async Task RefreshDataAsync()
        {
            await LoadDataAsync();
        }
    }

    public class MedicalRegistrationViewModel
    {
        public string? MedicationName { get; set; }
        public DateOnly? DateSubmitted { get; set; }
        public DateOnly? DateApproved { get; set; }
        public Student? Student { get; set; }
        public string? TotalDosages { get; set; }
        public bool? ParentalConsent { get; set; }
        public bool Status { get; set; }
        public string? Notes { get; set; }
        public string? StaffNurseName { get; set; }
    }
}