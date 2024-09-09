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

            List<ReportResultDto>? reportResults = null;

            switch (reportTime)
            {
                case ReportTime.DayTime:

                    reportResults = (from reportPres in reportPress
                                     join nomenklatura in nomenklaturas
                                     on reportPres.IdNomenklatura equals nomenklatura.Id
                                     group new { reportPres, nomenklatura } by reportPres.Id into reportGroup
                                     let firstItem = reportGroup.FirstOrDefault()
                                     select new ReportResultDto(
                                         firstItem.reportPres.Id.ToString("dd, MMMM, yyyy"),
                                         "Первый",
                                         ReportTimePeriodCalculator.TranslateEnumToLanguage(reportTime),
                                         firstItem.nomenklatura.Name,
                                         Math.Round(reportGroup.Count() * (double)(firstItem.nomenklatura.Col ?? 0), 2)
                                     )).ToList();
                    break;
                case ReportTime.NightTime:
                    reportResults = (from reportPres in reportPress
                                         join nomenklatura in nomenklaturas
                                         on reportPres.IdNomenklatura equals nomenklatura.Id
                                         group new { reportPres, nomenklatura } by reportPres.Id into reportGroup
                                         let firstItem = reportGroup.FirstOrDefault()
                                         select new ReportResultDto(
                                             firstItem.reportPres.Id.ToString("dd, MMMM, yyyy"),
                                             "Второй",
                                             ReportTimePeriodCalculator.TranslateEnumToLanguage(reportTime),
                                             firstItem.nomenklatura.Name,
                                             Math.Round(reportGroup.Count() * (double)(firstItem.nomenklatura.Col ?? 0), 2)
                                         )).ToList();
                    break;
            }

            double sumColl = 0;

            foreach (var item in reportResults)
            {
                sumColl += item.Coll;    
            }

            ReportResultDto result = new ReportResultDto(null, null, null, null, double.NegativeZero); ;

            for(int i = 0; i <= reportResults.Count; i++)
            {
                if (i == 0)
                    result = new ReportResultDto(reportResults[i].Date, reportResults[i].Position, reportResults[i].ReportTime, reportResults[i].NamePress, sumColl);
                else
                    break;
            } 

            if (reportResults != null)
                return (ReportResultDto) result;
            else
                return null;
        }
    }
}
