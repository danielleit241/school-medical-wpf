using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.App.Parent
{
    public partial class MedicalRegistrationHistoryPage : UserControl
    {
        private readonly MedicalRegistrationService _registrationService;
        private readonly User _currentUser;

        public ObservableCollection<MedicalRegistration> MedicalRegistrationList { get; set; } = [];

        public MedicalRegistrationHistoryPage(MedicalRegistrationService registrationService, User currentUser)
        {
            InitializeComponent();
            _registrationService = registrationService;
            _currentUser = currentUser;
        }

        private void CreateMedicalRegistration_Click(object sender, RoutedEventArgs e)
        {
            var form = ActivatorUtilities.CreateInstance<MedicalRegistrationFormWindow>(App.Services, _currentUser);
            form.Owner = Window.GetWindow(this);
            form.ShowDialog();
            FillData();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            var registrationsHistory = _registrationService.GetAllRegistrations();
            var userRegistrations = registrationsHistory
                .Where(r => r.UserId == _currentUser.UserId)
                .ToList();

            MedicalRegistrationList.Clear();
            foreach (var registration in userRegistrations)
            {
                MedicalRegistrationList.Add(registration);
            }
            this.DataContext = this;
        }
    }
}