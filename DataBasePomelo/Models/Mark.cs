using System;
using System.Collections.Generic;

namespace DataBasePomelo.Models;

public partial class Mark
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Comments { get; set; }
}
