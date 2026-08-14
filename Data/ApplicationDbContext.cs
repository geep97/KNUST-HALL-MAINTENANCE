using KnustHallMaintenance.Models;
using Microsoft.EntityFrameworkCore;

namespace KnustHallMaintenance.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    public DbSet<Complaint> Complaints => Set<Complaint>();
    public DbSet<ComplaintUpdate> ComplaintUpdates => Set<ComplaintUpdate>();

    public DbSet<Student> Students { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUser>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Complaint>()
            .HasOne(c => c.Student)
            .WithMany(u => u.Complaints)
            .HasForeignKey(c => c.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Complaint>()
            .HasOne(c => c.AssignedTo)
            .WithMany()
            .HasForeignKey(c => c.AssignedToId)
            .OnDelete(DeleteBehavior.Restrict);

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
    }
}
