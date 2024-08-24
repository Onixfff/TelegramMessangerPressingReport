using System;
using System.Collections.Generic;

namespace DataBasePomelo.Models;

/// <summary>
/// Групы материало(добавки. цемент, известь и т.п)
/// </summary>
public partial class GroupMaterial
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Comments { get; set; }

    public virtual ICollection<MaterialEntity> Materials { get; set; } = new List<MaterialEntity>();
}
