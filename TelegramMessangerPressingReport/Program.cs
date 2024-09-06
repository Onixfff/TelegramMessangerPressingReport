using DataBasePomelo;
using DataBasePomelo.Controllers;
using DataBasePomelo.Interface;
using DataBasePomelo.Models.Context;
using EndShiftService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedLibrary.Interface;
using TelegramMessangerPressingReport.Controller;
using TelegramService.Options;
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
                          .AddJsonFile("appsettings.json")
                          .AddUserSecrets<Program>(optional: true);
                })

                .ConfigureServices((context, services) =>
                {
                    var configuration = context.Configuration;

                    var connectionString = configuration.GetConnectionString(nameof(DataBasePomelo));
                    var userIds = configuration.GetSection("Peoples").Get<List<long>>();

                    if (userIds == null)
                    {
                        throw new InvalidOperationException("Configuration section 'Peoples' is missing or invalid.");
                    }

                    services.AddSingleton(userIds);
                    services.AddSingleton<EventAggregator>();

                    services.AddDbContext<SilikatContext>(options =>
                    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

                    services.AddScoped<IReportService, ReportGenerator>();
                    services.AddScoped<ITimeWaiting, ReportTimePeriodCalculator>();

                    services.Configure<TelegramOptions>(configuration.GetSection(TelegramOptions.Telegram));

                    services.AddScoped<SilicatDbContext>(provider =>
                    {
                        var silikatContext = provider.GetRequiredService<SilikatContext>();
                        return new SilicatDbContext(silikatContext);
                    });

                    // Регистрация background сервисов
                    services.AddHostedService<BackgroundTimerServices>();
                    services.AddHostedService<TelegramBotBackgroundService>();

                })
                .Build();

            host.Run();
        }
    }
}
