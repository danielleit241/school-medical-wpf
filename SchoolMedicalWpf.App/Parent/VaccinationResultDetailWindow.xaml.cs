using System.Windows;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Parent
{
    /// <summary>
    /// Interaction logic for VaccinationResultDetailWindow.xaml
    /// </summary>
    public partial class VaccinationResultDetailWindow : Window
    {
        public VaccinationResultDetailWindow(VaccinationResult vaccinationResult)
        {
            InitializeComponent();
            LoadVaccinationDetails(vaccinationResult);
        }

        private void LoadVaccinationDetails(VaccinationResult vaccinationResult)
        {
            if (vaccinationResult == null) return;

            StudentNameText.Text = vaccinationResult.HealthProfile?.Student?.FullName ?? "N/A";
            StudentClassText.Text = vaccinationResult.HealthProfile?.Student?.Grade ?? "N/A";
            DateText.Text = vaccinationResult.VaccinationDate?.ToString("dd/MM/yyyy");
            VaccineNameText.Text = vaccinationResult.Schedule?.Vaccine?.VaccineName ?? "N/A";
            DoseNumberText.Text = vaccinationResult.DoseNumber?.ToString() ?? "N/A";
            ManufacturerText.Text = vaccinationResult.Schedule?.Vaccine?.Manufacturer ?? "N/A";
            BatchNumberText.Text = vaccinationResult.Schedule?.Vaccine?.BatchNumber ?? "N/A";
            InjectionSiteText.Text = vaccinationResult.InjectionSite ?? "N/A";
            NotesText.Text = vaccinationResult.Notes ?? "Không có ghi chú";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
