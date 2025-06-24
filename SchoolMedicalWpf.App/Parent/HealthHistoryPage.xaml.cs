using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Parent
{
    /// <summary>
    /// Interaction logic for HealthHistoryPage.xaml
    /// </summary>

    public partial class HealthHistoryPage : UserControl
    {
        public List<Student> Students { get; set; } = [];

        private ContentControl _mainContent;
        private readonly UserService _userService;
        private readonly StudentService _studentSerivce;
        private readonly User _currentUser;

        public HealthHistoryPage(UserService userService, StudentService studentService, User currentUser, ContentControl mainContent)
        {
            InitializeComponent();
            _userService = userService;
            _studentSerivce = studentService;
            _currentUser = currentUser;
            _mainContent = mainContent;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var students = _studentSerivce.GetStudents();
            students = [.. students.Where(s => s.UserId == _currentUser.UserId)];
            Students = [.. students];
            this.DataContext = this;
        }

        private void HealthHistory_Click(object sender, RoutedEventArgs e)
        {
            var student = (sender as Button).DataContext as Student;
            if (student == null)
            {
                MessageBox.Show("Vui lòng chọn học sinh để xem lịch sử khám sức khỏe.");
                return;
            }
            var healthHistoryControl = ActivatorUtilities.CreateInstance<HealthCheckHistory>(App.Services, _currentUser, student);
            _mainContent.Content = healthHistoryControl;
        }

        private void VaccinationHistory_Click(object sender, RoutedEventArgs e)
        {
            var student = (sender as Button).DataContext as Student;
            if (student == null)
            {
                MessageBox.Show("Vui lòng chọn học sinh để xem lịch sử tiêm chủng.");
                return;
            }
            var vacciantionHistory = ActivatorUtilities.CreateInstance<VaccinationHistory>(App.Services, _currentUser, student);
            _mainContent.Content = vacciantionHistory;
        }
    }
}
