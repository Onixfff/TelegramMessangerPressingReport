using DataBasePomelo.Interface;
using Microsoft.EntityFrameworkCore;
using TelegramMessangerPressingReport.Controller;

namespace DataBasePomelo.Controllers
{
    public partial class ReportGenerator : IReportService
    {
        private readonly SilicatDbContext _dbContext;

        public ReportGenerator(SilicatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ReportResultDto>> GetCunsumptionReportAsync(ReportTime reportTime, CancellationToken cancellationToken)
        {
            var reportPeriod = ReportTimePeriodCalculator.GetReportPeriod(reportTime);

            if (reportPeriod.Start == DateTime.MinValue || reportPeriod.End == DateTime.MinValue)
            {
                cancellationToken.ThrowIfCancellationRequested();
                throw new InvalidOperationException("Report period contains invalid dates.");
            }

            DateTime start = reportPeriod.Start;
            DateTime end = reportPeriod.End;

            cancellationToken.ThrowIfCancellationRequested();

            // Выполнение запроса в SilikatContext
            var reports = await _dbContext.Reports
                .Where(report => report.Id >= start && report.Id <= end)
            .ToListAsync(cancellationToken);

            var recepts = await _dbContext.Recepts
                .ToListAsync(cancellationToken);

            // Выполнение запроса в MaterialCostumerManufacturContext
            var materials = await _dbContext.Materials
                .ToListAsync(cancellationToken);

            // Объединение данных в памяти
            var reportResults = from report in reports
                                join brand in materials on report.IdNameLime equals brand.Id
                                join sand1 in materials on report.IdnameSand1 equals sand1.Id
                                join sand2 in materials on report.IdnameSand2 equals sand2.Id
                                into reportGroup
                                select new ReportResultDto
                                {
                                    Date = reportGroup.
                                    Press = "Первый",
                                    Shift = reportGroup.First().report.Id.TimeOfDay >= TimeSpan.FromHours(8) && reportGroup.First().report.Id.TimeOfDay <= TimeSpan.FromHours(20) ? "день" : "ночь",
                                    RecipeName = reportGroup.First().recept.Name,
                                    LimeBrand = reportGroup.First().lime != null ? reportGroup.First().lime.Name : "Не указано",
                                    LimeConsumption = Math.Round(reportGroup.Sum(x => x.report.ActualLime1), 2),
                                    Sand1Name = reportGroup.First().sand1 != null ? reportGroup.First().sand1.Name : "Не указано",
                                    Sand1Consumption = Math.Round(reportGroup.Sum(x => x.report.ActualSand1), 2),
                                    Sand2Name = reportGroup.First().sand2 != null ? reportGroup.First().sand2.Name : "Не указано",
                                    Sand2Consumption = Math.Round(reportGroup.Sum(x => x.report.ActualSand2), 2)
                                };

            return reportResults.ToList();
        }
    }
}
