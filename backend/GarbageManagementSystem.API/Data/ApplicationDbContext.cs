using GarbageManagementSystem.API.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GarbageManagementSystem.API.Data;

/// <summary>
/// EF Core database context. Inherits from IdentityDbContext so the ASP.NET Identity
/// tables (AspNetUsers, AspNetRoles, etc.) are created alongside our own domain tables.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Resident> Residents => Set<Resident>();
    public DbSet<WasteCategory> WasteCategories => Set<WasteCategory>();
    public DbSet<CollectionRequest> CollectionRequests => Set<CollectionRequest>();
    public DbSet<CollectionSchedule> CollectionSchedules => Set<CollectionSchedule>();
    public DbSet<Complaint> Complaints => Set<Complaint>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Must run first - configures all the AspNetXxx Identity tables.
        base.OnModelCreating(builder);

        // ---------- ApplicationUser <-> Resident (one-to-one) ----------
        builder.Entity<Resident>()
            .HasOne(r => r.ApplicationUser)
            .WithOne(u => u.Resident)
            .HasForeignKey<Resident>(r => r.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Resident>()
            .HasIndex(r => r.ApplicationUserId)
            .IsUnique();

        // ---------- Resident <-> CollectionRequest (one-to-many) ----------
        builder.Entity<CollectionRequest>()
            .HasOne(cr => cr.Resident)
            .WithMany(r => r.CollectionRequests)
            .HasForeignKey(cr => cr.ResidentId)
            .OnDelete(DeleteBehavior.Cascade);

        // ---------- WasteCategory <-> CollectionRequest (one-to-many, required) ----------
        builder.Entity<CollectionRequest>()
            .HasOne(cr => cr.WasteCategory)
            .WithMany(wc => wc.CollectionRequests)
            .HasForeignKey(cr => cr.WasteCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---------- Resident <-> Complaint (one-to-many) ----------
        builder.Entity<Complaint>()
            .HasOne(c => c.Resident)
            .WithMany(r => r.Complaints)
            .HasForeignKey(c => c.ResidentId)
            .OnDelete(DeleteBehavior.Cascade);

        // ---------- WasteCategory <-> CollectionSchedule (one-to-many, optional) ----------
        builder.Entity<CollectionSchedule>()
            .HasOne(cs => cs.WasteCategory)
            .WithMany(wc => wc.CollectionSchedules)
            .HasForeignKey(cs => cs.WasteCategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // ---------- Useful indexes ----------
        builder.Entity<WasteCategory>()
            .HasIndex(wc => wc.Name)
            .IsUnique();

        builder.Entity<CollectionRequest>()
            .HasIndex(cr => cr.Status);

        builder.Entity<CollectionSchedule>()
            .HasIndex(cs => cs.Zone);

        builder.Entity<Complaint>()
            .HasIndex(c => c.Status);

        // ---------- Precision / required lengths ----------
        builder.Entity<ApplicationUser>().Property(u => u.FullName).HasMaxLength(150);
        builder.Entity<Resident>().Property(r => r.Zone).HasMaxLength(100);
        builder.Entity<WasteCategory>().Property(wc => wc.Name).HasMaxLength(100);
        builder.Entity<Complaint>().Property(c => c.Subject).HasMaxLength(200);
    }
}
