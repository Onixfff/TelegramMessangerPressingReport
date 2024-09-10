using DataBasePomelo.Controllers;
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
                    #region Ожидание времени

                    //DateTime currentTime = DateTime.Now;
                    DateTime currentTime = new DateTime(2024, 09, 10, 20, 04, 58);

                    TimeSpan? waitingTime = _timeWaiting.GetTimeWaitingRequest(currentTime);

                    if (waitingTime.HasValue && waitingTime.Value > TimeSpan.Zero && waitingTime != null)
                    {
                        if (_logger.IsEnabled(LogLevel.Information))
                        {
                            _logger.LogInformation("\tWaiting for\ndays : {days}\nhour : {Hours}\nminut : {time}\nseconds : {seconds}\n\tuntil next event.", waitingTime.Value.Days ,waitingTime.Value.Hours, waitingTime.Value.Minutes, waitingTime.Value.Seconds);
                        }

                        // Ждем до указанного времени
                        await Task.Delay(waitingTime.Value, stoppingToken);
                    }
                    #endregion

                    #region отправка запроса на взятие данных

                    stoppingToken.ThrowIfCancellationRequested();

                    ReportTime currentReportTime = _timeWaiting.GetTimeReport();

                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation("Fetching report for {reportTime} at {time}", currentReportTime, DateTimeOffset.Now);
                    }

                    var reportDataFirst = await _reportService.GetCunsumptionReportAsync(currentReportTime, DataBasePomelo.Controllers.ReportType.FirstReport, stoppingToken);
                    var reportDataSecond = await _reportService.GetCunsumptionReportAsync(currentReportTime, DataBasePomelo.Controllers.ReportType.SecondReport, stoppingToken);

                    string message = CreateMessage(reportDataFirst);

                    if (!string.IsNullOrEmpty(message))
                    {
                        await _eventAggregator.PublishMessage(message, stoppingToken);
                    }

                    message = CreateMessage(reportDataSecond);

                    if (!string.IsNullOrEmpty(message))
                    {
                        await _eventAggregator.PublishMessage(message, stoppingToken);
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during the report generation.");
                }
            }
        }

        private string CreateMessage(ReportGenerator.ReportResultDto reportResultDto)
        {
            string message = string.Empty;

            _logger.LogInformation($"Report First for Date: {reportResultDto.Date} | Position: {reportResultDto.Position}" +
            $" | ReportTime: {reportResultDto.ReportTime} | NamePress: {reportResultDto.NamePress} | Coll: {reportResultDto.Coll}",
            reportResultDto.Date, reportResultDto.Position);

            message += $"Дата производства : {reportResultDto.Date}\n" +
            $"№Пресса : {reportResultDto.Position}\n" +
            $"Смена : {reportResultDto.ReportTime}\n" +
            $"Рецепт : {reportResultDto.NamePress}\n" +
            $"Количетво кирпича, шт. : {reportResultDto.Coll}\n";

            return message;
        }
    }
}
