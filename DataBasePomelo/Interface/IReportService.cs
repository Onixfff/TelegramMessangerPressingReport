using TelegramMessangerPressingReport.Controller;
using static DataBasePomelo.Controllers.ReportGenerator;

namespace DataBasePomelo.Interface
{
    public interface IReportService
    {
        public Task<ReportResultDto> GetCunsumptionReportAsync(ReportTime reportTime, CancellationToken cancellationToken);
    }
}
