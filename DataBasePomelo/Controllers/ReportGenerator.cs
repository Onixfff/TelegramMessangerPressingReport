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


            var result = await (
                from report in _dbContext.Reports
                join recept in _dbContext.Recepts on report.IdRecept equals recept.Id
                join materialLime in _dbContext.Material on report.IdNameLime equals materialLime.Id into materialLimeGroup
                from lime in materialLimeGroup.DefaultIfEmpty()
                join materialSand1 in _dbContext.Material on report.IdnameSand1 equals materialSand1.Id into materialSand1Group
                from sand1 in materialSand1Group.DefaultIfEmpty()
                join materialSand2 in _dbContext.Material on report.IdnameSand2 equals materialSand2.Id into materialSand2Group
                from sand2 in materialSand2Group.DefaultIfEmpty()
                where report.Id >= start && report.Id <= end
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
                }
            ).ToListAsync(cancellationToken);

            return result;
        }
    }
}
