using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DataBasePomelo.Interface;
using DataBasePomelo.Controllers;
using DataBasePomelo;
using TelegramMessangerPressingReport.Controller;

namespace TelegramMessangerPressingReport
{
    internal class Program
    {
        static async Task Main(string[] args)
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
                    var connectionString = configuration.GetConnectionString("DataBasePomelo"); // Обновлено для соответствия конфигурации

                    // Регистрация DbContext
                    services.AddDbContext<SilicatDbContext>(options =>
                        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

                    // Регистрация других сервисов
                    services.AddScoped<IReportService, ReportGenerator>();
                    services.AddScoped<ewew>();

                })
                .Build();

            // Создание сервиса
            var serviceProvider = host.Services;
            var ewew = serviceProvider.GetRequiredService<ewew>();

            // Выбор типа отчета
            var reportTimeType = ReportTime.DayTime; // Или ReportTime.NightTime в зависимости от того, что вам нужно

            // Получение периода времени на основе типа отчета
            var (start, end) = ReportTimePeriodCalculator.GetReportPeriod(reportTimeType);

            // Вызов метода GenerateReportAsync
            await ewew.GenerateReportAsync(reportTimeType, CancellationToken.None);

            Console.WriteLine("Report generated.");
        }
    }
}
