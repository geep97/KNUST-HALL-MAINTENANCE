using KnustHallMaintenance.Models;
using Microsoft.EntityFrameworkCore;

namespace KnustHallMaintenance.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }


    // =========================================================
    // TABLES
    // =========================================================

    public DbSet<ApplicationUser> Users
        => Set<ApplicationUser>();

    public DbSet<Student> Students
        => Set<Student>();

    public DbSet<Complaint> Complaints
        => Set<Complaint>();

    public DbSet<ComplaintUpdate> ComplaintUpdates
        => Set<ComplaintUpdate>();


    // =========================================================
    // MODEL CONFIGURATION
    // =========================================================

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        // =====================================================
        // APPLICATION USERS
        // =====================================================

        modelBuilder.Entity<ApplicationUser>()
            .HasIndex(u => u.Email)
            .IsUnique();


        // =====================================================
        // STUDENTS
        // =====================================================

        modelBuilder.Entity<Student>()
            .HasIndex(s => s.StudentId)
            .IsUnique();

        modelBuilder.Entity<Student>()
            .HasIndex(s => s.Email)
            .IsUnique();


        // =====================================================
        // COMPLAINT -> STUDENT
        // =====================================================

        modelBuilder.Entity<Complaint>()
            .HasOne(c => c.Student)
            .WithMany()
            .HasForeignKey(c => c.StudentId)
            .OnDelete(DeleteBehavior.Restrict);


        // =====================================================
        // COMPLAINT -> ASSIGNED USER
        // =====================================================

        modelBuilder.Entity<Complaint>()
            .HasOne(c => c.AssignedTo)
            .WithMany(u => u.AssignedComplaints)
            .HasForeignKey(c => c.AssignedToId)
            .OnDelete(DeleteBehavior.Restrict);


        // =====================================================
        // COMPLAINT UPDATE -> COMPLAINT
        // =====================================================

        modelBuilder.Entity<ComplaintUpdate>()
            .HasOne(u => u.Complaint)
            .WithMany(c => c.Updates)
            .HasForeignKey(u => u.ComplaintId)
            .OnDelete(DeleteBehavior.Cascade);


        // =====================================================
        // COMPLAINT UPDATE -> USER
        // =====================================================

        modelBuilder.Entity<ComplaintUpdate>()
            .HasOne(u => u.UpdatedBy)
            .WithMany()
            .HasForeignKey(u => u.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}