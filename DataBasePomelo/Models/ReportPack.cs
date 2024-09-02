namespace DataBasePomelo.Models;

public partial class ReportPack
{
    public DateTime Id { get; set; }

    public int IdProduct { get; set; }

    public int CountPallet { get; set; }

    public int CountBrickInPallet { get; set; }
}
