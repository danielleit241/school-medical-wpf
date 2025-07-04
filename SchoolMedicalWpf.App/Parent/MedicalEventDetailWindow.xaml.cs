using System.Windows;
using System.Windows.Media;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Parent
{
    /// <summary>
    /// Interaction logic for MedicalEventDetailWindow.xaml
    /// </summary>
    public partial class MedicalEventDetailWindow : Window
    {
        public MedicalEventDetailWindow(MedicalEvent medicalEvent, string staffNurseName, string currentUserName)
        {
            InitializeComponent();
            LoadMedicalEventDetails(medicalEvent, staffNurseName, currentUserName);
        }

        private void LoadMedicalEventDetails(MedicalEvent medicalEvent, string staffNurseName, string currentUserName)
        {
            if (medicalEvent == null) return;

            StudentNameText.Text = medicalEvent.Student?.FullName ?? "N/A";
            StudentClassText.Text = medicalEvent.Student?.Grade ?? "N/A";
            DateText.Text = medicalEvent.EventDate?.ToString("dd/MM/yyyy");
            EventTypeText.Text = medicalEvent.EventType ?? "N/A";
            DescriptionText.Text = medicalEvent.EventDescription ?? "N/A";
            LocationText.Text = medicalEvent.Location ?? "N/A";
            StaffNurseText.Text = staffNurseName ?? "N/A";
            NotesText.Text = medicalEvent.Notes ?? "Không có ghi chú";

            SetSeverityDisplay(medicalEvent.SeverityLevel!);
            ViewTimeText.Text = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";
            ViewerText.Text = currentUserName ?? "N/A";
        }

        private void SetSeverityDisplay(string severityLevel)
        {
            var displayText = GetSeverityDisplayText(severityLevel);
            SeverityText.Text = displayText;

            switch (severityLevel?.ToLower())
            {
                case "nhẹ":
                case "nhe":
                    SeverityBorder.Background = new SolidColorBrush(Color.FromRgb(232, 245, 232)); // #E8F5E8
                    SeverityText.Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 80)); // #4CAF50
                    break;
                case "trung bình":
                case "trung binh":
                    SeverityBorder.Background = new SolidColorBrush(Color.FromRgb(255, 243, 224)); // #FFF3E0
                    SeverityText.Foreground = new SolidColorBrush(Color.FromRgb(255, 152, 0)); // #FF9800
                    break;
                case "nghiêm trọng":
                case "nghiem trong":
                    SeverityBorder.Background = new SolidColorBrush(Color.FromRgb(255, 235, 238)); // #FFEBEE
                    SeverityText.Foreground = new SolidColorBrush(Color.FromRgb(244, 67, 54)); // #F44336
                    break;
                default:
                    SeverityBorder.Background = new SolidColorBrush(Color.FromRgb(245, 245, 245)); // #F5F5F5
                    SeverityText.Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102)); // #666666
                    break;
            }
        }

        private string GetSeverityDisplayText(string severityLevel)
        {
            return severityLevel switch
            {
                "Nhẹ" or "nhe" => "Nhẹ",
                "Trung bình" or "trung binh" => "Trung bình",
                "Nghiêm trọng" or "nghiem trong" => "Nghiêm trọng",
                _ => severityLevel ?? "N/A"
            };
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
