using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Parent
{
    /// <summary>
    /// Interaction logic for ParentHealthDeclarationPage.xaml
    /// </summary>
    public partial class ParentHealthDeclarationPage : UserControl
    {
        public ObservableCollection<Student> StudentList { get; set; } = new();
        private readonly User _currentUser;
        private readonly StudentService _studentService;
        private bool _isLoading = false;

        public ParentHealthDeclarationPage(User user, StudentService studentService)
        {
            InitializeComponent();
            _currentUser = user ?? throw new ArgumentNullException(nameof(user));
            _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
            DataContext = this;
            Loaded += ParentHealthDeclarationPage_Loaded;
        }

        private async void ParentHealthDeclarationPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadStudentListAsync();
        }

        public async Task LoadStudentListAsync()
        {
            if (_isLoading) return;

            try
            {
                _isLoading = true;

                // Show loading indicator
                Application.Current.Dispatcher.Invoke(() =>
                {
                    LoadingPanel.Visibility = Visibility.Visible;
                });

                if (_currentUser == null || _studentService == null)
                {
                    MessageBox.Show("❌ Không có dữ liệu đăng nhập hoặc dịch vụ học sinh.\n\n" +
                        $"🕐 Thời gian: {DateTime.Now}\n" +
                        $"👤 User: {_currentUser!.FullName}", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var students = await Task.Run(() => _studentService.GetStudentsByUserId(_currentUser.UserId));

                Application.Current.Dispatcher.Invoke(() =>
                {
                    StudentList.Clear();
                    foreach (var student in students)
                    {
                        StudentList.Add(student);
                    }
                });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"❌ Lỗi khi tải danh sách học sinh: {ex.Message}\n\n" +
                        $"🕐 Thời gian: {DateTime.Now}\n" +
                        $"👤 User: {_currentUser!.FullName}", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                _isLoading = false;

                // Hide loading indicator
                Application.Current.Dispatcher.Invoke(() =>
                {
                    LoadingPanel.Visibility = Visibility.Collapsed;
                });
            }
        }
        public async Task RefreshAsync()
        {
            await LoadStudentListAsync();
        }
    }
}