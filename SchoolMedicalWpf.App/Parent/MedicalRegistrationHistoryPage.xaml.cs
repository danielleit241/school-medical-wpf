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
        private bool _isLoading = false;

        public ObservableCollection<MedicalRegistration> MedicalRegistrationList { get; set; } = new ObservableCollection<MedicalRegistration>();

        public MedicalRegistrationHistoryPage(MedicalRegistrationService registrationService, StudentService studentService, User currentUser)
        {
            InitializeComponent();
            _registrationService = registrationService;
            _currentUser = currentUser;
            _studentService = studentService;
            this.DataContext = this;
        }

        private async void CreateMedicalRegistration_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Disable button để tránh spam click
                if (sender is Button button)
                {
                    button.IsEnabled = false;
                }

                // ✅ Chuyển thành async để không block UI
                var userStudents = await Task.Run(() => _studentService.GetStudentsByUserId(_currentUser.UserId));

                if (userStudents.Count == 0)
                {
                    MessageBox.Show("Bạn phải có ít nhất một học sinh để tạo đơn đăng ký thuốc.", "Lỗi",
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
                MessageBox.Show($"Lỗi khi mở form đăng ký: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Re-enable button
                if (sender is Button button)
                {
                    button.IsEnabled = true;
                }
            }
            _ = RefreshDataAsync().ConfigureAwait(false);
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

                // ✅ Load data trên background thread
                var registrationsHistory = await Task.Run(() =>
                {
                    return _registrationService.GetAllRegistrations();
                });

                var userRegistrations = registrationsHistory
                    .Where(r => r.UserId == _currentUser.UserId)
                    .OrderByDescending(r => r.DateSubmitted) // Sort by newest first
                    .ToList();

                // ✅ Update UI trên UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MedicalRegistrationList.Clear();
                    foreach (var registration in userRegistrations)
                    {
                        MedicalRegistrationList.Add(registration);
                    }
                });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        // ✅ Public method để refresh từ bên ngoài
        public async Task RefreshDataAsync()
        {
            await LoadDataAsync();
        }
    }
}