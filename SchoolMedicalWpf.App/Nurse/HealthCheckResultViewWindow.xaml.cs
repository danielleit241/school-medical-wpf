using System.Windows;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Nurse
{
    public partial class HealthCheckResultViewWindow : Window
    {
        private readonly DateTime _currentDateTime = DateTime.Now;
        private readonly User _currentUser;

        public HealthCheckResultViewWindow(Guid healthProfileId,
                                          Guid? scheduleId,
                                          string studentCode,
                                          string studentName,
                                          string grade,
                                          HealthCheckResult existingResult,
                                          User currentUser)
        {
            InitializeComponent();

            Title = $"Xem kết quả khám sức khỏe - {studentName}";

            _currentUser = currentUser;

            InitializeWindow(studentCode, studentName, grade, existingResult, currentUser);
        }

        private void InitializeWindow(string studentCode, string studentName, string grade,
                                    HealthCheckResult result, User currentUser)
        {
            txtDateTime.Text = $"{_currentDateTime:yyyy-MM-dd HH:mm:ss} UTC - Y tá: {currentUser?.FullName}";

            txtStudentCode.Text = studentCode;
            txtStudentName.Text = studentName;
            txtGrade.Text = grade;
            txtHealthProfileId.Text = result.HealthProfileId.ToString();

            LoadHealthCheckData(result);
        }

        private void LoadHealthCheckData(HealthCheckResult result)
        {
            try
            {
                txtDatePerformed.Text = result.DatePerformed?.ToString("dd/MM/yyyy") ?? "Chưa ghi nhận";
                txtHeight.Text = result.Height?.ToString("F1") ?? "Chưa đo";
                txtWeight.Text = result.Weight?.ToString("F1") ?? "Chưa đo";
                txtVisionLeft.Text = result.VisionLeft?.ToString("F1") ?? "Chưa kiểm tra";
                txtVisionRight.Text = result.VisionRight?.ToString("F1") ?? "Chưa kiểm tra";
                txtHearing.Text = result.Hearing ?? "Chưa kiểm tra";
                txtNose.Text = result.Nose ?? "Chưa kiểm tra";
                txtBloodPressure.Text = result.BloodPressure ?? "Chưa đo";
                txtNotes.Text = result.Notes ?? "Không có ghi chú";

                var headerText = "👁️ XEM KẾT QUẢ KHÁM SỨC KHỎE";
                if (result.DatePerformed.HasValue)
                {
                    headerText += $" - {result.DatePerformed:dd/MM/yyyy}";
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi khi tải dữ liệu kết quả khám sức khỏe.", "Lỗi",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
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