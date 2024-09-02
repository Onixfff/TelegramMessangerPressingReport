using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DataBasePomelo.Interface;
using DataBasePomelo.Controllers;
using DataBasePomelo;
using EndShiftService.Services;
using SharedLibrary.Interface;
using TelegramMessangerPressingReport.Controller;
using TelegramService.Services;

namespace TelegramMessangerPressingReport
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(AppContext.BaseDirectory)
                          .AddJsonFile("appsettings.json");
                })

                .ConfigureServices((context, services) =>
                {
                    var configuration = context.Configuration;

                    // Получаем строку подключения из конфигурации
                    var connectionString = configuration.GetConnectionString(nameof(DataBasePomelo));

                    // Регистрация DbContext
                    services.AddDbContext<SilicatDbContext>(options =>
                        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

                    // Регистрация других сервисов
                    services.AddScoped<IReportService, ReportGenerator>();
                    services.AddScoped<ITimeWaiting, ReportTimePeriodCalculator>();
                    services.AddScoped<EventAggregator>();

                    // Регистрация background сервисов
                    services.AddHostedService<BackgroundTimerServices>();
                })
                .Build();

            host.Run();
        }
    }
}
