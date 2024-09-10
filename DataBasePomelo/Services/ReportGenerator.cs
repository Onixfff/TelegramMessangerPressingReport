using DataBasePomelo.Interface;
using DataBasePomelo.Models.silikat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TelegramMessangerPressingReport.Controller;

namespace DataBasePomelo.Controllers
{
    public partial class ReportGenerator : IReportService
    {
        private readonly SilicatDbContext _dbContext;
        private ILogger<ReportGenerator> _logger;

        public ReportGenerator(SilicatDbContext dbContext, ILogger<ReportGenerator> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ReportResultDto> GetCunsumptionReportAsync(ReportTime reportTime, CancellationToken cancellationToken)
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
            List<ReportPress> reportPress = await _dbContext.reportPresses
                .Where(report => report.Id >= start && report.Id <= end)
            .ToListAsync(cancellationToken);

            List<Nomenklatura> nomenklaturas = await _dbContext.Nomenklaturas
                .ToListAsync(cancellationToken);

            ReportResultDto reportResults = null;

            var results = (from reportPres in reportPress
                           join nomenklatura in nomenklaturas
                           on reportPres.IdNomenklatura equals nomenklatura.Id
                           group new { reportPres, nomenklatura } by reportPres.Id into reportGroup
                           let firstItem = reportGroup.FirstOrDefault()
                           select new
                           {
                               FirstItem = firstItem,
                           });

            var totalSum = (from reportPres in reportPress
                            join nomenklatura in nomenklaturas
                            on reportPres.IdNomenklatura equals nomenklatura.Id
                            select nomenklatura.Col).Sum();

            if (results != null && totalSum != null)
            {
                string FirstOrSecond = null;

                switch (reportTime)
                {
                    case ReportTime.DayTime:
                        FirstOrSecond = "Первый";
                        break;
                    case ReportTime.NightTime:
                        FirstOrSecond = "Второй";
                        break;
                }

                reportResults = new ReportResultDto(
                    results.FirstOrDefault().FirstItem.reportPres.Id.ToString("dd, MMMM, yyyy"),
                    FirstOrSecond,
                    ReportTimePeriodCalculator.TranslateEnumToLanguage(reportTime),
                    results.FirstOrDefault().FirstItem.nomenklatura.Name,
                    Math.Round((double)totalSum, 2)
);
            }

            return reportResults;
        }
    }
}
