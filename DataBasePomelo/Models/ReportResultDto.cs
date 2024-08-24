namespace DataBasePomelo.Controllers
{
    public partial class ReportGenerator
    {
        public class ReportResultDto
        {
            public string Date { get; set; }
            public string Press { get; set; }
            public string Shift { get; set; }
            public string RecipeName { get; set; }
            public string LimeBrand { get; set; }
            public double LimeConsumption { get; set; }
            public string Sand1Name { get; set; }
            public double Sand1Consumption { get; set; }
            public string Sand2Name { get; set; }
            public double Sand2Consumption { get; set; }
        }
    }
}
