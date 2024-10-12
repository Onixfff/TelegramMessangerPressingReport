using CSharpFunctionalExtensions;
using DataBasePomelo.Controllers;
using DataBasePomelo.Interface;
using SharedLibrary.Interface;
using TelegramMessangerPressingReport.Controller;
using TelegramService.Services;
using static DataBasePomelo.Controllers.ReportGenerator;

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
                    DateTime currentTime = DateTime.Now;

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
                    string message;

                    ReportTime currentReportTime = _timeWaiting.GetTimeReport();

                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation("Fetching report for {reportTime} at {time}", currentReportTime, DateTimeOffset.Now);
                    }

                    Result<ReportResultDto> resultReportDataFirst = await _reportService.GetCunsumptionReportAsync(currentReportTime, ReportType.FirstReport, stoppingToken);

                    if (resultReportDataFirst.IsFailure)
                    {
                        _logger.LogError($"Message is Null Or Empty \nmessage - {resultReportDataFirst.Error}");
                        message = $"Message is Null Or Empty \nmessage - {resultReportDataFirst.Error}";
                    }
                    else
                    {
                        message = CreateMessage(resultReportDataFirst.Value);
                    }

                    if (!string.IsNullOrEmpty(message))
                    {
                        await _eventAggregator.PublishMessage(message, stoppingToken);
                    }
                    else
                    {
                        _logger.LogWarning("Message is Null Or Empty \nmessage : {message}", message);
                    }

                    Result<ReportResultDto> resultReportDataSecond = await _reportService.GetCunsumptionReportAsync(currentReportTime, ReportType.SecondReport, stoppingToken);

                    if (resultReportDataSecond.IsFailure)
                    {
                        _logger.LogError($"Message is Null Or Empty \nmessage - {resultReportDataSecond.Error}");
                        message = ($"Message is Null Or Empty \nmessage - {resultReportDataSecond.Error}");
                    }
                    else
                    {
                        message = CreateMessage(resultReportDataSecond.Value);
                    }

                    if (!string.IsNullOrEmpty(message))
                    {
                        await _eventAggregator.PublishMessage(message, stoppingToken);
                    }
                    else
                    {
                        _logger.LogWarning("Message is Null Or Empty \nmessage : {message}", message);
                    }

                    #endregion
                }
                catch (OperationCanceledException ex)
                {
                    _logger.LogWarning("Operation was canceled: {message}", ex.Message);
                    // Прекращение цикла, если отменено
                    break;
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

            if (!string.IsNullOrEmpty(reportResultDto.NamePress))
            {
                if (!string.IsNullOrEmpty(reportResultDto.Date))
                {
                    _logger.LogInformation($"Report First for Date: {reportResultDto.Date} | Position: {reportResultDto.Position}" +
                                            $" | ReportTime: {reportResultDto.ReportTime} | NamePress: {reportResultDto.NamePress} | Coll: {reportResultDto.Coll}",
                                            reportResultDto.Date, reportResultDto.Position);

                    message += $"Дата производства : {reportResultDto.Date}\n" +
                                $"№Пресса : {reportResultDto.Position}\n" +
                                $"Смена : {reportResultDto.ReportTime}\n" +
                                $"Рецепт : {reportResultDto.NamePress}\n" +
                                $"Количетво кирпича, шт. : {reportResultDto.Coll}\n";
                }
            }
            return message;
        }
    }
}
