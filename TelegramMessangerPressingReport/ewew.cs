using DataBasePomelo.Interface;
using TelegramMessangerPressingReport.Controller;

namespace TelegramMessangerPressingReport
{
    public class ewew
    {
        private readonly IReportService _reportService;

        public ewew(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task GenerateReportAsync(ReportTime reportTime, CancellationToken cancellationToken)
        {
            // Генерация отчета с использованием периода времени
            var reportData = await _reportService.GetCunsumptionReportAsync(reportTime, cancellationToken);

            // Обработка данных отчета
            // Например: вывод в консоль
            Console.WriteLine($"Report generated with {reportData.Count} records.");
        }
    }
}