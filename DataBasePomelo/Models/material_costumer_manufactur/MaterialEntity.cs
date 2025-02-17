﻿namespace DataBasePomelo.Models.material_costumer_manufactur;

public partial class MaterialEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Shifr { get; set; } = null!;

    public int IdGroup { get; set; }

    public int IdManifactur { get; set; }

    public string? Comments { get; set; }

    public virtual GroupMaterialEntity IdGroupNavigation { get; set; } = null!;

    public virtual ManufacturerEntity IdManifacturNavigation { get; set; } = null!;
}
