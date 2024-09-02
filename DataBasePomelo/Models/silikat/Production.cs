namespace DataBasePomelo.Models.silikat;

public partial class Production
{
    public int Id { get; set; }

    public string Month { get; set; } = null!;

    public string Years { get; set; } = null!;

    public float Count { get; set; }
}
