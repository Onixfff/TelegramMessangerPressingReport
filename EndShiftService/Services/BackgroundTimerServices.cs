using DataBasePomelo.Interface;
using SharedLibrary.Interface;
using TelegramMessangerPressingReport.Controller;
using TelegramService.Services;

namespace EndShiftService.Services
{
    public class BackgroundTimerServices : BackgroundService
    {
        private readonly DateTime StartTime;
        private readonly ITimeWaiting _timeWaiting;
        private readonly IReportService _reportService;
        private readonly ILogger<BackgroundTimerServices> _logger;
        private readonly EventAggregator _eventAggregator;

        public BackgroundTimerServices(IReportService reportService, ITimeWaiting timeWaiting, EventAggregator eventAggregator, ILogger<BackgroundTimerServices> logger)
        {
            _logger = logger;
            _reportService = reportService;
            StartTime = DateTime.Now;
            _timeWaiting = timeWaiting;
            _eventAggregator = eventAggregator;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    #region �������� �������

                    DateTime currentTime = DateTime.Now;

                    TimeSpan? waitingTime = _timeWaiting.GetTimeWaitingRequest(currentTime);

                    if (waitingTime.HasValue && waitingTime.Value > TimeSpan.Zero && waitingTime != null)
                    {
                        if (_logger.IsEnabled(LogLevel.Information))
                        {
                            _logger.LogInformation("\tWaiting for\ndays : {days}\nhour : {Hours}\nminut : {time}\nseconds : {seconds}\n\tuntil next event.", waitingTime.Value.Days ,waitingTime.Value.Hours, waitingTime.Value.Minutes, waitingTime.Value.Seconds);
                        }

                        // ���� �� ���������� �������
                        await Task.Delay(waitingTime.Value, stoppingToken);
                    }
                    #endregion

                    #region �������� ������� �� ������ ������

                    stoppingToken.ThrowIfCancellationRequested();

                    ReportTime currentReportTime = _timeWaiting.GetTimeReport();

                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation("Fetching report for {reportTime} at {time}", currentReportTime, DateTimeOffset.Now);
                    }

                    var reportData = await _reportService.GetCunsumptionReportAsync(currentReportTime, stoppingToken);
                    string? lastMessage = null;

                    _logger.LogInformation($"Report for Date: {reportData.Date} | Position: {reportData.Position}" +
                        $" | ReportTime: {reportData.ReportTime} | NamePress: {reportData.NamePress} | Coll: {reportData.Coll}",
                        reportData.Date, reportData.Position);

                    lastMessage = $"���� ������������ : {reportData.Date}\n" +
                        $"������� : {reportData.Position}\n" +
                        $"����� : {reportData.ReportTime}\n" +
                        $"������ : {reportData.NamePress}\n" +
                        $"��������� �������, ��. : {reportData.Coll}";

                    if (!string.IsNullOrEmpty(lastMessage))
                    {
                        await _eventAggregator.PublishMessage(lastMessage, stoppingToken);

                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during the report generation.");
                }
            }
        }
    }
}
