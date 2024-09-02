namespace DataBasePomelo.Models;

public partial class ToolAssembly
{
    public int Id { get; set; }

    public DateTime? DateAssembly { get; set; }

    public int IdAccessory { get; set; }

    public int IdTool { get; set; }

    public int IdSklad { get; set; }

    public int Count { get; set; }

    public bool _1Assembly { get; set; }

    public bool Overturn { get; set; }

    public DateTime? DateOverturn { get; set; }

    public bool Scrap { get; set; }

    public DateTime? DateScrap { get; set; }

    public virtual SkladPlateActual IdSkladNavigation { get; set; } = null!;

    public virtual AccessoryPress IdToolNavigation { get; set; } = null!;
}
