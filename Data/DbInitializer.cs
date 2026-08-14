using KnustHallMaintenance.Models;
using Microsoft.AspNetCore.Identity;

namespace KnustHallMaintenance.Data;

public static class DbInitializer
{
    public static void Seed(
        ApplicationDbContext context,
        IPasswordHasher<ApplicationUser> hasher)
    {
        

        if (!context.Users.Any(u => u.Role == UserRole.Admin))
        {
            var admin = new ApplicationUser
            {
                FullName = "System Administrator",
                Email = "admin@knust.edu.gh",
                Role = UserRole.Admin
            };

            admin.PasswordHash =
                hasher.HashPassword(admin, "Admin123!");

            context.Users.Add(admin);

            context.SaveChanges();
        }


        
        if (context.Users.Any(u => u.Role == UserRole.Student))
        {
            return;
        }


        var student = new ApplicationUser
        {
            FullName = "Kwame Mensah",
            Email = "student@knust.edu.gh",
            Role = UserRole.Student,
            Hall = "Unity Hall",
            RoomNumber = "B12"
        };

        student.PasswordHash =
            hasher.HashPassword(student, "Password123!");


        var staff = new ApplicationUser
        {
            FullName = "Hall Assistant - Unity Hall",
            Email = "staff@knust.edu.gh",
            Role = UserRole.Staff
        };

        staff.PasswordHash =
            hasher.HashPassword(staff, "Password123!");


        var technician = new ApplicationUser
        {
            FullName = "John Boateng (Electrician)",
            Email = "maintenance@knust.edu.gh",
            Role = UserRole.Maintenance
        };

        technician.PasswordHash =
            hasher.HashPassword(technician, "Password123!");


        context.Users.AddRange(
            student,
            staff,
            technician
        );

        context.SaveChanges();


        

        var complaint = new Complaint
        {
            StudentId = student.Id,

            Category = ComplaintCategory.Electrical,

            Description =
                "No light in the room since Monday. Suspect a tripped breaker or a faulty socket near the reading desk.",

            Hall = student.Hall!,

            RoomNumber = student.RoomNumber!,

            Status = ComplaintStatus.Submitted,

            DateCreated = DateTime.Now.AddDays(-2)
        };


        context.Complaints.Add(complaint);

        context.SaveChanges();
    }
}