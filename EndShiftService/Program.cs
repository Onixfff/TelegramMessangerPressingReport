using EndShiftService.Services;

namespace EndShiftService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<BackgroundTimerServices>();

            var host = builder.Build();
            host.Run();
        }
    }
}