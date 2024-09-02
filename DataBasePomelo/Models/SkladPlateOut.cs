namespace DataBasePomelo.Models;

public partial class SkladPlateOut
{
    public int Id { get; set; }

    public int IdName { get; set; }

    public int IdOrder { get; set; }

    public int IdManufacturer { get; set; }

    public DateOnly DataOut { get; set; }

    public int CountOut { get; set; }
}
