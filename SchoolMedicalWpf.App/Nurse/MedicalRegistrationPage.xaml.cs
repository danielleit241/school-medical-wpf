using System.Windows;
using System.Windows.Controls;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Nurse
{
    /// <summary>
    /// Interaction logic for MedicalRegistrationPage.xaml
    /// </summary>
    public partial class MedicalRegistrationPage : UserControl
    {
        private readonly MedicalRegistrationService _medicalRegistrationService;
        private readonly User _currentUser;
        private List<MedicalRegistration> _allRegistrations;
        private string _currentFilter = "All";

        public MedicalRegistrationPage(MedicalRegistrationService medicalRegistrationService, User currentUser)
        {
            InitializeComponent();
            _medicalRegistrationService = medicalRegistrationService;
            _currentUser = currentUser;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_allRegistrations == null)
            {
                LoadRegistrations();
            }
        }

        private async void LoadRegistrations()
        {
            try
            {
                LoadingGrid.Visibility = Visibility.Visible;

                // Load registrations from service
                _allRegistrations = await Task.Run(() => _medicalRegistrationService.GetAllRegistrations());

                FilterAndDisplayRegistrations();
                UpdatePendingCount();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LoadingGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void FilterAndDisplayRegistrations()
        {
            if (_allRegistrations == null) return;

            List<MedicalRegistration> filteredRegistrations;

            switch (_currentFilter)
            {
                case "Pending":
                    filteredRegistrations = _allRegistrations.Where(r => r.Status == false).ToList();
                    break;
                case "Approved":
                    filteredRegistrations = _allRegistrations.Where(r => r.Status == true).ToList();
                    break;
                case "Rejected":
                    // If Status is bool (not nullable), we'll need another approach
                    // For now, let's assume rejected items might have a different field or we'll handle this differently
                    filteredRegistrations = _allRegistrations.Where(r => !r.Status).ToList(); // Same as Pending for now
                    break;
                default: // "All"
                    filteredRegistrations = _allRegistrations;
                    break;
            }

            RegistrationsDataGrid.ItemsSource = filteredRegistrations.OrderByDescending(r => r.DateSubmitted);
        }

        private void UpdatePendingCount()
        {
            var pendingCount = _allRegistrations?.Count(r => r.Status == false) ?? 0;
            PendingCountText.Text = $"{pendingCount} đơn chờ duyệt";
        }

        private void StatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StatusFilterComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                _currentFilter = selectedItem.Tag.ToString();
                FilterAndDisplayRegistrations();
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadRegistrations();
        }

        private void ViewDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is MedicalRegistration registration)
            {
                var detailWindow = new MedicalRegistrationDetailWindow(registration, _medicalRegistrationService);

                // Subscribe to refresh event
                detailWindow.RegistrationUpdated += () =>
                {
                    // Refresh display
                    LoadRegistrations();
                };

                detailWindow.ShowDialog();
            }
        }

        private async void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is MedicalRegistration registration)
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn duyệt đơn thuốc của {registration.Student?.FullName}?",
                    "Xác nhận duyệt",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    await UpdateRegistrationStatus(registration, true);
                }
            }
        }

        private async void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is MedicalRegistration registration)
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn từ chối đơn thuốc của {registration.Student?.FullName}?",
                    "Xác nhận từ chối",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    await UpdateRegistrationStatus(registration, false); // false for rejected
                }
            }
        }

        private async Task UpdateRegistrationStatus(MedicalRegistration registration, bool newStatus)
        {
            try
            {
                LoadingGrid.Visibility = Visibility.Visible;

                // Update status
                registration.Status = newStatus;
                // If your entity has ProcessedDate and ProcessedBy fields, uncomment these:
                // registration.ProcessedDate = DateTime.Now;
                // registration.ProcessedBy = "danielleit241";

                // Save to database
                await Task.Run(() => _medicalRegistrationService.UpdateRegistration(registration));

                // Update local list
                var index = _allRegistrations.FindIndex(r => r.RegistrationId == registration.RegistrationId);
                if (index >= 0)
                {
                    _allRegistrations[index] = registration;
                }

                // Refresh display
                FilterAndDisplayRegistrations();
                UpdatePendingCount();

                string statusText = newStatus ? "duyệt" : "từ chối";

                MessageBox.Show(
                    $"Đã {statusText} đơn thuốc thành công!",
                    "Thành công",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật trạng thái: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LoadingGrid.Visibility = Visibility.Collapsed;
            }
        }
    }
}