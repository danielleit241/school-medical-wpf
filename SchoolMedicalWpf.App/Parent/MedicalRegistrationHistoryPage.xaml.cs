using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace SchoolMedicalWpf.App.Parent
{
    public class StudentDto
    {
        public Guid StudentId { get; set; }
        public string FullName { get; set; }
    }

    public class MedicalRegistration
    {
        public Guid RegistrationId { get; set; }
        public StudentDto Student { get; set; }
        public string MedicationName { get; set; }
        public string TotalDosages { get; set; }
        public string Notes { get; set; }
        public bool ParentalConsent { get; set; }
        public bool Status { get; set; }
        public DateTime DateSubmitted { get; set; }
        public DateTime? DateApproved { get; set; }
    }

    public partial class MedicalRegistrationHistoryPage : UserControl
    {
        public ObservableCollection<MedicalRegistration> MedicalRegistrationList { get; set; } = [];

        public MedicalRegistrationHistoryPage()
        {
            InitializeComponent();
            // Dữ liệu học sinh giả
            var student1 = new StudentDto { StudentId = Guid.NewGuid(), FullName = "Nguyễn Văn An" };
            var student2 = new StudentDto { StudentId = Guid.NewGuid(), FullName = "Trần Thị Bình" };

            //Data giả cho lịch sử tiêm thuốc
            MedicalRegistrationList = new ObservableCollection<MedicalRegistration>
            {
                 new MedicalRegistration
                 {
                     RegistrationId = Guid.NewGuid(),
                     Student = student1,
                     MedicationName = "Vắc xin Cúm",
                     TotalDosages = "1 mũi",
                     Notes = "Không có phản ứng phụ",
                     ParentalConsent = true,
                     Status = true,
                     DateSubmitted = DateTime.Today.AddDays(-10),
                     DateApproved = DateTime.Today.AddDays(-9)
                 },
                 new MedicalRegistration
                 {
                     RegistrationId = Guid.NewGuid(),
                     Student = student2,
                     MedicationName = "Vắc xin Sởi",
                     TotalDosages = "2 mũi",
                     Notes = "Mệt nhẹ sau tiêm",
                     ParentalConsent = true,
                     Status = false,
                     DateSubmitted = DateTime.Today.AddDays(-5),
                     DateApproved = null
                 },
                 new MedicalRegistration
                 {
                     RegistrationId = Guid.NewGuid(),
                     Student = student1,
                     MedicationName = "Vắc xin Viêm gan B",
                     TotalDosages = "1 mũi",
                     Notes = "Đã tiêm tại trường",
                     ParentalConsent = true,
                     Status = true,
                     DateSubmitted = DateTime.Today.AddDays(-30),
                     DateApproved = DateTime.Today.AddDays(-29)
                 }
            };
            this.DataContext = this;
        }
        private void CreateMedicalRegistration_Click(object sender, RoutedEventArgs e)
        {
            var form = new MedicalRegistrationFormWindow();
            form.Owner = Window.GetWindow(this);
            form.ShowDialog();
        }
    }
}