using DataBasePomelo.Interface;
using DataBasePomelo.Models.silikat;
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

            DateTime currentTime = new DateTime(2024, 08, 27, 20, 04, 55);

            DateTime start = reportPeriod.Start;
            DateTime end = reportPeriod.End;

            cancellationToken.ThrowIfCancellationRequested();

            // Выполнение запроса в SilikatContext
            List<ReportPress> reportPress = await _dbContext.reportPresses
                .Where(report => report.Id >= start && report.Id <= end)
            .ToListAsync(cancellationToken);

            List<Nomenklatura> nomenklaturas = await _dbContext.Nomenklaturas
                .ToListAsync(cancellationToken);

            var reportResults = (from reportPres in reportPress
                                join nomenklatura in nomenklaturas
                                on reportPres.IdNomenklatura equals nomenklatura.Id
                                group new { reportPres, nomenklatura } by reportPres.Id into reportGroup
                                let firstItem = reportGroup.FirstOrDefault()
                                select new ReportResultDto(
                                    firstItem.reportPres.Id.ToString("dd, MMMM, yyyy"),
                                    "Первый",
                                    reportTime,
                                    firstItem.nomenklatura.Name,
                                    Math.Round(reportGroup.Count() * (double)(firstItem.nomenklatura.Col ?? 0), 2)
                                )).ToList();
            
            if (reportResults != null)
                return (List<ReportResultDto>)reportResults;
            else
                return null;
        }
    }
}
