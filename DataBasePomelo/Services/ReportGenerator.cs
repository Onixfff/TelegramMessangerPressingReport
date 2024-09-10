using DataBasePomelo.Interface;
using DataBasePomelo.Models.silikat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TelegramMessangerPressingReport.Controller;

namespace DataBasePomelo.Controllers
{
    public enum ReportType
    {
        FirstReport,
        SecondReport
    }

    public partial class ReportGenerator : IReportService
    {
        private readonly SilicatDbContext _dbContext;
        private ILogger<ReportGenerator> _logger;

        public ReportGenerator(SilicatDbContext dbContext, ILogger<ReportGenerator> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task <ReportResultDto> GetCunsumptionReportAsync(ReportTime reportTime, ReportType reportType, CancellationToken cancellationToken)
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

            List<Nomenklatura> nomenklaturas = await _dbContext.Nomenklaturas
                .ToListAsync(cancellationToken);

            ReportResultDto reportResults = new ReportResultDto(null, null, null, null, double.NegativeZero);

            switch (reportType)
            {
                case ReportType.FirstReport:
                    List<ReportPress> reportPress1 = await _dbContext.reportPresses
                    .Where(report => report.Id >= start && report.Id <= end)
                    .ToListAsync(cancellationToken);

                    var results1 = (from reportPres in reportPress1
                                    join nomenklatura in nomenklaturas
                                    on reportPres.IdNomenklatura equals nomenklatura.Id
                                    group new { reportPres, nomenklatura } by reportPres.Id into reportGroup
                                    let firstItem = reportGroup.FirstOrDefault()
                                    select new
                                    {
                                        FirstItem = firstItem,
                                    });

                    var totalSum1 = (from reportPres in reportPress1
                                     join nomenklatura in nomenklaturas
                                     on reportPres.IdNomenklatura equals nomenklatura.Id
                                     select nomenklatura.Col).Sum();

                    if (results1 != null && totalSum1 != null)
                    {
                        string FirstOrSecond = "Первый";

                        reportResults = new ReportResultDto(
                            results1.FirstOrDefault().FirstItem.reportPres.Id.ToString("dd, MMMM, yyyy"),
                            FirstOrSecond,
                            ReportTimePeriodCalculator.TranslateEnumToLanguage(reportTime),
                            results1.FirstOrDefault().FirstItem.nomenklatura.Name,
                            Math.Round((double)totalSum1, 2)
                        );
                    }

                    break;
                case ReportType.SecondReport:
                    List<ReportPress2> reportPress2 = await _dbContext.reportPresses2
                    .Where(report => report.Id >= start && report.Id <= end)
                    .ToListAsync(cancellationToken);

                    var results2 = (from reportPres in reportPress2
                                    join nomenklatura in nomenklaturas
                                    on reportPres.IdNomenklatura equals nomenklatura.Id
                                    group new { reportPres, nomenklatura } by reportPres.Id into reportGroup
                                    let firstItem = reportGroup.FirstOrDefault()
                                    select new
                                    {
                                        FirstItem = firstItem,
                                    });

                    var totalSum2 = (from reportPres in reportPress2
                                     join nomenklatura in nomenklaturas
                                     on reportPres.IdNomenklatura equals nomenklatura.Id
                                     select nomenklatura.Col).Sum();

                    if (results2 != null && totalSum2 != null)
                    {
                        string FirstOrSecond = "Второй";

                        reportResults = new ReportResultDto(
                            results2.FirstOrDefault().FirstItem.reportPres.Id.ToString("dd, MMMM, yyyy"),
                            FirstOrSecond,
                            ReportTimePeriodCalculator.TranslateEnumToLanguage(reportTime),
                            results2.FirstOrDefault().FirstItem.nomenklatura.Name,
                            Math.Round((double)totalSum2, 2)
                        );
                    }

                    break;
                default:
                    cancellationToken.ThrowIfCancellationRequested();
                    throw new InvalidOperationException("Invalid report type specified.");
            }

            return reportResults;
        }
    }
}
