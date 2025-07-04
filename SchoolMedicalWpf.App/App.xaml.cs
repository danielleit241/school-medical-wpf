﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SchoolMedicalWpf.App.Parent;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal;
using SchoolMedicalWpf.Dal.Entities;
using SchoolMedicalWpf.Dal.Repositories;
using System.Windows;

namespace SchoolMedicalWpf.App
{
    public partial class App : Application
    {
        private readonly IHost _host;
        public static IServiceProvider Services => ((App)Current)._host.Services;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    var config = context.Configuration;
                    var sqlServerConn = config.GetConnectionString("DBDefault");

                    services.AddDbContext<SchoolmedicalWpfContext>(options =>
                        options.UseSqlServer(sqlServerConn));

                    services.AddScoped<SchoolmedicalWpfContext>();
                    services.AddScoped<PasswordHasher<User>>();
                    services.AddScoped<UserRepository>();
                    services.AddScoped<UserService>();
                    services.AddScoped<RoleRepository>();
                    services.AddScoped<RoleService>();
                    services.AddScoped<StudentService>();
                    services.AddScoped<StudentRepository>();
                    services.AddScoped<HealthProfileRepo>();
                    services.AddScoped<HealthProfileService>();
                    services.AddScoped<MedicalRegistrationRepo>();
                    services.AddScoped<MedicalRegistrationService>();
                    services.AddScoped<HealthCheckResultRepo>();
                    services.AddScoped<HealthCheckResultService>();
                    services.AddScoped<VaccinationResultRepo>();
                    services.AddScoped<VaccinationResultService>();
                    services.AddScoped<MedicalEventRepo>();
                    services.AddScoped<MedicalEventService>();
                    services.AddScoped<HealthCheckScheduleRepo>();
                    services.AddScoped<HealthCheckScheduleService>();
                    services.AddScoped<VaccinationScheduleRepo>();
                    services.AddScoped<VaccinationScheduleService>();
                    services.AddScoped<VaccineDetailRepo>();
                    services.AddScoped<VaccineDetailService>();

                    services.AddTransient<LoginWindow>();

                    services.AddTransient<ParentMainWindow>();
                    services.AddTransient<ParentHomePage>();
                    services.AddTransient<ProfilePage>();
                    services.AddTransient<ParentHealthDeclarationPage>();
                    services.AddTransient<MedicalRegistrationHistoryPage>();
                    services.AddTransient<MedicalRegistrationFormWindow>();
                    services.AddTransient<HealthHistoryPage>();
                    services.AddTransient<HealthResultDetailWindow>();
                    services.AddTransient<VaccinationResultDetailWindow>();
                    services.AddTransient<MedicalEventDetailWindow>();

                    services.AddTransient<Nurse.NurseMainWindow>();
                    services.AddTransient<Nurse.NurseHomePage>();
                    services.AddTransient<Nurse.HealthSchedulePage>();
                    services.AddTransient<Nurse.MedicalEventPage>();
                    services.AddTransient<Nurse.MedicalEventFormWindow>();
                    services.AddTransient<Nurse.MedicalRegistrationPage>();
                    services.AddTransient<Nurse.MedicalRegistrationDetailWindow>();
                    services.AddTransient<Nurse.HealthCheckResultWindow>();
                    services.AddTransient<Nurse.VaccinationResultWindow>();



                    services.AddTransient<Admin.AdminMainWindow>();
                    services.AddTransient<Admin.AdminHomePage>();
                    services.AddTransient<Admin.AccountManagementPage>();
                    services.AddTransient<Admin.CampaignPage>();
                    services.AddTransient<Admin.StudentManagementPage>();
                    services.AddTransient<Admin.CreateHealthCheckWindow>();
                    services.AddTransient<Admin.CreateVaccinationWindow>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            // Lấy LoginWindow từ DI    
            var loginWindow = _host.Services.GetRequiredService<LoginWindow>();
            loginWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            base.OnExit(e);
        }
    }
}