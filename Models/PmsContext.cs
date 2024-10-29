using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GCBPMS.Models;

public partial class PmsContext : DbContext
{
    public PmsContext()
    {
    }

    public PmsContext(DbContextOptions<PmsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Phase> Phases { get; set; }

    public virtual DbSet<Plate> Plates { get; set; }

    public virtual DbSet<PlateHistoryUsage> PlateHistoryUsages { get; set; }

    public virtual DbSet<Pot> Pots { get; set; }

    public virtual DbSet<Press> Presses { get; set; }

    public virtual DbSet<Repair> Repairs { get; set; }

    public virtual DbSet<RepairCost> RepairCosts { get; set; }

    public virtual DbSet<SupplierDetail> SupplierDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Brands__3214EC078F50502C");

            entity.Property(e => e.BrandName).HasMaxLength(50);
        });

        modelBuilder.Entity<Phase>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Phases__3214EC07DAA6F8B4");

            entity.Property(e => e.PhaseName).HasMaxLength(255);
        });

        modelBuilder.Entity<Plate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Plates__3214EC07AEF0E39C");

            entity.Property(e => e.PlateInstallDatetime).HasColumnType("datetime");
            entity.Property(e => e.PlateName).HasMaxLength(100);
            entity.Property(e => e.PlateStatus).HasMaxLength(50);

            entity.HasOne(d => d.PhaseTypeNavigation).WithMany(p => p.Plates)
                .HasForeignKey(d => d.PhaseType)
                .HasConstraintName("FK__Plates__PhaseTyp__3F466844");

            entity.HasOne(d => d.PlateBrandNavigation).WithMany(p => p.Plates)
                .HasForeignKey(d => d.PlateBrand)
                .HasConstraintName("FK__Plates__PlateBra__3E52440B");
        });

        modelBuilder.Entity<PlateHistoryUsage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PlateHis__3214EC07122F96CA");

            entity.ToTable("PlateHistoryUsage");

            entity.Property(e => e.InstallDateTime).HasColumnType("datetime");
            entity.Property(e => e.RemoveDateTime).HasColumnType("datetime");

            entity.HasOne(d => d.Plate).WithMany(p => p.PlateHistoryUsages)
                .HasForeignKey(d => d.PlateId)
                .HasConstraintName("FK__PlateHist__Plate__45F365D3");

            entity.HasOne(d => d.Press).WithMany(p => p.PlateHistoryUsages)
                .HasForeignKey(d => d.PressId)
                .HasConstraintName("FK__PlateHist__Press__46E78A0C");
        });

        modelBuilder.Entity<Pot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Pots__3214EC07D7C1E33D");

            entity.Property(e => e.PotName).HasMaxLength(100);

            entity.HasOne(d => d.Plate).WithMany(p => p.Pots)
                .HasForeignKey(d => d.PlateId)
                .HasConstraintName("FK__Pots__PlateId__4222D4EF");

            entity.HasOne(d => d.Press).WithMany(p => p.Pots)
                .HasForeignKey(d => d.PressId)
                .HasConstraintName("FK__Pots__PressId__4316F928");
        });

        modelBuilder.Entity<Press>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Presses__3214EC07210EC634");

            entity.Property(e => e.PressName).HasMaxLength(50);

            entity.HasOne(d => d.Phase).WithMany(p => p.Presses)
                .HasForeignKey(d => d.PhaseId)
                .HasConstraintName("FK__Presses__PhaseId__398D8EEE");
        });

        modelBuilder.Entity<Repair>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Repairs__3214EC07B34002B8");

            entity.Property(e => e.FinishDatetime).HasColumnType("datetime");
            entity.Property(e => e.RepairStatus).HasMaxLength(50);
            entity.Property(e => e.RepairType).HasMaxLength(20);
            entity.Property(e => e.RequestDatetime).HasColumnType("datetime");
            entity.Property(e => e.TechnicianName).HasMaxLength(100);

            entity.HasOne(d => d.Plate).WithMany(p => p.Repairs)
                .HasForeignKey(d => d.PlateId)
                .HasConstraintName("FK__Repairs__PlateId__4BAC3F29");

            entity.HasOne(d => d.SupplierDetails).WithMany(p => p.Repairs)
                .HasForeignKey(d => d.SupplierDetailsId)
                .HasConstraintName("FK__Repairs__Supplie__4CA06362");
        });

        modelBuilder.Entity<RepairCost>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RepairCo__3214EC07C2D23036");

            entity.Property(e => e.Cost).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CostName).HasMaxLength(100);
            entity.Property(e => e.CostRemark).HasMaxLength(255);

            entity.HasOne(d => d.Repair).WithMany(p => p.RepairCosts)
                .HasForeignKey(d => d.RepairId)
                .HasConstraintName("FK__RepairCos__Repai__59FA5E80");
        });

        modelBuilder.Entity<SupplierDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Supplier__3214EC076A014891");

            entity.Property(e => e.Eta)
                .HasColumnType("datetime")
                .HasColumnName("ETA");
            entity.Property(e => e.SupplierName).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
