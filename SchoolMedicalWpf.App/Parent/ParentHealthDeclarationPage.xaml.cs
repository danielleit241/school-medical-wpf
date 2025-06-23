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

        public ParentHealthDeclarationPage(User user, StudentService studentService)
        {
            InitializeComponent();
            _currentUser = user ?? throw new ArgumentNullException(nameof(user));
            _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
            LoadStudentList();
            DataContext = this;
        }

        public void LoadStudentList()
        {
            try
            {
                if (_currentUser == null || _studentService == null)
                {
                    MessageBox.Show("Không có dữ liệu đăng nhập hoặc dịch vụ học sinh.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var students = _studentService.GetStudentsByUserId(_currentUser.UserId);
                StudentList.Clear();
                foreach (var student in students)
                {
                    StudentList.Add(student);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách học sinh: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
