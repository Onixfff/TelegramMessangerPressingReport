using System;
using System.Collections.Generic;

namespace DataBasePomelo.Models;

/// <summary>
/// Единицы измерения
/// </summary>
public partial class Unit
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
}
