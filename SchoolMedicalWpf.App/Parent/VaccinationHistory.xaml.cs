using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Parent
{
    /// <summary>
    /// Interaction logic for VaccinationHistory.xaml
    /// </summary>
    public partial class VaccinationHistory : UserControl
    {
        private readonly StudentService _studentService;
        private readonly User _currentUser;
        private readonly Student _student;
        private readonly VaccinationResultService _vaccinationResultService;

        public ObservableCollection<VaccinationResult> AllResults { get; set; } = [];

        public VaccinationHistory(StudentService studentService, VaccinationResultService vaccinationResultService, User currentUser, Student student)
        {
            InitializeComponent();
            _studentService = studentService;
            _currentUser = currentUser;
            _student = student;
            _vaccinationResultService = vaccinationResultService;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var results = _vaccinationResultService.GetAllVaccinationResults();

            var student = _studentService.GetStudent(_student.StudentId);
            if (student == null || student.HealthProfiles == null || !student.HealthProfiles.Any())
            {
                MessageBox.Show("Student or health profile data is missing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var healthProfileId = student.HealthProfiles.First().HealthProfileId;

            var studentResult = results
                .Where(r => r.HealthProfileId == healthProfileId)
                .OrderByDescending(r => r.VaccinationDate)
                .ToList();

            LoadData(student.FullName, studentResult);
            DataContext = this;
        }

        public void LoadData(string studentName, IEnumerable<VaccinationResult> data)
        {
            AllResults = [.. data];
            VaccinationHistoryDataGrid.ItemsSource = AllResults;
        }

        private void DoFilter()
        {

        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            DoFilter();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
