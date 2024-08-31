using TelegramService.Services;

namespace TelegramService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<TelegramBotBackgroundService>();

            var host = builder.Build();
            host.Run();
        }
    }
}