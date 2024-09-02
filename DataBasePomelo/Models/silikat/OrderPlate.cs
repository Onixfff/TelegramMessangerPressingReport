namespace DataBasePomelo.Models.silikat;

public partial class OrderPlate
{
    public int Id { get; set; }

    public int IdName { get; set; }

    public int IdOrder { get; set; }

    public DateOnly DateOrder { get; set; }

    public int IdManufacturer { get; set; }

    public int? Col { get; set; }

    /// <summary>
    /// Срок изготовления
    /// </summary>
    public int? ProductionTime { get; set; }

    /// <summary>
    /// Плановая дата готовности
    /// </summary>
    public DateOnly? PlannedDateReadiness { get; set; }

    public DateOnly? DatePay { get; set; }

    /// <summary>
    /// Статус платежа
    /// </summary>
    public bool? StatusPay { get; set; }

    public bool? PartSklad { get; set; }

    public bool? FullSklad { get; set; }

    public int? SkladCol { get; set; }

    public virtual PlatePress IdNameNavigation { get; set; } = null!;
}
