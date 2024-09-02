namespace DataBasePomelo.Models.silikat;

public partial class PlatePress
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? CodViso { get; set; }

    public string? CodPrazi { get; set; }

    public string? CodAeroblock { get; set; }

    public int? Col { get; set; }

    public virtual ICollection<OrderPlate> OrderPlates { get; set; } = new List<OrderPlate>();
}
