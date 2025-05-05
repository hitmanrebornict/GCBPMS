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

    public virtual DbSet<ActionCodeDetail> ActionCodeDetails { get; set; }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Phase> Phases { get; set; }

    public virtual DbSet<Plate> Plates { get; set; }

    public virtual DbSet<PlateHistoryUsage> PlateHistoryUsages { get; set; }

    public virtual DbSet<Pot> Pots { get; set; }

    public virtual DbSet<Press> Presses { get; set; }

    public virtual DbSet<Repair> Repairs { get; set; }

    public virtual DbSet<RepairCost> RepairCosts { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<SupplierDetail> SupplierDetails { get; set; }

    public virtual DbSet<UserAction> UserActions { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActionCodeDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ActionCo__3214EC2768D71FFB");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.FullCode).HasMaxLength(50);
        });

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.UserName, "UQ_AspNetUsers_UserName").IsUnique();

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.ProviderKey).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

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

            entity.Property(e => e.ChangeReason)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.InstallDateTime).HasColumnType("datetime");
            entity.Property(e => e.RemoveDateTime).HasColumnType("datetime");

            entity.HasOne(d => d.Plate).WithMany(p => p.PlateHistoryUsages)
                .HasForeignKey(d => d.PlateId)
                .HasConstraintName("FK__PlateHist__Plate__45F365D3");

            entity.HasOne(d => d.Pot).WithMany(p => p.PlateHistoryUsages)
                .HasForeignKey(d => d.PotId)
                .HasConstraintName("FK_PlateHistoryUsagePots");
        });

        modelBuilder.Entity<Pot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Pots__3214EC07D7C1E33D");

            entity.Property(e => e.InstallDatetime).HasColumnType("datetime");
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

            entity.Property(e => e.AcceptedBy).HasMaxLength(256);
            entity.Property(e => e.CompletedBy).HasMaxLength(256);
            entity.Property(e => e.FinishDatetime).HasColumnType("datetime");
            entity.Property(e => e.RepairRemark).HasMaxLength(255);
            entity.Property(e => e.RepairStatus).HasMaxLength(50);
            entity.Property(e => e.RepairType).HasMaxLength(20);
            entity.Property(e => e.StartDatetime).HasColumnType("datetime");
            entity.Property(e => e.TechnicianName).HasMaxLength(100);

            entity.HasOne(d => d.Request).WithMany(p => p.Repairs)
                .HasForeignKey(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Repairs_Requests");

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

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Requests__3214EC07001E1CAE");

            entity.Property(e => e.RepairReason)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RepairRemark)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RequestDatetime).HasColumnType("datetime");
            entity.Property(e => e.RequestStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Requested");
            entity.Property(e => e.Requestor).HasMaxLength(256);

            entity.HasOne(d => d.PlateHistoryUsage).WithMany(p => p.Requests)
                .HasForeignKey(d => d.PlateHistoryUsageId)
                .HasConstraintName("FK_Requests_PlateHistoryUsage");

            entity.HasOne(d => d.Plate).WithMany(p => p.Requests)
                .HasForeignKey(d => d.PlateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Request_Plate");

            entity.HasOne(d => d.RequestorNavigation).WithMany(p => p.Requests)
                .HasPrincipalKey(p => p.UserName)
                .HasForeignKey(d => d.Requestor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Requests_AspNetUsers");
        });

        modelBuilder.Entity<SupplierDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Supplier__3214EC076A014891");

            entity.Property(e => e.Eta)
                .HasColumnType("datetime")
                .HasColumnName("ETA");
            entity.Property(e => e.SupplierName).HasMaxLength(100);
        });

        modelBuilder.Entity<UserAction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserActi__3214EC273FB4B6C6");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ActionDatetime).HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(256);

            entity.HasOne(d => d.ActionNavigation).WithMany(p => p.UserActions)
                .HasForeignKey(d => d.Action)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserActio__Actio__1AD3FDA4");

            entity.HasOne(d => d.Plate).WithMany(p => p.UserActions)
                .HasForeignKey(d => d.PlateId)
                .HasConstraintName("FK__UserActio__Plate__17036CC0");

            entity.HasOne(d => d.UsernameNavigation).WithMany(p => p.UserActions)
                .HasPrincipalKey(p => p.UserName)
                .HasForeignKey(d => d.Username)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserActions_AspNetUsers");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
