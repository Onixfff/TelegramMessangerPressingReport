using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace DataBasePomelo.Models;

public partial class SilikatContext : DbContext
{
    public SilikatContext()
    {
    }

    public SilikatContext(DbContextOptions<SilikatContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccessoryPress> AccessoryPresses { get; set; }

    public virtual DbSet<NameBrick> NameBricks { get; set; }

    public virtual DbSet<Nomenklatura> Nomenklaturas { get; set; }

    public virtual DbSet<OrderPlate> OrderPlates { get; set; }

    public virtual DbSet<PlatePress> PlatePresses { get; set; }

    public virtual DbSet<Production> Productions { get; set; }

    public virtual DbSet<ReceptEntity> Recepts { get; set; }

    public virtual DbSet<ReportEntity> Reports { get; set; }

    public virtual DbSet<ReportPack> ReportPacks { get; set; }

    public virtual DbSet<ReportPress> ReportPresses { get; set; }

    public virtual DbSet<ReportPress2> ReportPress2s { get; set; }

    public virtual DbSet<SkladPlateActual> SkladPlateActuals { get; set; }

    public virtual DbSet<SkladPlateIn> SkladPlateIns { get; set; }

    public virtual DbSet<SkladPlateOut> SkladPlateOuts { get; set; }

    public virtual DbSet<Tmp> Tmps { get; set; }

    public virtual DbSet<ToolAssembly> ToolAssemblies { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=127.0.0.1;database=silikat;user=root;password=12345", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.23-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<AccessoryPress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("accessory_press", tb => tb.HasComment("Пресс-оснастака (короба, доборные короба, штампы)"))
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comments)
                .HasMaxLength(128)
                .HasColumnName("comments");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.Manufacturer).HasColumnName("manufacturer");
            entity.Property(e => e.Name)
                .HasMaxLength(128)
                .HasDefaultValueSql("'не известно'")
                .HasColumnName("name");
            entity.Property(e => e.Number)
                .HasMaxLength(15)
                .HasColumnName("number");
        });

        modelBuilder.Entity<NameBrick>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("name_brick")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NameAb)
                .HasMaxLength(128)
                .HasColumnName("name_AB");
            entity.Property(e => e.NameGost)
                .HasMaxLength(128)
                .HasColumnName("name_GOST");
            entity.Property(e => e.Size)
                .HasMaxLength(45)
                .HasColumnName("size");
        });

        modelBuilder.Entity<Nomenklatura>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("nomenklatura")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Col).HasColumnName("col");
            entity.Property(e => e.Comments)
                .HasMaxLength(128)
                .HasColumnName("comments");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<OrderPlate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("order_plate")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.HasIndex(e => e.IdManufacturer, "id_manufacturer_idx");

            entity.HasIndex(e => e.IdName, "id_name_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Col).HasColumnName("col");
            entity.Property(e => e.DateOrder).HasColumnName("date_order");
            entity.Property(e => e.DatePay).HasColumnName("date_pay");
            entity.Property(e => e.FullSklad).HasColumnName("full_sklad");
            entity.Property(e => e.IdManufacturer).HasColumnName("id_manufacturer");
            entity.Property(e => e.IdName).HasColumnName("id_name");
            entity.Property(e => e.IdOrder).HasColumnName("id_order");
            entity.Property(e => e.PartSklad).HasColumnName("part_sklad");
            entity.Property(e => e.PlannedDateReadiness)
                .HasComment("Плановая дата готовности")
                .HasColumnName("planned_date_readiness");
            entity.Property(e => e.ProductionTime)
                .HasComment("Срок изготовления")
                .HasColumnName("production_time");
            entity.Property(e => e.SkladCol).HasColumnName("sklad_col");
            entity.Property(e => e.StatusPay)
                .HasComment("Статус платежа")
                .HasColumnName("status_pay");

            entity.HasOne(d => d.IdNameNavigation).WithMany(p => p.OrderPlates)
                .HasForeignKey(d => d.IdName)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("id_name");
        });

        modelBuilder.Entity<PlatePress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("plate_press")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CodAeroblock)
                .HasMaxLength(45)
                .HasColumnName("cod_aeroblock");
            entity.Property(e => e.CodPrazi)
                .HasMaxLength(45)
                .HasColumnName("cod_prazi");
            entity.Property(e => e.CodViso)
                .HasMaxLength(45)
                .HasColumnName("cod_viso");
            entity.Property(e => e.Col).HasColumnName("col");
            entity.Property(e => e.Name)
                .HasMaxLength(128)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Production>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("production")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Count).HasColumnName("count");
            entity.Property(e => e.Month)
                .HasMaxLength(45)
                .HasColumnName("month");
            entity.Property(e => e.Years)
                .HasMaxLength(45)
                .HasColumnName("years");
        });

        modelBuilder.Entity<ReceptEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("recept")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.HasIndex(e => e.IdnameLime, "id_lime_idx");

            entity.HasIndex(e => e.IdnameSand1, "id_sand1_idx");

            entity.HasIndex(e => e.IdnameSand2, "id_sand2_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comments)
                .HasMaxLength(128)
                .HasColumnName("comments");
            entity.Property(e => e.Grade)
                .HasMaxLength(45)
                .HasColumnName("grade");
            entity.Property(e => e.IdnameLime).HasColumnName("idname_lime");
            entity.Property(e => e.IdnameSand1).HasColumnName("idname_sand1");
            entity.Property(e => e.IdnameSand2).HasColumnName("idname_sand2");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasColumnName("name");
            entity.Property(e => e.WeightLimi).HasColumnName("weight_limi");
            entity.Property(e => e.WeightSand1).HasColumnName("weight_sand1");
            entity.Property(e => e.WeigtSand2).HasColumnName("weigt_sand2");
            entity.Property(e => e.WeigtWater).HasColumnName("weigt_water");
        });

        modelBuilder.Entity<ReportEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("report")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Id)
                .HasColumnType("datetime")
                .HasColumnName("id");
            entity.Property(e => e.ActualLime1).HasColumnName("actual_lime1");
            entity.Property(e => e.ActualSand1).HasColumnName("actual_sand1");
            entity.Property(e => e.ActualSand2).HasColumnName("actual_sand2");
            entity.Property(e => e.ActualWater).HasColumnName("actual_water");
            entity.Property(e => e.IdNameLime).HasColumnName("id_name_lime");
            entity.Property(e => e.IdRecept).HasColumnName("id_recept");
            entity.Property(e => e.IdnameSand1).HasColumnName("idname_sand1");
            entity.Property(e => e.IdnameSand2).HasColumnName("idname_sand2");
            entity.Property(e => e.TargetLime).HasColumnName("target_lime");
            entity.Property(e => e.TargetSand1).HasColumnName("target_sand1");
            entity.Property(e => e.TargetSand2).HasColumnName("target_sand2");
            entity.Property(e => e.TargetWater).HasColumnName("target_water");
        });

        modelBuilder.Entity<ReportPack>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("report_pack")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("id");
            entity.Property(e => e.CountBrickInPallet).HasColumnName("count_brick_in_pallet");
            entity.Property(e => e.CountPallet).HasColumnName("count_pallet");
            entity.Property(e => e.IdProduct).HasColumnName("id_product");
        });

        modelBuilder.Entity<ReportPress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("report_press")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Id)
                .HasColumnType("datetime")
                .HasColumnName("id");
            entity.Property(e => e.ActualPress1)
                .HasDefaultValueSql("'0'")
                .HasColumnName("actual_press_1");
            entity.Property(e => e.ActualPress2)
                .HasDefaultValueSql("'0'")
                .HasColumnName("actual_press_2");
            entity.Property(e => e.ActualPress3)
                .HasDefaultValueSql("'0'")
                .HasColumnName("actual_press_3");
            entity.Property(e => e.ActualPress4)
                .HasDefaultValueSql("'0'")
                .HasColumnName("actual_press_4");
            entity.Property(e => e.ActualSize1)
                .HasDefaultValueSql("'0'")
                .HasColumnName("actual_size_1");
            entity.Property(e => e.ActualSize2)
                .HasDefaultValueSql("'0'")
                .HasColumnName("actual_size_2");
            entity.Property(e => e.ActualSize3)
                .HasDefaultValueSql("'0'")
                .HasColumnName("actual_size_3");
            entity.Property(e => e.ActualSize4)
                .HasDefaultValueSql("'0'")
                .HasColumnName("actual_size_4");
            entity.Property(e => e.IdNomenklatura).HasColumnName("id_nomenklatura");
            entity.Property(e => e.NomVagon)
                .HasDefaultValueSql("'0'")
                .HasColumnName("nom_vagon");
            entity.Property(e => e.PlaceInVagon)
                .HasDefaultValueSql("'0'")
                .HasColumnName("place_in_vagon");
        });

        modelBuilder.Entity<ReportPress2>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("report_press2")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Id)
                .HasColumnType("datetime")
                .HasColumnName("id");
            entity.Property(e => e.ActualPress1)
                .HasDefaultValueSql("'0'")
                .HasColumnName("actual_press_1");
            entity.Property(e => e.ActualPress2)
                .HasDefaultValueSql("'0'")
                .HasColumnName("actual_press_2");
            entity.Property(e => e.ActualPress3)
                .HasDefaultValueSql("'0'")
                .HasColumnName("actual_press_3");
            entity.Property(e => e.ActualPress4)
                .HasDefaultValueSql("'0'")
                .HasColumnName("actual_press_4");
            entity.Property(e => e.ActualSize1)
                .HasDefaultValueSql("'0'")
                .HasColumnName("actual_size_1");
            entity.Property(e => e.ActualSize2)
                .HasDefaultValueSql("'0'")
                .HasColumnName("actual_size_2");
            entity.Property(e => e.ActualSize3)
                .HasDefaultValueSql("'0'")
                .HasColumnName("actual_size_3");
            entity.Property(e => e.ActualSize4)
                .HasDefaultValueSql("'0'")
                .HasColumnName("actual_size_4");
            entity.Property(e => e.IdNomenklatura).HasColumnName("id_nomenklatura");
            entity.Property(e => e.NomVagon)
                .HasDefaultValueSql("'0'")
                .HasColumnName("nom_vagon");
            entity.Property(e => e.PlaceInVagon)
                .HasDefaultValueSql("'0'")
                .HasColumnName("place_in_vagon");
        });

        modelBuilder.Entity<SkladPlateActual>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("sklad_plate_actual")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CountAct).HasColumnName("count_act");
            entity.Property(e => e.DataAct).HasColumnName("data_act");
            entity.Property(e => e.IdManufacturer).HasColumnName("id_manufacturer");
            entity.Property(e => e.IdName).HasColumnName("id_name");
            entity.Property(e => e.IdOrder).HasColumnName("id_order");
        });

        modelBuilder.Entity<SkladPlateIn>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("sklad_plate_in")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CountIn).HasColumnName("count_in");
            entity.Property(e => e.DataIn).HasColumnName("data_in");
            entity.Property(e => e.IdManufacturer).HasColumnName("id_manufacturer");
            entity.Property(e => e.IdName).HasColumnName("id_name");
            entity.Property(e => e.IdOrder).HasColumnName("id_order");
        });

        modelBuilder.Entity<SkladPlateOut>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("sklad_plate_out")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CountOut).HasColumnName("count_out");
            entity.Property(e => e.DataOut).HasColumnName("data_out");
            entity.Property(e => e.IdManufacturer).HasColumnName("id_manufacturer");
            entity.Property(e => e.IdName).HasColumnName("id_name");
            entity.Property(e => e.IdOrder).HasColumnName("id_order");
        });

        modelBuilder.Entity<Tmp>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("tmp")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("id");
            entity.Property(e => e.Imp).HasColumnName("imp");
            entity.Property(e => e.PosActual).HasColumnName("pos_actual");
            entity.Property(e => e.PosCorrect).HasColumnName("pos_correct");
            entity.Property(e => e.PosNull).HasColumnName("pos_null");
            entity.Property(e => e.PosSetted).HasColumnName("pos_setted");
            entity.Property(e => e.PressActual).HasColumnName("press_actual");
            entity.Property(e => e.PressCorrect).HasColumnName("press_correct");
            entity.Property(e => e.PressFact).HasColumnName("press_fact");
            entity.Property(e => e.PressSetted).HasColumnName("press_setted");
            entity.Property(e => e.SizeActual).HasColumnName("size_actual");
            entity.Property(e => e.SizeCorrect).HasColumnName("size_correct");
            entity.Property(e => e.SizeFact).HasColumnName("size_fact");
            entity.Property(e => e.SizeForPress).HasColumnName("size_for_press");
            entity.Property(e => e.SizeRise).HasColumnName("size_rise");
            entity.Property(e => e.SizeSetted).HasColumnName("size_setted");
        });

        modelBuilder.Entity<ToolAssembly>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("tool_assembly")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.HasIndex(e => e.IdSklad, "id_sklad_idx");

            entity.HasIndex(e => e.IdTool, "id_tool_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Count).HasColumnName("count");
            entity.Property(e => e.DateAssembly)
                .HasColumnType("datetime")
                .HasColumnName("date_assembly");
            entity.Property(e => e.DateOverturn)
                .HasColumnType("datetime")
                .HasColumnName("date_overturn");
            entity.Property(e => e.DateScrap)
                .HasColumnType("datetime")
                .HasColumnName("date_scrap");
            entity.Property(e => e.IdAccessory).HasColumnName("id_accessory");
            entity.Property(e => e.IdSklad).HasColumnName("id_sklad");
            entity.Property(e => e.IdTool).HasColumnName("id_tool");
            entity.Property(e => e.Overturn).HasColumnName("overturn");
            entity.Property(e => e.Scrap).HasColumnName("scrap");
            entity.Property(e => e._1Assembly).HasColumnName("1_assembly");

            entity.HasOne(d => d.IdSkladNavigation).WithMany(p => p.ToolAssemblies)
                .HasForeignKey(d => d.IdSklad)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("id_sklad");

            entity.HasOne(d => d.IdToolNavigation).WithMany(p => p.ToolAssemblies)
                .HasForeignKey(d => d.IdTool)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("id_tool");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
