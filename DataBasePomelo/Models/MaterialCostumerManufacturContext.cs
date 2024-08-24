using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace DataBasePomelo.Models;

public partial class MaterialCostumerManufacturContext : DbContext
{
    public MaterialCostumerManufacturContext()
    {
    }

    public MaterialCostumerManufacturContext(DbContextOptions<MaterialCostumerManufacturContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<GroupMaterial> GroupMaterials { get; set; }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<Mark> Marks { get; set; }

    public virtual DbSet<MaterialEntity> Materials { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=127.0.0.1;database=material_costumer_manufactur;user=root;password=12345", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.23-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("class")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comments)
                .HasMaxLength(45)
                .HasColumnName("comments");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<GroupMaterial>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("group_material", tb => tb.HasComment("Групы материало(добавки. цемент, известь и т.п)"))
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comments)
                .HasMaxLength(45)
                .HasColumnName("comments");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("manufacturer", tb => tb.HasComment("Производители"))
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comments)
                .HasMaxLength(64)
                .HasColumnName("comments");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Mark>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("mark")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comments)
                .HasMaxLength(45)
                .HasColumnName("comments");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<MaterialEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("material")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.HasIndex(e => e.IdGroup, "id_group_idx");

            entity.HasIndex(e => e.IdManifactur, "id_manufactur_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comments)
                .HasMaxLength(45)
                .HasColumnName("comments");
            entity.Property(e => e.IdGroup).HasColumnName("id_group");
            entity.Property(e => e.IdManifactur).HasColumnName("id_manifactur");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
            entity.Property(e => e.Shifr)
                .HasMaxLength(45)
                .HasColumnName("shifr");

            entity.HasOne(d => d.IdGroupNavigation).WithMany(p => p.Materials)
                .HasForeignKey(d => d.IdGroup)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("id_group");

            entity.HasOne(d => d.IdManifacturNavigation).WithMany(p => p.Materials)
                .HasForeignKey(d => d.IdManifactur)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("id_manufactur");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("units", tb => tb.HasComment("Единицы измерения"))
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
