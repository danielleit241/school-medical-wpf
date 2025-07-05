using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SchoolMedicalWpf.App.Admin
{
    public partial class CreateUpdateStudent : Window, INotifyPropertyChanged
    {
        private readonly StudentService _studentService;
        private readonly UserService _userService; // Added UserService
        private readonly bool _isEdit;
        private readonly Student? _editingStudent;
        private bool _isFormValid = false;
        private ObservableCollection<User> _users = []; // Added Users collection

        public event PropertyChangedEventHandler? PropertyChanged;

        private string _validationMessage = "Vui lòng điền đầy đủ thông tin bắt buộc";
        public string ValidationMessage
        {
            get => _validationMessage;
            set
            {
                _validationMessage = value;
                OnPropertyChanged(nameof(ValidationMessage));
            }
        }

        public CreateUpdateStudent(StudentService studentService, UserService userService, Student? student = null)
        {
            InitializeComponent();
            DataContext = this;

            _studentService = studentService;
            _userService = userService;
            _isEdit = student != null;
            _editingStudent = student;

            InitializeWindow();
            SetupEventHandlers();

            LoadUsers();

            if (_isEdit && student != null)
            {
                LoadStudentData(student);
            }
            else
            {
                SetDefaultValues();
            }

            ValidateForm();
        }

        private void LoadUsers()
        {
            try
            {
                var task = Task.Run(async () =>
                {
                    var users = await _userService.GetAllUsers().ConfigureAwait(false);
                    return users.Where(u => u.RoleId == 4).ToList();
                });
                _users = new ObservableCollection<User>(task.Result);
                ParentComboBox.ItemsSource = task.Result;
                ParentComboBox.DisplayMemberPath = "FullName"; // Display FullName in ComboBox
                ParentComboBox.SelectedValuePath = "UserId"; // Use UserId as the value path
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách người dùng: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeWindow()
        {
            if (_isEdit)
            {
                WindowTitleTextBlock.Text = "Chỉnh sửa thông tin học sinh";
                SaveButton.Content = "💾 Cập nhật thông tin";
            }
            else
            {
                WindowTitleTextBlock.Text = "Thêm học sinh mới";
                SaveButton.Content = "💾 Lưu thông tin";
            }
        }

        private void SetupEventHandlers()
        {
            // Text change events for real-time validation and preview
            StudentCodeTextBox.TextChanged += OnFormFieldChanged;
            FullNameTextBox.TextChanged += OnFormFieldChanged;
            DayOfBirthPicker.SelectedDateChanged += OnFormFieldChanged;
            GenderComboBox.SelectionChanged += OnFormFieldChanged;
            GradeComboBox.SelectionChanged += OnFormFieldChanged;
            AddressTextBox.TextChanged += OnFormFieldChanged;
            ParentComboBox.SelectionChanged += OnFormFieldChanged;
        }

        private void LoadStudentData(Student student)
        {
            try
            {
                StudentCodeTextBox.Text = student.StudentCode ?? "";
                FullNameTextBox.Text = student.FullName ?? "";
                DayOfBirthPicker.SelectedDate = student.DayOfBirth.HasValue ?
                    student.DayOfBirth.Value.ToDateTime(TimeOnly.MinValue) : null;

                // Set gender combobox
                if (!string.IsNullOrEmpty(student.Gender))
                {
                    foreach (ComboBoxItem item in GenderComboBox.Items)
                    {
                        if (item.Content.ToString() == student.Gender)
                        {
                            GenderComboBox.SelectedItem = item;
                            break;
                        }
                    }
                }

                // Set grade combobox
                if (!string.IsNullOrEmpty(student.Grade))
                {
                    foreach (ComboBoxItem item in GradeComboBox.Items)
                    {
                        if (item.Content.ToString()!.Trim() == student.Grade.Trim())
                        {
                            GradeComboBox.SelectedItem = item;
                            break;
                        }
                    }
                }

                AddressTextBox.Text = student.Address ?? "";

                SetSelectedUser(student.UserId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu học sinh: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void SetSelectedUser(Guid? userId)
        {
            if (!userId.HasValue || !_users.Any())
                return;

            try
            {
                // Tìm user trong collection đã load
                var selectedUser = _users.FirstOrDefault(u => u.UserId == userId.Value);
                if (selectedUser != null)
                {
                    // Set đúng object, không phải UserId
                    ParentComboBox.SelectedItem = selectedUser;
                }
                else
                {
                    MessageBox.Show($"Không tìm thấy user với ID: {userId.Value}", "Debug");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi set selected user: {ex.Message}", "Lỗi");
            }
        }

        private void SetDefaultValues()
        {
            // Auto-generate student code for new students
            GenerateStudentCode();
        }

        private void OnFormFieldChanged(object sender, EventArgs e)
        {
            ValidateForm();
        }


        private void UserComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ValidateForm();
        }

        private void ParentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ValidateForm();
        }

        private void ValidateForm()
        {
            var errors = new List<string>();

            // Required field validation
            if (string.IsNullOrWhiteSpace(StudentCodeTextBox.Text))
                errors.Add("Mã học sinh là bắt buộc");

            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text))
                errors.Add("Họ tên là bắt buộc");

            if (!DayOfBirthPicker.SelectedDate.HasValue)
                errors.Add("Ngày sinh là bắt buộc");

            if (GenderComboBox.SelectedItem == null)
                errors.Add("Giới tính là bắt buộc");

            if (GradeComboBox.SelectedItem == null)
                errors.Add("Khối lớp là bắt buộc");

            if (ParentComboBox.SelectedItem == null) // Validate User selection
                errors.Add("Phải chọn người phụ trách");

            // Age validation
            if (DayOfBirthPicker.SelectedDate.HasValue)
            {
                var age = DateTime.Now.Year - DayOfBirthPicker.SelectedDate.Value.Year;
                if (age < 6 || age > 20)
                    errors.Add("Tuổi học sinh phải từ 6-20");
            }

            _isFormValid = errors.Count == 0;
            SaveButton.IsEnabled = _isFormValid;

            // Update validation message
            if (_isFormValid)
            {
                ValidationMessage = "✅ Tất cả thông tin hợp lệ";
                ValidationPanel.Background = new SolidColorBrush(Color.FromRgb(232, 245, 232)); // Light green
            }
            else
            {
                ValidationMessage = $"❌ {errors.Count} lỗi: {string.Join(", ", errors)}";
                ValidationPanel.Background = new SolidColorBrush(Color.FromRgb(253, 236, 234)); // Light red
            }
        }

        private void GenerateStudentCode()
        {
            try
            {
                // Generate code based on current year and a random number
                var year = DateTime.Now.Year.ToString().Substring(2); // Last 2 digits of year
                var random = new Random().Next(1000, 9999);
                var code = $"HS{year}{random}";

                StudentCodeTextBox.Text = code;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo mã học sinh: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isFormValid)
            {
                MessageBox.Show("Vui lòng kiểm tra và sửa các lỗi trong form trước khi lưu.",
                    "Thông tin không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                SaveButton.IsEnabled = false;
                SaveButton.Content = "🔄 Đang lưu...";

                var selectedUser = ParentComboBox.SelectedItem as User; // Get selected User

                if (_isEdit && _editingStudent != null)
                {
                    // Update existing student
                    _editingStudent.StudentCode = StudentCodeTextBox.Text.Trim();
                    _editingStudent.FullName = FullNameTextBox.Text.Trim();
                    _editingStudent.DayOfBirth = DayOfBirthPicker.SelectedDate.HasValue ?
                        DateOnly.FromDateTime(DayOfBirthPicker.SelectedDate.Value) : null;
                    _editingStudent.Gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                    _editingStudent.Grade = (GradeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                    _editingStudent.Address = AddressTextBox.Text.Trim();

                    // Assign UserId from selected User
                    _editingStudent.UserId = selectedUser?.UserId;
                    var user = ParentComboBox.SelectedItem as User;
                    if (user != null)
                    {
                        _editingStudent.ParentPhoneNumber = user.PhoneNumber;
                        _editingStudent.ParentEmailAddress = user.EmailAddress;
                    }

                    await _studentService.UpdateStudent(_editingStudent);
                    MessageBox.Show("Cập nhật thông tin học sinh thành công!", "Thành công",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Create new student
                    var newStudent = new Student
                    {
                        StudentCode = StudentCodeTextBox.Text.Trim(),
                        FullName = FullNameTextBox.Text.Trim(),
                        DayOfBirth = DayOfBirthPicker.SelectedDate.HasValue ?
                            DateOnly.FromDateTime(DayOfBirthPicker.SelectedDate.Value) : null,
                        Gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                        Grade = (GradeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                        Address = AddressTextBox.Text.Trim(),
                        UserId = selectedUser?.UserId // Assign UserId from dropdown selection
                    };

                    await _studentService.AddStudent(newStudent);
                    MessageBox.Show("Thêm học sinh mới thành công!", "Thành công",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu thông tin: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SaveButton.IsEnabled = true;
                SaveButton.Content = _isEdit ? "💾 Cập nhật thông tin" : "💾 Lưu thông tin";
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (HasUnsavedChanges())
            {
                var result = MessageBox.Show(
                    "Bạn có thay đổi chưa được lưu. Bạn có chắc muốn đóng?",
                    "Xác nhận đóng",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question,
                    MessageBoxResult.No);

                if (result == MessageBoxResult.No)
                    return;
            }

            DialogResult = false;
            Close();
        }

        private bool HasUnsavedChanges()
        {
            if (!_isEdit)
            {
                // For new students, check if any field has been filled
                return !string.IsNullOrWhiteSpace(StudentCodeTextBox.Text) ||
                       !string.IsNullOrWhiteSpace(FullNameTextBox.Text) ||
                       DayOfBirthPicker.SelectedDate.HasValue ||
                       GenderComboBox.SelectedItem != null ||
                       GradeComboBox.SelectedItem != null ||
                       !string.IsNullOrWhiteSpace(AddressTextBox.Text) ||
                       ParentComboBox.SelectedItem != null;
            }

            // For editing, compare with original values
            if (_editingStudent == null) return false;

            return StudentCodeTextBox.Text.Trim() != (_editingStudent.StudentCode ?? "") ||
                   FullNameTextBox.Text.Trim() != (_editingStudent.FullName ?? "") ||
                   (GenderComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() != _editingStudent.Gender ||
                   (GradeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() != _editingStudent.Grade ||
                   AddressTextBox.Text.Trim() != (_editingStudent.Address ?? "") ||
                   (ParentComboBox.SelectedItem as User)?.UserId != _editingStudent.UserId;
        }

        // Quick Action Event Handlers
        private void AutoGenerateCodeButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateStudentCode();
        }

        private void ResetFormButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Bạn có chắc muốn xóa tất cả thông tin đã nhập?",
                "Xác nhận làm mới",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {
                StudentCodeTextBox.Clear();
                FullNameTextBox.Clear();
                DayOfBirthPicker.SelectedDate = null;
                GenderComboBox.SelectedItem = null;
                GradeComboBox.SelectedItem = null;
                AddressTextBox.Clear();
                ParentComboBox.SelectedItem = null;
                ParentComboBox.SelectedItem = null; // Reset User selection

                if (!_isEdit)
                {
                    GenerateStudentCode();
                }
            }
        }

        private void CopyInfoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedUser = ParentComboBox.SelectedItem as User;

                var info = $"=== THÔNG TIN HỌC SINH ===\n" +
                          $"Mã học sinh: {StudentCodeTextBox.Text}\n" +
                          $"Họ tên: {FullNameTextBox.Text}\n" +
                          $"Ngày sinh: {DayOfBirthPicker.SelectedDate?.ToString("dd/MM/yyyy") ?? "Chưa có"}\n" +
                          $"Giới tính: {(GenderComboBox.SelectedItem as ComboBoxItem)?.Content ?? "Chưa chọn"}\n" +
                          $"Khối lớp: {(GradeComboBox.SelectedItem as ComboBoxItem)?.Content ?? "Chưa chọn"}\n" +
                          $"Địa chỉ: {AddressTextBox.Text}\n\n" +
                          $"=== NGƯỜI PHỤ TRÁCH ===\n" +
                          $"Họ tên: {selectedUser?.FullName ?? "Chưa chọn"}\n";

                Clipboard.SetText(info);
                MessageBox.Show("Đã sao chép thông tin vào clipboard!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi sao chép: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddNewParentButton_Click(object sender, RoutedEventArgs e)
        {
            // Implementation for adding new parent
            MessageBox.Show("Tính năng thêm phụ huynh mới sẽ được triển khai sau.", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}