using KnustHallMaintenance.Data;
using KnustHallMaintenance.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KnustHallMaintenance.Services;

public class StudentService : IStudentService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<Student> _passwordHasher;

    public StudentService(
        ApplicationDbContext context,
        IPasswordHasher<Student> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<Student?> GetByStudentIdAsync(
        string studentId)
    {
        return await _context.Students
            .FirstOrDefaultAsync(
                s => s.StudentId == studentId
            );
    }

    public bool VerifyPassword(
        Student student,
        string password)
    {
        var result =
            _passwordHasher.VerifyHashedPassword(
                student,
                student.PasswordHash,
                password
            );

        return result == PasswordVerificationResult.Success ||
               result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}