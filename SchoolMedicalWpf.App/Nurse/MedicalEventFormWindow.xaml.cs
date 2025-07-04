using System.Windows;
using System.Windows.Controls;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Nurse
{
    public partial class MedicalEventFormWindow : Window
    {
        private readonly MedicalEventService _medicalEventService;
        private readonly StudentService _studentService;
        private readonly User _currentUser;
        private bool _isSubmitting = false;

        public event Action EventCreated;

        public MedicalEventFormWindow(
            MedicalEventService medicalEventService,
            StudentService studentService,
            User currentUser)
        {
            InitializeComponent();
            _medicalEventService = medicalEventService;
            _studentService = studentService;
            _currentUser = currentUser;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadStudentsAsync();

            EventDatePicker.SelectedDate = DateTime.UtcNow;
            SeverityComboBox.SelectedIndex = 1;
            ParentNotifiedCheckBox.IsChecked = false;

            StudentComboBox.SelectionChanged += StudentComboBox_SelectionChanged;
        }

        private void StudentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StudentComboBox.SelectedItem is Student selectedStudent)
            {
                SelectedStudentCodeText.Text = selectedStudent.StudentCode ?? "N/A";
                SelectedStudentClassText.Text = selectedStudent.Grade ?? "N/A";
                StudentInfoDisplay.Visibility = Visibility.Visible;
            }
            else
            {
                StudentInfoDisplay.Visibility = Visibility.Collapsed;
            }
        }

        private async Task LoadStudentsAsync()
        {
            try
            {
                StudentComboBox.IsEnabled = false;
                var students = await Task.Run(() => _studentService.GetAllStudents());
                StudentComboBox.ItemsSource = students;
                StudentComboBox.DisplayMemberPath = "FullName";
                StudentComboBox.SelectedValuePath = "StudentId";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải học sinh: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                StudentComboBox.IsEnabled = true;
            }
        }

        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isSubmitting) return;

            try
            {
                _isSubmitting = true;
                CreateButton.IsEnabled = false;
                CreateButton.Content = "Đang tạo...";

                if (!ValidateInput()) return;

                var medicalEvent = new MedicalEvent
                {
                    EventId = Guid.NewGuid(),
                    StudentId = (Guid?)StudentComboBox.SelectedValue,
                    StaffNurseId = _currentUser?.UserId,
                    EventType = GetEventType(),
                    EventDescription = EventDescriptionTextBox.Text?.Trim(),
                    Location = LocationComboBox.Text?.Trim(),
                    SeverityLevel = GetSeverityLevel(),
                    ParentNotified = ParentNotifiedCheckBox.IsChecked ?? false,
                    EventDate = EventDatePicker.SelectedDate.HasValue
                        ? DateOnly.FromDateTime(EventDatePicker.SelectedDate.Value)
                        : DateOnly.FromDateTime(DateTime.UtcNow),
                    Notes = NotesTextBox.Text?.Trim()
                };

                await Task.Run(() => _medicalEventService.AddMedicalEvent(medicalEvent));

                MessageBox.Show("Sự kiện y tế đã được tạo thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                EventCreated?.Invoke();
                this.DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo sự kiện: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _isSubmitting = false;
                CreateButton.IsEnabled = true;
                CreateButton.Content = "💾 Tạo sự kiện";
            }
        }

        private string GetEventType()
        {
            if (EventTypeComboBox.SelectedItem is ComboBoxItem item)
            {
                var content = item.Content.ToString();
                return content!.Contains(' ') ? content.Substring(content.IndexOf(' ') + 1) : content;
            }
            return EventTypeComboBox.Text?.Trim()!;
        }

        private string GetSeverityLevel()
        {
            return ((ComboBoxItem)SeverityComboBox.SelectedItem)?.Tag?.ToString() ?? "Medium";
        }

        private bool ValidateInput()
        {
            if (StudentComboBox.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn học sinh.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(EventDescriptionTextBox.Text))
            {
                MessageBox.Show("Vui lòng nhập mô tả sự kiện.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!EventDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Vui lòng chọn ngày.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}