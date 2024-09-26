using SharedLibrary.Interface;

namespace TelegramMessangerPressingReport.Controller
{
    public enum ReportTime
    {
        DayTime,
        NightTime
    }

    public class ReportTimePeriodCalculator : ITimeWaiting
    {
        private static readonly TimeOnly _dayShift = new TimeOnly(8, 5, 0, 0, 0); //Время начала дневной смены
        private static readonly TimeOnly _nightShift = new TimeOnly(20, 5, 0, 0, 0); //Время начала ночной смены
        private ReportTime _reportTime;

        public static (DateTime Start, DateTime End) GetReportPeriod(ReportTime reportTime, DateTime currentTime)
        {
            DateTime today = currentTime.Date;

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
                //Возвращает ночной период: с 20:05 до 8:05
                TimeOnly time = TimeOnly.FromDateTime(currentTime);

                DateTime start;
                DateTime end;

                if (time >= _nightShift.AddMinutes(1))
                {
                    end = endDay;
                    start = startDay.AddDays(1);
                }
                else if (time <= _dayShift.AddMinutes(1))
                {
                    end = endDay.AddDays(-1);
                    start = startDay;
                }
                else
                {
                    start = DateTime.MinValue;
                    end = DateTime.MinValue;
                }

                return (end, start);
            }
        }

        public static string TranslateEnumToLanguage(ReportTime reportTime)
        {
            string translatedValue = null;

            switch (reportTime)
            {
                case ReportTime.DayTime:
                    translatedValue = "день";
                    break;

                case ReportTime.NightTime:
                    translatedValue = "ночь";
                    break;
            }

            return translatedValue;
        }

        public ReportTime GetTimeReport()
        {
            return _reportTime;
        }

        public TimeSpan? GetTimeWaitingRequest(DateTime time)
        {
            TimeSpan? waitingTime = null;

            TimeOnly currentTime = TimeOnly.FromDateTime(time);
            ChangeReportTime(currentTime);

            switch (_reportTime)
            {
                case ReportTime.DayTime:

                    DateTime thisDay = new DateTime(DateOnly.FromDateTime(time), _nightShift);

                    TimeSpan difference = thisDay - time;

                    waitingTime = difference;

                    break;
                case ReportTime.NightTime:

                    DateTime nextDay = DateTime.MinValue;

                    if (currentTime >= _nightShift)
                    {
                        nextDay = new DateTime(DateOnly.FromDateTime(time.AddDays(1)), _dayShift);
                    }
                    if (currentTime <= _dayShift)
                    {
                        nextDay = new DateTime(DateOnly.FromDateTime(time), _dayShift);
                    }


                    TimeSpan differenceNowDayAndNextDay = nextDay - time;

                    waitingTime = differenceNowDayAndNextDay;

                    break;
            }

            return waitingTime;
        }

        private void ChangeReportTime(TimeOnly currentTime)
        {
            if (currentTime >= _dayShift && currentTime < _nightShift)
            {
                _reportTime = ReportTime.DayTime;
            }
            else if (currentTime >= _nightShift)
            {
                _reportTime = ReportTime.NightTime;
            }
            else if (currentTime <= _dayShift)
            {
                _reportTime = ReportTime.NightTime;
            }
        }
    }
}
