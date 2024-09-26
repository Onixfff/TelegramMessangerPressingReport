using DataBasePomelo.Controllers;
using TelegramMessangerPressingReport.Controller;
using static DataBasePomelo.Controllers.ReportGenerator;

namespace DataBasePomelo.Interface
{
    public interface IReportService
    {
        public Task<ReportResultDto> GetCunsumptionReportAsync(DateTime currentTime, ReportTime reportTime, ReportType reportType, CancellationToken cancellationToken);
    }
}
