namespace DataBasePomelo.Models.silikat;

public partial class Nomenklatura
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? Col { get; set; } = 0!;

    public string? Comments { get; set; }
}
