using TelegramMessangerPressingReport.Controller;

namespace SharedLibrary.Interface
{
    public interface ITimeWaiting
    {
        public TimeSpan? GetTimeWaitingRequest(DateTime date);
        public ReportTime GetTimeReport();
    }
}
