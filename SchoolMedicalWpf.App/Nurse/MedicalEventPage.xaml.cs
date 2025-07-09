using Microsoft.Extensions.DependencyInjection;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;
using System.Windows;
using System.Windows.Controls;

namespace SchoolMedicalWpf.App.Nurse
{
    public partial class MedicalEventPage : UserControl
    {
        private readonly MedicalEventService _medicalEventService;
        private readonly StudentService _studentService;
        private readonly User _currentUser;
        private List<MedicalEvent> _allEvents;
        private string _currentFilter = "All";
        private bool _isLoading = false;

        public MedicalEventPage(MedicalEventService medicalEventService, StudentService studentService, User currentUser)
        {
            InitializeComponent();
            _medicalEventService = medicalEventService;
            _studentService = studentService;
            _currentUser = currentUser;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_allEvents == null)
            {
                await LoadEventsAsync();
            }
        }

        private async Task LoadEventsAsync()
        {
            if (_isLoading) return;

            try
            {
                _isLoading = true;
                LoadingGrid.Visibility = Visibility.Visible;

                _allEvents = await Task.Run(() => _medicalEventService.GetAllMedicalEvents());

                FilterAndDisplayEvents();
                UpdateEventCounts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _isLoading = false;
                LoadingGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void FilterAndDisplayEvents()
        {
            if (_allEvents == null) return;

            List<MedicalEvent> filteredEvents;

            switch (_currentFilter)
            {
                case "Nghiêm trọng":
                    filteredEvents = _allEvents.Where(e => e.SeverityLevel == "Nghiêm trọng").ToList();
                    break;
                case "Trung bình":
                    filteredEvents = _allEvents.Where(e => e.SeverityLevel == "Trung bình").ToList();
                    break;
                case "Nhẹ":
                    filteredEvents = _allEvents.Where(e => e.SeverityLevel == "Nhẹ").ToList();
                    break;
                default:
                    filteredEvents = _allEvents;
                    break;
            }

            var sortedEvents = filteredEvents
                .OrderByDescending(e => e.EventDate)
                .ThenBy(e => e.SeverityLevel == "Nghiêm trọng" ? 0 : e.SeverityLevel == "Trung Bình" ? 1 : 2)
                .ToList();

            EventsDataGrid.ItemsSource = sortedEvents;
        }

        private void UpdateEventCounts()
        {
            if (_allEvents == null) return;

            var today = DateOnly.FromDateTime(DateTime.Now);

            var todayCount = _allEvents.Count(e => e.EventDate == today);
            TodayEventCountText.Text = $"{todayCount} sự kiện hôm nay";

            var highSeverityCount = _allEvents.Count(e => e.SeverityLevel == "Nghiêm trọng");
            HighSeverityCountText.Text = $"{highSeverityCount} mức độ cao";
        }

        private void SeverityFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SeverityFilterComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                _currentFilter = selectedItem.Tag.ToString()!;
                FilterAndDisplayEvents();
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadEventsAsync();
        }

        private void CreateEventButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var createWindow = ActivatorUtilities.CreateInstance<MedicalEventFormWindow>(App.Services, _currentUser);
                createWindow.Owner = Window.GetWindow(this);

                createWindow.EventCreated += async () =>
                {
                    await LoadEventsAsync();
                };

                createWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form tạo sự kiện: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is MedicalEvent medicalEvent)
            {
                try
                {
                    var detailWindow = ActivatorUtilities.CreateInstance<MedicalEventDetailWindow>(
                        App.Services, medicalEvent, _currentUser);
                    detailWindow.Owner = Window.GetWindow(this);

                    detailWindow.EventUpdated += async () =>
                    {
                        await LoadEventsAsync();
                    };

                    detailWindow.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ Lỗi khi mở chi tiết sự kiện: {ex.Message}\n\n" +
                        $"🕐 Thời gian: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n" +
                        $"👤 User: {_currentUser?.FullName ?? "N/A"}", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}