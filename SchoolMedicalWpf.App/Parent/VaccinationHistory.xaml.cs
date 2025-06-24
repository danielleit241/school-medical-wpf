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

        public ObservableCollection<VaccinationResult> AllResults { get; set; } = [];

        public VaccinationHistory(StudentService studentService, User currentUser, Student student)
        {
            InitializeComponent();
            _studentService = studentService;
            _currentUser = currentUser;
            _student = student;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var fakeData = new List<VaccinationResult>
            {
                new VaccinationResult
                {
                    VaccinationResultId = Guid.NewGuid(),
                    VaccinationDate = new DateOnly(2023, 6, 12),
                    DoseNumber = 1,
                    InjectionSite = "Cánh tay trái",
                    ImmediateReaction = "Không",
                    ReactionStartTime = null,
                    ReactionType = null,
                    SeverityLevel = null,
                    Notes = "Vắc xin viêm gan B",
                },
                new VaccinationResult
                {
                    VaccinationResultId = Guid.NewGuid(),
                    VaccinationDate = new DateOnly(2023, 8, 22),
                    DoseNumber = 2,
                    InjectionSite = "Cánh tay phải",
                    ImmediateReaction = "Sưng nhẹ",
                    ReactionStartTime = DateTime.Parse("2023-08-22 15:20:00"),
                    ReactionType = "Phản ứng tại chỗ",
                    SeverityLevel = "Nhẹ",
                    Notes = "Vắc xin sởi",
                },
                new VaccinationResult
                {
                    VaccinationResultId = Guid.NewGuid(),
                    VaccinationDate = new DateOnly(2024, 1, 10),
                    DoseNumber = 1,
                    InjectionSite = "Cẳng chân trái",
                    ImmediateReaction = "Không",
                    ReactionStartTime = null,
                    ReactionType = null,
                    SeverityLevel = null,
                    Notes = "Vắc xin thủy đậu",
                },
                new VaccinationResult
                {
                    VaccinationResultId = Guid.NewGuid(),
                    VaccinationDate = new DateOnly(2024, 3, 5),
                    DoseNumber = 2,
                    InjectionSite = "Cẳng chân phải",
                    ImmediateReaction = "Sốt nhẹ",
                    ReactionStartTime = DateTime.Parse("2024-03-05 18:00:00"),
                    ReactionType = "Sốt",
                    SeverityLevel = "Nhẹ",
                    Notes = "Vắc xin cúm",
                }
            };
            LoadData("Nguyễn Văn A", fakeData);
            DataContext = this;
        }

        public void LoadData(string studentName, IEnumerable<VaccinationResult> data)
        {
            StudentNameText.Text = studentName;
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


    }
}
