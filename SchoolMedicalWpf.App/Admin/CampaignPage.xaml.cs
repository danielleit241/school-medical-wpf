using Microsoft.Extensions.DependencyInjection;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal.Entities;
using SchoolMedicalWpf.Dal.Repositories;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace SchoolMedicalWpf.App.Admin
{
    public partial class CampaignPage : UserControl
    {
        public ObservableCollection<HealthCheckSchedule> HealthCheckSchedules { get; set; }
        public ObservableCollection<VaccinationSchedule> VaccinationSchedules { get; set; }

        private readonly HealthCheckScheduleService _healthCheckSchedule;
        private readonly VaccinationScheduleService _vaccinationSchedule;
        private readonly User _currentUser;

        public CampaignPage(HealthCheckScheduleService healthCheckSchedule, VaccinationScheduleService vaccinationSchedule, User currentUser)
        {
            InitializeComponent();
            HealthCheckSchedules = new ObservableCollection<HealthCheckSchedule>();
            VaccinationSchedules = new ObservableCollection<VaccinationSchedule>();

            _healthCheckSchedule = healthCheckSchedule;
            _vaccinationSchedule = vaccinationSchedule;
            _currentUser = currentUser;

            DataContext = this;

            Loaded += CampaignPage_Loaded;
        }

        private void CampaignPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            LoadHealthCheckSchedules();
            LoadVaccinationSchedules();
        }

        private void LoadHealthCheckSchedules()
        {
            HealthCheckSchedules.Clear();
            var healthCheckSchedules = _healthCheckSchedule.GetAllHealthCheckSchedules();
            healthCheckSchedules.ToList().ForEach(r =>
            {
                HealthCheckSchedules.Add(r);
            });
        }

        private void LoadVaccinationSchedules()
        {
            VaccinationSchedules.Clear();
            var vaccinations = _vaccinationSchedule.GetAllVaccinationSchedules();
            vaccinations.ToList().ForEach(r => VaccinationSchedules.Add(r));
        }

        #region Health Check Events

        private void btnCreateHealthCheck_Click(object sender, RoutedEventArgs e)
        {
            var createWindow = ActivatorUtilities.CreateInstance<CreateHealthCheckWindow>(App.Services);
            if (createWindow.ShowDialog() == true)
            {
                LoadHealthCheckSchedules();
            }
        }

        private void btnRefreshHealthCheck_Click(object sender, RoutedEventArgs e)
        {
            LoadHealthCheckSchedules();
        }

        private void btnEditHealthCheck_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is HealthCheckSchedule schedule)
            {
                var editWindow = ActivatorUtilities.CreateInstance<CreateHealthCheckWindow>(App.Services, schedule);
                if (editWindow.ShowDialog() == true)
                {
                    LoadHealthCheckSchedules();
                }
            }
        }

        private void btnDeleteHealthCheck_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is HealthCheckSchedule schedule)
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa lịch khám '{schedule.Title}'?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    HealthCheckSchedules.Remove(schedule);
                    _healthCheckSchedule.Delete(schedule.ScheduleId);
                    MessageBox.Show("Đã xóa lịch khám thành công!", "Thông báo");
                }
            }
        }

        #endregion

        #region Vaccination Events

        private void btnCreateVaccination_Click(object sender, RoutedEventArgs e)
        {
            var createWindow = ActivatorUtilities.CreateInstance<CreateVaccinationWindow>(App.Services);
            if (createWindow.ShowDialog() == true)
            {
                LoadVaccinationSchedules();
            }
        }

        private void btnRefreshVaccination_Click(object sender, RoutedEventArgs e)
        {
            LoadVaccinationSchedules();
        }

        private void btnEditVaccination_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is VaccinationSchedule schedule)
            {
                var editWindow = ActivatorUtilities.CreateInstance<CreateVaccinationWindow>(App.Services, schedule);
                if (editWindow.ShowDialog() == true)
                {
                    LoadVaccinationSchedules();
                }
            }
        }

        private void btnDeleteVaccination_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is VaccinationSchedule schedule)
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa lịch tiêm '{schedule.Title}'?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    VaccinationSchedules.Remove(schedule);
                    _vaccinationSchedule.DeleteVaccinationSchedule(schedule.ScheduleId);
                    MessageBox.Show("Đã xóa lịch tiêm thành công!", "Thông báo");
                }
            }
        }

        #endregion
    }
}