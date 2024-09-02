using DataBasePomelo.Interface;
using DataBasePomelo.Models.Context;
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

            var manufactures = await _dbContext.Manufacturers
                .ToListAsync(cancellationToken);

            var groupMaterials = await _dbContext.GroupMaterials
                .ToListAsync(cancellationToken);

            // Объединение данных в памяти
            var reportResults = from report in reports
                                join recept in recepts on report.IdRecept equals recept.Id
                                join lime in materials on report.IdNameLime equals lime.Id into materialLimeGroup
                                from lime in materialLimeGroup.DefaultIfEmpty()
                                join sand1 in groupMaterials on report.IdnameSand1 equals sand1.Id into materialSand1Group
                                from sand1 in materialSand1Group.DefaultIfEmpty()
                                join sand2 in manufactures on report.IdnameSand2 equals sand2.Id into materialSand2Group
                                from sand2 in materialSand2Group.DefaultIfEmpty()
                                group new { report, recept, lime, sand1, sand2 } by new
                                {
                                    Date = report.Id.TimeOfDay < TimeSpan.FromHours(8)
                                            ? report.Id.AddDays(-1).ToString("dd MMMM yyyy")
                                            : report.Id.ToString("dd MMMM yyyy")
                                }
                                into reportGroup
                                select new ReportResultDto
                                {
                                    Date = reportGroup.Key.Date,
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
