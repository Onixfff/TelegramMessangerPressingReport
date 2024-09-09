using TelegramMessangerPressingReport.Controller;

namespace DataBasePomelo.Controllers
{
    public partial class ReportGenerator
    {
        public class ReportResultDto
        {
            public ReportResultDto(string date, string position, string reportTime, string namePress, double coll)
            {
                Date = date;
                Position = position;
                ReportTime = reportTime;
                NamePress = namePress;
                Coll = coll;
            }

            public string Date { get; set; }
            public string Position { get; set; }
            public string ReportTime { get; set; }
            public string NamePress { get; set; }
            public double Coll { get; set; }
        }
    }
}
