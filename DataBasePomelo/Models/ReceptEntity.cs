namespace DataBasePomelo.Models;

public partial class ReceptEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? IdnameLime { get; set; }

    public int? IdnameSand1 { get; set; }

    public int? IdnameSand2 { get; set; }

    public float? WeightLimi { get; set; }

    public float? WeightSand1 { get; set; }

    public float? WeigtSand2 { get; set; }

    public float? WeigtWater { get; set; }

    public string Grade { get; set; } = null!;

    public string? Comments { get; set; }
}
