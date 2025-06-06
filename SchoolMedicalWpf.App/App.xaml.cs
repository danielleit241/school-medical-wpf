using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SchoolMedicalWpf.App.Parent;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal;
using SchoolMedicalWpf.Dal.Repositories;

namespace SchoolMedicalWpf.App
{
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    var config = context.Configuration;
                    var sqlServerConn = config.GetConnectionString("DBDefault");

                    services.AddDbContext<SchoolmedicalWpfContext>(options =>
                        options.UseSqlServer(sqlServerConn));

                    // Đăng ký DI cho repo, service, window
                    services.AddScoped<UserRepository>();
                    services.AddScoped<UserService>();
                    services.AddTransient<LoginWindow>();

                    services.AddTransient<ParentMainWindow>();
                    services.AddTransient<ParentHomePage>();
                    services.AddTransient<ParentProfilePage>();
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