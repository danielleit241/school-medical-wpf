using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Parent
{
    public partial class HealthCheckHistory : UserControl
    {
        private readonly User _currentUser;
        private readonly Student _student;
        private readonly HealthCheckResultService _healthCheckResultService;
        private readonly StudentService _studentService;

        public ObservableCollection<HealthCheckResult> AllResults { get; set; } = [];

        public HealthCheckHistory(StudentService studentService, HealthCheckResultService healthCheckResultService, User currentUser, Student student)
        {
            InitializeComponent();
            _studentService = studentService;
            _currentUser = currentUser;
            _student = student;
            _healthCheckResultService = healthCheckResultService;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var results = _healthCheckResultService.GetAll();

            var student = _studentService.GetStudent(_student.StudentId);
            if (student == null || student.HealthProfiles == null || !student.HealthProfiles.Any())
            {
                MessageBox.Show("Student or health profile data is missing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var healthProfileId = student.HealthProfiles.First().HealthProfileId;

            var studentResult = results
                .Where(r => r.HealthProfileId == healthProfileId)
                .OrderByDescending(r => r.DatePerformed)
                .ToList();

            LoadData(student.FullName, studentResult);
            DataContext = this;
        }

        public void LoadData(string studentName, IEnumerable<HealthCheckResult> data)
        {
            AllResults = [.. data];
            HealthHistoryDataGrid.ItemsSource = AllResults;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}