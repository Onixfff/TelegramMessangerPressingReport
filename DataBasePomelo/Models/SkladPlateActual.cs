using System;
using System.Collections.Generic;

namespace DataBasePomelo.Models;

public partial class SkladPlateActual
{
    public int Id { get; set; }

    public int IdName { get; set; }

    public int IdOrder { get; set; }

    public int IdManufacturer { get; set; }

    public DateOnly DataAct { get; set; }

    public int CountAct { get; set; }

    public virtual ICollection<ToolAssembly> ToolAssemblies { get; set; } = new List<ToolAssembly>();
}
