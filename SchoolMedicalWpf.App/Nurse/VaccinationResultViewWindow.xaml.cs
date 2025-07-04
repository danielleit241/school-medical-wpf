using System.Windows;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Nurse
{
    public partial class VaccinationResultViewWindow : Window
    {
        private readonly DateTime _currentDateTime = DateTime.Now;
        private readonly User _currentUser;

        public VaccinationResultViewWindow(Guid healthProfileId,
                                          Guid? scheduleId,
                                          string studentCode,
                                          string studentName,
                                          string grade,
                                          VaccinationResult existingResult,
                                          User currentUser)
        {
            InitializeComponent();

            Title = $"Xem kết quả tiêm chủng - {studentName}";
            _currentUser = currentUser;

            InitializeWindow(studentCode, studentName, grade, existingResult, currentUser);
        }

        private void InitializeWindow(string studentCode, string studentName, string grade,
                                    VaccinationResult result, User currentUser)
        {
            txtDateTime.Text = $"{_currentDateTime:yyyy-MM-dd HH:mm:ss} UTC - Y tá: {currentUser?.FullName}";

            txtStudentCode.Text = studentCode;
            txtStudentName.Text = studentName;
            txtGrade.Text = grade;
            txtHealthProfileId.Text = result.HealthProfileId.ToString();

            LoadVaccinationData(result);
        }

        private void LoadVaccinationData(VaccinationResult result)
        {
            try
            {
                txtVaccinationDate.Text = result.VaccinationDate?.ToString("dd/MM/yyyy") ?? "Chưa ghi nhận";
                txtDoseNumber.Text = result.DoseNumber?.ToString() ?? "Chưa ghi nhận";
                txtInjectionSite.Text = result.InjectionSite ?? "Chưa ghi nhận";
                txtImmediateReaction.Text = result.ImmediateReaction ?? "Không có";
                txtReactionStartTime.Text = result.ReactionStartTime?.ToString("dd/MM/yyyy HH:mm") ?? "Không có";
                txtReactionType.Text = result.ReactionType ?? "Không có";
                txtSeverityLevel.Text = result.SeverityLevel ?? "Không có";
                txtNotes.Text = result.Notes ?? "Không có ghi chú";

                var headerText = "👁️ XEM KẾT QUẢ TIÊM CHỦNG";
                if (result.VaccinationDate.HasValue)
                {
                    headerText += $" - {result.VaccinationDate:dd/MM/yyyy}";
                }

                if (FindName("txtDateTime") is System.Windows.Controls.TextBlock header)
                {
                    var border = header.Parent as System.Windows.Controls.Border;
                    if (border?.Child is System.Windows.Controls.StackPanel panel &&
                        panel.Children[0] is System.Windows.Controls.TextBlock titleBlock)
                    {
                        titleBlock.Text = headerText;
                    }
                }

                if (FindName("txtSeverityLevel") is System.Windows.Controls.TextBox severityBox)
                {
                    switch (result.SeverityLevel?.ToLower())
                    {
                        case "nặng":
                        case "rất nặng":
                        case "rất nặng - cần cấp cứu":
                            severityBox.Foreground = System.Windows.Media.Brushes.Red;
                            severityBox.FontWeight = FontWeights.Bold;
                            break;
                        case "vừa":
                            severityBox.Foreground = System.Windows.Media.Brushes.Orange;
                            break;
                        case "nhẹ":
                            severityBox.Foreground = System.Windows.Media.Brushes.Green;
                            break;
                        default:
                            severityBox.Foreground = System.Windows.Media.Brushes.Black;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi khi tải dữ liệu kết quả tiêm chủng.", "Lỗi",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                Console.WriteLine($"Lỗi khi tải dữ liệu tiêm chủng: {ex.Message}");
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
        }
    }
}