using System.Windows;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Parent
{
    /// <summary>
    /// Interaction logic for HealthResultDetailWindow.xaml
    /// </summary>
    public partial class HealthResultDetailWindow : Window
    {
        public HealthResultDetailWindow(HealthCheckResult healthResult)
        {
            InitializeComponent();
            LoadHealthDetails(healthResult);
        }

        private void LoadHealthDetails(HealthCheckResult healthResult)
        {
            if (healthResult == null) return;

            StudentNameText.Text = healthResult.HealthProfile?.Student?.FullName ?? "N/A";
            StudentClassText.Text = healthResult.HealthProfile?.Student?.Grade ?? "N/A";
            DateText.Text = healthResult.DatePerformed?.ToString("dd/MM/yyyy") ?? "N/A";
            HeightText.Text = $"{healthResult.Height} cm";
            WeightText.Text = $"{healthResult.Weight} kg";
            VisionLeftText.Text = healthResult.VisionLeft?.ToString() ?? "N/A";
            VisionRightText.Text = healthResult.VisionRight?.ToString() ?? "N/A";
            HearingText.Text = healthResult.Hearing ?? "N/A";
            NoseText.Text = healthResult.Nose ?? "N/A";
            BloodPressureText.Text = healthResult.BloodPressure ?? "N/A";
            NotesText.Text = healthResult.Notes ?? "Không có ghi chú";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
