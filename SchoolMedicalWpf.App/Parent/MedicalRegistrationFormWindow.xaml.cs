using System.Windows;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Parent
{
    /// <summary>
    /// Interaction logic for MedicalRegistrationFormWindow.xaml
    /// </summary>
    public partial class MedicalRegistrationFormWindow : Window
    {
        private readonly StudentService _studentService;
        private readonly User _currentUser;
        private readonly MedicalRegistrationService _medicalRegistrationService;

        public MedicalRegistrationFormWindow(StudentService studentService, User currentUser, MedicalRegistrationService medicalRegistrationService)
        {
            InitializeComponent();
            _studentService = studentService;
            _currentUser = currentUser;
            _medicalRegistrationService = medicalRegistrationService;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var medicalRegistration = new MedicalRegistration
            {
                RegistrationId = Guid.NewGuid(),
                StudentId = (Guid?)StudentComboBox.SelectedValue,
                UserId = _currentUser.UserId,
                DateSubmitted = DateOnly.FromDateTime(DateSubmittedPicker.SelectedDate!.Value),
                MedicationName = MedicationNameTextBox.Text,
                TotalDosages = TotalDosagesTextBox.Text,
                Notes = NotesTextBox.Text,
                ParentalConsent = ParentalConsentCheckBox.IsChecked
            };

            _medicalRegistrationService.AddRegistration(medicalRegistration);
            MessageBox.Show("Medical registration submitted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FillComboBox();
        }

        private async void FillComboBox()
        {
            StudentComboBox.ItemsSource = null;
            StudentComboBox.ItemsSource = await _studentService.GetStudentsByUserId(_currentUser.UserId);

            StudentComboBox.DisplayMemberPath = "FullName";
            StudentComboBox.SelectedValuePath = "StudentId";
        }
    }
}
