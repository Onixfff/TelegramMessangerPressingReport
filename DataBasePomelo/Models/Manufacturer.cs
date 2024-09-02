namespace DataBasePomelo.Models;

/// <summary>
/// Производители
/// </summary>
public partial class Manufacturer
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Comments { get; set; }

    public virtual ICollection<MaterialEntity> Materials { get; set; } = new List<MaterialEntity>();
}
