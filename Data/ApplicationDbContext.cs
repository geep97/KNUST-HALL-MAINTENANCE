using KnustHallMaintenance.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KnustHallMaintenance.Data;

public class ApplicationDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Student> Students
        => Set<Student>();

    public DbSet<Complaint> Complaints
        => Set<Complaint>();

    public DbSet<ComplaintUpdate> ComplaintUpdates
        => Set<ComplaintUpdate>();

    public DbSet<ComplaintPhoto> ComplaintPhotos
        => Set<ComplaintPhoto>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Student>()
            .HasIndex(s => s.StudentId)
            .IsUnique();

        modelBuilder.Entity<Student>()
            .HasIndex(s => s.Email)
            .IsUnique();

        modelBuilder.Entity<Complaint>()
            .HasOne(c => c.Student)
            .WithMany()
            .HasForeignKey(c => c.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Complaint>()
            .HasOne(c => c.AssignedTo)
            .WithMany(u => u.AssignedComplaints)
            .HasForeignKey(c => c.AssignedToId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Complaint>()
            .HasMany(c => c.Photos)
            .WithOne(p => p.Complaint)
            .HasForeignKey(p => p.ComplaintId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ComplaintUpdate>()
            .HasOne(u => u.Complaint)
            .WithMany(c => c.Updates)
            .HasForeignKey(u => u.ComplaintId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ComplaintUpdate>()
            .HasOne(u => u.UpdatedBy)
            .WithMany()
            .HasForeignKey(u => u.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Complaint>()
            .HasIndex(c => new { c.StudentId, c.DateCreated });

        modelBuilder.Entity<Complaint>()
            .HasIndex(c => new { c.AssignedToId, c.DateCreated });

        modelBuilder.Entity<Complaint>()
            .HasIndex(c => new { c.Hall, c.DateCreated });

        modelBuilder.Entity<ComplaintUpdate>()
            .HasIndex(u => new { u.ComplaintId, u.Timestamp });
    }
}