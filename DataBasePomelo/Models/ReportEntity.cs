namespace DataBasePomelo.Models;

public partial class ReportEntity
{
    public DateTime Id { get; set; }

    public int IdRecept { get; set; }

    public int IdNameLime { get; set; }

    public int IdnameSand1 { get; set; }

    public int IdnameSand2 { get; set; }

    public float TargetLime { get; set; }

    public float TargetSand1 { get; set; }

    public float TargetSand2 { get; set; }

    public float ActualLime1 { get; set; }

    public float ActualSand1 { get; set; }

    public float ActualSand2 { get; set; }

    public float ActualWater { get; set; }

    public float TargetWater { get; set; }
}
