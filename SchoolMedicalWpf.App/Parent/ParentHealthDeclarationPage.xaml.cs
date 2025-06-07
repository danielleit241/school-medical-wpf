using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        private StudentService _studentService = new();

        public ParentHealthDeclarationPage(User user)
        {
            InitializeComponent();
            _currentUser = user;
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
