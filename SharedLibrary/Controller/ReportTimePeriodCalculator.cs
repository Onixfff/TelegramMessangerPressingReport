namespace TelegramMessangerPressingReport.Controller
{
    public enum ReportTime
    {
        DayTime,
        NightTime
    }

    public class ReportTimePeriodCalculator
    {
        public static (DateTime Start, DateTime End) GetReportPeriod(ReportTime reportTime)
        {
            DateTime now = DateTime.Now;
            DateTime today = now.Date;

            // Время начала дневного отчета - 8:05:00
            DateTime startDay = today.AddHours(8).AddMinutes(5);

            // Время окончания дневного отчета - 20:05:00
            DateTime endDay = today.AddHours(20).AddMinutes(5);

            if (reportTime == ReportTime.DayTime)
            {
                // Возвращаем дневной период: с 8:05 до 20:05
                return (startDay, endDay);
            }
            else
            {
                // Время окончания ночного отчета - 8:05 следующего дня
                DateTime nextDay = startDay.AddDays(1);

                // Возвращаем ночной период: с 20:05 до 8:05 следующего дня
                return (endDay, nextDay);
            }
        }
    }
}
