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
        private readonly StudentService _studentService;

        public ObservableCollection<HealthCheckResult> AllResults { get; set; } = [];

        public HealthCheckHistory(StudentService studentService, User currentUser, Student student)
        {
            InitializeComponent();
            _studentService = studentService;
            _currentUser = currentUser;
            _student = student;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var fakeData = new List<HealthCheckResult>
            {
                new HealthCheckResult
                {
                    ResultId = Guid.NewGuid(),
                    DatePerformed = new DateOnly(2023, 9, 20),
                    Height = 120.5,
                    Weight = 23.8,
                    VisionLeft = 10,
                    VisionRight = 10,
                    Hearing = "Bình thường",
                    Nose = "Không viêm",
                    BloodPressure = "105/65",
                    Notes = "Khỏe mạnh",
                },
                new HealthCheckResult
                {
                    ResultId = Guid.NewGuid(),
                    DatePerformed = new DateOnly(2024, 3, 1),
                    Height = 125.0,
                    Weight = 25.5,
                    VisionLeft = 9,
                    VisionRight = 10,
                    Hearing = "Bình thường",
                    Nose = "Viêm nhẹ",
                    BloodPressure = "108/67",
                    Notes = "Cần theo dõi mũi",
                },
                new HealthCheckResult
                {
                    ResultId = Guid.NewGuid(),
                    DatePerformed = new DateOnly(2025, 1, 12),
                    Height = 130.2,
                    Weight = 27.1,
                    VisionLeft = 9.5,
                    VisionRight = 9.5,
                    Hearing = "Bình thường",
                    Nose = "Không viêm",
                    BloodPressure = "110/70",
                    Notes = "Phát triển tốt",
                }
            };
            LoadData(_student.FullName, fakeData);
            DataContext = this;
        }

        public void LoadData(string studentName, IEnumerable<HealthCheckResult> data)
        {
            StudentNameText.Text = studentName;
            AllResults = [.. data];
            HealthCheckHistoryDataGrid.ItemsSource = AllResults;
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}