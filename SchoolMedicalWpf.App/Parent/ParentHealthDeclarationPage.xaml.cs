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
        public ObservableCollection<Student> StudentList { get; set; } = new ObservableCollection<Student>();
        private User _currentUser;
        private StudentService _studentService;

        public ParentHealthDeclarationPage(User user, StudentService studentService)
        {
            InitializeComponent();
            _currentUser = user;
            _studentService = studentService;
            LoadStudentListAsync();
            DataContext = this;
        }

        public void LoadStudentListAsync()
        {
            try
            {
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
