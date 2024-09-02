namespace DataBasePomelo.Models.silikat;

/// <summary>
/// Пресс-оснастака (короба, доборные короба, штампы)
/// </summary>
public partial class AccessoryPress
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Manufacturer { get; set; }

    public DateOnly? Data { get; set; }

    public string? Number { get; set; }

    public string? Comments { get; set; }

    public virtual ICollection<ToolAssembly> ToolAssemblies { get; set; } = new List<ToolAssembly>();
}
