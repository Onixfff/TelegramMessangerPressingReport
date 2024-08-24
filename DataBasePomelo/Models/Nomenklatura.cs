using System;
using System.Collections.Generic;

namespace DataBasePomelo.Models;

public partial class Nomenklatura
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? Col { get; set; }

    public string? Comments { get; set; }
}
