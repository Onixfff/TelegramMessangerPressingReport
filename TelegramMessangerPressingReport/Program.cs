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
using DataBasePomelo.Models.Context;

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
                    // Регистрация DbContext
                    services.AddDbContext<MaterialCostumerManufacturContext>();
                    services.AddDbContext<SilikatContext>();
                    
                    services.AddScoped<SilicatDbContext>(provider =>
                    {
                        var materialContext = provider.GetRequiredService<MaterialCostumerManufacturContext>();
                        var silikatContext = provider.GetRequiredService<SilikatContext>();
                        return new SilicatDbContext(materialContext, silikatContext);
                    });

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
