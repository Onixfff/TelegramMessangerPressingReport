using CSharpFunctionalExtensions;
using DataBasePomelo.Controllers;
using TelegramMessangerPressingReport.Controller;
using static DataBasePomelo.Controllers.ReportGenerator;

namespace DataBasePomelo.Interface
{
    public interface IReportService
    {
        public Task<Result<ReportResultDto>> GetCunsumptionReportAsync(ReportTime reportTime, ReportType reportType, CancellationToken cancellationToken);
    }
}
