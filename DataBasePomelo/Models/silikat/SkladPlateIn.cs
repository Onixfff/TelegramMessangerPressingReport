namespace DataBasePomelo.Models.silikat;

public partial class SkladPlateIn
{
    public int Id { get; set; }

    public int IdName { get; set; }

    public int IdOrder { get; set; }

    public int IdManufacturer { get; set; }

    public DateOnly DataIn { get; set; }

    public int CountIn { get; set; }
}
