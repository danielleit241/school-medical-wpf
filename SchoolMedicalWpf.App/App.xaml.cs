using System.Windows;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SchoolMedicalWpf.App.Parent;
using SchoolMedicalWpf.Bll.Services;
using SchoolMedicalWpf.Dal;
using SchoolMedicalWpf.Dal.Entities;
using SchoolMedicalWpf.Dal.Repositories;

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

                    services.AddDbContextFactory<SchoolmedicalWpfContext>(options =>
                        options.UseSqlServer(sqlServerConn));


                    // Đăng ký PasswordHasher<User> vào DI
                    services.AddScoped<PasswordHasher<User>>();

                    // Đăng ký DI cho repo, service, window
                    services.AddScoped<UserRepository>();
                    services.AddScoped<UserService>();
                    services.AddScoped<RoleRepository>();

                    services.AddTransient<LoginWindow>();
                    //services.AddTransient<ParentMainWindow>();
                    //services.AddTransient<ParentHomePage>();
                    //services.AddTransient<ParentProfilePage>();
                    //services.AddTransient<ParentHealthDeclarationPage>();
                    //services.AddTransient<HealthDeclarationForm>();
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